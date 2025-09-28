using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.DAL.Interfaces
{
    public interface ISanPhamRepository
    {
        // Query (tự mở/đóng kết nối)
        IEnumerable<SanPham> GetAll();
        IEnumerable<SanPham> SearchByIdLike(string idLike);
        IEnumerable<SanPham> SearchByNameLike(string tenLike);
        SanPham GetById(string id);
        DataTable LaySoLuongTon();

        // Command (dùng conn/tx do BLL mở)
        int UpdateQuantity(string maSanPham, int delta, SqlConnection conn, SqlTransaction tx);
        int Insert(SanPham sp, SqlConnection conn, SqlTransaction tx);
        int Update(SanPham sp, SqlConnection conn, SqlTransaction tx);
        int Delete(string id, SqlConnection conn, SqlTransaction tx);

        // Nâng giá vốn bình quân + cập nhật tồn (theo lô nhập)
        int UpdateAverageCost(string id, long giaMoi, long soLuongNhap, SqlConnection conn, SqlTransaction tx);
        DataTable DanhSachSanPham_DataTable();
        void CapNhatGiaNhapBinhQuan(string idSanPham, long giaNhapMoi, long soLuongMoi);
    }
}
