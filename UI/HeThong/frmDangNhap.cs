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
    public partial class frmDangNhap : Form
    {
        private UserController userController = new UserController();
        public frmDangNhap()
        {
            InitializeComponent();
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

        private void lblLinkThoat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát chương trình không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void chkNhoMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            // Nếu checkbox được tick thì hiện mật khẩu (bỏ dấu chấm)
            if (chkNhoMatKhau.Checked)
            {
                txtMatKhau.PasswordChar = '\0'; // '\0' = không ẩn ký tự
            }
            else
            {
                txtMatKhau.PasswordChar = '•'; // hoặc '*'
            }
        }

        //private void pictureBox2_Click(object sender, EventArgs e)
        //{

        //}
    }
}
