using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    /// <summary>
    /// Exposes product-centric business logic without Windows Forms dependencies.
    /// </summary>
    public interface ISanPhamService
    {
        DataTable GetProducts();
        DataTable FindByCode(string code);
        DataTable FindByName(string name);
        SanPham GetProduct(string id);
        IList<SoLuongTon> GetInventoryLevels();
        void UpdatePurchasePrice(string id, long newPrice, long quantity);
        DataRow CreateRow();
        void Add(DataRow row);
        bool SaveChanges();
    }
}
