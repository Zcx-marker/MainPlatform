using System;
using System.Diagnostics;
using System.Security.Principal;

namespace MainPlatform.Tools
{
    /// <summary>
    /// 开机自启 + 管理员权限辅助类。
    /// 实现方式：Windows 任务计划（schtasks ONLOGON + 最高权限）。
    /// 相比注册表 Run 键，登录时由任务计划直接以最高权限静默启动，不弹 UAC 提示，
    /// 真正满足"开机自动以管理员身份运行"。
    /// </summary>
    public static class AutoStartHelper
    {
        /// <summary>任务计划名称（唯一标识）</summary>
        private const string TaskName = "MainPlatformAutoStart";

        /// <summary>
        /// 当前进程是否以管理员身份运行。
        /// </summary>
        public static bool IsRunAsAdmin()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保证以管理员身份运行：非管理员时通过 UAC 提权重新启动自身，当前进程退出。
        /// 开机自启（任务计划最高权限启动）时已是管理员，不会触发提权。
        /// </summary>
        public static void EnsureElevated()
        {
            if (IsRunAsAdmin())
                return;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    Verb = "runas", // 请求 UAC 提升
                    UseShellExecute = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };
                Process.Start(psi);
            }
            catch
            {
                // 用户取消 UAC 时继续以普通权限运行，不阻断程序启动
                return;
            }

            // 原进程退出，由提权后的新进程接管
            Environment.Exit(0);
        }

        /// <summary>
        /// 注册开机自启：创建 Windows 任务计划，用户登录时以最高权限运行本程序。
        /// 需要管理员权限（程序以管理员身份运行时调用）。
        /// </summary>
        public static bool EnableAutoStart()
        {
            try
            {
                string exe = Process.GetCurrentProcess().MainModule.FileName;
                string args = string.Format(
                    "/Create /TN \"{0}\" /TR \"\\\"{1}\\\"\" /SC ONLOGON /RL HIGHEST /F",
                    TaskName, exe);

                var psi = new ProcessStartInfo("schtasks.exe", args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process p = Process.Start(psi))
                {
                    if (p == null) return false;
                    p.WaitForExit(5000);
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 取消开机自启：删除任务计划。
        /// </summary>
        public static bool DisableAutoStart()
        {
            try
            {
                var psi = new ProcessStartInfo("schtasks.exe",
                    string.Format("/Delete /TN \"{0}\" /F", TaskName))
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process p = Process.Start(psi))
                {
                    if (p == null) return false;
                    p.WaitForExit(5000);
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
