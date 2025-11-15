using System;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IPhieuNhapService
    {
        DataTable DanhSachPhieuNhap();
        DataRow TaoDongMoi();
        void Them(DataRow row);
        bool Luu();
        DataTable TimPhieuNhap(string maNhaCungCap, DateTime ngayNhap);
        PhieuNhap LayPhieuNhap(string id);
    }
}
