using System;

namespace LogModule
{
    /// <summary>
    /// 日志模块接口：定义日志记录方法与 UI 推送事件
    /// </summary>
    /// <typeparam name="T">日志数据类型</typeparam>
    public interface ILogInterface<T> where T : class
    {
        /// <summary>
        /// Debug 信息记录
        /// </summary>
        /// <param name="logSource">信息来源</param>
        /// <param name="message">记录信息</param>
        /// <param name="isOutToView">是否输出到界面</param>
        /// <param name="exception">Exception报错内容</param>
        void Debug(string logSource, string message, bool isOutToView = true, Exception exception = null);

        /// <summary>
        /// Info 信息记录
        /// </summary>
        /// <param name="logSource">信息来源</param>
        /// <param name="message">记录信息</param>
        /// <param name="isOutToView">是否输出到界面</param>
        /// <param name="exception">Exception报错内容</param>
        void Info(string logSource, string message, bool isOutToView = true, Exception exception = null);

        /// <summary>
        /// Warn 信息记录
        /// </summary>
        /// <param name="logSource">信息来源</param>
        /// <param name="message">记录信息</param>
        /// <param name="isOutToView">是否输出到界面</param>
        /// <param name="exception">Exception报错内容</param>
        void Warn(string logSource, string message, bool isOutToView = true, Exception exception = null);

        /// <summary>
        /// Error 信息记录
        /// </summary>
        /// <param name="logSource">信息来源</param>
        /// <param name="message">记录信息</param>
        /// <param name="isOutToView">是否输出到界面</param>
        /// <param name="exception">Exception报错内容</param>
        void Error(string logSource, string message, bool isOutToView = true, Exception exception = null);

        /// <summary>
        /// Fatal 致命错误信息记录
        /// </summary>
        /// <param name="logSource">信息来源</param>
        /// <param name="message">记录信息</param>
        /// <param name="isOutToView">是否输出到界面</param>
        /// <param name="exception">Exception报错内容</param>
        void Fatal(string logSource, string message, bool isOutToView = true, Exception exception = null);

        /// <summary>
        /// 日志发送事件
        /// </summary>
        event LogRecItemDelegate<T> LogRecItemEvent;
    }

    /// <summary>
    /// 日志发送事件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">具体的数据</param>
    public delegate void LogRecItemDelegate<T>(T data);
}
