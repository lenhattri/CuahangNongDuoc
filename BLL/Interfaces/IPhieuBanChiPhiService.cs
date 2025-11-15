using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Domain.Entities;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IPhieuBanChiPhiService
    {
        DataTable LayDanhSachTheoPhieuBan(string maPhieuBan);
        IList<ChiPhiPhatSinh> LayDanhSachTheoPhieu(string maPhieuBan);
        void LuuChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis);
        void CapNhatChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis);
        void Them(PhieuBanChiPhi phieuBanChiPhi);
    }
}
