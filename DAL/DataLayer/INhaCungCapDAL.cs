using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface INhaCungCapDAL
    {
        void Add(DataRow row);
        DataTable DanhsachNCC();
        DataTable LayNCC(string id);
        DataRow NewRow();
        bool Save(SqlCommand cmd);
        DataTable TimDiaChi(string diachi);
        DataTable TimHoTen(string hoten);
    }
}