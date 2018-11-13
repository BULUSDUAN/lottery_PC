namespace KaSon.FrameWork.Services.ServiceInter
{
    using System;

    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 初始化日志对象
        /// </summary>
        /// <param name="ConfigKey"></param>
        /// <returns></returns>
        ILogger Init(string ConfigKey);
        /// <summary>
        /// 写入业务日志
        /// </summary>
        /// <param name="entity">日志实体对象</param>
        void Log(LogEntity entity);
        /// <summary>
        /// 写入异常日志
        /// </summary>
        /// <param name="loglevel">日志级别</param>
        /// <param name="e">异常对象</param>
        void Log(KaSon.FrameWork.Services.LogLevel loglevel, Exception e);
    }
}

