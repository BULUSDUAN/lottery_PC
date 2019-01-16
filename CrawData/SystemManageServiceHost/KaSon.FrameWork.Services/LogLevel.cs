namespace KaSon.FrameWork.Services
{
    using System;

    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 任意级别
        /// </summary>
        ALL = 9,
        /// <summary>
        /// 调试日志，最低级别
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 错误日志信息
        /// </summary>
        Error = 4,
        /// <summary>
        /// 系统错误日志信息,最高级别
        /// </summary>
        Fatal = 5,
        /// <summary>
        /// 普通日志信息
        /// </summary>
        Info = 2,
        /// <summary>
        /// 警告日志信息
        /// </summary>
        Warn = 3
    }
}

