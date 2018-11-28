using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using Common.Cryptography;
using System.Xml;
using System.Configuration;
using System.IO;


namespace Common.Lottery.Gateway.Liangcai
{
    /// <summary>
    /// 亮彩
    /// </summary>
    public class GatewayHandler_LC
    {
        private static string _date;
        private static int _index = 1;
        public static string ServiceUrl { get; set; }
        public static string Key { get; set; }
        public static string PartnerId { get; set; }
        private static string _xmlDir;

        //测试平台接口地址：http://t.zc310.net:8089/bin/LotSaleHttp.dll
        //代理商编号：3821
        //代理商交易密钥：a8b8c8d8e8f8g8h8

        /// <summary>
        /// 得到开奖号码
        /// </summary>
        public string QueryWinNumber(string gameCode, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();

            var LotID = GetGameCode(gameCode);
            var LotIssue = issuseNumber;

            var wAction = 110;

            string wParam = string.Format("LotID={0}_LotIssue={1}", LotID, LotIssue);
            var doc = DoRequest(wAction, wParam);

            var info = DecodeResult(doc);
            if (info.Code == "0")
                return info.Value;
            return "";
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        public XmlDocument DoRequest(int action, string param)
        {
            //GatewayHandler.ServiceUrl = ConfigurationManager.AppSettings["LC_ServiceUrl"];
            //GatewayHandler.Key = ConfigurationManager.AppSettings["LC_Key"];
            //GatewayHandler.PartnerId = ConfigurationManager.AppSettings["LC_PartnerId"];

            var wMsgID = DateTime.Now.ToString("yyyyMMddHHmmss");
            string sign = string.Format("{0}{1}{2}{3}{4}", GatewayHandler_LC.PartnerId, action, wMsgID, param, GatewayHandler_LC.Key);
            string wSign = Encipherment.MD5(sign, Encoding.GetEncoding("GB2312")).ToLower();
            string request = string.Format("wAgent={0}&wAction={1}&wMsgID={2}&wSign={3}&wParam={4}", GatewayHandler_LC.PartnerId, action, wMsgID, wSign, param);
            if (!string.IsNullOrEmpty(_xmlDir) && action == 101)
            {
                SaveXML(param, action);
            }
            var xml = PostManager.Post(GatewayHandler_LC.ServiceUrl, request, Encoding.GetEncoding("gb2312"));
            if (string.IsNullOrEmpty(xml) || xml == "404")
                throw new Exception("返回结果不正确");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            if (!string.IsNullOrEmpty(_xmlDir) && action == 101)
            {
                var path = GetLogFileName(888);
                xmlDoc.Save(path);
            }
            return xmlDoc;
        }

        /// <summary>
        /// 解析回复结果
        /// </summary>
        public LC_ResposnseInfo DecodeResult(XmlDocument xmlDoc, string gameCode = "")
        {
            if (xmlDoc == null)
                return null;
            var root = xmlDoc.SelectSingleNode("ActionResult");
            if (root == null)
                throw new Exception("找不到该节点：ActionResult");

            var code = root.SelectSingleNode("xCode");
            var message = root.SelectSingleNode("xMessage");
            var value = root.SelectSingleNode("xValue");
            var spValue = string.Empty;
            if (gameCode.Equals("JCZQ"))
            {
                XmlElement xe = (XmlElement)value;
                XmlNodeList subList = xe.ChildNodes;
                var spTicketL = new List<string>();
                foreach (XmlNode xmlNode in subList)
                {
                    var spL = new List<string>();
                    var id = xmlNode.Attributes["id"].Value;
                    var blList = xmlNode.ChildNodes;
                    foreach (XmlNode bl in blList)
                    {
                        XmlNodeList matchList = bl.ChildNodes;
                        foreach (XmlNode m in matchList)
                        {
                            if ("match".Equals(m.Name))
                            {
                                spL.Add(string.Format("{0}_{1}", m.Attributes["id"].Value, m.InnerText));
                            }
                        }
                    }
                    spTicketL.Add(string.Format("{0}^{1}", id, string.Join("/", spL)));
                }
                //var array = spL.GroupBy(p => p).Select(p => p.Key).ToArray();
                spValue = string.Join("*", spTicketL);
            }
            var msgId = root.SelectSingleNode("xMsgID");
            var sign = root.SelectSingleNode("xSign");

            return new LC_ResposnseInfo
            {
                Code = code.InnerText,
                Message = message.InnerText,
                MsgId = msgId.InnerText,
                Sign = sign.InnerText,
                Value = gameCode.Equals("JCZQ") ? spValue : value.InnerText,
            };
        }

        public int GetGameCode(string gameCode, string gameType = "")
        {
            var game = 0;
            switch (gameCode)
            {
                case "PL3":
                    game = 33;
                    break;
                case "SSQ":
                    game = 51;
                    break;
                case "FC3D":
                    game = 52;
                    break;
                case "CQSSC":
                    game = 10401;
                    break;
                case "JX11X5":
                    game = 23009;
                    break;
                case "DLT":
                    game = 23529;
                    break;
                case "JCZQ":
                    game = 42;
                    break;
                case "BJDC":
                    game = 41;
                    break;
                case "JCLQ":
                    game = 43;
                    break;
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            game = 11;
                            break;
                        case "TR9":
                            game = 19;
                            break;
                        case "T6BQC":
                            game = 16;
                            break;
                        case "T4CJQ":
                            game = 18;
                            break;
                        default:
                            break;
                    }
                    break;
            }
            return game;
        }

        public static void SetXmlDir(string root)
        {
            _xmlDir = root;
        }
        public static void SaveXML(string content, int action)
        {
            var path = GetLogFileName(action);
            CreateXml(path);
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            if (doc != null)
            {
                var node = doc.SelectSingleNode("List");
                if (node == null)
                {
                    XmlElement xe = doc.CreateElement("List");
                    doc.AppendChild(xe);
                }
                var strList = content.Split('^');
                foreach (var item in strList)
                {
                    XmlElement xe = doc.CreateElement("item");
                    var orderList = item.Split('_');
                    foreach (var order in orderList)
                    {
                        var node_order = order.Split('=')[0];
                        var node_value = string.Empty;
                        if (order.Split('=').Length > 2)
                        {
                            var start = order.IndexOf("=") + "=".Length;
                            node_value = order.Substring(start);
                        }
                        else
                        {
                            node_value = order.Split('=')[1];
                        }
                        XmlElement xe1 = doc.CreateElement(node_order);
                        xe1.InnerText = node_value;
                        xe.AppendChild(xe1);
                    }
                    node.AppendChild(xe);
                }
            }
            doc.Save(path);
        }
        private static string GetLogFileName(int action)
        {
            string str = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (str == _date)
            {
                _index++;
            }
            else
            {
                _date = str;
                _index = 1;
            }
            return Path.Combine(new string[] { _xmlDir, "LiangCai", "Ticket", DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Now.Hour.ToString("D2"), string.Format("{0}-{1}-{2:D3}.xml", new object[] { action, _date, _index }) });
        }

        private static void CreateXml(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            string directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            if (!File.Exists(path))
            {
                using (FileStream fi = new FileStream(path, FileMode.Create))
                {
                    fi.Close();
                }
                XmlDocument doc = new XmlDocument();
                XmlNode node = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(node);

                XmlElement listNode = doc.CreateElement("List");
                doc.AppendChild(listNode);
                doc.Save(path);
            }
        }

    }
}
