using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface INhaCungCapService
    {
        DataTable DanhSachNhaCungCap();
        DataTable TimTheoHoTen(string hoTen);
        DataTable TimTheoDiaChi(string diaChi);
        NhaCungCap LayNhaCungCap(string id);
        IList<NhaCungCap> LayTatCa();
        DataRow TaoDongMoi();
        void Them(DataRow row);
        bool Luu();
    }
}
