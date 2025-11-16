//using System;
using CuahangNongduoc.DAL.Interfaces;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using CuahangNongduoc.BusinessObject;
//using CuahangNongduoc.DataLayer;


//namespace CuahangNongduoc.Controller
//{
//    public class KhachHangController
//    {
//        KhachHangFactory factory = new KhachHangFactory();

//        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb, bool loai)
//        {
//            cmb.DataSource = factory.DanhsachKhachHang(loai);
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";
//        }
//        public void HienthiChungAutoComboBox(System.Windows.Forms.ComboBox cmb)
//        {
//            cmb.DataSource = factory.DanhsachKhachHang();
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";
//        }

//        public void HienthiKhachHangDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
//        {
//            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
//            DataTable tbl = factory.DanhsachKhachHang(false);
//            tbl.Columns[4].DefaultValue = false;
//            bs.DataSource = tbl;
//            bn.BindingSource = bs;
//            dg.DataSource = bs;

//        }

//        public void HienthiKhachHangChungDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
//        {

//            cmb.DataSource = factory.DanhsachKhachHang();
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";
//            cmb.DataPropertyName = "ID_KHACH_HANG";
//            cmb.HeaderText = "Khách hàng";

//        }

//        public void HienthiKhachHangDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
//        {

//            cmb.DataSource = factory.DanhsachKhachHang(false);
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";
//            cmb.DataPropertyName = "ID_KHACH_HANG";
//            cmb.HeaderText = "Khách hàng";

//        }
//        public void HienthiDaiLyDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
//        {

//            cmb.DataSource = factory.DanhsachKhachHang(true);
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";
//            cmb.DataPropertyName = "ID_KHACH_HANG";
//            cmb.HeaderText = "Đại lý";

//        }
//        public void HienthiDaiLyDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
//        {
//            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
//            DataTable tbl = factory.DanhsachKhachHang(true);
//            tbl.Columns[4].DefaultValue = true;
//            bs.DataSource = tbl;
//            bn.BindingSource = bs;
//            dg.DataSource = bs;

//        }

//        public void TimHoTen(String hoten, bool loai)
//        {
//            factory.TimHoTen(hoten, loai);
//        }
//        public void TimDiaChi(String diachi, bool loai)
//        {
//            factory.TimDiaChi(diachi, loai);
//        }

//        public KhachHang LayKhachHang(String id)
//        {
//            DataTable tbl = factory.LayKhachHang(id);
//            KhachHang kh = new KhachHang();
//            if (tbl.Rows.Count > 0)
//            {
//                kh.Id = Convert.ToString(tbl.Rows[0]["ID"]);
//                kh.HoTen = Convert.ToString(tbl.Rows[0]["HO_TEN"]);
//                kh.DienThoai = Convert.ToString(tbl.Rows[0]["DIEN_THOAI"]);
//                kh.DiaChi = Convert.ToString(tbl.Rows[0]["DIA_CHI"]);
//                kh.LoaiKH = Convert.ToBoolean(tbl.Rows[0]["LOAI_KH"]);
//            }
//            return kh;
//        }

//        public IList<KhachHang> LayDanhSachKhachHang()
//        {
//            DataTable tbl = factory.DanhsachKhachHang();
//            IList<KhachHang> ds = new List<KhachHang>();

//            foreach (DataRow row in tbl.Rows)
//            {
//                KhachHang kh = new KhachHang();
//                kh.Id = Convert.ToString(row["ID"]);
//                kh.HoTen = Convert.ToString(row["HO_TEN"]);
//                kh.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
//                kh.DiaChi = Convert.ToString(row["DIA_CHI"]);
//                kh.LoaiKH = Convert.ToBoolean(row["LOAI_KH"]);
//                ds.Add(kh);
//            }
//            return ds;
//        }
//        public DataRow NewRow()
//        {
//            return factory.NewRow();
//        }
//        public void Add(DataRow row)
//        {
//            factory.Add(row);
//        }
//        public bool Save()
//        {
//            return factory.Save();
//        }
//    }
//}

using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.Controller
{
    public class KhachHangController
    {
        private readonly IKhachHangFactory _dal;

        // ✅ Constructor chính: inject DAL từ ngoài vào
        public KhachHangController(IKhachHangFactory dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // ✅ Constructor rỗng: tự tạo DAL mặc định để form cũ vẫn hoạt động
        public KhachHangController() : this(new KhachHangFactory())
        {
        }

        public void HienthiAutoComboBox(ComboBox cmb, bool loai)
        {
            cmb.DataSource = _dal.DanhsachKhachHang(loai);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }

        public void HienthiChungAutoComboBox(ComboBox cmb)
        {
            cmb.DataSource = _dal.DanhsachKhachHang();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }

        public void HienthiKhachHangDataGridview(DataGridView dg, BindingNavigator bn)
        {
            var bs = new BindingSource();
            DataTable tbl = _dal.DanhsachKhachHang(false);

            if (tbl.Columns.Contains("LOAI_KH"))
                tbl.Columns["LOAI_KH"].DefaultValue = false;

            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiKhachHangChungDataGridviewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _dal.DanhsachKhachHang();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Khách hàng";
        }

        public void HienthiKhachHangDataGridviewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _dal.DanhsachKhachHang(false);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Khách hàng";
        }

        public void HienthiDaiLyDataGridviewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _dal.DanhsachKhachHang(true);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Đại lý";
        }

        public void HienthiDaiLyDataGridview(DataGridView dg, BindingNavigator bn)
        {
            var bs = new BindingSource();
            DataTable tbl = _dal.DanhsachKhachHang(true);

            if (tbl.Columns.Contains("LOAI_KH"))
                tbl.Columns["LOAI_KH"].DefaultValue = true;

            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public DataTable TimHoTen(string hoten, bool loai)
        {
            return _dal.TimHoTen(hoten, loai);
        }

        public DataTable TimDiaChi(string diachi, bool loai)
        {
            return _dal.TimDiaChi(diachi, loai);
        }

        public KhachHang LayKhachHang(string id)
        {
            DataTable tbl = _dal.LayKhachHang(id);
            KhachHang kh = new KhachHang();

            if (tbl.Rows.Count > 0)
            {
                var r = tbl.Rows[0];
                kh.Id = Convert.ToString(r["ID"]);
                kh.HoTen = Convert.ToString(r["HO_TEN"]);
                kh.DienThoai = Convert.ToString(r["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(r["DIA_CHI"]);
                kh.LoaiKH = Convert.ToBoolean(r["LOAI_KH"]);
            }
            return kh;
        }

        public IList<KhachHang> LayDanhSachKhachHang()
        {
            DataTable tbl = _dal.DanhsachKhachHang();
            IList<KhachHang> ds = new List<KhachHang>();

            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(new KhachHang
                {
                    Id = Convert.ToString(row["ID"]),
                    HoTen = Convert.ToString(row["HO_TEN"]),
                    DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                    DiaChi = Convert.ToString(row["DIA_CHI"]),
                    LoaiKH = Convert.ToBoolean(row["LOAI_KH"])
                });
            }

            return ds;
        }

        // Overload cho form KH không cần truyền bool
        public DataTable TimHoTen(string hoten) => _dal.TimHoTen(hoten, false);
        public DataTable TimDiaChi(string diachi) => _dal.TimDiaChi(diachi, false);

        // CRUD binding-friendly
        public DataRow NewRow() => _dal.NewRow();
        public void Add(DataRow row) => _dal.Add(row);
        public bool Save() => _dal.Save();
    }
}

