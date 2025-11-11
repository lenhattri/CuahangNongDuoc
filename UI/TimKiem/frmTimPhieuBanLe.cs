using CuahangNongduoc.Controller;
using CuahangNongduoc.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmTimPhieuBanLe : Form
    {
        public frmTimPhieuBanLe()
        {
            InitializeComponent();
        }
        public frmTimPhieuBanLe(bool loai):this()
        {
            KhachHangController ctrlKH = new KhachHangController();
            ctrlKH.HienthiAutoComboBox(cmbNCC, loai);
        }

        private void frmTimPhieuBanLe_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
        }
    }
}