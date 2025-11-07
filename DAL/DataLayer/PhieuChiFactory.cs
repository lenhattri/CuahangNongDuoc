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
    public class PhieuChiFactory
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
        public static long LayTongTien(int lydo, int thang, int nam)
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