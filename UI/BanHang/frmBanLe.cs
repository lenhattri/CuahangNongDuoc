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
        private SanPhamController ctrlSanPham;
        private KhachHangController ctrlKhachHang;
        private MaSanPhamController ctrlMaSanPham;
        private PhieuBanController ctrlPhieuBan;
        private ChiTietPhieuBanController ctrlChiTiet;
        private PhieuBanChiPhiController ctrlPhieuBanChiPhi;
        private IChiTietPhieuBanDAL chiTietDal;
        private MaSanPhamController maSpCtrl;

        private IList<MaSanPham> deleted = new List<MaSanPham>();
        private List<ChiPhiPhatSinh> _dsChiPhiDaChon = new List<ChiPhiPhatSinh>();
        private Controll status = Controll.Normal;

        // ✅ Constructor mặc định
        public frmBanLe()
        {
            InitializeComponent();

            // Khởi tạo dependencies (DI thủ công)
            ctrlSanPham = new SanPhamController();
            ctrlKhachHang = new KhachHangController();
            ctrlMaSanPham = new MaSanPhamController();

            maSpCtrl = new MaSanPhamController();
            chiTietDal = new ChiTietPhieuBanDAL();
            ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, maSpCtrl);

            ctrlPhieuBan = new PhieuBanController(new PhieuBanFactory(), ctrlKhachHang);
            ctrlPhieuBanChiPhi = new PhieuBanChiPhiController();

            status = Controll.AddNew;
        }

        // ✅ Constructor inject từ ngoài (ví dụ mở form từ nơi khác)
        public frmBanLe(PhieuBanController ctrlPB)
        {
            InitializeComponent();

            ctrlSanPham = new SanPhamController();
            ctrlKhachHang = new KhachHangController();
            ctrlMaSanPham = new MaSanPhamController();

            ctrlPhieuBan = ctrlPB;

            maSpCtrl = new MaSanPhamController();
            chiTietDal = new ChiTietPhieuBanDAL();
            ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, maSpCtrl);

            ctrlPhieuBanChiPhi = new PhieuBanChiPhiController();

            status = Controll.Normal;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);

            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            }
            else
            {
                this.Allow(false);
            }

            AppTheme.ApplyTheme(this);
        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
                _dsChiPhiDaChon = ctrlPhieuBanChiPhi.LayDanhSachTheoPB(txtMaPhieu.Text).ToList();
                TinhTongTien(_dsChiPhiDaChon);
            }
        }

        void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue != null)
            {
                MaSanPhamController ctrlMSP = new MaSanPhamController();
                cmbMaSanPham.SelectedIndexChanged -= new EventHandler(cmbMaSanPham_SelectedIndexChanged);
                ctrlMSP.HienThiAutoComboBox(cmbSanPham.SelectedValue.ToString(), cmbMaSanPham);
                cmbMaSanPham.SelectedIndexChanged += new EventHandler(cmbMaSanPham_SelectedIndexChanged);

                decimal giaXuat = (CauHinhCuaHang.PhuongThucTinhGiaHienTai == CauHinhCuaHang.PhuongThucTinhGia.BQGQ)
                    ? ctrlChiTiet.TinhGiaBinhQuanGiaQuyen(cmbSanPham.SelectedValue.ToString())
                    : ctrlChiTiet.TinhGiaFIFO(cmbSanPham.SelectedValue.ToString());

                txtGiaBQGQ.Text = giaXuat.ToString("#,###0");
            }
        }

        void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null) return;

            MaSanPhamController ctrl = new MaSanPhamController();
            MaSanPham masp = ctrl.LayMaSanPham(cmbMaSanPham.SelectedValue.ToString());

            numDonGia.Value = masp.SanPham.GiaBanLe;
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.SanPham.GiaBanLe.ToString("#,###0");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã sản phẩm !", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập Số lượng !", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numDonGia.Value * numSoLuong.Value != numThanhTien.Value)
            {
                MessageBox.Show("Thành tiền sai!", "Phiếu Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            numTongTien.Value += numThanhTien.Value;
            DataRow row = ctrlChiTiet.NewRow();
            row["ID_MA_SAN_PHAM"] = cmbMaSanPham.SelectedValue;
            row["ID_PHIEU_BAN"] = txtMaPhieu.Text;
            row["DON_GIA"] = numDonGia.Value;
            row["SO_LUONG"] = numSoLuong.Value;
            row["THANH_TIEN"] = numThanhTien.Value;
            ctrlChiTiet.Add(row);
            
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
            MaSanPhanFactory factory = new MaSanPhanFactory();
            foreach (MaSanPham masp in deleted)
            {
                factory.CapNhatSoLuong(masp.Id, masp.SoLuong);
            }
            deleted.Clear();

            ctrlChiTiet.Save();

            ctrlPhieuBan.Update();

            ctrlPhieuBanChiPhi.CapNhatChiPhiPhatSinh(txtMaPhieu.Text, _dsChiPhiDaChon);
            

        }
        void ThemMoi()
        {

            DataRow row = ctrlPhieuBan.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["ID_NHAN_VIEN"] = Session.CurrentUser.Id;
            row["ID_KHACH_HANG"] = cmbKhachHang.SelectedValue;
            row["NGAY_BAN"] = dtNgayLapPhieu.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            ctrlPhieuBan.Add(row);

            PhieuBanController ctrl = new PhieuBanController(
                new PhieuBanFactory(),
                new KhachHangController()
                );

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

            ctrlPhieuBanChiPhi.LuuChiPhiPhatSinh(txtMaPhieu.Text, _dsChiPhiDaChon);

        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            ctrlPhieuBan = new PhieuBanController(
                new PhieuBanFactory(),
                new KhachHangController()
                );
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
                deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]), Convert.ToInt32(row["SO_LUONG"])));
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
                deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]), Convert.ToInt32(row["SO_LUONG"])));

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

                PhieuBanController ctrlPB = new PhieuBanController(
                    new PhieuBanFactory(),
                    new KhachHangController()
                    );

                CuahangNongduoc.BusinessObject.PhieuBan ph = ctrlPB.LayPhieuBan(ma_phieu);

                frmInPhieuBan InPhieuBan = new frmInPhieuBan(ph);

                InPhieuBan.Show();

            }
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }

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
            DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
            if (view != null)
            {
                if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    IChiTietPhieuBanDAL chiTietDal = new ChiTietPhieuBanDAL();
                    MaSanPhamController maSpCtrl = new MaSanPhamController();

                    ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController(chiTietDal, maSpCtrl);
                    IList<ChiTietPhieuBan> ds = ctrlChiTiet.ChiTietPhieuBan(view["ID"].ToString());
                    MaSanPhanFactory factory = new MaSanPhanFactory();
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        factory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
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
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }

        private void btnThemChiPhi_Click(object sender, EventArgs e)
        {
            if (status == Controll.AddNew)
            {
                MessageBox.Show("Vui lòng lưu phiếu bán trước khi chọn chi phí!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (frmPhieuBanChiPhi ChiPhi = new frmPhieuBanChiPhi(txtMaPhieu.Text,_dsChiPhiDaChon))
            {
                if(ChiPhi.ShowDialog() == DialogResult.OK)
                {
                   _dsChiPhiDaChon = ChiPhi.LayDanhSachChiPhiDaChon();
                    TinhTongTien(_dsChiPhiDaChon);
                }
            }

        }
        private void TinhTongTien(IList<ChiPhiPhatSinh> chiPhis)
        {
            long tongChiPhi = 0;
            foreach (var chiPhi in chiPhis)
            {
                tongChiPhi += (long)chiPhi.SoTien;
            }
            
            decimal tongHang = ctrlChiTiet.TinhTongTienBanTheoPhieuBan(txtMaPhieu.Text);
            numTongTien.Value = tongChiPhi + tongHang;
        }
    }
}
