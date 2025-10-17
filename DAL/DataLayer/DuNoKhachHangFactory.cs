// DAL/DataLayer/DuNoKhachHangFactory.cs
using System;
using System.Configuration; // (giữ lại nếu nơi khác cần, nhưng không dùng trực tiếp nữa)
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;   // CHANGED: dùng DbClient (singleton)

namespace CuahangNongduoc.DataLayer
{
    public class DuNoKhachHangDAL
    {
        // private readonly string _cs = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
        // CHANGED: bỏ chuỗi kết nối rải rác, dùng DbClient
        private readonly DbClient _db = DbClient.Instance;   // CHANGED

        // DataTable trung tâm, tương tự m_Ds trong bản cũ
        private DataTable _table;

        private const string SELECT_ALL =
            "SELECT ID_KHACH_HANG, THANG, NAM, DAU_KY, PHAT_SINH, DA_TRA, CUOI_KY FROM DU_NO_KH";

        /* ========================= Helpers ========================= */

        private SqlDataAdapter CreateAdapter(SqlConnection conn)
        {
            // CHANGED: dùng _db.Cmd để tạo SelectCommand (thay vì truyền string)
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(conn, SELECT_ALL, CommandType.Text)  // CHANGED
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;        // giữ nguyên
            var cb = new SqlCommandBuilder(da);                             // giữ nguyên (sinh CRUD)
            return da;
        }

        private void EnsureSchema()
        {
            if (_table != null) return;
            LoadSchema();
        }

        /* ========================= API tương đương ========================= */

        public void LoadSchema()
        {
            // Tương đương lấy schema rỗng; dùng DbClient để mở kết nối/command
            using (var conn = _db.Open())                                                   // CHANGED
            using (var cmd = _db.Cmd(conn, SELECT_ALL + " WHERE 1=0", CommandType.Text))   // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                      // CHANGED
            {
                _table = new DataTable("DU_NO_KH");
                // Có thể dùng FillSchema hoặc Fill rỗng; FillSchema rõ nghĩa hơn
                da.FillSchema(_table, SchemaType.Source);                                   // CHANGED
            }
        }

        public DataTable DanhsachDuNo(int thang, int nam)
        {
            using (var conn = _db.Open())                                                   // CHANGED
            using (var cmd = _db.Cmd(conn, SELECT_ALL + " WHERE THANG=@thang AND NAM=@nam",
                                       CommandType.Text, null, 30,
                                       _db.P("@thang", SqlDbType.Int, thang),              // CHANGED
                                       _db.P("@nam", SqlDbType.Int, nam)))               // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                      // CHANGED
            {
                var dt = new DataTable("DU_NO_KH");
                da.Fill(dt);

                // Đồng bộ _table để NewRow()/Add()/Save() dùng cùng schema
                _table = dt;
                return dt;
            }
        }

        public DataTable LayDuNoKhachHang(string kh, int thang, int nam)
        {
            using (var conn = _db.Open())                                                   // CHANGED
            using (var cmd = _db.Cmd(conn,
                    SELECT_ALL + " WHERE ID_KHACH_HANG=@kh AND THANG=@thang AND NAM=@nam",
                    CommandType.Text, null, 30,
                    _db.P("@kh", SqlDbType.NVarChar, kh, 50),                            // CHANGED: NVARCHAR để hỗ trợ Unicode
                    _db.P("@thang", SqlDbType.Int, thang),
                    _db.P("@nam", SqlDbType.Int, nam)))
            using (var da = new SqlDataAdapter(cmd))                                      // CHANGED
            {
                var dt = new DataTable("DU_NO_KH");
                da.Fill(dt);

                _table = dt; // giữ schema cho NewRow()/Add()/Save()
                return dt;
            }
        }

        public static long LayDuNo(string kh, int thang, int nam)
        {
            // CHANGED: dùng DbClient thay vì tự mở SqlConnection/ConfigurationManager
            var db = DbClient.Instance;                                                     // CHANGED
            const string sql =
                "SELECT CUOI_KY FROM DU_NO_KH WHERE ID_KHACH_HANG=@kh AND THANG=@thang AND NAM=@nam";
            // ExecuteScalar<long> trả 0 khi null theo helper hiện tại
            return db.ExecuteScalar<long>(sql, CommandType.Text,                            // CHANGED
                db.P("@kh", SqlDbType.NVarChar, kh, 50),
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));
        }

        public void Clear(int thang, int nam)
        {
            // CHANGED: dùng DbClient.ExecuteNonQuery
            const string sql = "DELETE FROM DU_NO_KH WHERE THANG=@thang AND NAM=@nam";      // CHANGED
            _db.ExecuteNonQuery(sql, CommandType.Text,
                _db.P("@thang", SqlDbType.Int, thang),
                _db.P("@nam", SqlDbType.Int, nam));
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
            using (var conn = _db.Open())                                                   // CHANGED
            using (var da = CreateAdapter(conn))
            {
                return da.Update(_table) > 0;
            }
        }
    }
}
