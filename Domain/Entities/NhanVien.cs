using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Domain.Entities
{
    public class NhanVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public NguoiDung NguoiDung { get; set; }
    }
}
