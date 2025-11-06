// DAL/DataLayer/DonViTinhDAL.cs
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IDonViTinhDAL
    {
        DataTable DanhSachDVT();
        int Delete(int id);
        int Insert(string ten, string ghiChu = null);
        DataTable LayDVT(int id);
        bool Save(DataTable table);
        int Update(int id, string ten, string ghiChu = null);
    }
}