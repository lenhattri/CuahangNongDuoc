// DAL/DataLayer/PhieuBanFactory.cs
using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuBanFactory
    {
        long LayConNo(string kh, int thang, int nam);
         int LaySoPhieu();
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