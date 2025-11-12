using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
namespace CuahangNongduoc
{
    public partial class frmTimPhieuNhap : Form
    {
        NhaCungCapController ctrlNCC = new NhaCungCapController(new NhaCungCapDAL());
        public frmTimPhieuNhap()
        {
            InitializeComponent();
        }

        private void frmTimPhieuNhap_Load(object sender, EventArgs e)
        {
            ctrlNCC.HienthiAutoComboBox(cmbNCC);

        }
    }
}