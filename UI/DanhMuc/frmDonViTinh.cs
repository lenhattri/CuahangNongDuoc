using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.Controller;
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
    public partial class frmDonViTinh : Form
    {
        private readonly DonViTinhController ctrl;
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();


        public frmDonViTinh()
        {
            InitializeComponent();

            // Khởi tạo DAL và inject vào controller
            IDonViTinhDAL dal = new DonViTinhDAL();
            ctrl = new DonViTinhController(dal);
        }

        private void frmDonViTinh_Load(object sender, EventArgs e)
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

                string url = Url + "/quan-ly/don-vi-tinh";
                IFU_Helper.IFU(url);
            }
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