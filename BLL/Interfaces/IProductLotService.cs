using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    /// <summary>
    /// Business abstraction for product lots (ma san pham).
    /// </summary>
    public interface IProductLotService
    {
        DataTable GetProductLots(string productId = null);
        DataTable GetProductLotDetails(string receiptId);
        SanPham GetProductForLot(string lotId);
        MaSanPham GetProductLot(string lotId);
        IList<MaSanPham> GetExpiredLots(DateTime referenceDate);
        IList<MaSanPham> GetReceiptDetails(string receiptId);
        void UpdateQuantity(string lotId, int deltaQuantity);
        DataRow CreateRow();
        void Add(DataRow row);
        bool SaveChanges();
    }
}
