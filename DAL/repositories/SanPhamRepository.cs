using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.Configs;
using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.DAL.Repositories
{
    public sealed class SanPhamRepository : ISanPhamRepository
    {
        private readonly string _connectionString;

        public SanPhamRepository(string connectionString = null)
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString)
                ? GlobalConfig.ConnectionString
                : connectionString;
        }

        // ------------------------ QUERY ------------------------

        public IEnumerable<SanPham> GetAll()
        {
            const string sql = @"
SELECT ID, TEN_SAN_PHAM, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE, ID_DON_VI_TINH, SO_LUONG
FROM SAN_PHAM
ORDER BY TEN_SAN_PHAM;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return Map(rd);
                }
            }
        }

        public IEnumerable<SanPham> SearchByIdLike(string idLike)
        {
            const string sql = @"
SELECT ID, TEN_SAN_PHAM, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE, ID_DON_VI_TINH, SO_LUONG
FROM SAN_PHAM
WHERE ID LIKE '%' + @id + '%'
ORDER BY ID;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = (object)idLike ?? DBNull.Value;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return Map(rd);
                }
            }
        }

        public IEnumerable<SanPham> SearchByNameLike(string tenLike)
        {
            const string sql = @"
SELECT ID, TEN_SAN_PHAM, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE, ID_DON_VI_TINH, SO_LUONG
FROM SAN_PHAM
WHERE TEN_SAN_PHAM LIKE '%' + @ten + '%'
ORDER BY TEN_SAN_PHAM;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 200).Value = (object)tenLike ?? DBNull.Value;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return Map(rd);
                }
            }
        }

        public SanPham GetById(string id)
        {
            const string sql = @"
SELECT ID, TEN_SAN_PHAM, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE, ID_DON_VI_TINH, SO_LUONG
FROM SAN_PHAM
WHERE ID = @id;";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = id;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        return Map(rd);
                }
            }
            return null;
        }

        public DataTable LaySoLuongTon()
        {
            const string sql = @"
SELECT SP.ID, SP.TEN_SAN_PHAM, SP.DON_GIA_NHAP, SP.GIA_BAN_SI, SP.GIA_BAN_LE, SP.ID_DON_VI_TINH , SP.SO_LUONG,
       SUM(MA.SO_LUONG) AS SO_LUONG_TON
FROM SAN_PHAM SP
INNER JOIN MA_SAN_PHAM MA ON SP.ID = MA.ID_SAN_PHAM
GROUP BY SP.ID, SP.TEN_SAN_PHAM, SP.DON_GIA_NHAP, SP.GIA_BAN_SI, SP.GIA_BAN_LE, SP.ID_DON_VI_TINH, SP.SO_LUONG
ORDER BY SP.TEN_SAN_PHAM;";

            var dt = new DataTable();
            using (var conn = new SqlConnection(_connectionString))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.Fill(dt);
            }
            return dt;
        }

        // ------------------------ COMMAND (conn/tx do BLL mở) ------------------------

        public int UpdateQuantity(string maSanPham, int delta, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
UPDATE SAN_PHAM
SET SO_LUONG = SO_LUONG + @delta
WHERE ID = @id;";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@delta", SqlDbType.Int).Value = delta;
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = maSanPham;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Insert(SanPham sp, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
INSERT INTO SAN_PHAM (ID, TEN_SAN_PHAM, DON_GIA_NHAP, GIA_BAN_SI, GIA_BAN_LE, ID_DON_VI_TINH, SO_LUONG)
VALUES (@id, @ten, @dgNhap, @gbSi, @gbLe, @idDvt, @soLuong);";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = sp.Id;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 200).Value = sp.TenSanPham ?? "";
                cmd.Parameters.Add("@dgNhap", SqlDbType.BigInt).Value = sp.DonGiaNhap;
                cmd.Parameters.Add("@gbSi", SqlDbType.BigInt).Value = sp.GiaBanSi;
                cmd.Parameters.Add("@gbLe", SqlDbType.BigInt).Value = sp.GiaBanLe;
                cmd.Parameters.Add("@idDvt", SqlDbType.NVarChar, 50).Value = sp.IdDonViTinh ?? "";
                cmd.Parameters.Add("@soLuong", SqlDbType.Int).Value = sp.SoLuong;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Update(SanPham sp, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
UPDATE SAN_PHAM
SET TEN_SAN_PHAM = @ten,
    DON_GIA_NHAP = @dgNhap,
    GIA_BAN_SI   = @gbSi,
    GIA_BAN_LE   = @gbLe,
    ID_DON_VI_TINH = @idDvt,
    SO_LUONG     = @soLuong
WHERE ID = @id;";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = sp.Id;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 200).Value = sp.TenSanPham ?? "";
                cmd.Parameters.Add("@dgNhap", SqlDbType.BigInt).Value = sp.DonGiaNhap;
                cmd.Parameters.Add("@gbSi", SqlDbType.BigInt).Value = sp.GiaBanSi;
                cmd.Parameters.Add("@gbLe", SqlDbType.BigInt).Value = sp.GiaBanLe;
                cmd.Parameters.Add("@idDvt", SqlDbType.NVarChar, 50).Value = sp.IdDonViTinh ?? "";
                cmd.Parameters.Add("@soLuong", SqlDbType.Int).Value = sp.SoLuong;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Delete(string id, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"DELETE FROM SAN_PHAM WHERE ID = @id;";
            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = id;
                return cmd.ExecuteNonQuery();
            }
        }

        // Tính giá vốn bình quân + tăng tồn trong 1 transaction
        public int UpdateAverageCost(string id, long giaMoi, long soLuongNhap, SqlConnection conn, SqlTransaction tx)
        {
            // Đọc tồn & giá hiện tại (WITH UPDLOCK để tránh race khi song song)
            const string selectSql = @"
SELECT SO_LUONG, DON_GIA_NHAP
FROM SAN_PHAM WITH (UPDLOCK, ROWLOCK)
WHERE ID = @id;";

            int tongSo = 0;
            long giaHienTai = 0;

            using (var select = new SqlCommand(selectSql, conn, tx))
            {
                select.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = id;
                using (var rd = select.ExecuteReader())
                {
                    if (!rd.Read()) return 0;
                    object so = rd["SO_LUONG"];
                    tongSo = (so == DBNull.Value) ? 0 : Convert.ToInt32(so);
                    giaHienTai = Convert.ToInt64(rd["DON_GIA_NHAP"]);
                }
            }

            // Tính bình quân gia nhập mới
            long tongGia = giaHienTai * (long)tongSo + giaMoi * soLuongNhap;
            int tongSoMoi = tongSo + (int)soLuongNhap;
            long giaBinhQuan = tongSoMoi > 0 ? (tongGia / tongSoMoi) : giaMoi;

            const string updateSql = @"
UPDATE SAN_PHAM
SET DON_GIA_NHAP = @giaBQ,
    SO_LUONG     = @soLuongMoi
WHERE ID = @id;";

            using (var update = new SqlCommand(updateSql, conn, tx))
            {
                update.Parameters.Add("@giaBQ", SqlDbType.BigInt).Value = giaBinhQuan;
                update.Parameters.Add("@soLuongMoi", SqlDbType.Int).Value = tongSoMoi;
                update.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = id;
                return update.ExecuteNonQuery();
            }
        }

        // ------------------------ Helper ------------------------

        private static SanPham Map(IDataRecord rd)
        {
            var sp = new SanPham();
            sp.Id = rd["ID"].ToString();
            sp.TenSanPham = rd["TEN_SAN_PHAM"].ToString();
            sp.DonGiaNhap = Convert.ToInt64(rd["DON_GIA_NHAP"]);
            sp.GiaBanSi = Convert.ToInt64(rd["GIA_BAN_SI"]);
            sp.GiaBanLe = Convert.ToInt64(rd["GIA_BAN_LE"]);
            sp.IdDonViTinh = rd["ID_DON_VI_TINH"].ToString();
            sp.SoLuong = rd["SO_LUONG"] == DBNull.Value ? 0 : Convert.ToInt32(rd["SO_LUONG"]);
            return sp;
        }
    }
}
