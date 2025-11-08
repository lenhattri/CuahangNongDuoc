using CuahangNongduoc.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.Domain.Entities
{
    public class PhieuBanChiPhi
    {
        
        private PhieuBan m_PhieuBan;
        public PhieuBan PhieuBan
        {
            get { return m_PhieuBan; }
            set { m_PhieuBan = value; }
        }
        private ChiPhiPhatSinh m_ChiPhiPhatSinh;
        public ChiPhiPhatSinh ChiPhiPhatSinh
        {
            get { return m_ChiPhiPhatSinh; }
            set { m_ChiPhiPhatSinh = value; }
        }
    }
}
