using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc.Controller
{
    
    public class PhieuChiController
    {
        // ✅ Đồng bộ lại với IPhieuChiFactory
        private readonly IPhieuChiFactory _dal = new PhieuChiFactory();

        // Bảng hiện tại cho binding + add + save
        private DataTable _currentTable;

        /* ===================== BINDING HIỂN THỊ ===================== */
        public void HienthiPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb,
                                    TextBox txtId, DateTimePicker dt,
                                    NumericUpDown numTongTien, TextBox txtGhichu)
        {
            _currentTable = _dal.DanhsachPhieuChi();
            var bs = new BindingSource { DataSource = _currentTable };
            bn.BindingSource = bs;
            dg.DataSource = bs;

            
            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_LY_DO_CHI");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_CHI");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            txtGhichu.DataBindings.Clear();
            txtGhichu.DataBindings.Add("Text", bs, "GHI_CHU");


        }

        public void TimPhieuChi(BindingNavigator bn, DataGridView dg, ComboBox cmb,
                                 TextBox txtId, DateTimePicker dt,
                                 NumericUpDown numTongTien, TextBox txtGhichu,
                                 int lydo, DateTime ngay)
        {
            _currentTable = _dal.TimPhieuChi(lydo, ngay);
            var bs = new BindingSource { DataSource = _currentTable };
            bn.BindingSource = bs;
            dg.DataSource = bs;


            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

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
            // NEW — dùng từ factory để lấy schema chuẩn
            return _dal.NewRow();
        }

        public void Add(DataRow row)
        {
            _dal.Add(row);
        }

        public bool Save()
        {
            // NEW — dùng hàm Save() trong factory
            return _dal.Save();
        }

        /* ===================== TRẢ VỀ SINGLE OBJECT ===================== */
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

        /* ===================== TRẢ VỀ DANH SÁCH ===================== */
        public IList<PhieuChi> LayDanhSachPhieuChi()
        {
            return MapToList(_dal.DanhsachPhieuChi());
        }

        public IList<PhieuChi> TimPhieuChiList(int lydo, DateTime ngay)
        {
            return MapToList(_dal.TimPhieuChi(lydo, ngay));
        }

        /* ===================== TÍNH TOÁN / BÁO CÁO ===================== */
        public long LayTongTien(string lydo, int thang, int nam)
        {
            return _dal.LayTongTien(lydo, thang, nam);
        }

        /* ===================== HELPERS ===================== */
        private static IList<PhieuChi> MapToList(DataTable tbl)
        {
            var ds = new List<PhieuChi>();
            var ctrlLyDo = new LyDoChiController();

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
