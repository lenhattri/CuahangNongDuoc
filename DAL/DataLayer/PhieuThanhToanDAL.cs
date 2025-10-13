/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuThanhToanFactory
    {
        DataService m_Ds = new DataService();

        public DataTable DanhsachPhieuThanhToan()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_THANH_TOAN ");
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable TimPhieuThanhToan(String kh, DateTime ngay)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG=@kh AND NGAY_THANH_TOAN = @ngay");
            cmd.Parameters.Add("kh", OleDbType.VarChar).Value = kh;
            cmd.Parameters.Add("ngay", OleDbType.Date).Value = ngay;

            m_Ds.Load(cmd);

            return m_Ds;
        }
      
        public DataTable LayPhieuThanhToan(String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_THANH_TOAN WHERE ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }


        public static long LayTongTien(String kh, int thang, int nam)
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT SUM(TONG_TIEN) FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG = @kh AND MONTH(NGAY_THANH_TOAN)=@thang AND YEAR(NGAY_THANH_TOAN)= @nam");
            cmd.Parameters.Add("kh", OleDbType.VarChar, 50).Value = kh;
            cmd.Parameters.Add("thang", OleDbType.Integer).Value = thang;
            cmd.Parameters.Add("nam", OleDbType.Integer).Value = nam;

            object obj = ds.ExecuteScalar(cmd);
            
            if (obj == null)
                return 0;
            else
                return Convert.ToInt64(obj);
        }
        
        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }
        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }
       public bool Save()
        {
           
            return m_Ds.ExecuteNoneQuery() > 0;
        }
    }
}
*/

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuThanhToanDAL
    {
        private DataTable m_DataTable;

        private static string ConnectionString
        {
            get
            {
                var connStr = ConfigurationManager.ConnectionStrings["ConnStr"]?.ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                    throw new InvalidOperationException("Connection string 'ConnStr' not found.");
                return connStr;
            }
        }

        // Danh sách tất cả phiếu
        public DataTable DanhsachPhieuThanhToan()
        {
            DataTable dt = new DataTable("PHIEU_THANH_TOAN");
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_THANH_TOAN", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            m_DataTable = dt;
            return dt;
        }

        // Tìm theo khách hàng + ngày
        public DataTable TimPhieuThanhToan(string kh, DateTime ngay)
        {
            m_DataTable = new DataTable("PHIEU_THANH_TOAN");
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT * FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG=@kh AND NGAY_THANH_TOAN >= @ngay AND NGAY_THANH_TOAN < DATEADD(day,1,@ngay)", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
                cmd.Parameters.Add("@ngay", SqlDbType.DateTime).Value = ngay.Date;

                da.Fill(m_DataTable);
            }
            return m_DataTable;
        }

        // Lấy phiếu theo ID
        public DataTable LayPhieuThanhToan(string id)
        {
            m_DataTable = new DataTable("PHIEU_THANH_TOAN");
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_THANH_TOAN WHERE ID = @id", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                da.Fill(m_DataTable);
            }
            return m_DataTable;
        }

        // Tổng tiền
        public static long LayTongTien(string kh, int thang, int nam)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT SUM(TONG_TIEN) FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG = @kh AND MONTH(NGAY_THANH_TOAN)=@thang AND YEAR(NGAY_THANH_TOAN)=@nam", conn))
            {
                cmd.Parameters.Add("@kh", SqlDbType.VarChar, 50).Value = kh;
                cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;

                conn.Open();
                var obj = cmd.ExecuteScalar();
                return (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt64(obj);
            }
        }

        // NewRow dựa trên DataTable hiện tại
        public DataRow NewRow()
        {
            if (m_DataTable == null)
                DanhsachPhieuThanhToan(); // load mặc định nếu chưa có
            return m_DataTable.NewRow();
        }

        // Add vào DataTable hiện tại
        public void Add(DataRow row)
        {
            if (m_DataTable == null)
                DanhsachPhieuThanhToan(); // đảm bảo đã có table
            m_DataTable.Rows.Add(row);
        }

        // Save thay đổi từ DataTable hiện tại
        public bool Save()
        {
            if (m_DataTable == null) return false;

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_THANH_TOAN", conn))
            using (var da = new SqlDataAdapter(cmd))
            using (var builder = new SqlCommandBuilder(da))
            {
                conn.Open();
                da.RowUpdated += (s, e) =>
                {
                    if (e.StatementType == StatementType.Insert)
                    {
                        MessageBox.Show("Inserted ID: " + e.Row["ID"].ToString());
                    }
                };
                int affectedRows = da.Update(m_DataTable);
                return affectedRows > 0;
            }
        }
    }
}
