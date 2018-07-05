using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.IO;


using EntityModel.Enum;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common;

namespace KaSon.FrameWork.Common.Xml
{
    /// <summary>
    /// 代理商域名映射配置信息类 KaSon.FrameWork.Analyzer.
    /// </summary>
    public static class AgentHostMappingConfigAnalyzer
    {
        private static string _configFileName = "";
        public static void RegConfigFile(string fileName)
        {
            _configFileName = fileName;
            var configWatcher = new FCFileWatcher(Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
            configWatcher.Changed += (object sender, FileSystemEventArgs e) =>
            {
               new Log4Log().WriteLog("ConfigChanged", "AgentMapping.Config",(int) LogType.Information, "AgentMapping.Config", "文件监控发现代理商域名映射配置信息文件已更新");
                _allAgentList.Clear();
            };
            configWatcher.Start();
        }

        private static Dictionary<string, string> _allAgentList = new Dictionary<string, string>();

        /// <summary>
        /// 获取代理商ID
        /// </summary>
        /// <param name="hostName">需要获取的域名</param>
        /// <returns></returns>
        public static string GetAgentIdByHostName(string hostName)
        {
            try
            {
                UpdateAgentList();

                if (_allAgentList.ContainsKey(hostName))
                {
                    return _allAgentList[hostName].Split('|')[0];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取代理商站点名称
        /// </summary>
        /// <param name="hostName">需要获取的域名</param>
        /// <returns></returns>
        public static string GetSiteNameByHostName(string hostName)
        {
            try
            {
                UpdateAgentList();

                if (_allAgentList.ContainsKey(hostName))
                {
                    return _allAgentList[hostName].Split('|')[2];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取代理商站点LOGO样式名称
        /// </summary>
        /// <param name="hostName">需要获取的域名</param>
        /// <returns></returns>
        public static string GetSiteLogoByHostName(string hostName)
        {
            try
            {
                UpdateAgentList();

                if (_allAgentList.ContainsKey(hostName))
                {
                    return _allAgentList[hostName].Split('|')[1];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取代理商对应的域名
        /// </summary>
        /// <param name="agentId">代理商ID</param>
        /// <returns></returns>
        public static string GetHostNameByAgentId(string agentId)
        {
            try
            {
                UpdateAgentList();

                return _allAgentList.Where(a => a.Value.Split('|').Contains(agentId)).FirstOrDefault().Key;

            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 更新代理商映射列表
        /// </summary>
        private static void UpdateAgentList()
        {
            try
            {
                if (_allAgentList.Count == 0)
                {
                    lock (_configFileName)
                    {
                        if (string.IsNullOrEmpty(_configFileName))
                        {
                            throw new ArgumentNullException("未设置配置文件 - AgentHostMappingConfigAnalyzer");
                        }
                        if (!File.Exists(_configFileName))
                        {
                            throw new ArgumentNullException("设置的配置文件不存在 - AgentHostMappingConfigAnalyzer - " + _configFileName);
                        }
                        XmlDocument doc = new XmlDocument();
                        doc.Load(_configFileName);
                        var mappings = doc.GetElementsByTagName("mapping");
                        foreach (XmlElement e in mappings)
                        {
                            _allAgentList.Add(e.Attributes["host"].Value, e.Attributes["pid"].Value + "|" + e.Attributes["logo"].Value + "|" + e.Attributes["sitename"].Value);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
