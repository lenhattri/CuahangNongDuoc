// DAL/DataLayer/UserDAL.cs
using System.Data;

namespace CuahangNongduoc.DAL.Interfaces
{
    public interface IUserDAL
    {
        void Add(DataRow row);
        void Delete(long id);
        DataTable LayDanhSachNguoiDung();
        DataRow LayNguoiDungTheoId(string id);
        DataRow LayNguoiDungTheoTenDangNhap(string tenDangNhap);
        DataRow NewRow();
        bool Save();
        void Update(long id);
    }
}