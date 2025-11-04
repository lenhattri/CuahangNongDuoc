using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmDonViTinh : Form
    {
        DonViTinhController ctrl = new DonViTinhController();
        public frmDonViTinh()
        {
            InitializeComponent();
        }

        private void frmDonViTinh_Load(object sender, EventArgs e)
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Đẩy các thay đổi từ cell đang edit xuống DataTable
                bindingNavigatorPositionItem.Focus();
                this.Validate();
                ((BindingSource)bindingNavigator.BindingSource).EndEdit();

                ctrl.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình lưu:\n" + ex.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            var drv = bindingNavigator.BindingSource.Current as DataRowView;
            if (drv != null)
            {
                drv["TEN_DON_VI"] = "";
            }

            // Di chuyển đến cuối để bắt đầu nhập
            bindingNavigator.BindingSource.MoveLast();
        }
    }
}