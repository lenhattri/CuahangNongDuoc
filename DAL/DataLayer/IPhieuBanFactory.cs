using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuBanFactory
    {
        void Add(DataRow row);
        DataTable DanhsachPhieuBanLe();
        DataTable DanhsachPhieuBanSi();
        DataTable LayChiTietPhieuBan(string idPhieuBan);
        long LayConNo(string kh, int thang, int nam);
        DataTable LayPhieuBan(string id);
        int LaySoPhieu();
        DataRow NewRow();
        bool Save(SqlCommand cmd);
        DataTable TimPhieuBan(string idKh, DateTime dt);
    }
}