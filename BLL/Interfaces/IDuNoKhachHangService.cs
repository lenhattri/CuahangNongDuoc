using System;
using System.Data;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IDuNoKhachHangService
    {
        DataTable TongHop(int thang, int nam, Action<int, int> capNhatTienDo = null);
        bool Luu();
    }
}
