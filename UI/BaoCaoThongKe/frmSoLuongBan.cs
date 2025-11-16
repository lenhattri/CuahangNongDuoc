using CuahangNongduoc.DAL.Interfaces;
﻿using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Policy;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmSoLuongBan : Form
    {
        private readonly ChiTietPhieuBanController _ctrlChiTiet;
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

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
            LoadReport(_ctrlChiTiet.LayTatCaChiTietPhieuBan());
            var param = new List<Microsoft.Reporting.WinForms.ReportParameter>
            {
                new Microsoft.Reporting.WinForms.ReportParameter("ngay", "Tất cả")
            };
            reportViewer.LocalReport.SetParameters(param);
            AppTheme.ApplyTheme(this);
            this.reportViewer.RefreshReport();
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/bao-cao/so-luong-ban";
                IFU_Helper.IFU(url);
            }
        }
        public void LoadReport(IList<ChiTietPhieuBan> dt)
        {
            IList<SoLuongBanReport> dsReport = new List<SoLuongBanReport>();

            foreach (var row in dt)
            {
                dsReport.Add(new SoLuongBanReport
                {
                    TenSanPham = row.MaSanPham.SanPham.TenSanPham,
                    MaSanPham = row.MaSanPham.Id,
                    DonGia = row.DonGia,
                    SoLuong = row.SoLuong,
                    ThanhTien = row.ThanhTien
                });
            }
            ReportHanler.LoadReport(
                this.reportViewer,
                dsReport,
                "rptSoLuongBan.rdlc",
                "SoLuongBan"
            );
        }
        private void btnXemNgay_Click(object sender, EventArgs e)
        {
            var param = new List<Microsoft.Reporting.WinForms.ReportParameter>
            {
                new Microsoft.Reporting.WinForms.ReportParameter("ngay", "Ngày " + dtNgay.Value.Date.ToString("dd/MM/yyyy"))
            };

            reportViewer.LocalReport.SetParameters(param);

            // ✅ Dùng _ctrlChiTiet thay vì ctrl
            //ChiTietPhieuBanBindingSource.DataSource = _ctrlChiTiet.ChiTietPhieuBan(dtNgay.Value.Date);
            LoadReport(_ctrlChiTiet.ChiTietPhieuBan(dtNgay.Value.Date));
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
            //ChiTietPhieuBanBindingSource.DataSource = _ctrlChiTiet.ChiTietPhieuBan(cmbThang.SelectedIndex + 1, Convert.ToInt32(numNam.Value));
            LoadReport(_ctrlChiTiet.ChiTietPhieuBan(cmbThang.SelectedIndex + 1, Convert.ToInt32(numNam.Value)));
            reportViewer.RefreshReport();
        }
    }
}
