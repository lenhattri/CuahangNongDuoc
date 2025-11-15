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
    public partial class frmThongtinCuahang : Form
    {
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

        public frmThongtinCuahang()
        {
            InitializeComponent();
        }

        private void frmThongtinCuahang_Load(object sender, EventArgs e)
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            txtTenCuaHang.Text = ch.TenCuaHang;
            txtDienThoai.Text = ch.DienThoai;
            txtDiaChi.Text = ch.DiaChi;
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/tuy-chinh/thong-tin-cua-hang";
                IFU_Helper.IFU(url);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ThamSo.GanCuaHang(txtTenCuaHang.Text, txtDiaChi.Text, txtDienThoai.Text);

            this.Close();
        }
    }
}