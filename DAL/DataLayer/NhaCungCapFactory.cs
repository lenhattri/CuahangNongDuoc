using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class NhaCungCapDAL : INhaCungCapDAL
    {
        private readonly DbClient _db = DbClient.Instance;
        private const string TABLE = "[dbo].[NHA_CUNG_CAP]";

        public DataTable DanhsachNCC()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NHA_CUNG_CAP");
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable TimDiaChi(String diachi)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NHA_CUNG_CAP WHERE DIA_CHI LIKE '%' + @diachi + '%' ");
            cmd.Parameters.Add("diachi", SqlDbType.VarChar).Value = diachi;
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable TimHoTen(String hoten)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NHA_CUNG_CAP WHERE HO_TEN LIKE '%' + @hoten + '%' ");
            cmd.Parameters.Add("hoten", SqlDbType.VarChar).Value = hoten;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayNCC(String id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NHA_CUNG_CAP WHERE ID = @id");
            cmd.Parameters.Add("id", SqlDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }
        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }
        public bool Save(SqlCommand cmd)
        {
            return m_Ds.ExecuteNoneQuery(cmd) > 0;
        }
    }
}
