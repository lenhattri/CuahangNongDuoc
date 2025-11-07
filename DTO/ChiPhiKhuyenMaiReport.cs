using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.DTO
{
    public class ChiPhiKhuyenMaiReport
    {
        public int Id {  get; set; }
        public string TenNguoiLap {  get; set; }
        public DateTime NgayLap { get; set; }
        public decimal ChiPhiVanChuyen { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongTien { get; set; }
        public decimal TongTienSauGiamGia { get; set; }
    }
}
