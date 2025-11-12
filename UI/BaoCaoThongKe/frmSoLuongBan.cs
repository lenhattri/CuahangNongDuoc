using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmSoLuongBan : Form
    {
        private readonly ChiTietPhieuBanController _ctrlChiTiet;

        public frmSoLuongBan()
        {
            InitializeComponent();

            // ✅ Inject dependencies trong constructor
            IChiTietPhieuBanDAL chiTietDal = new ChiTietPhieuBanDAL();
            MaSanPhamController maSpCtrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());

            _ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, maSpCtrl);
        }

        private void frmSoLuongBan_Load(object sender, EventArgs e)
        {
            cmbThang.SelectedIndex = DateTime.Now.Month - 1;
            numNam.Value = DateTime.Now.Year;
            AppTheme.ApplyTheme(this);
        }

        private void btnXemNgay_Click(object sender, EventArgs e)
        {
            var param = new List<Microsoft.Reporting.WinForms.ReportParameter>
            {
                new Microsoft.Reporting.WinForms.ReportParameter("ngay", "Ngày " + dtNgay.Value.Date.ToString("dd/MM/yyyy"))
            };

            reportViewer.LocalReport.SetParameters(param);

            // ✅ Dùng _ctrlChiTiet thay vì ctrl
            ChiTietPhieuBanBindingSource.DataSource = _ctrlChiTiet.ChiTietPhieuBan(dtNgay.Value.Date);
            reportViewer.RefreshReport();
        }

        private void btnXemThang_Click(object sender, EventArgs e)
        {
            var param = new List<Microsoft.Reporting.WinForms.ReportParameter>
            {
                new Microsoft.Reporting.WinForms.ReportParameter(
                    "ngay",
                    "Tháng " + (cmbThang.SelectedIndex + 1) + "/" + numNam.Value.ToString()
                )
            };

            reportViewer.LocalReport.SetParameters(param);

            // ✅ Dùng _ctrlChiTiet thay vì ctrl
            ChiTietPhieuBanBindingSource.DataSource = _ctrlChiTiet.ChiTietPhieuBan(cmbThang.SelectedIndex + 1, Convert.ToInt32(numNam.Value));
            reportViewer.RefreshReport();
        }
    }
}
