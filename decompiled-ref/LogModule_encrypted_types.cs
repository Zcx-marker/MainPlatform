// ============================================================================
// 本文件由编译产物(bin\Debug)反编译还原，用于前端开发参考。
// 对应源码原为 SafeNetLOCK 加密文件，此为反编译后的近似还原（ILSpy 风格），
// 结构/逻辑与原始源码一致，但并非逐字节原文（无原始注释、局部变量名可能不同）。
// 生成时间：2026-09-04
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using NLog;
using NLog.Config;

namespace LogModule
{
	public class NLogModule : ILogInterface<LogMessageModel>
	{
		private readonly ConcurrentQueue<LogMessageModel> logQue = new ConcurrentQueue<LogMessageModel>();

		private Timer logTimer;

		private static ILogger _logger;

		private static readonly Lazy<NLogModule> _instance = new Lazy<NLogModule>(() => new NLogModule());

		public static NLogModule Instance => _instance.Value;

		public event LogRecItemDelegate<LogMessageModel> LogRecItemEvent;

		private NLogModule()
		{
			LogManager.Configuration = GetXmlLoggingConfiguration();
			_logger = LogManager.GetCurrentClassLogger();
			logTimer = new Timer(UpdateUI, null, 0, 100);
		}

		private void UpdateUI(object state)
		{
			try
			{
				if (logQue.Count > 0 && LogRecItemEvent != null)
				{
					LogMessageModel result;
					while (logQue.TryDequeue(out result))
					{
						LogRecItemEvent(result);
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					Instance.Info("Log模块", "Log模块更新至UI出错：" + ex.Message);
				}
				catch
				{
				}
			}
		}

		private XmlLoggingConfiguration GetXmlLoggingConfiguration()
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nlog.config");
			if (!File.Exists(text))
			{
				throw new FileNotFoundException("未找到nlog.config配置文件: " + text);
			}
			return new XmlLoggingConfiguration(new FileInfo(text).FullName);
		}

		public void Debug(string logSource, string message, bool isOutToView = true, Exception exception = null)
		{
			LogMessageModel logMessageModel = new LogMessageModel("Debug", message, logSource, isOutToView, exception);
			_logger.Debug(logMessageModel);
			if (isOutToView)
			{
				logQue.Enqueue(logMessageModel);
			}
		}

		public void Error(string logSource, string message, int AlarmID, bool isOutToView = true, Exception exception = null)
		{
			AlarmID = CheckAlarmID(logSource, message, AlarmID);
			LogMessageModel logMessageModel = new LogMessageModel("Error", message, logSource, AlarmID, isOutToView, exception);
			_logger.Error(logMessageModel);
			if (isOutToView)
			{
				logQue.Enqueue(logMessageModel);
			}
		}

		private int CheckAlarmID(string logSource, string message, int AlarmID)
		{
			if (logSource.Contains("调度") || logSource.Contains("Dispatch"))
			{
				if (message.ToUpper().Contains("PMB") || message.ToUpper().Contains("LPB") || message.ToUpper().Contains("ROBOTB"))
				{
					AlarmID++;
				}
				else if (message.ToUpper().Contains("PMC") || message.ToUpper().Contains("LPC"))
				{
					AlarmID += 2;
				}
				else if (message.ToUpper().Contains("PMD") || message.ToUpper().Contains("LPD"))
				{
					AlarmID += 3;
				}
			}
			return AlarmID;
		}

		public void Error(string logSource, string message, bool isOutToView = true, Exception exception = null)
		{
			LogMessageModel logMessageModel = new LogMessageModel("Error", message, logSource, isOutToView, exception);
			_logger.Error(logMessageModel);
			if (isOutToView)
			{
				logQue.Enqueue(logMessageModel);
			}
		}

		public void Fatal(string logSource, string message, bool isOutToView = true, Exception exception = null)
		{
			LogMessageModel logMessageModel = new LogMessageModel("Fatal", message, logSource, isOutToView, exception);
			_logger.Fatal(logMessageModel);
			if (isOutToView)
			{
				logQue.Enqueue(logMessageModel);
			}
		}

		public void Info(string logSource, string message, bool isOutToView = true, Exception exception = null)
		{
			LogMessageModel logMessageModel = new LogMessageModel("Info", message, logSource, isOutToView, exception);
			_logger.Info(logMessageModel);
			if (isOutToView)
			{
				logQue.Enqueue(logMessageModel);
			}
		}

		public void Warn(string logSource, string message, bool isOutToView = true, Exception exception = null)
		{
			LogMessageModel logMessageModel = new LogMessageModel("Warning", message, logSource, isOutToView, exception);
			_logger.Warn(logMessageModel);
			if (isOutToView)
			{
				logQue.Enqueue(logMessageModel);
			}
		}
	}
}
