// DAL/DataLayer/PhieuThanhToanDAL.cs
using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuThanhToanDAL
    {
        static abstract long LayTongTien(string kh, int thang, int nam);
        void Add(DataRow row);
        DataTable DanhsachPhieuThanhToan();
        DataTable LayPhieuThanhToan(string id);
        DataRow NewRow();
        bool Save();
        DataTable TimPhieuThanhToan(string kh, DateTime ngay);
    }
}