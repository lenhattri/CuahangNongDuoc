using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DAL.Infrastructure;
using CuahangNongduoc.DataLayer;
namespace CuahangNongduoc
{
    public partial class frmDanhsachPhieuNhap : Form
    {
        public frmDanhsachPhieuNhap()
        {
            InitializeComponent();
        }

        PhieuNhapController ctrl = new PhieuNhapController(new PhieuNhapFactory(DbClient.Instance));
        NhaCungCapController ctrlNCC = new NhaCungCapController(new NhaCungCapDAL());

        private void frmDanhsachPhieuNhap_Load(object sender, EventArgs e)
        {
            ctrlNCC.HienthiDataGridviewComboBox(colNhaCungCap);
            ctrl.HienthiPhieuNhap(bindingNavigator, dataGridView);
        }

        frmNhapHang NhapHang = null;

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            // Mở form nhập hàng ở chế độ "Sửa" theo dòng hiện tại (nếu muốn)
            var current = bindingNavigator.BindingSource?.Current as DataRowView;
            string id = current?["ID"]?.ToString();

            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new frmNhapHang(ctrl); // truyền controller + mã phiếu (có thể null)
                NhapHang.Show();
            }
            else
            {
                NhapHang.Activate();
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // Mở form nhập hàng ở chế độ "Thêm mới"
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new frmNhapHang(ctrl);
                NhapHang.Show();
            }
            else
            {
                NhapHang.Activate();
            }
        }

        private void toolIn_Click(object sender, EventArgs e)
        {
            DataRowView row = (DataRowView)bindingNavigator.BindingSource.Current;
            if (row != null)
            {
                PhieuNhapController ctrlPN = new PhieuNhapController(new PhieuNhapFactory(DbClient.Instance));
                String ma_phieu = row["ID"].ToString();
                CuahangNongduoc.BusinessObject.PhieuNhap ph = ctrlPN.LayPhieuNhap(ma_phieu);
                frmInPhieuNhap PhieuNhap = new frmInPhieuNhap(ph);
                PhieuNhap.Show();
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Nhập",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var current = bindingNavigator.BindingSource?.Current;
                if (current != null)
                {
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrl.Save();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu để xóa.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            frmTimPhieuNhap TimPhieu = new frmTimPhieuNhap();
            Point p = PointToScreen(toolTimKiem.Bounds.Location);
            p.X += toolTimKiem.Width;
            p.Y += toolTimKiem.Height;
            TimPhieu.Location = p;
            TimPhieu.ShowDialog();
            if (TimPhieu.DialogResult == DialogResult.OK)
            {
                ctrl.TimPhieuNhap(TimPhieu.cmbNCC.SelectedValue?.ToString(), TimPhieu.dtNgayNhap.Value.Date);
            }
        }
    }
}
