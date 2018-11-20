using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Common.Log
{
    /// <summary>
    /// 日志记录器接口。提供系统日志记录的通用接口。
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// 将文本信息记入日志
        /// </summary>
        void Write(string category, string source, LogType logType, string logMsg, string detail);
        /// <summary>
        /// 将一个异常记入日志
        /// </summary>
        void Write(string category, string source, Exception exception);
    }
    /// <summary>
    /// 日志类型，日志按严重程度分成提示、警告和错误。
    /// </summary>
    [Serializable]
    public enum LogType
    {
        /// <summary>
        /// 提示
        /// </summary>
        Information = 0,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 1,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 2
    }
    /// <summary>
    /// 获取日志记录器
    /// </summary>
    public static class LogWriterGetter
    {
        public static ILogWriter GetLogWriter()
        {
            string writer = ConfigurationManager.AppSettings["LogWriter"];
            if (string.IsNullOrEmpty(writer))
            {
                return new FileLogWriter();
            }
            Type writerType = Type.GetType(writer, true, true);
            return writerType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as ILogWriter;
        }
    }
}
