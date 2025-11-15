using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class ProductLotService : IProductLotService
    {
        private readonly IMaSanPhanFactory _factory;
        private readonly IProductService _productService;

        public ProductLotService(IMaSanPhanFactory factory, IProductService productService)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public DataTable GetProductLots(string productId = null)
        {
            return string.IsNullOrEmpty(productId)
                ? _factory.DanhsachMaSanPham()
                : _factory.DanhsachMaSanPham(productId);
        }

        public DataTable GetProductLotDetails(string receiptId)
        {
            return _factory.DanhsachChiTiet(receiptId);
        }

        public SanPham GetProductForLot(string lotId)
        {
            var table = _factory.LaySanPham(lotId);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
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

        public MaSanPham GetProductLot(string lotId)
        {
            var table = _factory.LayMaSanPham(lotId);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            return new MaSanPham
            {
                Id = Convert.ToString(row["ID"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                SanPham = _productService.GetProduct(Convert.ToString(row["ID_SAN_PHAM"]))
            };
        }

        public IList<MaSanPham> GetExpiredLots(DateTime referenceDate)
        {
            var table = _factory.DanhsachMaSanPhamHetHan(referenceDate);
            var result = new List<MaSanPham>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new MaSanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                    NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                    NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                    SanPham = _productService.GetProduct(Convert.ToString(row["ID_SAN_PHAM"]))
                });
            }

            return result;
        }

        public IList<MaSanPham> GetReceiptDetails(string receiptId)
        {
            var table = _factory.DanhsachChiTiet(receiptId);
            var result = new List<MaSanPham>();
            foreach (DataRow row in table.Rows)
            {
                var lot = new MaSanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    GiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                    NgaySanXuat = Convert.ToDateTime(row["NGAY_SAN_XUAT"]),
                    NgayHetHan = Convert.ToDateTime(row["NGAY_HET_HAN"]),
                    SanPham = _productService.GetProduct(Convert.ToString(row["ID_SAN_PHAM"]))
                };
                lot.ThanhTien = lot.GiaNhap * lot.SoLuong;
                result.Add(lot);
            }

            return result;
        }

        public void UpdateQuantity(string lotId, int deltaQuantity)
        {
            _factory.CapNhatSoLuong(lotId, deltaQuantity);
        }

        public DataRow CreateRow() => _factory.NewRow();

        public void Add(DataRow row) => _factory.Add(row);

        public bool SaveChanges() => _factory.Save();
    }
}
