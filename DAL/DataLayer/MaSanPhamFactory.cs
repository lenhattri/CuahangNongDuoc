using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class MaSanPhamFactory : IMaSanPhamFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE ID = '-1'");
            m_Ds.Load(cmd);
        }

        public DataTable DanhsachMaSanPham(string sp)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_SAN_PHAM=@id AND SO_LUONG > 0");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachChiTiet(string sp)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_PHIEU_NHAP=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LaySanPham(string idMaSanPham)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT SP.* FROM SAN_PHAM SP INNER JOIN MA_SAN_PHAM MSP ON SP.ID = MSP.ID_SAN_PHAM WHERE MSP.ID = @id"
            );
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayMaSanPham(string idMaSanPham)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM MSP WHERE MSP.ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable DanhsachMaSanPhamHetHan(DateTime dt)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0 AND NGAY_HET_HAN <= @ngay");
            cmd.Parameters.Add("@ngay", SqlDbType.DateTime).Value = dt;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachMaSanPham()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public  void CapNhatSoLuong(string masp, int so_luong)
        {
            DataService ds = new DataService();
            SqlCommand cmd = new SqlCommand("UPDATE MA_SAN_PHAM SET SO_LUONG = SO_LUONG + @so WHERE ID = @id");
            cmd.Parameters.Add("@so", SqlDbType.Int).Value = so_luong;
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = masp;
            ds.ExecuteNoneQuery(cmd);
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
            SqlCommand cmd1 = new SqlCommand("SELECT * FROM SAN_PHAM");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(m_Ds); // Cập nhật thay đổi trong DataTable xuống DB
            return true;
        }
    }
}
