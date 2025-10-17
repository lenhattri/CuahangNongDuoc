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
        private readonly DonViTinhDAL _dal = new DonViTinhDAL();
        private DataTable _tableForEdit; // bảng đang bind để Save()

        public void HienthiAutoComboBox(ComboBox cmb)
        {
            var dt = _dal.DanhSachDVT();
            cmb.DataSource = dt;
            cmb.DisplayMember = "TEN";     // CHANGED: trước đây là "TEN_DON_VI", đổi về "TEN" để khớp DB/DAL
            cmb.ValueMember = "ID";
        }

        public DataGridViewComboBoxColumn HienthiDataGridViewComboBoxColumn()
        {
            var col = new DataGridViewComboBoxColumn
            {
                DataSource = _dal.DanhSachDVT(),
                DisplayMember = "TEN",              // CHANGED: trước đây là "TEN_DON_VI", đổi về "TEN"
                ValueMember = "ID",
                DataPropertyName = "ID_DON_VI_TINH",
                HeaderText = "Đơn vị tính"
            };
            return col;
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            _tableForEdit = _dal.DanhSachDVT();
            var bs = new BindingSource { DataSource = _tableForEdit };
            if (bn != null) bn.BindingSource = bs;
            dg.DataSource = bs;
            // (Tuỳ chọn) dg.AutoGenerateColumns = true; // nếu bạn không tự tạo cột trong Designer
        }

        public DonViTinh LayDVT(int id)
        {
            var tbl = _dal.LayDVT(id);
            if (tbl.Rows.Count == 0) return null;

            var r = tbl.Rows[0];
            return new DonViTinh(
                Convert.ToInt32(r["ID"]),
                Convert.ToString(r["TEN"])          // CHANGED: trước đây là "TEN_DON_VI", đổi về "TEN"
            );
        }

        public bool Save()
        {
            if (_tableForEdit == null) return false; // chưa gọi HienthiDataGridview
            return _dal.Save(_tableForEdit);         // đẩy các Added/Modified/Deleted
        }
    }
}
