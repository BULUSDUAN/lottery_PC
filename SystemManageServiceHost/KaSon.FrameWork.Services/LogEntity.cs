namespace KaSon.FrameWork.Services
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 日志实体对象基类
    /// </summary>
    public class LogEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LogEntity()
        {
            this.LogHost = Environment.MachineName;
            this.LogTime = DateTime.Now;
            this.Id = Guid.NewGuid().ToString();
            this.LogProcess = Process.GetCurrentProcess().Id.ToString();
            this.LogLevel = KaSon.FrameWork.Services.LogLevel.Debug;
            this.LogSource = "";
            this.LogMessage = "";
            this.LoginName = "";
            this.ClientIP = "";
        }

        public string ClientIP { get; set; }

        /// <summary>
        /// 日志主键
        /// </summary>
        public string Id { get; private set; }

        public string LogHost { get; private set; }

        public string LoginName { get; set; }

        public KaSon.FrameWork.Services.LogLevel LogLevel { get; set; }

        public string LogMessage { get; set; }

        public string LogProcess { get; private set; }

        public string LogSource { get; set; }

        public DateTime LogTime { get; private set; }
    }
}

