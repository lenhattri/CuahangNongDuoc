using CuahangNongduoc.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Utils.Functions;
using CuahangNongduoc.DTO;

namespace CuahangNongduoc
{
    public partial class frmSanphamHethan : Form
    {
        public frmSanphamHethan()
        {
            InitializeComponent();
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            var maSanPhanDal = new MaSanPhanFactory();
            var sanPhamDal = new SanPhamFactory();  // thêm dòng này

            var ctrl = new MaSanPhamController(maSanPhanDal, sanPhamDal);

            IList<CuahangNongduoc.BusinessObject.MaSanPham> data =
                ctrl.LayMaSanPhamHetHan(dt.Value.Date);

            IList<Microsoft.Reporting.WinForms.ReportParameter> param =
                new List<Microsoft.Reporting.WinForms.ReportParameter>();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter(
                "ngay_tinh", dt.Value.Date.ToString("dd/MM/yyyy")));

            IList<SanPhamHetHanReport> sanPhamHetHanReports = new List<SanPhamHetHanReport>();

            foreach (var item in data)
            {
                SanPhamHetHanReport reportItem = new SanPhamHetHanReport
                {
                    Id = item.Id,
                    GiaNhap = item.GiaNhap.ToString(),
                    SoLuong = item.SoLuong,
                    NgayNhap = item.NgayNhap,
                    NgaySanXuat = item.NgaySanXuat,
                    NgayHetHan = item.NgayHetHan,
                    TenSanPham = item.SanPham.TenSanPham,
                };
                sanPhamHetHanReports.Add(reportItem);
            }

            ReportHanler.LoadReport(
                this.reportViewer,
                sanPhamHetHanReports,
                "rptSanPhamHetHan.rdlc",
                "SanPhamHetHan",
                param);
        }

        private void frmSanphamHethan_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
        }
    }
}