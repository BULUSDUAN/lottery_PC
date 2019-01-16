namespace KaSon.FrameWork.Services
{
    using System;
    using System.Runtime.CompilerServices;

    public class LogConfig
    {
        /// <summary>
        /// 是否异步记录日志
        /// </summary>
        public bool LogAsync { get; set; }

        /// <summary>
        /// 日志存储库的dbkey 
        /// </summary>
        public string LogDbKey { get; set; }

        /// <summary>
        /// 日志库类型： mongodb 或者 orm
        /// </summary>
        public string LogDbType { get; set; }
    }
}

