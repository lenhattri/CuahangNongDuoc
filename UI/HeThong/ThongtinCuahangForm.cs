using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class ThongtinCuahangForm : Form
    {
        public ThongtinCuahangForm()
        {
            InitializeComponent();
        }

        private void frmThongtinCuahang_Load(object sender, EventArgs e)
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            txtTenCuaHang.Text = ch.TenCuaHang;
            txtDienThoai.Text = ch.DienThoai;
            txtDiaChi.Text = ch.DiaChi;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ThamSo.GanCuaHang(txtTenCuaHang.Text, txtDiaChi.Text, txtDienThoai.Text);

            this.Close();
        }
    }
}