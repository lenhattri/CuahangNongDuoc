using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.DataLayer;
using CuahangNongduoc.Domain.Entities;

namespace CuahangNongduoc.BLL.Services
{
    public class PhieuBanChiPhiService : IPhieuBanChiPhiService
    {
        private readonly IPhieuBanChiPhiFactory _factory;

        public PhieuBanChiPhiService(IPhieuBanChiPhiFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public DataTable LayDanhSachTheoPhieuBan(string maPhieuBan)
            => _factory.LayDanhSachTheoPhieuBan(maPhieuBan);

        public IList<ChiPhiPhatSinh> LayDanhSachTheoPhieu(string maPhieuBan)
        {
            var table = _factory.LayDanhSachTheoPhieuBan(maPhieuBan);
            var result = new List<ChiPhiPhatSinh>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new ChiPhiPhatSinh
                {
                    Id = Convert.ToString(row["ID"]),
                    TenChiPhi = Convert.ToString(row["TEN_CHI_PHI"]),
                    LoaiChiPhi = Convert.ToString(row["LOAI_CHI_PHI"]),
                    SoTien = Convert.ToInt64(row["SO_TIEN"])
                });
            }

            return result;
        }

        public void LuuChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis)
            => _factory.LuuChiPhiPhatSinh(maPhieuBan, chiPhis);

        public void CapNhatChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis)
        {
            _factory.DeletedByPhieuBan(maPhieuBan);
            _factory.LuuChiPhiPhatSinh(maPhieuBan, chiPhis);
        }

        public void Them(PhieuBanChiPhi phieuBanChiPhi)
        {
            if (phieuBanChiPhi == null)
            {
                throw new ArgumentNullException(nameof(phieuBanChiPhi));
            }

            _factory.Insert(phieuBanChiPhi);
        }
    }
}
