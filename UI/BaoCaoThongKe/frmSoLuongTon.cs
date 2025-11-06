using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils;
using Microsoft.Reporting.WinForms;
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
    public partial class frmSoLuongTon : Form
    {
        public frmSoLuongTon()
        {
            InitializeComponent();
        }

        private void frmSoLuongTon_Load(object sender, EventArgs e)
        {
            AppTheme.ApplyTheme(this);
            var raw = CuahangNongduoc.Controller.SanPhamController.LaySoLuongTon();

            var view = raw.Select(it => new SoLuongTonView
            {
                MaSP = it?.SanPham?.Id ?? "",
                TenSanPham = it?.SanPham?.TenSanPham ?? "",
                DonGiaNhap = it?.SanPham?.DonGiaNhap ?? 0,
                GiaBanLe = it?.SanPham?.GiaBanLe ?? 0,
                GiaBanSi = it?.SanPham?.GiaBanSi ?? 0,
                DonViTinh = it?.SanPham?.DonViTinh?.Ten ?? "",
                SoLuong = it?.SoLuong ?? 0
            }).ToList();

            reportViewer.ProcessingMode = ProcessingMode.Local;

            // nếu set ReportPath bằng code (không bắt buộc nếu đã set trong Designer)
            // reportViewer.LocalReport.ReportPath = @"Reports\SoLuongTon.rdlc";

            reportViewer.LocalReport.DataSources.Clear();
            // *** TÊN NÀY PHẢI TRÙNG VỚI DATASET TRONG RDLC SAU KHI BẠN SỬA Ở BƯỚC 2 ***
            reportViewer.LocalReport.DataSources.Add(
                new ReportDataSource("SoLuongTonView", view)
            );

            reportViewer.RefreshReport();
        }

        private void SoLuongTonBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}