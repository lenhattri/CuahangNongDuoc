using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class PhieuBan
    {
        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private KhachHang m_KH;

        public KhachHang KhachHang
        {
            get { return m_KH; }
            set { m_KH = value; }
        }

        private DateTime m_NgayBan;

        public DateTime NgayBan
        {
            get { return m_NgayBan; }
            set { m_NgayBan = value; }
        }
        private long m_TongTien;

        public long TongTien
        {
            get { return m_TongTien; }
            set { m_TongTien = value; }
        }
        private long m_DaTra;

        public long DaTra
        {
            get { return m_DaTra; }
            set { m_DaTra = value; }
        }
        private long m_ConNo;

        public long ConNo
        {
            get { return m_ConNo; }
            set { m_ConNo = value; }
        }
        //Thêm phí vận chuyển 
        private long m_ChiPhiVanChuyen;
        public long ChiPhiVanChuyen
        {
            get { return m_ChiPhiVanChuyen; }
            set { m_ChiPhiVanChuyen = value; }
        }
        //Thêm phí dịch vụ
        private long m_PhiDichVu;
        public long PhiDichVu
        {
            get { return m_PhiDichVu; }
            set { m_PhiDichVu = value; }
        }
        //Thêm giảm giá
        private long m_GiamGia;
        public long GiamGia
        {
            get { return m_GiamGia; }
            set { m_GiamGia = value; }
        }

        private IList<ChiTietPhieuBan> m_ChiTiet;

        public IList<ChiTietPhieuBan> ChiTiet
        {
            get { return m_ChiTiet; }
            set { m_ChiTiet = value; }
        }

	
    }
}
