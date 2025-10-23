using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.HeThong
{
    public partial class NguoiDungForm : Form
    {
        private UserController userController = new UserController();
        public NguoiDungForm()
        {
            InitializeComponent();
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            DataRow newRow = userController.NewRow();
            newRow["ID"] = DBNull.Value; // ID sẽ được gán khi lưu
            newRow["HO_TEN"] = string.Empty;
            newRow["DIA_CHI"] = string.Empty;
            newRow["DIEN_THOAI"] = string.Empty;
            newRow["TEN_DANG_NHAP"] = string.Empty;
            newRow["QUYEN"] = QuyenNguoiDung.NhanVien.ToString();
            newRow["MAT_KHAU"] = string.Empty;

            ChiTietNguoiDungForm frm = new ChiTietNguoiDungForm(newRow);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                userController.Add(frm.Tag as DataRow);
            }
        }

        private void frmNguoiDung_Load(object sender, EventArgs e)
        {
            userController.HienthiNguoiDungDataGridview(dataGridView, bindingNavigator);
        }

        private void toolLuu_Click_1(object sender, EventArgs e)
        {
            userController.Save();
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            DataRowView view = (DataRowView)bindingNavigator.BindingSource.Current;
            ChiTietNguoiDungForm frmChiTietNguoiDung = new ChiTietNguoiDungForm(view.Row, ActionUser.Sua);
            if (frmChiTietNguoiDung.ShowDialog() == DialogResult.OK)
            {
                userController.Update(Convert.ToInt64(view.Row["ID"]));
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DataRowView view = (DataRowView)bindingNavigator.BindingSource.Current;
            userController.Delete(Convert.ToInt64(view.Row["ID"]));
        }

        private void toolTimHoTen_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = true;
            toolTimTenDangNhap.Checked = false;
            toolTimHoTen.ForeColor = Color.Blue;
            toolTimTenDangNhap.ForeColor = Color.Black;
            toolTimNhanVien.Text = "Nhập họ tên cần tìm";
            toolTimNhanVien.ForeColor = Color.Gray;
        }

        private void toolTimTenDangNhap_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = false;
            toolTimTenDangNhap.Checked = true;
            toolTimHoTen.ForeColor = Color.Black;
            toolTimTenDangNhap.ForeColor = Color.Blue;
            toolTimNhanVien.Text = "Nhập tên đăng nhập cần tìm";
            toolTimNhanVien.ForeColor = Color.Gray;
        }

        private void toolTimNhanVien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (toolTimHoTen.Checked)
                {
                    bindingNavigator.BindingSource.Filter = $"HO_TEN LIKE '%{toolTimNhanVien.Text}%'";
                }
                else
                {
                    bindingNavigator.BindingSource.Filter = $"TEN_DANG_NHAP LIKE '%{toolTimNhanVien.Text}%'";
                }
            }
        }
    }
}
