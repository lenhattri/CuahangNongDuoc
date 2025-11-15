using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BLL.Services.Commands;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.Utils.Binding;
using CuahangNongduoc.Utils.Mapping;

namespace CuahangNongduoc.Controller
{
    public class SanPhamController : ISanPhamService
    {
        private readonly ISanPhamFactory _dal;
        private readonly IDataRowMapper<SanPham> _sanPhamMapper;
        private readonly IProductAverageCostCommand _averageCostCommand;

        public SanPhamController(ISanPhamFactory dal)
            : this(dal, new DonViTinhController(new DonViTinhDAL()))
        {
        }

        internal SanPhamController(
            ISanPhamFactory dal,
            DonViTinhController donViTinhController)
            : this(
                dal,
                donViTinhController,
                new SanPhamMapper(id => donViTinhController.LayDVT(id)),
                new UpdateAverageCostCommand(dal))
        {
        }

        internal SanPhamController(
            ISanPhamFactory dal,
            DonViTinhController donViTinhController,
            IDataRowMapper<SanPham> sanPhamMapper,
            IProductAverageCostCommand averageCostCommand)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _ = donViTinhController ?? throw new ArgumentNullException(nameof(donViTinhController));
            _sanPhamMapper = sanPhamMapper ?? throw new ArgumentNullException(nameof(sanPhamMapper));
            _averageCostCommand = averageCostCommand ?? throw new ArgumentNullException(nameof(averageCostCommand));
        }

        public DataTable LoadAll()
        {
            return _dal.DanhsachSanPham();
        }

        public DataTable GetProducts()
        {
            return LoadAll();
        }

        public DataTable FindByIdLike(string id)
        {
            return _dal.TimMaSanPham(id);
        }

        public DataTable FindByCode(string code)
        {
            return FindByIdLike(code);
        }

        public DataTable FindByNameLike(string name)
        {
            return _dal.TimTenSanPham(name);
        }

        public DataTable FindByName(string name)
        {
            return FindByNameLike(name);
        }

        public void HienthiAutoComboBox(ComboBox cmb)
        {
            DataTable tbl = LoadAll();
            cmb.DataSource = tbl;
            cmb.DisplayMember = "TEN_SAN_PHAM";
            cmb.ValueMember = "ID";
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public void HienthiDataGridViewComboBoxColumn(DataGridViewComboBoxColumn cmb)
        {
            cmb.DataSource = LoadAll();
            cmb.DisplayMember = "TEN_SAN_PHAM";
            cmb.ValueMember = "ID";
            cmb.AutoComplete = true;
        }

        public DataTable TimMaSanPham(string ma)
        {
            return FindByIdLike(ma);
        }

        public DataTable TimTenSanPham(string ten)
        {
            return FindByNameLike(ten);
        }

        public void HienthiDataGridview(DataGridView dg, BindingNavigator bn,
            TextBox txtMaSp, TextBox txtTenSp, ComboBox cmbDVT, NumericUpDown numSL,
            NumericUpDown numDonGiaNhap, NumericUpDown numGiaBanSi, NumericUpDown numGiaBanLe)
        {
            var bindingSource = new BindingSource
            {
                DataSource = LoadAll()
            };

            var builder = new ControlBindingBuilder(bindingSource);
            builder.BindText(txtMaSp, "ID")
                   .BindText(txtTenSp, "TEN_SAN_PHAM")
                   .BindSelectedValue(cmbDVT, "ID_DON_VI_TINH")
                   .BindValue(numSL, "SO_LUONG")
                   .BindValue(numDonGiaNhap, "DON_GIA_NHAP")
                   .BindValue(numGiaBanSi, "GIA_BAN_SI")
                   .BindValue(numGiaBanLe, "GIA_BAN_LE");

            if (bn != null)
            {
                bn.BindingSource = bindingSource;
            }

            dg.DataSource = bindingSource;
        }

        public void CapNhatGiaNhap(string id, long giaMoi, long soLuong)
        {
            _averageCostCommand.Execute(id, giaMoi, soLuong);
        }

        public void UpdatePurchasePrice(string id, long newPrice, long quantity)
        {
            CapNhatGiaNhap(id, newPrice, quantity);
        }

        public SanPham LaySanPham(string id)
        {
            DataTable tbl = _dal.LaySanPham(id);
            if (tbl.Rows.Count == 0)
            {
                return null;
            }

            return _sanPhamMapper.Map(tbl.Rows[0]);
        }

        public SanPham GetProduct(string id)
        {
            return LaySanPham(id);
        }

        public IList<SoLuongTon> GetInventory()
        {
            DataTable tbl = _dal.LaySoLuongTon();
            IList<SoLuongTon> ds = new List<SoLuongTon>();

            foreach (DataRow row in tbl.Rows)
            {
                var sanPham = _sanPhamMapper.Map(row);
                SoLuongTon slt = new SoLuongTon
                {
                    SanPham = sanPham,
                    SoLuong = Convert.ToInt32(row["SO_LUONG_TON"])
                };
                ds.Add(slt);
            }

            return ds;
        }

        public IList<SoLuongTon> GetInventoryLevels()
        {
            return GetInventory();
        }

        public static IList<SoLuongTon> LaySoLuongTon()
        {
            var controller = new SanPhamController(new SanPhamFactory());
            return controller.GetInventory();
        }

        public DataRow NewRow()
        {
            return _dal.NewRow();
        }

        public void Add(DataRow row)
        {
            _dal.Add(row);
        }

        public bool Save()
        {
            return _dal.Save();
        }

        public DataRow CreateRow()
        {
            return NewRow();
        }

        public void AddRow(DataRow row)
        {
            Add(row);
        }

        public bool SaveChanges()
        {
            return Save();
        }

        public SanPham GetById(string id)
        {
            return LaySanPham(id);
        }

        public void UpdateAverageCost(string productId, long newCost, long quantityChange)
        {
            CapNhatGiaNhap(productId, newCost, quantityChange);
        }

    }
}
