using CuahangNongduoc.BLL.Helpers;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.Controller
{
    public class MaSanPhamController
    {
        private readonly IMaSanPhanFactory _dal;
        private readonly ISanPhamService _sanPhamService;

        // ✅ Inject DAL qua constructor
        public MaSanPhamController(IMaSanPhanFactory dal, ISanPhamFactory sanPhamDal)
            : this(dal, sanPhamDal, new SanPhamController(sanPhamDal))
        {
        }

        internal MaSanPhamController(IMaSanPhanFactory dal, ISanPhamFactory sanPhamDal, ISanPhamService sanPhamService)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _ = sanPhamDal ?? throw new ArgumentNullException(nameof(sanPhamDal));
            _sanPhamService = sanPhamService ?? throw new ArgumentNullException(nameof(sanPhamService));
        }

        /* ===================== CRUD ===================== */

        public DataRow NewRow() => _dal.NewRow();
        public void Add(DataRow row) => _dal.Add(row);
        public bool Save() => _dal.Save();

        /* ===================== DOMAIN LOGIC ===================== */

        public SanPham LaySanPham(string idMaSanPham)
        {
            DataTable tbl = _dal.LaySanPham(idMaSanPham);
            if (tbl.Rows.Count == 0) return null;

            var row = tbl.Rows[0];
            return new SanPham
            {
                Id = Convert.ToString(row["ID"]),
                TenSanPham = Convert.ToString(row["TEN_SAN_PHAM"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                DonGiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                GiaBanLe = Convert.ToInt64(row["GIA_BAN_LE"]),
                GiaBanSi = Convert.ToInt64(row["GIA_BAN_SI"])
            };
        }

        public MaSanPham LayMaSanPham(string idMaSanPham)
        {
            DataTable tbl = _dal.LayMaSanPham(idMaSanPham);
            if (tbl.Rows.Count == 0) return null;

            var row = tbl.Rows[0];
            return new MaSanPham
            {
                Id = Convert.ToString(row["ID"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                SanPham = _sanPhamService.GetById(row["ID_SAN_PHAM"].ToString())
            };
        }

        public IList<MaSanPham> LayMaSanPhamHetHan(DateTime dt)
        {
            var ds = new List<MaSanPham>();
            var tbl = _dal.DanhsachMaSanPhamHetHan(dt);
            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(new MaSanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                    NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                    NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                    SanPham = _sanPhamService.GetById(row["ID_SAN_PHAM"].ToString())
                });
            }

            return ds;
        }

        /* ===================== HIỂN THỊ ===================== */

        public void HienThiAutoComboBox(string sp, ComboBox cmb)
        {
            if(Properties.Settings.Default.PPXuatHang == CauHinhCuaHang.PhuongThucXuatKho.ChonLo.ToString())
            {
                cmb.DataSource = _dal.DanhsachMaSanPham(sp);
                cmb.DisplayMember = "ID";
                cmb.ValueMember = "ID";
            }
            else
            {
                cmb.DataSource = _dal.DanhsachMaSanPhamFIFO(sp);
                cmb.DisplayMember = "ID";
                cmb.ValueMember = "ID";
            }
        }

        public void HienThiDataGridViewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _dal.DanhsachMaSanPham();
            cmb.DisplayMember = "ID";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_MA_SAN_PHAM";
            cmb.HeaderText = "Mã sản phẩm";
        }

        public void HienThiChiTietPhieuNhap(string id, DataGridView dg)
        {
            dg.DataSource = _dal.DanhsachChiTiet(id);
        }

        public IList<MaSanPham> ChiTietPhieuNhap(string id)
        {
            IList<MaSanPham> ds = new List<MaSanPham>();
            DataTable tbl = _dal.DanhsachChiTiet(id);
            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(new MaSanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    ThanhTien = Convert.ToInt64(row["SO_LUONG"]) * Convert.ToInt64(row["DON_GIA_NHAP"]),
                    NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                    NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                    NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                    SanPham = _sanPhamService.GetById(row["ID_SAN_PHAM"].ToString())
                });
            }

            return ds;
        }

        public void CapNhatSoLuong(string maSP, int soLuong)
        {
            _dal.CapNhatSoLuong(maSP, soLuong);
        }
    }
}
