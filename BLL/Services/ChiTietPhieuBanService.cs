using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class ChiTietPhieuBanService : IChiTietPhieuBanService
    {
        private readonly IChiTietPhieuBanDAL _dal;
        private readonly IMaSanPhamService _maSanPhamService;
        private readonly DataTable _buffer;

        public ChiTietPhieuBanService(IChiTietPhieuBanDAL dal, IMaSanPhamService maSanPhamService)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _maSanPhamService = maSanPhamService ?? throw new ArgumentNullException(nameof(maSanPhamService));
            _buffer = TaoBangBoDem();
        }

        public DataTable LayChiTietPhieuBan(string idPhieuBan) => _dal.LayChiTietPhieuBan(idPhieuBan);

        public DataTable LayChiTietPhieuBan(DateTime ngayBan) => _dal.LayChiTietPhieuBan(ngayBan);

        public DataTable LayChiTietPhieuBan(int thang, int nam) => _dal.LayChiTietPhieuBan(thang, nam);

        public DataRow TaoDongMoi() => _buffer.NewRow();

        public void ThemVaoBoDem(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (row.Table != _buffer)
            {
                var clone = _buffer.NewRow();
                foreach (DataColumn column in _buffer.Columns)
                {
                    if (row.Table.Columns.Contains(column.ColumnName))
                    {
                        clone[column.ColumnName] = row[column.ColumnName];
                    }
                }

                _buffer.Rows.Add(clone);
            }
            else
            {
                _buffer.Rows.Add(row);
            }
        }

        public bool LuuBoDem()
        {
            var result = _dal.SaveAddedRows(_buffer);
            if (result)
            {
                _buffer.Clear();
                _buffer.AcceptChanges();
            }

            return result;
        }

        public IList<ChiTietPhieuBan> LayDanhSachTheoPhieu(string idPhieuBan)
            => MapToList(_dal.LayChiTietPhieuBan(idPhieuBan));

        public IList<ChiTietPhieuBan> LayDanhSachTheoNgay(DateTime ngayBan)
            => MapToList(_dal.LayChiTietPhieuBan(ngayBan));

        public IList<ChiTietPhieuBan> LayDanhSachTheoThoiGian(int thang, int nam)
            => MapToList(_dal.LayChiTietPhieuBan(thang, nam));

        public IList<ChiTietPhieuBan> LayTatCaChiTiet()
            => MapToList(_dal.LayTatCaChiTietPhieuBan());

        public decimal TinhTongTienTheoPhieu(string maPhieuBan)
            => _dal.TinhTongThanhTienTheoPhieu(maPhieuBan);

        public decimal TinhGiaBinhQuanGiaQuyen(string idSanPham)
            => _dal.TinhGiaBinhQuanGiaQuyen(idSanPham);

        public decimal TinhGiaFIFO(string idSanPham)
            => _dal.TinhGiaFIFO(idSanPham);

        private IList<ChiTietPhieuBan> MapToList(DataTable table)
        {
            var result = new List<ChiTietPhieuBan>();
            if (table == null)
            {
                return result;
            }

            foreach (DataRow row in table.Rows)
            {
                var chiTiet = new ChiTietPhieuBan
                {
                    DonGia = Convert.ToInt64(row["DON_GIA"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    ThanhTien = Convert.ToInt64(row["THANH_TIEN"]),
                    MaSanPham = _maSanPhamService.GetProductLot(Convert.ToString(row["ID_MA_SAN_PHAM"]))
                };

                result.Add(chiTiet);
            }

            return result;
        }

        private static DataTable TaoBangBoDem()
        {
            var table = new DataTable("CHI_TIET_PHIEU_BAN");
            table.Columns.Add("ID_PHIEU_BAN", typeof(string));
            table.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            table.Columns.Add("SO_LUONG", typeof(int));
            table.Columns.Add("DON_GIA", typeof(long));
            table.Columns.Add("THANH_TIEN", typeof(long));
            return table;
        }
    }
}
