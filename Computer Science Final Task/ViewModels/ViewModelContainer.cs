using Autofac;
using Caching;
using Computer_Science_Final_Task.Models;
using DataAccessLayer;
using GalaSoft.MvvmLight.Views;

namespace Computer_Science_Final_Task.ViewModels
{
    public class ViewModelContainer
    {
        private IContainer Container { get; set; }

        public ViewModelContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<NavigationService>().As<INavigationService>().InstancePerLifetimeScope();
            builder.RegisterType<MainPageViewModel>().AsSelf().InstancePerDependency();
            builder.RegisterType<MainPageModelWithCaching>().As<IMainPageModel>().InstancePerDependency();
            builder.RegisterType<InMemoryCacheProvider>().As<ICacheProvider>().InstancePerLifetimeScope();
            builder.RegisterType<History>().AsSelf().InstancePerDependency();
            builder.RegisterType<FileRepository>().As<IRepository>().InstancePerDependency();
            Container = builder.Build();
        }

        public MainPageViewModel MainPageInstance => Container.Resolve<MainPageViewModel>();
    }
}