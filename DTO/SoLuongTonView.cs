using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class SoLuongTonView
    {
        public string MaSP { get; set; }          // string cho chắc
        public string TenSanPham { get; set; }
        public long DonGiaNhap { get; set; }      // hoặc decimal
        public long GiaBanLe { get; set; }
        public long GiaBanSi { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuong { get; set; }
    }
}
