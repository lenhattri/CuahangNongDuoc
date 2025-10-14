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
    public enum ActionUser
    {
        Them,
        Sua,
        Xem
    }
    public partial class frmChiTietNguoiDung : Form
    {
        private readonly DataRow _dataRow;
        private readonly ActionUser _actionUser;
        public frmChiTietNguoiDung(DataRow dataRow, ActionUser action = ActionUser.Them)
        {
            _dataRow = dataRow;
            _actionUser = action;
            InitializeComponent();
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (_dataRow != null && _actionUser != ActionUser.Xem)
            {
                _dataRow["HO_TEN"] = txtHoTen.Text;
                _dataRow["DIA_CHI"] = txtDiaChi.Text;
                _dataRow["DIEN_THOAI"] = txtDienThoai.Text;
                _dataRow["TEN_DANG_NHAP"] = txtTenNguoiDung.Text;
                if (_actionUser == ActionUser.Sua && txtMatKhau.Text == "")
                {
                    _dataRow["MAT_KHAU"] = _dataRow["MAT_KHAU"];
                }
                else
                {
                    _dataRow["MAT_KHAU"] = txtMatKhau.Text;
                }
                _dataRow["QUYEN"] = cbbQuyen.SelectedItem;

                this.Tag = _dataRow;
            }
            else
            {
                MessageBox.Show("Dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private void frmChiTietNguoiDung_Load(object sender, EventArgs e)
        {
            cbbQuyen.DataSource = Enum.GetValues(typeof(QuyenNguoiDung));
            cbbQuyen.SelectedIndex = 0;

            if (_actionUser == ActionUser.Sua || _actionUser == ActionUser.Xem)
            {
                if (_dataRow != null)
                {
                    txtHoTen.Text = _dataRow["HO_TEN"].ToString();
                    txtDiaChi.Text = _dataRow["DIA_CHI"].ToString();
                    txtDienThoai.Text = _dataRow["DIEN_THOAI"].ToString();
                    txtTenNguoiDung.Text = _dataRow["TEN_DANG_NHAP"].ToString();
                    //txtMatKhau.Text = _dataRow["MAT_KHAU"].ToString();
                    cbbQuyen.SelectedItem = _dataRow["QUYEN"].ToString();
                }
            }

            if (_actionUser == ActionUser.Xem)
            {
                txtTenNguoiDung.Enabled = false;
                txtMatKhau.Enabled = false;
                cbbQuyen.Enabled = false;
                btnAction.Visible = false;
                this.Text = "Người dùng";
            }
            else if (_actionUser == ActionUser.Them)
            {
                btnAction.Text = "Thêm";
                this.Text = "Thêm người dùng";
            }
            else if (_actionUser == ActionUser.Sua)
            {
                btnAction.Text = "Lưu";
                this.Text = "Sửa người dùng";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
