using System;

namespace CuahangNongduoc.Entities
{
    public sealed class SanPham
    {
        public string Id { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public long DonGiaNhap { get; set; }
        public long GiaBanSi { get; set; }
        public long GiaBanLe { get; set; }
        public string IdDonViTinh { get; set; }
    }
}
