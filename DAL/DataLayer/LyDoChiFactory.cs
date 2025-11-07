using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory : ILyDoChiFactory
    {
        DataService m_Ds = new DataService();

        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        /// <summary>
        /// Retrieves all reasons for expenditure.
        /// </summary>
        /// <returns>A DataTable containing all records.</returns>
       /* public DataTable DanhsachLyDo()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM LY_DO_CHI");
            m_Ds.Load(cmd);

            return dataTable;
        }*/

        public DataTable LayLyDoChi(long id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM LY_DO_CHI WHERE ID = " + id);
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public bool Save(SqlCommand cmd)
        {
            SqlCommand cmd1 = new SqlCommand("SELECT * FROM LY_DO_CHI");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(m_Ds);
            return true;
        }
    }
}
