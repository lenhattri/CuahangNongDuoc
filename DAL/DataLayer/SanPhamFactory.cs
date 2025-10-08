using CuahangNongduoc.BusinessObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CuahangNongduoc.DataLayer
{
    public class SanPhamFactory
    {
        DataService m_Ds = new DataService();

        public DataTable DanhsachSanPham()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM SAN_PHAM");

            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable TimMaSanPham(string id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM SAN_PHAM WHERE ID LIKE '%' + @id + '%'");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable TimTenSanPham(string ten)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM SAN_PHAM WHERE TEN_SAN_PHAM LIKE '%' + @ten + '%'");
            cmd.Parameters.Add("@ten", SqlDbType.VarChar, 100).Value = ten; // giả sử TEN_SAN_PHAM max 100 ký tự
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LaySanPham(string id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM SAN_PHAM WHERE ID = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LaySoLuongTon()
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT SP.ID, SP.TEN_SAN_PHAM, SP.DON_GIA_NHAP, SP.GIA_BAN_SI, SP.GIA_BAN_LE, " +
                "SP.ID_DON_VI_TINH, SP.SO_LUONG, SUM(MA.SO_LUONG) AS SO_LUONG_TON " +
                "FROM SAN_PHAM SP INNER JOIN MA_SAN_PHAM MA ON SP.ID = MA.ID_SAN_PHAM " +
                "GROUP BY SP.ID, SP.TEN_SAN_PHAM, SP.DON_GIA_NHAP, SP.GIA_BAN_SI, SP.GIA_BAN_LE, SP.ID_DON_VI_TINH, SP.SO_LUONG"
            );

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
            SqlCommand cmd1 = new SqlCommand("SELECT * FROM SAN_PHAM");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(m_Ds); // Cập nhật thay đổi trong DataTable xuống DB
            return true;
        }

        public bool Insert(SanPham sp)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO SAN_PHAM (ID, TEN_SAN_PHAM, ID_DON_VI_TINH, SO_LUONG, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE) " +
                "VALUES (@id, @ten, @id_dvt, @soluong, @gianhap, @giabansi, @giabanle)"
            );
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = sp.Id;
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = sp.TenSanPham;
            cmd.Parameters.Add("@id_dvt", SqlDbType.Int).Value = sp.DonViTinh.Id;
            cmd.Parameters.Add("@soluong", SqlDbType.Int).Value = sp.SoLuong;
            cmd.Parameters.Add("@gianhap", SqlDbType.BigInt).Value = sp.DonGiaNhap;
            cmd.Parameters.Add("@giabansi", SqlDbType.BigInt).Value = sp.GiaBanSi;
            cmd.Parameters.Add("@giabanle", SqlDbType.BigInt).Value = sp.GiaBanLe;

            return m_Ds.ExecuteNoneQuery(cmd) > 0;
        }

        public bool Update(SanPham sp)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE SAN_PHAM SET TEN_SAN_PHAM=@ten, ID_DON_VI_TINH=@id_dvt, SO_LUONG=@soluong, " +
                "DON_GIA_NHAP=@gianhap, GIA_BAN_SI=@giabansi, GIA_BAN_LE=@giabanle WHERE ID=@id"
            );
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = sp.Id;
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = sp.TenSanPham;
            cmd.Parameters.Add("@id_dvt", SqlDbType.Int).Value = sp.DonViTinh.Id;
            cmd.Parameters.Add("@soluong", SqlDbType.Int).Value = sp.SoLuong;
            cmd.Parameters.Add("@gianhap", SqlDbType.BigInt).Value = sp.DonGiaNhap;
            cmd.Parameters.Add("@giabansi", SqlDbType.BigInt).Value = sp.GiaBanSi;
            cmd.Parameters.Add("@giabanle", SqlDbType.BigInt).Value = sp.GiaBanLe;

            return m_Ds.ExecuteNoneQuery(cmd) > 0;
        }

        public bool Delete(string id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM SAN_PHAM WHERE ID=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;

            return m_Ds.ExecuteNoneQuery(cmd) > 0;
        }

        internal void Save()
        {
            throw new NotImplementedException();
        }
    }
}
