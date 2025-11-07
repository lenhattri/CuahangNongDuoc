// DAL/DataLayer/SanPhamFactory.cs
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.Infrastructure;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class SanPhamFactory
    {
        private readonly DbClient _db = DbClient.Instance;  // CHANGED:
        private DataTable _table;                           // NEW: DataTable nội bộ cho pattern NewRow/Add/Save

        private const string SELECT_ALL = "SELECT * FROM SAN_PHAM";

        /* ==================== Helpers ==================== */

        private void EnsureSchema()                         // NEW
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("SAN_PHAM");
                da.FillSchema(_table, SchemaType.Source);   // lấy schema rỗng rõ ràng
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn) // NEW: phục vụ Save()
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da);                  // auto sinh Insert/Update/Delete
            return da;
        }

        /* ==================== SELECTs ==================== */

        public DataTable DanhsachSanPham()
        {
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);   // CHANGED
            _table = dt;                                                   // đồng bộ để Save() hoạt động
            return dt;
        }

        public DataTable TimMaSanPham(string id)
        {
            const string sql = "SELECT * FROM SAN_PHAM WHERE ID LIKE @id"; // CHANGED: LIKE @param
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, $"%{id ?? string.Empty}%", 50));
            _table = dt;
            return dt;
        }

        public DataTable TimTenSanPham(string ten)
        {
            const string sql = "SELECT * FROM SAN_PHAM WHERE TEN_SAN_PHAM LIKE @ten"; // CHANGED
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@ten", SqlDbType.NVarChar, $"%{ten ?? string.Empty}%", 200));
            _table = dt;
            return dt;
        }

        public DataTable LaySanPham(string id)
        {
            const string sql = "SELECT * FROM SAN_PHAM WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, id, 50));                  // CHANGED
            _table = dt;
            return dt;
        }

        public DataTable LaySoLuongTon()
        {
            const string sql = @"
                SELECT SP.ID, SP.TEN_SAN_PHAM, SP.DON_GIA_NHAP, SP.GIA_BAN_SI, SP.GIA_BAN_LE,
                       SP.ID_DON_VI_TINH, SP.SO_LUONG, SUM(MA.SO_LUONG) AS SO_LUONG_TON
                FROM SAN_PHAM SP
                INNER JOIN MA_SAN_PHAM MA ON SP.ID = MA.ID_SAN_PHAM
                GROUP BY SP.ID, SP.TEN_SAN_PHAM, SP.DON_GIA_NHAP, SP.GIA_BAN_SI, SP.GIA_BAN_LE,
                         SP.ID_DON_VI_TINH, SP.SO_LUONG";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text);           // CHANGED
            // Không buộc _table ở đây vì Save() áp dụng cho SAN_PHAM gốc
            return dt;
        }

        /* ==================== CRUD trực tiếp ==================== */

        public bool Insert(SanPham sp)
        {
            const string sql = @"
                INSERT INTO SAN_PHAM
                (ID, TEN_SAN_PHAM, ID_DON_VI_TINH, SO_LUONG, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE)
                VALUES(@id, @ten, @id_dvt, @soluong, @gianhap, @giabansi, @giabanle)";
            // Giữ kiểu long như code hiện tại (nếu DB là DECIMAL nên cân nhắc decimal về lâu dài)
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, sp.Id, 50),
                _db.P("@ten", SqlDbType.NVarChar, sp.TenSanPham, 200),
                _db.P("@id_dvt", SqlDbType.Int, sp.DonViTinh.Id),
                _db.P("@soluong", SqlDbType.Int, sp.SoLuong),
                _db.P("@gianhap", SqlDbType.BigInt, sp.DonGiaNhap),
                _db.P("@giabansi", SqlDbType.BigInt, sp.GiaBanSi),
                _db.P("@giabanle", SqlDbType.BigInt, sp.GiaBanLe)
            ) > 0;
        }

        public bool Update(SanPham sp)
        {
            const string sql = @"
                UPDATE SAN_PHAM
                SET TEN_SAN_PHAM=@ten, ID_DON_VI_TINH=@id_dvt, SO_LUONG=@soluong,
                    DON_GIA_NHAP=@gianhap, GIA_BAN_SI=@giabansi, GIA_BAN_LE=@giabanle
                WHERE ID=@id";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, sp.Id, 50),
                _db.P("@ten", SqlDbType.NVarChar, sp.TenSanPham, 200),
                _db.P("@id_dvt", SqlDbType.Int, sp.DonViTinh.Id),
                _db.P("@soluong", SqlDbType.Int, sp.SoLuong),
                _db.P("@gianhap", SqlDbType.BigInt, sp.DonGiaNhap),
                _db.P("@giabansi", SqlDbType.BigInt, sp.GiaBanSi),
                _db.P("@giabanle", SqlDbType.BigInt, sp.GiaBanLe)
            ) > 0;
        }

        public bool Delete(string id)
        {
            const string sql = "DELETE FROM SAN_PHAM WHERE ID=@id";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, id, 50)) > 0;              // CHANGED
        }

        /* ==================== DataTable pattern (giữ API cũ) ==================== */

        public DataRow NewRow()
        {
            EnsureSchema();                      // CHANGED
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();                      // CHANGED
            _table.Rows.Add(row);
        }

        public bool Save()
        {
            // CHANGED: thay toàn bộ DataService + tự mở connection bằng DbClient + DataAdapter
            EnsureSchema();

            return DataAccessHelper.PerformSave(
                _table,             // DataTable nội bộ
                _sanPhamRules,      // Danh sách quy tắc
                this.CreateAdapter, // Phương thức tạo Adapter
                _db                 // Instance DbClient
            );
        }

        // REMOVED: internal void Save() { throw new NotImplementedException(); }
        // CHANGED: xoá method trùng tên gây lỗi "already defines a member called 'Save'"

        private static readonly List<ValidationRule> _sanPhamRules = new List<ValidationRule>
        {
            new ValidationRule("ID", ValidationType.NotEmpty, "Mã sản phẩm không được để trống."),
            new ValidationRule("TEN_SAN_PHAM", ValidationType.NotEmpty, "Tên sản phẩm không được để trống."),
            new ValidationRule("ID_DON_VI_TINH", ValidationType.NotNull, "Chưa chọn đơn vị tính."),
            
            // Các quy tắc cho số
            new ValidationRule("SO_LUONG", ValidationType.NotNegativeInt, "Số lượng không được âm."),
            new ValidationRule("SO_LUONG", ValidationType.NotZero, "Số lượng không được bằng 0."),
            new ValidationRule("DON_GIA_NHAP", ValidationType.NotNegativeLong, "Giá nhập không được âm."),
            new ValidationRule("DON_GIA_NHAP", ValidationType.NotZero, "Số lượng không được bằng 0."),
            new ValidationRule("GIA_BAN_SI", ValidationType.NotNegativeLong, "Giá bán sỉ không được âm."),
            new ValidationRule("GIA_BAN_SI", ValidationType.NotZero, "Số lượng không được bằng 0."),
            new ValidationRule("GIA_BAN_LE", ValidationType.NotNegativeLong, "Giá bán lẻ không được âm."),
            new ValidationRule("GIA_BAN_LE", ValidationType.NotZero, "Số lượng không được bằng 0."),
        };
    }
}
