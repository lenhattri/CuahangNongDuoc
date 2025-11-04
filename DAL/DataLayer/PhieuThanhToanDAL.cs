// DAL/DataLayer/PhieuThanhToanDAL.cs
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CuahangNongduoc.DAL.Infrastructure;
using System.Collections.Generic;      // NEW: Thêm để dùng List
using CuahangNongduoc.Utils.Functions; // NEW: Thêm để dùng DataAccessHelper

namespace CuahangNongduoc.DataLayer
{
    public class PhieuThanhToanDAL
    {
        private DataTable _dataTable; // CHANGED: đổi tên từ m_DataTable
        private readonly DbClient _db = DbClient.Instance;

        // NEW: Định nghĩa Select-All
        private const string SELECT_ALL = "SELECT * FROM PHIEU_THANH_TOAN";

        /* ==================== Helpers ==================== */

        // NEW: Đảm bảo _dataTable có schema
        private void EnsureSchema()
        {
            if (_dataTable != null) return;

            // Dùng ExecuteDataTable với 1=0 để lấy schema
            _dataTable = _db.ExecuteDataTable(SELECT_ALL + " WHERE 1=0", CommandType.Text);
            _dataTable.TableName = "PHIEU_THANH_TOAN";
        }

        // NEW: Hàm tạo Adapter để dùng cho DataAccessHelper/SqlCommandBuilder
        private SqlDataAdapter CreateAdapter(SqlConnection cn)
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            // Quan trọng: Phải có AddWithKey để CommandBuilder 
            // biết khóa chính và tạo lệnh UPDATE/DELETE
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            _ = new SqlCommandBuilder(da); // Tự sinh INSERT, UPDATE, DELETE
            return da;
        }

        /* ===================== SELECT ALL ===================== */
        public DataTable DanhsachPhieuThanhToan()
        {
            // CHANGED: Dùng DbClient.ExecuteDataTable cho ngắn gọn
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            dt.TableName = "PHIEU_THANH_TOAN";
            _dataTable = dt; // Đồng bộ
            return dt;
        }

        /* ===================== FIND BY CUSTOMER + DATE (range) ===================== */
        public DataTable TimPhieuThanhToan(string kh, DateTime ngay)
        {
            var start = ngay.Date;
            var end = start.AddDays(1);

            const string sql = @"
                SELECT * FROM PHIEU_THANH_TOAN
                WHERE ID_KHACH_HANG = @kh
                  AND NGAY_THANH_TOAN >= @start
                  AND NGAY_THANH_TOAN <  @end";

            // CHANGED: Dùng DbClient.ExecuteDataTable cho ngắn gọn
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@kh", SqlDbType.NVarChar, kh, 50),
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));

            dt.TableName = "PHIEU_THANH_TOAN";
            _dataTable = dt; // Đồng bộ
            return dt;
        }

        /* ===================== GET BY ID ===================== */
        public DataTable LayPhieuThanhToan(string id)
        {
            // CHANGED: Dùng DbClient.ExecuteDataTable cho ngắn gọn
            const string sql = SELECT_ALL + " WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, id, 50));

            dt.TableName = "PHIEU_THANH_TOAN";
            _dataTable = dt; // Đồng bộ
            return dt;
        }

        /* ===================== AGGREGATE: SUM(TONG_TIEN) ===================== */
        // Phương thức này là static và đã dùng DbClient, giữ nguyên
        public static long LayTongTien(string kh, int thang, int nam)
        {
            var db = DbClient.Instance;
            const string sql = @"
                SELECT SUM(TONG_TIEN)
                FROM PHIEU_THANH_TOAN
                WHERE ID_KHACH_HANG = @kh
                  AND MONTH(NGAY_THANH_TOAN) = @thang
                  AND YEAR(NGAY_THANH_TOAN)  = @nam";
            var obj = db.ExecuteScalar<object>(sql, CommandType.Text,
                db.P("@kh", SqlDbType.NVarChar, kh, 50),
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));
            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }

        /* ===================== DATATABLE HELPERS (giữ API cũ) ===================== */
        public DataRow NewRow()
        {
            // CHANGED: Dùng EnsureSchema (lấy schema) thay vì DanhsachPhieuThanhToan (lấy data)
            EnsureSchema();
            return _dataTable.NewRow();
        }

        public void Add(DataRow row)
        {
            // CHANGED: Dùng EnsureSchema
            EnsureSchema();
            _dataTable.Rows.Add(row);
        }

        /* ===================== SAVE (DataAccessHelper) ===================== */

        // NEW: Danh sách các quy tắc kiểm tra hợp lệ
        private static readonly List<ValidationRule> _phieuThanhToanRules = new List<ValidationRule>
        {
            new ValidationRule("ID", ValidationType.NotEmpty, "Số phiếu thanh toán không được để trống."),
            new ValidationRule("ID_KHACH_HANG", ValidationType.NotEmpty, "Khách hàng không được để trống."),
            new ValidationRule("NGAY_THANH_TOAN", ValidationType.NotEmpty, "Ngày thanh toán không được để trống."),
            new ValidationRule("TONG_TIEN", ValidationType.NotEmpty, "Tổng tiền không được để trống.")
        };

        /// <summary>
        /// Lưu tất cả thay đổi (Thêm, Sửa, Xóa) vào CSDL
        /// </summary>
        public bool Save()
        {
            // CHANGED: Thay thế toàn bộ logic cũ
            if (_dataTable == null) return false;

            EnsureSchema();
            return DataAccessHelper.PerformSave(
                _dataTable,
                _phieuThanhToanRules,
                this.CreateAdapter,
                _db
            );
        }
    }
}