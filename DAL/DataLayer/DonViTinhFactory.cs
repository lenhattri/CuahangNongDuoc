//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

//namespace CuahangNongduoc.DataLayer
//{
//    public class DonViTinhFactory
//    {
//        DataService m_Ds = new DataService();

//        public DataTable DanhsachDVT()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM DON_VI_TINH");
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }


//        public DataTable LayDVT(int id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM DON_VI_TINH WHERE ID = @id");
//            cmd.Parameters.Add("id", OleDbType.Integer).Value = id;
//            m_Ds.Load(cmd);
//            return m_Ds;
//        }
//        public bool Save()
//        {
//            return m_Ds.ExecuteNoneQuery() > 0;
//        }
//    }
//}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CuahangNongduoc.DataLayer
{
    public class DonViTinhDAL : IDonViTinhDAL
    {
        private readonly DbClient _db = DbClient.Instance; // CHANGED
        public static IDonViTinhDAL Create()
        {
            return new DonViTinhDAL();
        }
        // SELECT * FROM DON_VI_TINH
        public DataTable DanhSachDVT()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM DON_VI_TINH", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            return dt;
        }

        // SELECT * FROM DON_VI_TINH WHERE ID = @id
        public DataTable LayDVT(int id)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM DON_VI_TINH WHERE ID = @id", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                da.Fill(dt);
            }
            return dt;
        }

        /* ==== CRUD nhanh (tuỳ dùng) ==== */
        // Nếu bảng có cột TEN (và có thể có GHI_CHU). Bổ sung cột nếu DB của bạn khác.
        public int Insert(string ten, string ghiChu = null)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "INSERT INTO DON_VI_TINH(TEN, GHI_CHU) VALUES(@TEN, @GHI_CHU)", conn))
            {
                cmd.Parameters.Add("@TEN", SqlDbType.NVarChar, 100).Value = ten;
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.NVarChar, 255).Value =
                    (object)ghiChu ?? DBNull.Value;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Update(int id, string ten, string ghiChu = null)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "UPDATE DON_VI_TINH SET TEN=@TEN, GHI_CHU=@GHI_CHU WHERE ID=@ID", conn))
            {
                cmd.Parameters.Add("@TEN", SqlDbType.NVarChar, 100).Value = ten;
                cmd.Parameters.Add("@GHI_CHU", SqlDbType.NVarChar, 255).Value =
                    (object)ghiChu ?? DBNull.Value;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Delete(int id)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "DELETE FROM DON_VI_TINH WHERE ID=@ID", conn))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /* ==== Phiên bản "Save(DataTable)" tương tự DataService.ExecuteNoneQuery() ==== */
        // Gọi khi bạn đang có 1 DataTable đã Add/Modify/Delete rows và muốn đẩy lên DB.
        public bool Save(DataTable table)
        {
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter("SELECT * FROM DON_VI_TINH", conn))
            using (var cb = new SqlCommandBuilder(da))
            {
                // SqlCommandBuilder tự sinh Insert/Update/Delete nếu SELECT có cột khóa (ID).
                return da.Update(table) > 0;
            }
        }
    }
}
