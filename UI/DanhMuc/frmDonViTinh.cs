using CuahangNongduoc.UI.Facades;
using CuahangNongduoc.Utils;
using System;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmDonViTinh : Form
    {
        private readonly UnitFacade _unitFacade;
        private BindingSource _bindingSource;

        public frmDonViTinh()
        {
            InitializeComponent();
            _unitFacade = ServiceLocator.Resolve<UnitFacade>();
        }

        private void frmDonViTinh_Load(object sender, EventArgs e)
        {
            _bindingSource = _unitFacade.BindUnits(dataGridView, bindingNavigator);
            AppTheme.ApplyTheme(this);
            Refresh();
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            try
            {
                bindingNavigatorPositionItem.Focus();
                Validate();
                _bindingSource.EndEdit();

                _unitFacade.SaveChanges();
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
