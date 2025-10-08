using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory
    {
        DataService m_Ds = new DataService();

        public DataTable DanhsachLyDo()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM LY_DO_CHI");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayLyDoChi(long id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM LY_DO_CHI WHERE ID = " + id);
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public bool Save(SqlCommand cmd)
        {
            SqlCommand cmd1= new SqlCommand("SELECT * FROM LY_DO_CHI");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(m_Ds);
            return true;
        }
    }
}
