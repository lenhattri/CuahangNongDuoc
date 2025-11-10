// DAL/DataLayer/MaSanPhanFactory.cs
using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IMaSanPhanFactory
    {
         void CapNhatSoLuong(string masp, int so_luong);
        void Add(DataRow row);
        DataTable DanhsachChiTiet(string sp);
        DataTable DanhsachMaSanPham();
        DataTable DanhsachMaSanPham(string sp);
        DataTable DanhsachMaSanPhamHetHan(DateTime dt);
        DataTable LayMaSanPham(string idMaSanPham);
        DataTable LaySanPham(string idMaSanPham);
        void LoadSchema();
        DataRow NewRow();
        bool Save();
    }
}