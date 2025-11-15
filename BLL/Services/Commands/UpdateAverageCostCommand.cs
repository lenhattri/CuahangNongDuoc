using System;
using System.Data;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BLL.Services.Commands
{
    public sealed class UpdateAverageCostCommand : IProductAverageCostCommand
    {
        private readonly ISanPhamFactory _repository;

        public UpdateAverageCostCommand(ISanPhamFactory repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Execute(string productId, long newCost, long quantityChange)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                throw new ArgumentException("Product id is required", nameof(productId));
            }

            if (quantityChange <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantityChange), "Quantity change must be positive");
            }

            DataTable table = _repository.LaySanPham(productId);
            if (table.Rows.Count == 0)
            {
                return;
            }

            DataRow row = table.Rows[0];
            long currentQuantity = Convert.ToInt64(row["SO_LUONG"]);
            long currentCost = Convert.ToInt64(row["DON_GIA_NHAP"]);

            if (currentCost == newCost)
            {
                return; // nothing to do
            }

            long totalQuantity = currentQuantity + quantityChange;
            if (totalQuantity <= 0)
            {
                throw new InvalidOperationException("Total quantity must stay positive");
            }

            long totalValue = (currentCost * currentQuantity) + (newCost * quantityChange);
            long averageCost = totalValue / totalQuantity;

            row["SO_LUONG"] = totalQuantity;
            row["DON_GIA_NHAP"] = averageCost;

            _repository.Save();
        }
    }
}
