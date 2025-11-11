using CuahangNongduoc.Controller;
using CuahangNongduoc.DAL.DataLayer;
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
        private UserDAL userDAL = new UserDAL();
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
                decimal TongTien = item["TONG_TIEN"].ToString() != "" ? decimal.Parse(item["TONG_TIEN"].ToString()) : 0;
                decimal GiamGia = item["GIAM_GIA"].ToString() != "" ? decimal.Parse(item["GIAM_GIA"].ToString()) : 0;

                ChiPhiKhuyenMaiReport chiPhiKhuyenMaiReport = new ChiPhiKhuyenMaiReport()
                {
                    NgayLap = DateTime.Parse(item["NGAY_BAN"].ToString()),
                    TongTien = TongTien,
                    TenNguoiLap = userDAL.LayTenNguoiDungTheoId(item["ID_NHAN_VIEN"].ToString()),
                    GiamGia = GiamGia,
                    TongTienSauGiamGia = TongTien - GiamGia
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
