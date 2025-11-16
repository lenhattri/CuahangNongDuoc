using CuahangNongduoc.BLL.Controller;
using CuahangNongduoc.BLL.Helpers;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Properties;
using CuahangNongduoc.UI.PhieuThuChi;
using CuahangNongduoc.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmBanSi : Form
    {
        // ======= Controllers/DAL (một nguồn duy nhất) =======
        private SanPhamController ctrlSanPham;
        private KhachHangController ctrlKhachHang;
        private MaSanPhamController ctrlMaSanPham;
        private PhieuBanController ctrlPhieuBan;
        private ChiTietPhieuBanController ctrlChiTiet;
        private PhieuBanChiPhiController ctrlPhieuBanChiPhi;
        private IChiTietPhieuBanDAL _chiTietDal;

        // ======= State/UI =======
        private readonly IMaSanPhanFactory _maSpDal;
        private readonly IList<MaSanPham> _deleted = new List<MaSanPham>();
        private List<ChiPhiPhatSinh> _dsChiPhiDaChon = new List<ChiPhiPhatSinh>();
        private Controll _status = Controll.Normal;

        // ======= Constructor chuẩn DI =======
        public frmBanSi()
        {
            InitializeComponent();

            _maSpDal = new MaSanPhanFactory();
            _chiTietDal = new ChiTietPhieuBanDAL();

            var phieuBanDal = new PhieuBanFactory();
            var khachHangCtrl = new KhachHangController();
            var maSpCtrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());

            ctrlSanPham = new SanPhamController(new SanPhamFactory());
            ctrlKhachHang = khachHangCtrl;
            ctrlMaSanPham = maSpCtrl;
            ctrlPhieuBan = new PhieuBanController(phieuBanDal, khachHangCtrl);
            ctrlChiTiet = new ChiTietPhieuBanController(_chiTietDal, maSpCtrl);
            ctrlPhieuBanChiPhi = new PhieuBanChiPhiController();

            _status = Controll.AddNew;
        }

        // ======= Constructor inject controller từ ngoài =======
        public frmBanSi(PhieuBanController ctrlPB)
        {
            InitializeComponent();

            _maSpDal = new MaSanPhanFactory();
            _chiTietDal = new ChiTietPhieuBanDAL();

            var maSpCtrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());

            ctrlPhieuBan = ctrlPB;
            ctrlChiTiet = new ChiTietPhieuBanController(_chiTietDal, maSpCtrl);
            ctrlPhieuBanChiPhi = new PhieuBanChiPhiController();

            ctrlSanPham = new SanPhamController(new SanPhamFactory());
            ctrlKhachHang = new KhachHangController();
            ctrlMaSanPham = maSpCtrl;

            _status = Controll.Normal;
        }

        // ======= STUB giữ tương thích Designer cũ (nếu .Designer.cs còn gán frmNhapHang_Load) =======
        private void frmNhapHang_Load(object sender, EventArgs e) => frmBanSi_Load(sender, e);

        // ======= Form Load =======
        private void frmBanSi_Load(object sender, EventArgs e)
        {
            // Sản phẩm & cột mã sản phẩm (lô)
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            EnforceCombo(cmbSanPham, "ID", "TEN"); // sửa DisplayMember theo cột của bạn

            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged -= cmbSanPham_SelectedIndexChanged;
            cmbSanPham.SelectedIndexChanged += cmbSanPham_SelectedIndexChanged;

            // Khách hàng
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
            EnforceCombo(cmbKhachHang, "ID", "TEN");

            // Phiếu bán + binding
            ctrlPhieuBan.HienthiPhieuBan(
                bindingNavigator, cmbKhachHang, txtMaPhieu, dtNgayLapPhieu,
                numTongTien, numDaTra, numConNo);

            if (bindingNavigator?.BindingSource != null)
            {
                bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                bindingNavigator.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
            }

            if (_status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
                Allow(true);
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text); // grid rỗng theo mã mới
            }
            else
            {
                Allow(false);
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            }

            lb_PPTinhGia.Text =
            Settings.Default.PPTinhGia == CauHinhCuaHang.PhuongThucTinhGia.BQGQ.ToString()
                ? "Giá BQGQ"
                : "Giá FIFO";

            AppTheme.ApplyTheme(this);
        }

        private static void EnforceCombo(ComboBox cb, string valueMember, string displayMember)
        {
            try { cb.ValueMember = valueMember; cb.DisplayMember = displayMember; }
            catch { /* ignore nếu đã set sẵn */ }
        }

        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (_status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
                _dsChiPhiDaChon = ctrlPhieuBanChiPhi.LayDanhSachTheoPB(txtMaPhieu.Text).ToList();
                TinhTongTien(_dsChiPhiDaChon);
            }
        }

        // ======= Chọn SẢN PHẨM → nạp danh sách MÃ (lô) + tính giá gợi ý =======
        private void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue == null) return;

            var idSanPham = cmbSanPham.SelectedValue.ToString();

            cmbMaSanPham.SelectedIndexChanged -= cmbMaSanPham_SelectedIndexChanged;
            ctrlMaSanPham.HienThiAutoComboBox(idSanPham, cmbMaSanPham);
            EnforceCombo(cmbMaSanPham, "ID", "TEN"); // sửa DisplayMember nếu cần
            cmbMaSanPham.SelectedIndexChanged += cmbMaSanPham_SelectedIndexChanged;
            decimal giaXuat = (Properties.Settings.Default.PPTinhGia == CauHinhCuaHang.PhuongThucTinhGia.BQGQ.ToString())
                ? ctrlChiTiet.TinhGiaBinhQuanGiaQuyen(idSanPham)
                : ctrlChiTiet.TinhGiaFIFO(idSanPham);
            txtGiaBQGQ.Text = giaXuat.ToString("#,###0");
        }

        // ======= Chọn MÃ (lô) → hiển thị Giá nhập / sỉ / lẻ, set mặc định đơn giá = Giá sỉ =======
        private void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null) return;

            var idLo = cmbMaSanPham.SelectedValue.ToString();
            MaSanPham masp = ctrlMaSanPham.LayMaSanPham(idLo);
            if (masp == null || masp.SanPham == null) return;

            numDonGia.Value = masp.SanPham.GiaBanSi;  // Bán sỉ
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.SanPham.GiaBanLe.ToString("#,###0");
        }

        // ======= Add dòng =======
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!TryValidateBeforeAdd(out string err))
            {
                MessageBox.Show(err, "Phiếu Bán Sỉ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;

            if (dt == null)
            {
                // fallback: thêm qua controller + refresh binding
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

            TinhTongTien(_dsChiPhiDaChon);
        }

        // ======= Remove dòng =======
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var bs = dgvDanhsachSP.DataSource as BindingSource;
            if (bs == null || bs.Current == null) return;

            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Bán Sỉ",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var row = (DataRowView)bs.Current;
            _deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]),
                                       Convert.ToInt32(row["SO_LUONG"])));
            bs.RemoveCurrent();

            TinhTongTien(_dsChiPhiDaChon);
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Bán Sỉ",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                var bs = dgvDanhsachSP.DataSource as BindingSource;
                var row = (DataRowView)bs?.Current;
                if (row != null)
                {
                    _deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]),
                                               Convert.ToInt32(row["SO_LUONG"])));
                    TinhTongTien(_dsChiPhiDaChon);
                }
            }
        }

        // ======= Tự tính thành tiền =======
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

        // ======= Lưu =======
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
                MessageBox.Show(err, "Phiếu Bán Sỉ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_status == Controll.AddNew) ThemMoi();
            else CapNhat();
        }

        private void CapNhat()
        {
            foreach (MaSanPham masp in _deleted)
                _maSpDal.CapNhatSoLuong(masp.Id, masp.SoLuong);
            _deleted.Clear();

            ctrlChiTiet.Save();
            ctrlPhieuBan.Update();
            ctrlPhieuBanChiPhi.CapNhatChiPhiPhatSinh(txtMaPhieu.Text, _dsChiPhiDaChon);

            TinhTongTien(_dsChiPhiDaChon);
        }

        private void ThemMoi()
        {
            // Duplicate-check TRƯỚC khi Add
            var ctrlCheck = new PhieuBanController(new PhieuBanFactory(), new KhachHangController());
            if (ctrlCheck.LayPhieuBan(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu bán này đã tồn tại !", "Phiếu Bán Sỉ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuBan())
                    ThamSo.GanMaPhieuBan(so + 1);
            }

            ctrlPhieuBan.Save();
            ctrlChiTiet.Save();
            ctrlPhieuBanChiPhi.LuuChiPhiPhatSinh(txtMaPhieu.Text, _dsChiPhiDaChon);

            TinhTongTien(_dsChiPhiDaChon);
        }

        // ======= Lưu & tạo mới =======
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

        // ======= In phiếu =======
        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (_status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại!",
                                "Phiếu Bán Sỉ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ma_phieu = txtMaPhieu.Text;
            var ctrlPB = new PhieuBanController(new PhieuBanFactory(), new KhachHangController());
            var ph = ctrlPB.LayPhieuBan(ma_phieu);
            if (ph == null)
            {
                MessageBox.Show("Không tìm thấy phiếu để in.", "Phiếu Bán Sỉ",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frm = new frmInPhieuBan(ph);
            frm.Show();
        }

        // ======= Xoá phiếu =======
        private void toolXoa_Click(object sender, EventArgs e)
        {
            var bs = bindingNavigator?.BindingSource;
            var view = (DataRowView)bs?.Current;
            if (view == null) return;

            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phiếu Bán Sỉ",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // Hoàn kho toàn bộ chi tiết phiếu trước khi xóa
            var maSpCtrl = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
            var chiTiet = new ChiTietPhieuBanController(new ChiTietPhieuBanDAL(), maSpCtrl);
            IList<ChiTietPhieuBan> ds = chiTiet.ChiTietPhieuBan(view["ID"].ToString());

            foreach (var ct in ds)
                _maSpDal.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);

            bs.RemoveCurrent();
            ctrlPhieuBan.Save();
        }

        // ======= Misc =======
        private void toolXemLai_Click(object sender, EventArgs e)
        {
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
            EnforceCombo(cmbSanPham, "ID", "TEN");
            EnforceCombo(cmbKhachHang, "ID", "TEN");
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            using (var f = new frmDaiLy()) f.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
            EnforceCombo(cmbKhachHang, "ID", "TEN");
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            using (var f = new frmSanPham()) f.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            EnforceCombo(cmbSanPham, "ID", "TEN");
        }

        private void btnThemChiPhi_Click(object sender, EventArgs e)
        {

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
            e.Cancel = true;
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
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu bán này không?",
                                    "Phiếu Bán Sỉ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Luu();
                }
            }
            Close();
        }

        private void Allow(bool val)
        {
            // Tổng/ Còn nợ tính tự động
            txtMaPhieu.Enabled = val && _status == Controll.AddNew;
            dtNgayLapPhieu.Enabled = val;

            numTongTien.Enabled = false;
            numConNo.Enabled = false;

            numDaTra.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dgvDanhsachSP.Enabled = val;

            cmbSanPham.Enabled = val;
            cmbMaSanPham.Enabled = val;
            cmbKhachHang.Enabled = val;
        }

        // =================== VALIDATION & TÍNH TOÁN UI ===================

        private bool TryValidateBeforeAdd(out string error)
        {
            error = null;

            if (cmbSanPham.SelectedValue == null)
            { error = "Chưa chọn Sản phẩm."; return false; }

            if (cmbMaSanPham.SelectedValue == null)
            { error = "Chưa chọn Mã sản phẩm (lô)."; return false; }

            if (numSoLuong.Value <= 0)
            { error = "Số lượng phải > 0."; return false; }

            // Đồng bộ thành tiền
            var donGia = (decimal)numDonGia.Value;
            var soLuong = (int)numSoLuong.Value;
            var ttCalc = donGia * soLuong;
            if (numThanhTien.Value != ttCalc)
                numThanhTien.Value = ttCalc;

            // Kiểm tồn theo nghiệp vụ
            string idLo = cmbMaSanPham.SelectedValue.ToString();
            string idSanPham = (_chiTietDal as ChiTietPhieuBanDAL)?.LayIdSanPhamTuMaSanPham(idLo);
            if (string.IsNullOrWhiteSpace(idSanPham))
            { error = $"Không tìm thấy ID sản phẩm từ lô {idLo}."; return false; }

            var phuongPhap = CauHinhCuaHang.PhuongThucXuatKhoHienTai;
            if (phuongPhap == CauHinhCuaHang.PhuongThucXuatKho.ChonLo)
            {
                var lo = (_chiTietDal as ChiTietPhieuBanDAL)?.LayThongTinMotLo(idLo);
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
                var dsLo = (_chiTietDal as ChiTietPhieuBanDAL)?.LayDanhSachMaTheoSanPham(idSanPham);
                int tongTon = 0;
                if (dsLo != null) foreach (DataRow r in dsLo.Rows) tongTon += Convert.ToInt32(r["SO_LUONG"]);

                int daChonTrongGrid = TinhSoLuongDangChoTrongGridTheoSanPham(idSanPham);
                int tonKhaDung = tongTon - daChonTrongGrid;

                if (tonKhaDung < soLuong)
                { error = $"Sản phẩm {idSanPham} không đủ hàng. Cần {soLuong}, khả dụng {tonKhaDung}."; return false; }
            }

            return true;
        }

        private bool TryValidateBeforeSave(out string error)
        {
            error = null;

            if (cmbKhachHang.SelectedValue == null)
            { error = "Chưa chọn Khách hàng."; return false; }

            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;
            if (dt == null || !dt.Rows.Cast<DataRow>().Any(r => r.RowState != DataRowState.Deleted))
            { error = "Phiếu bán chưa có dòng hàng."; return false; }

            // Recalc tổng để chắc chắn
            TinhTongTien(_dsChiPhiDaChon);

            if (numTongTien.Value < 0)
            { error = "Tổng tiền không hợp lệ."; return false; }

            if (numDaTra.Value < 0 || numDaTra.Value > numTongTien.Value)
            { error = "Số tiền đã trả không hợp lệ."; return false; }

            return true;
        }

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

        private int TinhSoLuongDangChoTrongGridTheoSanPham(string idSanPham)
        {
            var bs = dgvDanhsachSP.DataSource as BindingSource;
            var dt = bs?.DataSource as DataTable;
            if (dt == null) return 0;

            int sum = 0;
            foreach (DataRow r in dt.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted))
            {
                var idLo = Convert.ToString(r["ID_MA_SAN_PHAM"]);
                var spOfLo = (_chiTietDal as ChiTietPhieuBanDAL)?.LayIdSanPhamTuMaSanPham(idLo);
                if (spOfLo == idSanPham) sum += Convert.ToInt32(r["SO_LUONG"]);
            }
            return sum;
        }

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
