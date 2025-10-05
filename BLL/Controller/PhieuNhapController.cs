//using System;
//using System.Collections.Generic;
//using System.Text;
//using CuahangNongduoc.DataLayer;
//using CuahangNongduoc.BusinessObject;
//using System.Windows.Forms;
//using System.Data;

//namespace CuahangNongduoc.Controller
//{

//    public class PhieuNhapController
//    {
//        PhieuNhapFactory factory = new PhieuNhapFactory();
//        BindingSource bs = new BindingSource();

//        public PhieuNhapController()
//        {
//            bs.DataSource = factory.LayPhieuNhap("-1");
//        }

//        public DataRow NewRow()
//        {
//            return factory.NewRow();
//        }
//        public void Add(DataRow row)
//        {
//            factory.Add(row);
//        }

//        public void Update()
//        {
//            bs.MoveNext();
//            factory.Save();
//        }
//        public void Save()
//        {
//            factory.Save();
//        }


//        public PhieuNhap LayPhieuNhap(String id)
//        {
//            DataTable tbl = factory.LayPhieuNhap(id);
//            PhieuNhap ph = null;
//            NhaCungCapController ctrlNCC = new NhaCungCapController();
//            if (tbl.Rows.Count > 0)
//            {

//                ph = new PhieuNhap();
//                ph.Id =Convert.ToString( tbl.Rows[0]["ID"]);
//                ph.NgayNhap = Convert.ToDateTime(tbl.Rows[0]["NGAY_NHAP"]);
//                ph.TongTien = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
//                ph.DaTra = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
//                ph.ConNo = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
//                ph.NhaCungCap = ctrlNCC.LayNCC(Convert.ToString(tbl.Rows[0]["ID_NHA_CUNG_CAP"]));
//                MaSanPhamController ctrl = new MaSanPhamController();
//                ph.ChiTiet = ctrl.ChiTietPhieuNhap(ph.Id);
//            }
//            return ph;
//        }
//        public void HienthiPhieuNhap(BindingNavigator bn, DataGridView dg)
//        {

//            bs.DataSource = factory.DanhsachPhieuNhap();
//            bn.BindingSource = bs;
//            dg.DataSource = bs;
//        }

//        public void HienthiPhieuNhap(BindingNavigator bn,TextBox txt,ComboBox cmb, DateTimePicker dt, NumericUpDown numTongTien, NumericUpDown numDaTra, NumericUpDown numConNo)
//        {

//            bn.BindingSource = bs;

//            txt.DataBindings.Clear();
//            txt.DataBindings.Add("Text", bs,"ID");

//            cmb.DataBindings.Clear();
//            cmb.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP");

//            dt.DataBindings.Clear();
//            dt.DataBindings.Add("Value", bs, "NGAY_NHAP");

//            numTongTien.DataBindings.Clear();
//            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

//            numDaTra.DataBindings.Clear();
//            numDaTra.DataBindings.Add("Value", bs, "DA_TRA");

//            numConNo.DataBindings.Clear();
//            numConNo.DataBindings.Add("Value", bs, "CON_NO");

//        }

//        public void TimPhieuNhap(String maNCC, DateTime dt)
//        {
//            factory.TimPhieuNhap(maNCC, dt);
//        }

//    }
//}


using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuNhapController
    {
        private readonly PhieuNhapFactory factory = new PhieuNhapFactory();

        public DataRow NewRow()
        {
            return factory.NewRow();
        }

        public void Add(DataRow row)
        {
            factory.Add(row);
        }

        public void Save()
        {
            factory.Save();
        }

        public PhieuNhap LayPhieuNhap(string id)
        {
            DataTable tbl = factory.LayPhieuNhap(id);
            if (tbl.Rows.Count == 0)
                return null;

            var row = tbl.Rows[0];
            var ph = new PhieuNhap
            {
                Id = Convert.ToString(row["ID"]),
                NgayNhap = Convert.ToDateTime(row["NGAY_NHAP"]),
                TongTien = Convert.ToInt64(row["TONG_TIEN"]),
                DaTra = Convert.ToInt64(row["DA_TRA"]),
                ConNo = Convert.ToInt64(row["CON_NO"]),
                NhaCungCap = new NhaCungCapController().LayNCC(Convert.ToString(row["ID_NHA_CUNG_CAP"]))
            };

            // L?y chi ti?t phi?u nh?p (list domain objects)
            var ctrlMaSP = new MaSanPhamController();
            ph.ChiTiet = ctrlMaSP.ChiTietPhieuNhap(ph.Id);

            return ph;
        }

        // Hi?n th? danh sách phi?u nh?p trong DataGridView & BindingNavigator
        public void HienThiPhieuNhap(BindingNavigator bn, DataGridView dg)
        {
            var bs = new BindingSource
            {
                DataSource = factory.DanhsachPhieuNhap()
            };
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        // Bind chi ti?t phi?u nh?p lên controls
        public void HienThiPhieuNhap(BindingNavigator bn, TextBox txtId, ComboBox cmbNCC, DateTimePicker dtNgayNhap,
            NumericUpDown numTongTien, NumericUpDown numDaTra, NumericUpDown numConNo)
        {
            var bs = new BindingSource
            {
                DataSource = factory.DanhsachPhieuNhap()
            };
            bn.BindingSource = bs;

            txtId.DataBindings.Clear();
            txtId.DataBindings.Add("Text", bs, "ID");

            cmbNCC.DataBindings.Clear();
            cmbNCC.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP");

            dtNgayNhap.DataBindings.Clear();
            dtNgayNhap.DataBindings.Add("Value", bs, "NGAY_NHAP");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO");
        }

        // Tìm phi?u nh?p theo nhà cung c?p và ngày nh?p
        public void TimPhieuNhap(string maNCC, DateTime dt, BindingNavigator bn, DataGridView dg)
        {
            var bs = new BindingSource
            {
                DataSource = factory.TimPhieuNhap(maNCC, dt)
            };
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }
    }
}
