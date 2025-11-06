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
    public class SanPhamController
    {
        private readonly ISanPhamFactory factory;

        public SanPhamController()
        {
            factory = new SanPhamFactory(); // dùng interface
        }

        /* ==================== Hiển thị ==================== */

        public void HienthiAutoComboBox(ComboBox cmb)
        {
            DataTable tbl = factory.DanhsachSanPham();
            cmb.DataSource = tbl;
            cmb.DisplayMember = "TEN_SAN_PHAM";
            cmb.ValueMember = "ID";
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public void HienthiDataGridViewComboBoxColumn(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = factory.DanhsachSanPham();
            cmb.DisplayMember = "TEN_SAN_PHAM";
            cmb.ValueMember = "ID";
            cmb.AutoComplete = true;
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn,
            TextBox txtMaSp, TextBox txtTenSp, ComboBox cmbDVT,
            NumericUpDown numSL, NumericUpDown numDonGiaNhap, NumericUpDown numGiaBanSi, NumericUpDown numGiaBanLe)
        {
            BindingSource bs = new BindingSource
            {
                DataSource = factory.DanhsachSanPham()
            };

            txtMaSp.DataBindings.Clear();
            txtMaSp.DataBindings.Add("Text", bs, "ID");

            txtTenSp.DataBindings.Clear();
            txtTenSp.DataBindings.Add("Text", bs, "TEN_SAN_PHAM");

            cmbDVT.DataBindings.Clear();
            cmbDVT.DataBindings.Add("SelectedValue", bs, "ID_DON_VI_TINH");

            numSL.DataBindings.Clear();
            numSL.DataBindings.Add("Value", bs, "SO_LUONG");

            numDonGiaNhap.DataBindings.Clear();
            numDonGiaNhap.DataBindings.Add("Value", bs, "DON_GIA_NHAP");

            numGiaBanSi.DataBindings.Clear();
            numGiaBanSi.DataBindings.Add("Value", bs, "GIA_BAN_SI");

            numGiaBanLe.DataBindings.Clear();
            numGiaBanLe.DataBindings.Add("Value", bs, "GIA_BAN_LE");

            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        /* ==================== CRUD ==================== */

        public bool ThemSanPham(SanPham sp) => factory.Insert(sp);

        public bool CapNhatSanPham(SanPham sp) => factory.Update(sp);

        public bool XoaSanPham(string id) => factory.Delete(id);

        public DataRow NewRow() => factory.NewRow();

        public void Add(DataRow row) => factory.Add(row);

        public bool Save() => factory.Save();

        /* ==================== Tìm kiếm ==================== */

        public DataTable TimMaSanPham(string ma) => factory.TimMaSanPham(ma);

        public DataTable TimTenSanPham(string ten) => factory.TimTenSanPham(ten);

        /* ==================== Logic nghiệp vụ ==================== */

        public void CapNhatGiaNhap(string id, long giaMoi, long soLuong)
        {
            DataTable tbl = factory.LaySanPham(id);
            if (tbl.Rows.Count == 0) return;

            long tongSo = Convert.ToInt32(tbl.Rows[0]["SO_LUONG"]);
            long tongGia = Convert.ToInt64(tbl.Rows[0]["DON_GIA_NHAP"]);

            if (tongGia != giaMoi)
            {
                long thanhTien = giaMoi * soLuong + tongGia * tongSo;
                tongSo += soLuong;
                tbl.Rows[0]["DON_GIA_NHAP"] = thanhTien / tongSo;
                tbl.Rows[0]["SO_LUONG"] = tongSo;
                factory.Save();
            }
        }

        public SanPham LaySanPham(string id)
        {
            DataTable tbl = factory.LaySanPham(id);
            SanPham sp = new SanPham();
            DonViTinhController ctrlDVT = new DonViTinhController();

            if (tbl.Rows.Count > 0)
            {
                sp.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                sp.TenSanPham = Convert.ToString(tbl.Rows[0]["TEN_SAN_PHAM"]);
                sp.SoLuong = Convert.ToInt32(tbl.Rows[0]["SO_LUONG"]);
                sp.DonGiaNhap = Convert.ToInt64(tbl.Rows[0]["DON_GIA_NHAP"]);
                sp.GiaBanLe = Convert.ToInt64(tbl.Rows[0]["GIA_BAN_LE"]);
                sp.GiaBanSi = Convert.ToInt64(tbl.Rows[0]["GIA_BAN_SI"]);
                sp.DonViTinh = ctrlDVT.LayDVT(Convert.ToInt32(tbl.Rows[0]["ID_DON_VI_TINH"]));
            }
            return sp;
        }

        public static IList<SoLuongTon> LaySoLuongTon()
        {
            SanPhamFactory f = new SanPhamFactory();
            DataTable tbl = f.LaySoLuongTon();
            IList<SoLuongTon> ds = new List<SoLuongTon>();

            DonViTinhController ctrlDVT = new DonViTinhController();
            foreach (DataRow row in tbl.Rows)
            {
                SoLuongTon slt = new SoLuongTon();
                SanPham sp = new SanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    TenSanPham = Convert.ToString(row["TEN_SAN_PHAM"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    DonGiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    GiaBanLe = Convert.ToInt64(row["GIA_BAN_LE"]),
                    GiaBanSi = Convert.ToInt64(row["GIA_BAN_SI"]),
                    DonViTinh = ctrlDVT.LayDVT(Convert.ToInt32(row["ID_DON_VI_TINH"]))
                };
                slt.SanPham = sp;
                slt.SoLuong = Convert.ToInt32(row["SO_LUONG_TON"]);
                ds.Add(slt);
            }
            return ds;
        }
    }
}
