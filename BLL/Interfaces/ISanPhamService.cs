using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface ISanPhamService
    {
        DataTable LoadAll();
        DataTable FindByIdLike(string id);
        DataTable FindByNameLike(string name);
        SanPham GetById(string id);
        IList<SoLuongTon> GetInventory();
        void UpdateAverageCost(string productId, long newCost, long quantityChange);
        DataRow CreateRow();
        void AddRow(DataRow row);
        bool SaveChanges();
    }
}
