using Autofac;
using Computer_Science_Final_Task.Models;
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
            builder.RegisterType<MainPageModel>().As<IMainPageModel>().InstancePerDependency();
            Container = builder.Build();
        }

        public MainPageViewModel MainPageInstance => Container.Resolve<MainPageViewModel>();
    }
}