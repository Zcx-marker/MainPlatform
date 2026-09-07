using System;
using System.Collections.Generic;
using System.IO;
using MainPlatform.Models;
using MainPlatform.Tools;

namespace MainPlatform.Models
{
	public class AppConfig
	{
		public string CurrentMode { get; set; }

		public int YGChanged { get; set; }

		public int ZStartPos { get; set; }
	}
}
namespace MainPlatform.Common
{
	internal class ConfigManager
	{
		private static readonly Lazy<ConfigManager> _instance = new Lazy<ConfigManager>(() => new ConfigManager());

		private DbConfig _dbConfig;

		private bool _isDbInitialized;

		private UiConfig _uiConfig;

		private AppConfig _appConfig;

		public static ConfigManager Instance => _instance.Value;

		public DbConfig DbConfig
		{
			get
			{
				return _dbConfig;
			}
			set
			{
				_dbConfig = value;
			}
		}

		public bool IsDbInitialized
		{
			get
			{
				return _isDbInitialized;
			}
			set
			{
				_isDbInitialized = value;
			}
		}

		public UiConfig UiConfig
		{
			get
			{
				return _uiConfig;
			}
			private set
			{
				_uiConfig = value;
			}
		}

		public AppConfig AppConfig
		{
			get
			{
				return _appConfig;
			}
			set
			{
				_appConfig = value;
			}
		}

		public void Initialize()
		{
			LoadDbConfig();
			LoadUiConfig();
			LoadAppConfig();
		}

		public void LoadAppConfig()
		{
			AppConfig = JsonTool.Instance.LoadFromJsonFile<AppConfig>(Path.Combine(PathTool.RunningFolderPath, "Config", "AppConfig.json"));
			if (AppConfig == null)
			{
				AppConfig = new AppConfig();
			}
		}

		public void LoadDbConfig()
		{
			DbConfig = JsonTool.Instance.LoadFromJsonFile<DbConfig>(Path.Combine(PathTool.RunningFolderPath, "Config", "DbStr.json"));
		}

		public void LoadUiConfig()
		{
			UiConfig = JsonTool.Instance.LoadFromJsonFile<UiConfig>(PathTool.GetUiConfigFolderPath());
			if (UiConfig == null || UiConfig.MenuList == null)
			{
				throw new InvalidDataException("界面配置文件内容为空或格式不正确。");
			}
			foreach (Menu menu in UiConfig.MenuList)
			{
				if (menu.SubMenuList == null)
				{
					menu.SubMenuList = new List<SubMenu>();
				}
				foreach (SubMenu subMenu in menu.SubMenuList)
				{
					subMenu.ParentKey = menu.Key;
					subMenu.FullKey = menu.Key + "/" + subMenu.Key;
				}
			}
		}
	}
}
