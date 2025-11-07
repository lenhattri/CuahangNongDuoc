using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.PhieuThuChi
{
    public partial class frmPhieuBanChiPhi: Form
    {
        private string _maPhieuBan;
        private List<ChiPhiPhatSinh> _dsDaChon;
        ChiPhiPhatSinhController ctrlChiPhi = new ChiPhiPhatSinhController();
        
        public frmPhieuBanChiPhi(string maPhieuBan, List<ChiPhiPhatSinh> ds)
        {
            InitializeComponent();
            _maPhieuBan = maPhieuBan;
            _dsDaChon = ds ?? new List<ChiPhiPhatSinh>();

        }

        private void frmPhieuBanChiPhi_Load(object sender, EventArgs e)
        {
            dgvChiPhi.AutoGenerateColumns = false;
            DataTable tbl = ctrlChiPhi.DanhSachChiPhiPhatSinh();
            dgvChiPhi.DataSource = tbl;
            foreach (DataGridViewRow row in dgvChiPhi.Rows)
            {
                string maChiPhi = row.Cells["ID"].Value.ToString();
                if (_dsDaChon.Any(cp => cp.Id == maChiPhi))
                {
                    row.Cells["Chon"].Value = true;
                }
            }
        }
        public List<ChiPhiPhatSinh> LayDanhSachChiPhiDaChon()
        {
            List<ChiPhiPhatSinh> ds = new List<ChiPhiPhatSinh>();
            foreach (DataGridViewRow row in dgvChiPhi.Rows)
            {
                // Nếu là dòng mới trống hoặc chưa chọn thì bỏ qua
                if (row.IsNewRow) continue;

                bool duocChon = false;
                if (row.Cells["Chon"].Value != null)
                    duocChon = Convert.ToBoolean(row.Cells["Chon"].Value);
                if (duocChon)
                {
                    var cp = new ChiPhiPhatSinh
                    {
                        Id = row.Cells["ID"].Value.ToString(),
                        TenChiPhi = row.Cells["TEN_CHI_PHI"].Value.ToString(),
                        LoaiChiPhi = row.Cells["LOAI_CHI_PHI"].Value.ToString(),
                        SoTien = Convert.ToInt32(row.Cells["SO_TIEN"].Value)
                    };
                    ds.Add(cp);
                }
            }
            return ds;
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            _dsDaChon = LayDanhSachChiPhiDaChon();
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
