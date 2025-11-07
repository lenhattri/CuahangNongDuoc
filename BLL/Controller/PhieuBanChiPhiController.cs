using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DAL.DataLayer;
using CuahangNongduoc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.BLL.Controller
{
    class PhieuBanChiPhiController
    {
        PhieuBanChiPhiFactory dal = new PhieuBanChiPhiFactory();
        public DataTable LayDanhSachTheoPhieuBan(string maPhieuBan)
        {
            return dal.LayDanhSachTheoPhieuBan(maPhieuBan);
        }
        public IList<ChiPhiPhatSinh> LayDanhSachTheoPB(string maPhieuBan)
        {
            DataTable table = dal.LayDanhSachTheoPhieuBan(maPhieuBan);
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

            dal.LuuChiPhiPhatSinh(maPhieuBan, chiPhis);
        }
        public void CapNhatChiPhiPhatSinh(string maPhieuBan, List<ChiPhiPhatSinh> chiPhis)
        {
            dal.DeletedByPhieuBan(maPhieuBan);
            dal.LuuChiPhiPhatSinh(maPhieuBan, chiPhis);
        }
        public void HienThiDataGridView(System.Windows.Forms.DataGridView dgv, System.Windows.Forms.BindingNavigator bnv, string maPhieuBan)
        {
            DataTable table = LayDanhSachTheoPhieuBan(maPhieuBan);
            System.Windows.Forms.BindingSource bsource = new System.Windows.Forms.BindingSource();
            bsource.DataSource = table;
            bnv.BindingSource = bsource;
            dgv.DataSource = bsource;
        }
        public void Insert(PhieuBanChiPhi phieuBanChiPhi)
        {
             dal.Insert(phieuBanChiPhi);
        }
    }
}
