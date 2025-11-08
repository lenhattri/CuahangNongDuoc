// DAL/DataLayer/ChiPhiPhatSinhDAL.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;
using System.Collections.Generic;    // NEW: Thêm để dùng List
using CuahangNongduoc.Utils.Functions; // NEW: Thêm để dùng DataAccessHelper
using System.Windows.Forms;          // NEW: Thêm để dùng ValidationRule

namespace CuahangNongduoc.DataLayer
{
    public class ChiPhiPhatSinhDAL
    {
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table; // DataTable nội bộ

        // NEW: Khai báo rõ TẤT CẢ các cột để SqlCommandBuilder hoạt động
        private const string SELECT_ALL = "SELECT ID, TEN_CHI_PHI, LOAI_CHI_PHI, SO_TIEN FROM [dbo].[CHI_PHI_PHAT_SINH]";

        /* ==================== Helpers ==================== */

        // NEW: Đảm bảo _table có schema
        private void EnsureSchema()
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("CHI_PHI_PHAT_SINH");
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

        public DataRow AddRow()
        {
            EnsureSchema();
            var row = _table.NewRow();
            _table.Rows.Add(row);
            return row;
        }

        /* ==================== API ==================== */

        /// <summary>
        /// Lấy toàn bộ danh sách chi phí phát sinh.
        /// </summary>
        public DataTable DanhSachChiPhiPhatSinh()
        {
            // CHANGED: Dùng DbClient và đồng bộ _table
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            _table = dt;
            return dt;
        }

        /// <summary>
        /// Lấy một chi phí phát sinh theo ID.
        /// </summary>
        public DataTable LayCPPS(string id)
        {
            // NEW: Thêm hàm lấy theo ID
            const string sql = SELECT_ALL + " WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, id, 50));
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
                _chiPhiPhatSinhRules, // Sử dụng Validation Rules bên dưới
                this.CreateAdapter,
                _db
            );
        }

        // REMOVED: Toàn bộ các hàm InSert, Update, Delete thủ công

        /// <summary>
        /// Danh sách các quy tắc kiểm tra hợp lệ cho bảng CHI_PHI_PHAT_SINH
        /// </summary>
        private static readonly List<ValidationRule> _chiPhiPhatSinhRules = new List<ValidationRule>
        {
            // Giả định ID là khóa chính (NVarChar), không phải tự tăng
            new ValidationRule("ID", ValidationType.NotEmpty, "Mã chi phí không được để trống."),
            new ValidationRule("TEN_CHI_PHI", ValidationType.NotEmpty, "Tên chi phí không được để trống."),
            new ValidationRule("LOAI_CHI_PHI", ValidationType.NotEmpty, "Loại chi phí không được để trống.")
            // SO_TIEN có thể là 0 nên không cần rule NotEmpty
        };
    }
}