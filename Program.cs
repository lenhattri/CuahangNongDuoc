using System;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Interfaces;
using CuahangNongduoc.BLL.Services;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.UI.Facades;
using CuahangNongduoc.Utils;

namespace CuahangNongduoc
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ConfigureServices();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        private static void ConfigureServices()
        {
            ServiceLocator.Register<IUnitService>(() => new UnitService(new DonViTinhDAL()));
            ServiceLocator.Register<IProductService>(() => new ProductService(new SanPhamFactory(), ServiceLocator.Resolve<IUnitService>()));
            ServiceLocator.Register<IProductLotService>(() => new ProductLotService(new MaSanPhanFactory(), ServiceLocator.Resolve<IProductService>()));

            ServiceLocator.Register<ProductFacade>(() => new ProductFacade(
                ServiceLocator.Resolve<IProductService>(),
                ServiceLocator.Resolve<IUnitService>()));

            ServiceLocator.Register<UnitFacade>(() => new UnitFacade(ServiceLocator.Resolve<IUnitService>()));
            ServiceLocator.Register<ProductLotFacade>(() => new ProductLotFacade(ServiceLocator.Resolve<IProductLotService>()));
        }
    }
}
