using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmInPhieuNhap : Form
    {
        CuahangNongduoc.BusinessObject.PhieuNhap m_PhieuNhap = null;
        public frmInPhieuNhap(CuahangNongduoc.BusinessObject.PhieuNhap ph)
        {
            m_PhieuNhap = ph;
            InitializeComponent();

            this.reportViewer.LocalReport.SubreportProcessing += new Microsoft.Reporting.WinForms.SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
        }

        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();

            IList<ChiTietPhieuNhapReport> chiTietPhieuNhapReports = new List<ChiTietPhieuNhapReport>();
            foreach (var item in m_PhieuNhap.ChiTiet)
            {
                ChiTietPhieuNhapReport ct = new ChiTietPhieuNhapReport
                {
                    Id = item.Id,
                    TenSanPham = item.SanPham.TenSanPham,
                    GiaNhap = item.GiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.ThanhTien,
                    NgaySanXuat = item.NgaySanXuat,
                    NgayHetHan = item.NgayHetHan
                };
                chiTietPhieuNhapReports.Add(ct);
            }

            e.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("ChiTietPhieuNhap", chiTietPhieuNhapReports));
        }

        private void frmInPhieuNhap_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            Num2Str num = new Num2Str();
            IList<Microsoft.Reporting.WinForms.ReportParameter> param = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", ch.TenCuaHang));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", ch.DiaChi));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", ch.DienThoai));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", num.NumberToString(m_PhieuNhap.TongTien.ToString())));

            //TODO: fix dữ liệu ngày nhập, tổng tiền không hiển thị
            PhieuNhapReport phieuNhapReport = new PhieuNhapReport
            {
                Id = m_PhieuNhap.Id,
                NgayNhap = m_PhieuNhap.NgayNhap, // Chưa hiện dữ liệu
                TongTien = m_PhieuNhap.TongTien, // Chưa hiện dữ liệu
                TenNhaCungCap = m_PhieuNhap.NhaCungCap?.HoTen,
            };

            ReportHanler.LoadReport(
                this.reportViewer,
                new List<CuahangNongduoc.DTO.PhieuNhapReport>() { phieuNhapReport },
                "rptPhieuNhap.rdlc",
                "PhieuNhap", 
                param
            );
        }
    }
}