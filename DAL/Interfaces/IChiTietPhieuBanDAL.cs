// DAL/DataLayer/ChiTietPhieuBanFactory.cs
using CuahangNongduoc.BusinessObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CuahangNongduoc.DataLayer
{
    public interface IChiTietPhieuBanDAL
    {
        int Insert(DataRow row, SqlTransaction tx = null);
        DataTable LayChiTietPhieuBan(DateTime ngayBan);
        DataTable LayChiTietPhieuBan(int thang, int nam);
        DataTable LayChiTietPhieuBan(string idPhieuBan);
        DataTable LayDanhSachMaTheoSanPham(string idSanPham);
        string LayIdSanPhamTuMaSanPham(string idMaSanPham);
        DataTable LayThongTinMotLo(string idMaSanPham);
        bool SaveAddedRows(DataTable table);
        decimal TinhGiaBinhQuanGiaQuyen(string idSanPham);
        decimal TinhGiaFIFO(string idSanPham);
        decimal TinhTongThanhTienTheoPhieu(string maPhieuBan);
        int UpdateTongTonKhoSanPham(string idSanPham, int deltaSoLuong, SqlTransaction tx);
        int UpdateTonKho(string idMaSanPham, int deltaSoLuong, SqlTransaction tx);
        void XuatTheoChonLo(DataRow row, SqlTransaction tx, string idPhieuBan);
        void XuatTheoFIFO(DataRow row, SqlTransaction tx, string idPhieuBan);
        // Lấy toàn bộ chi tiết phiếu bán
        DataTable LayTatCaChiTietPhieuBan();
        // Báo cáo doanh thu
        DataTable BaoCaoDoanhThu();
    }
}