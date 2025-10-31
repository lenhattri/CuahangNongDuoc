using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class DuNoKhachHangReport
    {
        public String TenKhachHang { get; set; }
        public String DiaChi { get; set; }
        public String DienThoai { get; set; }
        public long DauKy { get; set; }
        public long PhatSinh { get; set; }
        public long DaTra { get; set; }
        public long CuoiKy { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
