using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LogModule;

namespace MainPlatform
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // 全局异常兜底记录日志（不打断 Caliburn.Micro 的启动流程）
            DispatcherUnhandledException += (s, e) =>
            {
                try { NLogModule.Instance.Error("全局异常", e.Exception?.Message ?? "未知异常", true, e.Exception); } catch { }
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try
                {
                    var ex = e.ExceptionObject as Exception;
                    NLogModule.Instance.Error("全局异常", ex?.Message ?? "未知异常", true, ex);
                }
                catch { }
            };

            // 程序启动时初始化日志模块并记录启动日志
            Startup += (s, e) =>
            {
                try
                {
                    NLogModule.Instance.Info("系统", "上位机软件启动");
                }
                catch (Exception ex)
                {
                    // 日志模块初始化失败不应影响主程序运行
                    System.Diagnostics.Debug.WriteLine("日志模块初始化失败: " + ex.Message);
                }
            };
        }
    }
}
