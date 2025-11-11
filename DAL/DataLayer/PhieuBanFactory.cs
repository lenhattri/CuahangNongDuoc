// DAL/DataLayer/PhieuBanFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure;             // CHANGED: dùng DbClient (singleton)

namespace CuahangNongduoc.DataLayer
{
    public class PhieuBanFactory : IPhieuBanFactory
    {
        // DataService m_Ds = new DataService();
        private readonly DbClient _db = DbClient.Instance; // CHANGED: bỏ DataService, dùng DbClient
        private DataTable _table;                          // NEW: giữ DataTable đang bind để Save()

        private const string SELECT_BASE =
            "SELECT ID, ID_KHACH_HANG, ID_NHAN_VIEN, NGAY_BAN, TONG_TIEN, DA_TRA, CON_NO, GIAM_GIA FROM PHIEU_BAN";

        /* ===================== Helpers ===================== */

        private void EnsureSchema()                       // NEW: bảo đảm _table có schema
        {
            if (_table != null) return;
            _table = new DataTable("PHIEU_BAN");
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, SELECT_BASE + " WHERE 1=0", CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.FillSchema(_table, SchemaType.Source);
            }
        }

        private SqlDataAdapter CreateAdapter(SqlConnection cn) // NEW: adapter + commandBuilder cho Save()
        {
            var da = new SqlDataAdapter
            {
                SelectCommand = _db.Cmd(cn, SELECT_BASE, CommandType.Text)
            };
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            _ = new SqlCommandBuilder(da); // tự sinh Insert/Update/Delete
            return da;
        }

        /* ===================== Queries ===================== */

        public DataTable TimPhieuBan(string idKh, DateTime dt)
        {
            // (Giữ semantics cũ: so sánh bằng ngày—nếu cột là datetime có time, cân nhắc CAST khi cần)
            using (var cn = _db.Open()) // CHANGED
            using (var cmd = _db.Cmd(cn,
                   "SELECT * FROM PHIEU_BAN WHERE NGAY_BAN = @ngay AND ID_KHACH_HANG = @kh",
                   CommandType.Text, null, 30,
                   _db.P("@ngay", SqlDbType.Date, dt.Date),                // CHANGED
                   _db.P("@kh", SqlDbType.NVarChar, idKh, 50)))           // CHANGED
            using (var da = new SqlDataAdapter(cmd))                         // CHANGED
            {
                var dtRes = new DataTable("PHIEU_BAN");
                da.Fill(dtRes);
                _table = dtRes; // NEW: đồng bộ cho NewRow/Add/Save nếu cần
                return dtRes;
            }
        }

        public DataTable DanhsachPhieuBanLe()
        {
            const string sql =
                "SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID WHERE KH.LOAI_KH = 0";
            using (var cn = _db.Open())                                      // CHANGED
            using (var cmd = _db.Cmd(cn, sql, CommandType.Text))             // CHANGED
            using (var da = new SqlDataAdapter(cmd))                         // CHANGED
            {
                var dtRes = new DataTable("PHIEU_BAN");
                da.Fill(dtRes);
                _table = dtRes; // NEW
                return dtRes;
            }
        }

        public DataTable DanhsachPhieuBanSi()
        {
            const string sql =
                "SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID WHERE KH.LOAI_KH = 1";
            using (var cn = _db.Open())                                      // CHANGED
            using (var cmd = _db.Cmd(cn, sql, CommandType.Text))             // CHANGED
            using (var da = new SqlDataAdapter(cmd))                         // CHANGED
            {
                var dtRes = new DataTable("PHIEU_BAN");
                da.Fill(dtRes);
                _table = dtRes; // NEW
                return dtRes;
            }
        }

        public DataTable DanhsachPhieu()
        {
            const string sql =
                "SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID";
            using (var cn = _db.Open())
            using (var cmd = _db.Cmd(cn, sql, CommandType.Text))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dtRes = new DataTable("PHIEU_BAN");
                da.Fill(dtRes);
                _table = dtRes;
                return dtRes;
            }
        }

        public DataTable LayPhieuBan(string id)
        {
            using (var cn = _db.Open())                                      // CHANGED
            using (var cmd = _db.Cmd(cn,
                   "SELECT * FROM PHIEU_BAN WHERE ID = @id",
                   CommandType.Text, null, 30,
                   _db.P("@id", SqlDbType.NVarChar, id, 50)))                // CHANGED
            using (var da = new SqlDataAdapter(cmd))                         // CHANGED
            {
                var dtRes = new DataTable("PHIEU_BAN");
                da.Fill(dtRes);
                _table = dtRes; // NEW
                return dtRes;
            }
        }

        public DataTable LayChiTietPhieuBan(string idPhieuBan)
        {
            using (var cn = _db.Open())                                      // CHANGED
            using (var cmd = _db.Cmd(cn,
                   "SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id",
                   CommandType.Text, null, 30,
                   _db.P("@id", SqlDbType.NVarChar, idPhieuBan, 50)))        // CHANGED
            using (var da = new SqlDataAdapter(cmd))                         // CHANGED
            {
                var dtRes = new DataTable("CHI_TIET_PHIEU_BAN");
                da.Fill(dtRes);
                return dtRes; // (Không buộc _table vì Save() chỉ áp dụng cho PHIEU_BAN)
            }
        }

        public long LayConNo(string kh, int thang, int nam)
        {
            // CHANGED: bỏ DataService, dùng DbClient + tham số đúng kiểu
            var db = DbClient.Instance;
            const string sql = @"
                SELECT SUM(CON_NO) FROM PHIEU_BAN
                WHERE ID_KHACH_HANG = @kh AND MONTH(NGAY_BAN) = @thang AND YEAR(NGAY_BAN) = @nam";
            var val = db.ExecuteScalar<object>(sql, CommandType.Text,        // lấy object rồi tự xử lý null
                db.P("@kh", SqlDbType.NVarChar, kh, 50),
                db.P("@thang", SqlDbType.Int, thang),
                db.P("@nam", SqlDbType.Int, nam));
            return (val == null || val == DBNull.Value) ? 0L : Convert.ToInt64(val);
        }

        public int LaySoPhieu()
        {
            var db = DbClient.Instance;                                      // CHANGED
            const string sql = "SELECT COUNT(*) FROM PHIEU_BAN";
            return db.ExecuteScalar<int>(sql, CommandType.Text);             // CHANGED
        }

        /* ===================== DataTable pattern (giữ API cũ) ===================== */

        public DataRow NewRow()
        {
            EnsureSchema(); // NEW
            return _table.NewRow();
        }

        public void Add(DataRow row)
        {
            EnsureSchema(); // NEW
            _table.Rows.Add(row);
        }

        public bool Save()
        {
            EnsureSchema(); // NEW
            using (var cn = _db.Open())
            using (var da = CreateAdapter(cn))
            {
                return da.Update(_table) > 0; // CHANGED: thay cho m_Ds.ExecuteNoneQuery()
            }
        }
    }
}
