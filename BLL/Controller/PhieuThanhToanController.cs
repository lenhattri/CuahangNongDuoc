using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuThanhToanController
    {
        private readonly IPhieuThanhToanDAL factory;
        private readonly BindingSource bs = new BindingSource();

        public PhieuThanhToanController()
        {
            factory = new PhieuThanhToanDAL();

            // Khởi tạo schema rỗng cho DataBinding tránh lỗi Null
            bs.DataSource = factory.LayPhieuThanhToan("-1");
        }

        /* ===================== CRUD ===================== */

        public DataRow NewRow() => factory.NewRow();

        public void Add(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            factory.Add(row);
        }

        public void Save()
        {
            var cm = GetCurrencyManager();
            cm?.EndCurrentEdit();
            factory.Save();
        }

        /* ===================== LẤY DỮ LIỆU ===================== */

        public PhieuThanhToan LayPhieuThanhToan(string id)
        {
            var tbl = factory.LayPhieuThanhToan(id);
            if (tbl.Rows.Count == 0) return null;

            var r = tbl.Rows[0];
            var ctrlKH = new KhachHangController(new KhachHangFactory());

            long ReadLong(object v)
            {
                if (v == null || v == DBNull.Value) return 0L;
                return Convert.ToInt64(v);
            }

            var ph = new PhieuThanhToan
            {
                Id = Convert.ToString(r["ID"]),
                KhachHang = ctrlKH.LayKhachHang(Convert.ToString(r["ID_KHACH_HANG"])),
                NgayThanhToan = (r["NGAY_THANH_TOAN"] == DBNull.Value)
                    ? DateTime.MinValue : Convert.ToDateTime(r["NGAY_THANH_TOAN"]),
                TongTien = ReadLong(r["TONG_TIEN"]),
                GhiChu = Convert.ToString(r["GHI_CHU"])
            };

            return ph;
        }

        public void TimPhieuThanhToan(BindingNavigator navigator, DataGridView grid,
                              ComboBox cmbKhachHang, TextBox txtMaPhieu,
                              DateTimePicker dtNgayThanhToan, NumericUpDown numTongTien,
                              TextBox txtGhiChu, string kh, DateTime ngay)
        {
            // Gọi DAL để lấy dữ liệu
            var data = factory.TimPhieuThanhToan(kh, ngay);

            // Bind lên gridview
            grid.DataSource = data;

            // (Optionally) cập nhật binding nếu bạn muốn cho các control
            BindingSource bs = new BindingSource { DataSource = data };
            navigator.BindingSource = bs;
            cmbKhachHang.DataBindings.Clear();
            txtMaPhieu.DataBindings.Clear();
            dtNgayThanhToan.DataBindings.Clear();
            numTongTien.DataBindings.Clear();
            txtGhiChu.DataBindings.Clear();

            cmbKhachHang.DataBindings.Add("SelectedValue", bs, "ID_KHACH_HANG");
            txtMaPhieu.DataBindings.Add("Text", bs, "ID");
            dtNgayThanhToan.DataBindings.Add("Value", bs, "NGAY_THANH_TOAN");
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");
            txtGhiChu.DataBindings.Add("Text", bs, "GHI_CHU");
        }


        public long LayTongTien(string maKH, int thang, int nam)
        {
            return factory.LayTongTien(maKH, thang, nam);
        }

        /* ===================== HIỂN THỊ (DATA BINDING) ===================== */

        public void HienthiPhieuThanhToan(BindingNavigator bn, DataGridView dg)
        {
            bs.DataSource = factory.DanhsachPhieuThanhToan();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuThanhToan(
            BindingNavigator bn,
            ComboBox cmbKhachHang,
            TextBox txtId,
            DateTimePicker dtNgay,
            NumericUpDown numTongTien,
            TextBox txtGhiChu)
        {
            bn.BindingSource = bs;

            txtId.DataBindings.Clear();
            txtId.DataBindings.Add("Text", bs, "ID", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbKhachHang.DataBindings.Clear();
            cmbKhachHang.DataBindings.Add("SelectedValue", bs, "ID_KHACH_HANG", true, DataSourceUpdateMode.OnPropertyChanged);

            dtNgay.DataBindings.Clear();
            dtNgay.DataBindings.Add("Value", bs, "NGAY_THANH_TOAN", true, DataSourceUpdateMode.OnPropertyChanged);

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN", true, DataSourceUpdateMode.OnPropertyChanged, 0m);

            txtGhiChu.DataBindings.Clear();
            txtGhiChu.DataBindings.Add("Text", bs, "GHI_CHU", true, DataSourceUpdateMode.OnPropertyChanged);
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

        
    }
}
