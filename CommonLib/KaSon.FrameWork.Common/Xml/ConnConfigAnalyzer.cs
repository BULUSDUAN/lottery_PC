using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using KaSon.FrameWork.Common.Database;

namespace KaSon.FrameWork.Common.Xml
{
    /// <summary>
    /// 配置文件中，数据库连接配置分析器
    /// </summary>
    public static class ConnConfigAnalyzer
    {
        public static List<ConnConfigInfo> AnalyzeSection(XmlDocument doc, Type baseType)
        {
            var list = new List<ConnConfigInfo>();
            var cfgSections = doc.SelectNodes("configuration/configSections/section");
            foreach (XmlNode node in cfgSections)
            {
                try
                {
                    if (node.Attributes["type"] != null)
                    {
                        var str = node.Attributes["type"].Value;
                        var name = node.Attributes["name"].Value;
                        var type = Type.GetType(str);
                        if (baseType.IsAssignableFrom(type))
                        {
                            var ele = doc.SelectSingleNode("configuration/" + name);
                            if (ele != null)
                            {
                                var isEncrypt = false;
                                if (ele.Attributes["UseEncryption"] != null)
                                {
                                    isEncrypt = bool.Parse(ele.Attributes["UseEncryption"].Value);
                                }
                                var info = new ConnConfigInfo
                                {
                                    IsSuccess = true,
                                    ErrMessage = "配置正确",
                                    ConnConfigName = name,
                                    ConnType = "common.database",
                                    HandleType = type,
                                    IsEncrypt = isEncrypt,
                                    ItsNode = node,
                                };
                                list.Add(info);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var info = new ConnConfigInfo
                    {
                        IsSuccess = false,
                        ErrMessage = ex.Message,
                    };
                    list.Add(info);
                }
            }
            return list;
        }
        public static void SaveSection(string fileName, string configName, string serverName, bool isIntegrate, string userId, string password, string dbName, string handleTypeName, ICryptConnection cryptObj)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            var root = doc.SelectSingleNode("configuration/configSections/section[@name='" + configName + "']");
            if (root != null)
            {
                root.Attributes["type"].Value = handleTypeName;
            }
            else
            {
                throw new ArgumentException("数据库配置节点不存在 - " + "configuration/configSections/section[@name='" + configName + "']");
            }
            var node = doc.SelectSingleNode("configuration/" + configName);
            if (node != null)
            {
                string connectionFormat = "{0}Data Source={1};Initial Catalog={2};Persist Security Info=True;";
                string connString = string.Format(connectionFormat, isIntegrate ? "Integrated Security=SSPI;" : "", serverName, dbName);
                if (isIntegrate)
                {
                    userId = "";
                    password = "";
                }
                else
                {
                    userId = cryptObj.EncryptConnString(userId);
                    password = cryptObj.EncryptConnString(password);
                }
                node.Attributes["UseEncryption"].Value = "true";
                node.Attributes["PartialConnectionString"].Value = connString;
                node.Attributes["User"].Value = userId;
                node.Attributes["Password"].Value = password;
            }
            else
            {
                throw new ArgumentException("数据库配置节点不存在 - " + "configuration/" + configName);
            }
            doc.Save(fileName);
        }
        public static List<ConnConfigInfo> AnalyzeFactory(XmlDocument doc, Type baseType)
        {
            var list = new List<ConnConfigInfo>();
            var xmlnsm = new XmlNamespaceManager(doc.NameTable);
            xmlnsm.AddNamespace("urn", "urn:nhibernate-configuration-2.2");
            var factories = doc.SelectNodes("urn:hibernate-configuration/urn:session-factory", xmlnsm);
            foreach (XmlNode node in factories)
            {
                try
                {
                    var name = node.Attributes["name"].Value;
                    var isEncrypt = false;
                    Type type = null;
                    foreach (XmlNode prop in node.SelectNodes("urn:property", xmlnsm))
                    {
                        if (prop.Attributes["name"].Value.Equals("connection.provider", StringComparison.OrdinalIgnoreCase))
                        {
                            type = Type.GetType(prop.InnerText);
                            if (baseType.IsAssignableFrom(type))
                            {
                                isEncrypt = true;
                            }
                            break;
                        }
                    }
                    var info = new ConnConfigInfo
                    {
                        IsSuccess = true,
                        ErrMessage = "配置正确",
                        ConnConfigName = name,
                        ConnType = "nhibernate",
                        HandleType = type,
                        IsEncrypt = isEncrypt,
                        ItsNode = node,
                    };
                    list.Add(info);
                }
                catch (Exception ex)
                {
                    var info = new ConnConfigInfo
                    {
                        IsSuccess = false,
                        ErrMessage = ex.Message,
                    };
                    list.Add(info);
                }
            }
            return list;
        }
        public static void SaveFactory(string fileName, XmlNode itsNode, string serverName, bool isIntegrate, string userId, string password, string dbName, string handleTypeName, ICryptConnection cryptObj)
        {
            var connStr = CreateMssqlConnectionString(serverName, isIntegrate, userId, password, dbName);
            var encode = cryptObj.EncryptConnString(connStr);

            var xmlnsm = new XmlNamespaceManager(itsNode.OwnerDocument.NameTable);
            xmlnsm.AddNamespace("urn", "urn:nhibernate-configuration-2.2");
            var connection = itsNode.SelectSingleNode("urn:property[@name='connection.connection_string']", xmlnsm);
            if (connection == null)
            {
                connection = itsNode.OwnerDocument.CreateElement("property", "waiting delete");
                var attr = itsNode.OwnerDocument.CreateAttribute("name");
                attr.Value = "connection.connection_string";
                connection.InnerText = encode;
                connection.Attributes.Append(attr);
                itsNode.PrependChild(connection);
            }
            else
            {
                connection.InnerText = encode;
            }
            var provider = itsNode.SelectSingleNode("urn:property[@name='connection.provider']", xmlnsm);
            if (provider == null)
            {
                provider = itsNode.OwnerDocument.CreateElement("property", "waiting delete");
                var attr = itsNode.OwnerDocument.CreateAttribute("name");
                attr.Value = "connection.provider";
                provider.InnerText = handleTypeName;
                provider.Attributes.Append(attr);
                itsNode.PrependChild(provider);
            }
            else
            {
                provider.InnerText = handleTypeName;
            }
            itsNode.OwnerDocument.InnerXml = itsNode.OwnerDocument.InnerXml.Replace(" xmlns=\"waiting delete\"", "");
            itsNode.OwnerDocument.Save(fileName);
        }
        private static string CreateMssqlConnectionString(string serverName, bool isIntegrate, string userId, string password, string dbName)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = serverName;
            builder.InitialCatalog = dbName;
            builder.IntegratedSecurity = isIntegrate;
            builder.UserID = userId;
            builder.Password = password;
            return builder.ConnectionString;
        }
    }
    public class ConnConfigInfo
    {
        public bool IsSuccess { get; set; }
        public string ErrMessage { get; set; }
        /// <summary>
        /// 数据库连接配置名称
        /// </summary>
        public string ConnConfigName { get; set; }
        /// <summary>
        /// 连接类型。nhibernate、common.database等等 字符串枚举
        /// </summary>
        public string ConnType { get; set; }
        /// <summary>
        /// 是否已加密
        /// </summary>
        public bool IsEncrypt { get; set; }
        /// <summary>
        /// 处理数据库连接及加密的类型
        /// </summary>
        public Type HandleType { get; set; }
        /// <summary>
        /// 所属XML节点
        /// </summary>
        internal XmlNode ItsNode { get; set; }

        public bool SaveConnConfig(string fileName, string serverName, bool isIntegrate, string userId, string password, string dbName, string handleTypeName, ICryptConnection cryptObj, out string errMsg)
        {
            try
            {
                if (!IsSuccess)
                {
                    errMsg = ErrMessage;
                    return false;
                }
                if (ItsNode == null)
                {
                    errMsg = "未绑定到XML节点";
                    return false;
                }
                if (ConnType.Equals("nhibernate", StringComparison.OrdinalIgnoreCase))
                {
                    ConnConfigAnalyzer.SaveFactory(fileName, ItsNode, serverName, isIntegrate, userId, password, dbName, handleTypeName, cryptObj);
                }
                else if (ConnType.Equals("common.database", StringComparison.OrdinalIgnoreCase))
                {
                    ConnConfigAnalyzer.SaveSection(fileName, ConnConfigName, serverName, isIntegrate, userId, password, dbName, handleTypeName, cryptObj);
                }
                else
                {
                    errMsg = "不支持的数据库连接类型 - " + ConnType;
                    return false;
                }
                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }
        public override string ToString()
        {
            return ConnConfigName;
        }
    }
}
