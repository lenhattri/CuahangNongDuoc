using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangFactory _dal;

        public KhachHangService(IKhachHangFactory dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DataTable DanhSachKhachHang(bool? laDaiLy = null)
        {
            return laDaiLy.HasValue
                ? _dal.DanhsachKhachHang(laDaiLy.Value)
                : _dal.DanhsachKhachHang();
        }

        public DataTable TimTheoHoTen(string hoTen, bool laDaiLy)
            => _dal.TimHoTen(hoTen ?? string.Empty, laDaiLy);

        public DataTable TimTheoDiaChi(string diaChi, bool laDaiLy)
            => _dal.TimDiaChi(diaChi ?? string.Empty, laDaiLy);

        public KhachHang LayKhachHang(string id)
        {
            var table = _dal.LayKhachHang(id);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            return new KhachHang
            {
                Id = Convert.ToString(row["ID"]),
                HoTen = Convert.ToString(row["HO_TEN"]),
                DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                DiaChi = Convert.ToString(row["DIA_CHI"]),
                LoaiKH = row.Table.Columns.Contains("LOAI_KH") && Convert.ToBoolean(row["LOAI_KH"])
            };
        }

        public IList<KhachHang> LayTatCa()
        {
            var table = _dal.DanhsachKhachHang();
            var result = new List<KhachHang>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new KhachHang
                {
                    Id = Convert.ToString(row["ID"]),
                    HoTen = Convert.ToString(row["HO_TEN"]),
                    DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                    DiaChi = Convert.ToString(row["DIA_CHI"]),
                    LoaiKH = row.Table.Columns.Contains("LOAI_KH") && Convert.ToBoolean(row["LOAI_KH"])
                });
            }

            return result;
        }

        public DataRow TaoDongMoi() => _dal.NewRow();

        public void Them(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            _dal.Add(row);
        }

        public bool Luu() => _dal.Save();
    }
}
