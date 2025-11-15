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
            ServiceLocator.Register<IDonViTinhService>(() => new DonViTinhService(new DonViTinhDAL()));
            ServiceLocator.Register<ISanPhamService>(() => new SanPhamService(new SanPhamFactory(), ServiceLocator.Resolve<IDonViTinhService>()));
            ServiceLocator.Register<IMaSanPhamService>(() => new MaSanPhamService(new MaSanPhanFactory(), ServiceLocator.Resolve<ISanPhamService>()));

            ServiceLocator.Register<SanPhamFacade>(() => new SanPhamFacade(
                ServiceLocator.Resolve<ISanPhamService>(),
                ServiceLocator.Resolve<IDonViTinhService>()));

            ServiceLocator.Register<DonViTinhFacade>(() => new DonViTinhFacade(ServiceLocator.Resolve<IDonViTinhService>()));
            ServiceLocator.Register<MaSanPhamFacade>(() => new MaSanPhamFacade(ServiceLocator.Resolve<IMaSanPhamService>()));
        }
    }
}
