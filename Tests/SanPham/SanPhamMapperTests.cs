using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Utils.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CuahangNongduoc.Tests.SanPham
{
    [TestClass]
    public class SanPhamMapperTests
    {
        [TestMethod]
        public void Map_ShouldPopulateDomainObjectFromDataRow()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("TEN_SAN_PHAM", typeof(string));
            table.Columns.Add("SO_LUONG", typeof(int));
            table.Columns.Add("DON_GIA_NHAP", typeof(long));
            table.Columns.Add("GIA_BAN_LE", typeof(long));
            table.Columns.Add("GIA_BAN_SI", typeof(long));
            table.Columns.Add("ID_DON_VI_TINH", typeof(int));

            var row = table.NewRow();
            row["ID"] = "SP01";
            row["TEN_SAN_PHAM"] = "Thuoc test";
            row["SO_LUONG"] = 10;
            row["DON_GIA_NHAP"] = 2000L;
            row["GIA_BAN_LE"] = 2500L;
            row["GIA_BAN_SI"] = 2300L;
            row["ID_DON_VI_TINH"] = 1;
            table.Rows.Add(row);

            var mapper = new SanPhamMapper(id => new DonViTinh(id, "Hop"));

            SanPham result = mapper.Map(row);

            Assert.AreEqual("SP01", result.Id);
            Assert.AreEqual("Thuoc test", result.TenSanPham);
            Assert.AreEqual(10, result.SoLuong);
            Assert.AreEqual(2000L, result.DonGiaNhap);
            Assert.AreEqual(2500L, result.GiaBanLe);
            Assert.AreEqual(2300L, result.GiaBanSi);
            Assert.IsNotNull(result.DonViTinh);
            Assert.AreEqual("Hop", result.DonViTinh.Ten);
        }
    }
}
