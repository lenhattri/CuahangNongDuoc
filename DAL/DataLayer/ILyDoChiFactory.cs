using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface ILyDoChiFactory
    {
        DataTable LayLyDoChi(long id);
        bool Save(SqlCommand cmd);
    }
}