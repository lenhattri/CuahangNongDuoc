// DAL/DataLayer/DuNoKhachHangFactory.cs
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IDuNoKhachHangDAL
    {
        static abstract long LayDuNo(string kh, int thang, int nam);
        void Add(DataRow row);
        void Clear(int thang, int nam);
        DataTable DanhsachDuNo(int thang, int nam);
        DataTable LayDuNoKhachHang(string kh, int thang, int nam);
        void LoadSchema();
        DataRow NewRow();
        bool Save();
    }
}