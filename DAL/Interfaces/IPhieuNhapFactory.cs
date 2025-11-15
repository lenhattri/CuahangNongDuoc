using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuNhapFactory
    {
        void Add(DataRow row);
        DataTable DanhsachPhieuNhap();
        DataTable LayPhieuNhap(string id);
        DataRow NewRow();
        bool Save();
        DataTable TimPhieuNhap(string maNCC, DateTime ngay);
    }
}