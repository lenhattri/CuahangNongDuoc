using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;
using CuahangNongduoc.Utils.Functions;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
namespace CuahangNongduoc
{
    public partial class frmSoLuongTon : Form
    {
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

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
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/bao-cao/so-luong-ton";
                IFU_Helper.IFU(url);
            }
        }

        private void SoLuongTonBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }
    }
}
