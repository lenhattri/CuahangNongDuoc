// DAL/DataLayer/LyDoChiFactory.cs
using CuahangNongduoc.DAL.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;
using System.Collections.Generic;           // NEW: Thêm để dùng List
using CuahangNongduoc.Utils.Functions;
using System.Windows.Forms;       // NEW: Thêm để dùng ValidationRule

namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory : ILyDoChiFactory
    {
        // REMOVED: Bỏ _connectionString và DataService m_Ds
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table; // DataTable nội bộ
        private const string SELECT_ALL = "SELECT ID, LY_DO FROM LY_DO_CHI";

        /* ==================== Helpers ==================== */

        private void EnsureSchema()
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("LY_DO_CHI");
                da.FillSchema(_table, SchemaType.Source);
            }
        }

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
        /// Lấy toàn bộ danh sách lý do chi.
        /// </summary>
        public DataTable DanhsachLyDo()
        {
            // CHANGED: Dùng DbClient và đồng bộ _table
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            _table = dt;
            return dt;
        }

        /// <summary>
        /// Lấy một lý do chi theo ID.
        /// </summary>
        public DataTable LayLyDoChi(long id)
        {
            // CHANGED: Dùng DbClient, tham số hóa, và đồng bộ _table
            const string sql = SELECT_ALL + " WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.BigInt, id));
            _table = dt;
            return dt;
        }

        /// <summary>
        /// Tạo một DataRow mới theo schema.
        /// </summary>
        public DataRow NewRow()
        {
            // CHANGED: Dùng _table nội bộ
            EnsureSchema();
            return _table.NewRow();
        }

        /// <summary>
        /// Thêm một DataRow vào _table nội bộ.
        /// </summary>
        public void Add(DataRow row)
        {
            // CHANGED: Dùng _table nội bộ
            EnsureSchema();
            _table.Rows.Add(row);
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
                _lyDoChiRules,
                this.CreateAdapter,
                _db
            );
        }

        // REMOVED: Toàn bộ các hàm Insert, Update, Delete, SaveChanges thủ công

        /// <summary>
        /// Danh sách các quy tắc kiểm tra hợp lệ cho bảng LY_DO_CHI
        /// </summary>
        private static readonly List<ValidationRule> _lyDoChiRules = new List<ValidationRule>
        {
            // Giả định ID là cột IDENTITY tự tăng, nên không cần validate khi thêm
            new ValidationRule("LY_DO", ValidationType.NotEmpty, "Lý do chi không được để trống.")
        };
    }
}