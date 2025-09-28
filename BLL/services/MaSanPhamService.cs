using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.Configs;
using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.DAL.Repositories;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.BLL.Services
{
    public sealed class MaSanPhamService : IMaSanPhamService
    {
        private readonly IMaSanPhamRepository _mspRepo;
        private readonly ISanPhamRepository _spRepo; // dùng khi cần đồng bộ tồn tổng ở SAN_PHAM
        private readonly string _cs;

        public MaSanPhamService(
            IMaSanPhamRepository mspRepo = null,
            ISanPhamRepository spRepo = null,
            string connectionString = null)
        {
            _cs = string.IsNullOrWhiteSpace(connectionString) ? GlobalConfig.ConnectionString : connectionString;
            _mspRepo = mspRepo ?? new MaSanPhamRepository(_cs);
            _spRepo = spRepo ?? new SanPhamRepository(_cs);
        }

        // -------------------- Query thuần --------------------

        public MaSanPham LayMaSanPham(string idMa) { return _mspRepo.GetById(idMa); }

        public IEnumerable<MaSanPham> DanhSachMaSanPhamConHang(string idSanPham)
        {
            return _mspRepo.GetBySanPhamIdAvailable(idSanPham);
        }

        public IEnumerable<MaSanPham> ChiTietPhieuNhap(string idPhieuNhap)
        {
            return _mspRepo.GetByPhieuNhapId(idPhieuNhap);
        }

        public IEnumerable<MaSanPham> DanhSachConHang()
        {
            return _mspRepo.GetAllAvailable();
        }

        public IEnumerable<MaSanPham> DanhSachHetHanTruoc(DateTime date)
        {
            return _mspRepo.GetExpiredBefore(date);
        }

        // -------------------- DataTable cho UI cũ --------------------

        public DataTable DanhSachMaSanPhamConHang_Table(string idSanPham)
        {
            return _mspRepo.GetBySanPhamIdAvailable_Table(idSanPham);
        }

        public DataTable ChiTietPhieuNhap_Table(string idPhieuNhap)
        {
            return _mspRepo.GetByPhieuNhapId_Table(idPhieuNhap);
        }

        public DataTable DanhSachConHang_Table()
        {
            return _mspRepo.GetAllAvailable_Table();
        }

        // -------------------- Nghiệp vụ --------------------
        // Lưu ý: nếu bạn duy trì cả tồn tổng ở SAN_PHAM, hãy đồng bộ luôn khi đổi số lượng mã con.

        public void CapNhatSoLuong(string idMaSanPham, int delta)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Cập nhật mã con
                        _mspRepo.UpdateQuantity(idMaSanPham, delta, conn, tx);

                        // Nếu muốn đồng bộ tồn tổng ở SAN_PHAM:
                        var msp = _mspRepo.GetById(idMaSanPham); // đọc lại ngoài tx là không lý tưởng
                        // tốt hơn: SELECT trước trong cùng tx ở Repository; demo ngắn gọn:
                        if (msp != null)
                        {
                            _spRepo.UpdateQuantity(msp.IdSanPham, delta, conn, tx);
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void TaoMaSanPham(MaSanPham msp)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        _mspRepo.Insert(msp, conn, tx);
                        // tuỳ luật: có cộng tồn tổng ở SAN_PHAM không?
                        _spRepo.UpdateQuantity(msp.IdSanPham, msp.SoLuong, conn, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void CapNhatMaSanPham(MaSanPham msp)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        _mspRepo.Update(msp, conn, tx);
                        // không tự động chỉnh tồn tổng ở đây vì khó tính delta; nên có hàm chuyên dụng
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void XoaMaSanPham(string idMaSanPham)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // nếu xóa khi SO_LUONG > 0, cân nhắc trừ tồn tổng
                        var msp = _mspRepo.GetById(idMaSanPham);
                        if (msp != null && msp.SoLuong != 0)
                        {
                            _spRepo.UpdateQuantity(msp.IdSanPham, -msp.SoLuong, conn, tx);
                        }

                        _mspRepo.Delete(idMaSanPham, conn, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}
