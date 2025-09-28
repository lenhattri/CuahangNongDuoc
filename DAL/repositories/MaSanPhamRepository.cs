using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.Configs;
using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.DAL.Repositories
{
    public sealed class MaSanPhamRepository : IMaSanPhamRepository
    {
        private readonly string _cs;
        public MaSanPhamRepository(string connectionString = null)
        {
            _cs = string.IsNullOrWhiteSpace(connectionString) ? GlobalConfig.ConnectionString : connectionString;
        }

        // -------------------- Query (IEnumerable) --------------------

        public MaSanPham GetById(string idMa)
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE ID = @id;";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idMa;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read()) return Map(rd);
                }
            }
            return null;
        }

        public IEnumerable<MaSanPham> GetBySanPhamIdAvailable(string idSanPham)
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE ID_SAN_PHAM = @id AND SO_LUONG > 0
ORDER BY NGAY_HET_HAN ASC, NGAY_NHAP ASC;";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idSanPham;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        public IEnumerable<MaSanPham> GetByPhieuNhapId(string idPhieuNhap)
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE ID_PHIEU_NHAP = @id
ORDER BY ID;";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idPhieuNhap;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        public IEnumerable<MaSanPham> GetAllAvailable()
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE SO_LUONG > 0
ORDER BY ID_SAN_PHAM, NGAY_HET_HAN;";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        public IEnumerable<MaSanPham> GetExpiredBefore(DateTime date)
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE SO_LUONG > 0 AND CAST(NGAY_HET_HAN AS DATE) <= @ngay
ORDER BY NGAY_HET_HAN ASC;";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@ngay", SqlDbType.Date).Value = date.Date;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) yield return Map(rd);
                }
            }
        }

        // -------------------- Query (DataTable) --------------------

        public DataTable GetBySanPhamIdAvailable_Table(string idSanPham)
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE ID_SAN_PHAM = @id AND SO_LUONG > 0
ORDER BY NGAY_HET_HAN ASC, NGAY_NHAP ASC;";

            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idSanPham;
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetByPhieuNhapId_Table(string idPhieuNhap)
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE ID_PHIEU_NHAP = @id
ORDER BY ID;";

            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idPhieuNhap;
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetAllAvailable_Table()
        {
            const string sql = @"
SELECT ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP
FROM MA_SAN_PHAM
WHERE SO_LUONG > 0
ORDER BY ID_SAN_PHAM, NGAY_HET_HAN;";

            var dt = new DataTable();
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.Fill(dt);
            }
            return dt;
        }

        // -------------------- Command (dùng conn/tx từ BLL) --------------------

        public int UpdateQuantity(string idMa, int delta, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
UPDATE MA_SAN_PHAM
SET SO_LUONG = SO_LUONG + @delta
WHERE ID = @id;";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@delta", SqlDbType.Int).Value = delta;
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idMa;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Insert(MaSanPham msp, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
INSERT INTO MA_SAN_PHAM
    (ID, ID_SAN_PHAM, SO_LUONG, DON_GIA_NHAP, NGAY_NHAP, NGAY_SAN_XUAT, NGAY_HET_HAN, ID_PHIEU_NHAP)
VALUES
    (@id, @idSp, @sl, @gia, @ngayNhap, @nsx, @nhh, @idPn);";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = msp.Id;
                cmd.Parameters.Add("@idSp", SqlDbType.NVarChar, 50).Value = msp.IdSanPham;
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = msp.SoLuong;
                cmd.Parameters.Add("@gia", SqlDbType.BigInt).Value = msp.DonGiaNhap;
                cmd.Parameters.Add("@ngayNhap", SqlDbType.DateTime).Value = msp.NgayNhap;
                cmd.Parameters.Add("@nsx", SqlDbType.DateTime).Value = msp.NgaySanXuat;
                cmd.Parameters.Add("@nhh", SqlDbType.DateTime).Value = msp.NgayHetHan;
                cmd.Parameters.Add("@idPn", SqlDbType.NVarChar, 50).Value = msp.IdPhieuNhap ?? (object)DBNull.Value;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Update(MaSanPham msp, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
UPDATE MA_SAN_PHAM
SET ID_SAN_PHAM = @idSp,
    SO_LUONG    = @sl,
    DON_GIA_NHAP= @gia,
    NGAY_NHAP   = @ngayNhap,
    NGAY_SAN_XUAT = @nsx,
    NGAY_HET_HAN  = @nhh,
    ID_PHIEU_NHAP = @idPn
WHERE ID = @id;";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = msp.Id;
                cmd.Parameters.Add("@idSp", SqlDbType.NVarChar, 50).Value = msp.IdSanPham;
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = msp.SoLuong;
                cmd.Parameters.Add("@gia", SqlDbType.BigInt).Value = msp.DonGiaNhap;
                cmd.Parameters.Add("@ngayNhap", SqlDbType.DateTime).Value = msp.NgayNhap;
                cmd.Parameters.Add("@nsx", SqlDbType.DateTime).Value = msp.NgaySanXuat;
                cmd.Parameters.Add("@nhh", SqlDbType.DateTime).Value = msp.NgayHetHan;
                cmd.Parameters.Add("@idPn", SqlDbType.NVarChar, 50).Value = msp.IdPhieuNhap ?? (object)DBNull.Value;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Delete(string idMa, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"DELETE FROM MA_SAN_PHAM WHERE ID = @id;";
            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = idMa;
                return cmd.ExecuteNonQuery();
            }
        }

        // -------------------- Helper --------------------

        private static MaSanPham Map(IDataRecord rd)
        {
            var m = new MaSanPham();
            m.Id = rd["ID"].ToString();
            m.IdSanPham = rd["ID_SAN_PHAM"].ToString();
            m.SoLuong = rd["SO_LUONG"] == DBNull.Value ? 0 : Convert.ToInt32(rd["SO_LUONG"]);
            m.DonGiaNhap = rd["DON_GIA_NHAP"] == DBNull.Value ? 0 : Convert.ToInt64(rd["DON_GIA_NHAP"]);
            m.NgayNhap = rd["NGAY_NHAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rd["NGAY_NHAP"]);
            m.NgaySanXuat = rd["NGAY_SAN_XUAT"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rd["NGAY_SAN_XUAT"]);
            m.NgayHetHan = rd["NGAY_HET_HAN"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(rd["NGAY_HET_HAN"]);
            m.IdPhieuNhap = rd["ID_PHIEU_NHAP"] == DBNull.Value ? null : rd["ID_PHIEU_NHAP"].ToString();
            return m;
        }
    }
}
