using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainPlatform.Tools
{
    public static class PathTool
    {
        /// <summary>
        /// 当前程序运行目录
        /// </summary>
        public static string RunningFolderPath => AppDomain.CurrentDomain.BaseDirectory;

        //public PathTool() { }

        /// <summary>
        /// 获取UiConfig.json文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetUiConfigFolderPath()
        {
            return Path.Combine(RunningFolderPath, "Config","UiConfig.json");
        }

        /// <summary>
        /// 获取Config文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetConfigFolderPath()
        {
            return Path.Combine(RunningFolderPath,"Config");
        }
    }
}
