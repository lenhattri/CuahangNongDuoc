using CuahangNongduoc.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.BusinessObject
{
    public class ChiPhiPhatSinh
    {
        public ChiPhiPhatSinh() { }

        private String m_Id;
        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        private String m_TenChiPhi;
        public String TenChiPhi
        {
            get { return m_TenChiPhi; }
            set { m_TenChiPhi = value; }
        }
        private String m_LoaiChiPhi;
        public String LoaiChiPhi
        {
            get { return m_LoaiChiPhi; }
            set { m_LoaiChiPhi = value; }
        }
        private Decimal m_SoTien;
        public Decimal SoTien
        {
            get { return m_SoTien; }
            set { m_SoTien = value; }
        }

    }
}