using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DAL.Infrastructure;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmNhapHang : Form
    {
        // ====== Đổi cho khớp Designer của bạn ======
        private const string COL_MA_SP_GRID = "colSanPham";      // cột chọn Sản phẩm (Value: ID_SP)
        private const string COL_DON_GIA_GRID = "colDonGiaNhap";   // cột Đơn giá nhập
        private const string COL_SO_LUONG_GRID = "colSoLuong";      // cột Số lượng
        private const string COL_MA_SO_GRID = "colMaSanPham";    // cột Mã số (nếu có)
        private const string COL_NSX_GRID = "colNgaySanXuat";  // nếu có
        private const string COL_HH_GRID = "colNgayHetHan";   // nếu có
        private const string COL_THANH_TIEN_GRID = "colThanhTien";  // nếu lưới có cột Thành tiền
        // ===== Factory =====
        private readonly IPhieuNhapFactory _phieuNhapFactory;
        private readonly IMaSanPhanFactory _maSanPhamFactory;
        private readonly ISanPhamFactory _sanPhamFactory;
        private readonly INhaCungCapDAL _nhaCungCapDAL;

        // ===== Controller =====
        private readonly PhieuNhapController _ctrlPhieuNhap;
        private readonly MaSanPhamController _ctrlMaSP;
        private readonly SanPhamController _ctrlSanPham;
        private readonly NhaCungCapController _ctrlNCC;

        //SanPhamController ctrlSanPham = new SanPhamController(new SanPhamFactory());
        //IPhieuNhapFactory factories = new PhieuNhapFactory(DbClient.Instance);
        //PhieuNhapController ctrl = new PhieuNhapController(new PhieuNhapFactory(DbClient.Instance));

        //MaSanPhamController ctrlMaSP = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
        //NhaCungCapController ctrlNCC = new NhaCungCapController(new NhaCungCapDAL());
        //PhieuNhap m_PhieuNhap = null;

        Controll status = Controll.Normal;

        private readonly ErrorProvider errorProvider1 = new ErrorProvider();
        private bool _isSubmitting = false;

        private string _maPhieuNhap = null;

        public frmNhapHang()
        {
            InitializeComponent();
            // ===== Khởi tạo Factory =====
            _phieuNhapFactory = new PhieuNhapFactory(DbClient.Instance);
            _maSanPhamFactory = new MaSanPhanFactory();
            _sanPhamFactory = new SanPhamFactory();
            _nhaCungCapDAL = new NhaCungCapDAL();

            // ===== Khởi tạo Controller =====
            _ctrlPhieuNhap = new PhieuNhapController(_phieuNhapFactory);
            _ctrlMaSP = new MaSanPhamController(_maSanPhamFactory, _sanPhamFactory);
            _ctrlSanPham = new SanPhamController(_sanPhamFactory);
            _ctrlNCC = new NhaCungCapController(_nhaCungCapDAL);
            status = Controll.AddNew;

            // Khởi gán ErrorProvider
            errorProvider1.ContainerControl = this;

            // ===== Khóa & tính realtime cho numThanhTien (ô Thành tiền trên form) =====
            numThanhTien.Enabled = false;           // user không được nhập tay
            numThanhTien.ReadOnly = true;           // tăng tính chắc chắn

            // Gắn validate control bắt buộc
            txtMaPhieu.Validating += (s, e) => ValidateTextRequired(txtMaPhieu, e, "Mã phiếu không được để trống.");
            cmbNhaCungCap.Validating += (s, e) => ValidateComboRequired(cmbNhaCungCap, e, "Phải chọn Nhà cung cấp.");
            dtNgayNhap.Validating += (s, e) => ValidateDateRequired(dtNgayNhap, e, "Phải chọn Ngày nhập.");

            // DataGridView validate
            dataGridView.CellValidating += dataGridView_CellValidating;
            dataGridView.RowValidating += dataGridView_RowValidating;
            dataGridView.DataError += dataGridView_DataError;
            dataGridView.UserDeletingRow += dataGridView_UserDeletingRow;

            // ===== Tính Thành tiền (form) realtime =====
            numGiaNhap.ValueChanged += QuickLine_Recalc;
            numSoLuong.ValueChanged += QuickLine_Recalc;
            numGiaNhap.KeyUp += (s, e) => QuickLine_Recalc(s, e);
            numSoLuong.KeyUp += (s, e) => QuickLine_Recalc(s, e);

            // ===== Hook lưới để tự tính tổng =====
            dataGridView.CellValueChanged += dataGridView_CellValueChanged_Recalc;
            dataGridView.RowsAdded += (s, e) => RecalcTotalsFromGrid();
            dataGridView.RowsRemoved += (s, e) => RecalcTotalsFromGrid();
            dataGridView.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dataGridView.IsCurrentCellDirty)
                    dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
        }

        public frmNhapHang(PhieuNhapController ctrlPN) : this()
        {
            _ctrlPhieuNhap = ctrlPN ?? new PhieuNhapController(_phieuNhapFactory);
            status = Controll.Normal;
        }

        public frmNhapHang(PhieuNhapController ctrlPN, string MaPhieuNhap) : this(ctrlPN)
        {
            _maPhieuNhap = MaPhieuNhap;
            // status đã được set là Normal bởi constructor bên trên (this(ctrlPN))
        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status != Controll.Normal && status != Controll.Edit)
            {
                return;
            }

            string currentMaPhieu = null;

            BindingSource bs = sender as BindingSource;
            if (bs != null && bs.Current != null)
            {
                DataRowView drv = bs.Current as DataRowView;
                if (drv != null)
                {
                    currentMaPhieu = drv["ID"]?.ToString();
                }
            }

            if (string.IsNullOrEmpty(currentMaPhieu))
            {
                currentMaPhieu = txtMaPhieu.Text;
            }

            if (!string.IsNullOrWhiteSpace(currentMaPhieu))
            {
                _ctrlMaSP.HienThiChiTietPhieuNhap(currentMaPhieu, dataGridView);
            }
            else
            {
                _ctrlMaSP.HienThiChiTietPhieuNhap(null, dataGridView);
            }

            // Luôn tính lại tổng sau khi lưới thay đổi
            RecalcTotalsFromGrid();
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            _ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            _ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
            _ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);

            // Tải dữ liệu header
            _ctrlPhieuNhap.HienthiPhieuNhap(bindingNavigator, txtMaPhieu, cmbNhaCungCap, dtNgayNhap, numTongTien, numDaTra, numConNo);
            // Gắn event
            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);

            // Cột Thành tiền trên lưới (nếu có) → chỉ đọc
            if (dataGridView.Columns.Contains(COL_THANH_TIEN_GRID))
            {
                dataGridView.Columns[COL_THANH_TIEN_GRID].ReadOnly = true;
                dataGridView.Columns[COL_THANH_TIEN_GRID].DefaultCellStyle.Format = "N0";
            }

            if (status == Controll.AddNew)
            {
                // --- CHẾ ĐỘ THÊM MỚI ---
                txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
                Allow(true);
                // reset số liệu
                numTongTien.Value = 0;
                numDaTra.Value = 0;
                numConNo.Value = 0;
                // Tải lưới chi tiết rỗng
                _ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
            }
            else
            {
                // --- CHẾ ĐỘ XEM (NORMAL) ---
                Allow(false);

                // Nếu có mã phiếu được truyền vào, tìm và nhảy tới đó
                if (!string.IsNullOrEmpty(_maPhieuNhap))
                {
                    int index = bindingNavigator.BindingSource.Find("ID", _maPhieuNhap);
                    if (index != -1)
                    {
                        bindingNavigator.BindingSource.Position = index;
                    }
                    _maPhieuNhap = null; // Xóa đi để không dùng lại
                }

                // *** THAY ĐỔI QUAN TRỌNG ***
                // Xóa dòng này đi:
                // ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);

                // Thay bằng cách: Kích hoạt sự kiện CurrentChanged bằng tay
                // để tải lưới cho vị trí ĐÚNG (dù là 0 hay vị trí vừa nhảy tới).
                // Hàm này giờ đã an toàn (ở Bước 1).
                BindingSource_CurrentChanged(bindingNavigator.BindingSource, EventArgs.Empty);
            }

            numDaTra.ValueChanged += numDaTra_ValueChanged;

            AppTheme.ApplyTheme(this);
        }

        // ===== Recalc ô Thành tiền ở panel nhập nhanh =====
        private void QuickLine_Recalc(object sender, EventArgs e)
        {
            numThanhTien.Value = numGiaNhap.Value * numSoLuong.Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validate nhập nhanh dòng chi tiết (panel bên phải)
            if (cmbSanPham.SelectedValue == null || string.IsNullOrWhiteSpace(Convert.ToString(cmbSanPham.SelectedValue)))
            {
                MessageBox.Show("Vui lòng chọn Sản phẩm!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MaSanPhamController ctrlTemp = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
            MaSanPham masp = ctrlTemp.LayMaSanPham(txtMaSo.Text.Trim());
            if (masp == null)
            {
                foreach (DataGridViewRow view in dataGridView.Rows)
                {
                    if (view.IsNewRow) continue;
                    var existMaSo = Convert.ToString(view.Cells[COL_MA_SO_GRID]?.Value);
                    if (!string.IsNullOrEmpty(existMaSo) && txtMaSo.Text.Trim().Equals(existMaSo))
                    {
                        MessageBox.Show("Mã sản phẩm này đã tồn tại trong danh sách! Vui lòng nhập lại!", "Phiếu Nhập",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (txtMaSo.Text.Trim().Length <= 0)
                {
                    MessageBox.Show("Vui lòng nhập Mã sản phẩm!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (numGiaNhap.Value < 0)
                {
                    MessageBox.Show("Đơn giá phải >= 0!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (numSoLuong.Value <= 0)
                {
                    MessageBox.Show("Số lượng phải > 0!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (dtNgaySanXuat.Value >= dtNgayHetHan.Value)
                {
                    MessageBox.Show("Ngày hết hạn phải lớn hơn ngày sản xuất!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Không cộng tay vào numTongTien nữa
                    numThanhTien.Value = numGiaNhap.Value * numSoLuong.Value;

                    DataRow row = _ctrlMaSP.NewRow();
                    row["ID_SAN_PHAM"] = cmbSanPham.SelectedValue;
                    row["ID_PHIEU_NHAP"] = txtMaPhieu.Text;
                    row["ID"] = txtMaSo.Text.Trim();
                    row["DON_GIA_NHAP"] = numGiaNhap.Value;
                    row["SO_LUONG"] = numSoLuong.Value;
                    row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
                    row["NGAY_SAN_XUAT"] = dtNgaySanXuat.Value.Date;
                    row["NGAY_HET_HAN"] = dtNgayHetHan.Value.Date;
                    _ctrlMaSP.Add(row);

                    // Sau khi Add → tính lại tổng từ lưới
                    RecalcTotalsFromGrid();
                }
            }
            else
            {
                MessageBox.Show("Mã sản phẩm này đã tồn tại! Vui lòng nhập lại!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== Realtime tính thành tiền từng dòng & tổng từ lưới ==========
        private void dataGridView_CellValueChanged_Recalc(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            // Lấy số lượng & đơn giá
            decimal soLuong = ToDecimalSafe(row.Cells[COL_SO_LUONG_GRID]?.Value);
            decimal donGia = ToDecimalSafe(row.Cells[COL_DON_GIA_GRID]?.Value);

            // Gán Thành tiền của dòng (nếu có cột)
            if (dataGridView.Columns.Contains(COL_THANH_TIEN_GRID))
            {
                row.Cells[COL_THANH_TIEN_GRID].Value = donGia * soLuong;
            }

            // Tính lại tổng
            RecalcTotalsFromGrid();
        }

        private void RecalcTotalsFromGrid()
        {
            decimal sum = 0;

            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;

                decimal sl = ToDecimalSafe(r.Cells[COL_SO_LUONG_GRID]?.Value);
                decimal dg = ToDecimalSafe(r.Cells[COL_DON_GIA_GRID]?.Value);
                sum += (dg * sl);
            }

            numTongTien.Value = sum;
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        private void numGiaNhap_ValueChanged(object sender, EventArgs e)
        {
            // (đã chuyển sang QuickLine_Recalc)
            numThanhTien.Value = numGiaNhap.Value * numSoLuong.Value;
        }

        private void toolLuuThoat_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();

            if (!ValidateBeforeSave()) return;

            this.Luu();
            status = Controll.Normal;
            this.Allow(false);
        }

        private bool ValidateBeforeSave()
        {
            _isSubmitting = true;

            // Kích hoạt Validate của toàn form
            if (!this.ValidateChildren(ValidationConstraints.Enabled))
            {
                MessageBox.Show("Vui lòng điền đầy đủ dữ liệu bắt buộc.", "Thiếu dữ liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _isSubmitting = false;
                return false;
            }

            // Kiểm tra DataGridView có ít nhất 1 dòng hợp lệ
            if (!HasAtLeastOneValidDetailRow())
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất 1 dòng chi tiết hợp lệ.", "Thiếu chi tiết",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _isSubmitting = false;
                return false;
            }

            // Kiểm tra hạn dùng từng dòng nếu lưới có cột ngày
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;
                DateTime? nsx = GetCellDate(r, COL_NSX_GRID);
                DateTime? hhs = GetCellDate(r, COL_HH_GRID);
                if (nsx.HasValue && hhs.HasValue && nsx.Value >= hhs.Value)
                {
                    r.ErrorText = "Ngày hết hạn phải lớn hơn ngày sản xuất.";
                    MessageBox.Show("Có dòng chi tiết có ngày hết hạn không hợp lệ.", "Lỗi dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _isSubmitting = false;
                    return false;
                }
            }

            _isSubmitting = false;
            return true;
        }

        void Luu()
        {
            if (status == Controll.AddNew)
                ThemMoi();
            else
                CapNhat();
        }

        void CapNhat()
        {
            // Lưu chi tiết
            _ctrlMaSP.Save();
            // Cập nhật header
            _ctrlPhieuNhap.Update();
        }

        void ThemMoi()
        {
            // Kiểm tra trùng mã phiếu trước khi Add
            PhieuNhapController ctrlPNCheck = new PhieuNhapController(_phieuNhapFactory);
            if (ctrlPNCheck.LayPhieuNhap(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu nhập này đã tồn tại!", "Phiếu Nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow row = _ctrlPhieuNhap.NewRow();
            row["ID"] = txtMaPhieu.Text.Trim();
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["ID_NHA_CUNG_CAP"] = cmbNhaCungCap.SelectedValue;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            _ctrlPhieuNhap.Add(row);

            // Tự tăng mã phiếu nếu là số
            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuNhap())
                {
                    ThamSo.GanMaPhieuNhap(so + 1);
                }
            }

            // Lưu header + chi tiết
            _ctrlPhieuNhap.Save();
            _ctrlMaSP.Save();

            // Cập nhật giá nhập mặc định sản phẩm
            SanPhamController ctrlSP = new SanPhamController(new SanPhamFactory());
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;

                var sp = Convert.ToString(view.Cells[COL_MA_SP_GRID].Value);
                long dg = ToInt64Safe(view.Cells[COL_DON_GIA_GRID].Value);
                long sl = ToInt64Safe(view.Cells[COL_SO_LUONG_GRID].Value);
                if (!string.IsNullOrWhiteSpace(sp))
                {
                    ctrlSP.CapNhatGiaNhap(sp, dg, sl);
                }
            }
        }

        private void toolLuuThem_Click(object sender, EventArgs e)
        {
            // tạo factory với DbClient
            IPhieuNhapFactory factory = new PhieuNhapFactory(DbClient.Instance);

            // tạo controller với factory
            PhieuNhapController ctrlPN = new PhieuNhapController(factory);
            status = Controll.AddNew;

            txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;

            // Refresh chi tiết rỗng cho mã mới
            _ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
            this.Allow(true);

            // Sau refresh → tính tổng
            RecalcTotalsFromGrid();
        }

        private void toolSavePrint_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu nhập hiện tại!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                String ma_phieu = txtMaPhieu.Text;
                // tạo factory với DbClient
                IPhieuNhapFactory factory = new PhieuNhapFactory(DbClient.Instance);

                // tạo controller với factory
                PhieuNhapController ctrlPN = new PhieuNhapController(factory);
                PhieuNhapController ctrl = new PhieuNhapController(factory);
                CuahangNongduoc.BusinessObject.PhieuNhap ph = ctrl.LayPhieuNhap(ma_phieu);
                frmInPhieuNhap PhieuNhap = new frmInPhieuNhap(ph);
                PhieuNhap.Show();
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu nhập này không?", "Phiếu Nhập Hàng",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!ValidateBeforeSave()) { /* không lưu */ }
                    else this.Luu();
                }
            }
            this.Close();
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            _ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
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
            numTongTien.Enabled = false; // tổng tiền do chi tiết quyết định
            numDaTra.Enabled = val;
            numConNo.Enabled = false;    // tự tính
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
            if (MessageBox.Show("Bạn chắc chắn xóa dòng chi tiết này không?", "Phiếu Nhập",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Xóa dòng đang chọn trên lưới (KHÔNG trừ numTongTien thủ công)
                foreach (DataGridViewRow r in dataGridView.SelectedRows)
                {
                    if (!r.IsNewRow)
                    {
                        dataGridView.Rows.Remove(r);
                    }
                }

                // Sau xóa → RowsRemoved đã gọi RecalcTotalsFromGrid(); 
                // gọi thêm lần nữa cho chắc:
                RecalcTotalsFromGrid();
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa dòng chi tiết này không?", "Phiếu Nhập",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            // KHÔNG trừ numTongTien thủ công ở đây nữa (RowsRemoved sẽ tự tổng)
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa phiếu này không?", "Phiếu Nhập",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                _ctrlPhieuNhap.Save();
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
            _ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);
        }

        // =========================
        // Validate helpers
        // =========================
        private void SetError(Control c, string msg)
        {
            errorProvider1.SetError(c, msg ?? string.Empty);
        }

        private void ValidateTextRequired(TextBox txt, CancelEventArgs e, string msg)
        {
            if (_isSubmitting && string.IsNullOrWhiteSpace(txt.Text))
            {
                SetError(txt, msg);
                e.Cancel = true;
            }
            else
            {
                SetError(txt, null);
            }
        }

        private void ValidateComboRequired(ComboBox cbo, CancelEventArgs e, string msg)
        {
            if (_isSubmitting && (cbo.SelectedValue == null || string.IsNullOrWhiteSpace(cbo.SelectedValue.ToString())))
            {
                SetError(cbo, msg);
                e.Cancel = true;
            }
            else
            {
                SetError(cbo, null);
            }
        }

        private void ValidateDateRequired(DateTimePicker dtp, CancelEventArgs e, string msg)
        {
            // DateTimePicker luôn có value; nhánh này chỉ để đồng nhất flow
            if (_isSubmitting && dtp.Value == default(DateTime))
            {
                SetError(dtp, msg);
                e.Cancel = true;
            }
            else
            {
                SetError(dtp, null);
            }
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!_isSubmitting) return;
            if (e.RowIndex < 0) return;

            var grid = dataGridView;
            var row = grid.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            var colName = grid.Columns[e.ColumnIndex].Name;
            string val = e.FormattedValue?.ToString();

            if (colName == COL_MA_SP_GRID)
            {
                if (string.IsNullOrWhiteSpace(val))
                {
                    row.ErrorText = "Sản phẩm không được để trống.";
                    e.Cancel = true;
                }
                else row.ErrorText = string.Empty;
            }

            if (colName == COL_SO_LUONG_GRID)
            {
                if (!decimal.TryParse(val, out var sl) || sl <= 0)
                {
                    row.ErrorText = "Số lượng phải > 0.";
                    e.Cancel = true;
                }
                else row.ErrorText = string.Empty;
            }

            if (colName == COL_DON_GIA_GRID)
            {
                if (!decimal.TryParse(val, out var dg) || dg < 0)
                {
                    row.ErrorText = "Đơn giá phải >= 0.";
                    e.Cancel = true;
                }
                else row.ErrorText = string.Empty;
            }
        }

        private void dataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!_isSubmitting) return;
            if (e.RowIndex < 0) return;

            var r = dataGridView.Rows[e.RowIndex];
            if (r.IsNewRow) return;

            string maSP = Convert.ToString(r.Cells[COL_MA_SP_GRID]?.Value);
            decimal soLuong = ToDecimalSafe(r.Cells[COL_SO_LUONG_GRID]?.Value);
            decimal donGia = ToDecimalSafe(r.Cells[COL_DON_GIA_GRID]?.Value);

            if (string.IsNullOrWhiteSpace(maSP))
            {
                r.ErrorText = "Sản phẩm không được để trống.";
                e.Cancel = true; return;
            }
            if (soLuong <= 0)
            {
                r.ErrorText = "Số lượng phải > 0.";
                e.Cancel = true; return;
            }
            if (donGia < 0)
            {
                r.ErrorText = "Đơn giá phải >= 0.";
                e.Cancel = true; return;
            }

            // Nếu có cột ngày trên lưới
            DateTime? nsx = GetCellDate(r, COL_NSX_GRID);
            DateTime? hhs = GetCellDate(r, COL_HH_GRID);
            if (nsx.HasValue && hhs.HasValue && nsx.Value >= hhs.Value)
            {
                r.ErrorText = "Ngày hết hạn phải lớn hơn ngày sản xuất.";
                e.Cancel = true; return;
            }

            r.ErrorText = string.Empty;
        }

        private bool HasAtLeastOneValidDetailRow()
        {
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;

                string maSP = Convert.ToString(r.Cells[COL_MA_SP_GRID]?.Value);
                decimal soLuong = ToDecimalSafe(r.Cells[COL_SO_LUONG_GRID]?.Value);
                decimal donGia = ToDecimalSafe(r.Cells[COL_DON_GIA_GRID]?.Value);

                if (!string.IsNullOrWhiteSpace(maSP) && soLuong > 0 && donGia >= 0)
                    return true;
            }
            return false;
        }

        private static long ToInt64Safe(object v)
        {
            if (v == null) return 0;
            long.TryParse(Convert.ToString(v), out var r);
            return r;
        }

        private static decimal ToDecimalSafe(object v)
        {
            if (v == null) return 0;
            decimal.TryParse(Convert.ToString(v), out var r);
            return r;
        }

        private static DateTime? GetCellDate(DataGridViewRow r, string colName)
        {
            if (string.IsNullOrEmpty(colName)) return null;
            if (!r.DataGridView.Columns.Contains(colName)) return null;
            var val = r.Cells[colName]?.Value;
            if (val == null) return null;
            if (val is DateTime dt) return dt;
            if (DateTime.TryParse(Convert.ToString(val), out var d)) return d;
            return null;
        }
    }
}
