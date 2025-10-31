using CuahangNongduoc.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class PhieuBanReport
    {
        public string Id { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public DateTime NgayBan { get; set; }
        public long TongTien { get; set; }
        public long DaTra { get; set; }
        public long ConNo { get; set; }
    }
}
