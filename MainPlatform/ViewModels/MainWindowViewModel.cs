using Caliburn.Micro;
using DbModels;
using MainPlatform.Common;
using MainPlatform.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MainPlatform.ViewModels
{
    /// <summary>
    /// 页签项：一个二级页面（SubMenu + 对应 Screen 实例）
    /// </summary>
    public class TabPage
    {
        public SubMenu Menu { get; set; }
        public Screen Screen { get; set; }
        public string Header => Menu?.DisplayName ?? string.Empty;
        public string IconGlyph => Menu?.IconGlyph ?? "\uE8A9";
    }

    /// <summary>
    /// 主界面 ViewModel：按竞品 HMI 布局 —— 左侧固定一级导航，内容区用底部页签切换二级页面。
    /// 自动读取 UiConfig.json 渲染导航，支持页面缓存与前进/后退历史。
    /// </summary>
    public class MainWindowViewModel : Screen
    {
        private readonly Dictionary<string, Screen> _pageCache = new Dictionary<string, Screen>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, SubMenu> _pageMap = new Dictionary<string, SubMenu>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Menu> _groupMap = new Dictionary<string, Menu>(StringComparer.OrdinalIgnoreCase);

        private readonly List<string> _history = new List<string>();
        private int _historyIndex = -1;

        // 窗口管理器，用于弹出其他窗口
        private readonly IWindowManager _windowManager;
        // 定时器，用于更新当前时间显示
        private readonly DispatcherTimer _clockTimer;

        public ObservableCollection<Menu> Menus { get; set; } = new ObservableCollection<Menu>();

        /// <summary>当前一级菜单下的页签集合</summary>
        public ObservableCollection<TabPage> Pages { get; } = new ObservableCollection<TabPage>();

        private string _appTitle = string.Empty;
        public string AppTitle
        {
            get => _appTitle;
            private set { _appTitle = value; NotifyOfPropertyChange(() => AppTitle); }
        }

        private string _appSubTitle = string.Empty;
        public string AppSubTitle
        {
            get => _appSubTitle;
            private set { _appSubTitle = value; NotifyOfPropertyChange(() => AppSubTitle); }
        }

        private Menu _currentMenu;
        /// <summary>当前选中的一级菜单</summary>
        public Menu CurrentMenu
        {
            get => _currentMenu;
            private set { _currentMenu = value; NotifyOfPropertyChange(() => CurrentMenu); }
        }

        private TabPage _currentTab;
        /// <summary>当前选中的页签</summary>
        public TabPage CurrentTab
        {
            get => _currentTab;
            set
            {
                if (ReferenceEquals(_currentTab, value)) return;
                _currentTab = value;
                NotifyOfPropertyChange(() => CurrentTab);
                if (_currentTab != null)
                    ActivateTab(_currentTab.Menu, true);
            }
        }

        private string _currentPageTitle = string.Empty;
        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            private set { _currentPageTitle = value; NotifyOfPropertyChange(() => CurrentPageTitle); }
        }

        private string _currentPagePath = string.Empty;
        public string CurrentPagePath
        {
            get => _currentPagePath;
            private set { _currentPagePath = value; NotifyOfPropertyChange(() => CurrentPagePath); }
        }

        private string _currentTime = string.Empty;
        public string CurrentTime
        {
            get => _currentTime;
            private set { _currentTime = value; NotifyOfPropertyChange(() => CurrentTime); }
        }

        public UserData CurrentUser
        {
            get => IoC.Get<LoginViewModel>()?.CurrentUser;
        }

        /// <summary>
        /// 顶部栏显示的用户名（未登录时兜底显示"未登录"）
        /// </summary>
        public string DisplayUserName => CurrentUser?.UserName ?? "未登录";

        // ============ 程序信息（当前为占位，后续接入 RecipeService） ============
        public string RecipeName { get; set; } = "wsd";
        public string SequenceNo { get; set; } = "1";

        // ============ 实时坐标（占位，后续接入运动控制） ============
        public string MachineX { get; set; } = "0.000";
        public string MachineY { get; set; } = "0.000";
        public string MachineZ { get; set; } = "0.000";

        public bool CanGoBack => _historyIndex > 0;
        public bool CanGoForward => _historyIndex >= 0 && _historyIndex < _history.Count - 1;

        public MainWindowViewModel()
        {
            _windowManager = IoC.Get<IWindowManager>();

            // 初始化定时器，每秒更新一次当前时间显示
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (a, b) => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _clockTimer.Start();
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            LoadConfiguration();

            // 默认选中第一个一级菜单
            if (Menus.Count > 0)
                SelectMenu(Menus[0]);
        }

        private void LoadConfiguration()
        {
            var config = ConfigManager.Instance.UiConfig;

            AppTitle = string.IsNullOrWhiteSpace(config.AppTitle) ? "上位机控制平台" : config.AppTitle;
            AppSubTitle = config.AppSubTitle ?? string.Empty;

            foreach (var menu in config.MenuList)
            {
                _groupMap[menu.Key] = menu;
                Menus.Add(menu);
                foreach (var page in menu.SubMenuList)
                {
                    _pageMap[page.FullKey] = page;
                }
            }
        }

        /// <summary>
        /// 切换一级菜单：重建当前分组页签，并选中第一个页面。
        /// </summary>
        public void SelectMenu(Menu menu)
        {
            if (menu == null) return;

            CurrentMenu = menu;
            foreach (var m in Menus)
                m.IsSelected = ReferenceEquals(m, menu);

            Pages.Clear();
            foreach (var sub in menu.SubMenuList)
            {
                var screen = GetOrCreatePageViewModel(sub);
                if (screen != null)
                    Pages.Add(new TabPage { Menu = sub, Screen = screen });
            }

            if (Pages.Count > 0)
            {
                _currentTab = null; // 强制触发 setter，记录历史
                CurrentTab = Pages[0];
            }
            UpdatePath();
        }

        /// <summary>
        /// 点击页签切换页面。
        /// </summary>
        public void SelectTab(TabPage tab)
        {
            if (tab == null) return;
            CurrentTab = tab;
        }

        /// <summary>
        /// 激活页面：更新选中态、路径与导航历史。
        /// </summary>
        private void ActivateTab(SubMenu sub, bool recordHistory)
        {
            foreach (var item in Menus.SelectMany(g => g.SubMenuList))
                item.IsSelected = item.FullKey == sub.FullKey;

            CurrentPageTitle = sub.DisplayName ?? sub.Key;
            UpdatePath();

            if (recordHistory)
                PushHistory(sub.FullKey);

            NotifyOfPropertyChange(() => CanGoBack);
            NotifyOfPropertyChange(() => CanGoForward);
        }

        private void UpdatePath()
        {
            if (CurrentTab != null && _groupMap.TryGetValue(CurrentTab.Menu.ParentKey ?? string.Empty, out var g))
                CurrentPagePath = $"{g.DisplayName} / {CurrentTab.Menu.DisplayName}";
            else if (CurrentMenu != null)
                CurrentPagePath = CurrentMenu.DisplayName;
            else
                CurrentPagePath = string.Empty;
        }

        private void PushHistory(string fullKey)
        {
            // 点击当前页时不重复入栈
            if (_historyIndex >= 0 && _historyIndex < _history.Count && _history[_historyIndex] == fullKey)
                return;

            // 丢弃“前进”分支
            if (_historyIndex < _history.Count - 1)
                _history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);

            _history.Add(fullKey);
            _historyIndex = _history.Count - 1;
        }

        public Task GoBackAsync()
        {
            if (!CanGoBack) return Task.CompletedTask;
            _historyIndex--;
            NavigateToHistory(_history[_historyIndex]);
            return Task.CompletedTask;
        }

        public Task GoForwardAsync()
        {
            if (!CanGoForward) return Task.CompletedTask;
            _historyIndex++;
            NavigateToHistory(_history[_historyIndex]);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 按历史定位到指定页面（不重复写入历史）。
        /// </summary>
        private void NavigateToHistory(string fullKey)
        {
            if (!_pageMap.TryGetValue(fullKey, out var sub)) return;
            if (!_groupMap.TryGetValue(sub.ParentKey ?? string.Empty, out var group)) return;

            // 切换到所属分组并重建页签
            CurrentMenu = group;
            foreach (var m in Menus)
                m.IsSelected = ReferenceEquals(m, group);

            Pages.Clear();
            foreach (var s in group.SubMenuList)
            {
                var screen = GetOrCreatePageViewModel(s);
                if (screen != null)
                    Pages.Add(new TabPage { Menu = s, Screen = screen });
            }

            var tab = Pages.FirstOrDefault(p => p.Menu.FullKey == fullKey);
            if (tab != null)
            {
                _currentTab = tab;
                NotifyOfPropertyChange(() => CurrentTab);
                ActivateTab(tab.Menu, false);
            }
            UpdatePath();

            NotifyOfPropertyChange(() => CanGoBack);
            NotifyOfPropertyChange(() => CanGoForward);
        }

        // ============ 程序管理（占位，后续接入 RecipeService） ============
        public void OpenRecipe() => Notify("打开程序功能开发中");
        public void SaveRecipeAs() => Notify("另存程序功能开发中");
        public void ShowStatistics() => Notify("生产统计功能开发中");
        private static void Notify(string msg)
            => System.Windows.MessageBox.Show(msg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

        /// <summary>
        /// 退出登录：先弹窗确认，再显示登录窗口并关闭主界面，保证应用不会因无窗口而退出。
        /// </summary>
        public async Task LogoutAsync()
        {
            var result = System.Windows.MessageBox.Show(
                "确定要退出登录吗？",
                "退出登录",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result != System.Windows.MessageBoxResult.Yes)
                return;

            var loginViewModel = IoC.Get<LoginViewModel>();
            loginViewModel.Initialize();
            await _windowManager.ShowWindowAsync(loginViewModel);
            await TryCloseAsync();
        }

        /// <summary>
        /// 按 FullKey 缓存页面实例，保证来回切换时页面状态不丢失。
        /// </summary>
        private Screen GetOrCreatePageViewModel(SubMenu page)
        {
            if (_pageCache.TryGetValue(page.FullKey, out var cached))
                return cached;

            var vmType = ResolveViewModelType(page.ViewModel);

            Screen vm = null;
            try
            {
                vm = (Screen)Activator.CreateInstance(vmType);
            }
            catch
            {
            }

            if (vm != null)
                _pageCache[page.FullKey] = vm;

            return vm;
        }

        private static Type ResolveViewModelType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            return Type.GetType(typeName, false)
                   ?? Assembly.GetExecutingAssembly().GetType(typeName, false, true);
        }
    }
}
