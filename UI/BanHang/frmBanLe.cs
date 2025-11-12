using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.BLL.Helpers;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.UI.PhieuThuChi;
using CuahangNongduoc.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;


namespace CuahangNongduoc
{
    public partial class frmBanLe : Form
    {
        // ===== Controllers/DAL – 1 nguồn duy nhất, không new lung tung trong event =====
        private SanPhamController ctrlSanPham;
        private KhachHangController ctrlKhachHang;
        private MaSanPhamController ctrlMaSanPham;     // DÙNG CÁI NÀY DUY NHẤT CHO MÃ SẢN PHẨM (LÔ)
        private PhieuBanController ctrlPhieuBan;
        private ChiTietPhieuBanController ctrlChiTiet;
        private PhieuBanChiPhiController ctrlPhieuBanChiPhi;
        private IChiTietPhieuBanDAL chiTietDal;

        // ===== Biến trạng thái/UI =====
        private readonly IList<MaSanPham> _deleted = new List<MaSanPham>();
        private List<ChiPhiPhatSinh> _dsChiPhiDaChon = new List<ChiPhiPhatSinh>();
        private Controll _status = Controll.Normal;

        // ===== Constructor mặc định =====
        public frmBanLe()
        {
            InitializeComponent();

            // DI thủ công – thống nhất factory
            ISanPhamFactory dalSanPham = new SanPhamFactory();
            ctrlSanPham = new SanPhamController(dalSanPham);
            ctrlKhachHang = new KhachHangController();
            ctrlMaSanPham = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
            ctrlPhieuBan = new PhieuBanController(new PhieuBanFactory(), ctrlKhachHang);

            chiTietDal = new ChiTietPhieuBanDAL();
            ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, ctrlMaSanPham);

            ctrlPhieuBanChiPhi = new PhieuBanChiPhiController();

            _status = Controll.AddNew;
        }

        // ===== Constructor inject từ ngoài =====
        public frmBanLe(PhieuBanController ctrlPB)
        {
            InitializeComponent();

            ISanPhamFactory dalSanPham = new SanPhamFactory();
            ctrlSanPham = new SanPhamController(dalSanPham);
            ctrlKhachHang = new KhachHangController();
            ctrlMaSanPham = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
            ctrlPhieuBan = ctrlPB;

            chiTietDal = new ChiTietPhieuBanDAL();
            ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, ctrlMaSanPham);

            ctrlPhieuBanChiPhi = new PhieuBanChiPhiController();

            _status = Controll.Normal;
        }

        // ===== Form Load (đặt lại tên đúng cho khớp file) =====
        private void frmBanLe_Load(object sender, EventArgs e)
        {
            // Nạp nguồn cho combo Sản phẩm
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            EnforceCombo(cmbSanPham, valueMember: "ID", displayMember: "TEN"); // Sửa name cột nếu khác

            // Nạp source cho cột combobox trong grid (ID_MA_SAN_PHAM)
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            // Khách hàng
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
            EnforceCombo(cmbKhachHang, valueMember: "ID", displayMember: "TEN");

            // Nạp phiếu bán lên navigator + binding vào các control
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            // Gắn/đổi sự kiện chỉ 1 lần
            cmbSanPham.SelectedIndexChanged -= cmbSanPham_SelectedIndexChanged;
            cmbSanPham.SelectedIndexChanged += cmbSanPham_SelectedIndexChanged;

            if (bindingNavigator?.BindingSource != null)
            {
                bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                bindingNavigator.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
            }

            // Nếu tạo mới phiếu
            if (_status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
                Allow(true);
                // Hiển thị grid rỗng cho mã phiếu này
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            }
            else
            {
                Allow(false);
            }

            AppTheme.ApplyTheme(this);
        }

        // ===== Force thiết lập ComboBox để không bị lệch ValueMember/DisplayMember =====
        private static void EnforceCombo(ComboBox cb, string valueMember, string displayMember)
        {
            try
            {
                cb.ValueMember = valueMember;
                cb.DisplayMember = displayMember;
            }
            catch
            {
                // Ignore nếu HienthiAutoComboBox đã set; mục tiêu là đảm bảo có ValueMember/DisplayMember
            }
        }

        // ===== Khi đổi record phiếu trên navigator =====
        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (_status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
                _dsChiPhiDaChon = ctrlPhieuBanChiPhi.LayDanhSachTheoPB(txtMaPhieu.Text).ToList();
                TinhTongTien(_dsChiPhiDaChon);
            }
        }

        // ===== Khi chọn SẢN PHẨM: nạp danh sách MÃ SẢN PHẨM (lô) + tính giá xuất gợi ý =====
        private void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue == null) return;

            // Rebind mã sản phẩm theo SP
            cmbMaSanPham.SelectedIndexChanged -= cmbMaSanPham_SelectedIndexChanged;
            ctrlMaSanPham.HienThiAutoComboBox(cmbSanPham.SelectedValue.ToString(), cmbMaSanPham);
            EnforceCombo(cmbMaSanPham, valueMember: "ID", displayMember: "TEN"); // display bạn tùy chỉnh
            cmbMaSanPham.SelectedIndexChanged += cmbMaSanPham_SelectedIndexChanged;

            // Tính giá xuất gợi ý (BQGQ hoặc FIFO) theo ID_SAN_PHAM
            string idSanPham = cmbSanPham.SelectedValue.ToString();
            decimal giaXuat = (CauHinhCuaHang.PhuongThucTinhGiaHienTai == CauHinhCuaHang.PhuongThucTinhGia.BQGQ)
                ? ctrlChiTiet.TinhGiaBinhQuanGiaQuyen(idSanPham)
                : ctrlChiTiet.TinhGiaFIFO(idSanPham);

            txtGiaBQGQ.Text = giaXuat.ToString("#,###0");
        }

        // ===== Khi chọn MÃ SẢN PHẨM: hiển thị đủ Giá nhập/giá sỉ/giá lẻ + set đơn giá mặc định =====
        private void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null) return;

            var idMa = cmbMaSanPham.SelectedValue.ToString();
            MaSanPham masp = ctrlMaSanPham.LayMaSanPham(idMa);
            if (masp == null || masp.SanPham == null) return;

            numDonGia.Value = masp.SanPham.GiaBanLe; // mặc định bán lẻ
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.SanPham.GiaBanLe.ToString("#,###0");
        }

        // ===== Nút Add: validate UI + kiểm tồn ngay trên form + thêm vào grid hiện tại + cập nhật tổng =====
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!TryValidateBeforeAdd(out string err))
            {
                MessageBox.Show(err, "Phiếu Bán", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;

            if (dt == null)
            {
                // Fallback: tạo qua controller rồi ép grid refresh
                DataRow r = ctrlChiTiet.NewRow();
                r["ID_MA_SAN_PHAM"] = cmbMaSanPham.SelectedValue.ToString();
                r["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                r["DON_GIA"] = (decimal)numDonGia.Value;
                r["SO_LUONG"] = (int)numSoLuong.Value;
                r["THANH_TIEN"] = (decimal)numThanhTien.Value;
                ctrlChiTiet.Add(r);

                ((BindingSource)dgvDanhsachSP.DataSource).EndEdit();
                ((BindingSource)dgvDanhsachSP.DataSource).ResetBindings(false);
            }
            else
            {
                var row = dt.NewRow();
                row["ID_MA_SAN_PHAM"] = cmbMaSanPham.SelectedValue.ToString();
                row["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                row["DON_GIA"] = (decimal)numDonGia.Value;
                row["SO_LUONG"] = (int)numSoLuong.Value;
                row["THANH_TIEN"] = (decimal)numThanhTien.Value;
                dt.Rows.Add(row);

                bs.EndEdit();
                bs.ResetBindings(false);
            }

            // Cập nhật tổng theo dữ liệu đang hiển thị
            TinhTongTien(_dsChiPhiDaChon);
        }

        // ===== Remove dòng hiện tại trong grid =====
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var bs = dgvDanhsachSP.DataSource as BindingSource;
            if (bs == null || bs.Current == null) return;

            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Bán Lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var row = (DataRowView)bs.Current;
            // Lưu lại để xử lý hoàn kho khi đang Edit (giữ nguyên logic cũ của bạn)
            _deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]), Convert.ToInt32(row["SO_LUONG"])));

            bs.RemoveCurrent();

            // Cập nhật tổng ngay
            TinhTongTien(_dsChiPhiDaChon);
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Bán Lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                var bs = dgvDanhsachSP.DataSource as BindingSource;
                var row = (DataRowView)bs?.Current;
                if (row != null)
                {
                    _deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]), Convert.ToInt32(row["SO_LUONG"])));
                    // Tổng sẽ tự cập nhật ở DataBindingComplete hoặc bạn chủ động:
                    TinhTongTien(_dsChiPhiDaChon);
                }
            }
        }

        // ===== Tự tính thành tiền khi đổi đơn giá/số lượng =====
        private void numDonGia_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numDonGia.Value * numSoLuong.Value;
        }

        private void numSoLuong_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numDonGia.Value * numSoLuong.Value;
        }

        private void numTongTien_ValueChanged(object sender, EventArgs e)
        {
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        // ===== Lưu (Save) =====
        private void toolLuu_Click(object sender, EventArgs e)
        {
            Luu();
            _status = Controll.Normal;
            Allow(false);
        }

        private void Luu()
        {
            if (!TryValidateBeforeSave(out string err))
            {
                MessageBox.Show(err, "Phiếu Bán", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_status == Controll.AddNew) ThemMoi();
            else CapNhat();
        }

        // ===== Cập nhật phiếu hiện tại =====
        private void CapNhat()
        {
            // Hoàn kho các dòng đã xóa (logic cũ – sửa tên factory)
            var factory = new MaSanPhanFactory();
            foreach (MaSanPham masp in _deleted)
            {
                factory.CapNhatSoLuong(masp.Id, masp.SoLuong);
            }
            _deleted.Clear();

            // Lưu chi tiết & phiếu
            ctrlChiTiet.Save();
            ctrlPhieuBan.Update();

            // Cập nhật chi phí phát sinh
            ctrlPhieuBanChiPhi.CapNhatChiPhiPhatSinh(txtMaPhieu.Text, _dsChiPhiDaChon);

            TinhTongTien(_dsChiPhiDaChon);
        }

        // ===== Thêm mới phiếu =====
        private void ThemMoi()
        {
            // Duplicate-check PHẢI trước khi Add
            var ctrlCheck = new PhieuBanController(new PhieuBanFactory(), new KhachHangController());
            if (ctrlCheck.LayPhieuBan(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu bán này đã tồn tại !", "Phiếu Bán", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow row = ctrlPhieuBan.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["ID_NHAN_VIEN"] = Session.CurrentUser.Id;
            row["ID_KHACH_HANG"] = cmbKhachHang.SelectedValue;
            row["NGAY_BAN"] = dtNgayLapPhieu.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;

            ctrlPhieuBan.Add(row);

            // Tăng mã tự động
            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuBan())
                {
                    ThamSo.GanMaPhieuBan(so + 1);
                }
            }

            // Lưu
            ctrlPhieuBan.Save();
            ctrlChiTiet.Save();
            ctrlPhieuBanChiPhi.LuuChiPhiPhatSinh(txtMaPhieu.Text, _dsChiPhiDaChon);

            TinhTongTien(_dsChiPhiDaChon);
        }

        // ===== Lưu & Tạo mới một phiếu khác =====
        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            ctrlPhieuBan = new PhieuBanController(new PhieuBanFactory(), new KhachHangController());

            _status = Controll.AddNew;
            txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();

            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;

            _dsChiPhiDaChon.Clear();
            _deleted.Clear();

            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            Allow(true);
        }

        // ===== In phiếu =====
        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (_status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại!", "Phiếu Bán Lẻ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ma_phieu = txtMaPhieu.Text;

            var ctrlPB = new PhieuBanController(new PhieuBanFactory(), new KhachHangController());
            var ph = ctrlPB.LayPhieuBan(ma_phieu);
            if (ph == null)
            {
                MessageBox.Show("Không tìm thấy phiếu để in.", "Phiếu Bán Lẻ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new frmInPhieuBan(ph))
            {
                frm.Show();
            }
        }

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
            _status = Controll.Edit;
            Allow(true);
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (_status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu bán này không?", "Phiếu Bán Lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Luu();
                }
            }
            Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            var bs = bindingNavigator?.BindingSource;
            var view = (DataRowView)bs?.Current;
            if (view == null) return;

            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Bán Lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // Hoàn kho tất cả chi tiết trước khi xóa phiếu
            var ds = ctrlChiTiet.ChiTietPhieuBan(view["ID"].ToString());
            var factory = new MaSanPhanFactory();
            foreach (var ct in ds)
            {
                factory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
            }

            bs.RemoveCurrent();
            ctrlPhieuBan.Save();
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            using (var f = new frmKhachHang())
            {
                f.ShowDialog();
            }
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
            EnforceCombo(cmbKhachHang, valueMember: "ID", displayMember: "TEN");
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            using (var f = new frmSanPham())
            {
                f.ShowDialog();
            }
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            EnforceCombo(cmbSanPham, valueMember: "ID", displayMember: "TEN");
        }

        private void btnThemChiPhi_Click(object sender, EventArgs e)
        {
            if (_status == Controll.AddNew)
            {
                MessageBox.Show("Vui lòng lưu phiếu bán trước khi chọn chi phí!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var f = new frmPhieuBanChiPhi(txtMaPhieu.Text, _dsChiPhiDaChon))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    _dsChiPhiDaChon = f.LayDanhSachChiPhiDaChon();
                    TinhTongTien(_dsChiPhiDaChon);
                }
            }
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true; // tránh crash do format
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            // Để trống (đang dùng toolXoa riêng)
        }

        private void Allow(bool val)
        {
            // Mã phiếu thường không cho sửa khi Edit, nhưng giữ theo code gốc
            txtMaPhieu.Enabled = val && _status == Controll.AddNew;
            dtNgayLapPhieu.Enabled = val;
            numTongTien.Enabled = false; // tính tự động
            numDaTra.Enabled = val;
            numConNo.Enabled = false; // tính tự động

            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dgvDanhsachSP.Enabled = val;

            cmbSanPham.Enabled = val;
            cmbMaSanPham.Enabled = val;
            cmbKhachHang.Enabled = val;
        }

        // ================== VALIDATION & NGHIỆP VỤ UI ==================

        // Validate trước khi Add 1 dòng
        private bool TryValidateBeforeAdd(out string error)
        {
            error = null;

            if (cmbSanPham.SelectedValue == null)
            { error = "Chưa chọn Sản phẩm."; return false; }

            if (cmbMaSanPham.SelectedValue == null)
            { error = "Chưa chọn Mã sản phẩm (lô)."; return false; }

            if (numSoLuong.Value <= 0)
            { error = "Số lượng phải > 0."; return false; }

            // Đồng bộ thành tiền (tránh lệch do nhập tay)
            var donGia = (decimal)numDonGia.Value;
            var soLuong = (int)numSoLuong.Value;
            var ttCalc = donGia * soLuong;
            if (numThanhTien.Value != ttCalc)
                numThanhTien.Value = ttCalc;

            // Kiểm tồn tại form: Chọn Lô → check theo lô | FIFO → check tổng theo SP
            string idLo = cmbMaSanPham.SelectedValue.ToString();
            string idSanPham = ((ChiTietPhieuBanDAL)chiTietDal).LayIdSanPhamTuMaSanPham(idLo);
            if (string.IsNullOrWhiteSpace(idSanPham))
            { error = $"Không tìm thấy ID sản phẩm từ lô {idLo}."; return false; }

            var phuongPhap = CauHinhCuaHang.PhuongThucXuatKhoHienTai;
            if (phuongPhap == CauHinhCuaHang.PhuongThucXuatKho.ChonLo)
            {
                var lo = ((ChiTietPhieuBanDAL)chiTietDal).LayThongTinMotLo(idLo);
                if (lo == null || lo.Rows.Count == 0)
                { error = $"Không tìm thấy thông tin lô {idLo}."; return false; }

                int tonLo = Convert.ToInt32(lo.Rows[0]["SO_LUONG"]);
                int daChonTrongGrid = TinhSoLuongDangChoTrongGridTheoLo(idLo);
                int tonKhaDung = tonLo - daChonTrongGrid;

                if (tonKhaDung < soLuong)
                { error = $"Lô {idLo} không đủ hàng. Cần {soLuong}, khả dụng {tonKhaDung}."; return false; }
            }
            else // FIFO
            {
                var dsLo = ((ChiTietPhieuBanDAL)chiTietDal).LayDanhSachMaTheoSanPham(idSanPham);
                int tongTon = 0;
                foreach (DataRow r in dsLo.Rows) tongTon += Convert.ToInt32(r["SO_LUONG"]);

                int daChonTrongGrid = TinhSoLuongDangChoTrongGridTheoSanPham(idSanPham);
                int tonKhaDung = tongTon - daChonTrongGrid;

                if (tonKhaDung < soLuong)
                { error = $"Sản phẩm {idSanPham} không đủ hàng. Cần {soLuong}, khả dụng {tonKhaDung}."; return false; }
            }

            return true;
        }

        // Validate trước khi Save phiếu
        private bool TryValidateBeforeSave(out string error)
        {
            error = null;

            if (cmbKhachHang.SelectedValue == null)
            { error = "Chưa chọn Khách hàng."; return false; }

            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;
            if (dt == null || !dt.Rows.Cast<DataRow>().Any(r => r.RowState != DataRowState.Deleted))
            { error = "Phiếu bán chưa có dòng hàng."; return false; }


            // Recalc tổng để chắc số liệu đúng
            TinhTongTien(_dsChiPhiDaChon);

            if (numTongTien.Value < 0)
            { error = "Tổng tiền không hợp lệ."; return false; }

            if (numDaTra.Value < 0 || numDaTra.Value > numTongTien.Value)
            { error = "Số tiền đã trả không hợp lệ."; return false; }

            return true;
        }

        // Số lượng đang “treo” trong grid (chưa lưu) theo LÔ
        private int TinhSoLuongDangChoTrongGridTheoLo(string idLo)
        {
            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;
            if (dt == null) return 0;

            return dt.Rows.Cast<DataRow>()
                        .Where(r => r.RowState != DataRowState.Deleted &&
                            string.Equals(Convert.ToString(r["ID_MA_SAN_PHAM"]), idLo))
                        .Sum(r => Convert.ToInt32(r["SO_LUONG"]));
        }

        // Số lượng đang “treo” trong grid (chưa lưu) theo SẢN PHẨM
        private int TinhSoLuongDangChoTrongGridTheoSanPham(string idSanPham)
        {
            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;
            if (dt == null) return 0;

            int sum = 0;
            foreach (DataRow r in dt.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted))
            {
                var idLo = Convert.ToString(r["ID_MA_SAN_PHAM"]);
                var spOfLo = ((ChiTietPhieuBanDAL)chiTietDal).LayIdSanPhamTuMaSanPham(idLo);
                if (spOfLo == idSanPham) sum += Convert.ToInt32(r["SO_LUONG"]);
            }
            return sum;
          
        }

        // Tính tổng tiền dựa trên DataTable đang gắn vào grid + chi phí phát sinh
        private void TinhTongTien(IList<ChiPhiPhatSinh> chiPhis)
        {
            long tongChiPhi = chiPhis?.Sum(x => (long)x.SoTien) ?? 0;

            decimal tongHang = 0;
            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;
            if (dt != null)
            {
                tongHang = dt.Rows.Cast<DataRow>()
                    .Where(r => r.RowState != DataRowState.Deleted)
                    .Sum(r => Convert.ToDecimal(r["THANH_TIEN"]));
            }

            numTongTien.Value = tongHang + tongChiPhi;
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }
    }
}
