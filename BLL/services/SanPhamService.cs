using System;
using System.Data;
using System.Data.SqlClient;
using CuahangNongduoc.Configs;
using CuahangNongduoc.Entities;
using CuahangNongduoc.DAL.Interfaces;
using CuahangNongduoc.DAL.Repositories;
using CuahangNongduoc.BLL.Interfaces;

namespace CuahangNongduoc.BLL.Services
{
    public sealed class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _repo;
        private readonly string _cs;

        public SanPhamService(ISanPhamRepository repo = null, string connectionString = null)
        {
            _cs = string.IsNullOrWhiteSpace(connectionString) ? GlobalConfig.ConnectionString : connectionString;
            _repo = repo ?? new SanPhamRepository(_cs);
        }

        public DataTable DanhSachSanPham_DataTable()
        {
            var dt = CreateSanPhamTable();
            foreach (var sp in _repo.GetAll())
                dt.Rows.Add(sp.Id, sp.TenSanPham, sp.DonGiaNhap, sp.GiaBanSi, sp.GiaBanLe, sp.IdDonViTinh, sp.SoLuong);
            return dt;
        }

        public DataTable TimMaSanPham_DataTable(string idLike)
        {
            var dt = CreateSanPhamTable();
            foreach (var sp in _repo.SearchByIdLike(idLike))
                dt.Rows.Add(sp.Id, sp.TenSanPham, sp.DonGiaNhap, sp.GiaBanSi, sp.GiaBanLe, sp.IdDonViTinh, sp.SoLuong);
            return dt;
        }

        public DataTable TimTenSanPham_DataTable(string tenLike)
        {
            var dt = CreateSanPhamTable();
            foreach (var sp in _repo.SearchByNameLike(tenLike))
                dt.Rows.Add(sp.Id, sp.TenSanPham, sp.DonGiaNhap, sp.GiaBanSi, sp.GiaBanLe, sp.IdDonViTinh, sp.SoLuong);
            return dt;
        }

        public SanPham LaySanPham(string id)
        {
            return _repo.GetById(id);
        }

        public DataTable LaySoLuongTon_DataTable()
        {
            return _repo.LaySoLuongTon();
        }

        public void CapNhatGiaNhap_BinhQuan(string id, long giaMoi, long soLuongNhap)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        _repo.UpdateAverageCost(id, giaMoi, soLuongNhap, conn, tx);
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

        // helper
        private static DataTable CreateSanPhamTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("TEN_SAN_PHAM", typeof(string));
            dt.Columns.Add("DON_GIA_NHAP", typeof(long));
            dt.Columns.Add("GIA_BAN_SI", typeof(long));
            dt.Columns.Add("GIA_BAN_LE", typeof(long));
            dt.Columns.Add("ID_DON_VI_TINH", typeof(string));
            dt.Columns.Add("SO_LUONG", typeof(int));
            return dt;
        }
    }
}
