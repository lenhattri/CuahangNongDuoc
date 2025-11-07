
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.DAL.Infrastructure
{
    public sealed class DbClient
    {
        // test commit
        private static readonly Lazy<DbClient> _lazy = new Lazy<DbClient>(() => new DbClient());
        public static DbClient Instance => _lazy.Value;
        private readonly string _cs;
        private DbClient()
        {
            var cs = ConfigurationManager.ConnectionStrings["ConnStr"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                throw new InvalidOperationException("Missing connectionStrings: ConnStr in App.config");
            _cs = cs.ConnectionString;
        }


        public SqlConnection Open()
        {
            var cn = new SqlConnection(_cs);
            cn.Open();
            return cn;
        }


        public SqlCommand Cmd(SqlConnection cn, string sql, CommandType type = CommandType.Text, SqlTransaction tx = null,
        int timeoutSeconds = 30, params SqlParameter[] parameters)
        {
            var cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = type;
            cmd.Transaction = tx;
            cmd.CommandTimeout = timeoutSeconds;
            if (parameters != null)
                foreach (var p in parameters) cmd.Parameters.Add(p);
            return cmd;
        }

        public SqlParameter P(string name, SqlDbType type, object value, int size = 0, byte precision = 0, byte scale = 0)
        {
            var p = new SqlParameter(name, type) { Value = value ?? DBNull.Value };
            if (size > 0) p.Size = size;
            if (precision > 0) p.Precision = precision;
            if (scale > 0) p.Scale = scale;
            return p;
        }


        // Convenience for decimal (sets precision/scale by default to 18,2 if not provided)
        public SqlParameter PDec(string name, object value, byte precision = 18, byte scale = 2)
        => P(name, SqlDbType.Decimal, value, 0, precision, scale);


        public int ExecuteNonQuery(string sql, CommandType type = CommandType.Text, params SqlParameter[] ps)
        {
            using (var cn = Open())
            using (var cmd = Cmd(cn, sql, type, null, 30, ps))
                return cmd.ExecuteNonQuery();
        }


        public T ExecuteScalar<T>(string sql, CommandType type = CommandType.Text, params SqlParameter[] ps)
        {
            using (var cn = Open())
            using (var cmd = Cmd(cn, sql, type, null, 30, ps))
            {
                var obj = cmd.ExecuteScalar();
                if (obj == null || obj == DBNull.Value) return default(T);
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }


        public DataTable ExecuteDataTable(string sql, CommandType type = CommandType.Text, params SqlParameter[] ps)
        {
            using (var cn = Open())
            using (var cmd = Cmd(cn, sql, type, null, 30, ps))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public int InTx(Func<SqlConnection, SqlTransaction, int> action, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            using (var cn = Open())
            using (var tx = cn.BeginTransaction(isolation))
            {
                try { var n = action(cn, tx); tx.Commit(); return n; }
                catch { try { tx.Rollback(); } catch { } throw; }
            }
        }
    }
}
