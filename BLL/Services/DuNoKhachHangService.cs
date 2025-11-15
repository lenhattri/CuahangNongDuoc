using System;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc;

namespace CuahangNongduoc.BLL.Services
{
    public class DuNoKhachHangService : IDuNoKhachHangService
    {
        private readonly IDuNoKhachHangDAL _duNoDal;
        private readonly IKhachHangFactory _khachHangDal;
        private readonly IPhieuBanFactory _phieuBanDal;
        private readonly IPhieuThanhToanDAL _phieuThanhToanDal;

        public DuNoKhachHangService(
            IDuNoKhachHangDAL duNoDal,
            IKhachHangFactory khachHangDal,
            IPhieuBanFactory phieuBanDal,
            IPhieuThanhToanDAL phieuThanhToanDal)
        {
            _duNoDal = duNoDal ?? throw new ArgumentNullException(nameof(duNoDal));
            _khachHangDal = khachHangDal ?? throw new ArgumentNullException(nameof(khachHangDal));
            _phieuBanDal = phieuBanDal ?? throw new ArgumentNullException(nameof(phieuBanDal));
            _phieuThanhToanDal = phieuThanhToanDal ?? throw new ArgumentNullException(nameof(phieuThanhToanDal));
        }

        public DataTable TongHop(int thang, int nam, Action<int, int> capNhatTienDo = null)
        {
            int thangTruoc = 0, namTruoc = 0;
            ThamSo.PreMonth(ref thangTruoc, ref namTruoc, thang, nam);

            _duNoDal.Clear(thang, nam);
            _duNoDal.LoadSchema();

            var ketQua = _duNoDal.DanhsachDuNo(thang, nam);
            ketQua.Clear();

            var khachHang = _khachHangDal.DanhsachKhachHang();
            int tong = khachHang.Rows.Count;
            int daXuLy = 0;

            foreach (DataRow row in khachHang.Rows)
            {
                string idKhach = Convert.ToString(row["ID"]);

                long dauKy = _duNoDal.LayDuNo(idKhach, thangTruoc, namTruoc);
                long phatSinh = _phieuBanDal.LayConNo(idKhach, thang, nam);
                long daTra = _phieuThanhToanDal.LayTongTien(idKhach, thang, nam);
                long cuoiKy = dauKy + phatSinh - daTra;

                var dongMoi = _duNoDal.NewRow();
                dongMoi["ID_KHACH_HANG"] = idKhach;
                dongMoi["THANG"] = thang;
                dongMoi["NAM"] = nam;
                dongMoi["DAU_KY"] = dauKy;
                dongMoi["PHAT_SINH"] = phatSinh;
                dongMoi["DA_TRA"] = daTra;
                dongMoi["CUOI_KY"] = cuoiKy;
                _duNoDal.Add(dongMoi);

                var dongHienThi = ketQua.NewRow();
                dongHienThi.ItemArray = (object[])dongMoi.ItemArray.Clone();
                ketQua.Rows.Add(dongHienThi);

                daXuLy++;
                capNhatTienDo?.Invoke(daXuLy, tong);
            }

            return ketQua;
        }

        public bool Luu() => _duNoDal.Save();
    }
}
