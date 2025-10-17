// DAL/DataLayer/PhieuChiFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure; 

namespace CuahangNongduoc.DataLayer
{
    public class PhieuChiFactory
    {
        
        private readonly DbClient _db = DbClient.Instance;   // CHANGED
        private DataTable _table;                            // NEW: DataTable nội bộ cho pattern NewRow/Add/Save

        private const string SELECT_ALL = "SELECT * FROM PHIEU_CHI"; // giữ nguyên SELECT * để CommandBuilder sinh CRUD

        /* ===================== Helpers ===================== */

        private void EnsureSchema() // NEW
        {
            if (_table != null) return;
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_ALL + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                _table = new DataTable("PHIEU_CHI");
                da.FillSchema(_table, SchemaType.Source); // CHANGED: lấy schema rỗng rõ ràng
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn) // NEW: phục vụ Save()
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_ALL, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // auto sinh Insert/Update/Delete
            return da;
        }

        /* ===================== SELECTs ===================== */

        // Tìm theo lý do + NGAY_CHI (dùng range [@start,@end) để sargable)
        public DataTable TimPhieuChi(int lydo, DateTime ngay)
        {
            var start = ngay.Date;              // NEW
            var end = start.AddDays(1);       // NEW

            const string sql = @"
                SELECT * FROM PHIEU_CHI
                WHERE ID_LY_DO_CHI = @lydo
                  AND NGAY_CHI >= @start AND NGAY_CHI < @end";            // CHANGED

            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@lydo", SqlDbType.Int, lydo),                // CHANGED: đúng kiểu Int
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));
            _table = dt; // đồng bộ cho NewRow/Add/Save
            return dt;
        }

        public DataTable DanhsachPhieuChi()
        {
            var dt = _db.ExecuteDataTable(SELECT_ALL, CommandType.Text);   // CHANGED
            _table = dt;
            return dt;
        }

        public DataTable LayPhieuChi(string id)
        {
            const string sql = "SELECT * FROM PHIEU_CHI WHERE ID = @id";
            var dt = _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.NVarChar, id, 50));                 // CHANGED: tham số hoá, Unicode-safe
            _table = dt;
            return dt;
        }

        public static long LayTongTien(string lydo, int thang, int nam)
        {
            var db = DbClient.Instance;                                     // CHANGED
            const string sql = @"
                SELECT SUM(TONG_TIEN)
                FROM PHIEU_CHI
                WHERE ID_LY_DO_CHI = @lydo
                  AND MONTH(NGAY_CHI) = @thang
                  AND YEAR(NGAY_CHI)  = @nam";

            // Nếu ID_LY_DO_CHI là INT trong DB, có thể đổi sang SqlDbType.Int và Convert.ToInt32(lydo)
            var obj = db.ExecuteScalar<object>(sql, CommandType.Text,
                db.P("@lydo", SqlDbType.NVarChar, lydo, 50),              // CHANGED: giữ nguyên kiểu chuỗi 
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));
            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }

        public DataRow NewRow()
        {
            EnsureSchema(); // CHANGED
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema(); // CHANGED
            _table.Rows.Add(row);
        }

        public bool Save()
        {
            // thay m_Ds.ExecuteNoneQuery()
            EnsureSchema();
            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))
            {
                return da.Update(_table) > 0;
            }
        }
    }
}
