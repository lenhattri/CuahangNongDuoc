using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface IMaSanPhamService
    {
        // Query thuần
        MaSanPham LayMaSanPham(string idMa);
        IEnumerable<MaSanPham> DanhSachMaSanPhamConHang(string idSanPham);
        IEnumerable<MaSanPham> ChiTietPhieuNhap(string idPhieuNhap);
        IEnumerable<MaSanPham> DanhSachConHang();
        IEnumerable<MaSanPham> DanhSachHetHanTruoc(DateTime date);

        // Dành cho UI cũ (DataTable)
        DataTable DanhSachMaSanPhamConHang_Table(string idSanPham);
        DataTable ChiTietPhieuNhap_Table(string idPhieuNhap);
        DataTable DanhSachConHang_Table();

        // Nghiệp vụ thường dùng
        void CapNhatSoLuong(string idMaSanPham, int delta);  // + tăng / - giảm
        void TaoMaSanPham(MaSanPham msp);
        void CapNhatMaSanPham(MaSanPham msp);
        void XoaMaSanPham(string idMaSanPham);

        /// <summary>
        /// Lấy thông tin cơ bản của 1 mã lô (ID, ID_SAN_PHAM, SO_LUONG).
        /// Trả về null nếu không tồn tại.
        /// </summary>
        MaSanPhamBasic LayMaSanPhamBasic(string id);

        /// <summary>
        /// Danh sách mã lô còn tồn theo ID sản phẩm (để bind combobox/column).
        /// </summary>
        DataTable DanhSachMaSanPhamConTonTheoSanPham_DataTable(string idSanPham);

        /// <summary>
        /// Danh sách tất cả mã lô còn tồn (nếu cần).
        /// </summary>
        DataTable DanhSachMaSanPhamConTon_DataTable();
    }
}
