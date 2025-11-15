using CuahangNongduoc.UI.Facades;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmSanPham : Form
    {
        private readonly SanPhamFacade _sanPhamFacade;
        private readonly DonViTinhFacade _donViTinhFacade;
        private BindingSource _productBinding;
        private readonly string Url = ConfigurationManager.AppSettings["Url"].ToString();

        public frmSanPham()
        {
            InitializeComponent();

            _sanPhamFacade = ServiceLocator.Resolve<SanPhamFacade>();
            _donViTinhFacade = ServiceLocator.Resolve<DonViTinhFacade>();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            _donViTinhFacade.ConfigureComboBox(cmbDVT);
            dataGridView.Columns.Add(_donViTinhFacade.CreateColumn());

            _productBinding = _sanPhamFacade.BindProducts(
                dataGridView,
                bindingNavigator,
                txtMaSanPham,
                txtTenSanPham,
                cmbDVT,
                numSoLuong,
                numDonGiaNhap,
                numGiaBanSi,
                numGiaBanLe);

            AppTheme.ApplyTheme(this);

            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/quan-ly/san-pham";
                IFU_Helper.IFU(url);
            }
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            _sanPhamFacade.SaveChanges();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            DataRow row = _sanPhamFacade.CreateRow();
            long maso = ThamSo.SanPham;
            ThamSo.SanPham = maso + 1;
            row["ID"] = maso;
            row["TEN_SAN_PHAM"] = "";
            row["SO_LUONG"] = 0;
            row["DON_GIA_NHAP"] = 0;
            row["GIA_BAN_SI"] = 0;
            row["GIA_BAN_LE"] = 0;
            _sanPhamFacade.Add(row);
            _productBinding.MoveLast();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "San Pham", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _productBinding.RemoveCurrent();
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnThemDVT_Click(object sender, EventArgs e)
        {
            using (var dvt = new frmDonViTinh())
            {
                dvt.ShowDialog();
            }

            _donViTinhFacade.ConfigureComboBox(cmbDVT);
        }

        private void toolTimMaSanPham_Click(object sender, EventArgs e)
        {
            toolTimMaSanPham.Checked = true;
            toolTimTenSanPham.Checked = false;
            toolTimSanPham.Text = "";
        }

        private void mnuTimTenSanPham_Click(object sender, EventArgs e)
        {
            toolTimMaSanPham.Checked = false;
            toolTimTenSanPham.Checked = true;
            toolTimSanPham.Text = "";
        }

        private void toolTimSanPham_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TimSanPham();
            }
        }

        private void toolTimSanPham_Leave(object sender, EventArgs e)
        {
            TimSanPham();
        }

        void TimSanPham()
        {
            DataTable dt;

            if (toolTimMaSanPham.Checked)
            {
                dt = _sanPhamFacade.SearchByCode(toolTimSanPham.Text);
            }
            else
            {
                dt = _sanPhamFacade.SearchByName(toolTimSanPham.Text);
            }

            _productBinding.DataSource = dt;
        }

        private void toolTimSanPham_Enter(object sender, EventArgs e)
        {
            toolTimSanPham.Text = "";
            toolTimSanPham.ForeColor = Color.Black;
        }
    }
}
