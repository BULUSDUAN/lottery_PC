namespace KaSon.FrameWork.Services
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 简单的异常日志实体信息类
    /// </summary>
    [Log("gs_ErrorLog")]
    public sealed class ExceptionLogEntity : LogEntity
    {
        private Exception ex;

        public ExceptionLogEntity(KaSon.FrameWork.Services.LogLevel loglevel, string message, Exception e)
        {
            base.LoginName = "";
            base.ClientIP = "";
            base.LogSource = string.Empty;
            this.StackTrace = string.Empty;
            base.LogLevel = loglevel;
            if (e != null)
            {
                this.ex = e;
                this.StackTrace = e.ToString();
                if (string.IsNullOrEmpty(message))
                {
                    base.LogMessage = e.Message ?? string.Empty;
                }
            }
        }

        public string StackTrace { get; set; }
    }
}

