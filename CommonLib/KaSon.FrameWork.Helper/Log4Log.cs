using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Helper
{
    public class Log4Log:IKgLog
    {
        private static  log4net.ILog errorlogger =null ;//LogManager.GetLogger(typeof(Log4Log));
        private static log4net.ILog infologger = null;
        private static ILoggerRepository repository { get; set; }
        //ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static Log4Log()
        {
            //NETCoreRepository NETStandardRepository
            repository = LogManager.CreateRepository("NETCoreRepository");
            //  log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net_ORMHelper.config"));
            var fileinfo = new FileInfo("log4net_ORMHelper.config");
            XmlConfigurator.Configure(repository, fileinfo);

            log4net.Config.BasicConfigurator.Configure(repository);
            errorlogger = LogManager.GetLogger(repository.Name, "logerror");
            infologger = LogManager.GetLogger(repository.Name, "loginfo");
            // log4net.ILog logger = LogManager.GetLogger(typeof(Log4Log));
        }

        public  void Log(string name,Exception ex)
        {
            if (errorlogger == null)
            {
                errorlogger = LogManager.GetLogger(repository.Name, "logerror");
            }
            errorlogger.Error(name,ex);
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public  void Log(string msg)
        {
            if (infologger == null)
            {
                infologger = LogManager.GetLogger(repository.Name, "loginfo");
            }
            infologger.Info(msg);
         
        }

       
    }
}
