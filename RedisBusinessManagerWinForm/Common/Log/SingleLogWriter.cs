using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace Common.Log
{
    /// <summary>
    /// 简单日志记录器
    /// 生成的日志在指定文件名后追加，达到指定阀值后新建文件
    /// </summary>
    [Serializable]
    public class SingleLogWriter : ILogWriter
    {
        private static int _orderIndex = 1;
        private static object lockObj = new object();
        public void Write(string category, string source, LogType logType, string logMsg, string detail)
        {
            lock (lockObj)
            {
                lock (lockObj)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("[0][{1:yyyy-MM-dd HH:mm:ss}] {2}", logType, DateTime.Now, logMsg);
                    sb.AppendLine();
                    if (!string.IsNullOrEmpty(detail))
                    {
                        sb.AppendLine("    " + detail);
                        sb.AppendLine("    " + "----------------------------------------------------------------------------");
                    }
                    AppendTextToFile(sb.ToString(), category, source);
                }
            }
        }

        public void Write(string category, string source, Exception exception)
        {
            if (exception == null)
                return;
            Write(category, source, LogType.Error, exception.Message, exception.ToString());
        }

        private void AppendTextToFile(string text, string category, string source)
        {
            string fileName = CreateFileName(category, source);
            string path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.AppendAllText(fileName, text);
        }

        private string CreateFileName(string category, string source)
        {
            var filePath = "";
            while (true)
            {
                string fn = string.Format(@"{2:yyyy-MM-dd\\HH}\[{0}].[{1}].{3}", category, source, DateTime.Now, _orderIndex) + ".log";
                var logRoot = ConfigurationManager.AppSettings["LogFolder"];
                logRoot = string.IsNullOrEmpty(logRoot) ? "LogsRoot" : logRoot;
                filePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Site.Log", logRoot, fn);

                var maxSize = ConfigurationManager.AppSettings["LogMaxSize"];
                if (string.IsNullOrEmpty(maxSize))
                    maxSize = "1024";
                if (!File.Exists(filePath))
                    break;
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > int.Parse(maxSize))
                {
                    _orderIndex++;
                    if (_orderIndex > 10000)
                        _orderIndex = 1;
                }
                else
                {
                    break;
                }
            }
            return filePath;
        }
    }
}
