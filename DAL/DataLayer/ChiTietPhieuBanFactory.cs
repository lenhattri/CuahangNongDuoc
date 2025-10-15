
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;

namespace CuahangNongduoc.DataLayer
{
    /// <summary>
    /// DAL cho CHI_TIET_PHIEU_BAN – bản ADO.NET (SqlClient)
    /// </summary>
    public class ChiTietPhieuBanDAL
    {
        private readonly string _cs = //"Server=.\\DESKTOP-LP4MN8P;Database=cuahangnongduoc;Trusted_Connection=True;Encrypt=False;";
        ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        /* ===================== SELECT ===================== */

        public DataTable LayChiTietPhieuBan(string idPhieuBan)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idPhieuBan;
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable LayChiTietPhieuBan(DateTime ngayBan)
        {
            // So khớp theo ngày (bỏ phần time)
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                @"SELECT CT.* 
                  FROM CHI_TIET_PHIEU_BAN CT 
                  JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID
                  WHERE CAST(PB.NGAY_BAN AS date) = @ngayban", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ngayban", SqlDbType.Date).Value = ngayBan.Date;
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable LayChiTietPhieuBan(int thang, int nam)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                @"SELECT CT.* 
                  FROM CHI_TIET_PHIEU_BAN CT 
                  JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID
                  WHERE MONTH(PB.NGAY_BAN) = @thang AND YEAR(PB.NGAY_BAN) = @nam", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
                cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;
                da.Fill(dt);
            }
            return dt;
        }

        /* ===================== INSERT/UPDATE/DELETE ===================== */
        // Gợi ý: dùng theo kiểu “row-based” cho nhanh; nếu team muốn DTO, mình viết thêm class DTO.

        public int Insert(DataRow row, SqlTransaction tx = null)
        {
            // Giả định các cột đã tồn tại trong row
            const string sql =
                @"INSERT INTO CHI_TIET_PHIEU_BAN
                    (ID_PHIEU_BAN, ID_MA_SAN_PHAM, SO_LUONG, DON_GIA, THANH_TIEN)
                  VALUES(@ID_PHIEU_BAN, @ID_MA_SAN_PHAM, @SO_LUONG, @DON_GIA, @THANH_TIEN)";

            using (var cmd = new SqlCommand(sql, tx?.Connection, tx))
            {
                cmd.Parameters.Add("@ID_PHIEU_BAN", SqlDbType.VarChar, 50).Value = row["ID_PHIEU_BAN"];
                cmd.Parameters.Add("@ID_MA_SAN_PHAM", SqlDbType.VarChar, 50).Value = row["ID_MA_SAN_PHAM"];
                cmd.Parameters.Add("@SO_LUONG", SqlDbType.Int).Value = row["SO_LUONG"];
                cmd.Parameters.Add("@DON_GIA", SqlDbType.Decimal).Value = row["DON_GIA"];
                cmd.Parameters.Add("@THANH_TIEN", SqlDbType.Decimal).Value = row["THANH_TIEN"];
                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateTonKho(string idMaSanPham, int deltaSoLuong, SqlTransaction tx)
        {
            // deltaSoLuong âm nghĩa là trừ kho (bán ra)
            const string sql =
                @"UPDATE MA_SAN_PHAM 
                  SET SO_LUONG = SO_LUONG + @delta
                  WHERE ID = @id";
            using (var cmd = new SqlCommand(sql, tx.Connection, tx))
            {
                cmd.Parameters.Add("@delta", SqlDbType.Int).Value = deltaSoLuong;
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idMaSanPham;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Thay cho m_Ds.ExecuteNoneQuery(): duyệt bảng, thêm CT + cập nhật kho trong 1 transaction.
        /// Chỉ xử lý các row trạng thái Added (giống code cũ).
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
                            if (row.RowState != DataRowState.Added) continue;

                            // 1) Cập nhật tồn kho (trừ số lượng đã bán)
                            UpdateTonKho(
                                Convert.ToString(row["ID_MA_SAN_PHAM"]),
                                -Convert.ToInt32(row["SO_LUONG"]),
                                tx);

                            // 2) Ghi chi tiết phiếu bán
                            Insert(row, tx);
                        }

                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw; // cho BLL bắt và báo lỗi UI
                    }
                }
            }
        }
    }
}
