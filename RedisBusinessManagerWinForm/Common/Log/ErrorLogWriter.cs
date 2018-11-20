using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace Common.Log
{
    /// <summary>
    /// 错误日志记录器
    /// </summary>
    [Serializable]
    public class ErrorLogWriter : ILogWriter
    {
        private const int _maxFileCount = 10;
        private static List<ErrorLogInfo> _currentBuffer = new List<ErrorLogInfo>();
        /// <summary>
        /// 将消息添加到日志文件
        /// </summary>
        private void AppendTextToFile(string text, string category, string source)
        {
            //缓存中是否有key值
            var cache = _currentBuffer.FirstOrDefault(p => p.Key == source);
            if (cache == null)
            {
                cache = new ErrorLogInfo
                {
                    Key = source,
                    Index = 0,
                    LastUpdateTime = DateTime.Now,
                    //Buffer = new List<byte>(),
                };
                _currentBuffer.Add(cache);
            }
            if (cache.Index > _maxFileCount)
                cache.Index = 0;

            cache.Index = ((DateTime.Now - cache.LastUpdateTime).TotalHours < 1) ? cache.Index + 1 : 1;

            string fileName = CreateFileName(category, source, cache.Index);
            string path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //File.WriteAllBytes(fileName, cache.Buffer.ToArray());
            //cache.LastUpdateTime = DateTime.Now;
            //cache.Buffer.Clear();
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
        }


        private string CreateFileName(string category, string source, int index)
        {
            string fn = string.Format(@"{2:yyyy-MM-dd\\HH}\[{0}].[{1}].{3}", category, source, DateTime.Now, index) + ".log";
            //string fn = string.Format(@"{2:yyyy-MM-dd\\HH}\[{0}].[{1}]", category, source, DateTime.Now) + ".log";
            //string fn = string.Format(@"{1:yyyy-MM-dd\\HH}\[{0}]", category, DateTime.Now) + ".log";
            //return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\" + fn);
            var logRoot = ConfigurationManager.AppSettings["LogFolder"];
            logRoot = string.IsNullOrEmpty(logRoot) ? "LogsRoot" : logRoot;
            return Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Site.Log", logRoot, fn);
        }


        private static object lockObj = new object();
        public void Write(string category, string source, LogType logType, string logMsg, string detail)
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

        public void Write(string category, string source, Exception exception)
        {
            lock (lockObj)
            {
                if (exception == null)
                {
                    return;
                }
                Write(category, source, LogType.Error, exception.Message, exception.ToString());
            }
        }
    }

    /// <summary>
    /// 错误日志缓存对象
    /// </summary>
    public class ErrorLogInfo
    {
        public string Key { get; set; }
        public int Index { get; set; }
        public DateTime LastUpdateTime { get; set; }
        //public List<byte> Buffer { get; set; }
    }
}
