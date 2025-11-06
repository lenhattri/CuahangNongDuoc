// DAL/DataLayer/ChiTietPhieuNhapFactory.cs
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IChiTietPhieuNhapFactory
    {
        void Add(DataRow row);
        DataTable LayChiTietPhieuNhap(string id);
        void LoadSchema();
        DataRow NewRow();
        bool Save();
        int XoaChiTietPhieuNhap(string id);
    }
}