using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.UI.HeThong;
using CuahangNongduoc.Utils;
using Microsoft.Win32;

namespace CuahangNongduoc
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        DonViTinhForm DonViTinh = null;

        private void mnuDonViTinh_Click(object sender, EventArgs e)
        {
            if (DonViTinh == null || DonViTinh.IsDisposed)
            {
                DonViTinh = new DonViTinhForm();
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

            DataService.OpenConnection();
            
        }
        SanPhamForm SanPham = null;
        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            if (SanPham == null || SanPham.IsDisposed)
            {
                SanPham = new SanPhamForm();
                SanPham.MdiParent = this;
                SanPham.Show();
            }
            else
                SanPham.Activate();
        }
        KhachHangForm KhachHang = null;
        private void mnuKhachHang_Click(object sender, EventArgs e)
        {
            if (KhachHang == null || KhachHang.IsDisposed)
            {
                KhachHang = new KhachHangForm();
                KhachHang.MdiParent = this;
                KhachHang.Show();
            }
            else
                KhachHang.Activate();
        }
        DaiLyForm DaiLy = null;
        private void mnuDaiLy_Click(object sender, EventArgs e)
        {
            if (DaiLy == null || DaiLy.IsDisposed)
            {
                DaiLy = new DaiLyForm();
                DaiLy.MdiParent = this;
                DaiLy.Show();
            }
            else
                DaiLy.Activate();

        }
        DanhsachPhieuNhapForm NhapHang = null;
        private void mnuNhapHang_Click(object sender, EventArgs e)
        {
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new DanhsachPhieuNhapForm();
                NhapHang.MdiParent = this;
                NhapHang.Show();
            }
            else
                NhapHang.Activate();
        }
        DanhsachPhieuBanLeForm BanLe = null;
        private void mnuBanHangKH_Click(object sender, EventArgs e)
        {
            if (BanLe == null || BanLe.IsDisposed)
            {
                BanLe = new DanhsachPhieuBanLeForm();
                BanLe.MdiParent = this;
                BanLe.Show();
            }
            else
                BanLe.Activate();
        }
        DanhsachPhieuBanSiForm BanSi = null;
        private void mnuBanHangDL_Click(object sender, EventArgs e)
        {
            if (BanSi == null || BanSi.IsDisposed)
            {
                BanSi = new DanhsachPhieuBanSiForm();
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
        ThanhToanForm ThanhToan = null;
        private void mnuThanhtoan_Click(object sender, EventArgs e)
        {
            if (ThanhToan == null || ThanhToan.IsDisposed)
            {
                ThanhToan = new ThanhToanForm();
                ThanhToan.MdiParent = this;
                ThanhToan.Show();
            }
            else
                ThanhToan.Activate();
        }
        DunoKhachhangForm DunoKhachhang = null;
        private void mnuTonghopDuno_Click(object sender, EventArgs e)
        {
            if (DunoKhachhang == null || DunoKhachhang.IsDisposed)
            {
                DunoKhachhang = new DunoKhachhangForm();
                DunoKhachhang.MdiParent = this;
                DunoKhachhang.Show();
            }
            else
                DunoKhachhang.Activate();
        }
        DoanhThuForm DoanhThu = null;
        private void mnuBaocaoDoanhThu_Click(object sender, EventArgs e)
        {
            if (DoanhThu == null || DoanhThu.IsDisposed)
            {
                DoanhThu = new DoanhThuForm();
                DoanhThu.MdiParent = this;
                DoanhThu.Show();
            }
            else
                DoanhThu.Activate();

        }

        SoLuongTonForm SoLuongTon = null;
        private void mnuBaocaoSoluongton_Click(object sender, EventArgs e)
        {

            if (SoLuongTon == null || SoLuongTon.IsDisposed)
            {
                SoLuongTon = new SoLuongTonForm();
                SoLuongTon.MdiParent = this;
                SoLuongTon.Show();
            }
            else
                SoLuongTon.Activate();

        }
        SoLuongBanForm SoLuongBan = null;
        private void mnuSoLuongBan_Click(object sender, EventArgs e)
        {
            if (SoLuongBan == null || SoLuongBan.IsDisposed)
            {
                SoLuongBan = new SoLuongBanForm();
                SoLuongBan.MdiParent = this;
                SoLuongBan.Show();
            }
            else
                SoLuongBan.Activate();
        }
        SanphamHethanForm SanphamHethan = null;
        private void mnuSanphamHethan_Click(object sender, EventArgs e)
        {
            if (SanphamHethan == null || SanphamHethan.IsDisposed)
            {
                SanphamHethan = new SanphamHethanForm();
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
        ThongtinCuahangForm ThongtinCuahang = null;
        private void mnuTuychinhThongtin_Click(object sender, EventArgs e)
        {

            if (ThongtinCuahang == null || ThongtinCuahang.IsDisposed)
            {
                ThongtinCuahang = new ThongtinCuahangForm();
                ThongtinCuahang.MdiParent = this;
                ThongtinCuahang.Show();
            }
            else
                ThongtinCuahang.Activate();
        }
        ThongtinLienheForm ThongtinLienhe = null;
        private void mnuTrogiupLienhe_Click(object sender, EventArgs e)
        {
            if (ThongtinLienhe == null || ThongtinLienhe.IsDisposed)
            {
                ThongtinLienhe = new ThongtinLienheForm();
                ThongtinLienhe.MdiParent = this;
                ThongtinLienhe.Show();
            }
            else
                ThongtinLienhe.Activate();
        }

        NhaCungCapForm NhaCungCap = null;
        private void mnuNhaCungCap_Click(object sender, EventArgs e)
        {
            if (NhaCungCap == null || NhaCungCap.IsDisposed)
            {
                NhaCungCap = new NhaCungCapForm();
                NhaCungCap.MdiParent = this;
                NhaCungCap.Show();
            }
            else
                NhaCungCap.Activate();
        }
        LyDoChiForm LyDoChi = null;
        private void mnuLyDoChi_Click(object sender, EventArgs e)
        {
            if (LyDoChi == null || LyDoChi.IsDisposed)
            {
                LyDoChi = new LyDoChiForm();
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

        DangNhapForm DangNhap = null;
        private void mnuDangNhap_Click(object sender, EventArgs e)
        {
            if (DangNhap == null || DangNhap.IsDisposed)
            {
                DangNhap = new DangNhapForm();
                //DangNhap.MdiParent = this;
                if( DangNhap.ShowDialog() == DialogResult.OK )
                {
                    toolstlb_StatusLogin.Text = "Người dùng: " + Session.CurrentUser.HoTen + " - Quyền: " + Session.CurrentUser.Quyen;
                }
            }
            else
                DangNhap.Activate();
        }

        private void mnuDangXuat_Click(object sender, EventArgs e)
        {
            Session.CurrentUser = null;
        }

        NguoiDungForm NguoiDung = null;
        private void mnuNguoiDung_Click(object sender, EventArgs e)
        {
            if (NguoiDung == null || NguoiDung.IsDisposed)
            {
                NguoiDung = new NguoiDungForm();
                NguoiDung.MdiParent = this;
                NguoiDung.Show();
            }
            else
                NguoiDung.Activate();
        }
    }
}