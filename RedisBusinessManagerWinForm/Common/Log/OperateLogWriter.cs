using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common.Log
{
    /// <summary>
    /// 操作日志记录器。作为文件记录
    /// </summary>
    [Serializable]
    public class OperateLogWriter : ILogWriter
    {
        /// <summary>
        /// 将消息添加到日志文件
        /// </summary>
        /// <param name="text">消息内容</param>  
        private void AppendTextToFile(string text, string category, string source)
        {
            string FileName = CreateFileName(category, source);
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
        private string CreateFileName(string category, string source)
        {
            string fn = string.Format(@"{2:yyyy-MM\\dd\\HH}\[{0}].[{1}]", category, source, DateTime.Now) + ".log";
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"OperateLogs\" + fn);
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
}
