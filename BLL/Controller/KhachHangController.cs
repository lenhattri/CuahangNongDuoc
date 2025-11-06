using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;


namespace CuahangNongduoc.Controller
{
    public class KhachHangController
    {
        private readonly IKhachHangFactory _factory;

        // ✅ Inject Factory qua constructor (Dependency Injection)
        public KhachHangController(IKhachHangFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void HienthiAutoComboBox(ComboBox cmb, bool loai)
        {
            cmb.DataSource = _factory.DanhsachKhachHang(loai);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }
        public void HienthiChungAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            cmb.DataSource = _factory.DanhsachKhachHang();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }

        public void HienthiKhachHangDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            var bs = new BindingSource();
            DataTable tbl = _factory.DanhsachKhachHang(false);

            if (tbl.Columns.Contains("LOAI_KH"))
                tbl.Columns["LOAI_KH"].DefaultValue = false;

            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
            
        }

        public void HienthiKhachHangChungDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _factory.DanhsachKhachHang();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Khách hàng";

        }

        public void HienthiKhachHangDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _factory.DanhsachKhachHang(false);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Khách hàng";

        }
        public void HienthiDaiLyDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

        public void HienthiDaiLyDataGridviewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _factory.DanhsachKhachHang(true);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Đại lý";

        }
        public void HienthiDaiLyDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            var bs = new BindingSource();
            DataTable tbl = _factory.DanhsachKhachHang(true);

            if (tbl.Columns.Contains("LOAI_KH"))
                tbl.Columns["LOAI_KH"].DefaultValue = true;

            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;

        }

        public void TimHoTen(String hoten, bool loai)
        {
            _factory.TimHoTen(hoten, loai);
        }
        public void TimDiaChi(String diachi, bool loai)
        {
            _factory.TimDiaChi(diachi, loai);
        }
        
        public KhachHang LayKhachHang(String id)
        {
            DataTable tbl = _factory.LayKhachHang(id);
            KhachHang kh = new KhachHang();
            if (tbl.Rows.Count > 0)
            {
                kh.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                kh.HoTen = Convert.ToString(tbl.Rows[0]["HO_TEN"]);
                kh.DienThoai = Convert.ToString(tbl.Rows[0]["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(tbl.Rows[0]["DIA_CHI"]);
                kh.LoaiKH = Convert.ToBoolean(tbl.Rows[0]["LOAI_KH"]);
            }
            return kh;
        }

        public IList<KhachHang> LayDanhSachKhachHang()
        {
            DataTable tbl = _factory.DanhsachKhachHang();
            IList<KhachHang> ds = new List<KhachHang>();

            foreach (DataRow row in tbl.Rows)
            {
                KhachHang kh = new KhachHang();
                kh.Id = Convert.ToString(row["ID"]);
                kh.HoTen = Convert.ToString(row["HO_TEN"]);
                kh.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(row["DIA_CHI"]);
                kh.LoaiKH = Convert.ToBoolean(row["LOAI_KH"]);
                ds.Add(kh);
            }
            return ds;
        }
        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }

        public DataRow NewRow() => _factory.NewRow();
        public void Add(DataRow row) => _factory.Add(row);
        public bool Save() => _factory.Save();
    }
}

