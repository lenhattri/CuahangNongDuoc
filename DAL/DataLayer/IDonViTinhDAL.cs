// DAL/DataLayer/DonViTinhDAL.cs
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IDonViTinhDAL
    {
        DataTable DanhSachDVT();
        DataTable LayDVT(int id);
        bool Save();
    }
}