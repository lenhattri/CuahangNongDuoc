using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuNhapFactory
    {
        void Add(DataRow row);
        DataTable DanhsachPhieuNhap();
        DataTable LayPhieuNhap(string id);
        void LoadSchema();
        DataRow NewRow();
        bool Save(SqlCommand cmd);
        DataTable TimPhieuNhap(string maNCC, DateTime dt);
    }
}