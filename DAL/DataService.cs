using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace CuahangNongduoc
{
    public class DataService : DataTable
    {
        private static SqlConnection m_Connection;
        private static string m_ConnectString =
            @"Data Source=localhost\SQLEXPRESS;Initial Catalog=cuahang;Integrated Security=True";

        public DataService() { }

        public static bool OpenConnection()
        {
            try
            {
                if (m_Connection == null)
                    m_Connection = new SqlConnection(m_ConnectString);

                if (m_Connection.State == ConnectionState.Closed)
                    m_Connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CloseConnection()
        {
            if (m_Connection != null && m_Connection.State != ConnectionState.Closed)
                m_Connection.Close();
        }

        /// <summary>
        /// Load dữ liệu vào DataTable này
        /// </summary>
        internal void Load(SqlCommand cmd)
        {
            OpenConnection();
            cmd.Connection = m_Connection;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            this.Clear();
            da.Fill(this);
        }

        /// <summary>
        /// Thực thi NonQuery (INSERT, UPDATE, DELETE)
        /// </summary>
        internal int ExecuteNoneQuery(SqlCommand cmd)
        {
            int result = 0;
            SqlTransaction tr = null;

            try
            {
                tr = m_Connection.BeginTransaction();
                cmd.Connection = m_Connection;
                cmd.Transaction = tr;

                result = cmd.ExecuteNonQuery();
                this.AcceptChanges();

                tr.Commit();
            }
            catch
            {
                if (tr != null) tr.Rollback();
                throw;
            }
            return result;
        }

        /// <summary>
        /// Thực thi Scalar (trả về 1 giá trị duy nhất)
        /// </summary>
        internal object ExecuteScalar(SqlCommand cmd)
        {
            object result = null;
            SqlTransaction tr = null;

            try
            {
                tr = m_Connection.BeginTransaction();
                cmd.Connection = m_Connection;
                cmd.Transaction = tr;

                result = cmd.ExecuteScalar();
                this.AcceptChanges();

                tr.Commit();

                if (result == DBNull.Value)
                    result = null;
            }
            catch
            {
                if (tr != null) tr.Rollback();
                throw;
            }
            return result;
        }

        //internal void Load(OleDbCommand cmd)
        //{
        //    throw new NotImplementedException();
        //}

        //internal object ExecuteScalar(OleDbCommand cmd)
        /*{
            throw new NotImplementedException();
        }

        internal int ExecuteNoneQuery(OleDbCommand cmd)
        {
            throw new NotImplementedException();
        }

        internal int ExecuteNoneQuery()
        {
            throw new NotImplementedException();
        }*/
    }
}
