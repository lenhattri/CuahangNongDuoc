namespace CuahangNongduoc.BLL.Services.Commands
{
    public interface IProductAverageCostCommand
    {
        void Execute(string productId, long newCost, long quantityChange);
    }
}
