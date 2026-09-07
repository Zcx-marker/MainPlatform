using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using System;
using System.Text;

namespace LogModule
{
    /// <summary>
    /// 日志消息模型：同时作为 NLog 自定义 LayoutRenderer，供 nlog.config 的 ${LogMessageModel:属性名} 占位符使用
    /// </summary>
    [LayoutRenderer("LogMessageModel")]
    [ThreadAgnostic]
    public class LogMessageModel : LayoutRenderer
    {
        public LogMessageModel(string messageCode, string message, string logSource, bool isOutToView = true, Exception exception = null)
        {
            LogTime = DateTime.Now;
            IsOutToView = isOutToView;
            MessageCode = messageCode;
            Message = message;
            LogSource = logSource;
            if (exception == null)
                Exception = new Exception(message);
            else
                Exception = exception;
        }

        public LogMessageModel(string messageCode, string message, string logSource, int alarmID, bool isOutToView = true, Exception exception = null)
        {
            LogTime = DateTime.Now;
            IsOutToView = isOutToView;
            MessageCode = messageCode;
            Message = message;
            LogSource = logSource;
            AlarmID = alarmID;
            if (exception == null)
                Exception = new Exception(message);
            else
                Exception = exception;
        }

        public LogMessageModel() { }

        /// <summary>
        /// 报警ID
        /// </summary>
        public int AlarmID { get; set; }

        /// <summary>
        /// 日志生成的时间
        /// </summary>
        public DateTime LogTime { set; get; }

        /// <summary>
        /// 是否输出到消息栏
        /// </summary>
        public bool IsOutToView { get; set; } = false;

        /// <summary>
        /// 是否存储数据库
        /// </summary>
        public bool IsSaveDataBase { get; set; } = false;

        /// <summary>
        /// 错误代码（日志级别：Debug/Info/Warning/Error/Fatal/FA）
        /// </summary>
        public string MessageCode { get; set; } = "";

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 日志来源
        /// </summary>
        public string LogSource { get; set; } = "";

        /// <summary>
        /// 异常消息
        /// </summary>
        public Exception Exception { get; set; } = new Exception();

        /// <summary>
        /// 用来标记NLog属性的自定义属性的名字
        /// </summary>
        [DefaultParameter]
        public string LayoutPropertyName { get; set; } = "";

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if ((logEvent.Parameters != null) && (logEvent.Parameters.Length > 0) && (logEvent.Parameters[0] is LogMessageModel))
            {
                try
                {
                    builder.Append(((LogMessageModel)logEvent.Parameters[0]).GetType().GetProperty(LayoutPropertyName).GetValue
                    (((LogMessageModel)logEvent.Parameters[0]), null).ToString());
                }
                catch (Exception ex)
                {
                    builder.Append("属性获取失败:LogMessageModel 中不包含此属性或此属性为NULL！");
                }
            }
            else
            {
                builder.Append("属性获取失败:logEvent中不包含此类型参数！");
            }
        }
    }
}
