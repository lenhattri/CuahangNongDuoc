using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

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

        // 📦 Load dữ liệu từ SqlCommand (SELECT)
        public void Load(SqlCommand cmd)
        {
            cmd.Connection = m_Connection;
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                this.Clear();
                adapter.Fill(this);
            }
        }

        // ⚙️ Thực thi câu lệnh INSERT, UPDATE, DELETE
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

        // 🧮 Thực thi câu lệnh trả về 1 giá trị (SELECT COUNT, MAX,...)
        public object ExecuteScalar(SqlCommand cmd)
        {
            object result = null;
            try
            {
                cmd.Connection = m_Connection;
                if (m_Connection.State == ConnectionState.Closed)
                    m_Connection.Open();

                result = cmd.ExecuteScalar();
            }
            finally
            {
                if (m_Connection.State == ConnectionState.Open)
                    m_Connection.Close();
            }
            return result;
        }

        // 🔗 Mở kết nối (nếu cần thủ công)
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

        // ❌ Loại bỏ hoàn toàn các hàm OleDb cũ vì không dùng nữa
    }
}
