using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Utilities;
using System.IO;
using Common.Log;
using Common.Net.SMS;
using System.Xml.Linq;

namespace Common.XmlAnalyzer
{
    /// <summary>
    /// 敏感字管理
    /// </summary>
    public static class SensitiveAnalyzer
    {
        private static string _configFileName = "";
        public static void RegConfigFile(string fileName)
        {
            _configFileName = fileName;
            var configWatcher = new FCFileWatcher(Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
            configWatcher.Changed += (object sender, FileSystemEventArgs e) =>
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("SensitiveChanged", "SensitiveRegister.Sensitive", LogType.Information, "SensitiveRegister.Sensitive", "文件监控发现系统敏感文件已更新");
                UpdateSensitive();
            };
            configWatcher.Start();
        }

        private static string[] _sensitiveList = new string[] { };
        private static void UpdateSensitive()
        {
            var txt = ReadFileString(_configFileName);
            _sensitiveList = txt.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 判断字符串是否包含敏感词
        /// </summary>
        /// <param name="word">目标字符串</param>
        /// <returns></returns>
        public static bool IsHaveSensitive(string word)
        {
            try
            {
                return _sensitiveList.Where(a => word.Contains(a)).Count() > 0;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        /// <summary>
        /// 读取物理文件路径
        /// </summary>
        /// <param name="fileName">文件物理地址</param>
        /// <returns>文件内容</returns>
        private static string ReadFileString(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
