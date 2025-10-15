using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class ChiTietPhieuNhap
    {
        private PhieuNhap m_PhieuNhap;

        public PhieuNhap PhieuNhap
        {
            get { return m_PhieuNhap; }
            set { m_PhieuNhap = value; }
        }
        private MaSanPham m_MaSP;

        public MaSanPham MaSanPham
        {
            get { return m_MaSP; }
            set { m_MaSP = value; }
        }

        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }   
        public decimal ThanhTien { get; set; }   

    }
}
