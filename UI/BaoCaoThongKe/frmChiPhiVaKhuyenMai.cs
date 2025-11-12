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
            LoadReport(phieuBanFactory.DanhsachPhieu());
        }
        private void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            var ds = phieuBanFactory.DanhsachPhieu();
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;
            var dsFiltered = ds.Clone();
            foreach (DataRow row in ds.Rows)
            {
                DateTime ngayBan = Convert.ToDateTime(row["NGAY_BAN"]);
                if (ngayBan >= tuNgay && ngayBan <= denNgay)
                {
                    dsFiltered.ImportRow(row);
                }
            }
            if(dsFiltered.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong khoảng thời gian đã chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                LoadReport(dsFiltered);
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            frmChiPhiVaKhuyenMai_Load(sender,e);
        }
        public void LoadReport(DataTable dt)
        {
            IList<ChiPhiKhuyenMaiReport> phieuChiReport = new List<ChiPhiKhuyenMaiReport>();

            foreach (DataRow item in dt.Rows) // Fix: Use DataRow type for iteration
            {
                ChiPhiKhuyenMaiReport chiPhiKhuyenMaiReport = new ChiPhiKhuyenMaiReport()
                {
                    Id = int.Parse(item["ID"].ToString()), // No change needed here
                    TenKhachHang = item["TEN_KHACH_HANG"]?.ToString() ?? "", // Added null-conditional operator
                    TenNhanVien = item["TEN_NHAN_VIEN"]?.ToString() ?? "",
                    NgayLap = DateTime.Parse(item["NGAY_BAN"].ToString()), // No change needed here
                    LoaiChiPhi = item["LOAI_CHI_PHI"]?.ToString() ?? "",
                    SoTien = decimal.Parse(item["SO_TIEN"].ToString())
                };
                phieuChiReport.Add(chiPhiKhuyenMaiReport);
            }

            ReportHanler.LoadReport(
                this.reportViewer1,
                phieuChiReport,
                "rptChiPhi_KhuyenMai.rdlc",
                "ChiPhiKhuyenMai"
            );
            this.reportViewer1.RefreshReport();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dtpTuNgay_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtpDenNgay_ValueChanged(object sender, EventArgs e)
        {

        }

        
    }
}
