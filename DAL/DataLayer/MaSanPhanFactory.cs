// DAL/DataLayer/MaSanPhanFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;   

namespace CuahangNongduoc.DataLayer
{
    public class MaSanPhanFactory : IMaSanPhanFactory
    {
        private readonly DbClient _db = DbClient.Instance;   // CHANGED: thay thế DataService
        private DataTable _table;                             // NEW: giữ DataTable đang thao tác để Save()

        public void LoadSchema()
        {
            // CHANGED: lấy schema rỗng bằng DbClient
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, "SELECT * FROM MA_SAN_PHAM WHERE ID = '-1'", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("MA_SAN_PHAM");
                da.FillSchema(_table, SchemaType.Source);     // CHANGED: chỉ lấy schema
            }
        }

        private void EnsureSchema()                           // NEW
        {
            if (_table != null) return;
            LoadSchema();
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn) // NEW: phục vụ Save()
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, "SELECT * FROM MA_SAN_PHAM", CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // sinh Insert/Update/Delete
            return da;
        }

        /* ===================== QUERIES ===================== */

        public DataTable DanhsachMaSanPham(string sp)
        {
            const string sql = "SELECT * FROM MA_SAN_PHAM WHERE ID_SAN_PHAM = @id AND SO_LUONG > 0";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, sp, 50));            // CHANGED: tham số hóa qua DbClient
            _table = dt;                                              // NEW: đồng bộ để NewRow/Add/Save dùng chung schema
            return dt;
        }

        public DataTable DanhsachMaSanPhamFIFO(string sp)
        {
            const string sql = "SELECT TOP 1 ID FROM MA_SAN_PHAM WHERE ID_SAN_PHAM = @id AND SO_LUONG > 0 ORDER BY NGAY_NHAP ASC";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, sp, 50));            // CHANGED: tham số hóa qua DbClient
            _table = dt;                                              // NEW: đồng bộ để NewRow/Add/Save dùng chung schema
            return dt;
        }

        public DataTable DanhsachChiTiet(string sp)
        {
            const string sql = "SELECT * FROM MA_SAN_PHAM WHERE ID_PHIEU_NHAP = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, sp, 50));            // CHANGED
            _table = dt;                                              // NEW
            return dt;
        }

        public DataTable LaySanPham(string idMaSanPham)
        {
            const string sql = @"
                SELECT SP.*
                FROM SAN_PHAM SP
                INNER JOIN MA_SAN_PHAM MSP ON SP.ID = MSP.ID_SAN_PHAM
                WHERE MSP.ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, idMaSanPham, 50));   // CHANGED
            // Không set _table ở đây vì Save() dành cho MA_SAN_PHAM
            return dt;
        }

        public DataTable LayMaSanPham(string idMaSanPham)
        {
            const string sql = @"SELECT msp.*,sp.TEN_SAN_PHAM 
                                FROM MA_SAN_PHAM msp
                                LEFT JOIN SAN_PHAM sp ON msp.ID_SAN_PHAM = sp.ID
                                WHERE msp.ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, idMaSanPham, 50));   // CHANGED
            _table = dt;                                              // NEW
            return dt;
        }

        public DataTable DanhsachMaSanPhamHetHan(DateTime dt)
        {
            const string sql = "SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0 AND NGAY_HET_HAN <= @ngay";
            var res = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@ngay", SqlDbType.Date, dt.Date));            // CHANGED
            _table = res;                                            // NEW
            return res;
        }

        public DataTable DanhsachMaSanPham()
        {
            const string sql = "SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text);     // CHANGED
            _table = dt;                                              // NEW
            return dt;
        }

        public void CapNhatSoLuong(string masp, int so_luong)
        {
            // CHANGED: bỏ OleDb & DataService; dùng DbClient.ExecuteNonQuery
            var db = DbClient.Instance;
            const string sql = "UPDATE MA_SAN_PHAM SET SO_LUONG = SO_LUONG + @so WHERE ID = @id";
            db.ExecuteNonQuery(sql, CommandType.Text,
                db.P("@so", SqlDbType.Int, so_luong),
                db.P("@id", SqlDbType.NVarChar, masp, 50));
        }

        /* ===================== DATATABLE PATTERN (NewRow/Add/Save) ===================== */

        public DataRow NewRow()
        {
            EnsureSchema();                          // CHANGED: thay cho m_Ds.NewRow()
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();                          // CHANGED
            _table.Rows.Add(row);
        }

        public bool Save()
        {
            EnsureSchema();                          // CHANGED
            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))       // CHANGED: dùng DataAdapter + CommandBuilder
            {
                return da.Update(_table) > 0;
            }
        }

        public int LaySoLuongTon(string maSP)
        {
            var db = DbClient.Instance;
            const string sql = "SELECT SO_LUONG FROM MA_SAN_PHAM WHERE ID = @id";
            return db.ExecuteScalar<int>(sql, CommandType.Text,
                db.P("@id", SqlDbType.NVarChar, maSP, 50));
        }
    }
}
