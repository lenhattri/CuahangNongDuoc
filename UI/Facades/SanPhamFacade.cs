using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.UI.Facades
{
    /// <summary>
    /// UI façade that encapsulates Windows Forms binding logic for products
    /// while delegating business rules to <see cref="ISanPhamService"/>.
    /// </summary>
    public class SanPhamFacade
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly IDonViTinhService _donViTinhService;

        public SanPhamFacade(ISanPhamService sanPhamService, IDonViTinhService donViTinhService)
        {
            _sanPhamService = sanPhamService ?? throw new ArgumentNullException(nameof(sanPhamService));
            _donViTinhService = donViTinhService ?? throw new ArgumentNullException(nameof(donViTinhService));
        }

        public void BindAutoComplete(ComboBox comboBox)
        {
            var table = _sanPhamService.GetProducts();
            comboBox.DataSource = table;
            comboBox.DisplayMember = "TEN_SAN_PHAM";
            comboBox.ValueMember = "ID";
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public DataGridViewComboBoxColumn CreateProductColumn()
        {
            var table = _sanPhamService.GetProducts();
            return new DataGridViewComboBoxColumn
            {
                DataSource = table,
                DisplayMember = "TEN_SAN_PHAM",
                ValueMember = "ID",
                DataPropertyName = "ID_SAN_PHAM",
                AutoComplete = true,
                HeaderText = "Sản phẩm"
            };
        }

        public BindingSource BindProducts(
            DataGridView grid,
            BindingNavigator navigator,
            TextBox txtMaSp,
            TextBox txtTenSp,
            ComboBox cmbDvt,
            NumericUpDown numSoLuong,
            NumericUpDown numDonGiaNhap,
            NumericUpDown numGiaBanSi,
            NumericUpDown numGiaBanLe)
        {
            var bindingSource = new BindingSource { DataSource = _sanPhamService.GetProducts() };

            BindTextBox(txtMaSp, bindingSource, "ID");
            BindTextBox(txtTenSp, bindingSource, "TEN_SAN_PHAM");
            BindComboBox(cmbDvt, bindingSource, "ID_DON_VI_TINH");
            BindNumeric(numSoLuong, bindingSource, "SO_LUONG");
            BindNumeric(numDonGiaNhap, bindingSource, "DON_GIA_NHAP");
            BindNumeric(numGiaBanSi, bindingSource, "GIA_BAN_SI");
            BindNumeric(numGiaBanLe, bindingSource, "GIA_BAN_LE");

            if (navigator != null)
            {
                navigator.BindingSource = bindingSource;
            }

            grid.DataSource = bindingSource;
            return bindingSource;
        }

        public DataTable SearchByCode(string code) => _sanPhamService.FindByCode(code);

        public DataTable SearchByName(string name) => _sanPhamService.FindByName(name);

        public DataRow CreateRow() => _sanPhamService.CreateRow();

        public void Add(DataRow row) => _sanPhamService.Add(row);

        public bool SaveChanges() => _sanPhamService.SaveChanges();

        public SanPham GetProduct(string id) => _sanPhamService.GetProduct(id);

        public IList<SoLuongTon> GetInventoryLevels() => _sanPhamService.GetInventoryLevels();

        public void UpdatePurchasePrice(string id, long newPrice, long quantity) => _sanPhamService.UpdatePurchasePrice(id, newPrice, quantity);

        public void RefreshUnitCombo(ComboBox comboBox)
        {
            var table = _donViTinhService.GetUnits();
            comboBox.DataSource = table;
            comboBox.DisplayMember = table.Columns.Contains("TEN_DON_VI") ? "TEN_DON_VI" : "TEN";
            comboBox.ValueMember = table.Columns.Contains("ID") ? "ID" : table.Columns[0].ColumnName;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private static void BindTextBox(TextBox textBox, BindingSource source, string dataMember)
        {
            if (textBox == null)
            {
                return;
            }

            textBox.DataBindings.Clear();
            textBox.DataBindings.Add("Text", source, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void BindComboBox(ComboBox comboBox, BindingSource source, string dataMember)
        {
            if (comboBox == null)
            {
                return;
            }

            RefreshUnitCombo(comboBox);
            comboBox.DataBindings.Clear();
            comboBox.DataBindings.Add("SelectedValue", source, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private static void BindNumeric(NumericUpDown numeric, BindingSource source, string dataMember)
        {
            if (numeric == null)
            {
                return;
            }

            numeric.DataBindings.Clear();
            numeric.DataBindings.Add("Value", source, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
