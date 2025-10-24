// DAL/DataLayer/ChiTietPhieuBanFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.DAL.Infrastructure; // CHANGED: dùng DbClient

namespace CuahangNongduoc.DataLayer
{
    /// <summary>
    /// DAL cho CHI_TIET_PHIEU_BAN – bản ADO.NET (SqlClient) qua DbClient
    /// </summary>
    public class ChiTietPhieuBanDAL
    {
        private readonly DbClient _db = DbClient.Instance; // CHANGED

        /* ===================== SELECT ===================== */

        public DataTable LayChiTietPhieuBan(string idPhieuBan)
        {
            // CHANGED: dùng DbClient + chỉ chọn cột cần thiết (tùy ý)
            const string sql =
                "SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@id", SqlDbType.VarChar, idPhieuBan, 50));
        }

        public DataTable LayChiTietPhieuBan(DateTime ngayBan)
        {
            // CHANGED: sargable theo khoảng [start, end)
            var start = ngayBan.Date;
            var end = start.AddDays(1);

            const string sql = @"
                SELECT CT.*
                FROM CHI_TIET_PHIEU_BAN CT
                JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID
                WHERE PB.NGAY_BAN >= @start AND PB.NGAY_BAN < @end";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));
        }

        public DataTable LayChiTietPhieuBan(int thang, int nam)
        {
            // CHANGED: chuyển sang range đầu-tháng → đầu-tháng-sau (sargable)
            var start = new DateTime(nam, thang, 1);
            var end = start.AddMonths(1);

            const string sql = @"
                SELECT CT.*
                FROM CHI_TIET_PHIEU_BAN CT
                JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID
                WHERE PB.NGAY_BAN >= @start AND PB.NGAY_BAN < @end";
            return _db.ExecuteDataTable(sql, CommandType.Text,
                _db.P("@start", SqlDbType.DateTime, start),
                _db.P("@end", SqlDbType.DateTime, end));
        }

        /* ===================== INSERT/UPDATE/DELETE ===================== */

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            // CHANGED: dùng DbClient.Cmd + P/PDec
            const string sql = @"
                INSERT INTO CHI_TIET_PHIEU_BAN
                (ID_PHIEU_BAN, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN)
                VALUES(@ID_PHIEU_BAN, @ID_MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @THANH_TIEN)";

            using (var cmd = _db.Cmd(tx?.Connection, sql, CommandType.Text, tx, 30,
                _db.P("@ID_PHIEU_BAN", SqlDbType.VarChar, row["ID_PHIEU_BAN"], 50),
                _db.P("@ID_MA_SAN_PHAM", SqlDbType.VarChar, row["ID_MA_SAN_PHAM"], 50),
                _db.P("@SO_LUONG", SqlDbType.Int, row["SO_LUONG"]),
                _db.PDec("@DON_GIA", row["DON_GIA"]),          // NEW: decimal 18,2
                _db.PDec("@THANH_TIEN", row["THANH_TIEN"])))   // NEW: decimal 18,2
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateTonKho(string idMaSanPham, int deltaSoLuong, SqlTransaction tx)
        {
            // Giữ đúng logic cũ: cộng/trừ tồn kho theo delta
            const string sql = @"
                UPDATE MA_SAN_PHAM 
                   SET SO_LUONG = SO_LUONG + @delta
                 WHERE ID = @id";
            using (var cmd = _db.Cmd(tx.Connection, sql, CommandType.Text, tx, 30,
                _db.P("@delta", SqlDbType.Int, deltaSoLuong),
                _db.P("@id", SqlDbType.VarChar, idMaSanPham, 50)))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Duyệt bảng, thêm CT + cập nhật kho trong 1 transaction.
        /// Chỉ xử lý các row trạng thái Added.
        /// </summary>
        public bool SaveAddedRows(DataTable table)
        {
            // CHANGED: dùng DbClient.InTx
            return _db.InTx((cn, tx) =>
            {
                int n = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState != DataRowState.Added) continue;

                    // 1) Cập nhật tồn kho (trừ số lượng đã bán)
                    UpdateTonKho(
                        Convert.ToString(row["ID_MA_SAN_PHAM"]),
                        -Convert.ToInt32(row["SO_LUONG"]),
                        tx);

                    // 2) Ghi chi tiết phiếu bán
                    n += Insert(row, tx);
                }
                return n;
            }) > 0;
        }
    }
}
