using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface ILyDoChiService
    {
        DataTable DanhSachLyDo();
        LyDoChi LayLyDo(long id);
        DataRow TaoDongMoi();
        void Them(DataRow row);
        bool Luu();
    }
}
