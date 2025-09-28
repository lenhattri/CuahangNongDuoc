using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.DAL.Interfaces
{
    public interface IMaSanPhamRepository
    {
        // Query: tự mở/đóng kết nối
        MaSanPham GetById(string idMa);
        IEnumerable<MaSanPham> GetBySanPhamIdAvailable(string idSanPham);      // SO_LUONG > 0
        IEnumerable<MaSanPham> GetByPhieuNhapId(string idPhieuNhap);           // chi tiết một phiếu nhập
        IEnumerable<MaSanPham> GetAllAvailable();                               // tất cả mã còn số lượng
        IEnumerable<MaSanPham> GetExpiredBefore(DateTime date);                 // hết hạn đến ngày

        // DataTable tiện cho UI cũ
        DataTable GetBySanPhamIdAvailable_Table(string idSanPham);
        DataTable GetByPhieuNhapId_Table(string idPhieuNhap);
        DataTable GetAllAvailable_Table();

        // Command: dùng conn/tx do BLL mở
        int UpdateQuantity(string idMa, int delta, SqlConnection conn, SqlTransaction tx);
        int Insert(MaSanPham msp, SqlConnection conn, SqlTransaction tx);
        int Update(MaSanPham msp, SqlConnection conn, SqlTransaction tx);
        int Delete(string idMa, SqlConnection conn, SqlTransaction tx);
    }
}
