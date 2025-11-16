// DAL/DataLayer/KhachHangFactory.cs
using System.Data;

namespace CuahangNongduoc.DAL.Interfaces
{
    public interface IKhachHangFactory
    {
        void Add(DataRow row);
        DataTable DanhsachKhachHang();
        DataTable DanhsachKhachHang(bool loai);
        DataTable LayKhachHang(string id);
        DataRow NewRow();
        bool Save();
        DataTable TimDiaChi(string diachi, bool loai);
        DataTable TimHoTen(string hoten, bool loai);
    }
}