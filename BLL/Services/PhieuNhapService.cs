using System;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class PhieuNhapService : IPhieuNhapService
    {
        private readonly IPhieuNhapFactory _factory;
        private readonly INhaCungCapService _nhaCungCapService;
        private readonly IMaSanPhamService _maSanPhamService;

        public PhieuNhapService(
            IPhieuNhapFactory factory,
            INhaCungCapService nhaCungCapService,
            IMaSanPhamService maSanPhamService)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _nhaCungCapService = nhaCungCapService ?? throw new ArgumentNullException(nameof(nhaCungCapService));
            _maSanPhamService = maSanPhamService ?? throw new ArgumentNullException(nameof(maSanPhamService));
        }

        public DataTable DanhSachPhieuNhap() => _factory.DanhsachPhieuNhap();

        public DataRow TaoDongMoi() => _factory.NewRow();

        public void Them(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            _factory.Add(row);
        }

        public bool Luu() => _factory.Save();

        public DataTable TimPhieuNhap(string maNhaCungCap, DateTime ngayNhap)
            => _factory.TimPhieuNhap(maNhaCungCap, ngayNhap);

        public PhieuNhap LayPhieuNhap(string id)
        {
            var table = _factory.LayPhieuNhap(id);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            var phieuNhap = new PhieuNhap
            {
                Id = Convert.ToString(row["ID"]),
                NgayNhap = row["NGAY_NHAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAY_NHAP"]),
                TongTien = Convert.ToInt64(row["TONG_TIEN"]),
                DaTra = Convert.ToInt64(row["DA_TRA"]),
                ConNo = Convert.ToInt64(row["CON_NO"]),
                NhaCungCap = _nhaCungCapService.LayNhaCungCap(Convert.ToString(row["ID_NHA_CUNG_CAP"]))
            };

            phieuNhap.ChiTiet = _maSanPhamService.GetReceiptDetails(phieuNhap.Id);
            return phieuNhap;
        }
    }
}
