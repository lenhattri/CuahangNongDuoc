using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Controller
{
    public class MaSanPhamController
    {
        private readonly IMaSanPhamFactory factory;

        public MaSanPhamController()
        {
            factory = new MaSanPhamFactory(); // Inject DAL
        }

        /* ===================== CRUD ===================== */
        public DataRow NewRow() => factory.NewRow();

        public void Add(DataRow row) => factory.Add(row);

        public bool Save() => factory.Save();

        /* ===================== DOMAIN OPERATIONS ===================== */
        public SanPham LaySanPham(string idMaSanPham)
        {
            var tbl = factory.LaySanPham(idMaSanPham);
            if (tbl.Rows.Count == 0) return null;

            var row = tbl.Rows[0];
            var sp = new SanPham
            {
                Id = Convert.ToString(row["ID"]),
                TenSanPham = Convert.ToString(row["TEN_SAN_PHAM"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                DonGiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                GiaBanLe = Convert.ToInt64(row["GIA_BAN_LE"]),
                GiaBanSi = Convert.ToInt64(row["GIA_BAN_SI"]),
                DonViTinh = new DonViTinhController().LayDVT(Convert.ToInt32(row["ID_DON_VI_TINH"]))
            };
            return sp;
        }

        public MaSanPham LayMaSanPham(string idMaSanPham)
        {
            var tbl = factory.LayMaSanPham(idMaSanPham);
            if (tbl.Rows.Count == 0) return null;

            var row = tbl.Rows[0];
            var sp = new MaSanPham
            {
                Id = Convert.ToString(row["ID"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                SanPham = new SanPhamController().LaySanPham(Convert.ToString(row["ID_SAN_PHAM"]))
            };
            return sp;
        }

        public IList<MaSanPham> LayMaSanPhamHetHan(DateTime dt)
        {
            var ds = new List<MaSanPham>();
            var tbl = factory.DanhsachMaSanPhamHetHan(dt);
            var ctrlSanPham = new SanPhamController();

            foreach (DataRow row in tbl.Rows)
            {
                var sp = new MaSanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                    NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                    NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                    SanPham = ctrlSanPham.LaySanPham(Convert.ToString(row["ID_SAN_PHAM"]))
                };
                ds.Add(sp);
            }
            return ds;
        }

        /* ===================== UI BINDING ===================== */
        public void HienThiAutoComboBox(string sp, ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachMaSanPham(sp);
            cmb.DisplayMember = "ID";
            cmb.ValueMember = "ID";
        }

        public void HienThiDataGridViewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = factory.DanhsachMaSanPham();
            cmb.DisplayMember = "ID";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_MA_SAN_PHAM";
            cmb.HeaderText = "Mã sản phẩm";
        }

        public void HienThiChiTietPhieuNhap(string id, DataGridView dg)
        {
            dg.DataSource = factory.DanhsachChiTiet(id);
        }

        public IList<MaSanPham> ChiTietPhieuNhap(string id)
        {
            var ds = new List<MaSanPham>();
            var tbl = factory.DanhsachChiTiet(id);
            var ctrlSanPham = new SanPhamController();

            foreach (DataRow row in tbl.Rows)
            {
                var sp = new MaSanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                    NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                    NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                    SanPham = ctrlSanPham.LaySanPham(Convert.ToString(row["ID_SAN_PHAM"])),
                    ThanhTien = Convert.ToInt32(row["SO_LUONG"]) * Convert.ToInt64(row["DON_GIA_NHAP"])
                };
                ds.Add(sp);
            }
            return ds;
        }
    }
}
