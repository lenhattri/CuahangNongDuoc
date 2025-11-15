using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class NhaCungCapService : INhaCungCapService
    {
        private readonly INhaCungCapDAL _dal;

        public NhaCungCapService(INhaCungCapDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DataTable DanhSachNhaCungCap() => _dal.DanhsachNCC();

        public DataTable TimTheoHoTen(string hoTen) => _dal.TimHoTen(hoTen ?? string.Empty);

        public DataTable TimTheoDiaChi(string diaChi) => _dal.TimDiaChi(diaChi ?? string.Empty);

        public NhaCungCap LayNhaCungCap(string id)
        {
            var table = _dal.LayNCC(id);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            return new NhaCungCap
            {
                Id = Convert.ToString(row["ID"]),
                HoTen = Convert.ToString(row["HO_TEN"]),
                DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                DiaChi = Convert.ToString(row["DIA_CHI"])
            };
        }

        public IList<NhaCungCap> LayTatCa()
        {
            var table = _dal.DanhsachNCC();
            var result = new List<NhaCungCap>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new NhaCungCap
                {
                    Id = Convert.ToString(row["ID"]),
                    HoTen = Convert.ToString(row["HO_TEN"]),
                    DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                    DiaChi = Convert.ToString(row["DIA_CHI"])
                });
            }

            return result;
        }

        public DataRow TaoDongMoi() => _dal.NewRow();

        public void Them(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            _dal.Add(row);
        }

        public bool Luu() => _dal.Save();
    }
}
