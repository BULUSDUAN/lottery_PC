using KaSon.FrameWork.Common.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Common
{
    public  class KgLog : IKgLog
    {
        private static bool _AppLogEnabled = true;
        private static object LockObj = new object();

        static KgLog()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"KgLog.xml");
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\KgLog.xml");
            }
            var list = xmlHelper.SerializerList<LogConfigInfo>(path);
            if (list.Count > 0)
            {

                _AppLogEnabled = list[0].isDebuger == 1 ? true : false;

            }
          //  log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net_ORMHelper.config"));



            //[assembly: log4net.Config.XmlConfigurator(ConfigFile="Log4Net.config", Watch=true)]  
            //_AppLogEnabled = AppSettings.AppLogEnabled;
            //if (!_AppLogEnabled)
            //{
            //    WriteLog("未开启该功能。可以通过在配置文件的appSettings配置节设置为true来开启该项功能", null);
            //}
        }

        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        public  void Log(string name,Exception ex)
        {
            Log(name,ex, null);
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public  void Log(string msg)
        {
            Log(msg, "");
        }

        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="logfile">日志文件</param>
        public static void Log(string name,Exception ex, string logfile)
        {
            if (_AppLogEnabled && (ex != null))
            {
                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrEmpty(name)) {
                    builder.AppendFormat("错误标签:{0}\r\n", name);
                }
               
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

        private static void WriteLog(string msg, string logfile = "")
        {
            lock (LockObj)
            {
                string str = "";
                if (!string.IsNullOrEmpty(logfile))
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
