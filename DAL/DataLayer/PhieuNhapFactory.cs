using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuNhapFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
           SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP WHERE ID='-1'");
            m_Ds.Load(cmd);

        }

        public DataTable DanhsachPhieuNhap()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable TimPhieuNhap(String maNCC, DateTime dt)
        {
            String sql = "SELECT * FROM PHIEU_NHAP WHERE NGAY_NHAP = @ngay AND ID_NHA_CUNG_CAP = @ncc";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.Add("ngay", SqlDbType.Date).Value = dt;
            cmd.Parameters.Add("ncc", SqlDbType.VarChar).Value = maNCC;
            
            m_Ds.Load(cmd);

            return m_Ds;
        }


        public DataTable LayPhieuNhap(String id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP WHERE ID = @id");
            cmd.Parameters.Add("id", SqlDbType.VarChar,50).Value = id;
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

        internal void Save()
        {
            throw new NotImplementedException();
        }
    }
}
