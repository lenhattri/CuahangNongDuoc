using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
namespace CuahangNongduoc
{
    public partial class frmNhaCungCap : Form
    {
        CuahangNongduoc.Controller.NhaCungCapController ctrl = new CuahangNongduoc.Controller.NhaCungCapController(new NhaCungCapDAL());
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

        public frmNhaCungCap()
        {
            InitializeComponent();
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Nha Cung Cap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Nha Cung Cap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
            }
        }

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            AppTheme.ApplyTheme(this);
            this.Refresh();
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/quan-ly/nha-cung-cap";
                IFU_Helper.IFU(url);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            long maso = ThamSo.NhaCungCap;
            ThamSo.NhaCungCap = maso + 1;

            DataRow row = ctrl.NewRow();
            row["ID"] = maso;

            ctrl.Add(row);
            bindingNavigator.BindingSource.MoveLast();
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            ctrl.Save();
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolTimNhaCungCap_Enter(object sender, EventArgs e)
        {
            toolTimNhaCungCap.Text = "";
            toolTimNhaCungCap.ForeColor = Color.Black;
        }

        private void toolTimNhaCungCap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                TimNhaCungCap();
                e.Handled = true;
            }
        }

        private void toolTimNhaCungCap_Leave(object sender, EventArgs e)
        {
            if (toolTimHoTen.Checked == true)
                toolTimNhaCungCap.Text = "Tìm theo Nhà cung cấp";
            else
                toolTimNhaCungCap.Text = "Tìm theo Địa chỉ";

            toolTimNhaCungCap.ForeColor = Color.FromArgb(224, 224, 224);
        }

        private void toolTimHoTen_Click(object sender, EventArgs e)
        {
            toolTimDiaChi.Checked = !toolTimDiaChi.Checked;
            toolTimHoTen.Checked = !toolTimDiaChi.Checked;
            toolTimNhaCungCap.Text = "Tìm theo Nhà cung cấp";
            bindingNavigator.Focus();
        }

        private void toolTimDiaChi_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = !toolTimHoTen.Checked;
            toolTimDiaChi.Checked = !toolTimHoTen.Checked;
            toolTimNhaCungCap.Text = "Tìm theo Địa chỉ";
            bindingNavigator.Focus();
        }

        private void TimNhaCungCap()
        {
            DataTable dt;

            if (toolTimHoTen.Checked)
                dt = ctrl.TimHoTen(toolTimNhaCungCap.Text);
            else
                dt = ctrl.TimDiaChi(toolTimNhaCungCap.Text);

            dataGridView.DataSource = dt;
        }

    }
}