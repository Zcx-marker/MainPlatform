using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using MainPlatform.Common;
using MainPlatform.ViewModels;
using Service;
using Service.ActualService;
using Service.DbService;
using Service.MotionControl;
using Service.SimulationService;

namespace MainPlatform
{
    /// <summary>
    /// 引导程序：统一注册窗口管理器、事件聚合器、页面 VM 与各类服务。
    /// 注册均收敛于此；服务按 IsSimulationMode 分流（模拟 / 真实）。
    /// 注：IMotionController 以实例注册（Dmc3000Controller 仅有 cardNo 构造、无无参构造），
    /// 供 ActualLockService / ActualForceService 的无参构造经 IoC.Get 解析。
    /// </summary>
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.PerRequest<MainWindowViewModel>();
            _container.PerRequest<LoginViewModel>();
            _container.PerRequest<CalibrationTestViewModel>();
            _container.PerRequest<UserDataService>();

            ConfigManager.Instance.Initialize();

            if (IsSimulationMode)
            {
                _container.Singleton<IBaseDispService, SimDispatcherService>();
                _container.Singleton<ILock, SimLockService>();
                _container.Singleton<IForce, SimForceService>();
                _container.Singleton<IUltra, SimUltraService>();
                _container.Instance<IMotionController>(new SimController());
            }
            else
            {
                _container.Singleton<IBaseDispService, ActualDispatcherService>();
                _container.Singleton<ILock, ActualLockService>();
                _container.Singleton<IForce, ActualForceService>();
                _container.Singleton<IUltra, ActualUltraService>();
                _container.Instance<IMotionController>(new Dmc3000Controller(0));
            }
        }

        /// <summary>
        /// 是否模拟模式（读取 Config/AppConfig.json 的 IsSimulationMode，需重启生效）。
        /// </summary>
        private bool IsSimulationMode
        {
            get
            {
                try
                {
                    return ConfigManager.Instance.AppConfig.IsSimulationMode;
                }
                catch
                {
                    return false;
                }
            }
        }

        protected override object GetInstance(System.Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(System.Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<LoginViewModel>();
        }
    }
}
