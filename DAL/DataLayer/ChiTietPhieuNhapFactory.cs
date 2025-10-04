//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

//namespace CuahangNongduoc.DataLayer
//{
//    public class ChiTietPhieuNhapFactory
//    {
//        DataService m_Ds = new DataService();

//        public void LoadSchema()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = '-1'");
//            m_Ds.Load(cmd);
//        }

//        public DataTable LayChiTietPhieuNhap(String id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = @id");
//            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = id;
//            m_Ds.Load(cmd);
//            return m_Ds;
//        }

//        public int XoaChiTietPhieuNhap(String id)
//        {
//            OleDbCommand cmd = new OleDbCommand("DELETE FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = @id");
//            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = id;
//            return m_Ds.ExecuteNoneQuery(cmd);
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

//           return m_Ds.ExecuteNoneQuery() > 0;
//        }
//    }
//}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CuahangNongduoc.DataLayer
{
    public class ChiTietPhieuNhapDAL
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        public DataTable LayChiTietPhieuNhap(string idPhieuNhap)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = @id", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idPhieuNhap;
                da.Fill(dt);
            }
            return dt;
        }

        public int XoaChiTietPhieuNhap(string idPhieuNhap, SqlTransaction tx = null)
        {
            using (var cmd = new SqlCommand("DELETE FROM CHI_TIET_PHIEU_NHAP WHERE ID_PHIEU_NHAP = @id", tx?.Connection ?? new SqlConnection(_cs), tx))
            {
                if (tx == null)
                    cmd.Connection.Open();

                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idPhieuNhap;
                int rows = cmd.ExecuteNonQuery();

                if (tx == null)
                    cmd.Connection.Close();

                return rows;
            }
        }

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            const string sql = @"
                INSERT INTO CHI_TIET_PHIEU_NHAP
                    (ID_PHIEU_NHAP, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN)
                VALUES
                    (@ID_PHIEU_NHAP, @ID_MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @THANH_TIEN)";

            using (var cmd = new SqlCommand(sql, tx?.Connection ?? new SqlConnection(_cs), tx))
            {
                if (tx == null)
                    cmd.Connection.Open();

                cmd.Parameters.Add("@ID_PHIEU_NHAP", SqlDbType.VarChar, 50).Value = row["ID_PHIEU_NHAP"];
                cmd.Parameters.Add("@ID_MA_SAN_PHAM", SqlDbType.VarChar, 50).Value = row["ID_MA_SAN_PHAM"];
                cmd.Parameters.Add("@SO_LUONG", SqlDbType.Int).Value = row["SO_LUONG"];
                cmd.Parameters.Add("@DON_GIA", SqlDbType.Decimal).Value = row["DON_GIA"];
                cmd.Parameters.Add("@THANH_TIEN", SqlDbType.Decimal).Value = row["THANH_TIEN"];

                int result = cmd.ExecuteNonQuery();

                if (tx == null)
                    cmd.Connection.Close();

                return result;
            }
        }

        /// <summary>
        /// Lưu tất cả các dòng mới (Added) trong DataTable vào DB trong 1 transaction.
        /// </summary>
        public bool SaveAddedRows(DataTable table)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            if (row.RowState == DataRowState.Added)
                            {
                                Insert(row, tx);
                            }
                        }
                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public DataRow NewRow()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM CHI_TIET_PHIEU_NHAP WHERE 1=0", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            return dt.NewRow();
        }
    }
}
