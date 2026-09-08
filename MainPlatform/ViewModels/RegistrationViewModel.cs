using Caliburn.Micro;
using DbModels;
using Service.DbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace MainPlatform.ViewModels
{
    /// <summary>
    /// 用户注册界面：管理员登录后可为产线添加新用户（操作员/管理员）。
    /// 校验：用户名必填且格式正确、密码必填且长度合规、二次确认一致、手机号选填格式校验、用户名查重。
    /// 通过校验后保存到数据库 UserData 表。
    /// </summary>
    public class RegistrationViewModel : Screen
    {
        private readonly UserDataService _userDataService;

        private string _userName = "";
        private string _password = "";
        private string _confirmPassword = "";
        private string _phoneNumber = "";
        private string _errorMessage = "";
        private KeyValuePair<string, string> _selectedIdentity;

        public RegistrationViewModel()
        {
            _userDataService = IoC.Get<UserDataService>();
            IdentityList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("操作员", "Operater"),
                new KeyValuePair<string, string>("管理员", "Admin")
            };
            // 默认注册为操作员，避免误开管理员权限
            _selectedIdentity = IdentityList[0];
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; NotifyOfPropertyChange(() => UserName); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; NotifyOfPropertyChange(() => Password); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; NotifyOfPropertyChange(() => ConfirmPassword); }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; NotifyOfPropertyChange(() => PhoneNumber); }
        }

        public List<KeyValuePair<string, string>> IdentityList { get; }

        public KeyValuePair<string, string> SelectedIdentity
        {
            get => _selectedIdentity;
            set { _selectedIdentity = value; NotifyOfPropertyChange(() => SelectedIdentity); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; NotifyOfPropertyChange(() => ErrorMessage); }
        }

        /// <summary>
        /// 注册按钮：逐项校验后写入数据库。
        /// </summary>
        public void Register()
        {
            // 1. 用户名：必填 + 格式（3~20 位字母、数字、下划线）
            var name = (UserName ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                ErrorMessage = "请输入用户名";
                return;
            }
            if (!Regex.IsMatch(name, "^[A-Za-z0-9_]{3,20}$"))
            {
                ErrorMessage = "用户名须为 3~20 位字母、数字或下划线";
                return;
            }

            // 2. 密码：必填 + 长度（6~20 位）
            var pwd = Password ?? "";
            if (string.IsNullOrEmpty(pwd))
            {
                ErrorMessage = "请输入密码";
                return;
            }
            if (pwd.Length < 6 || pwd.Length > 20)
            {
                ErrorMessage = "密码长度须为 6~20 位";
                return;
            }

            // 3. 二次确认密码一致
            if (pwd != (ConfirmPassword ?? ""))
            {
                ErrorMessage = "两次输入的密码不一致";
                return;
            }

            // 4. 手机号：选填，填写则校验 11 位手机号
            var phone = (PhoneNumber ?? "").Trim();
            if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, "^1[3-9]\\d{9}$"))
            {
                ErrorMessage = "手机号格式不正确（11 位数字）";
                return;
            }

            // 5. 用户名查重
            var exist = _userDataService.GetList(u => u.UserName == name);
            if (exist != null && exist.Count > 0)
            {
                ErrorMessage = "该用户名已存在，请更换";
                return;
            }

            // 6. 组装新用户并保存
            var user = new UserData
            {
                UserName = name,
                Password = pwd,
                PhoneNumber = phone,
                Identity = SelectedIdentity.Value,
                CreateTime = DateTime.Now,
                UpdateTime = null,
                LastLoginTime = null,
                IsRemembered = false
            };

            bool ok = _userDataService.Insert(user);
            if (ok)
            {
                MessageBox.Show("用户注册成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                TryCloseAsync();
            }
            else
            {
                ErrorMessage = "保存失败，请重试";
            }
        }

        /// <summary>
        /// 取消按钮：关闭注册窗口。
        /// </summary>
        public void Cancel()
        {
            TryCloseAsync();
        }
    }
}
