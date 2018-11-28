using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Log;

namespace RedisBusinessManager
{
    /// <summary>
    /// 自动任务接口
    /// </summary>
    public interface IAutoTask
    {
        /// <summary>
        /// 开始任务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止任务
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 自动任务基类
    /// </summary>
    public abstract class AutoTaskBase
    {
        /// <summary>
        /// 是否停止任务
        /// </summary>
        public bool BeStop { get; set; }

        private ILogWriter _logWriter = null;
        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogWriter LogWriter
        {
            get
            {
                if (_logWriter == null)
                    _logWriter = LogWriterGetter.GetLogWriter();
                return _logWriter;
            }
        }

        /// <summary>
        /// 日志类别
        /// </summary>
        public abstract string LogCategory { get; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public abstract string TaskName { get; }
        /// <summary>
        /// 任务排序
        /// </summary>
        public abstract int TaskOrder { get; }

        /// <summary>
        /// 写日志
        /// </summary>
        public void WriteLog(string log)
        {
            if (LogWriter != null)
                LogWriter.Write(this.LogCategory, this.LogCategory, LogType.Error, this.TaskName, log);
        }

        /// <summary>
        /// 获取彩种中文名称
        /// </summary>
        public string GetGameName(string gameCode)
        {
            switch (gameCode)
            {
                case "SSQ":
                    return "双色球";
                case "DLT":
                    return "大乐透";
                case "FC3D":
                    return "福彩3D";
                case "PL3":
                    return "排列3";
                case "CQSSC":
                    return "重庆时时彩";
                case "JX11X5":
                    return "江西11选5";
                case "SD11X5":
                    return "山东11选5";
                case "GD11X5":
                    return "广东11选5";
                case "GDKLSF":
                    return "广东快乐十分";
                case "JSKS":
                    return "江苏快三";
                case "SDKLPK3":
                    return "山东快乐扑克3";
                case "CTZQ":
                    return "传统足球";
                case "BJDC":
                    return "北京单场";
                case "JCZQ":
                    return "竞彩足球";
                case "JCLQ":
                    return "竞彩篮球";
                default:
                    break;
            }
            return gameCode;
        }
    }
}
