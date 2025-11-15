using System.Data;
using CuahangNongduoc.BLL.Services.Commands;
using CuahangNongduoc.DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CuahangNongduoc.Tests.SanPham
{
    [TestClass]
    public class UpdateAverageCostCommandTests
    {
        [TestMethod]
        public void Execute_ShouldUpdateAverageCostAndQuantity()
        {
            var repository = new FakeSanPhamFactory();
            var command = new UpdateAverageCostCommand(repository);

            command.Execute("SP01", 4000, 5);

            DataRow row = repository.Table.Rows[0];
            Assert.AreEqual(15L, row["SO_LUONG"]);
            Assert.AreEqual(3000L, row["DON_GIA_NHAP"]);
            Assert.IsTrue(repository.SaveCalled);
        }

        private sealed class FakeSanPhamFactory : ISanPhamFactory
        {
            public FakeSanPhamFactory()
            {
                Table = new DataTable();
                Table.Columns.Add("ID", typeof(string));
                Table.Columns.Add("SO_LUONG", typeof(long));
                Table.Columns.Add("DON_GIA_NHAP", typeof(long));
                var row = Table.NewRow();
                row["ID"] = "SP01";
                row["SO_LUONG"] = 10L;
                row["DON_GIA_NHAP"] = 2000L;
                Table.Rows.Add(row);
            }

            public DataTable Table { get; }
            public bool SaveCalled { get; private set; }

            public void Add(DataRow row) => Table.Rows.Add(row);
            public DataTable DanhsachSanPham() => Table;
            public bool Delete(string id) => false;
            public bool Insert(CuahangNongduoc.BusinessObject.SanPham sp) => false;
            public DataTable LaySanPham(string id) => Table;
            public DataTable LaySoLuongTon() => Table;
            public DataRow NewRow() => Table.NewRow();
            public bool Save()
            {
                SaveCalled = true;
                return true;
            }
            public DataTable TimMaSanPham(string id) => Table;
            public DataTable TimTenSanPham(string ten) => Table;
            public bool Update(CuahangNongduoc.BusinessObject.SanPham sp) => false;
        }
    }
}
