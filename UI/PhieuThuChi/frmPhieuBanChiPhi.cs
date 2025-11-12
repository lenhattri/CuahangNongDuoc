using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.PhieuThuChi
{
    public partial class frmPhieuBanChiPhi : Form
    {
        private readonly string _maPhieuBan;
        private List<ChiPhiPhatSinh> _dsDaChon;
        private readonly ChiPhiPhatSinhController ctrlChiPhi;

        public frmPhieuBanChiPhi(string maPhieuBan, List<ChiPhiPhatSinh> ds)
        {
            InitializeComponent();

            _maPhieuBan = maPhieuBan;
            _dsDaChon = ds ?? new List<ChiPhiPhatSinh>();

            // Tạo controller trực tiếp với factory, không cần DI container
            ctrlChiPhi = new ChiPhiPhatSinhController(new ChiPhiPhatSinhFactory());
        }

        private void frmPhieuBanChiPhi_Load(object sender, EventArgs e)
        {
            dgvChiPhi.AutoGenerateColumns = false;

            // Lấy danh sách chi phí phát sinh
            DataTable tbl = ctrlChiPhi.DanhSachChiPhiPhatSinh();
            dgvChiPhi.DataSource = tbl;

            // đánh dấu những chi phí đã chọn trước đó
            foreach (DataGridViewRow row in dgvChiPhi.Rows)
            {
                if (row.IsNewRow) continue;

                string maChiPhi = row.Cells["ID"].Value?.ToString();
                if (!string.IsNullOrEmpty(maChiPhi) && _dsDaChon.Any(cp => cp.Id == maChiPhi))
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
                if (row.IsNewRow) continue;

                bool duocChon = row.Cells["Chon"].Value != null && Convert.ToBoolean(row.Cells["Chon"].Value);
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
            lblThanhTien.Text = "Thành tiền: "+ds.Sum(c => c.SoTien).ToString("N0") +"đ";
            return ds;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            _dsDaChon = LayDanhSachChiPhiDaChon();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dgvChiPhi_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(dgvChiPhi.Columns[e.ColumnIndex].Name == "Chon")
            {
                LayDanhSachChiPhiDaChon();
            }
        }

        private void dgvChiPhi_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if(dgvChiPhi.IsCurrentCellDirty)
            {
                dgvChiPhi.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
