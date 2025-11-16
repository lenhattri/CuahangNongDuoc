using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Domain.Entities;
using System.Collections.Generic;
using System.Data;

namespace CuahangNongduoc.DAL.Interfaces
{
    public interface IPhieuBanChiPhiFactory
    {
        void DeletedByPhieuBan(string maPhieuBan);
        int Insert(PhieuBanChiPhi phieuBanChiPhi);
        DataTable LayDanhSachTheoPhieuBan(string maPhieuBan);
        void LuuChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis);
    }
}