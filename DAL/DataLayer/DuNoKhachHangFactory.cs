using System;
using System.Configuration;  
using System.Data; 
using System.Data.SqlClient; 
using CuahangNongduoc.DAL.Infrastructure; // CHANGED: dùng DbClient (singleton)

namespace CuahangNongduoc.DataLayer
{
    public class DuNoKhachHangDAL : IDuNoKhachHangDAL
    {
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table;
        public static IDuNoKhachHangDAL Create()
        {
            return new DuNoKhachHangDAL();
        }
        private const string SELECT_ALL =
            "SELECT ID_KHACH_HANG, THANG, NAM, DAU_KY, PHAT_SINH, DA_TRA, CUOI_KY FROM DU_NO_KH";

        /* ========================= Private Helpers ========================= */

        private SqlDataAdapter CreateAdapter(SqlConnection conn)
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(conn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // auto sinh CRUD commands
            return da;
        }

        private void EnsureSchema()
        {
            if (_table == null)
                LoadSchema();
        }

        /* ========================= Public API ========================= */

        public void LoadSchema()
        {
            using (var conn = _db.Open())
            using (var cmd = _db.Cmd(conn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("DU_NO_KH");
                da.FillSchema(_table, SchemaType.Source);
            }
        }

        public DataTable DanhsachDuNo(int thang, int nam)
        {
            const string sql = SELECT_ALL + " WHERE THANG=@thang AND NAM=@nam";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@thang", SqlDbType.Int, thang),
                _db.P("@nam", SqlDbType.Int, nam));

            _table = dt;
            return dt;
        }

        public DataTable LayDuNoKhachHang(string kh, int thang, int nam)
        {
            const string sql = SELECT_ALL +
                " WHERE ID_KHACH_HANG=@kh AND THANG=@thang AND NAM=@nam";

            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@kh", SqlDbType.NVarChar, kh, 50),
                _db.P("@thang", SqlDbType.Int, thang),
                _db.P("@nam", SqlDbType.Int, nam));

            _table = dt;
            return dt;
        }

        public long LayDuNo(string kh, int thang, int nam)
        {
            const string sql =
                "SELECT ISNULL(CUOI_KY, 0) FROM DU_NO_KH WHERE ID_KHACH_HANG=@kh AND THANG=@thang AND NAM=@nam";

            return _db.ExecuteScalar<long>(sql, CommandType.Text,
                _db.P("@kh", SqlDbType.NVarChar, kh, 50),
                _db.P("@thang", SqlDbType.Int, thang),
                _db.P("@nam", SqlDbType.Int, nam));
        }

        public void Clear(int thang, int nam)
        {
            const string sql = "DELETE FROM DU_NO_KH WHERE THANG=@thang AND NAM=@nam";
            _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@thang", SqlDbType.Int, thang),
                _db.P("@nam", SqlDbType.Int, nam));
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

        public bool Save()
        {
            EnsureSchema();
            using (var conn = _db.Open())
            using (var da = CreateAdapter(conn))
            {
                return da.Update(_table) > 0;
            }
        }
    }
}

