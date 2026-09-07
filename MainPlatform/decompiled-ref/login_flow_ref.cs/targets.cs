#define DEBUG
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using DbModels;
using LogModule;
using MainPlatform.Common;
using MainPlatform.Models;
using MainPlatform.ViewModels;
using Service.DbService;
using Service.SimulationService;

namespace MainPlatform
{
	public class App : Application
	{
		private bool _contentLoaded;

		public App()
		{
			base.DispatcherUnhandledException += delegate(object s, DispatcherUnhandledExceptionEventArgs e)
			{
				try
				{
					NLogModule.Instance.Error("全局异常", e.Exception?.Message ?? "未知异常", isOutToView: true, e.Exception);
				}
				catch
				{
				}
			};
			AppDomain.CurrentDomain.UnhandledException += delegate(object s, UnhandledExceptionEventArgs e)
			{
				try
				{
					Exception ex = e.ExceptionObject as Exception;
					NLogModule.Instance.Error("全局异常", ex?.Message ?? "未知异常", isOutToView: true, ex);
				}
				catch
				{
				}
			};
			base.Startup += delegate
			{
				try
				{
					NLogModule.Instance.Info("系统", "上位机软件启动");
				}
				catch (Exception ex)
				{
					Debug.WriteLine("日志模块初始化失败: " + ex.Message);
				}
			};
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/MainPlatform;component/app.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
	public class Bootstrapper : BootstrapperBase
	{
		private SimpleContainer _container;

		public Bootstrapper()
		{
			_container = new SimpleContainer();
			Initialize();
		}

		protected override void Configure()
		{
			_container.Instance(_container);
			_container.Singleton<IWindowManager, WindowManager>();
			_container.Singleton<IEventAggregator, EventAggregator>();
			_container.PerRequest<MainWindowViewModel>();
			_container.PerRequest<LoginViewModel>();
			_container.PerRequest<UserDataService>();
			ConfigManager.Instance.Initialize();
		}

		protected override async void OnStartup(object sender, StartupEventArgs e)
		{
			await DisplayRootViewForAsync<LoginViewModel>();
		}

		protected override object GetInstance(Type service, string key)
		{
			return _container.GetInstance(service, key);
		}
	}
}
namespace MainPlatform.Models
{
	public class AppConfig
	{
		public string CurrentMode { get; set; }

		public int YGChanged { get; set; }

		public int ZStartPos { get; set; }
	}
}
namespace MainPlatform.ViewModels
{
	public class LoginViewModel : Screen
	{
		private UserData _currentUser;

		private string _username;

		private string _password;

		private bool _isRemembered;

		private bool _isPasswordVisible;

		private ObservableCollection<UserData> _userList;

		private string _currentMode = "深腔球焊";

		private ObservableCollection<string> _modeList = new ObservableCollection<string> { "深腔楔焊", "浅腔楔焊", "平面楔焊", "深腔球焊", "粗丝" };

		private int _ygChanged = 8;

		private int _zStartPos = 24000;

		public UserData CurrentUser
		{
			get
			{
				return _currentUser;
			}
			set
			{
				_currentUser = value;
				NotifyOfPropertyChange(() => CurrentUser);
			}
		}

		public string Username
		{
			get
			{
				return _username;
			}
			set
			{
				_username = value;
				NotifyOfPropertyChange(() => Username);
			}
		}

		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
			}
		}

		public bool IsRemembered
		{
			get
			{
				return _isRemembered;
			}
			set
			{
				_isRemembered = value;
				NotifyOfPropertyChange(() => IsRemembered);
			}
		}

		public bool IsPasswordVisible
		{
			get
			{
				return _isPasswordVisible;
			}
			set
			{
				_isPasswordVisible = value;
				NotifyOfPropertyChange(() => IsPasswordVisible);
			}
		}

		public ObservableCollection<UserData> UserList
		{
			get
			{
				return _userList;
			}
			set
			{
				_userList = value;
				NotifyOfPropertyChange(() => UserList);
			}
		}

		private IWindowManager _windowManager { get; set; }

		public UserDataService UserDataService { get; set; }

		public string CurrentMode
		{
			get
			{
				return _currentMode;
			}
			set
			{
				_currentMode = value;
				NotifyOfPropertyChange(() => CurrentMode);
			}
		}

		public ObservableCollection<string> ModeList
		{
			get
			{
				return _modeList;
			}
			set
			{
				_modeList = value;
				NotifyOfPropertyChange(() => ModeList);
			}
		}

		public int YgChanged
		{
			get
			{
				return _ygChanged;
			}
			set
			{
				_ygChanged = value;
				NotifyOfPropertyChange(() => YgChanged);
			}
		}

		public int ZStartPos
		{
			get
			{
				return _zStartPos;
			}
			set
			{
				_zStartPos = value;
				NotifyOfPropertyChange(() => ZStartPos);
			}
		}

		public LoginViewModel()
		{
			UserList = new ObservableCollection<UserData>();
			Initialize();
		}

		public void Initialize()
		{
			_windowManager = IoC.Get<IWindowManager>();
			UserDataService = IoC.Get<UserDataService>();
			List<UserData> list = UserDataService.GetAll().ToList();
			UserList.Clear();
			foreach (UserData item in list)
			{
				UserList.Add(item);
			}
			if (UserList.Count <= 0)
			{
				CurrentUser = null;
				Username = string.Empty;
				Password = string.Empty;
				IsRemembered = false;
				MessageBox.Show("请联系开发商为该软件添加一个管理员!!!");
				return;
			}
			List<UserData> list2 = (from u in UserList
				where u.LastLoginTime.HasValue
				orderby u.LastLoginTime descending
				select u).ToList();
			if (list2 != null && list2.Count <= 0)
			{
				CurrentUser = UserList.FirstOrDefault();
			}
			else
			{
				CurrentUser = list2.FirstOrDefault();
			}
			Username = CurrentUser.UserName;
			IsRemembered = CurrentUser.IsRemembered;
			if (IsRemembered)
			{
				Password = CurrentUser.Password;
			}
			else
			{
				Password = string.Empty;
			}
			CurrentMode = ConfigManager.Instance.AppConfig.CurrentMode;
			ZStartPos = ConfigManager.Instance.AppConfig.ZStartPos;
			YgChanged = ConfigManager.Instance.AppConfig.YGChanged;
		}

		public void Register()
		{
			if (CurrentUser == null)
			{
				MessageBox.Show("请联系开发商为该软件添加一个管理员后管理员注册新用户!!!");
			}
			else if (CurrentUser.Identity.Equals(Enum.GetName(typeof(Identity), Identity.Admin)))
			{
				MessageBox.Show("注册功能等待实现！");
			}
			else
			{
				MessageBox.Show("只有管理员才能注册新用户！");
			}
		}

		public void ClearPassWord()
		{
			Password = string.Empty;
		}

		public async Task Login()
		{
			if (CurrentUser == null)
			{
				MessageBox.Show("请联系开发商为该软件添加一个管理员后管理员注册新用户!!!");
			}
			else if (ZStartPos >= 2000 && ZStartPos <= 33500)
			{
				ConfigManager.Instance.AppConfig.ZStartPos = ZStartPos;
				ConfigManager.Instance.AppConfig.CurrentMode = CurrentMode;
				ConfigManager.Instance.AppConfig.YGChanged = YgChanged;
				if (string.Equals(CurrentUser.Password, Password))
				{
					if (CurrentUser.IsRemembered != IsRemembered)
					{
						CurrentUser.IsRemembered = IsRemembered;
						CurrentUser.UpdateTime = DateTime.Now;
					}
					CurrentUser.LastLoginTime = DateTime.Now;
					UserDataService.Update(CurrentUser);
					MainWindowViewModel mainWindowViewModel = IoC.Get<MainWindowViewModel>();
					await _windowManager.ShowWindowAsync(mainWindowViewModel);
					LogSimulationService.Instance.Start();
					await TryCloseAsync();
				}
				else
				{
					ClearPassWord();
					MessageBox.Show("登陆失败！用户名、密码错误或账户未注册！！");
				}
			}
			else
			{
				MessageBox.Show("Z向起点位置必须在2000um到33500um之间！");
			}
		}
	}
	public class MainWindowViewModel : Screen
	{
		private readonly Dictionary<string, Screen> _pageCache = new Dictionary<string, Screen>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, SubMenu> _pageMap = new Dictionary<string, SubMenu>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, Menu> _groupMap = new Dictionary<string, Menu>(StringComparer.OrdinalIgnoreCase);

		private readonly List<string> _history = new List<string>();

		private int _historyIndex = -1;

		private readonly IWindowManager _windowManager;

		private readonly DispatcherTimer _clockTimer;

		private string _appTitle = string.Empty;

		private string _appSubTitle = string.Empty;

		private Menu _currentMenu;

		private TabPage _currentTab;

		private int _menuCount;

		private int _subMeanuCount;

		private string _currentPageTitle = string.Empty;

		private string _currentPagePath = string.Empty;

		private string _currentTime = string.Empty;

		public ObservableCollection<Menu> Menus { get; set; } = new ObservableCollection<Menu>();

		public ObservableCollection<TabPage> Pages { get; } = new ObservableCollection<TabPage>();

		public string AppTitle
		{
			get
			{
				return _appTitle;
			}
			private set
			{
				_appTitle = value;
				NotifyOfPropertyChange(() => AppTitle);
			}
		}

		public string AppSubTitle
		{
			get
			{
				return _appSubTitle;
			}
			private set
			{
				_appSubTitle = value;
				NotifyOfPropertyChange(() => AppSubTitle);
			}
		}

		public Menu CurrentMenu
		{
			get
			{
				return _currentMenu;
			}
			private set
			{
				_currentMenu = value;
				NotifyOfPropertyChange(() => CurrentMenu);
			}
		}

		public TabPage CurrentTab
		{
			get
			{
				return _currentTab;
			}
			set
			{
				if (_currentTab != value)
				{
					_currentTab = value;
					NotifyOfPropertyChange(() => CurrentTab);
					if (_currentTab != null)
					{
						ActivateTab(_currentTab.Menu, recordHistory: true);
					}
				}
			}
		}

		public int MenuCount
		{
			get
			{
				return _menuCount;
			}
			set
			{
				_menuCount = value;
				NotifyOfPropertyChange(() => MenuCount);
			}
		}

		public int SubMeanuCount
		{
			get
			{
				return _subMeanuCount;
			}
			set
			{
				_subMeanuCount = value;
				NotifyOfPropertyChange("SubMeanuCount");
			}
		}

		public string CurrentPageTitle
		{
			get
			{
				return _currentPageTitle;
			}
			private set
			{
				_currentPageTitle = value;
				NotifyOfPropertyChange(() => CurrentPageTitle);
			}
		}

		public string CurrentPagePath
		{
			get
			{
				return _currentPagePath;
			}
			private set
			{
				_currentPagePath = value;
				NotifyOfPropertyChange(() => CurrentPagePath);
			}
		}

		public string CurrentTime
		{
			get
			{
				return _currentTime;
			}
			private set
			{
				_currentTime = value;
				NotifyOfPropertyChange(() => CurrentTime);
			}
		}

		public UserData CurrentUser => IoC.Get<LoginViewModel>()?.CurrentUser;

		public string DisplayUserName => CurrentUser?.UserName ?? "未登录";

		public string RecipeName { get; set; } = "wsd";

		public string SequenceNo { get; set; } = "1";

		public string MachineX { get; set; } = "0.000";

		public string MachineY { get; set; } = "0.000";

		public string MachineZ { get; set; } = "0.000";

		public bool CanGoBack => _historyIndex > 0;

		public bool CanGoForward => _historyIndex >= 0 && _historyIndex < _history.Count - 1;

		public MainWindowViewModel()
		{
			_windowManager = IoC.Get<IWindowManager>();
			_clockTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.0)
			};
			_clockTimer.Tick += delegate
			{
				CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			};
			_clockTimer.Start();
			CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			LoadConfiguration();
			if (Menus.Count > 0)
			{
				SelectMenu(Menus[0]);
			}
		}

		private void LoadConfiguration()
		{
			UiConfig uiConfig = ConfigManager.Instance.UiConfig;
			string text = ConfigManager.Instance.AppConfig?.CurrentMode;
			if (uiConfig == null)
			{
				MessageBox.Show("Ui配置文件加载失败!");
			}
			AppTitle = (string.IsNullOrWhiteSpace(uiConfig.AppTitle) ? "上位机控制平台" : uiConfig.AppTitle);
			AppSubTitle = uiConfig.AppSubTitle ?? string.Empty;
			MenuCount = uiConfig.MenuList.Count;
			if (text.Contains("球焊") || text.Contains("粗丝"))
			{
				foreach (Menu menu in uiConfig.MenuList)
				{
					if (menu.Key.ToLower().Contains("process"))
					{
						menu.SubMenuList = menu.SubMenuList.Where((SubMenu p) => p.Key.ToLower().Contains("qh")).ToList();
					}
				}
			}
			else
			{
				foreach (Menu menu2 in uiConfig.MenuList)
				{
					if (menu2.Key.ToLower().Contains("process"))
					{
						menu2.SubMenuList = menu2.SubMenuList.Where((SubMenu p) => p.Key.ToLower().Contains("xh")).ToList();
					}
				}
			}
			foreach (Menu menu3 in uiConfig.MenuList)
			{
				_groupMap[menu3.Key] = menu3;
				Menus.Add(menu3);
				foreach (SubMenu subMenu in menu3.SubMenuList)
				{
					_pageMap[subMenu.FullKey] = subMenu;
				}
			}
		}

		public void SelectMenu(Menu menu)
		{
			if (menu == null || CurrentMenu == menu)
			{
				return;
			}
			CurrentMenu = menu;
			foreach (Menu menu2 in Menus)
			{
				menu2.IsSelected = menu2 == menu;
			}
			Pages.Clear();
			if (CurrentMenu.SubMenuList != null)
			{
				SubMeanuCount = CurrentMenu.SubMenuList.Count;
			}
			foreach (SubMenu subMenu in menu.SubMenuList)
			{
				Screen orCreatePageViewModel = GetOrCreatePageViewModel(subMenu);
				if (orCreatePageViewModel != null)
				{
					Pages.Add(new TabPage
					{
						Menu = subMenu,
						Screen = orCreatePageViewModel
					});
				}
			}
			if (Pages.Count > 0)
			{
				_currentTab = null;
				CurrentTab = Pages[0];
			}
			UpdatePath();
		}

		public void SelectTab(TabPage tab)
		{
			if (tab != null)
			{
				CurrentTab = tab;
			}
		}

		private void ActivateTab(SubMenu sub, bool recordHistory)
		{
			foreach (SubMenu item in Menus.SelectMany((Menu g) => g.SubMenuList))
			{
				item.IsSelected = item.FullKey == sub.FullKey;
			}
			CurrentPageTitle = sub.DisplayName ?? sub.Key;
			UpdatePath();
			if (recordHistory)
			{
				PushHistory(sub.FullKey);
			}
			NotifyOfPropertyChange(() => CanGoBack);
			NotifyOfPropertyChange(() => CanGoForward);
		}

		private void UpdatePath()
		{
			if (CurrentTab != null && _groupMap.TryGetValue(CurrentTab.Menu.ParentKey ?? string.Empty, out var value))
			{
				CurrentPagePath = value.DisplayName + " / " + CurrentTab.Menu.DisplayName;
			}
			else if (CurrentMenu != null)
			{
				CurrentPagePath = CurrentMenu.DisplayName;
			}
			else
			{
				CurrentPagePath = string.Empty;
			}
		}

		private void PushHistory(string fullKey)
		{
			if (_historyIndex < 0 || _historyIndex >= _history.Count || !(_history[_historyIndex] == fullKey))
			{
				if (_historyIndex < _history.Count - 1)
				{
					_history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);
				}
				_history.Add(fullKey);
				_historyIndex = _history.Count - 1;
			}
		}

		public Task GoBackAsync()
		{
			if (!CanGoBack)
			{
				return Task.CompletedTask;
			}
			_historyIndex--;
			NavigateToHistory(_history[_historyIndex]);
			return Task.CompletedTask;
		}

		public Task GoForwardAsync()
		{
			if (!CanGoForward)
			{
				return Task.CompletedTask;
			}
			_historyIndex++;
			NavigateToHistory(_history[_historyIndex]);
			return Task.CompletedTask;
		}

		private void NavigateToHistory(string fullKey)
		{
			if (!_pageMap.TryGetValue(fullKey, out var value) || !_groupMap.TryGetValue(value.ParentKey ?? string.Empty, out var value2))
			{
				return;
			}
			CurrentMenu = value2;
			foreach (Menu menu in Menus)
			{
				menu.IsSelected = menu == value2;
			}
			Pages.Clear();
			foreach (SubMenu subMenu in value2.SubMenuList)
			{
				Screen orCreatePageViewModel = GetOrCreatePageViewModel(subMenu);
				if (orCreatePageViewModel != null)
				{
					Pages.Add(new TabPage
					{
						Menu = subMenu,
						Screen = orCreatePageViewModel
					});
				}
			}
			TabPage tabPage = Pages.FirstOrDefault((TabPage p) => p.Menu.FullKey == fullKey);
			if (tabPage != null)
			{
				_currentTab = tabPage;
				NotifyOfPropertyChange(() => CurrentTab);
				ActivateTab(tabPage.Menu, recordHistory: false);
			}
			UpdatePath();
			NotifyOfPropertyChange(() => CanGoBack);
			NotifyOfPropertyChange(() => CanGoForward);
		}

		public void OpenRecipe()
		{
			Notify("打开程序功能开发中");
		}

		public void SaveRecipeAs()
		{
			Notify("另存程序功能开发中");
		}

		public void ShowStatistics()
		{
			Notify("生产统计功能开发中");
		}

		private static void Notify(string msg)
		{
			MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}

		public async Task LogoutAsync()
		{
			MessageBoxResult result = MessageBox.Show("确定要退出登录吗？", "退出登录", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				ConfigManager.Instance.LoadUiConfig();
				LogSimulationService.Instance.Stop();
				LoginViewModel loginViewModel = IoC.Get<LoginViewModel>();
				await _windowManager.ShowWindowAsync(loginViewModel);
				await TryCloseAsync();
			}
		}

		private Screen GetOrCreatePageViewModel(SubMenu page)
		{
			if (_pageCache.TryGetValue(page.FullKey, out var value))
			{
				return value;
			}
			Type type = ResolveViewModelType(page.ViewModel);
			Screen screen = null;
			try
			{
				screen = (Screen)Activator.CreateInstance(type);
			}
			catch
			{
			}
			if (screen != null)
			{
				_pageCache[page.FullKey] = screen;
			}
			return screen;
		}

		private static Type ResolveViewModelType(string typeName)
		{
			if (string.IsNullOrWhiteSpace(typeName))
			{
				return null;
			}
			return Type.GetType(typeName, throwOnError: false) ?? Assembly.GetExecutingAssembly().GetType(typeName, throwOnError: false, ignoreCase: true);
		}
	}
}
