using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class DuNoKhachHangDAL
    {
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        // DataTable trung tâm, tương tự m_Ds trong bản cũ
        private DataTable _table;

        private const string SELECT_ALL =
            "SELECT ID_KHACH_HANG, THANG, NAM, DAU_KY, PHAT_SINH, DA_TRA, CUOI_KY FROM DU_NO_KH";

        /* ========================= Helpers ========================= */

        private SqlDataAdapter CreateAdapter(SqlConnection conn)
        {
            var da = new SqlDataAdapter(SELECT_ALL, conn)
            {
                // Lấy đủ schema/PK để SqlCommandBuilder sinh lệnh CRUD
                MissingSchemaAction = MissingSchemaAction.AddWithKey
            };
            var cb = new SqlCommandBuilder(da);
            // (cb sẽ tự gán da.InsertCommand/UpdateCommand/DeleteCommand)
            return da;
        }

        private void EnsureSchema()
        {
            if (_table != null) return;
            LoadSchema();
        }

        /* ========================= API tương đương ========================= */

        public void LoadSchema()
        {
            // Tương đương: SELECT * WHERE '-1' để lấy schema rỗng
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(SELECT_ALL + " WHERE 1=0", conn))
            {
                _table = new DataTable("DU_NO_KH");
                da.Fill(_table); // fill schema (rỗng dữ liệu)
            }
        }

        public DataTable DanhsachDuNo(int thang, int nam)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                SELECT_ALL + " WHERE THANG=@thang AND NAM=@nam", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;

                var dt = new DataTable("DU_NO_KH");
                da.Fill(dt);

                // Đồng bộ _table để NewRow()/Add()/Save() dùng cùng schema
                _table = dt;
                return dt;
            }
        }

        public DataTable LayDuNoKhachHang(string kh, int thang, int nam)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                SELECT_ALL + " WHERE ID_KHACH_HANG=@kh AND THANG=@thang AND NAM=@nam", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
                cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;

                var dt = new DataTable("DU_NO_KH");
                da.Fill(dt);

                _table = dt; // giữ schema cho NewRow()/Add()/Save()
                return dt;
            }
        }

        public static long LayDuNo(string kh, int thang, int nam)
        {
            var cs = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(
                "SELECT CUOI_KY FROM DU_NO_KH WHERE ID_KHACH_HANG=@kh AND THANG=@thang AND NAM=@nam", conn))
            {
                cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
                cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;

                conn.Open();
                object obj = cmd.ExecuteScalar();
                return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
            }
        }

        public void Clear(int thang, int nam)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "DELETE FROM DU_NO_KH WHERE THANG=@thang AND NAM=@nam", conn))
            {
                cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
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
            using (var conn = new SqlConnection(_cs))
            using (var da = CreateAdapter(conn))
            {
                return da.Update(_table) > 0;
            }
        }
    }
}

