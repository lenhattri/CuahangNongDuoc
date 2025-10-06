//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

//namespace CuahangNongduoc.DataLayer
//{
//    public class PhieuNhapFactory
//    {
//        DataService m_Ds = new DataService();

//        public void LoadSchema()
//        {
//           OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE ID='-1'");
//            m_Ds.Load(cmd);

//        }

//        public DataTable DanhsachPhieuNhap()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP");
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable TimPhieuNhap(String maNCC, DateTime dt)
//        {
//            String sql = "SELECT * FROM PHIEU_NHAP WHERE NGAY_NHAP = @ngay AND ID_NHA_CUNG_CAP = @ncc";
//            OleDbCommand cmd = new OleDbCommand(sql);
//            cmd.Parameters.Add("ngay", OleDbType.Date).Value = dt;
//            cmd.Parameters.Add("ncc", OleDbType.VarChar).Value = maNCC;

//            m_Ds.Load(cmd);

//            return m_Ds;
//        }


//        public DataTable LayPhieuNhap(String id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE ID = @id");
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
//       public bool Save()
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
    public class PhieuNhapDAL
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        private DataTable _table;  // Lưu giữ DataTable trong class này

        public PhieuNhapDAL()
        {
            _table = DanhsachPhieuNhap(); // load dữ liệu lúc khởi tạo
        }

        public DataTable LoadSchema()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP WHERE ID = '-1'", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.FillSchema(dt, SchemaType.Source);
            }
            return dt;
        }

        public DataTable DanhsachPhieuNhap()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable TimPhieuNhap(string maNCC, DateTime dtNgayNhap)
        {
            var dt = new DataTable();
            string sql = "SELECT * FROM PHIEU_NHAP WHERE NGAY_NHAP = @ngay AND ID_NHA_CUNG_CAP = @ncc";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ngay", SqlDbType.Date).Value = dtNgayNhap.Date;
                cmd.Parameters.Add("@ncc", SqlDbType.VarChar, 50).Value = maNCC;
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable LayPhieuNhap(string id)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP WHERE ID = @id", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                da.Fill(dt);
            }
            return dt;
        }

        // Tạo DataRow mới dựa trên _table hiện có hoặc nếu null thì load schema
        public DataRow NewRow()
        {
            if (_table == null)
                _table = LoadSchema();
            return _table.NewRow();
        }

        // Thêm DataRow vào _table đã lưu
        public void Add(DataRow row)
        {
            if (_table == null)
                _table = LoadSchema();
            _table.Rows.Add(row);
        }

        // Lưu _table xuống database
        public bool Save()
        {
            if (_table == null)
                return false;

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM PHIEU_NHAP WHERE 1=0", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                var cb = new SqlCommandBuilder(da);
                conn.Open();
                int updated = da.Update(_table);
                return updated > 0;
            }
        }
    }
}
