using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Utils.Functions
{
    public static class SqlHelper
    {
        private static readonly string _cs =
            ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable ExecuteDataTable(string sql, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                adapter.Fill(dt);
            }
            return dt;
        }
    }
}
