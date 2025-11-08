using CuahangNongduoc.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmKhachHang : Form
    {
        CuahangNongduoc.Controller.KhachHangController ctrl = new CuahangNongduoc.Controller.KhachHangController();
        public frmKhachHang()
        {
            InitializeComponent();
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            ctrl.HienthiKhachHangDataGridview(dataGridView, bindingNavigator);
            AppTheme.ApplyTheme(this);
            this.Refresh();
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            ctrl.Save();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            DataRow row = ctrl.NewRow();
            long maso = ThamSo.KhachHang;
            ThamSo.KhachHang = maso + 1;

            row["ID"] = maso;
            ctrl.Add(row);
            bindingNavigator.BindingSource.MoveLast();
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Dai Ly", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Khach Hang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void toolTimHoTen_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = true;
            toolTimDiaChi.Checked = false;
            toolTimKhachHang.Text = "Tìm theo Họ tên";
            toolTimKhachHang.ForeColor = Color.FromArgb(224, 224, 224);
            bindingNavigator.Focus();
        }

        private void toolTimDiaChi_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = false;
            toolTimDiaChi.Checked = true;
            toolTimKhachHang.Text = "Tìm theo Địa chỉ";
            toolTimKhachHang.ForeColor = Color.FromArgb(224, 224, 224);
            bindingNavigator.Focus();
        }

        private void toolTimKhachHang_Enter(object sender, EventArgs e)
        {
            toolTimKhachHang.Text = "";
            toolTimKhachHang.ForeColor = Color.Black;
        }


        private void toolTimKhachHang_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(toolTimKhachHang.Text))
            {
                if (toolTimHoTen.Checked)
                    toolTimKhachHang.Text = "Tìm theo Họ tên";
                else
                    toolTimKhachHang.Text = "Tìm theo Địa chỉ";

                toolTimKhachHang.ForeColor = Color.FromArgb(224, 224, 224);
            }
        }


        private void toolTimKhachHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // chặn tiếng "ping"

                string keyword = toolTimKhachHang.Text.Trim();

                if (string.IsNullOrEmpty(keyword) || toolTimKhachHang.ForeColor == Color.FromArgb(224, 224, 224))
                {
                    // ✅ Nếu ô trống hoặc đang là placeholder → hiển thị lại toàn bộ danh sách
                    ctrl.HienthiKhachHangDataGridview(dataGridView, bindingNavigator);
                }
                else
                {
                    // ✅ Có từ khóa thì lọc theo chế độ đang chọn
                    DataTable dt;
                    if (toolTimHoTen.Checked)
                        dt = ctrl.TimHoTen(keyword);
                    else
                        dt = ctrl.TimDiaChi(keyword);

                    dataGridView.DataSource = dt;
                }
            }
        }
    }
}