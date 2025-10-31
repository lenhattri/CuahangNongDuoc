using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class ChiTietPhieuNhapReport
    {
        public string Id { get; set; }
        public string TenSanPham { get; set; }
        public long GiaNhap { get; set; }
        public int SoLuong { get; set; }
        public long ThanhTien { get; set; }
        public DateTime NgaySanXuat { get; set; }
        public DateTime NgayHetHan { get; set; }
    }
}
