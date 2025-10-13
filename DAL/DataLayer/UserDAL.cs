using CuahangNongduoc.Domain.Entities;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc.DAL.DataLayer
{
    public class UserDAL
    {
        private DataTable _dataTable;
        private static readonly string _cs = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
        public DataTable LayDanhSachNguoiDung()
        {
            return SafeExecutor.Execute(() =>
            {
                const string query = @"
            SELECT 
                NV.ID,
                NV.HO_TEN,
                NV.DIA_CHI,
                NV.DIEN_THOAI,
                ND.TEN_DANG_NHAP,
                ND.MAT_KHAU,
                ND.QUYEN
            FROM NHAN_VIEN NV
            JOIN NGUOI_DUNG ND ON NV.ID = ND.ID";

                _dataTable = DataTableHelper.GetDataTable(query);
                LogHelper.LogInfo($"Đã lấy danh sách người dùng mở rộng: {_dataTable.Rows.Count}");
                return _dataTable;
            }, nameof(LayDanhSachNguoiDung));
        }


        public DataRow LayNguoiDungTheoId(string id)
        {
            return SafeExecutor.Execute(() =>
            {
                const string query = "SELECT ID, TEN_DANG_NHAP, QUYEN FROM NGUOI_DUNG WHERE ID = @id";
                var parameters = new[]
                {
                    new SqlParameter("@id", SqlDbType.VarChar) { Value = id }
                };
                var dt = DataTableHelper.GetDataTable(query, parameters);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }, nameof(LayNguoiDungTheoId));
        }

        public DataRow LayNguoiDungTheoTenDangNhap(string tenDangNhap)
        {
            return SafeExecutor.Execute(() =>
            {
                const string query = "SELECT NHAN_VIEN.ID, HO_TEN, DIA_CHI, DIEN_THOAI, TEN_DANG_NHAP, MAT_KHAU, QUYEN FROM NGUOI_DUNG JOIN NHAN_VIEN ON NGUOI_DUNG.ID = NHAN_VIEN.ID WHERE TEN_DANG_NHAP = @tenDangNhap";
                var parameters = new[]
                {
                    new SqlParameter("@tenDangNhap", SqlDbType.VarChar) { Value = tenDangNhap }
                };
                var dt = DataTableHelper.GetDataTable(query, parameters);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }, nameof(LayNguoiDungTheoTenDangNhap));
        }

        public DataRow NewRow()
        {
            return SafeExecutor.Execute(() =>
            {
                if (_dataTable == null)
                    _dataTable = LayDanhSachNguoiDung();
                return _dataTable.NewRow();

            }, nameof(NewRow));
        }

        public void Add(DataRow row)
        {
            SafeExecutor.Execute(() =>
            {
                if (_dataTable == null)
                    _dataTable = LayDanhSachNguoiDung();
                _dataTable.Rows.Add(row);
            }, nameof(Add));
        }

        public bool Save()
        {
            bool success = false;

            SafeExecutor.Execute(() =>
            {
                if (_dataTable == null || _dataTable.Rows.Count == 0)
                    throw new InvalidOperationException("Không có dữ liệu để lưu.");

                using (var conn = new SqlConnection(_cs))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // ✅ BẮT ĐẦU VÒNG LẶP QUA TẤT CẢ CÁC DÒNG
                            foreach (DataRow row in _dataTable.Rows)
                            {
                                // Chỉ xử lý những dòng mới được thêm vào
                                if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Modified)
                                    continue; // Bỏ qua dòng này và chuyển đến dòng tiếp theo

                                // 1️⃣ Chuẩn bị DataTable NHAN_VIEN
                                var tableNV = new DataTable("NHAN_VIEN");
                                // Cấu hình cột ID để mô phỏng khóa chính tự tăng
                                var idColumn = new DataColumn("ID", typeof(long))
                                {
                                    AutoIncrement = true,       // Tự động tăng
                                    AutoIncrementSeed = -1,     // Bắt đầu từ số âm (để phân biệt với ID từ DB)
                                    AutoIncrementStep = -1,
                                    AllowDBNull = false,        // Không cho phép null
                                    Unique = true               // Là duy nhất
                                };
                                tableNV.Columns.Add(idColumn);
                                tableNV.PrimaryKey = new DataColumn[] { idColumn };

                                tableNV.Columns.Add("HO_TEN", typeof(string));
                                tableNV.Columns.Add("DIA_CHI", typeof(string));
                                tableNV.Columns.Add("DIEN_THOAI", typeof(string));

                                var newRowNV = tableNV.NewRow();
                                newRowNV["HO_TEN"] = row["HO_TEN"];
                                newRowNV["DIA_CHI"] = row["DIA_CHI"];
                                newRowNV["DIEN_THOAI"] = row["DIEN_THOAI"];
                                tableNV.Rows.Add(newRowNV);

                                // 🧩 Lưu và lấy ID nhân viên
                                long idNhanVien = DataTableHelper.SaveAndGetId(tableNV, conn, tran);

                                // Nếu không lấy được ID, có lỗi xảy ra, dừng lại
                                if (idNhanVien == -1)
                                {
                                    throw new Exception("Không thể lấy được ID của nhân viên vừa tạo.");
                                }

                                // 2️⃣ Chuẩn bị DataTable NGUOI_DUNG
                                var tableND = new DataTable("NGUOI_DUNG");
                                tableND.Columns.Add("TEN_DANG_NHAP", typeof(string));
                                tableND.Columns.Add("MAT_KHAU", typeof(string));
                                tableND.Columns.Add("QUYEN", typeof(string));
                                tableND.Columns.Add("ID", typeof(long));

                                var newRowND = tableND.NewRow();
                                newRowND["TEN_DANG_NHAP"] = row["TEN_DANG_NHAP"];
                                newRowND["MAT_KHAU"] = row["MAT_KHAU"];
                                newRowND["QUYEN"] = row["QUYEN"];
                                newRowND["ID"] = idNhanVien;
                                tableND.Rows.Add(newRowND);

                                row["ID"] = idNhanVien; // Cập nhật ID vào DataRow gốc

                                // 🧩 Lưu NGUOI_DUNG
                                DataTableHelper.Save(tableND, conn, tran);
                            } // ✅ KẾT THÚC VÒNG LẶP

                            tran.Commit(); // Chỉ commit khi tất cả các dòng đã được xử lý thành công
                            _dataTable.AcceptChanges();
                            success = true;
                        }
                        catch
                        {
                            tran.Rollback(); // Nếu có bất kỳ lỗi nào, hủy bỏ toàn bộ giao dịch
                            throw; // Ném lại lỗi để SafeExecutor xử lý
                        }
                    }
                }
            }, nameof(Save));

            return success;
        }

        public void Update(long id)
        {
            SafeExecutor.Execute(() =>
            {
                if (_dataTable == null || _dataTable.Rows.Count == 0)
                    throw new InvalidOperationException("Không có dữ liệu để cập nhật.");

                using (var conn = new SqlConnection(_cs))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Tìm dòng cần cập nhật
                            DataRow rowToUpdate = null;
                            foreach (DataRow row in _dataTable.Rows)
                            {
                                if (row.RowState == DataRowState.Modified && Convert.ToInt64(row["ID"]) == id)
                                {
                                    rowToUpdate = row;
                                    break;
                                }
                            }

                            if (rowToUpdate == null)
                                throw new Exception($"Không tìm thấy người dùng với ID = {id} để cập nhật.");

                            // Cập nhật NHAN_VIEN (không thay đổi)
                            var updateNVQuery = @"
                        UPDATE NHAN_VIEN
                        SET HO_TEN = @hoTen,
                            DIA_CHI = @diaChi,
                            DIEN_THOAI = @dienThoai
                        WHERE ID = @id";
                            using (var cmdNV = new SqlCommand(updateNVQuery, conn, tran))
                            {
                                cmdNV.Parameters.AddWithValue("@hoTen", rowToUpdate["HO_TEN"]);
                                cmdNV.Parameters.AddWithValue("@diaChi", rowToUpdate["DIA_CHI"]);
                                cmdNV.Parameters.AddWithValue("@dienThoai", rowToUpdate["DIEN_THOAI"]);
                                cmdNV.Parameters.AddWithValue("@id", id);
                                cmdNV.ExecuteNonQuery();
                            }


                            // Kiểm tra xem mật khẩu có thực sự thay đổi không
                            var originalPassword = rowToUpdate["MAT_KHAU", DataRowVersion.Original];
                            var currentPassword = rowToUpdate["MAT_KHAU", DataRowVersion.Current];
                            bool passwordChanged = !object.Equals(originalPassword, currentPassword);

                            string updateNDQuery;
                            string hashedPassword = null;

                            if (passwordChanged)
                            {
                                // Nếu mật khẩu đã thay đổi, tạo query có cập nhật MAT_KHAU
                                updateNDQuery = @"
                                UPDATE NGUOI_DUNG
                                SET TEN_DANG_NHAP = @tenDangNhap,
                                    MAT_KHAU = @matKhau,
                                    QUYEN = @quyen
                                WHERE ID = @id";
                                // Băm mật khẩu mới
                                hashedPassword = BCrypt.Net.BCrypt.HashPassword(Convert.ToString(currentPassword));
                            }
                            else
                            {
                                // Nếu không, tạo query không cập nhật MAT_KHAU
                                updateNDQuery = @"
                                UPDATE NGUOI_DUNG
                                SET TEN_DANG_NHAP = @tenDangNhap,
                                    QUYEN = @quyen
                                WHERE ID = @id";
                            }

                            using (var cmdND = new SqlCommand(updateNDQuery, conn, tran))
                            {
                                cmdND.Parameters.AddWithValue("@tenDangNhap", rowToUpdate["TEN_DANG_NHAP"]);
                                cmdND.Parameters.AddWithValue("@quyen", rowToUpdate["QUYEN"]);
                                // Lưu ý: Sửa lỗi chính tả từ "@id " thành "@id"
                                cmdND.Parameters.AddWithValue("@id", id);

                                // Chỉ thêm parameter @matKhau nếu mật khẩu thay đổi
                                if (passwordChanged)
                                {
                                    cmdND.Parameters.AddWithValue("@matKhau", hashedPassword);
                                }
                                cmdND.ExecuteNonQuery();
                            }

                            tran.Commit();
                            _dataTable.AcceptChanges();
                            LogHelper.LogInfo($"Cập nhật người dùng thành công với ID = {id}");
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }, nameof(Update));
        }

        public void Delete(long id)
        {
            SafeExecutor.Execute(() =>
            {
                using (var conn = new SqlConnection(_cs))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Xóa từ NGUOI_DUNG trước
                            var deleteNDQuery = "DELETE FROM NGUOI_DUNG WHERE ID = @id";
                            using (var cmdND = new SqlCommand(deleteNDQuery, conn, tran))
                            {
                                cmdND.Parameters.AddWithValue("@id", id);
                                cmdND.ExecuteNonQuery();
                            }
                            // Xóa từ NHAN_VIEN sau
                            var deleteNVQuery = "DELETE FROM NHAN_VIEN WHERE ID = @id";
                            using (var cmdNV = new SqlCommand(deleteNVQuery, conn, tran))
                            {
                                cmdNV.Parameters.AddWithValue("@id", id);
                                cmdNV.ExecuteNonQuery();
                            }
                            tran.Commit();
                            // Cập nhật lại DataTable nội bộ
                            if (_dataTable != null)
                            {
                                DataRow rowToDelete = null;
                                foreach (DataRow row in _dataTable.Rows)
                                {
                                    if (Convert.ToInt64(row["ID"]) == id)
                                    {
                                        rowToDelete = row;
                                        break;
                                    }
                                }
                                if (rowToDelete != null)
                                {
                                    _dataTable.Rows.Remove(rowToDelete);
                                    _dataTable.AcceptChanges();
                                }
                            }
                            LogHelper.LogInfo($"Xóa người dùng thành công với ID = {id}");
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }, nameof(Delete));
        }
    }
}
