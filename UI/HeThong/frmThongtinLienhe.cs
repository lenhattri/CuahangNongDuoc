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
    public partial class frmThongtinLienhe : Form
    {
        public frmThongtinLienhe()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmThongtinLienhe_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
        }
    }
}