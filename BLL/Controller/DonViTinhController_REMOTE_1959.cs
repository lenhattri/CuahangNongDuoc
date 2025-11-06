using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class DonViTinhController
    {
        // Giờ không phụ thuộc trực tiếp vào DonViTinhDAL nữa
        private readonly IDonViTinhDAL _dal;
        private DataTable _tableForEdit; // bảng đang bind để Save()

        // ====================== CONSTRUCTOR ==========================
        public DonViTinhController()
        {
            // Inject thủ công qua Factory (trường hợp WinForms chưa có IoC)
            _dal = DonViTinhDAL.Create();
        }

        // Cho phép inject test hoặc mock DAL từ ngoài
        public DonViTinhController(IDonViTinhDAL dal)
        {
            _dal = dal ?? DonViTinhDAL.Create();
        }

        // ====================== HÀM TIỆN ÍCH ==========================
        /// <summary>
        /// Chọn tên cột hiển thị phù hợp (tuỳ bảng)
        /// </summary>
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

        // ====================== BINDING COMBOBOX ==========================
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

        // ====================== BINDING DATAGRIDVIEW (COMBO COLUMN) ==========================
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

        // ====================== BINDING DATAGRIDVIEW (TOÀN BẢNG) ==========================
        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            _tableForEdit = _dal.DanhSachDVT();
            var bs = new BindingSource { DataSource = _tableForEdit };
            if (bn != null) bn.BindingSource = bs;
            dg.DataSource = bs;
            // (Tuỳ chọn) dg.AutoGenerateColumns = true;
        }

        // ====================== LẤY 1 DVT CỤ THỂ ==========================
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

        // ====================== LƯU THAY ĐỔI (UPDATE/INSERT/DELETE) ==========================
        public bool Save()
        {
            if (_tableForEdit == null) return false; // chưa gọi HienthiDataGridview
            return _dal.Save(_tableForEdit);
        }
    }
}
