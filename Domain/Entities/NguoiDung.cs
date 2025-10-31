using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Domain.Entities
{
    public enum QuyenNguoiDung
    {
        Admin,
        NhanVien,
    }
    public class NguoiDung
    {
        // id vừa là khóa chính, vừa là khóa ngoại tham chiếu đến bảng nhân viên
        public int Id { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public QuyenNguoiDung VaiTro { get; set; }
        // Thông tin chi tiết sẽ được lưu trong bảng NHAN_VIEN
    }
}
