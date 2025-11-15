using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IChiTietPhieuNhapService
    {
        void ThemChiTiet(string idPhieuNhap, string idMaSanPham);
        int XoaChiTiet(string idPhieuNhap);
        IList<ChiTietPhieuNhap> LayChiTiet(string idPhieuNhap);
        DataTable LayBangChiTiet(string idPhieuNhap);
    }
}
