using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface ILyDoChiFactory
    {
        void Add(DataTable dataTable, DataRow row);
        DataTable DanhsachLyDo();
        int Delete(long id, SqlTransaction tx = null);
        int Insert(DataRow row, SqlTransaction tx = null);
        DataTable LayLyDoChi(long id);
        DataRow NewRow();
        bool SaveChanges(DataTable table);
        int Update(DataRow row, SqlTransaction tx = null);
    }
}