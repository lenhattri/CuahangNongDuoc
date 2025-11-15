using Microsoft.Reporting.WinForms;
using System;
using System.Linq;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;

namespace CuahangNongduoc
{
    public partial class frmSoLuongTon : Form
    {
        public frmSoLuongTon()
        {
            InitializeComponent();
        }

        private void frmSoLuongTon_Load(object sender, EventArgs e)
        {
            var raw = ServiceLocator.Resolve<ISanPhamService>().GetInventoryLevels();

            var view = raw.Select(it => new SoLuongTonView
            {
                MaSP = it?.SanPham?.Id ?? string.Empty,
                TenSanPham = it?.SanPham?.TenSanPham ?? string.Empty,
                DonGiaNhap = it?.SanPham?.DonGiaNhap ?? 0,
                GiaBanLe = it?.SanPham?.GiaBanLe ?? 0,
                GiaBanSi = it?.SanPham?.GiaBanSi ?? 0,
                DonViTinh = it?.SanPham?.DonViTinh?.Ten ?? string.Empty,
                SoLuong = it?.SoLuong ?? 0
            }).ToList();

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("SoLuongTonView", view));

            reportViewer.RefreshReport();
            AppTheme.ApplyTheme(this);
        }

        private void SoLuongTonBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }
    }
}
