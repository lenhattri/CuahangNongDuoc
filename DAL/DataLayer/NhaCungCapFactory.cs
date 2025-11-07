// DAL/DataLayer/NhaCungCapFactory.cs
using CuahangNongduoc.BusinessObject;        // NEW: Thêm BusinessObject
using CuahangNongduoc.DAL.Infrastructure;
using CuahangNongduoc.Utils.Functions;       // NEW: Thêm để dùng ValidationRule
using System;                                 // NEW
using System.Collections.Generic;           // NEW: Thêm để dùng List
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    // CHANGED: Đổi tên class
    public class NhaCungCapDAL
    {
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table;                           // NEW: DataTable nội bộ
        private const string SELECT_ALL = "SELECT * FROM NHA_CUNG_CAP"; // NEW

        /* ==================== Helpers (NEW) ==================== */

        private void EnsureSchema()
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("NHA_CUNG_CAP");
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
            _ = new SqlCommandBuilder(da); // auto sinh Insert/Update/Delete
            return da;
        }

        /* ==================== SELECTs ==================== */

        // Lấy toàn bộ danh sách NCC
        public DataTable DanhsachNCC()
        {
            // CHANGED: Dùng SELECT_ALL
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);
            _table = dt; // NEW: đồng bộ
            return dt;
        }

        // Lấy NCC theo ID
        public DataTable LayNCC(string id)
        {
            // CHANGED: Dùng SELECT_ALL
            const string sql = SELECT_ALL + " WHERE ID = @Id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, id, 50));
            _table = dt; // NEW: đồng bộ
            return dt;
        }

        // Tìm theo địa chỉ (LIKE)
        public DataTable TimDiaChi(string diachi)
        {
            // CHANGED: Dùng SELECT_ALL
            const string sql = SELECT_ALL + " WHERE DIA_CHI LIKE @DiaChi";
            var pattern = $"%{diachi ?? string.Empty}%";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@DiaChi", SqlDbType.NVarChar, pattern, 255));
            _table = dt; // NEW: đồng bộ
            return dt;
        }

        // Tìm theo họ tên (LIKE)
        public DataTable TimHoTen(string hoten)
        {
            // CHANGED: Dùng SELECT_ALL
            const string sql = SELECT_ALL + " WHERE HO_TEN LIKE @HoTen";
            var pattern = $"%{hoten ?? string.Empty}%";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@HoTen", SqlDbType.NVarChar, pattern, 200));
            _table = dt; // NEW: đồng bộ
            return dt;
        }

        /* ==================== CRUD trực tiếp (Giữ lại) ==================== */

        // Thêm mới
        // CHANGED: Đổi return int -> bool
        public bool Insert(NhaCungCap ncc)
        {
            const string sql = @"
                INSERT INTO NHA_CUNG_CAP(ID, HO_TEN, DIEN_THOAI, DIA_CHI)
                VALUES (@Id, @HoTen, @DienThoai, @DiaChi)";
            // CHANGED: return > 0
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, ncc.Id, 50),
                _db.P("@HoTen", SqlDbType.NVarChar, ncc.HoTen, 200),
                _db.P("@DienThoai", SqlDbType.NVarChar, ncc.DienThoai, 50),
                _db.P("@DiaChi", SqlDbType.NVarChar, ncc.DiaChi, 255)) > 0;
        }

        // Cập nhật
        // CHANGED: Đổi return int -> bool
        public bool Update(NhaCungCap ncc)
        {
            const string sql = @"
                UPDATE NHA_CUNG_CAP
                   SET HO_TEN=@HoTen, DIEN_THOAI=@DienThoai, DIA_CHI=@DiaChi
                 WHERE ID=@Id";
            // CHANGED: return > 0
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@HoTen", SqlDbType.NVarChar, ncc.HoTen, 200),
                _db.P("@DienThoai", SqlDbType.NVarChar, ncc.DienThoai, 50),
                _db.P("@DiaChi", SqlDbType.NVarChar, ncc.DiaChi, 255),
                _db.P("@Id", SqlDbType.NVarChar, ncc.Id, 50)) > 0;
        }

        // Xóa
        // CHANGED: Đổi return int -> bool
        public bool Delete(string id)
        {
            const string sql = "DELETE FROM NHA_CUNG_CAP WHERE ID=@Id";
            // CHANGED: return > 0
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@Id", SqlDbType.NVarChar, id, 50)) > 0;
        }

        /* ==================== DataTable pattern (NEW) ==================== */

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

        public bool Save()
        {
            EnsureSchema();

            // Gọi helper tĩnh để thực hiện logic Save
            return DataAccessHelper.PerformSave(
                _table,             // DataTable nội bộ
                _nhaCungCapRules,   // Danh sách quy tắc
                this.CreateAdapter, // Phương thức tạo Adapter
                _db                 // Instance DbClient
            );
        }

        /// <summary>
        /// Danh sách các quy tắc kiểm tra hợp lệ cho bảng NHA_CUNG_CAP
        /// </summary>
        private static readonly List<ValidationRule> _nhaCungCapRules = new List<ValidationRule>
        {
            new ValidationRule("ID", ValidationType.NotEmpty, "Mã nhà cung cấp không được để trống."),
            new ValidationRule("HO_TEN", ValidationType.NotEmpty, "Tên nhà cung cấp không được để trống.")
            // Các cột DIEN_THOAI, DIA_CHI được phép trống
        };
    }
}