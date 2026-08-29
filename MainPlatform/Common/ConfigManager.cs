using MainPlatform.Models;
using MainPlatform.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainPlatform.Common
{
    internal class ConfigManager
    {
        //单例
        private static readonly Lazy<ConfigManager> _instance = new Lazy<ConfigManager>(() => new ConfigManager());
        public static ConfigManager Instance => _instance.Value;

        /// <summary>界面配置文件路径（随程序输出目录走，运行期自动读取）</summary>

        // 数据库配置
        private DbConfig _dbConfig;
        public DbConfig DbConfig
        {
            get => _dbConfig;
            set => _dbConfig = value;
        }

        private bool _isDbInitialized;
        public bool IsDbInitialized
        {
            get => _isDbInitialized;
            set => _isDbInitialized = value;
        }

        // 界面配置
        private UiConfig _uiConfig;
        public UiConfig UiConfig
        {
            get => _uiConfig;
            private set => _uiConfig = value;
        }

        public ConfigManager()
        {
            LoadDbConfig();
            LoadUiConfig();
        }

        public void LoadDbConfig()
        {
            DbConfig = JsonTool.Instance.LoadFromJsonFile < DbConfig > (Path.Combine(PathTool.RunningFolderPath, "Config", "DbStr.json"));
        }

        /// <summary>
        /// 读取界面配置文件，并补齐每个页面的 ParentKey / FullKey，用于导航定位。
        /// </summary>
        public void LoadUiConfig()
        {
            UiConfig = JsonTool.Instance.LoadFromJsonFile<UiConfig>(PathTool.GetUiConfigFolderPath());

            if (UiConfig == null || UiConfig.MenuList == null)
                throw new InvalidDataException("界面配置文件内容为空或格式不正确。");

            foreach (var menu in UiConfig.MenuList)
            {
                if (menu.SubMenuList == null) menu.SubMenuList = new System.Collections.Generic.List<SubMenu>();
                foreach (var page in menu.SubMenuList)
                {
                    page.ParentKey = menu.Key;
                    page.FullKey = $"{menu.Key}/{page.Key}";
                }
            }
        }
    }
}
