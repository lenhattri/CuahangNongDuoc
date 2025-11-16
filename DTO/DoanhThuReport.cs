using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class DoanhThuReport
    {
        public string TenSanPham { get; set; }
        public string MaSanPham { get; set; }
        public int SoLuongBan { get; set; }
        public long DoanhThu { get; set; }
        public long GiaVon { get; set; }
        public long LoiNhuan { get; set; }
    }
}
