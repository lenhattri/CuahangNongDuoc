using CuahangNongduoc.BLL.Helpers;
using CuahangNongduoc.Utils.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuahangNongduoc.UI.HeThong
{
    public partial class frmCauHinh: Form
    {
        private string Url = ConfigurationManager.AppSettings["Url"].ToString();

        public frmCauHinh()
        {
            InitializeComponent();
        }

        private void frmCauHinh_Load(object sender, EventArgs e)
        {
            if(CauHinhCuaHang.PhuongThucXuatKhoHienTai == CauHinhCuaHang.PhuongThucXuatKho.FIFO)
            {
                rdbFIFO.Checked = true;
            }
            else
            {
                rdbChonLo.Checked = true;
            }
            
            if(CauHinhCuaHang.PhuongThucTinhGiaHienTai == CauHinhCuaHang.PhuongThucTinhGia.BQGQ)
            {
                rdbBQGQ.Checked = true;
            }
            else
            {
                rdbGiaFIFO.Checked = true;
            }
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.F1))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string url = Url + "/tuy-chinh/cau-hinh";
                IFU_Helper.IFU(url);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if(rdbFIFO.Checked)
            {
                CauHinhCuaHang.PhuongThucXuatKhoHienTai = CauHinhCuaHang.PhuongThucXuatKho.FIFO;
            }
            else
            {
                CauHinhCuaHang.PhuongThucXuatKhoHienTai = CauHinhCuaHang.PhuongThucXuatKho.ChonLo;
            }

            if(rdbBQGQ.Checked)
            {
                CauHinhCuaHang.PhuongThucTinhGiaHienTai = CauHinhCuaHang.PhuongThucTinhGia.BQGQ;
            }
            else
            {
                CauHinhCuaHang.PhuongThucTinhGiaHienTai = CauHinhCuaHang.PhuongThucTinhGia.FIFO;
            }
            MessageBox.Show("Lưu cấu hình thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
