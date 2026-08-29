using Caliburn.Micro;
using MainPlatform.ViewModels;
using Service.DbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainPlatform
{
    public class Bootstrapper: BootstrapperBase
    {
        private SimpleContainer _container;

        public Bootstrapper()
        {
            _container = new SimpleContainer();
            Initialize();
            //LiveCharts.Configure(static config => config.DisableAnimations());
        }

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<MainWindowViewModel>();
            _container.Singleton<LoginViewModel>();
            _container.Singleton<UserDataService>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            // 使用 await 调用异步方法
            await DisplayRootViewForAsync<ViewModels.MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }
    }
}
