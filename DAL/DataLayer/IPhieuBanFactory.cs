// DAL/DataLayer/PhieuBanFactory.cs
using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuBanFactory
    {
        static abstract long LayConNo(string kh, int thang, int nam);
        static abstract int LaySoPhieu();
        void Add(DataRow row);
        DataTable DanhsachPhieu();
        DataTable DanhsachPhieuBanLe();
        DataTable DanhsachPhieuBanSi();
        DataTable LayChiTietPhieuBan(string idPhieuBan);
        DataTable LayPhieuBan(string id);
        DataRow NewRow();
        bool Save();
        DataTable TimPhieuBan(string idKh, DateTime dt);
    }
}