using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.Utils;
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
    public partial class DangNhapForm : Form
    {
        private UserController userController = new UserController();
        public DangNhapForm()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if(userController.KiemTraDangNhap(txtTenDangNhap.Text, txtMatKhau.Text))
            {
                this.DialogResult = DialogResult.OK;
                Session.CurrentUser = userController.LayNguoiDungTheoTenDangNhap(txtTenDangNhap.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
