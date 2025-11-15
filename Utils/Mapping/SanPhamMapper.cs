using System;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Utils.Mapping
{
    public sealed class SanPhamMapper : IDataRowMapper<SanPham>
    {
        private readonly Func<int, DonViTinh> _donViTinhResolver;

        public SanPhamMapper(Func<int, DonViTinh> donViTinhResolver)
        {
            _donViTinhResolver = donViTinhResolver ?? throw new ArgumentNullException(nameof(donViTinhResolver));
        }

        public SanPham Map(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var sanPham = new SanPham
            {
                Id = Convert.ToString(row["ID"]),
                TenSanPham = Convert.ToString(row["TEN_SAN_PHAM"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                DonGiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                GiaBanLe = Convert.ToInt64(row["GIA_BAN_LE"]),
                GiaBanSi = Convert.ToInt64(row["GIA_BAN_SI"])
            };

            if (row.Table.Columns.Contains("ID_DON_VI_TINH") && row["ID_DON_VI_TINH"] != DBNull.Value)
            {
                sanPham.DonViTinh = _donViTinhResolver(Convert.ToInt32(row["ID_DON_VI_TINH"]));
            }

            return sanPham;
        }
    }
}
