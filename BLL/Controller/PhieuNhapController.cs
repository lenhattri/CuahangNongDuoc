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
        private readonly IPhieuNhapFactory factory;
        private readonly BindingSource bs = new BindingSource();

        public PhieuNhapController()
        {
            factory = new PhieuNhapFactory();

            // Khi form khởi tạo: bind schema rỗng để không lỗi null
            bs.DataSource = factory.LayPhieuNhap("-1");
        }

        /* ===================== CRUD LOGIC ===================== */

        public DataRow NewRow() => factory.NewRow();

        public void Add(DataRow row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));
            factory.Add(row);
        }

        public void Save()
        {
            var cm = GetCurrencyManager();
            cm?.EndCurrentEdit();
            factory.Save();
        }
        public void Save()
        {
            factory.Save();
        }

        /* ===================== LẤY DỮ LIỆU ===================== */

        public PhieuNhap LayPhieuNhap(string id)
        {
            var tbl = factory.LayPhieuNhap(id);
            if (tbl.Rows.Count == 0) return null;

            var r = tbl.Rows[0];
            var ctrlNCC = new NhaCungCapController();

            long ReadLong(object v)
            {
                if (v == null || v == DBNull.Value) return 0L;
                return Convert.ToInt64(v);
            }

            var ph = new PhieuNhap
            {
                Id = Convert.ToString(r["ID"]),
                NgayNhap = (r["NGAY_NHAP"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(r["NGAY_NHAP"]),
                TongTien = ReadLong(r["TONG_TIEN"]),
                DaTra = ReadLong(r["DA_TRA"]),
                ConNo = ReadLong(r["CON_NO"]),
                NhaCungCap = ctrlNCC.LayNCC(Convert.ToString(r["ID_NHA_CUNG_CAP"]))
            };

            // Nạp chi tiết (liên quan bảng khác)
            var ctrlSP = new MaSanPhamController();
            ph.ChiTiet = ctrlSP.ChiTietPhieuNhap(ph.Id);

            return ph;
        }

        public void TimPhieuNhap(string maNCC, DateTime ngay)
        {
            bs.DataSource = factory.TimPhieuNhap(maNCC, ngay);
        }

        /* ===================== HIỂN THỊ BINDING ===================== */

        public void HienthiPhieuNhap(BindingNavigator bn, DataGridView dg)
        {
            
            bs.DataSource = factory.DanhsachPhieuNhap();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuNhap(
            BindingNavigator bn,
            TextBox txtId,
            ComboBox cmbNCC,
            DateTimePicker dtNgayNhap,
            NumericUpDown numTongTien,
            NumericUpDown numDaTra,
            NumericUpDown numConNo)
        {

            bn.BindingSource = bs;

            txtId.DataBindings.Clear();
            txtId.DataBindings.Add("Text", bs, "ID", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbNCC.DataBindings.Clear();
            cmbNCC.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP", true, DataSourceUpdateMode.OnPropertyChanged);

            dtNgayNhap.DataBindings.Clear();
            dtNgayNhap.DataBindings.Add("Value", bs, "NGAY_NHAP", true, DataSourceUpdateMode.OnPropertyChanged);

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO");
            
        }

        /* ===================== HỖ TRỢ ===================== */

        private CurrencyManager GetCurrencyManager()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.BindingContext[bs] is CurrencyManager cm)
                    return cm;
            }
            return null;
        }

        public void Update()
        {
            // Kết thúc mọi binding từ UI trước khi lưu
            var cm = GetCurrencyManager();
            cm?.EndCurrentEdit();

            // Gọi lưu thay đổi xuống DAL
            factory.Save();
        }

    }
}

