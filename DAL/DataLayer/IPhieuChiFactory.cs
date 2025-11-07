using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface IPhieuChiFactory
    {
         long LayTongTien(string lydo, int thang, int nam);
        void Add(DataRow row);
        DataTable DanhsachPhieuChi();
        int Insert(DataRow row, SqlTransaction tx = null);
        DataTable LayPhieuChi(string id);
        DataRow NewRow();
        bool Save(SqlCommand cmd);
        DataTable TimPhieuChi(int lydo, DateTime ngay);
        int Update(DataRow row, SqlTransaction tx = null);
    }
}