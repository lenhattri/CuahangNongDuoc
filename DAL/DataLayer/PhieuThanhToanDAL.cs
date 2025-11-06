// DAL/DataLayer/PhieuThanhToanDAL.cs
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CuahangNongduoc.DAL.Infrastructure; // CHANGED: dùng DbClient (singleton) thay vì ConfigurationManager/SqlConnection tự mở

namespace CuahangNongduoc.DataLayer
{
    public class PhieuThanhToanDAL : IPhieuThanhToanDAL
    {
        private DataTable m_DataTable;

        // CHANGED: bỏ property ConnectionString; dùng DbClient cho tất cả truy vấn
        private readonly DbClient _db = DbClient.Instance; // NEW

        /* ===================== SELECT ALL ===================== */
        public DataTable DanhsachPhieuThanhToan()
        {
            var dt = new DataTable("PHIEU_THANH_TOAN");
            // CHANGED: dùng _db.Open() + _db.Cmd(...)
            using (var cn = _db.Open())                                                                 // CHANGED
            using (var cmd = _db.Cmd(cn, "SELECT * FROM PHIEU_THANH_TOAN", CommandType.Text))           // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                                    // CHANGED
            {
                da.Fill(dt);
            }
            m_DataTable = dt;
            return dt;
        }

        /* ===================== FIND BY CUSTOMER + DATE (range) ===================== */
        public DataTable TimPhieuThanhToan(string kh, DateTime ngay)
        {
            m_DataTable = new DataTable("PHIEU_THANH_TOAN");
            var start = ngay.Date;                 // NEW: giữ sargability
            var end = start.AddDays(1);          // NEW

            // CHANGED: thay DATEADD bằng khoảng [@start, @end)
            const string sql = @"
                SELECT * FROM PHIEU_THANH_TOAN
                WHERE ID_KHACH_HANG = @kh
                  AND NGAY_THANH_TOAN >= @start
                  AND NGAY_THANH_TOAN <  @end";

            using (var cn = _db.Open())                                                                    // CHANGED
            using (var cmd = _db.Cmd(cn, sql, CommandType.Text, null, 30,
                       _db.P("@kh", SqlDbType.NVarChar, kh, 50),                                        // CHANGED: NVarChar hỗ trợ Unicode
                       _db.P("@start", SqlDbType.DateTime, start),                                        // CHANGED
                       _db.P("@end", SqlDbType.DateTime, end)))                                         // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                                        // CHANGED
            {
                da.Fill(m_DataTable);
            }
            return m_DataTable;
        }

        /* ===================== GET BY ID ===================== */
        public DataTable LayPhieuThanhToan(string id)
        {
            m_DataTable = new DataTable("PHIEU_THANH_TOAN");
            using (var cn = _db.Open())                                                                    // CHANGED
            using (var cmd = _db.Cmd(cn, "SELECT * FROM PHIEU_THANH_TOAN WHERE ID = @id", CommandType.Text,
                       null, 30,
                       _db.P("@id", SqlDbType.NVarChar, id, 50)))                                          // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                                        // CHANGED
            {
                da.Fill(m_DataTable);
            }
            return m_DataTable;
        }

        /* ===================== AGGREGATE: SUM(TONG_TIEN) ===================== */
        public long LayTongTien(string kh, int thang, int nam)
        {
            // CHANGED: dùng DbClient thay vì tự tạo SqlConnection/ConnectionString
            var db = DbClient.Instance;
            const string sql = @"
                SELECT SUM(TONG_TIEN)
                FROM PHIEU_THANH_TOAN
                WHERE ID_KHACH_HANG = @kh
                  AND MONTH(NGAY_THANH_TOAN) = @thang
                  AND YEAR(NGAY_THANH_TOAN)  = @nam";
            var obj = db.ExecuteScalar<object>(sql, CommandType.Text,                                      // CHANGED
                db.P("@kh", SqlDbType.NVarChar, kh, 50),
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));
            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }

        /* ===================== DATATABLE HELPERS (giữ API cũ) ===================== */
        public DataRow NewRow()
        {
            if (m_DataTable == null)
                DanhsachPhieuThanhToan(); // giữ hành vi cũ
            return m_DataTable.NewRow();
        }

        public void Add(DataRow row)
        {
            if (m_DataTable == null)
                DanhsachPhieuThanhToan(); // giữ hành vi cũ
            m_DataTable.Rows.Add(row);
        }

        /* ===================== SAVE (DataAdapter + CommandBuilder) ===================== */
        public bool Save()
        {
            if (m_DataTable == null) return false;

            // CHANGED: dùng DbClient cho SelectCommand, để CommandBuilder sinh CRUD
            using (var cn = _db.Open())                                                                    // CHANGED
            using (var cmd = _db.Cmd(cn, "SELECT * FROM PHIEU_THANH_TOAN", CommandType.Text))              // CHANGED
            using (var da = new SqlDataAdapter(cmd))                                                       // CHANGED
            using (var builder = new SqlCommandBuilder(da))
            {
                da.RowUpdated += (s, e) =>
                {
                    if (e.StatementType == StatementType.Insert)
                    {
                        // giữ nguyên behavior cũ
                        MessageBox.Show("Inserted ID: " + e.Row["ID"].ToString());
                    }
                };

                int affectedRows = da.Update(m_DataTable);
                return affectedRows > 0;
            }
        }
    }
}
