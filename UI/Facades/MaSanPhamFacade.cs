using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.UI.Facades
{
    public class MaSanPhamFacade
    {
        private readonly IMaSanPhamService _maSanPhamService;

        public MaSanPhamFacade(IMaSanPhamService maSanPhamService)
        {
            _maSanPhamService = maSanPhamService ?? throw new ArgumentNullException(nameof(maSanPhamService));
        }

        public void ConfigureAutoComplete(ComboBox comboBox, string productId)
        {
            var table = _maSanPhamService.GetProductLots(productId);
            comboBox.DataSource = table;
            comboBox.DisplayMember = "ID";
            comboBox.ValueMember = "ID";
        }

        public void ConfigureColumn(DataGridViewComboBoxColumn column)
        {
            var table = _maSanPhamService.GetProductLots();
            column.DataSource = table;
            column.DisplayMember = "ID";
            column.ValueMember = "ID";
            column.DataPropertyName = "ID_MA_SAN_PHAM";
            column.HeaderText = "Mã sản phẩm";
        }

        public void BindReceiptDetails(string receiptId, DataGridView grid)
        {
            grid.DataSource = _maSanPhamService.GetProductLotDetails(receiptId);
        }

        public SanPham GetProductForLot(string lotId) => _maSanPhamService.GetProductForLot(lotId);

        public MaSanPham GetProductLot(string lotId) => _maSanPhamService.GetProductLot(lotId);

        public IList<MaSanPham> GetExpiredLots(DateTime referenceDate) => _maSanPhamService.GetExpiredLots(referenceDate);

        public IList<MaSanPham> GetReceiptLots(string receiptId) => _maSanPhamService.GetReceiptDetails(receiptId);

        public void UpdateQuantity(string lotId, int deltaQuantity) => _maSanPhamService.UpdateQuantity(lotId, deltaQuantity);

        public DataRow CreateRow() => _maSanPhamService.CreateRow();

        public void Add(DataRow row) => _maSanPhamService.Add(row);

        public bool SaveChanges() => _maSanPhamService.SaveChanges();
    }
}
