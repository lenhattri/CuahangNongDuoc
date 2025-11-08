// DAL/DataLayer/SanPhamFactory.cs
using CuahangNongduoc.BusinessObject;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface ISanPhamFactory
    {
        void Add(DataRow row);
        DataTable DanhsachSanPham();
        bool Delete(string id);
        bool Insert(SanPham sp);
        DataTable LaySanPham(string id);
        DataTable LaySoLuongTon();
        DataRow NewRow();
        bool Save();
        DataTable TimMaSanPham(string id);
        DataTable TimTenSanPham(string ten);
        bool Update(SanPham sp);
    }
}