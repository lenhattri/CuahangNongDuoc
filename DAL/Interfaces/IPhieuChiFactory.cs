// DAL/DataLayer/PhieuChiFactory.cs
using System;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DAL.Interfaces
{
    public interface IPhieuChiFactory
    {
        long LayTongTien(int lydo, int thang, int nam);
        void Add(DataRow row);
        DataTable DanhsachPhieuChi();
        int Insert(DataRow row, SqlTransaction tx = null);
        DataTable LayPhieuChi(string id);
        DataRow NewRow();
        bool Save();
        DataTable TimPhieuChi(int lydo, DateTime ngay);
        int Update(DataRow row, SqlTransaction tx = null);
    }
}