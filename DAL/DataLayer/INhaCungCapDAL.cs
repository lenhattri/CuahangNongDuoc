// DAL/DataLayer/NhaCungCapFactory.cs
using CuahangNongduoc.BusinessObject;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface INhaCungCapDAL
    {
        void Add(DataRow row);
        DataTable DanhsachNCC();
        bool Delete(string id);
        bool Insert(NhaCungCap ncc);
        DataTable LayNCC(string id);
        DataRow NewRow();
        bool Save();
        DataTable TimDiaChi(string diachi);
        DataTable TimHoTen(string hoten);
        bool Update(NhaCungCap ncc);
    }
}