using CuahangNongduoc.Domain.Entities;
using CuahangNongduoc.UI.BaoCaoThongKe;
using CuahangNongduoc.UI.HeThong;
using CuahangNongduoc.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            SetMdiBackground();
        }

        private void SetMdiBackground()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MdiClient client)
                {
                    try
                    {
                        string imagePath = Path.Combine(Application.StartupPath, "../../images", "background.jpg");

                        if (File.Exists(imagePath))
                        {
                            // 🖼️ Set ảnh nền
                            client.BackgroundImage = Image.FromFile(imagePath);

                            // 💡 FIX QUAN TRỌNG: Stretch = căng toàn khung, không bị lặp
                            client.BackgroundImageLayout = ImageLayout.Center;

                            // 🎨 Màu nền phụ nhẹ nếu ảnh nhỏ hơn form
                            client.BackColor = Color.FromArgb(235, 255, 235);
                        }
                        else
                        {
                            // fallback nếu ảnh không tồn tại
                            client.BackColor = Color.FromArgb(235, 255, 235);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải ảnh nền: " + ex.Message,
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }




        frmDonViTinh DonViTinh = null;
        private void mnuDonViTinh_Click(object sender, EventArgs e)
        {
            if (DonViTinh == null || DonViTinh.IsDisposed)
            {
                DonViTinh = new frmDonViTinh();
                DonViTinh.MdiParent = this;
                DonViTinh.Show();
                
            }
            else
                DonViTinh.Activate();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\CoolSoft\\CuahangNongduoc");

            //if (regKey == null)
            //{
            //    DataService.m_ConnectString = "";
            //}
            //else
            //{
            //    try
            //    {
            //        DataService.m_ConnectString = (String)regKey.GetValue("ConnectString");
            //    }
            //    catch
            //    {
            //    }
            //    finally
            //    {
            //        regKey.Close();
            //    }
            //}

            //if (DataService.OpenConnection() == false)
            //{
            //    MessageBox.Show("Không thể kết nối dữ liệu!", "Cua hang Nong duoc", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    this.Close();
            //}

            ChuaDangNhap();

            frmDangNhap frmDangNhap = new frmDangNhap();
            //DangNhap.MdiParent = this;
            if (frmDangNhap.ShowDialog() == DialogResult.OK)
            {
                PhanQuyen();
            }


            DataService.OpenConnection();
            
        }
        frmSanPham SanPham = null;
        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            if (SanPham == null || SanPham.IsDisposed)
            {
                SanPham = new frmSanPham();
                SanPham.MdiParent = this;
                SanPham.Show();
            }
            else
                SanPham.Activate();
        }
        frmKhachHang KhachHang = null;
        private void mnuKhachHang_Click(object sender, EventArgs e)
        {
            if (KhachHang == null || KhachHang.IsDisposed)
            {
                KhachHang = new frmKhachHang();
                KhachHang.MdiParent = this;
                KhachHang.Show();
            }
            else
                KhachHang.Activate();
        }
        frmDaiLy DaiLy = null;
        private void mnuDaiLy_Click(object sender, EventArgs e)
        {
            if (DaiLy == null || DaiLy.IsDisposed)
            {
                DaiLy = new frmDaiLy();
                DaiLy.MdiParent = this;
                DaiLy.Show();
            }
            else
                DaiLy.Activate();

        }
        frmDanhsachPhieuNhap NhapHang = null;
        private void mnuNhapHang_Click(object sender, EventArgs e)
        {
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new frmDanhsachPhieuNhap();
                NhapHang.MdiParent = this;
                NhapHang.Show();
            }
            else
                NhapHang.Activate();
        }
        frmDanhsachPhieuBanLe BanLe = null;
        private void mnuBanHangKH_Click(object sender, EventArgs e)
        {
            if (BanLe == null || BanLe.IsDisposed)
            {
                BanLe = new frmDanhsachPhieuBanLe();
                BanLe.MdiParent = this;
                BanLe.Show();
            }
            else
                BanLe.Activate();
        }
        frmDanhsachPhieuBanSi BanSi = null;
        private void mnuBanHangDL_Click(object sender, EventArgs e)
        {
            if (BanSi == null || BanSi.IsDisposed)
            {
                BanSi = new frmDanhsachPhieuBanSi();
                BanSi.MdiParent = this;
                BanSi.Show();
            }
            else
                BanSi.Activate();
        }

        private void mnuThanhCongCu_Click(object sender, EventArgs e)
        {
            mnuThanhCongCu.Checked = !mnuThanhCongCu.Checked;
            toolStrip.Visible = mnuThanhCongCu.Checked;
        }

        private void mnuThanhChucNang_Click(object sender, EventArgs e)
        {
            mnuThanhChucNang.Checked = !mnuThanhChucNang.Checked;
            taskPane.Visible = mnuThanhChucNang.Checked;
        }
        frmThanhToan ThanhToan = null;
        private void mnuThanhtoan_Click(object sender, EventArgs e)
        {
            if (ThanhToan == null || ThanhToan.IsDisposed)
            {
                ThanhToan = new frmThanhToan();
                ThanhToan.MdiParent = this;
                ThanhToan.Show();
            }
            else
                ThanhToan.Activate();
        }
        frmDunoKhachhang DunoKhachhang = null;
        private void mnuTonghopDuno_Click(object sender, EventArgs e)
        {
            if (DunoKhachhang == null || DunoKhachhang.IsDisposed)
            {
                DunoKhachhang = new frmDunoKhachhang();
                DunoKhachhang.MdiParent = this;
                DunoKhachhang.Show();
            }
            else
                DunoKhachhang.Activate();
        }
        frmDoanhThu DoanhThu = null;
        private void mnuBaocaoDoanhThu_Click(object sender, EventArgs e)
        {
            if (DoanhThu == null || DoanhThu.IsDisposed)
            {
                DoanhThu = new frmDoanhThu();
                DoanhThu.MdiParent = this;
                DoanhThu.Show();
            }
            else
                DoanhThu.Activate();

        }

        frmSoLuongTon SoLuongTon = null;
        private void mnuBaocaoSoluongton_Click(object sender, EventArgs e)
        {

            if (SoLuongTon == null || SoLuongTon.IsDisposed)
            {
                SoLuongTon = new frmSoLuongTon();
                SoLuongTon.MdiParent = this;
                SoLuongTon.Show();
            }
            else
                SoLuongTon.Activate();

        }

        frmChiPhiVaKhuyenMai ChiPhiVaKhuyenMai = null;
        private void mnuBaocaoChiphiKhuyenmai_Click(object sender, EventArgs e)
        {
            if (ChiPhiVaKhuyenMai == null || ChiPhiVaKhuyenMai.IsDisposed)
            {
                ChiPhiVaKhuyenMai = new frmChiPhiVaKhuyenMai();
                ChiPhiVaKhuyenMai.MdiParent = this;
                ChiPhiVaKhuyenMai.Show();
            }
            else
                ChiPhiVaKhuyenMai.Activate();
        }

        frmSoLuongBan SoLuongBan = null;
        private void mnuSoLuongBan_Click(object sender, EventArgs e)
        {
            if (SoLuongBan == null || SoLuongBan.IsDisposed)
            {
                SoLuongBan = new frmSoLuongBan();
                SoLuongBan.MdiParent = this;
                SoLuongBan.Show();
            }
            else
                SoLuongBan.Activate();
        }
        frmSanphamHethan SanphamHethan = null;
        private void mnuSanphamHethan_Click(object sender, EventArgs e)
        {
            if (SanphamHethan == null || SanphamHethan.IsDisposed)
            {
                SanphamHethan = new frmSanphamHethan();
                SanphamHethan.MdiParent = this;
                SanphamHethan.Show();
            }
            else
                SanphamHethan.Activate();
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        frmThongtinCuahang ThongtinCuahang = null;
        private void mnuTuychinhThongtin_Click(object sender, EventArgs e)
        {

            if (ThongtinCuahang == null || ThongtinCuahang.IsDisposed)
            {
                ThongtinCuahang = new frmThongtinCuahang();
                ThongtinCuahang.MdiParent = this;
                ThongtinCuahang.Show();
            }
            else
                ThongtinCuahang.Activate();
        }
        frmThongtinLienhe ThongtinLienhe = null;
        private void mnuTrogiupLienhe_Click(object sender, EventArgs e)
        {
            if (ThongtinLienhe == null || ThongtinLienhe.IsDisposed)
            {
                ThongtinLienhe = new frmThongtinLienhe();
                ThongtinLienhe.MdiParent = this;
                ThongtinLienhe.Show();
            }
            else
                ThongtinLienhe.Activate();
        }

        frmNhaCungCap NhaCungCap = null;
        private void mnuNhaCungCap_Click(object sender, EventArgs e)
        {
            if (NhaCungCap == null || NhaCungCap.IsDisposed)
            {
                NhaCungCap = new frmNhaCungCap();
                NhaCungCap.MdiParent = this;
                NhaCungCap.Show();
            }
            else
                NhaCungCap.Activate();
        }
        frmLyDoChi LyDoChi = null;
        private void mnuLyDoChi_Click(object sender, EventArgs e)
        {
            if (LyDoChi == null || LyDoChi.IsDisposed)
            {
                LyDoChi = new frmLyDoChi();
                LyDoChi.MdiParent = this;
                LyDoChi.Show();
            }
            else
                LyDoChi.Activate();
        }

        frmPhieuChi PhieuChi = null;
        private void mnuPhieuChi_Click(object sender, EventArgs e)
        {
            if (PhieuChi == null || PhieuChi.IsDisposed)
            {
                PhieuChi = new frmPhieuChi();
                PhieuChi.MdiParent = this;
                PhieuChi.Show();
            }
            else
                PhieuChi.Activate();
        }

        private void mnuTrogiupHuongdan_Click(object sender, EventArgs e)
        {
           // Help.ShowHelp(this, "CPP.CHM");
        }

        frmDangNhap DangNhap = null;
        private void mnuDangNhap_Click(object sender, EventArgs e)
        {
            if (DangNhap == null || DangNhap.IsDisposed)
            {
                DangNhap = new frmDangNhap();
                //DangNhap.MdiParent = this;
                if( DangNhap.ShowDialog() == DialogResult.OK )
                {
                    PhanQuyen();
                }
            }
            else
                DangNhap.Activate();
        }

        private void mnuDangXuat_Click(object sender, EventArgs e)
        {
            Session.CurrentUser = null;
            ChuaDangNhap();
        }

        frmNguoiDung NguoiDung = null;
        private void mnuNguoiDung_Click(object sender, EventArgs e)
        {
            if (NguoiDung == null || NguoiDung.IsDisposed)
            {
                NguoiDung = new frmNguoiDung();
                NguoiDung.MdiParent = this;
                NguoiDung.Show();
            }
            else
                NguoiDung.Activate();
        }
        frmCauHinh CauHinh = null;
        private void mnuTuyChinhCauHinh_Click(object sender, EventArgs e)
        {
            if (CauHinh != null || CauHinh.IsDisposed)
            {
                CauHinh = new frmCauHinh();
                CauHinh.MdiParent = this;
                CauHinh.Show();
            }
            else
            {
                CauHinh.Activate();
            }
        }

        private void ChuaDangNhap()
        {
            mnuDangNhap.Enabled = true;
            mnuBanHang.Enabled = false;
            mnuDangXuat.Enabled = false;
            mnuBaocao.Enabled = false;
            toolStrip.Enabled = false;
            taskPane.Enabled = false;
            mnuQuanLy.Enabled = false;
            mnuNghiepVu.Enabled = false;
        }

        private void QuyenAdmin()
        {
            mnuDangNhap.Enabled = false;
            mnuBanHang.Enabled = true;
            mnuDangXuat.Enabled = true;
            mnuBaocao.Enabled = true;
            toolStrip.Enabled = true;
            taskPane.Enabled = true;
            mnuQuanLy.Enabled = true;
            mnuNghiepVu.Enabled = true;
        }

        private void QuyenNhanVien()
        {
            mnuDangNhap.Enabled = false;
            mnuBanHang.Enabled = true;
            toolStrip.Enabled = true;
            taskPane.Enabled = true;
            mnuDangXuat.Enabled = true;
        }

        private void PhanQuyen()
        {
            if (Session.CurrentUser.Quyen == "Admin")
            {
                QuyenAdmin();
            }
            else if (Session.CurrentUser.Quyen == "NhanVien")
            {
                QuyenNhanVien();
            }
            toolstlb_StatusLogin.Text = "Người dùng: " + Session.CurrentUser.HoTen + " - Quyền: " + Session.CurrentUser.Quyen;
        }
    }
}