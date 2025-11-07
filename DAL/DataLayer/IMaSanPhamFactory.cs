// DAL/DataLayer/MaSanPhanFactory.cs
using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IMaSanPhamFactory
    {
        void Add(DataRow row);
        void CapNhatSoLuong(string masp, int so_luong);
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