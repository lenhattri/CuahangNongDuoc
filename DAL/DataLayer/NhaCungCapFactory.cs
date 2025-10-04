//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

//namespace CuahangNongduoc.DataLayer
//{
//    public class NhaCungCapFactory
//    {
//        DataService m_Ds = new DataService();

//        public DataTable DanhsachNCC()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP");
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }
//        public DataTable TimDiaChi(String diachi)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP WHERE DIA_CHI LIKE '%' + @diachi + '%' ");
//            cmd.Parameters.Add("diachi", OleDbType.VarChar).Value = diachi;
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }
//        public DataTable TimHoTen(String hoten)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP WHERE HO_TEN LIKE '%' + @hoten + '%' ");
//            cmd.Parameters.Add("hoten", OleDbType.VarChar).Value = hoten;
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable LayNCC(String id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP WHERE ID = @id");
//            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = id;
//            m_Ds.Load(cmd);
//            return m_Ds;
//        }

//        public DataRow NewRow()
//        {
//            return m_Ds.NewRow();
//        }
//        public void Add(DataRow row)
//        {
//            m_Ds.Rows.Add(row);
//        }
//        public bool Save()
//        {
//            return m_Ds.ExecuteNoneQuery() > 0;
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.DataLayer
{
    public class NhaCungCapDAL
    {
        private string connectionString = "Server=.;Database=cuahangnongduoc;Trusted_Connection=True;Encrypt=False";

        // L?y toàn b? danh sách NCC
        public DataTable DanhsachNCC()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM NhaCungCap";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // L?y NCC theo ID
        public DataTable LayNCC(string id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM NhaCungCap WHERE ID = @Id";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@Id", id);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Tìm theo ??a ch?
        public DataTable TimDiaChi(string diachi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM NhaCungCap WHERE DIA_CHI LIKE @DiaChi";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@DiaChi", "%" + diachi + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Tìm theo h? tên
        public DataTable TimHoTen(string hoten)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID, HO_TEN, DIEN_THOAI, DIA_CHI FROM NhaCungCap WHERE HO_TEN LIKE @HoTen";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@HoTen", "%" + hoten + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Thêm m?i
        public void Insert(NhaCungCap ncc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO NhaCungCap(ID, HO_TEN, DIEN_THOAI, DIA_CHI) VALUES(@Id, @HoTen, @DienThoai, @DiaChi)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", ncc.Id);
                cmd.Parameters.AddWithValue("@HoTen", ncc.HoTen);
                cmd.Parameters.AddWithValue("@DienThoai", ncc.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", ncc.DiaChi);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // C?p nh?t
        public void Update(NhaCungCap ncc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE NhaCungCap SET HO_TEN=@HoTen, DIEN_THOAI=@DienThoai, DIA_CHI=@DiaChi WHERE ID=@Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", ncc.Id);
                cmd.Parameters.AddWithValue("@HoTen", ncc.HoTen);
                cmd.Parameters.AddWithValue("@DienThoai", ncc.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", ncc.DiaChi);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Xóa
        public void Delete(string id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM NhaCungCap WHERE ID=@Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
