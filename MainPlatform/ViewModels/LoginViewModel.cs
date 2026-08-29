using Caliburn.Micro;
using DbModels;
using Service.DbService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainPlatform.ViewModels
{
    public class LoginViewModel : Screen
    {

        // 当前登录用户属性
        private UserData _currentUser;
        public UserData CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                NotifyOfPropertyChange(() => CurrentUser);
            }
        }

        // 用户名属性
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        // 密码属性
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        // 记住密码属性
        private bool _isRemembered;
        public bool IsRemembered
        {
            get { return _isRemembered; }
            set
            {
                _isRemembered = value;
                NotifyOfPropertyChange(() => IsRemembered);
            }
        }

        // 密码可见属性
        private bool _isPasswordVisible;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                NotifyOfPropertyChange(() => IsPasswordVisible);
            }
        }

        // 窗口管理器，用于显示其他窗口
        private IWindowManager _windowManager { get; set; }

        public UserDataService UserDataService { get; set; }

        // 无参构造
        public LoginViewModel()
        {
            Initialize();
        }

        public void Initialize()
        {
            _windowManager = IoC.Get<IWindowManager>();
            UserDataService = IoC.Get<UserDataService>();
            CurrentUser = UserDataService.Queryable<UserData>(user => user.LastLoginTime != null, user => user.LastLoginTime, OrderByType.Desc).FirstOrDefault();
            if (CurrentUser != null)
            {
                if (CurrentUser.IsRemembered)
                {
                    Username = CurrentUser.UserName;
                    Password = CurrentUser.Password;
                }
                else
                {
                    Username = CurrentUser.UserName;
                    Password = string.Empty;
                }
                IsRemembered = CurrentUser.IsRemembered;
            }
            else
            {
                Username = string.Empty;
                Password = string.Empty;
                IsRemembered = false;
            }
        }

        public void Register()
        {
            //打开注册窗口
            _windowManager.ShowDialogAsync(new RegisterViewModel());
        }

        public async Task Login()
        {
            // 查询数据库中是否存在匹配的用户名和密码
            var user = UserDataService.Queryable<UserData>(us => us.UserName == Username && us.Password == Password);
            CurrentUser = user.FirstOrDefault();
            if (CurrentUser != null)
            {
                // 登录成功：打开主界面并关闭登录窗口
                if (CurrentUser.IsRemembered != IsRemembered)
                {
                    // 如果记住密码状态发生变化，则更新数据库
                    CurrentUser.IsRemembered = IsRemembered;
                    CurrentUser.UpdateTime = DateTime.Now; // 更新时间
                }
                CurrentUser.LastLoginTime = DateTime.Now; // 更新最后登录时间 
                UserDataService.Update(CurrentUser); // 将更改保存到数据库
                var mainWindowViewModel = IoC.Get<MainWindowViewModel>();
                await _windowManager.ShowWindowAsync(mainWindowViewModel);
                await TryCloseAsync();
            }
            else
            {
                // 登录失败
                MessageBox.Show("登陆失败！用户名、密码错误或账户未注册！！");
            }
        }
    }}
