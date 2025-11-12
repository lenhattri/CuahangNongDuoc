using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.DataLayer;
using CuahangNongduoc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.BLL.Controller
{
    public class PhieuBanChiPhiController
    {
        private readonly IPhieuBanChiPhiFactory _dal;

        // ✅ Constructor inject interface
        public PhieuBanChiPhiController(IPhieuBanChiPhiFactory dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // ✅ Constructor mặc định (dùng nhanh trong WinForms)
        public PhieuBanChiPhiController() : this(new PhieuBanChiPhiFactory()) { }

        public DataTable LayDanhSachTheoPhieuBan(string maPhieuBan)
        {
            return _dal.LayDanhSachTheoPhieuBan(maPhieuBan);
        }

        public IList<ChiPhiPhatSinh> LayDanhSachTheoPB(string maPhieuBan)
        {
            DataTable table = _dal.LayDanhSachTheoPhieuBan(maPhieuBan);
            IList<ChiPhiPhatSinh> ds = new List<ChiPhiPhatSinh>();
            foreach (DataRow row in table.Rows)
            {
                ChiPhiPhatSinh chiPhi = new ChiPhiPhatSinh
                {
                    Id = row["ID"].ToString(),
                    TenChiPhi = row["TEN_CHI_PHI"].ToString(),
                    LoaiChiPhi = row["LOAI_CHI_PHI"].ToString(),
                    SoTien = Convert.ToInt32(row["SO_TIEN"])
                };
                ds.Add(chiPhi);
            }
            return ds;
        }

        public void LuuChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis)
        {
            _dal.LuuChiPhiPhatSinh(maPhieuBan, chiPhis);
        }

        public void CapNhatChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis)
        {
            _dal.DeletedByPhieuBan(maPhieuBan);
            _dal.LuuChiPhiPhatSinh(maPhieuBan, chiPhis);
        }

        public void HienThiDataGridView(DataGridView dgv, BindingNavigator bnv, string maPhieuBan)
        {
            DataTable table = LayDanhSachTheoPhieuBan(maPhieuBan);
            BindingSource bsource = new BindingSource { DataSource = table };
            bnv.BindingSource = bsource;
            dgv.DataSource = bsource;
        }

        public void Insert(PhieuBanChiPhi phieuBanChiPhi)
        {
            _dal.Insert(phieuBanChiPhi);
        }
    }
}
