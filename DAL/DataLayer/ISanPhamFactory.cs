using CuahangNongduoc.BusinessObject;
using System.Data;
using System.Data.SqlClient;

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
        bool Save(SqlCommand cmd);
        DataTable TimMaSanPham(string id);
        DataTable TimTenSanPham(string ten);
        bool Update(SanPham sp);
    }
}