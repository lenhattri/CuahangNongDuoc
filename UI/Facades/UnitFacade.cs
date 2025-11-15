using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.UI.Facades
{
    public class UnitFacade
    {
        private readonly IUnitService _unitService;

        public UnitFacade(IUnitService unitService)
        {
            _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));
        }

        public BindingSource BindUnits(DataGridView grid, BindingNavigator navigator)
        {
            var source = new BindingSource { DataSource = _unitService.GetUnits() };
            if (navigator != null)
            {
                navigator.BindingSource = source;
            }

            grid.DataSource = source;
            return source;
        }

        public void ConfigureComboBox(ComboBox comboBox)
        {
            var table = _unitService.GetUnits();
            comboBox.DataSource = table;
            comboBox.DisplayMember = table.Columns.Contains("TEN_DON_VI") ? "TEN_DON_VI" : "TEN";
            comboBox.ValueMember = table.Columns.Contains("ID") ? "ID" : table.Columns[0].ColumnName;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public DataGridViewComboBoxColumn CreateColumn()
        {
            var table = _unitService.GetUnits();
            var displayMember = table.Columns.Contains("TEN_DON_VI") ? "TEN_DON_VI" : "TEN";
            var valueMember = table.Columns.Contains("ID") ? "ID" : table.Columns[0].ColumnName;

            return new DataGridViewComboBoxColumn
            {
                DataSource = table,
                DisplayMember = displayMember,
                ValueMember = valueMember,
                DataPropertyName = "ID_DON_VI_TINH",
                AutoComplete = true,
                HeaderText = "Đơn vị tính"
            };
        }

        public DonViTinh GetUnit(int id) => _unitService.GetUnit(id);

        public bool SaveChanges() => _unitService.SaveChanges();
    }
}
