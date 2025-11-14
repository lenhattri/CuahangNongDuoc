using CuahangNongduoc.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class SanPhamHetHanReport
    {
        public string Id { get; set; }
        public string GiaNhap { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime NgaySanXuat { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string TenSanPham { get; set; }
    }
}
