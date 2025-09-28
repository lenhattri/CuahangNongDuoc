using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CuahangNongduoc.BusinessObject; // giữ để dùng các controller cũ khác
using CuahangNongduoc.Controller;     // KhachHang/PhieuBan/ChiTietPhieuBan vẫn dùng tạm

// NEW (BLL)
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BLL.Services;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc
{
    public partial class frmBanLe : Form
    {
        // === GIỮ LẠI CÁC CONTROLLER KHÁC ===
        KhachHangController ctrlKhachHang = new KhachHangController();
        PhieuBanController ctrlPhieuBan = new PhieuBanController();
        ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController();

        // === THAY THẾ: dùng Service thay cho SanPhamController/MaSanPhamController ===
        private readonly ISanPhamService _sanPhamService;
        private readonly IMaSanPhamService _maSanPhamService;

        // Danh sách “đã xoá” để cộng trả tồn mã lô (id mã, số lượng)
        private IList<Tuple<string, int>> deleted = new List<Tuple<string, int>>();

        Controll status = Controll.Normal;

        public frmBanLe()
        {
            InitializeComponent();
            // Khởi tạo service (dùng connection string mặc định trong GlobalConfig)
            _sanPhamService = new SanPhamService();
            _maSanPhamService = new MaSanPhamService();

            status = Controll.AddNew;
        }

        public frmBanLe(PhieuBanController ctrlPB) : this()
        {
            this.ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            // ====== SAN PHAM: Combo chính (theo tên) ======
            var dtSanPham = _sanPhamService.DanhSachSanPham_DataTable();
            cmbSanPham.DataSource = dtSanPham;
            cmbSanPham.DisplayMember = "TEN_SAN_PHAM";
            cmbSanPham.ValueMember = "ID";
            cmbSanPham.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbSanPham.AutoCompleteSource = AutoCompleteSource.ListItems;

            // ====== MA SAN PHAM: cột combobox trên lưới (theo mã còn hàng) ======
            // colMaSanPham là DataGridViewComboBoxColumn đã có sẵn trên form
            colMaSanPham.DataSource = _maSanPhamService.DanhSachConHang_Table();
            colMaSanPham.DisplayMember = "ID";
            colMaSanPham.ValueMember = "ID";
            colMaSanPham.DataPropertyName = "ID_MA_SAN_PHAM";
            colMaSanPham.HeaderText = "Mã sản phẩm";
            colMaSanPham.AutoComplete = true;

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            // Khách hàng như cũ
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);

            // Phiếu bán như cũ
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu,
                dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);

            // Chi tiết như cũ (tạm thời dùng Controller cũ)
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);

            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            }
            else
            {
                this.Allow(false);
            }
        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            }
        }

        // Khi chọn Sản phẩm => nạp danh sách mã còn hàng vào cmbMaSanPham
        void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue != null)
            {
                cmbMaSanPham.SelectedIndexChanged -= new EventHandler(cmbMaSanPham_SelectedIndexChanged);

                var idSp = cmbSanPham.SelectedValue.ToString();
                var dtMa = _maSanPhamService.DanhSachMaSanPhamConHang_Table(idSp);

                cmbMaSanPham.DataSource = dtMa;
                cmbMaSanPham.DisplayMember = "ID";
                cmbMaSanPham.ValueMember = "ID";

                cmbMaSanPham.SelectedIndexChanged += new EventHandler(cmbMaSanPham_SelectedIndexChanged);
            }
        }

        // Khi chọn Mã sản phẩm => hiển thị giá bán lẻ/giá nhập/giá sỉ/giá bình quân
        void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null) return;

            var idMa = cmbMaSanPham.SelectedValue.ToString();
            var msp = _maSanPhamService.LayMaSanPham(idMa);
            if (msp == null) return;

            var sp = _sanPhamService.LaySanPham(msp.IdSanPham); // load sản phẩm để lấy giá bán/gia bình quân
            var giaBanLe = sp != null ? sp.GiaBanLe : 0;
            var giaBanSi = sp != null ? sp.GiaBanSi : 0;
            var giaBQ = sp != null ? sp.DonGiaNhap : 0;

            // NumericUpDown dùng decimal
            numDonGia.Value = (decimal)giaBanLe;

            txtGiaNhap.Text = msp.DonGiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = giaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = giaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = giaBQ.ToString("#,###0");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã sản phẩm !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập Số lượng !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numDonGia.Value * numSoLuong.Value != numThanhTien.Value)
            {
                MessageBox.Show("Thành tiền sai!", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                numTongTien.Value += numThanhTien.Value;

                DataRow row = ctrlChiTiet.NewRow(); // vẫn dùng controller cũ cho chi tiết
                row["ID_MA_SAN_PHAM"] = cmbMaSanPham.SelectedValue;
                row["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                row["DON_GIA"] = (int)numDonGia.Value;     // DB int
                row["SO_LUONG"] = (int)numSoLuong.Value;   // DB int
                row["THANH_TIEN"] = (int)numThanhTien.Value;
                ctrlChiTiet.Add(row);
            }
        }

        private void numDonGia_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numDonGia.Value * numSoLuong.Value;
        }

        private void numTongTien_ValueChanged(object sender, EventArgs e)
        {
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            this.Luu();
            status = Controll.Normal;
            this.Allow(false);
        }

        void Luu()
        {
            if (status == Controll.AddNew) ThemMoi();
            else CapNhat();
        }

        void CapNhat()
        {
            // Cộng trả tồn lại cho các mã đã xoá khỏi chi tiết
            foreach (var t in deleted)
            {
                // t.Item1 = ID_MA_SAN_PHAM, t.Item2 = SO_LUONG
                _maSanPhamService.CapNhatSoLuong(t.Item1, t.Item2);
            }
            deleted.Clear();

            ctrlChiTiet.Save();
            ctrlPhieuBan.Update();
        }

        void ThemMoi()
        {
            DataRow row = ctrlPhieuBan.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["ID_KHACH_HANG"] = cmbKhachHang.SelectedValue;
            row["NGAY_BAN"] = dtNgayLapPhieu.Value.Date;
            row["TONG_TIEN"] = (int)numTongTien.Value;
            row["DA_TRA"] = (int)numDaTra.Value;
            row["CON_NO"] = (int)numConNo.Value;
            ctrlPhieuBan.Add(row);

            PhieuBanController ctrl = new PhieuBanController();

            if (ctrl.LayPhieuBan(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu bán này đã tồn tại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuBan())
                {
                    ThamSo.GanMaPhieuBan(so + 1);
                }
            }

            ctrlPhieuBan.Save();
            ctrlChiTiet.Save();
        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            ctrlPhieuBan = new PhieuBanController();
            status = Controll.AddNew;
            txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            numTongTien.Value = 0;
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            this.Allow(true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);

                // Lưu lại để cộng trả tồn khi Save
                deleted.Add(Tuple.Create(
                    Convert.ToString(row["ID_MA_SAN_PHAM"]),
                    Convert.ToInt32(row["SO_LUONG"])
                ));

                bs.RemoveCurrent();
            }
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);

                // Lưu lại để cộng trả tồn khi Save
                deleted.Add(Tuple.Create(
                    Convert.ToString(row["ID_MA_SAN_PHAM"]),
                    Convert.ToInt32(row["SO_LUONG"])
                ));
            }
        }

        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại!", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                String ma_phieu = txtMaPhieu.Text;

                PhieuBanController ctrlPB = new PhieuBanController();
                CuahangNongduoc.BusinessObject.PhieuBan ph = ctrlPB.LayPhieuBan(ma_phieu);

                frmInPhieuBan InPhieuBan = new frmInPhieuBan(ph);
                InPhieuBan.Show();
            }
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e) { }

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
            status = Controll.Edit;
            this.Allow(true);
        }

        void Allow(bool val)
        {
            txtMaPhieu.Enabled = val;
            dtNgayLapPhieu.Enabled = val;
            numTongTien.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dgvDanhsachSP.Enabled = val;
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu bán này không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Luu();
                }
            }
            this.Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            DataRowView view = (DataRowView)bindingNavigator.BindingSource.Current;
            if (view != null)
            {
                if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                    IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());

                    // Trả tồn lại theo chi tiết đã xoá
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        // ct.MaSanPham.Id và ct.SoLuong là từ layer cũ, dùng tạm
                        _maSanPhamService.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                    }

                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrlPhieuBan.Save();
                }
            }
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmKhachHang KhachHang = new frmKhachHang();
            KhachHang.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();

            // Reload nguồn sản phẩm (đã đổi sang service)
            var dtSanPham = _sanPhamService.DanhSachSanPham_DataTable();
            cmbSanPham.DataSource = dtSanPham;
        }
    }
}
