// DAL/DataLayer/LyDoChiFactory.cs
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    public interface ILyDoChiFactory
    {
        void Add(DataRow row);
        DataTable DanhsachLyDo();
        DataTable LayLyDoChi(long id);
        DataRow NewRow();
        bool Save();
    }
}