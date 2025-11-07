using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IKhachHangFactory
    {
        void Add(DataRow row);
        DataTable DanhsachKhachHang();
        DataTable DanhsachKhachHang(bool loai);
        DataTable LayKhachHang(string id);
        DataRow NewRow();
        DataTable TimDiaChi(string diachi, bool loai);
        DataTable TimHoTen(string hoten, bool loai);
    }
}