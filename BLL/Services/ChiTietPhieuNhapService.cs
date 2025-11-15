using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class ChiTietPhieuNhapService : IChiTietPhieuNhapService
    {
        private readonly IChiTietPhieuNhapFactory _factory;
        private readonly IMaSanPhamService _maSanPhamService;
        private readonly IPhieuNhapService _phieuNhapService;

        public ChiTietPhieuNhapService(
            IChiTietPhieuNhapFactory factory,
            IMaSanPhamService maSanPhamService,
            IPhieuNhapService phieuNhapService)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _maSanPhamService = maSanPhamService ?? throw new ArgumentNullException(nameof(maSanPhamService));
            _phieuNhapService = phieuNhapService ?? throw new ArgumentNullException(nameof(phieuNhapService));
        }

        public void ThemChiTiet(string idPhieuNhap, string idMaSanPham)
        {
            if (string.IsNullOrWhiteSpace(idPhieuNhap))
            {
                throw new ArgumentException("Mã phiếu nhập không hợp lệ", nameof(idPhieuNhap));
            }

            if (string.IsNullOrWhiteSpace(idMaSanPham))
            {
                throw new ArgumentException("Mã sản phẩm không hợp lệ", nameof(idMaSanPham));
            }

            _factory.LoadSchema();
            var row = _factory.NewRow();
            row["ID_PHIEU_NHAP"] = idPhieuNhap;
            row["ID_MA_SAN_PHAM"] = idMaSanPham;
            _factory.Add(row);
            _factory.Save();
        }

        public int XoaChiTiet(string idPhieuNhap)
        {
            if (string.IsNullOrWhiteSpace(idPhieuNhap))
            {
                throw new ArgumentException("Mã phiếu nhập không hợp lệ", nameof(idPhieuNhap));
            }

            return _factory.XoaChiTietPhieuNhap(idPhieuNhap);
        }

        public IList<ChiTietPhieuNhap> LayChiTiet(string idPhieuNhap)
        {
            var table = _factory.LayChiTietPhieuNhap(idPhieuNhap);
            var result = new List<ChiTietPhieuNhap>();
            if (table == null)
            {
                return result;
            }

            foreach (DataRow row in table.Rows)
            {
                var chiTiet = new ChiTietPhieuNhap
                {
                    MaSanPham = _maSanPhamService.GetProductLot(Convert.ToString(row["ID_MA_SAN_PHAM"])),
                    PhieuNhap = _phieuNhapService.LayPhieuNhap(Convert.ToString(row["ID_PHIEU_NHAP"]))
                };
                result.Add(chiTiet);
            }

            return result;
        }

        public DataTable LayBangChiTiet(string idPhieuNhap) => _factory.LayChiTietPhieuNhap(idPhieuNhap);
    }
}
