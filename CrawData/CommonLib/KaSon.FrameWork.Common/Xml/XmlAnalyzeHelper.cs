using EntityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace KaSon.FrameWork.Common.Xml
{
    public class XmlAnalyzeHelper
    {
        public static T AnalyseResponse<T>(string body, string root)
            where T : XmlMappingObject, new()
        {
            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                    return AnalyseXmlToCommunicationObject<T>(body, root);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new XmlException("解析完整请求错误。" + body, ex);     
            }
        }

        public static T AnalyseXmlToCommunicationObject<T>(string xml, string root)
            where T : XmlMappingObject, new()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return AnalyseXmlToCommunicationObject<T>(root, doc);
            }
            catch (Exception ex)
            {
                throw new XmlException("解析XML错误。" + xml, ex);
            }
        }

        public static T AnalyseXmlPathToCommunicationObject<T>(string xmlPath)
            where T : XmlMappingObject, new()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);
                return AnalyseXmlToCommunicationObject<T>("message", doc);
            }
            catch (Exception ex)
            {
                throw new XmlException("解析XML路径错误。" + xmlPath, ex);
            }
        }

        private static T AnalyseXmlToCommunicationObject<T>(string root, XmlDocument doc)
            where T : XmlMappingObject, new()
        {
            T t = new T();
            t.XmlHeader = doc.FirstChild.OuterXml;
            XmlNodeList nodeList = doc.GetElementsByTagName(root);
            if (nodeList.Count > 0)
            {
                t.AnalyzeXmlNode(nodeList[0]);
                return t;
            }
            return null;
        }

        public static T AnalyseXmlPathToMappingObject<T>(string xmlPath)
            where T : XmlMappingObject, new()
        {
            return AnalyseXmlPathToMappingObject<T>(xmlPath, "message");
        }

        public static T AnalyseXmlPathToMappingObject<T>(string xmlPath, string root)
            where T : XmlMappingObject, new()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);
                return AnalyseXmlToMappingObject<T>(root, doc);
            }
            catch (Exception ex)
            {
                throw new XmlException("解析XML路径" + root + "错误。" + xmlPath, ex);
            }
        }

        public static T AnalyseXmlToMappingObject<T>(string xml)
            where T : XmlMappingObject, new()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return AnalyseXmlToMappingObject<T>("message", doc);
            }
            catch (Exception ex)
            {
                throw new XmlException("解析XML错误。" + xml, ex);
            }
        }

        private static T AnalyseXmlToMappingObject<T>(string root, XmlDocument doc) where T : XmlMappingObject, new()
        {
            T t = new T();
            XmlNodeList nodeList = doc.GetElementsByTagName(root);
            if (nodeList.Count > 0)
            {
                t.AnalyzeXmlNode(nodeList[0]);
                return t;
            }
            return null;
        }

        public static Dictionary<string, string> GetParameters(string body)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                foreach (string item in body.Split('&'))
                {
                    string[] keyValue = item.Split(new char[] { '=' }, 2);
                    parameters.Add(keyValue[0], keyValue[1]);
                }
                return parameters;
            }
            catch (Exception ex)
            {
                throw new XmlException("解析请求文本，并返回参数列表错误。" + body, ex);
            }
        }
    }
}
