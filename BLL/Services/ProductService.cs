using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly ISanPhamFactory _factory;
        private readonly IUnitService _unitService;

        public ProductService(ISanPhamFactory factory, IUnitService unitService)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));
        }

        public DataTable GetProducts() => _factory.DanhsachSanPham();

        public DataTable FindByCode(string code) => _factory.TimMaSanPham(code);

        public DataTable FindByName(string name) => _factory.TimTenSanPham(name);

        public SanPham GetProduct(string id)
        {
            var table = _factory.LaySanPham(id);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            var product = new SanPham
            {
                Id = Convert.ToString(row["ID"]),
                TenSanPham = Convert.ToString(row["TEN_SAN_PHAM"]),
                SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                DonGiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                GiaBanLe = Convert.ToInt64(row["GIA_BAN_LE"]),
                GiaBanSi = Convert.ToInt64(row["GIA_BAN_SI"])
            };

            var unitId = row.Table.Columns.Contains("ID_DON_VI_TINH")
                ? Convert.ToInt32(row["ID_DON_VI_TINH"])
                : 0;
            if (unitId > 0)
            {
                product.DonViTinh = _unitService.GetUnit(unitId);
            }

            return product;
        }

        public IList<SoLuongTon> GetInventoryLevels()
        {
            var table = _factory.LaySoLuongTon();
            var result = new List<SoLuongTon>();

            foreach (DataRow row in table.Rows)
            {
                var product = new SanPham
                {
                    Id = Convert.ToString(row["ID"]),
                    TenSanPham = Convert.ToString(row["TEN_SAN_PHAM"]),
                    SoLuong = Convert.ToInt32(row["SO_LUONG"]),
                    DonGiaNhap = Convert.ToInt64(row["DON_GIA_NHAP"]),
                    GiaBanLe = Convert.ToInt64(row["GIA_BAN_LE"]),
                    GiaBanSi = Convert.ToInt64(row["GIA_BAN_SI"])
                };

                if (row.Table.Columns.Contains("ID_DON_VI_TINH"))
                {
                    product.DonViTinh = _unitService.GetUnit(Convert.ToInt32(row["ID_DON_VI_TINH"]));
                }

                result.Add(new SoLuongTon
                {
                    SanPham = product,
                    SoLuong = Convert.ToInt32(row["SO_LUONG_TON"])
                });
            }

            return result;
        }

        public void UpdatePurchasePrice(string id, long newPrice, long quantity)
        {
            var table = _factory.LaySanPham(id);
            if (table.Rows.Count == 0)
            {
                return;
            }

            var row = table.Rows[0];
            long currentQuantity = Convert.ToInt64(row["SO_LUONG"]);
            long currentPrice = Convert.ToInt64(row["DON_GIA_NHAP"]);

            if (currentPrice == newPrice)
            {
                return;
            }

            long totalValue = newPrice * quantity + currentPrice * currentQuantity;
            long totalQuantity = currentQuantity + quantity;

            if (totalQuantity <= 0)
            {
                return;
            }

            row["DON_GIA_NHAP"] = totalValue / totalQuantity;
            row["SO_LUONG"] = totalQuantity;

            _factory.Save();
        }

        public DataRow CreateRow() => _factory.NewRow();

        public void Add(DataRow row) => _factory.Add(row);

        public bool SaveChanges() => _factory.Save();
    }
}
