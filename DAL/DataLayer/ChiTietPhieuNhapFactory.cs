// DAL/DataLayer/ChiTietPhieuNhapFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure; // CHANGED: dùng DbClient thay cho DataService

namespace CuahangNongduoc.DataLayer
{
    public class ChiTietPhieuNhapFactory
    {
        private readonly DbClient _db = DbClient.Instance;   // NEW
        private DataTable _table;                             // NEW: thay cho DataService m_Ds

        /* ================== SCHEMA ================== */
        public void LoadSchema()
        {
            // CHANGED: Lấy schema rỗng (ID_PHIEU_NHAP = '-1') nhưng dùng DbClient
            const string sql = "SELECT * FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = '-1'";
            _table = _db.ExecuteDataTable(sql, CommandType.Text);
        }

        private void EnsureSchema()                           // NEW
        {
            if (_table != null) return;
            LoadSchema();
        }

        /* ================== SELECT ================== */
        public DataTable LayChiTietPhieuNhap(string id)
        {
            // CHANGED: dùng DbClient + tham số hóa
            const string sql = "SELECT * FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.VarChar, id, 50));
            _table = dt; // đồng bộ để NewRow/Add/Save dùng cùng schema
            return dt;
        }

        /* ================== DELETE ================== */
        public int XoaChiTietPhieuNhap(string id)
        {
            // CHANGED: dùng DbClient
            const string sql = "DELETE FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = @id";
            return _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@id", SqlDbType.VarChar, id, 50));
        }

        /* ================== DATATABLE PATTERN ================== */
        public DataRow NewRow()
        {
            EnsureSchema();                                   // NEW
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();                                   // NEW
            if (!ReferenceEquals(row.Table, _table))
            {
                var clone = _table.NewRow();
                foreach (DataColumn c in _table.Columns)
                    clone[c.ColumnName] = row.Table.Columns.Contains(c.ColumnName)
                        ? row[c.ColumnName] : DBNull.Value;
                _table.Rows.Add(clone);
            }
            else
            {
                _table.Rows.Add(row);
            }
        }

        public bool Save()
        {
            // CHANGED: thay DataService.ExecuteNoneQuery() 
            EnsureSchema();
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, "SELECT * FROM CHI_TIET_PHIEU_NHAP", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            using (var cb = new SqlCommandBuilder(da))
            {
                da.MissingSchemaAction = MissingSchemaAction.AddWithKey; // NEW: để sinh CRUD
                var n = da.Update(_table);
                return n > 0;
            }
        }
    }
}
