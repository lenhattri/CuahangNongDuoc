// BLL/Controller/DonViTinhController.cs
using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class DonViTinhController
    {
        private readonly IDonViTinhDAL _dal;
        public DonViTinhController(IDonViTinhDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }


        // Pick the first available column name from candidates; fall back to a string column or the first column.
        private static string PickColumn(DataTable dt, params string[] candidates)
        {
            if (dt == null) return null;
            foreach (var c in candidates)
            {
                if (!string.IsNullOrEmpty(c) && dt.Columns.Contains(c))
                    return c;
            }

            foreach (DataColumn col in dt.Columns)
            {
                if (col.DataType == typeof(string))
                    return col.ColumnName;
            }

            return dt.Columns.Count > 0 ? dt.Columns[0].ColumnName : null;
        }

        public void HienthiAutoComboBox(ComboBox cmb)
        {
            var dt = _dal.DanhSachDVT();
            if (dt == null) return;

            cmb.DataSource = dt;
            cmb.DisplayMember = PickColumn(dt, "TEN", "TEN_DON_VI", "TEN_DON_VI_T", "NAME");
            cmb.ValueMember = dt.Columns.Contains("ID") ? "ID" : PickColumn(dt, "Id", "id");
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public DataGridViewComboBoxColumn HienthiDataGridViewComboBoxColumn()
        {
            var dt = _dal.DanhSachDVT();
            var display = PickColumn(dt, "TEN", "TEN_DON_VI", "TEN_DON_VI_T", "NAME");
            var value = dt != null && dt.Columns.Contains("ID") ? "ID" : PickColumn(dt, "Id", "id");

            var col = new DataGridViewComboBoxColumn
            {
                DataSource = dt,
                DisplayMember = display,
                ValueMember = value,
                DataPropertyName = "ID_DON_VI_TINH",
                HeaderText = "Đơn vị tính",
                AutoComplete = true
            };
            return col;
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            var bs = new BindingSource { DataSource = _dal.DanhSachDVT() };
            if (bn != null) bn.BindingSource = bs;
            dg.DataSource = bs;
            // (Tuỳ chọn) dg.AutoGenerateColumns = true; // nếu bạn không tự tạo cột trong Designer
        }

        public DonViTinh LayDVT(int id)
        {
            var tbl = _dal.LayDVT(id);
            if (tbl == null || tbl.Rows.Count == 0) return null;

            var r = tbl.Rows[0];
            var nameCol = PickColumn(tbl, "TEN_DON_VI", "TEN", "TEN_DON_VI_T", "NAME");
            return new DonViTinh(
                Convert.ToInt32(r["ID"]),
                Convert.ToString(r[nameCol])
            );
        }

        public bool Save()
        {
            return _dal.Save();         // đẩy các Added/Modified/Deleted
        }
    }
}