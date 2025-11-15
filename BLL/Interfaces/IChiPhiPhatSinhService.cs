using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IChiPhiPhatSinhService
    {
        DataTable DanhSachChiPhiPhatSinh();
        void Them(ChiPhiPhatSinh chiPhi);
        void CapNhat(ChiPhiPhatSinh chiPhi);
        void Xoa(string id);
    }
}
