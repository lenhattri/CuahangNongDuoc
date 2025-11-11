using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmInPhieuBan : Form
    {
        CuahangNongduoc.BusinessObject.PhieuBan m_PhieuBan;
        
        public frmInPhieuBan(CuahangNongduoc.BusinessObject.PhieuBan ph)
        {
            InitializeComponent();
            m_PhieuBan = ph;
            this.reportViewer.LocalReport.SubreportProcessing += new Microsoft.Reporting.WinForms.SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
        }

        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();
            IList<ChiTietPhieuBanReport> chiTietPhieuBanReports = new List<ChiTietPhieuBanReport>();
        
            foreach (var item in m_PhieuBan.ChiTiet)
            {
                string tenSP = item.MaSanPham.SanPham.TenSanPham;
                ChiTietPhieuBanReport ct = new ChiTietPhieuBanReport
                {
                    TenSanPham = tenSP,
                    DonGia = item.DonGia,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.ThanhTien
                };
                chiTietPhieuBanReports.Add(ct);
            }
            e.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("ChiTietPhieuBan", chiTietPhieuBanReports));
        }

        private void frmInPhieuBan_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            Num2Str num = new Num2Str();
            IList<Microsoft.Reporting.WinForms.ReportParameter> param = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", ch.TenCuaHang));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", ch.DiaChi));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", ch.DienThoai));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", num.NumberToString(m_PhieuBan.TongTien.ToString())));

            PhieuBanReport phieuBanReport = new PhieuBanReport
            {
                Id = m_PhieuBan.Id,
                NgayBan = m_PhieuBan.NgayBan,
                TongTien = m_PhieuBan.TongTien,
                DaTra = m_PhieuBan.DaTra,
                ConNo = m_PhieuBan.ConNo,
                TenKhachHang = m_PhieuBan.KhachHang?.HoTen,
                DiaChi = m_PhieuBan.KhachHang?.DiaChi,
                DienThoai = m_PhieuBan.KhachHang?.DienThoai
            };

            ReportHanler.LoadReport(
                this.reportViewer,
                new List<CuahangNongduoc.DTO.PhieuBanReport> { phieuBanReport },
                "rptPhieuBan.rdlc",
                "PhieuBan",
                param
            );
        }
    }
}