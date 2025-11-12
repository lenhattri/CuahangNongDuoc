using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.Infrastructure;
using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.Controller
{
    public class ChiTietPhieuNhapController
    {
        private readonly IChiTietPhieuNhapFactory _factory;

        // ✅ Inject DAL qua constructor
        public ChiTietPhieuNhapController(IChiTietPhieuNhapFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void ThemChiTietPhieuNhap(string idPhieuNhap, string idMaSP)
        {
            _factory.LoadSchema();
            DataRow row = _factory.NewRow();
            row["ID_PHIEU_NHAP"] = idPhieuNhap;
            row["ID_MA_SAN_PHAM"] = idMaSP;

            _factory.Add(row);
            _factory.Save();
        }

        public int XoaChiTietPhieuNhap(string idPhieuNhap)
        {
            return _factory.XoaChiTietPhieuNhap(idPhieuNhap);
        }

        public void HienThiChiTietPhieuNhap(string id, ListView lvw)
        {
            // Khởi tạo các controller với DI đúng
            var ctrlMSP = new MaSanPhamController(new MaSanPhanFactory(), new SanPhamFactory());
            var phieuNhapFactory = new PhieuNhapFactory(DbClient.Instance);
            var ctrlPN = new PhieuNhapController(phieuNhapFactory);

            // Lấy chi tiết phiếu nhập
            DataTable tbl = phieuNhapFactory.LayPhieuNhap(id);

            lvw.Items.Clear();
            foreach (DataRow row in tbl.Rows)
            {
                ChiTietPhieuNhap ct = new ChiTietPhieuNhap
                {
                    MaSanPham = ctrlMSP.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"])),
                    PhieuNhap = ctrlPN.LayPhieuNhap(Convert.ToString(row["ID_PHIEU_NHAP"]))
                };

                ListViewItem item = new ListViewItem((lvw.Items.Count + 1).ToString());
                item.SubItems.Add(ct.MaSanPham.SanPham.TenSanPham);
                item.SubItems.Add(ct.MaSanPham.Id);
                item.SubItems.Add(ct.MaSanPham.GiaNhap.ToString("#,###0"));
                item.SubItems.Add(ct.MaSanPham.SoLuong.ToString("#,###0"));

                // ⚠ Sửa cách tính thành tiền: thường là SoLuong * GiaNhap
                long thanhtien = ct.MaSanPham.SoLuong * ct.MaSanPham.GiaNhap;
                item.SubItems.Add(thanhtien.ToString("#,###0"));

                item.SubItems.Add(ct.MaSanPham.NgaySanXuat.ToString("dd/MM/yyyy"));
                item.SubItems.Add(ct.MaSanPham.NgayHetHan.ToString("dd/MM/yyyy"));

                item.Tag = ct;
                lvw.Items.Add(item);
            }
        }
    }
}
