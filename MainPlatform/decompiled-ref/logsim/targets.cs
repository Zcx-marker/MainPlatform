using System;
using System.Threading;
using LogModule;

namespace Service.SimulationService
{
	public class LogSimulationService
	{
		private static readonly Lazy<LogSimulationService> _instance = new Lazy<LogSimulationService>(() => new LogSimulationService());

		private Timer _logTimer;

		private int _logCount = 0;

		public static LogSimulationService Instance => _instance.Value;

		private void LogSimulation(object state)
		{
			_logCount++;
			NLogModule.Instance.Info("LogSimulationService", "第一个焊点等待焊接,等待点火......");
			Thread.Sleep(30);
			NLogModule.Instance.Info("LogSimulationService", "点火完毕!!");
			Thread.Sleep(30);
			NLogModule.Instance.Info("LogSimulationService", "第一个焊点正在焊接!");
			Thread.Sleep(30);
			NLogModule.Instance.Info("LogSimulationService", "第一个焊点焊接完毕!");
		}

		public void Start()
		{
			_logTimer = new Timer(LogSimulation, null, 0, 700);
		}

		public void Stop()
		{
			_logTimer?.Dispose();
		}
	}
}
