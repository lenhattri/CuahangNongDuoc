using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// GIỮ: các controller cũ cho các phần chưa refactor
using CuahangNongduoc.Controller;
using CuahangNongduoc.BusinessObject;

// NEW: services cho SanPham/MaSanPham
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BLL.Services;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc
{
    public partial class frmNhapHang : Form
    {
        // GIỮ: controller cũ cho phiếu nhập, NCC, chi tiết (dựa trên DataTable)
        PhieuNhapController ctrl = new PhieuNhapController();
        MaSanPhamController ctrlMaSP = new MaSanPhamController();
        NhaCungCapController ctrlNCC = new NhaCungCapController();

        // NEW: thay SanPhamController bằng service
        private readonly ISanPhamService _sanPhamService;
        private readonly IMaSanPhamService _maSanPhamService;

        PhieuNhap m_PhieuNhap = null;

        Controll status = Controll.Normal;

        public frmNhapHang()
        {
            InitializeComponent();

            _sanPhamService = new SanPhamService();
            _maSanPhamService = new MaSanPhamService();

            status = Controll.AddNew;
        }

        public frmNhapHang(PhieuNhapController ctrlPN) : this()
        {
            this.ctrl = ctrlPN;
            status = Controll.Normal;
        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
                ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            // ===== SAN PHAM Combo trên form =====
            var dtSp = _sanPhamService.DanhSachSanPham_DataTable();
            cmbSanPham.DataSource = dtSp;
            cmbSanPham.DisplayMember = "TEN_SAN_PHAM";
            cmbSanPham.ValueMember = "ID";
            cmbSanPham.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbSanPham.AutoCompleteSource = AutoCompleteSource.ListItems;

            // ===== SAN PHAM Column trên lưới =====
            colSanPham.DataSource = dtSp;
            colSanPham.DisplayMember = "TEN_SAN_PHAM";
            colSanPham.ValueMember = "ID";
            colSanPham.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;

            // ===== NHA CUNG CAP =====
            ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);

            // ===== Header phiếu nhập =====
            ctrl.HienthiPhieuNhap(bindingNavigator, txtMaPhieu, cmbNhaCungCap, dtNgayNhap,
                                  numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);

            // ===== Chi tiết phiếu nhập (DataTable) =====
            ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);

            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
                Allow(true);
            }
            else
            {
                Allow(false);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // kiểm tra xem mã lô đã tồn tại trong DB chưa (service)
            var idMa = txtMaSo.Text.Trim();
            var existed = !string.IsNullOrEmpty(idMa) ? _maSanPhamService.LayMaSanPhamBasic(idMa) : null;

            if (existed == null)
            {
                // kiểm tra trùng ngay trong lưới
                foreach (DataGridViewRow view in dataGridView.Rows)
                {
                    var cellVal = view.Cells["colMaSanPham"].Value;
                    if (cellVal != null && idMa.Equals(Convert.ToString(cellVal)))
                    {
                        MessageBox.Show("Mã sản phẩm này đã tồn tại trong danh sách! Vui lòng nhập lại!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (idMa.Length <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Mã sản phẩm!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (numGiaNhap.Value <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Đơn giá!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (numSoLuong.Value <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Số lượng!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (dtNgaySanXuat.Value >= dtNgayHetHan.Value)
                {
                    MessageBox.Show("Ngày hết hạn phải lớn hơn ngày sản xuất!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    numTongTien.Value += numThanhTien.Value;

                    DataRow row = ctrlMaSP.NewRow();
                    row["ID_SAN_PHAM"] = cmbSanPham.SelectedValue;
                    row["ID_PHIEU_NHAP"] = txtMaPhieu.Text;
                    row["ID"] = idMa;
                    row["DON_GIA_NHAP"] = (int)numGiaNhap.Value;   // DB int
                    row["SO_LUONG"] = (int)numSoLuong.Value;   // DB int

                    // SỬA BUG: NGAY_NHAP phải lấy từ dtNgayNhap (header), không phải ngày SX
                    row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
                    row["NGAY_SAN_XUAT"] = dtNgaySanXuat.Value.Date;
                    row["NGAY_HET_HAN"] = dtNgayHetHan.Value.Date;

                    ctrlMaSP.Add(row);
                }
            }
            else
            {
                MessageBox.Show("Mã sản phẩm này đã tồn tại! Vui lòng nhập lại!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void numGiaNhap_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numGiaNhap.Value * numSoLuong.Value;
        }

        private void toolLuuThoat_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            this.Luu();
            status = Controll.Normal;
            this.Allow(false);
        }

        void Luu()
        {
            if (status == Controll.AddNew)
            {
                ThemMoi();
            }
            else
            {
                CapNhat();
            }
        }

        void CapNhat()
        {
            ctrlMaSP.Save();   // lưu chi tiết lô
            ctrl.Update();     // cập nhật đầu phiếu
        }

        void ThemMoi()
        {
            DataRow row = ctrl.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["TONG_TIEN"] = (int)numTongTien.Value;
            row["ID_NHA_CUNG_CAP"] = cmbNhaCungCap.SelectedValue;
            row["DA_TRA"] = (int)numDaTra.Value;
            row["CON_NO"] = (int)numConNo.Value;
            ctrl.Add(row);

            PhieuNhapController ctrlPN = new PhieuNhapController();

            if (ctrlPN.LayPhieuNhap(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu nhập này đã tồn tại!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuNhap())
                {
                    ThamSo.GanMaPhieuNhap(so + 1);
                }
            }

            ctrl.Save();      // lưu đầu phiếu
            ctrlMaSP.Save();  // lưu chi tiết lô

            // Cập nhật giá nhập bình quân của sản phẩm
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;

                var idSp = Convert.ToString(view.Cells["colSanPham"].Value);
                var giaNhap = Convert.ToInt64(view.Cells["colDonGiaNhap"].Value);
                var soLuong = Convert.ToInt64(view.Cells["colSoLuong"].Value);

                _sanPhamService.CapNhatGiaNhap(idSp, giaNhap, soLuong);
            }
        }

        private void toolLuuThem_Click(object sender, EventArgs e)
        {
            ctrl = new PhieuNhapController();
            status = Controll.AddNew;

            txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;
            ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
            this.Allow(true);
        }

        private void toolSavePrint_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu nhập hiện tại!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string ma_phieu = txtMaPhieu.Text;
                PhieuNhapController ctrlPN = new PhieuNhapController();
                CuahangNongduoc.BusinessObject.PhieuNhap ph = ctrlPN.LayPhieuNhap(ma_phieu);

                frmInPhieuNhap PhieuNhap = new frmInPhieuNhap(ph);
                PhieuNhap.Show();
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu nhập này không?", "Phieu Nhap Hang", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Luu();
                }
            }
            this.Close();
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            // reload danh sách sản phẩm sau khi thêm mới
            var dtSp = _sanPhamService.DanhSachSanPham_DataTable();
            cmbSanPham.DataSource = dtSp;
            colSanPham.DataSource = dtSp;
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void Allow(bool val)
        {
            txtMaPhieu.Enabled = val;
            cmbNhaCungCap.Enabled = val;
            dtNgayNhap.Enabled = val;
            numTongTien.Enabled = val;
            numDaTra.Enabled = val;
            numConNo.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dataGridView.Enabled = val;
        }

        private void toolChinhsua_Click(object sender, EventArgs e)
        {
            status = Controll.Edit;
            this.Allow(true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa dòng chi tiết này không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BindingSource bs = (BindingSource)dataGridView.DataSource;
                DataRowView row = (DataRowView)bs.Current;
                if (row != null)
                {
                    numTongTien.Value -= Convert.ToInt64(row["DON_GIA_NHAP"]) * Convert.ToInt64(row["SO_LUONG"]);
                    bs.RemoveCurrent();
                }
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa dòng chi tiết này không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                numTongTien.Value -= Convert.ToInt64(e.Row.Cells["colDonGiaNhap"].Value) * Convert.ToInt64(e.Row.Cells["colSoLuong"].Value);
            }
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa phiếu nhập hiện tại không?", "Phieu Nhap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                ctrl.Save();
            }
        }

        private void numDaTra_ValueChanged(object sender, EventArgs e)
        {
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmNhaCungCap NCC = new frmNhaCungCap();
            NCC.ShowDialog();
            ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);
        }
    }
}
