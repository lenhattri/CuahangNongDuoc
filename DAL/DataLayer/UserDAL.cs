// DAL/DataLayer/UserDAL.cs
using CuahangNongduoc.DAL.Interfaces;
using BCrypt.Net;                                        // CHANGED: BCrypt.Net-Next (nuget)
using CuahangNongduoc.DAL.Infrastructure;                 // CHANGED: dùng DbClient
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc.DAL.DataLayer
{
    public class UserDAL : IUserDAL
    {
        private readonly DbClient _db = DbClient.Instance; // CHANGED: thay vì ConfigurationManager + SqlConnection
        private DataTable _dataTable;                      // giữ bảng đang bind/sửa

        // NEW: schema DataTable hợp nhất 2 bảng để NewRow/Add/Save giữ được cột
        private void EnsureSchema()
        {
            if (_dataTable != null) return;
            _dataTable = new DataTable("USERS_VIEW");
            _dataTable.Columns.Add("ID", typeof(long));
            _dataTable.Columns.Add("HO_TEN", typeof(string));
            _dataTable.Columns.Add("DIA_CHI", typeof(string));
            _dataTable.Columns.Add("DIEN_THOAI", typeof(string));
            _dataTable.Columns.Add("TEN_DANG_NHAP", typeof(string));
            _dataTable.Columns.Add("MAT_KHAU", typeof(string));
            _dataTable.Columns.Add("QUYEN", typeof(string));
        }

        /* ========================= SELECTs ========================= */

        public DataTable LayDanhSachNguoiDung()
        {
            const string sql = @"
                SELECT  NV.ID,
                        NV.HO_TEN,
                        NV.DIA_CHI,
                        NV.DIEN_THOAI,
                        ND.TEN_DANG_NHAP,
                        ND.MAT_KHAU,
                        ND.QUYEN
                FROM NHAN_VIEN NV
                JOIN NGUOI_DUNG ND ON NV.ID = ND.ID";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text);          // CHANGED
            _dataTable = dt;                                               // đồng bộ để NewRow/Add/Save dùng chung
            return dt;
        }

        public DataRow LayNguoiDungTheoId(string id)
        {
            const string sql = @"SELECT ID, TEN_DANG_NHAP, QUYEN FROM NGUOI_DUNG WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.BigInt, Convert.ToInt64(id)));      // CHANGED: ID kiểu long
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataRow LayNguoiDungTheoTenDangNhap(string tenDangNhap)
        {
            const string sql = @"
                SELECT  NV.ID, NV.HO_TEN, NV.DIA_CHI, NV.DIEN_THOAI,
                        ND.TEN_DANG_NHAP, ND.MAT_KHAU, ND.QUYEN
                FROM NGUOI_DUNG ND
                JOIN NHAN_VIEN NV ON ND.ID = NV.ID
                WHERE ND.TEN_DANG_NHAP = @user";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@user", SqlDbType.NVarChar, tenDangNhap, 100));     // CHANGED: NVARCHAR và chiều dài rõ ràng
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /* ========================= DataTable pattern ========================= */

        public DataRow NewRow()
        {
            EnsureSchema();                                                // CHANGED
            return _dataTable.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();                                                // CHANGED
            if (!ReferenceEquals(row.Table, _dataTable))
            {
                var clone = _dataTable.NewRow();
                foreach (DataColumn c in _dataTable.Columns)
                    clone[c.ColumnName] = row.Table.Columns.Contains(c.ColumnName) ? row[c.ColumnName] : DBNull.Value;
                _dataTable.Rows.Add(clone);
            }
            else
            {
                _dataTable.Rows.Add(row);
            }
        }

        /* ========================= SAVE (insert batch) ========================= */

        public bool Save()
        {
            EnsureSchema();
            if (_dataTable.Rows.Count == 0) return false;

            bool allRowsValid = true;

            try
            {
                // Duyệt qua tất cả các dòng và kiểm tra
                foreach (DataRow row in _dataTable.Rows)
                {
                    // Chỉ kiểm tra các hàng đã thay đổi
                    if (row.RowState == DataRowState.Unchanged) continue;

                    // 1. Kiểm tra các quy tắc chung (Họ tên, Tên ĐN, Quyền)
                    if (ValidationRule.ValidateRow(row, _userRules) == false)
                    {
                        allRowsValid = false;
                        break;
                    }

                    // 2. <<< THÊM LẠI: Kiểm tra quy tắc đặc biệt (Mật khẩu) >>>
                    if (row.RowState == DataRowState.Added)
                    {
                        var passValue = row["MAT_KHAU"];
                        if (passValue == DBNull.Value || string.IsNullOrWhiteSpace(passValue.ToString()))
                        {
                            // Đây là lỗi bị thiếu mà code của bạn không bắt được
                            throw new Exception("Mật khẩu là bắt buộc khi tạo người dùng mới.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi validation và dừng lại
                MessageBox.Show(ex.Message, "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!allRowsValid)
            {
                return false;
            }

            return _db.InTx((cn, tx) =>
            {
                int affected = 0;

                foreach (DataRow row in _dataTable.Rows)
                {
                    if (row.RowState != DataRowState.Added &&
                        row.RowState != DataRowState.Modified) continue;

                    // 1) Insert/Update NHAN_VIEN
                    long idNhanVien;

                    if (row.RowState == DataRowState.Added || row.IsNull("ID") || Convert.ToInt64(row["ID"]) <= 0)
                    {
                        // INSERT NV -> lấy ID
                        const string sqlNvIns = @"
                            INSERT INTO NHAN_VIEN (HO_TEN, DIA_CHI, DIEN_THOAI)
                            OUTPUT INSERTED.ID
                            VALUES (@HOTEN, @DIACHI, @DT)";
                        using (var cmd = _db.Cmd(cn, sqlNvIns, CommandType.Text, tx, 30,
                            _db.P("@HOTEN", SqlDbType.NVarChar, row["HO_TEN"], 200),
                            _db.P("@DIACHI", SqlDbType.NVarChar, row["DIA_CHI"], 255),
                            _db.P("@DT", SqlDbType.NVarChar, row["DIEN_THOAI"], 50)))
                        {
                            var obj = cmd.ExecuteScalar();
                            idNhanVien = (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
                            row["ID"] = idNhanVien;                         // cập nhật lại vào DataRow
                        }

                        // 2) Insert NGUOI_DUNG
                        const string sqlNdIns = @"
                            INSERT INTO NGUOI_DUNG (ID, TEN_DANG_NHAP, MAT_KHAU, QUYEN)
                            VALUES (@ID, @USER, @PASS, @ROLE)";
                        // Hash mật khẩu trước khi lưu
                        var hashed = BCrypt.Net.BCrypt.HashPassword(Convert.ToString(row["MAT_KHAU"]));
                        using (var cmd = _db.Cmd(cn, sqlNdIns, CommandType.Text, tx, 30,
                            _db.P("@ID", SqlDbType.BigInt, idNhanVien),
                            _db.P("@USER", SqlDbType.NVarChar, row["TEN_DANG_NHAP"], 100),
                            _db.P("@PASS", SqlDbType.NVarChar, hashed, 200),
                            _db.P("@ROLE", SqlDbType.NVarChar, row["QUYEN"], 50)))
                        {
                            affected += cmd.ExecuteNonQuery();
                        }
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        idNhanVien = Convert.ToInt64(row["ID"]);

                        // UPDATE NHAN_VIEN
                        const string sqlNvUpd = @"
                            UPDATE NHAN_VIEN
                               SET HO_TEN=@HOTEN, DIA_CHI=@DIACHI, DIEN_THOAI=@DT
                             WHERE ID=@ID";
                        using (var cmd = _db.Cmd(cn, sqlNvUpd, CommandType.Text, tx, 30,
                            _db.P("@HOTEN", SqlDbType.NVarChar, row["HO_TEN"], 200),
                            _db.P("@DIACHI", SqlDbType.NVarChar, row["DIA_CHI"], 255),
                            _db.P("@DT", SqlDbType.NVarChar, row["DIEN_THOAI"], 50),
                            _db.P("@ID", SqlDbType.BigInt, idNhanVien)))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // Kiểm tra mật khẩu có đổi không
                        var orig = row["MAT_KHAU", DataRowVersion.Original];
                        var curr = row["MAT_KHAU", DataRowVersion.Current];
                        bool passChanged = !Equals(orig, curr);

                        string sqlNdUpd = passChanged
                            ? @"UPDATE NGUOI_DUNG
                                   SET TEN_DANG_NHAP=@USER, MAT_KHAU=@PASS, QUYEN=@ROLE
                                 WHERE ID=@ID"
                            : @"UPDATE NGUOI_DUNG
                                   SET TEN_DANG_NHAP=@USER, QUYEN=@ROLE
                                 WHERE ID=@ID";

                        using (var cmd = _db.Cmd(cn, sqlNdUpd, CommandType.Text, tx, 30,
                            _db.P("@USER", SqlDbType.NVarChar, row["TEN_DANG_NHAP"], 100),
                            _db.P("@ROLE", SqlDbType.NVarChar, row["QUYEN"], 50),
                            _db.P("@ID", SqlDbType.BigInt, idNhanVien),
                            // @PASS chỉ thêm khi đổi mật khẩu
                            passChanged
                                ? _db.P("@PASS", SqlDbType.NVarChar, BCrypt.Net.BCrypt.HashPassword(Convert.ToString(curr)), 200)
                                : _db.P("@PASS", SqlDbType.NVarChar, DBNull.Value, 200)))
                        {
                            if (!passChanged)
                                cmd.Parameters.RemoveAt("@PASS"); // bỏ param thừa nếu không dùng
                            affected += cmd.ExecuteNonQuery();
                        }
                    }
                }

                _dataTable.AcceptChanges();
                return affected;
            }) > 0;
        }

        /* ========================= UPDATE (by ID) ========================= */
        // Nếu bạn vẫn muốn API Update riêng (song song Save), mình refactor theo DbClient:
        public void Update(long id)
        {
            // Tìm dòng Modified trong _dataTable để cập nhật
            EnsureSchema();
            DataRow rowToUpdate = null;
            foreach (DataRow r in _dataTable.Rows)
            {
                if (r.RowState == DataRowState.Modified && Convert.ToInt64(r["ID"]) == id)
                {
                    rowToUpdate = r;
                    break;
                }
            }
            if (rowToUpdate == null)
                throw new InvalidOperationException($"Không tìm thấy người dùng với ID = {id} để cập nhật.");

            bool allRowsValid = true;

            try
            {
                // 1. Chỉ kiểm tra các quy tắc chung TRÊN 'rowToUpdate'
                // (Không cần lặp lại _dataTable)
                if (ValidationRule.ValidateRow(rowToUpdate, _userRules) == false)
                    allRowsValid = false;

                // 2. Khi SỬA, chúng ta không cần kiểm tra mật khẩu
                // (logic "passChanged" bên dưới đã xử lý việc này)
                // Nếu bạn muốn bắt buộc người dùng phải nhập lại mật khẩu khi sửa,
                // hãy thêm logic đó vào đây.
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi validation và dừng lại
                MessageBox.Show(ex.Message, "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng, không lưu
            }

            if (!allRowsValid)
                return;

            _db.InTx((cn, tx) =>
            {
                // NV
                const string sqlNv = @"
                    UPDATE NHAN_VIEN
                       SET HO_TEN=@HOTEN, DIA_CHI=@DIACHI, DIEN_THOAI=@DT
                     WHERE ID=@ID";
                using (var cmd = _db.Cmd(cn, sqlNv, CommandType.Text, tx, 30,
                    _db.P("@HOTEN", SqlDbType.NVarChar, rowToUpdate["HO_TEN"], 200),
                    _db.P("@DIACHI", SqlDbType.NVarChar, rowToUpdate["DIA_CHI"], 255),
                    _db.P("@DT", SqlDbType.NVarChar, rowToUpdate["DIEN_THOAI"], 50),
                    _db.P("@ID", SqlDbType.BigInt, id)))
                {
                    cmd.ExecuteNonQuery();
                }

                // ND
                var orig = rowToUpdate["MAT_KHAU", DataRowVersion.Original];
                var curr = rowToUpdate["MAT_KHAU", DataRowVersion.Current];
                bool passChanged = !Equals(orig, curr);

                string sqlNd = passChanged
                    ? @"UPDATE NGUOI_DUNG SET TEN_DANG_NHAP=@USER, MAT_KHAU=@PASS, QUYEN=@ROLE WHERE ID=@ID"
                    : @"UPDATE NGUOI_DUNG SET TEN_DANG_NHAP=@USER, QUYEN=@ROLE WHERE ID=@ID";

                using (var cmd = _db.Cmd(cn, sqlNd, CommandType.Text, tx, 30,
                    _db.P("@USER", SqlDbType.NVarChar, rowToUpdate["TEN_DANG_NHAP"], 100),
                    _db.P("@ROLE", SqlDbType.NVarChar, rowToUpdate["QUYEN"], 50),
                    _db.P("@ID", SqlDbType.BigInt, id),
                    passChanged
                        ? _db.P("@PASS", SqlDbType.NVarChar, BCrypt.Net.BCrypt.HashPassword(Convert.ToString(curr)), 200)
                        : _db.P("@PASS", SqlDbType.NVarChar, DBNull.Value, 200)))
                {
                    if (!passChanged) cmd.Parameters.RemoveAt("@PASS");
                    cmd.ExecuteNonQuery();
                }

                _dataTable.AcceptChanges();
                return 2; // 2 lệnh update
            });
        }

        /* ========================= DELETE ========================= */

        public void Delete(long id)
        {
            _db.InTx((cn, tx) =>
            {
                // Xóa từ NGUOI_DUNG trước (FK)
                using (var cmd = _db.Cmd(cn, "DELETE FROM NGUOI_DUNG WHERE ID=@ID", CommandType.Text, tx, 30,
                    _db.P("@ID", SqlDbType.BigInt, id)))
                {
                    cmd.ExecuteNonQuery();
                }

                // Sau đó xóa NHAN_VIEN
                using (var cmd = _db.Cmd(cn, "DELETE FROM NHAN_VIEN WHERE ID=@ID", CommandType.Text, tx, 30,
                    _db.P("@ID", SqlDbType.BigInt, id)))
                {
                    cmd.ExecuteNonQuery();
                }

                // Cập nhật DataTable nội bộ nếu có
                if (_dataTable != null)
                {
                    DataRow toRemove = null;
                    foreach (DataRow r in _dataTable.Rows)
                        if (!r.IsNull("ID") && Convert.ToInt64(r["ID"]) == id) { toRemove = r; break; }
                    if (toRemove != null)
                    {
                        _dataTable.Rows.Remove(toRemove);
                        _dataTable.AcceptChanges();
                    }
                }

                return 2;
            });
        }

        private static readonly List<ValidationRule> _userRules = new List<ValidationRule>
        {
            new ValidationRule("HO_TEN", ValidationType.NotEmpty, "Họ tên không được để trống."),
            new ValidationRule("TEN_DANG_NHAP", ValidationType.NotEmpty, "Tên đăng nhập không được để trống."),
            new ValidationRule("QUYEN", ValidationType.NotEmpty, "Quyền người dùng không được để trống.")
            // Mật khẩu sẽ được kiểm tra riêng bên dưới
        };

        // NEW: Hàm helper để chạy validation cho một hàng
        //private void ValidateRow(DataRow row)
        //{
        //    // Chỉ kiểm tra các hàng đã thay đổi
        //    if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Modified)
        //        return;

        //    foreach (var rule in _userRules)
        //    {
        //        if (rule.Type == ValidationType.NotEmpty)
        //        {
        //            var value = row[rule.ColumnName];
        //            if (value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
        //            {
        //                throw new Exception(rule.ErrorMessage);
        //            }
        //        }
        //    }

        //    // Kiểm tra đặc biệt: Mật khẩu là bắt buộc KHI THÊM MỚI
        //    if (row.RowState == DataRowState.Added)
        //    {
        //        var passValue = row["MAT_KHAU"];
        //        if (passValue == DBNull.Value || string.IsNullOrWhiteSpace(passValue.ToString()))
        //        {
        //            throw new Exception("Mật khẩu là bắt buộc khi tạo người dùng mới.");
        //        }
        //    }
        //}
    }
}
