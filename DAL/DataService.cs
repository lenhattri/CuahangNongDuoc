using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;

namespace CuahangNongduoc
{
    public class DataService : DataTable
    {
        private static SqlConnection m_Connection;

        static DataService()
        {
            // Đọc chuỗi kết nối từ app.config
            string connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            m_Connection = new SqlConnection(connectionString);
        }

        // Load dữ liệu từ SqlCommand
        public void Load(SqlCommand cmd)
        {
            cmd.Connection = m_Connection;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            this.Clear();
            adapter.Fill(this);
        }

        // Thực thi câu lệnh không trả về (INSERT, UPDATE, DELETE)
        public int ExecuteNoneQuery(SqlCommand cmd)
        {
            int result = 0;
            try
            {
                cmd.Connection = m_Connection;
                if (m_Connection.State == ConnectionState.Closed)
                    m_Connection.Open();

                result = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (m_Connection.State == ConnectionState.Open)
                    m_Connection.Close();
            }
            return result;
        }

        // Hàm này không cần thiết nếu không dùng DataAdapter.Update
        // nhưng giữ lại nếu bạn có các thao tác cập nhật DataTable
        public int ExecuteNoneQuery(System.Data.OleDb.OleDbCommand cmd)
        {
            int result = 0;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                result = adapter.Update(this);
            }
            catch
            {
                throw;
            }
            return result;
        }

        internal void Load1(SqlCommand cmd)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                this.Clear(); // xóa dữ liệu cũ trong DataTable nếu có
                adapter.Fill(this);
            }
        }


        internal int ExecuteNoneQuery()
        {
            throw new NotImplementedException();
        }

        internal object ExecuteScalar(OleDbCommand cmd)
        {
            throw new NotImplementedException();
        }

        internal static void OpenConnection()
        {
            if (m_Connection == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
                m_Connection = new SqlConnection(connectionString);
            }

            if (m_Connection.State == ConnectionState.Closed)
            {
                m_Connection.Open();
            }
        }

        internal void Load1(OleDbCommand cmd)
        { if (m_Connection == null) {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                this.Clear(); // xóa dữ liệu cũ trong DataTable nếu có
                adapter.Fill(this);
            }
        }
    }
}
