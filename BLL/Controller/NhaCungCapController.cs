//using System;
using CuahangNongduoc.DAL.Interfaces;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using CuahangNongduoc.BusinessObject;
//using CuahangNongduoc.DataLayer;


//namespace CuahangNongduoc.Controller
//{
//    public class NhaCungCapController
//    {
//        NhaCungCapFactory factory = new  NhaCungCapFactory();

//        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
//        {
//            cmb.DataSource = factory.DanhsachNCC();
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";

//        }
//        public void HienthiAllComboBox(System.Windows.Forms.ComboBox cmb)
//        {
//            IList<NhaCungCap> ds = this.LayDanhSachNCC();
//            ds.Add(new NhaCungCap("ALL","Tất cả"));
//            cmb.DataSource = ds;
//            cmb.DisplayMember = "HoTen";
//            cmb.ValueMember = "Id";

//        }

//        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
//        {
//            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
//            DataTable tbl = factory.DanhsachNCC();
//            bs.DataSource = tbl;
//            bn.BindingSource = bs;
//            dg.DataSource = bs;

//        }

//        public void HienthiDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
//        {

//            cmb.DataSource = factory.DanhsachNCC();
//            cmb.DisplayMember = "HO_TEN";
//            cmb.ValueMember = "ID";
//            cmb.DataPropertyName = "ID_NHA_CUNG_CAP";
//            cmb.HeaderText = "Nhà cung cấp";

//        }

//        public NhaCungCap LayNCC(String id)
//        {
//            DataTable tbl = factory.LayNCC(id);
//            NhaCungCap ncc = new NhaCungCap();
//            if (tbl.Rows.Count > 0)
//            {
//                ncc.Id = Convert.ToString(tbl.Rows[0]["ID"]);
//                ncc.HoTen = Convert.ToString(tbl.Rows[0]["HO_TEN"]);
//                ncc.DienThoai = Convert.ToString(tbl.Rows[0]["DIEN_THOAI"]);
//                ncc.DiaChi = Convert.ToString(tbl.Rows[0]["DIA_CHI"]);
//            }
//            return ncc;
//        }

//        public IList<NhaCungCap> LayDanhSachNCC()
//        {
//            DataTable tbl = factory.DanhsachNCC();
//            IList<NhaCungCap> ds = new List<NhaCungCap>();

//            foreach (DataRow row in tbl.Rows)
//            {
//                NhaCungCap kh = new NhaCungCap();
//                kh.Id = Convert.ToString(row["ID"]);
//                kh.HoTen = Convert.ToString(row["HO_TEN"]);
//                kh.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
//                kh.DiaChi = Convert.ToString(row["DIA_CHI"]);
//                ds.Add(kh);
//            }
//            return ds;
//        }

//        public void TimDiaChi(String diachi)
//        {
//            factory.TimDiaChi(diachi);
//        }
//        public void TimHoTen(String hoten)
//        {
//            factory.TimHoTen(hoten);
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




using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class NhaCungCapController
    {
        // ✅ Dùng interface để tách phụ thuộc — chuẩn Dependency Injection
        private readonly INhaCungCapDAL _dal;

        // ==================== Constructor (Inject) ====================
        public NhaCungCapController(INhaCungCapDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        // ==================== Hiển thị dữ liệu ====================

        public void HienthiAutoComboBox(ComboBox cmb)
        {
            cmb.DataSource = _dal.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }

        public void HienthiAllComboBox(ComboBox cmb)
        {
            IList<NhaCungCap> ds = LayDanhSachNCC();
            ds.Add(new NhaCungCap("ALL", "Tất cả"));
            cmb.DataSource = ds;
            cmb.DisplayMember = "HoTen";
            cmb.ValueMember = "Id";
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            BindingSource bs = new BindingSource();
            DataTable tbl = _dal.DanhsachNCC();
            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiDataGridviewComboBox(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = _dal.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_NHA_CUNG_CAP";
            cmb.HeaderText = "Nhà cung cấp";
        }

        // ==================== Lấy dữ liệu ====================

        public NhaCungCap LayNCC(string id)
        {
            DataTable tbl = _dal.LayNCC(id);
            NhaCungCap ncc = new NhaCungCap();

            if (tbl.Rows.Count > 0)
            {
                DataRow row = tbl.Rows[0];
                ncc.Id = Convert.ToString(row["ID"]);
                ncc.HoTen = Convert.ToString(row["HO_TEN"]);
                ncc.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
                ncc.DiaChi = Convert.ToString(row["DIA_CHI"]);
            }

            return ncc;
        }

        public IList<NhaCungCap> LayDanhSachNCC()
        {
            DataTable tbl = _dal.DanhsachNCC();
            IList<NhaCungCap> ds = new List<NhaCungCap>();

            foreach (DataRow row in tbl.Rows)
            {
                ds.Add(new NhaCungCap
                {
                    Id = Convert.ToString(row["ID"]),
                    HoTen = Convert.ToString(row["HO_TEN"]),
                    DienThoai = Convert.ToString(row["DIEN_THOAI"]),
                    DiaChi = Convert.ToString(row["DIA_CHI"])
                });
            }

            return ds;
        }

        // ==================== Tìm kiếm ====================

        public DataTable TimHoTen(string hoten)
        {
            return string.IsNullOrWhiteSpace(hoten)
                ? _dal.DanhsachNCC()
                : _dal.TimHoTen(hoten);
        }

        public DataTable TimDiaChi(string diachi)
        {
            return string.IsNullOrWhiteSpace(diachi)
                ? _dal.DanhsachNCC()
                : _dal.TimDiaChi(diachi);
        }

        // ==================== CRUD ====================

        public bool Insert(NhaCungCap ncc)
        {
            return _dal.Insert(ncc);
        }

        public bool Update(NhaCungCap ncc)
        {
            return _dal.Update(ncc);
        }

        public bool Delete(string id)
        {
            return _dal.Delete(id);
        }

        public bool Save()
        {
            return _dal.Save();
        }

        // ==================== DataTable pattern ====================

        public DataRow NewRow() => _dal.NewRow();

        public void Add(DataRow row) => _dal.Add(row);
    }
}

