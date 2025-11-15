using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IChiTietPhieuBanService
    {
        DataTable LayChiTietPhieuBan(string idPhieuBan);
        DataTable LayChiTietPhieuBan(DateTime ngayBan);
        DataTable LayChiTietPhieuBan(int thang, int nam);
        DataRow TaoDongMoi();
        void ThemVaoBoDem(DataRow row);
        bool LuuBoDem();
        IList<ChiTietPhieuBan> LayDanhSachTheoPhieu(string idPhieuBan);
        IList<ChiTietPhieuBan> LayDanhSachTheoNgay(DateTime ngayBan);
        IList<ChiTietPhieuBan> LayDanhSachTheoThoiGian(int thang, int nam);
        IList<ChiTietPhieuBan> LayTatCaChiTiet();
        decimal TinhTongTienTheoPhieu(string maPhieuBan);
        decimal TinhGiaBinhQuanGiaQuyen(string idSanPham);
        decimal TinhGiaFIFO(string idSanPham);
    }
}
