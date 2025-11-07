// DAL/DataLayer/DonViTinhDAL.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;
using System.Collections.Generic;      // NEW: Thêm để dùng List
using CuahangNongduoc.Utils.Functions; // NEW: Thêm để dùng DataAccessHelper
using System.Windows.Forms;             // NEW: Thêm để dùng ValidationRule

namespace CuahangNongduoc.DataLayer
{
    public class DonViTinhDAL : IDonViTinhDAL
    {
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table; // DataTable nội bộ
        // NEW: Khai báo rõ cột để SqlCommandBuilder hoạt động chính xác
        private const string SELECT_ALL = "SELECT ID, TEN_DON_VI FROM DON_VI_TINH";

        /* ==================== Helpers ==================== */

        // NEW: Đảm bảo _table có schema
        private void EnsureSchema()
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("DON_VI_TINH");
                da.FillSchema(_table, SchemaType.Source);
            }
        }

        // NEW: Hàm tạo Adapter để dùng cho SqlCommandBuilder
        private SqlDataAdapter CreateAdapter(SqlConnection cn)
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // Tự sinh INSERT, UPDATE, DELETE
            return da;
        }

        /* ==================== API ==================== */

        /// <summary>
        /// Lấy toàn bộ danh sách đơn vị tính.
        /// </summary>
        public DataTable DanhSachDVT()
        {
            // CHANGED: Dùng DbClient và đồng bộ _table
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            _table = dt;
            return dt;
        }

        /// <summary>
        /// Lấy một đơn vị tính theo ID.
        /// </summary>
        public DataTable LayDVT(int id)
        {
            // CHANGED: Dùng DbClient, tham số hóa, và đồng bộ _table
            const string sql = SELECT_ALL + " WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.Int, id));
            _table = dt;
            return dt;
        }

        /// <summary>
        /// Lưu tất cả thay đổi (Thêm, Sửa, Xóa) vào CSDL.
        /// </summary>
        public bool Save()
        {
            // NEW: Dùng helper chung
            EnsureSchema();
            return DataAccessHelper.PerformSave(
                _table,
                _donViTinhRules, // Sử dụng Validation Rules bên dưới
                this.CreateAdapter,
                _db
            );
        }

        // REMOVED: Toàn bộ các hàm Insert, Update, Delete, và Save(DataTable table) thủ công

        /// <summary>
        /// Danh sách các quy tắc kiểm tra hợp lệ cho bảng DON_VI_TINH
        /// </summary>
        private static readonly List<ValidationRule> _donViTinhRules = new List<ValidationRule>
        {
            // Giả định ID là cột IDENTITY tự tăng
            new ValidationRule("TEN_DON_VI", ValidationType.NotEmpty, "Tên đơn vị tính không được để trống.")
            // GHI_CHU có thể trống nên không cần rule
        };
    }
}