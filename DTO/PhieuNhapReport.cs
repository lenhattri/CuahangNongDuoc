using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class PhieuNhapReport
    {
        public String Id { get; set; }
        public DateTime NgayNhap { get; set; }
        public long TongTien { get; set; }
        public string TenNhaCungCap { get; set; }
    }
}
