
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace KaSon.FrameWork.Common
{
    public enum KLogLevel
    {
        APIError = 0,
        GenError = 1,
        Info = 2,
        Debug = 3,
        TimeInfo = 4,
        SevTimeInfo = 5,
        RedisTimeInfo=6,

    }
    public class Log4Log : IKgLog
    {
        private static log4net.ILog errorlogger = null;//LogManager.GetLogger(typeof(Log4Log));
        private static log4net.ILog infologger = null;
        private static log4net.ILog apiLogerror = null;
        private static log4net.ILog logWarning = null;

        private static log4net.ILog timeInfoLog = null;
        private static log4net.ILog sevTimeInfo = null;
        private static log4net.ILog redisTimeInfoLog = null;

        private static log4net.ILog iLog = null;

        private static ILoggerRepository repository { get; set; }
        //ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static XmlElement log4netEle = null;
        static XmlDocument log4netConfig = null;
        static Log4Log()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"log4net.config");
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\log4net.config");
            }
            log4netConfig = new XmlDocument();
            var stream = File.OpenRead(path);
            log4netConfig.Load(stream);
            stream.Close();
            log4netEle = log4netConfig["log4net"];
            repository = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repository, log4netEle);
            LogManager.GetLogger(repository.Name, "ClassName");
            //NETCoreRepository NETStandardRepository
            //repository = LogManager.CreateRepository("NETCoreRepository");
            ////  log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net_ORMHelper.config"));

            //string path = Path.Combine(Directory.GetCurrentDirectory(), @"log4net.xml");
            //if (!File.Exists(path))
            //{
            //    path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\log4net.xml");
            //}
            //var fileinfo = new FileInfo(path);
            //if (!fileinfo.Exists)
            //{
            //    Console.WriteLine("日志配置文件不存在！");
            //}

            //XmlConfigurator.Configure(repository, fileinfo);

            //log4net.Config.BasicConfigurator.Configure(repository);
            //apiLogerror = LogManager.GetLogger(repository.Name, "apiLogerror");
            //errorlogger = LogManager.GetLogger(repository.Name, "logerror");
            //infologger = LogManager.GetLogger(repository.Name, "loginfo");
            //logWarning = LogManager.GetLogger(repository.Name, "logWarning");
            //timeInfoLog = LogManager.GetLogger(repository.Name, "ApiTimeInfo");
            //redisTimeInfoLog = LogManager.GetLogger(repository.Name, "RedisTimeInfoLog");
            //sevTimeInfo = LogManager.GetLogger(repository.Name, "SevtimeIoginfo");

            // logWarning = LogManager.GetLogger(repository.Name, "logWarning");
            //logWarning
            // log4net.ILog logger = LogManager.GetLogger(typeof(Log4Log));
        }

        public void Log(string name, Exception ex)
        {

            if (errorlogger == null)
            {
                errorlogger = LogManager.GetLogger(repository.Name, "logerror");
            }
            errorlogger.Error(name, ex);
        }
        /// <summary>
        /// 创建日志工具
        /// </summary>
        /// <param name="ClassName"></param>
        public static ILog CreateLog(string ClassName= "ClassName") {

            return LogManager.GetLogger(repository.Name, ClassName);
        }

        public static void Fatal(string message="",Exception exception=null) {
            if (exception == null) exception= new Exception("");
            LogManager.GetLogger(repository.Name, "ClassName").Fatal(message, exception);
        }
        public static void Info(string message = "", Exception exception = null)
        {
            if (exception == null) exception = new Exception("");
            iLog.Info(message, exception);
        }
        public static void Warn(string message = "", Exception exception = null)
        {
            if (exception == null) exception = new Exception("");
            iLog.Warn(message, exception);
        }
        public static void Debug(string message = "", Exception exception = null)
        {
            if (exception == null) exception = new Exception("");
            iLog.Debug(message, exception);
        }
        public static void Error(string message = "", Exception exception = null)
        {
            if (exception == null) exception = new Exception("");
            iLog.Error(message, exception);
        }

        //public static void LogEX(KLogLevel lev, string name, object info=null)
        //{
        //    Exception ex = new Exception();
        //    switch (lev)
        //    {
        //        case KLogLevel.APIError:
        //            if (apiLogerror == null)
        //            {
        //                apiLogerror = LogManager.GetLogger(repository.Name, "apiLogerror");
        //            }
        //            ex = info as Exception;
        //            apiLogerror.Error(name, ex);
        //            break;
        //        case KLogLevel.GenError:
        //            if (errorlogger == null)
        //            {
        //                errorlogger = LogManager.GetLogger(repository.Name, "logerror");
        //            }
        //             ex = info as Exception;
        //            errorlogger.Error(name, ex);
        //            break;
        //        case KLogLevel.Info:
        //            ex = info as Exception;
        //            infologger.Info(name, ex);
        //            break;
        //        case KLogLevel.Debug:
        //            ex = info as Exception;
        //            logWarning.Info(name, ex);
        //            break;
        //        case KLogLevel.TimeInfo:
        //            string str = info as string;
        //            timeInfoLog.Info(name+"|"+ str);
        //            break;
        //        case KLogLevel.SevTimeInfo:
        //            // string str = info as string;
        //            sevTimeInfo.Info(name);
        //            break;
        //        case KLogLevel.RedisTimeInfo:
        //            // string str = info as string;
        //            redisTimeInfoLog.Info(name + "|" + (info==null?"":info.ToString()));
        //           // redisTimeInfoLog.Info(name);
        //            break;
        //        default:
        //            if (errorlogger == null)
        //            {
        //                errorlogger = LogManager.GetLogger(repository.Name, "logerror");
        //            }
        //            errorlogger.Error(name, ex);
        //            break;
        //    }

        //}

        public void ErrrorLog(string name, Exception ex)
        {
            if (errorlogger == null)
            {
                errorlogger = LogManager.GetLogger(repository.Name, "logerror");
            }
            errorlogger.Error(name, ex);
        }
        public void WarningLog(string msg)
        {
            if (logWarning == null)
            {
                logWarning = LogManager.GetLogger(repository.Name, "logWarning");
            }
            logWarning.Info(msg);
        }
        //public void WriteLog(string category, string source, int logType, string logMsg, string detail)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("****************** " + DateTime.Now + " ******************");
        //    sb.AppendLine("Category: " + category);
        //    sb.AppendLine("Source: " + source);
        //    sb.AppendLine("Type: " + logType.ToString());
        //    sb.AppendLine("Message: " + logMsg);
        //    if (!string.IsNullOrEmpty(detail))
        //    {
        //        sb.AppendLine(detail);
        //    }
        //    sb.AppendLine("*****************END*******************");
        //    sb.AppendLine("");

        //    switch (logType)
        //    {
        //        case 0://Information
        //            Log(sb.ToString());
        //            break;
        //        case 1://Warning
        //            WarningLog(sb.ToString());
        //            break;
        //        case 2://Error
        //            ErrrorLog(sb.ToString(), new Exception("错误日志:" + category + source + ""));
        //            break;
        //        default:
        //            Log(sb.ToString());
        //            break;
        //    }

        //    //     Log( sb.ToString());

        //}

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public void Log(string msg)
        {
            if (infologger == null)
            {
                infologger = LogManager.GetLogger(repository.Name, "loginfo");
            }
            infologger.Info(msg);

        }


    }
}
