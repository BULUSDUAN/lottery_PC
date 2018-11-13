using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services
{
    public static class LogHelper
    {
        private static bool _AppLogEnabled = true;
        private static object LockObj = new object();

        static LogHelper()
        {
          // _AppLogEnabled = AppSettings.AppLogEnabled;
            //if (!_AppLogEnabled)
            //{
            //    WriteLog("未开启该功能。可以通过在配置文件的appSettings配置节设置Kad.FrameWork.AppLogEnabled为true来开启该项功能", null);
            //}
        }

        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        public static void Log(Exception ex)
        {
            Log(ex, null);
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public static void Log(string msg)
        {
            Log(msg, null);
        }

        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="logfile">日志文件</param>
        public static void Log(Exception ex, string logfile)
        {
            if (_AppLogEnabled && (ex != null))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("发生［{0}］异常，异常相关信息如下：", ex.GetType().ToString());
                builder.AppendFormat("\r\n异常描述：{0}", ex.Message);
                builder.AppendFormat("\r\n异 常 源：{0}", ex.Source);
                builder.AppendFormat("\r\n堆栈跟踪：\r\n{0}", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    builder.AppendFormat("\r\n内含异常：{0}", ex.InnerException.GetType().ToString());
                    builder.AppendFormat("\r\n异常描述：{0}", ex.InnerException.Message);
                    builder.AppendFormat("\r\n异 常 源：{0}", ex.InnerException.Source);
                    builder.AppendFormat("\r\n堆栈跟踪：\r\n{0}\r\n", ex.InnerException.StackTrace);
                }
                else
                {
                    builder.Append("\r\n内含异常：无\r\n");
                }
                WriteLog(builder.ToString(), logfile);
            }
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        /// <param name="logfile">日志文件</param>
        public static void Log(string msg, string logfile)
        {
            if (_AppLogEnabled && !string.IsNullOrEmpty(msg))
            {
                WriteLog(msg, logfile);
            }
        }

        private static void WriteLog(string msg, string logfile = null)
        {
            lock (LockObj)
            {
                string str = "";
                if (!string.IsNullOrWhiteSpace(logfile))
                {
                    str = @"FrameWork_Log\" + logfile;
                }
                else
                {
                    str = @"FrameWork_Log\" + DateTime.Now.ToString("yyyyMMddHH") + "_" + AppDomain.CurrentDomain.Id.ToString() + ".log";
                }
                str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
                string directoryName = Path.GetDirectoryName(str);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                string str3 = string.Format("{0}：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), msg);
                using (StreamWriter writer = new StreamWriter(str, true, Encoding.Default))
                {
                    writer.WriteLine(str3);
                }
            }
        }
    }
}
