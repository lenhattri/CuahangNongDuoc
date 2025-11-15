using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IKhachHangService
    {
        DataTable DanhSachKhachHang(bool? laDaiLy = null);
        DataTable TimTheoHoTen(string hoTen, bool laDaiLy);
        DataTable TimTheoDiaChi(string diaChi, bool laDaiLy);
        KhachHang LayKhachHang(string id);
        IList<KhachHang> LayTatCa();
        DataRow TaoDongMoi();
        void Them(DataRow row);
        bool Luu();
    }
}
