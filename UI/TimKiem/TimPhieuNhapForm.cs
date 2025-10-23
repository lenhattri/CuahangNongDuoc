using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class TimPhieuNhapForm : Form
    {
        NhaCungCapController ctrlNCC = new NhaCungCapController();
        public TimPhieuNhapForm()
        {
            InitializeComponent();
        }

        private void frmTimPhieuNhap_Load(object sender, EventArgs e)
        {
            ctrlNCC.HienthiAutoComboBox(cmbNCC);

        }
    }
}