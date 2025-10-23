using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class TimPhieuChiForm : Form
    {
        
        public TimPhieuChiForm()
        {
            InitializeComponent();
        }

        private void frmTimPhieuChi_Load(object sender, EventArgs e)
        {
            Controller.LyDoChiController ctrl = new CuahangNongduoc.Controller.LyDoChiController();
            ctrl.HienthiAutoComboBox(cmbLyDo);
        }
    }
}