using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.BLL.Helpers
{
    public static class CauHinhCuaHang
    {
        public enum PhuongThucXuatKho
        {
            FIFO,
            ChonLo
        }
        public enum PhuongThucTinhGia
        {
            BQGQ,
            FIFO
        }
        public static PhuongThucXuatKho PhuongThucXuatKhoHienTai = PhuongThucXuatKho.FIFO;
        public static PhuongThucTinhGia PhuongThucTinhGiaHienTai = PhuongThucTinhGia.BQGQ;
    }
}
