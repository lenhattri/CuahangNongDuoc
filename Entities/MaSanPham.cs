using System;

namespace CuahangNongduoc.Entities
{
    public sealed class MaSanPham
    {
        public string Id { get; set; }              // MA_SAN_PHAM.ID
        public string IdSanPham { get; set; }       // MA_SAN_PHAM.ID_SAN_PHAM
        public int SoLuong { get; set; }            // MA_SAN_PHAM.SO_LUONG
        public long DonGiaNhap { get; set; }        // MA_SAN_PHAM.DON_GIA_NHAP
        public DateTime NgayNhap { get; set; }      // MA_SAN_PHAM.NGAY_NHAP
        public DateTime NgaySanXuat { get; set; }   // MA_SAN_PHAM.NGAY_SAN_XUAT
        public DateTime NgayHetHan { get; set; }    // MA_SAN_PHAM.NGAY_HET_HAN
        public string IdPhieuNhap { get; set; }     // MA_SAN_PHAM.ID_PHIEU_NHAP
    }
}
