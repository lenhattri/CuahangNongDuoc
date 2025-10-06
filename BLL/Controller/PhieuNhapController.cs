using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{
    
    public class PhieuNhapController
    {
        PhieuNhapDAL dal = new PhieuNhapDAL();
        BindingSource bs = new BindingSource();

        public PhieuNhapController()
        {
            bs.DataSource = dal.LayPhieuNhap("-1");
        }

        public DataRow NewRow()
        {
            return dal.NewRow();
        }
        public void Add(DataRow row)
        {
            dal.Add(row);
        }

        public void Update()
        {
            bs.MoveNext();
            dal.Save();
        }
        public void Save()
        {
            dal.Save();
        }

    
        public PhieuNhap LayPhieuNhap(String id)
        {
            DataTable tbl = dal.LayPhieuNhap(id);
            PhieuNhap ph = null;
            NhaCungCapController ctrlNCC = new NhaCungCapController();
            if (tbl.Rows.Count > 0)
            {

                ph = new PhieuNhap();
                ph.Id =Convert.ToString( tbl.Rows[0]["ID"]);
                ph.NgayNhap = Convert.ToDateTime(tbl.Rows[0]["NGAY_NHAP"]);
                ph.TongTien = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
                ph.DaTra = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
                ph.ConNo = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
                ph.NhaCungCap = ctrlNCC.LayNCC(Convert.ToString(tbl.Rows[0]["ID_NHA_CUNG_CAP"]));
                MaSanPhamController ctrl = new MaSanPhamController();
                ph.ChiTiet = ctrl.ChiTietPhieuNhap(ph.Id);
            }
            return ph;
        }
        public void HienthiPhieuNhap(BindingNavigator bn, DataGridView dg)
        {
            
            bs.DataSource = dal.DanhsachPhieuNhap();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuNhap(BindingNavigator bn,TextBox txt,ComboBox cmb, DateTimePicker dt, NumericUpDown numTongTien, NumericUpDown numDaTra, NumericUpDown numConNo)
        {

            bn.BindingSource = bs;

            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs,"ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_NHAP");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO");
            
        }

        public void TimPhieuNhap(String maNCC, DateTime dt)
        {
            dal.TimPhieuNhap(maNCC, dt);
        }
   
    }
}
