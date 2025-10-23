//using System;
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
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class NhaCungCapController
    {
        NhaCungCapDAL dal = new NhaCungCapDAL();

        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            cmb.DataSource = dal.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }

        public void HienthiAllComboBox(System.Windows.Forms.ComboBox cmb)
        {
            IList<BusinessObject.NhaCungCap> ds = this.LayDanhSachNCC();
            ds.Add(new BusinessObject.NhaCungCap("ALL", "Tất cả"));
            cmb.DataSource = ds;
            cmb.DisplayMember = "HoTen";
            cmb.ValueMember = "Id";
        }

        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
            DataTable tbl = dal.DanhsachNCC();
            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = dal.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_NHA_CUNG_CAP";
            cmb.HeaderText = "Nhà cung cấp";
        }

        public BusinessObject.NhaCungCap LayNCC(string id)
        {
            DataTable tbl = dal.LayNCC(id);
            BusinessObject.NhaCungCap ncc = new BusinessObject.NhaCungCap();
            if (tbl.Rows.Count > 0)
            {
                ncc.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                ncc.HoTen = Convert.ToString(tbl.Rows[0]["HO_TEN"]);
                ncc.DienThoai = Convert.ToString(tbl.Rows[0]["DIEN_THOAI"]);
                ncc.DiaChi = Convert.ToString(tbl.Rows[0]["DIA_CHI"]);
            }
            return ncc;
        }

        public IList<BusinessObject.NhaCungCap> LayDanhSachNCC()
        {
            DataTable tbl = dal.DanhsachNCC();
            IList<BusinessObject.NhaCungCap> ds = new List<BusinessObject.NhaCungCap>();

            foreach (DataRow row in tbl.Rows)
            {
                BusinessObject.NhaCungCap kh = new BusinessObject.NhaCungCap();
                kh.Id = Convert.ToString(row["ID"]);
                kh.HoTen = Convert.ToString(row["HO_TEN"]);
                kh.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(row["DIA_CHI"]);
                ds.Add(kh);
            }
            return ds;
        }

        public void TimDiaChi(string diachi)
        {
            dal.TimDiaChi(diachi);
        }

        public void TimHoTen(string hoten)
        {
            dal.TimHoTen(hoten);
        }

        public void Insert(BusinessObject.NhaCungCap ncc)
        {
            dal.Insert(ncc);
        }

        public void Update(BusinessObject.NhaCungCap ncc)
        {
            dal.Update(ncc);
        }

        public void Delete(string id)
        {
            dal.Delete(id);
        }
    }
}
