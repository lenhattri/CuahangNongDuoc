using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmDoanhThu : Form
    {
        private readonly ChiTietPhieuBanController _ctrlChiTiet;

        public frmDoanhThu()
        {
            InitializeComponent();
            IChiTietPhieuBanDAL chiTietDal = new ChiTietPhieuBanDAL();
            MaSanPhamController maSpCtrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
            _ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, maSpCtrl);
        }

        private void frmDoanhThu_Load(object sender, EventArgs e)
        {
            cmbThang.SelectedIndex = DateTime.Now.Month - 1;
            numNam.Value = DateTime.Now.Year;
            LoadReport(_ctrlChiTiet.BaoCaoDoanhThu());
            var param = new List<Microsoft.Reporting.WinForms.ReportParameter>
            {
                new Microsoft.Reporting.WinForms.ReportParameter("ngay", "Tất cả")
            };
            reportViewer.LocalReport.SetParameters(param);
            AppTheme.ApplyTheme(this);
            this.reportViewer.RefreshReport();
        }
        public void LoadReport(IList<ChiTietPhieuBan> chiTiet)
        {
            IList<DoanhThuReport> dtReport = new List<DoanhThuReport>();
            foreach (var ct in chiTiet)
            {
                Console.WriteLine($"SL: {ct.SoLuong}, Giá nhập: {ct.MaSanPham.GiaNhap}, Thành tiền: {ct.ThanhTien}");
                dtReport.Add(new DoanhThuReport
                {
                    TenSanPham = ct.MaSanPham.SanPham.TenSanPham,
                    MaSanPham = ct.MaSanPham.Id,
                    SoLuongBan = ct.SoLuong,
                    GiaVon = ct.MaSanPham.GiaNhap * ct.SoLuong,
                    DoanhThu = ct.ThanhTien,
                    LoiNhuan = ct.ThanhTien - (ct.SoLuong * ct.MaSanPham.GiaNhap)
                });
            }
            ReportHanler.LoadReport(
                this.reportViewer,
                dtReport,
                "rptDoanhThu.rdlc",
                "DoanhThu");
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
            var dt = _ctrlChiTiet.BaoCaoDoanhThu();
            int thangChon = Convert.ToInt32(cmbThang.SelectedValue);
            int namChon = Convert.ToInt32(numNam.Value);
            var dtFiltered = dt.Where(ct => ct.PhieuBan.NgayBan.Month == thangChon && ct.PhieuBan.NgayBan.Year == namChon).ToList();
            LoadReport(dtFiltered);
            this.reportViewer.RefreshReport();
        }

        

        private void btnXemNgay_Click(object sender, EventArgs e)
        {
            var param = new List<Microsoft.Reporting.WinForms.ReportParameter>
            {
                new Microsoft.Reporting.WinForms.ReportParameter("ngay", "Ngày " + dtNgay.Value.Date.ToString("dd/MM/yyyy"))
            };

            reportViewer.LocalReport.SetParameters(param);
            var dt = _ctrlChiTiet.BaoCaoDoanhThu();
            DateTime ngayChon = dtNgay.Value.Date;
            var dtFiltered = dt.Where(ct => ct.PhieuBan.NgayBan.Date == ngayChon).ToList();
            LoadReport(dtFiltered);
            this.reportViewer.RefreshReport();
        }
    }
}