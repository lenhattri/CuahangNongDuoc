using CuahangNongduoc.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{
    
    public class PhieuThanhToanController
    {
        private readonly IPhieuThanhToanDAL _factory; // Inject DAL qua interface
        private readonly BindingSource _bs = new BindingSource();

        public PhieuThanhToanController(IPhieuThanhToanDAL factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _bs.DataSource = _factory.LayPhieuThanhToan("-1"); // schema rỗng ban đầu
        }

        public DataRow NewRow()
        {
            return _factory.NewRow();
        }
        public void Add(DataRow row)
        {
            _factory.Add(row);
        }
        public void Save()
        {
            _factory.Save();
        }

        public PhieuThanhToan LayPhieuThanhToan(String id)
        {
            PhieuThanhToan ph = null;
            DataTable tbl = _factory.LayPhieuThanhToan(id);
            if (tbl.Rows.Count > 0 )
            {
                ph = new PhieuThanhToan();
                ph.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                KhachHangController ctrlKH = new KhachHangController();
                ph.KhachHang = ctrlKH.LayKhachHang(Convert.ToString(tbl.Rows[0]["ID_KHACH_HANG"]));
                ph.NgayThanhToan = Convert.ToDateTime(tbl.Rows[0]["NGAY_THANH_TOAN"]);
                ph.TongTien = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
                ph.GhiChu = Convert.ToString(tbl.Rows[0]["GHI_CHU"]);
            }
            return ph;
        }
        
        public void HienthiPhieuThanhToan(BindingNavigator bn, DataGridView dg,ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = _factory.DanhsachPhieuThanhToan();
            bn.BindingSource = bs;
            dg.DataSource = bs;

            
            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_KHACH_HANG");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_THANH_TOAN");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            txtGhichu.DataBindings.Clear();
            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");
        }
        public void TimPhieuThanhToan(BindingNavigator bn, DataGridView dg, ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu,
            String idKH, DateTime dtNgayThu)
        {
            
            BindingSource bs = new BindingSource();
            bs.DataSource = _factory.TimPhieuThanhToan(idKH, dtNgayThu);
            bn.BindingSource = bs;
            dg.DataSource = bs;


            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_KHACH_HANG");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_THANH_TOAN");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            txtGhichu.DataBindings.Clear();
            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");
        }
    }
}
