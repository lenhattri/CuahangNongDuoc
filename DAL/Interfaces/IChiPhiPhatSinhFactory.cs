using CuahangNongduoc.BusinessObject;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IChiPhiPhatSinhFactory
    {
        DataTable DanhSachChiPhiPhatSinh();
        int Delete(string id);
        int InSert(ChiPhiPhatSinh chiPhi);
        int Update(ChiPhiPhatSinh chiPhi);
    }
}