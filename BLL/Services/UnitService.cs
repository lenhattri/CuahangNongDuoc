using System;
using System.Data;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services
{
    public class UnitService : IUnitService
    {
        private readonly IDonViTinhDAL _dal;

        public UnitService(IDonViTinhDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public DataTable GetUnits() => _dal.DanhSachDVT();

        public DonViTinh GetUnit(int id)
        {
            var table = _dal.LayDVT(id);
            if (table == null || table.Rows.Count == 0)
            {
                return null;
            }

            var row = table.Rows[0];
            var nameColumn = ResolveNameColumn(table);
            var unit = new DonViTinh
            {
                Id = Convert.ToInt32(row["ID"]),
                Ten = nameColumn != null ? Convert.ToString(row[nameColumn]) : Convert.ToString(row[1])
            };

            return unit;
        }

        public bool SaveChanges() => _dal.Save();

        private static string ResolveNameColumn(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            if (table.Columns.Contains("TEN_DON_VI"))
            {
                return "TEN_DON_VI";
            }

            if (table.Columns.Contains("TEN"))
            {
                return "TEN";
            }

            if (table.Columns.Count > 1)
            {
                return table.Columns[1].ColumnName;
            }

            return null;
        }
    }
}
