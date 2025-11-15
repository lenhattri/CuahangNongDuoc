using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    /// <summary>
    /// Abstraction for unit of measurement operations.
    /// </summary>
    public interface IDonViTinhService
    {
        DataTable GetUnits();
        DonViTinh GetUnit(int id);
        bool SaveChanges();
    }
}
