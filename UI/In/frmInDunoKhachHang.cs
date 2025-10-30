using CuahangNongduoc.DTO;
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
    public partial class frmInDunoKhachHang : Form
    {
        CuahangNongduoc.BusinessObject.DuNoKhachHang m_DuNo;
        public frmInDunoKhachHang(CuahangNongduoc.BusinessObject.DuNoKhachHang  dn)
        {
            InitializeComponent();
            m_DuNo = dn;
        }

        private void frmInDunoKhachHang_Load(object sender, EventArgs e)
        {
            Num2Str num = new Num2Str();
            IList<Microsoft.Reporting.WinForms.ReportParameter> param = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", ch.TenCuaHang));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", ch.DiaChi));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", ch.DienThoai));

            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", num.NumberToString(m_DuNo.CuoiKy.ToString())));

            DuNoKhachHangReport duNoKhachHangReport = new DuNoKhachHangReport
            {
                TenKhachHang = m_DuNo.KhachHang.HoTen,
                DiaChi = m_DuNo.KhachHang.DiaChi,
                DienThoai = m_DuNo.KhachHang.DienThoai,
                DauKy = m_DuNo.DauKy,
                PhatSinh = m_DuNo.PhatSinh,
                DaTra = m_DuNo.DaTra,
                CuoiKy = m_DuNo.CuoiKy,
                Thang = m_DuNo.Thang,
                Nam = m_DuNo.Nam
            };

            ReportHanler.LoadReport(
                this.reportViewer,
                new List<DuNoKhachHangReport> { duNoKhachHangReport },
                "rptDuNoKhachHang.rdlc",
                "DuNoKhachHang",
                param
            );
        }
    }
}