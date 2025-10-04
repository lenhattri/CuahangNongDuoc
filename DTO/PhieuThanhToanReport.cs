using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.BusinessObject
{
    public class PhieuThanhToanReport
    {
        public string Id { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public long TongTien { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string GhiChu { get; set; }
    }
}
