// DAL/DataLayer/PhieuChiFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;
using System.Collections.Generic;      // NEW: Thêm để dùng List
using CuahangNongduoc.Utils.Functions; // NEW: Thêm để dùng DataAccessHelper
using System.Windows.Forms;             // NEW: Thêm để dùng ValidationRule

namespace CuahangNongduoc.DataLayer
{
    public class PhieuChiFactory : IPhieuChiFactory
    {
        // REMOVED: Bỏ _connectionString

        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table; // DataTable nội bộ cho pattern NewRow/Add/Save
        private const string SELECT_ALL = "SELECT * FROM PHIEU_CHI";

        /* ===================== Helpers ===================== */

        private void EnsureSchema() // NEW
        {
            if (_table != null) return;
            // Dùng ExecuteDataTable với 1=0 để chỉ lấy schema
            _table = _db.ExecuteDataTable(SELECT_ALL + " WHERE 1=0", CommandType.Text);
            _table.TableName = "PHIEU_CHI";
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn) // NEW: phục vụ Save()
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // auto sinh Insert/Update/Delete
            return da;
        }

        /* ===================== SELECTs (Refactored) ===================== */

        /// <summary>
        /// Lấy tất cả phiếu chi.
        /// </summary>
        public DataTable DanhsachPhieuChi()
        {
            // CHANGED: Dùng DbClient.ExecuteDataTable và đồng bộ _table
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            dt.TableName = "PHIEU_CHI";
            _table = dt;
            return dt;
        }

        /// <summary>
        /// Tìm phiếu chi theo lý do và ngày (chính xác trong ngày).
        /// </summary>
        public DataTable TimPhieuChi(int lydo, DateTime ngay)
        {
            // <<< SỬA LỖI: Dùng khoảng thời gian [start, end)
            var start = ngay.Date;
            var end = start.AddDays(1);

            const string sql = @"
                SELECT * FROM PHIEU_CHI 
                WHERE ID_LY_DO_CHI = @lydo 
                  AND NGAY_CHI >= @start 
                  AND NGAY_CHI < @end";

            // CHANGED: Dùng DbClient.ExecuteDataTable
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@lydo", SqlDbType.Int, lydo),
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));

            dt.TableName = "PHIEU_CHI";
            _table = dt; // Đồng bộ
            return dt;
        }

        /// <summary>
        /// Lấy phiếu chi theo ID.
        /// </summary>
        /// <returns>A new DataRow with the schema of PHIEU_CHI table.</returns>
        //public DataRow NewRow()
        //{
        //    var dataTable = new DataTable();
        //    using (var connection = new SqlConnection(_connectionString))
        //    using (var command = new SqlCommand("SELECT * FROM PHIEU_CHI WHERE 1=0", connection))
        //    using (var adapter = new SqlDataAdapter(command))
        //    {
        //        adapter.FillSchema(dataTable, SchemaType.Source);
        //    }

        //    return dataTable.NewRow();
        //}

        /* ===================== INSERT/UPDATE/DELETE ===================== */
        // Gợi ý: dùng theo kiểu “row-based” cho nhanh; nếu team muốn DTO, mình viết thêm class DTO.

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row
            const string sql =
                @"INSERT INTO PHIEU_CHI
                    (ID, ID_LY_DO_CHI, NGAY_CHI, TONG_TIEN, GHI_CHU)
                  VALUES(@ID, @ID_LY_DO_CHI, @NGAY_CHI, @TONG_TIEN, @GHI_CHU)";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = row["ID"];
                cmd.Parameters.Add("@ID_LY_DO_CHI", SqlDbType.Int).Value = row["ID_LY_DO_CHI"];
                cmd.Parameters.Add("@NGAY_CHI", SqlDbType.DateTime).Value = row["NGAY_CHI"];
                cmd.Parameters.Add("@TONG_TIEN", SqlDbType.BigInt).Value = row["TONG_TIEN"];
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.VarChar, 255).Value = row["GHI_CHU"] ?? (object)DBNull.Value; // Assuming max length 255, adjust as needed
                return cmd.ExecuteNonQuery();
            }
        }
        public int Update(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row
            const string sql =
                @"UPDATE PHIEU_CHI 
                  SET ID_LY_DO_CHI = @ID_LY_DO_CHI, NGAY_CHI = @NGAY_CHI, TONG_TIEN = @TONG_TIEN, GHI_CHU = @GHI_CHU
                  WHERE ID = @ID";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID_LY_DO_CHI", SqlDbType.VarChar, 50).Value = row["ID_LY_DO_CHI"];
                cmd.Parameters.Add("@NGAY_CHI", SqlDbType.DateTime).Value = row["NGAY_CHI"];
                cmd.Parameters.Add("@TONG_TIEN", SqlDbType.BigInt).Value = row["TONG_TIEN"];
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.VarChar, 255).Value = row["GHI_CHU"] ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = row["ID"];
                return cmd.ExecuteNonQuery();

                //    var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);   // CHANGED
                //_table = dt;
                //return dt;
            }
        }

        public DataTable LayPhieuChi(string id)
        {
            // CHANGED: Dùng DbClient.ExecuteDataTable
            const string sql = SELECT_ALL + " WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.VarChar, id, 50)); // Giả định ID là VarChar(50)

            dt.TableName = "PHIEU_CHI";
            _table = dt; // Đồng bộ
            return dt;
        }

        /// <summary>
        /// Lấy tổng tiền chi theo lý do, tháng, năm.
        /// </summary>
        public long LayTongTien(int lydo, int thang, int nam)
        {
            // <<< SỬA LỖI: Nhất quán kiểu dữ liệu, dùng int cho lydo
            var db = DbClient.Instance; // Lấy instance vì là hàm static
            const string sql = @"
                SELECT SUM(TONG_TIEN) FROM PHIEU_CHI 
                WHERE ID_LY_DO_CHI = @lydo 
                  AND MONTH(NGAY_CHI) = @thang 
                  AND YEAR(NGAY_CHI) = @nam";

            var result = db.ExecuteScalar<object>(sql, CommandType.Text,
                db.P("@lydo", SqlDbType.Int, lydo), // Sửa thành Int
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));

            return result == null || result == DBNull.Value ? 0L : Convert.ToInt64(result);
        }

        /* ===================== DataTable pattern (Refactored) ===================== */

        /// <summary>
        /// Tạo một DataRow mới theo schema PHIEU_CHI.
        /// </summary>
        public DataRow NewRow()
        {
            // CHANGED: Chỉ lấy schema, không lấy data
            EnsureSchema();
            return _table.NewRow();
        }

        /// <summary>
        /// Thêm một DataRow vào DataTable nội bộ.
        /// </summary>
        public void Add(DataRow row)
        {
            EnsureSchema();
            _table.Rows.Add(row);
        }

        /* ===================== SAVE (DataAccessHelper) ===================== */

        // NEW: Danh sách quy tắc kiểm tra hợp lệ
        private static readonly List<ValidationRule> _phieuChiRules = new List<ValidationRule>
        {
            new ValidationRule("ID", ValidationType.NotEmpty, "Số phiếu chi không được để trống."),
            new ValidationRule("ID_LY_DO_CHI", ValidationType.NotEmpty, "Lý do chi không được để trống."),
            new ValidationRule("NGAY_CHI", ValidationType.NotEmpty, "Ngày chi không được để trống."),
            new ValidationRule("TONG_TIEN", ValidationType.NotEmpty, "Tổng tiền không được để trống.")
        };

        /// <summary>
        /// Lưu tất cả thay đổi (Thêm, Sửa, Xóa) vào CSDL.
        /// </summary>
        public bool Save()
        {
            // REMOVED: Toàn bộ logic Insert, Update, Delete, SaveChanges thủ công
            // CHANGED: Thay bằng DataAccessHelper
            EnsureSchema();
            if (_table == null) return false;

            return DataAccessHelper.PerformSave(
                _table,
                _phieuChiRules,
                this.CreateAdapter,
                _db
            );
        }
    }
}