using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using System.Data.SqlClient;


namespace CuahangNongduoc.Controller
{
    public class NhaCungCapController
    {
        private readonly INhaCungCapDAL _dal;

        // ✅ Cho phép DI hoặc dùng mặc định
        public NhaCungCapController(INhaCungCapDAL dal = null)
        {
            _dal = dal ?? new NhaCungCapDAL();
        }

        public void HienthiAutoComboBox(ComboBox cmb)
        {
            cmb.DataSource = _dal.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            
        }

        public void HienthiAllComboBox(ComboBox cmb)
        {
            IList<NhaCungCap> ds = this.LayDanhSachNCC();
            ds.Add(new NhaCungCap("ALL","Tất cả"));
            cmb.DataSource = ds;
            cmb.DisplayMember = "HoTen";
            cmb.ValueMember = "Id";

        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn)
        {
            var bs = new BindingSource
            {
                DataSource = _dal.DanhsachNCC()
            };
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
        
        public NhaCungCap LayNCC(String id)
        {
            DataTable tbl = _dal.LayNCC(id);
            if (tbl.Rows.Count == 0)
                return null;

            DataRow r = tbl.Rows[0];
            return new NhaCungCap
            {
                Id = Convert.ToString(r["ID"]),
                HoTen = Convert.ToString(r["HO_TEN"]),
                DienThoai = Convert.ToString(r["DIEN_THOAI"]),
                DiaChi = Convert.ToString(r["DIA_CHI"])
            };
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

        public void TimDiaChi(string diachi) => _dal.TimDiaChi(diachi);
        public void TimHoTen(string hoten) => _dal.TimHoTen(hoten);
        public void Insert(NhaCungCap ncc) => _dal.Insert(ncc);
        public void Update(NhaCungCap ncc) => _dal.Update(ncc);
        public void Delete(string id) => _dal.Delete(id);
    }
}

