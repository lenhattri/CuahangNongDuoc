// DAL/DataLayer/LyDoChiFactory.cs
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;

namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory
    {
        private readonly DbClient _db = DbClient.Instance;
        private DataTable _table;  // DataTable nội bộ cho pattern Save()

        private const string SELECT_ALL = "SELECT ID, LY_DO FROM LY_DO_CHI";

        /* Helpers */
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
            _ = new SqlCommandBuilder(da);
            return da;
        }

        /* API */
        public DataTable DanhsachLyDo()
        {
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL, CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable("LY_DO_CHI");
                da.Fill(dt);
                _table = dt;
                return dt;
            }
        }

        public DataTable LayLyDoChi(long id)
        {
            const string sql = "SELECT ID, LY_DO FROM LY_DO_CHI WHERE ID = @id";
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, sql, CommandType.Text, null, 30,
                       _db.P("@id", SqlDbType.BigInt, id)))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable("LY_DO_CHI");
                da.Fill(dt);
                _table = dt;
                return dt;
            }
        }

        public bool Save()
        {
            EnsureSchema();
            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))
            {
                return da.Update(_table) > 0;
            }
        }

    }
}
