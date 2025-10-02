//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

//namespace CuahangNongduoc.DataLayer
//{
//    public class KhachHangFactory
//    {
//        DataService m_Ds = new DataService();

//        public DataTable DanhsachKhachHang(bool loai)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE LOAI_KH = " + loai);
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }
//        public DataTable TimHoTen(String hoten, bool loai)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE HO_TEN LIKE '%' + @hoten + '%' AND LOAI_KH = " + loai);
//            cmd.Parameters.Add("hoten", OleDbType.VarChar).Value = hoten;
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable TimDiaChi(String diachi, bool loai)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE DIA_CHI LIKE '%' + @diachi + '%' AND LOAI_KH = " + loai);
//            cmd.Parameters.Add("diachi", OleDbType.VarChar).Value = diachi;
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable DanhsachKhachHang()
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG");
//            m_Ds.Load(cmd);

//            return m_Ds;
//        }

//        public DataTable LayKhachHang(String id)
//        {
//            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE ID = @id");
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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public class KhachHangFactory
    {
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        private DataTable _table;                    // thay cho m_Ds cũ
        private const string SELECT_ALL = "SELECT * FROM KHACH_HANG";

        /* ===== Helpers ===== */
        private SqlDataAdapter CreateAdapter(SqlConnection conn)
        {
            var da = new SqlDataAdapter(SELECT_ALL, conn)
            {
                MissingSchemaAction = MissingSchemaAction.AddWithKey // để SqlCommandBuilder sinh CRUD
            };
            var _ = new SqlCommandBuilder(da);
            return da;
        }

        private void EnsureSchema()
        {
            if (_table != null) return;
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(SELECT_ALL + " WHERE 1=0", conn))
            {
                _table = new DataTable("KHACH_HANG");
                da.Fill(_table); // chỉ lấy schema
            }
        }

        /* ===== API ===== */

        public DataTable DanhsachKhachHang(bool loai)
        {
            var dt = new DataTable("KHACH_HANG");
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM KHACH_HANG WHERE LOAI_KH = @loai", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@loai", SqlDbType.Bit).Value = loai;
                da.Fill(dt);
            }
            _table = dt;
            return dt;
        }

        public DataTable TimHoTen(string hoten, bool loai)
        {
            var dt = new DataTable("KHACH_HANG");
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "SELECT * FROM KHACH_HANG WHERE HO_TEN LIKE '%' + @hoten + '%' AND LOAI_KH = @loai", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@hoten", SqlDbType.NVarChar, 200).Value = hoten ?? "";
                cmd.Parameters.Add("@loai", SqlDbType.Bit).Value = loai;
                da.Fill(dt);
            }
            _table = dt;
            return dt;
        }

        public DataTable TimDiaChi(string diachi, bool loai)
        {
            var dt = new DataTable("KHACH_HANG");
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "SELECT * FROM KHACH_HANG WHERE DIA_CHI LIKE '%' + @diachi + '%' AND LOAI_KH = @loai", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@diachi", SqlDbType.NVarChar, 200).Value = diachi ?? "";
                cmd.Parameters.Add("@loai", SqlDbType.Bit).Value = loai;
                da.Fill(dt);
            }
            _table = dt;
            return dt;
        }

        public DataTable DanhsachKhachHang()
        {
            var dt = new DataTable("KHACH_HANG");
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(SELECT_ALL, conn))
            {
                da.Fill(dt);
            }
            _table = dt;
            return dt;
        }

        public DataTable LayKhachHang(string id)
        {
            var dt = new DataTable("KHACH_HANG");
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT * FROM KHACH_HANG WHERE ID = @id", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                da.Fill(dt);
            }
            _table = dt;
            return dt;
        }

        public DataRow NewRow()
        {
            EnsureSchema();
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema();
            _table.Rows.Add(row);
        }

        public bool Save()
        {
            EnsureSchema();
            using (var conn = new SqlConnection(_cs))
            using (var da = CreateAdapter(conn))
            {
                return da.Update(_table) > 0;
            }
        }
    }
}
