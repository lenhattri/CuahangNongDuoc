using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuThanhToanFactory
    {
        static abstract long LayTongTien(string kh, int thang, int nam);
        void Add(DataRow row);
        DataTable DanhsachPhieuThanhToan();
        DataTable LayPhieuThanhToan(string id);
        DataRow NewRow();
        bool Save(SqlCommand cmd);
        DataTable TimPhieuThanhToan(string kh, DateTime ngay);
    }
}