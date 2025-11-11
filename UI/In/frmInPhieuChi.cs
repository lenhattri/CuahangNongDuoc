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
    public partial class frmInPhieuChi : Form
    {
        CuahangNongduoc.BusinessObject.PhieuChi m_PhieuChi;
        public frmInPhieuChi(CuahangNongduoc.BusinessObject.PhieuChi ph)
        {
            InitializeComponent();
            m_PhieuChi = ph;
        }

        private void frmInPhieuChi_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            Num2Str num = new Num2Str();
            IList<Microsoft.Reporting.WinForms.ReportParameter> param = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", ch.TenCuaHang));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", ch.DiaChi));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", ch.DienThoai));

            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", num.NumberToString(m_PhieuChi.TongTien.ToString())));

            PhieuChiReport phieuChiReport = new PhieuChiReport
            {
                Id = m_PhieuChi.Id,
                NgayChi = m_PhieuChi.NgayChi,
                TongTien = m_PhieuChi.TongTien,
                LyDoChi = m_PhieuChi.LyDoChi.LyDo,
                GhiChu = m_PhieuChi.GhiChu
            };

            ReportHanler.LoadReport(
                this.reportViewer,
                new List<PhieuChiReport> { phieuChiReport },
                "rptPhieuChi.rdlc",
                "PhieuChi",
                param
            );

        }
    }
}