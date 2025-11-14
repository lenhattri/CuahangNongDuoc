using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;

namespace CuahangNongduoc
{
    public partial class frmDanhsachPhieuBanLe : Form
    {
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

        public frmDanhsachPhieuBanLe()
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
        }

        PhieuBanController ctrl = new PhieuBanController(
            new PhieuBanFactory(),
            new KhachHangController()
            );
        KhachHangController ctrlKH = new KhachHangController();
        
        private void frmDanhsachPhieuNhap_Load(object sender, EventArgs e)
        {
            ctrlKH.HienthiKhachHangDataGridviewComboBox(colKhachhang);
            ctrl.HienthiPhieuBanLe(bindingNavigator, dataGridView);
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

                string url = Url + "/nghiep-vu/ban-le-ban-si";
                IFU_Helper.IFU(url);
            }
        }


        frmBanLe BanLe = null;
        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (BanLe == null || BanLe.IsDisposed)
            {
                BanLe = new frmBanLe(ctrl);
                BanLe.Show();
            }
            else
                BanLe.Activate();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (BanLe == null || BanLe.IsDisposed)
            {
                BanLe = new frmBanLe();
                BanLe.Show();
            }
            else
                BanLe.Activate();
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                DataRowView view = (DataRowView)bindingNavigator.BindingSource.Current;
                ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController(new ChiTietPhieuBanDAL(), new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory()));
                IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());
                MaSanPhanFactory factory = new MaSanPhanFactory();
                foreach (ChiTietPhieuBan ct in ds)
                {
                    factory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                }
                ctrl.Save();
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
             DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
             if (view != null)
             {
                 if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                 {
                     ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController(new ChiTietPhieuBanDAL(), new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory()));
                     IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());
                    MaSanPhanFactory factory = new MaSanPhanFactory();
                    foreach (ChiTietPhieuBan ct in ds)
                     {
                         factory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                     }
                     bindingNavigator.BindingSource.RemoveCurrent();
                     ctrl.Save();
                 }
             }
        }

        private void toolPrint_Click(object sender, EventArgs e)
        {
            DataRowView row = (DataRowView)bindingNavigator.BindingSource.Current;
            if (row != null)
            {
                PhieuBanController ctrlPB = new PhieuBanController(
                    new PhieuBanFactory(),
                new KhachHangController());
                String ma_phieu = row["ID"].ToString();
                CuahangNongduoc.BusinessObject.PhieuBan ph = ctrlPB.LayPhieuBan(ma_phieu);
                frmInPhieuBan PhieuBan = new frmInPhieuBan(ph);
                PhieuBan.Show();
            }
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            frmTimPhieuBanLe Tim = new frmTimPhieuBanLe(false);
            Point p = PointToScreen(toolTimKiem.Bounds.Location);
            p.X += toolTimKiem.Width;
            p.Y += toolTimKiem.Height;
            Tim.Location = p;
            Tim.ShowDialog();
            if (Tim.DialogResult == DialogResult.OK)
            {
                ctrl.TimPhieuBan(Tim.cmbNCC.SelectedValue.ToString(), Tim.dtNgayNhap.Value.Date);
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}