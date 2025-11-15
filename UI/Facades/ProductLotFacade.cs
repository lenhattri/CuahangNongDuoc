using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.UI.Facades
{
    public class ProductLotFacade
    {
        private readonly IProductLotService _productLotService;

        public ProductLotFacade(IProductLotService productLotService)
        {
            _productLotService = productLotService ?? throw new ArgumentNullException(nameof(productLotService));
        }

        public void ConfigureAutoComplete(ComboBox comboBox, string productId)
        {
            var table = _productLotService.GetProductLots(productId);
            comboBox.DataSource = table;
            comboBox.DisplayMember = "ID";
            comboBox.ValueMember = "ID";
        }

        public void ConfigureColumn(DataGridViewComboBoxColumn column)
        {
            var table = _productLotService.GetProductLots();
            column.DataSource = table;
            column.DisplayMember = "ID";
            column.ValueMember = "ID";
            column.DataPropertyName = "ID_MA_SAN_PHAM";
            column.HeaderText = "Mã sản phẩm";
        }

        public void BindReceiptDetails(string receiptId, DataGridView grid)
        {
            grid.DataSource = _productLotService.GetProductLotDetails(receiptId);
        }

        public SanPham GetProductForLot(string lotId) => _productLotService.GetProductForLot(lotId);

        public MaSanPham GetProductLot(string lotId) => _productLotService.GetProductLot(lotId);

        public IList<MaSanPham> GetExpiredLots(DateTime referenceDate) => _productLotService.GetExpiredLots(referenceDate);

        public IList<MaSanPham> GetReceiptLots(string receiptId) => _productLotService.GetReceiptDetails(receiptId);

        public void UpdateQuantity(string lotId, int deltaQuantity) => _productLotService.UpdateQuantity(lotId, deltaQuantity);

        public DataRow CreateRow() => _productLotService.CreateRow();

        public void Add(DataRow row) => _productLotService.Add(row);

        public bool SaveChanges() => _productLotService.SaveChanges();
    }
}
