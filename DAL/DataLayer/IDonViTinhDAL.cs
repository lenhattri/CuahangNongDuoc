//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.OleDb;

using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface IDonViTinhDAL
    {
        static abstract IDonViTinhDAL Create();
        DataTable DanhSachDVT();
        int Delete(int id);
        int Insert(string ten, string ghiChu = null);
        DataTable LayDVT(int id);
        bool Save(DataTable table);
        int Update(int id, string ten, string ghiChu = null);
    }
}