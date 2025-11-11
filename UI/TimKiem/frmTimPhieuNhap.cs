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
    public partial class frmTimPhieuNhap : Form
    {
        NhaCungCapController ctrlNCC = new NhaCungCapController();
        public frmTimPhieuNhap()
        {
            InitializeComponent();
        }

        private void frmTimPhieuNhap_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            ctrlNCC.HienthiAutoComboBox(cmbNCC);

        }
    }
}