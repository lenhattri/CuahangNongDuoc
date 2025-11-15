using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.DTO;
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
    public partial class frmSanphamHethan : Form
    {
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

        public frmSanphamHethan()
        {
            InitializeComponent();
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            // ✅ Tạo DAL và inject vào controller
            var maSanPhanDal = new MaSanPhanFactory();
            var sanPhamDal = new SanPhamFactory();  // thêm dòng này

            var ctrl = new MaSanPhamController(maSanPhanDal, sanPhamDal);

            // ✅ Gọi qua đối tượng ctrl
            IList<CuahangNongduoc.BusinessObject.MaSanPham> data =
                ctrl.LayMaSanPhamHetHan(dt.Value.Date);

            IList<Microsoft.Reporting.WinForms.ReportParameter> param =
                new List<Microsoft.Reporting.WinForms.ReportParameter>();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter(
                "ngay_tinh", dt.Value.Date.ToString("dd/MM/yyyy")));

            this.MaSanPhamBindingSource.DataSource = data;
            this.reportViewer.LocalReport.SetParameters(param);
            this.reportViewer.RefreshReport();
        }

        private void frmSanphamHethan_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/bao-cao/san-pham-het-han";
                IFU_Helper.IFU(url);
            }
        }
    }
}