using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.IO;

using System.Xml.Linq;
using EntityModel.Enum;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common;

namespace EntityModel.Xml
{
    /// <summary>
    /// 网站配置信息类
    /// </summary>
    public static class SettingConfigAnalyzer
    {
        private static string _configFileName = "";
        public static void RegConfigFile(string fileName)
        {
            _configFileName = fileName;
            var configWatcher = new FCFileWatcher(Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
            configWatcher.Changed += (object sender, FileSystemEventArgs e) =>
            {
                new Log4Log().WriteLog("ConfigChanged", "SettingConfigRegister.Configuration",(int) LogType.Information, "SettingConfigRegister.Configuration", "文件监控发现系统配置文件已更新");
                _allConfigList.Clear();
                xElement.Clear();
            };
            configWatcher.Start();
        }

        private static Dictionary<string, XmlElement> _allConfigList = new Dictionary<string, XmlElement>();
        /// <summary>
        /// 根据节点key值获取整个节点对象
        /// </summary>
        /// <param name="key">节点key值</param>
        /// <returns>节点对象</returns>
        public static XmlElement GetConfigElementByKey(string key)
        {
            if (_allConfigList.Count == 0)
            {
                lock (_configFileName)
                {
                    if (string.IsNullOrEmpty(_configFileName))
                    {
                        throw new ArgumentNullException("未设置配置文件 - SettingConfigAnalyzer");
                    }
                    if (!File.Exists(_configFileName))
                    {
                        throw new ArgumentNullException("设置的配置文件不存在 - SettingConfigAnalyzer - " + _configFileName);
                    }
                    var doc = new XmlDocument();
                    doc.Load(_configFileName);
                    foreach (XmlElement item in doc.SelectNodes("settings/config"))
                    {
                        var itemkey = item.GetAttribute("Key");
                        _allConfigList.Add(itemkey, item);
                    }
                }
            }
            return _allConfigList[key];
        }

        private static Dictionary<string, XElement> xElement = new Dictionary<string, XElement>();
        public static XElement GetConfigXElementByKey(string key)
        {
            if (xElement.Count == 0)
            {
                lock (_configFileName)
                {
                    var doc = XElement.Load(_configFileName);
                    foreach (var item in doc.Elements("config"))
                    {
                        var itemkey = item.Attribute("Key").Value;
                        xElement.Add(itemkey, item);
                    }
                }
            }
            return xElement[key];
        }


        /// <summary>
        /// 根据节点key值获取节点value，并转换为T对象
        /// </summary>
        /// <typeparam name="T">返回数据对象类型</typeparam>
        /// <param name="key">节点key值</param>
        /// <returns>返回数据</returns>
        public static T GetConfigValueByKey<T>(string key)
        {
            var value = GetConfigValueByKey(key, "Value");
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 根据节点key值获取节点value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValueByKey(string key)
        {
            return GetConfigValueByKey(key, "Value");
        }

        /// <summary>
        /// 根据节点key值以及属性名称获取属性值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static string GetConfigValueByKey(string key, string attrName)
        {
            var ele = GetConfigElementByKey(key);
            if (ele == null)
            {
                return null;
            }
            return ele.Attributes[attrName].Value;
        }
    }
}
