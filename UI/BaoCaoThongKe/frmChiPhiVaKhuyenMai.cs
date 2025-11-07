using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.DTO;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.BaoCaoThongKe
{
    public partial class frmChiPhiVaKhuyenMai : Form
    {
        private PhieuBanFactory phieuBanFactory = new PhieuBanFactory();
        public frmChiPhiVaKhuyenMai()
        {
            InitializeComponent();
        }

        private void frmChiPhiVaKhuyenMai_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();

            IList <ChiPhiKhuyenMaiReport> phieuChiReport = new List<ChiPhiKhuyenMaiReport>();
            foreach (var item in phieuBanFactory.DanhsachPhieu().Select())
            {
                ChiPhiKhuyenMaiReport chiPhiKhuyenMaiReport = new ChiPhiKhuyenMaiReport()
                {
                    NgayLap = DateTime.Parse(item["NGAY_BAN"].ToString()),
                    TongTien = item["TONG_TIEN"].ToString() != "" ? decimal.Parse(item["TONG_TIEN"].ToString()) : 0,
                };
                phieuChiReport.Add(chiPhiKhuyenMaiReport);
            }

            ReportHanler.LoadReport(
                this.reportViewer1,
                phieuChiReport,
                "rptChiPhi_KhuyenMai.rdlc",
                "ChiPhiKhuyenMai"
            );
        }
    }
}
