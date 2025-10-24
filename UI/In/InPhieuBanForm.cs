using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Utils;
// ?? thêm dòng này ?? nh?n ra NumberToVietnamese
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class InPhieuBanForm : Form
    {
        private PhieuBan m_PhieuBan;

        public InPhieuBanForm(PhieuBan ph)
        {
            InitializeComponent();
            reportViewer.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
            reportViewer.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            m_PhieuBan = ph;
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();
            e.DataSources.Add(new ReportDataSource(
                "CuahangNongduoc_BusinessObject_ChiTietPhieuBan",
                m_PhieuBan.ChiTiet
            ));
        }

        private void InPhieuBanForm_Load(object sender, EventArgs e)
        {
            // ? T?o ??i t??ng ??c s? thành ch?
            var num = new Num2Str();

            var param = new List<ReportParameter>();
            CuaHang ch = ThamSo.LayCuaHang();

            param.Add(new ReportParameter("ten_cua_hang", ch.TenCuaHang));
            param.Add(new ReportParameter("dia_chi", ch.DiaChi));
            param.Add(new ReportParameter("dien_thoai", ch.DienThoai));

            // ? G?i ?úng hàm trong NumberToVietnamese
            param.Add(new ReportParameter(
                "bang_chu",
                num.NumberToString(m_PhieuBan.TongTien.ToString())
            ));

            reportViewer.LocalReport.SetParameters(param);
            PhieuBanBindingSource.DataSource = m_PhieuBan;
            reportViewer.RefreshReport();
        }
    }
}
