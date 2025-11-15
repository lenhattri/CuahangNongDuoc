using System;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class ChiPhiPhatSinhService : IChiPhiPhatSinhService
    {
        private readonly IChiPhiPhatSinhFactory _dal;

        public ChiPhiPhatSinhService(IChiPhiPhatSinhFactory dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DataTable DanhSachChiPhiPhatSinh() => _dal.DanhSachChiPhiPhatSinh();

        public void Them(ChiPhiPhatSinh chiPhi)
        {
            if (chiPhi == null)
            {
                throw new ArgumentNullException(nameof(chiPhi));
            }

            _dal.InSert(chiPhi);
        }

        public void CapNhat(ChiPhiPhatSinh chiPhi)
        {
            if (chiPhi == null)
            {
                throw new ArgumentNullException(nameof(chiPhi));
            }

            _dal.Update(chiPhi);
        }

        public void Xoa(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Giá trị không hợp lệ", nameof(id));
            }

            _dal.Delete(id);
        }
    }
}
