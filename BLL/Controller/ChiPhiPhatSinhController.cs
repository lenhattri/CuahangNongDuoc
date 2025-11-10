/*using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.BLL.Controller
{
    class ChiPhiPhatSinhController
    {
        ChiPhiPhatSinhFactory dal = new ChiPhiPhatSinhFactory();
        public DataTable DanhSachChiPhiPhatSinh()
        {
            return dal.DanhSachChiPhiPhatSinh();
        }
        
        public void HienThiDataGridView(System.Windows.Forms.DataGridView dgv, System.Windows.Forms.BindingNavigator bnv)
        {
            DataTable table = DanhSachChiPhiPhatSinh();
            System.Windows.Forms.BindingSource bsource = new System.Windows.Forms.BindingSource();
            bsource.DataSource = table;
            bnv.BindingSource = bsource;
            dgv.DataSource = bsource;
        }
        public void HienThiComboBox(System.Windows.Forms.ComboBox bnv)
        {
            DataTable dt = dal.DanhSachChiPhiPhatSinh();
            bnv.DataSource = dt;
            bnv.DisplayMember = "TEN_CHI_PHI";
            bnv.ValueMember = "ID";
            bnv.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            bnv.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
        }
        public void InSert(ChiPhiPhatSinh chiPhi)
        {
            dal.InSert(chiPhi);
        }
        public void Update(ChiPhiPhatSinh chiPhi)
        {
            dal.Update(chiPhi);
        }
        public void Delete(string id)
        {
            dal.Delete(id);
        }
    }
}*/
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.BLL.Controller
{
    public class ChiPhiPhatSinhController
    {
        private readonly IChiPhiPhatSinhFactory _dal;

        // ✅ Constructor injection (chuẩn DI)
        public ChiPhiPhatSinhController(IChiPhiPhatSinhFactory dal)
        {
            _dal = dal;
        }

        // ✅ Lấy danh sách chi phí phát sinh
        public DataTable DanhSachChiPhiPhatSinh()
        {
            return _dal.DanhSachChiPhiPhatSinh();
        }

        // ✅ Hiển thị dữ liệu lên DataGridView và BindingNavigator
        public void HienThiDataGridView(DataGridView dgv, BindingNavigator bnv)
        {
            DataTable table = DanhSachChiPhiPhatSinh();
            BindingSource bsource = new BindingSource { DataSource = table };
            bnv.BindingSource = bsource;
            dgv.DataSource = bsource;
        }

        // ✅ Thêm mới chi phí
        public void InSert(ChiPhiPhatSinh chiPhi)
        {
            _dal.InSert(chiPhi);
        }

        // ✅ Cập nhật chi phí
        public void Update(ChiPhiPhatSinh chiPhi)
        {
            _dal.Update(chiPhi);
        }

        // ✅ Xóa chi phí
        public void Delete(string id)
        {
            _dal.Delete(id);
        }
    }
}
