using CuahangNongduoc.UI.Facades;
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
        private readonly DonViTinhFacade _donViTinhFacade;
        private BindingSource _bindingSource;
        private readonly DonViTinhController ctrl;
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();


        public frmDonViTinh()
        {
            InitializeComponent();
            _donViTinhFacade = ServiceLocator.Resolve<DonViTinhFacade>();
        }

        private void frmDonViTinh_Load(object sender, EventArgs e)
        {
            _bindingSource = _donViTinhFacade.BindUnits(dataGridView, bindingNavigator);
            AppTheme.ApplyTheme(this);
            Refresh();
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
                bindingNavigatorPositionItem.Focus();
                Validate();
                _bindingSource.EndEdit();

                _donViTinhFacade.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình lưu:\n" + ex.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            var drv = _bindingSource.Current as DataRowView;
            if (drv != null)
            {
                drv["TEN_DON_VI"] = "";
            }

            _bindingSource.MoveLast();
        }
    }
}
