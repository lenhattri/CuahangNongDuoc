﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using CuahangNongduoc.DataLayer;
//using CuahangNongduoc.BusinessObject;
//using System.Windows.Forms;
//using System.Data;

//namespace CuahangNongduoc.Controller
//{

//    public class PhieuChiController
//    {
//        PhieuChiFactory factory = new PhieuChiFactory();
//        private DataTable _currentTable;

//        public DataRow NewRow()
//        {
//            return factory.NewRow();
//        }
//        public void Add(DataRow row)
//        {
//            factory.Insert(_currentTable,row);
//        }
//        public void Save()
//        {
//            factory.SaveAddedRows(_currentTable);
//        }

//        public PhieuChi LayPhieuChi(String id)
//        {
//            PhieuChi ph = null;
//            DataTable tbl = factory.LayPhieuChi(id);
//            if (tbl.Rows.Count > 0 )
//            {
//                ph = new PhieuChi();
//                ph.Id = Convert.ToString(tbl.Rows[0]["ID"]);
//                LyDoChiController ctrlLyDo = new LyDoChiController();
//                ph.LyDoChi = ctrlLyDo.LayLyDoChi(Convert.ToInt64(tbl.Rows[0]["ID_LY_DO_CHI"]));
//                ph.NgayChi = Convert.ToDateTime(tbl.Rows[0]["NGAY_CHI"]);
//                ph.TongTien = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
//                ph.GhiChu = Convert.ToString(tbl.Rows[0]["GHI_CHU"]);
//            }
//            return ph;
//        }

//        public void HienthiPhieuChi(BindingNavigator bn, DataGridView dg,ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu)
//        {
//            BindingSource bs = new BindingSource();
//            bs.DataSource = factory.DanhsachPhieuChi();
//            bn.BindingSource = bs;
//            dg.DataSource = bs;


//            txt.DataBindings.Clear();
//            txt.DataBindings.Add("Text", bs, "ID");

//            cmb.DataBindings.Clear();
//            cmb.DataBindings.Add("SelectedValue", bs, "ID_LY_DO_CHI");

//            dt.DataBindings.Clear();
//            dt.DataBindings.Add("Value", bs, "NGAY_CHI");

//            numTongTien.DataBindings.Clear();
//            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

//            txtGhichu.DataBindings.Clear();
//            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");


//        }

//        public void TimPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb, TextBox txt, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu,
//            int lydo, DateTime ngay)
//        {

//            BindingSource bs = new BindingSource();
//            bs.DataSource = factory.TimPhieuChi(lydo, ngay);
//            bn.BindingSource = bs;
//            dg.DataSource = bs;


//            txt.DataBindings.Clear();
//            txt.DataBindings.Add("Text", bs, "ID");

//            cmb.DataBindings.Clear();
//            cmb.DataBindings.Add("SelectedValue", bs, "ID_LY_DO_CHI");

//            dt.DataBindings.Clear();
//            dt.DataBindings.Add("Value", bs, "NGAY_CHI");

//            numTongTien.DataBindings.Clear();
//            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

//            txtGhichu.DataBindings.Clear();
//            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuChiController
    {
        // DAL ADO.NET (SqlClient) đã viết ở bước trước
        private readonly PhieuChiFactory _dal = new PhieuChiFactory();

        // Bảng hiện tại cho binding + add (thay cho DataService cũ)
        private DataTable _currentTable;

        /* ===================== BINDING HIỂN THỊ ===================== */
        public void HienthiPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb, TextBox txtId, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu)
        {
            _currentTable = _dal.DanhsachPhieuChi();
            var bs = new BindingSource
            {
                DataSource = _currentTable
            };
            bn.BindingSource = bs;
            dg.DataSource = bs;

            txtId.DataBindings.Clear();
            txtId.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_LY_DO_CHI");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_CHI");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            txtGhichu.DataBindings.Clear();
            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");
        }

        public void TimPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb, TextBox txtId, DateTimePicker dt, NumericUpDown numTongTien, TextBox txtGhichu, int lydo, DateTime ngay)
        {
            _currentTable = _dal.TimPhieuChi(lydo, ngay);
            var bs = new BindingSource
            {
                DataSource = _currentTable
            };
            bn.BindingSource = bs;
            dg.DataSource = bs;

            txtId.DataBindings.Clear();
            txtId.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_LY_DO_CHI");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_CHI");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            txtGhichu.DataBindings.Clear();
            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");
        }

        /* ===================== API GIỮ NGUYÊN CHO UI ===================== */
        public DataRow NewRow()
        {
            if (_currentTable == null)
                throw new InvalidOperationException("Phải load dữ liệu trước bằng HienthiPhieuChi hoặc TimPhieuChi.");

            return _currentTable.NewRow();
        }

        public void Add(DataRow row)
        {
            if (_currentTable == null)
                throw new InvalidOperationException("Phải load dữ liệu trước bằng HienthiPhieuChi hoặc TimPhieuChi.");

            if (row == null)
                throw new ArgumentNullException(nameof(row));

            _currentTable.Rows.Add(row);
        }

        public bool Save()
        {
            if (_currentTable == null)
                throw new InvalidOperationException("Phải load dữ liệu trước bằng HienthiPhieuChi hoặc TimPhieuChi.");

            // Chỉ ghi các row trạng thái Added → DAL sẽ Insert (transaction)
            bool ok = _dal.SaveChanges(_currentTable);

            if (ok)
            {
                _currentTable.AcceptChanges(); // reset RowState sau khi commit
            }

            return ok;
        }

        /* ===================== TRẢ VỀ SINGLE DOMAIN OBJECT ===================== */
        public PhieuChi LayPhieuChi(string id)
        {
            DataTable tbl = _dal.LayPhieuChi(id);
            if (tbl.Rows.Count == 0)
                return null;

            var ctrlLyDo = new LyDoChiController();
            var row = tbl.Rows[0];
            return new PhieuChi
            {
                Id = Convert.ToString(row["ID"]),
                LyDoChi = ctrlLyDo.LayLyDoChi(Convert.ToInt64(row["ID_LY_DO_CHI"])),
                NgayChi = Convert.ToDateTime(row["NGAY_CHI"]),
                TongTien = Convert.ToInt64(row["TONG_TIEN"]),
                GhiChu = Convert.ToString(row["GHI_CHU"])
            };
        }

        /* ===================== TRẢ VỀ LIST DOMAIN OBJECT (THÊM MỚI) ===================== */
        public IList<PhieuChi> LayDanhSachPhieuChi()
        {
            return MapToList(_dal.DanhsachPhieuChi());
        }

        public IList<PhieuChi> TimPhieuChi(int lydo, DateTime ngay)
        {
            return MapToList(_dal.TimPhieuChi(lydo, ngay));
        }

        /* ===================== HELPERS ===================== */
        private static IList<PhieuChi> MapToList(DataTable tbl)
        {
            var ds = new List<PhieuChi>();
            var ctrlLyDo = new LyDoChiController(); // tạo 1 lần, dùng lại

            foreach (DataRow row in tbl.Rows)
            {
                var ph = new PhieuChi
                {
                    Id = Convert.ToString(row["ID"]),
                    LyDoChi = ctrlLyDo.LayLyDoChi(Convert.ToInt64(row["ID_LY_DO_CHI"])),
                    NgayChi = Convert.ToDateTime(row["NGAY_CHI"]),
                    TongTien = Convert.ToInt64(row["TONG_TIEN"]),
                    GhiChu = Convert.ToString(row["GHI_CHU"])
                };
                ds.Add(ph);
            }
            return ds;
        }
    }
}