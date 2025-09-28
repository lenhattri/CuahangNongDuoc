using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.BLL.Interfaces
{
    public interface ISanPhamService
    {
        // DataTable cho UI cũ
        DataTable DanhSachSanPham_DataTable();
        DataTable TimMaSanPham_DataTable(string idLike);
        DataTable TimTenSanPham_DataTable(string tenLike);
        DataTable LaySoLuongTon_DataTable();

        // Entity
        SanPham LaySanPham(string id);

        // Business logic
        void CapNhatGiaNhap_BinhQuan(string id, long giaMoi, long soLuongNhap);

     

        /// <summary>
        /// Cập nhật đơn giá nhập bình quân = (giá_cũ*tồn_cũ + giá_mới*sl_mới) / (tồn_cũ+sl_mới),
        /// đồng thời tăng SO_LUONG lên sl_mới.
        /// </summary>
        void CapNhatGiaNhap(string idSanPham, long giaMoi, long soLuongNhap);
    }
}
