using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmTimPhieuThu : Form
    {
        public frmTimPhieuThu()
        {
            InitializeComponent();
        }

        private void frmTimPhieuThu_Load(object sender, EventArgs e)
        {
            Controller.KhachHangController ctrl = new CuahangNongduoc.Controller.KhachHangController(new KhachHangFactory());
            ctrl.HienthiChungAutoComboBox(cmbKhachHang);
        }
    }
}