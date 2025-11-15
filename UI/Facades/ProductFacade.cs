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
    /// while delegating business rules to <see cref="IProductService"/>.
    /// </summary>
    public class ProductFacade
    {
        private readonly IProductService _productService;
        private readonly IUnitService _unitService;

        public ProductFacade(IProductService productService, IUnitService unitService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));
        }

        public void BindAutoComplete(ComboBox comboBox)
        {
            var table = _productService.GetProducts();
            comboBox.DataSource = table;
            comboBox.DisplayMember = "TEN_SAN_PHAM";
            comboBox.ValueMember = "ID";
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public DataGridViewComboBoxColumn CreateProductColumn()
        {
            var table = _productService.GetProducts();
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
            var bindingSource = new BindingSource { DataSource = _productService.GetProducts() };

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

        public DataTable SearchByCode(string code) => _productService.FindByCode(code);

        public DataTable SearchByName(string name) => _productService.FindByName(name);

        public DataRow CreateRow() => _productService.CreateRow();

        public void Add(DataRow row) => _productService.Add(row);

        public bool SaveChanges() => _productService.SaveChanges();

        public SanPham GetProduct(string id) => _productService.GetProduct(id);

        public IList<SoLuongTon> GetInventoryLevels() => _productService.GetInventoryLevels();

        public void UpdatePurchasePrice(string id, long newPrice, long quantity) => _productService.UpdatePurchasePrice(id, newPrice, quantity);

        public void RefreshUnitCombo(ComboBox comboBox)
        {
            var table = _unitService.GetUnits();
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
