using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace Common.Log
{
    /// <summary>
    /// 文件日志记录器
    /// </summary>
    [Serializable]
    public class FileLogWriter : ILogWriter
    {
        private static string CurrentDate = null;
        private static int CurrentIndex = 1;
        /// <summary>
        ///要向其中追加日志消息的文件的路径
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 将消息添加到日志文件
        /// </summary>
        /// <param name="text">消息内容</param>  
        private void AppendTextToFile(string category, string source, string text)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                FileName = CreateFileName(category + source);
            }
            TryAppendText(text, 0, 3);
        }

        private void TryAppendText(string text, int time, int max)
        {
            try
            {
                string path = Path.GetDirectoryName(FileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (StreamWriter sw = File.AppendText(FileName))
                {
                    sw.WriteLine(text);
                    sw.Flush();
                }
            }
            catch
            {
                if (time++ < max)
                {
                    System.Threading.Thread.Sleep(100);
                    TryAppendText(text, time, max);
                }
            }
        }

        private string CreateFileName(string prev)
        {
            string date = DateTime.Now.ToString("yyyyMMdd-HHmm");
            if (date == CurrentDate)
            {
                CurrentIndex += 1;
            }
            else
            {
                CurrentDate = date;
                CurrentIndex = 1;
            }
            string fn = string.Format("{0}-{1}-{2}-{3}", "LOG", prev, CurrentDate, CurrentIndex.ToString().PadLeft(3, '0')) + ".log";
            var logRoot = ConfigurationManager.AppSettings["LogFolder"];
            logRoot = string.IsNullOrEmpty(logRoot) ? "LogsRoot" : logRoot;
            return Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Site.Log", logRoot, fn);
        }

        private static object lockObj = new object();
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="source">源</param>
        /// <param name="logType">类型</param>
        /// <param name="logMsg">消息</param>
        /// <param name="detail">明细</param>
        public void Write(string category, string source, LogType logType, string logMsg, string detail)
        {
            lock (lockObj)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("****************** " + DateTime.Now + " ******************");
                sb.AppendLine("Category: " + category);
                sb.AppendLine("Source: " + source);
                sb.AppendLine("Type: " + logType.ToString());
                sb.AppendLine("Message: " + logMsg);
                if (!string.IsNullOrEmpty(detail))
                {
                    sb.AppendLine(detail);
                }
                sb.AppendLine("*****************END*******************");
                sb.AppendLine("");

                AppendTextToFile(category, source, sb.ToString());
            }
        }
        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="source">源</param>
        /// <param name="exception">异常</param>
        public void Write(string category, string source, Exception exception)
        {
            lock (lockObj)
            {
                if (exception == null)
                {
                    return;
                }
                var detail = new StringBuilder();
                detail.AppendLine("Detail:" + exception.ToString());
                detail.AppendLine("StackTrace:" + exception.StackTrace);
                if (exception.TargetSite != null)
                {
                    detail.AppendLine("Method:" + exception.TargetSite.Name);
                }
                if (exception.Data != null)
                {
                    foreach (var key in exception.Data.Keys)
                    {
                        detail.AppendLine("    " + key + ":" + exception.Data[key]);
                    }
                }
                Write(category, source, LogType.Error, exception.Message, detail.ToString());
                if (exception.InnerException != null)
                {
                    Write(category, source, exception.InnerException);
                }
            }
        }
    }
}
