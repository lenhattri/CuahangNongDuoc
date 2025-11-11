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
    public partial class frmTimPhieuChi : Form
    {
        
        public frmTimPhieuChi()
        {
            InitializeComponent();
        }

        private void frmTimPhieuChi_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            Controller.LyDoChiController ctrl = new CuahangNongduoc.Controller.LyDoChiController();
            ctrl.HienthiAutoComboBox(cmbLyDo);
        }
    }
}