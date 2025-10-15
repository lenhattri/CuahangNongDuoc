// Path: BLL/CONTROLLER/ChiTietPhieuNhapController.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class ChiTietPhieuNhapController
    {
        private readonly ChiTietPhieuNhapFactory _dal = new ChiTietPhieuNhapFactory();

        // buffer cho các dòng Added (n?u b?n v?n mu?n pattern Save() m?t l??t)
        private readonly DataTable _buffer = CreateBufferSchema();

        public void ThemChiTietPhieuNhap(string idPhieuNhap, string idMaSP, int soLuong, decimal giaNhap)
        {
            var row = _buffer.NewRow();
            row["ID_PHIEU_NHAP"] = idPhieuNhap;
            row["ID_MA_SAN_PHAM"] = idMaSP;
            row["SO_LUONG"] = soLuong;
            row["GIA_NHAP"] = giaNhap;
            row["THANH_TIEN"] = soLuong * giaNhap; // ? ?úng công th?c
            _buffer.Rows.Add(row);

          
            _dal.SaveAddedRows(_buffer);
            _buffer.Clear();
        }

        public int XoaChiTietPhieuNhap(string idPhieuNhap)
            => _dal.XoaChiTietPhieuNhap(idPhieuNhap);

        public void HienThiChiTietPhieuNhap(string idPhieuNhap, ListView lvw)
        {
            var ctrlMSP = new MaSanPhamController();
            var ctrlPN = new PhieuNhapController();

            DataTable tbl = _dal.LayChiTietPhieuNhap(idPhieuNhap);

            lvw.Items.Clear();
            foreach (DataRow row in tbl.Rows)
            {
                var ct = new ChiTietPhieuNhap
                {
                    MaSanPham = ctrlMSP.LayMaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"])),
                    PhieuNhap = ctrlPN.LayPhieuNhap(Convert.ToString(row["ID_PHIEU_NHAP"])),
                    SoLuong = row.IsNull("SO_LUONG") ? 0 : Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = row.IsNull("GIA_NHAP") ? 0m : Convert.ToDecimal(row["GIA_NHAP"]),
                    ThanhTien = row.IsNull("THANH_TIEN")
                                ? (row.IsNull("SO_LUONG") || row.IsNull("GIA_NHAP")
                                    ? 0m
                                    : Convert.ToInt32(row["SO_LUONG"]) * Convert.ToDecimal(row["GIA_NHAP"]))
                                : Convert.ToDecimal(row["THANH_TIEN"])
                };

                var item = new ListViewItem((lvw.Items.Count + 1).ToString());
                item.SubItems.Add(ct.MaSanPham.SanPham.TenSanPham);
                item.SubItems.Add(ct.MaSanPham.Id);
                item.SubItems.Add(ct.GiaNhap.ToString("#,###0"));      // ??n giá nh?p
                item.SubItems.Add(ct.SoLuong.ToString("#,###0"));
                item.SubItems.Add(ct.ThanhTien.ToString("#,###0"));    // ? soLuong * giaNhap
                item.SubItems.Add(ct.MaSanPham.NgaySanXuat.ToString("dd/MM/yyyy"));
                item.SubItems.Add(ct.MaSanPham.NgayHetHan.ToString("dd/MM/yyyy"));

                item.Tag = ct;
                lvw.Items.Add(item);
            }
        }

        private static DataTable CreateBufferSchema()
        {
            var t = new DataTable("CHI_TIET_PHIEU_NHAP");
            t.Columns.Add("ID_PHIEU_NHAP", typeof(string));
            t.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            t.Columns.Add("SO_LUONG", typeof(int));
            t.Columns.Add("GIA_NHAP", typeof(decimal));
            t.Columns.Add("THANH_TIEN", typeof(decimal));
            return t;
        }
    }
}
