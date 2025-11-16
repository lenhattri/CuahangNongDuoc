// DAL/DataLayer/KhachHangFactory.cs
using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.DAL.Infrastructure;                 // CHANGED: dùng DbClient (singleton)
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class KhachHangFactory : IKhachHangFactory
    {
        // private readonly string _cs = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
        private readonly DbClient _db = DbClient.Instance; // CHANGED: bỏ _cs rải rác, dùng DbClient thống nhất

        private DataTable _table;                          // giữ vai trò như m_Ds cũ
        private const string SELECT_ALL = "SELECT * FROM KHACH_HANG";

        /* ===== Helpers ===== */
        private SqlDataAdapter CreateAdapter(SqlConnection conn)
        {
            // CHANGED: tạo SelectCommand qua DbClient để CommandBuilder sinh CRUD
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(conn, SELECT_ALL, CommandType.Text) // CHANGED
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;        // giữ nguyên
            _ = new SqlCommandBuilder(da);                                  // giữ nguyên (auto Insert/Update/Delete)
            return da;
        }

        private void EnsureSchema()
        {
            if (_table != null) return;
            // CHANGED: lấy schema rỗng qua DbClient
            using (var conn = _db.Open())                                                    // CHANGED
            using (var cmd = _db.Cmd(conn, SELECT_ALL + " WHERE 1=0", CommandType.Text))    // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                       // CHANGED
            {
                _table = new DataTable("KHACH_HANG");
                da.FillSchema(_table, SchemaType.Source);                                    // CHANGED (rõ ràng schema)
            }
        }

        /* ===== API ===== */

        public DataTable DanhsachKhachHang(bool loai)
        {
            const string sql = "SELECT * FROM KHACH_HANG WHERE LOAI_KH = @loai";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,                             // CHANGED
                _db.P("@loai", SqlDbType.Bit, loai));
            _table = dt;
            return dt;
        }

        public DataTable TimHoTen(string hoten, bool loai)
        {
            // CHANGED: dùng LIKE @hoten và truyền giá trị có wildcard để sargable hơn
            const string sql = @"SELECT * FROM KHACH_HANG
                                 WHERE HO_TEN LIKE @hoten AND LOAI_KH = @loai";
            var term = $"%{hoten ?? string.Empty}%";                                         // CHANGED
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,                             // CHANGED
                _db.P("@hoten", SqlDbType.NVarChar, term, 200),
                _db.P("@loai", SqlDbType.Bit, loai));
            _table = dt;
            return dt;
        }

        public DataTable TimDiaChi(string diachi, bool loai)
        {
            const string sql = @"SELECT * FROM KHACH_HANG
                                 WHERE DIA_CHI LIKE @diachi AND LOAI_KH = @loai";
            var term = $"%{diachi ?? string.Empty}%";                                        // CHANGED
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,                             // CHANGED
                _db.P("@diachi", SqlDbType.NVarChar, term, 200),
                _db.P("@loai", SqlDbType.Bit, loai));
            _table = dt;
            return dt;
        }

        public DataTable DanhsachKhachHang()
        {
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);                     // CHANGED
            _table = dt;
            return dt;
        }

        public DataTable LayKhachHang(string id)
        {
            const string sql = "SELECT * FROM KHACH_HANG WHERE ID = @id";
            // CHANGED: dùng NVarChar 50 để hỗ trợ Unicode ID (nếu ID là varchar, vẫn hoạt động)
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,                             // CHANGED
                _db.P("@id", SqlDbType.NVarChar, id, 50));
            _table = dt;
            return dt;
        }

        public DataRow NewRow()
        {
            EnsureSchema();
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();
            _table.Rows.Add(row);
        }

        // CHANGED: Thay thế hàm Save() cũ
        public bool Save()
        {
            EnsureSchema();

            // Gọi helper tĩnh để thực hiện logic Save
            return DataAccessHelper.PerformSave(
                _table,             // DataTable nội bộ
                _khachHangRules,    // Danh sách quy tắc
                this.CreateAdapter, // Phương thức tạo Adapter
                _db                 // Instance DbClient
            );
        }

        // NEW: Thêm danh sách quy tắc cho Khách Hàng
        /// <summary>
        /// Danh sách các quy tắc kiểm tra hợp lệ cho bảng KHACH_HANG
        /// </summary>
        private static readonly List<ValidationRule> _khachHangRules = new List<ValidationRule>
        {
            new ValidationRule("ID", ValidationType.NotEmpty, "Mã khách hàng không được để trống."),
            new ValidationRule("HO_TEN", ValidationType.NotEmpty, "Tên khách hàng không được để trống."),
            new ValidationRule("LOAI_KH", ValidationType.NotNull, "Chưa chọn loại khách hàng."),
            
            // Các cột DIEN_THOAI, DIA_CHI được phép trống (không cần quy tắc)
        };
    }
}
