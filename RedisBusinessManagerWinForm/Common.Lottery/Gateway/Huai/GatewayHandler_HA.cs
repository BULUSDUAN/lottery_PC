using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Cryptography;
using System.IO;
using Common.Net;

namespace Common.Lottery.Gateway.HuAi
{
    /// <summary>
    /// 互爱科技
    /// </summary>
    public class GatewayHandler_HA
    {
        private static string _date;
        private static int _index = 1;
        /// <summary>
        /// 代理商编号，由销售平台统一分配的能唯一标识代理商的一个整数编号
        /// </summary>
        public static string exAgent { get; set; }
        /// <summary>
        /// 代理商密钥
        /// </summary>
        public static string Key { get; set; }
        private static string _xmlDir;
        public static string ServiceUrl { get; set; }


        /// <summary>
        /// 发送请求
        /// </summary>
        public XmlDocument DoRequest(int exAction, string param)
        {
            var exMsgID = DateTime.Now.ToString("yyyyMMddHHmmss");
            string sign = string.Format("{0}{1}{2}{3}{4}", exAgent, exAction, exMsgID, param, Key);
            string wSign = Encipherment.MD5(sign, Encoding.UTF8).ToLower();
            string request = string.Format("exAgent={0}&exAction={1}&exMsgID={2}&exParam={3}&exSign={4}", exAgent, exAction, exMsgID, param, wSign);
            if (!string.IsNullOrEmpty(_xmlDir) && exAction == 101)
            {
                SaveXML(param, exAction);
            }
            var xml = PostManager.Post(ServiceUrl, request, Encoding.UTF8);//.GetEncoding("gb2312")
            if (string.IsNullOrEmpty(xml) || xml == "404")
                throw new Exception("返回结果不正确");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            if (!string.IsNullOrEmpty(_xmlDir) && exAction == 101)
            {
                var path = GetLogFileName(888);
                xmlDoc.Save(path);
            }
            return xmlDoc;
        }

        /// <summary>
        /// 解析回复结果
        /// </summary>
        public HuAi_ResponseInfo DecodeResult(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return null;
            var root = xmlDoc.SelectSingleNode("ActionResult");
            if (root == null)
                throw new Exception("找不到该节点：ActionResult");

            var code = root.SelectSingleNode("reCode");
            var message = root.SelectSingleNode("reMessage");
            var value = root.SelectSingleNode("reValue");
            var spValue = string.Empty;
            var msgId = root.SelectSingleNode("reMsgID");
            var sign = root.SelectSingleNode("reSign");

            return new HuAi_ResponseInfo
            {
                reCode = int.Parse(code.InnerText),
                reMessage = message.InnerText,
                reMsgID = msgId.InnerText,
                reSign = sign.InnerText,
                reValue = value == null ? string.Empty : value.InnerText,
            };
        }

        /// <summary>
        /// 解析投注回复结果
        /// </summary>
        public HuAi_ResponseInfo DecodeResult_Bet(XmlDocument xmlDoc, out List<string> strL)
        {
            strL = new List<string>();
            if (xmlDoc == null)
                return null;
            var root = xmlDoc.SelectSingleNode("ActionResult");
            if (root == null)
                throw new Exception("找不到该节点：ActionResult");

            var code = root.SelectSingleNode("reCode");
            var message = root.SelectSingleNode("reMessage");
            var value = root.SelectSingleNode("reValue");
            var spValue = string.Empty;
            var msgId = root.SelectSingleNode("reMsgID");
            var sign = root.SelectSingleNode("reSign");
            if (code.InnerText != "0")
            {
                XmlElement xe = (XmlElement)value;
                XmlNodeList subList = xe.ChildNodes;
                var spTicketL = new List<string>();
                foreach (XmlNode xmlNode in subList)
                {
                    var id = xmlNode.Attributes["OrderID"].Value;
                    var error = xmlNode.Attributes["Error"].Value;
                    strL.Add(string.Format("{0}^{1}^{2}", id, error, CodeToMessage(error)));
                }
            }

            return new HuAi_ResponseInfo
            {
                reCode = int.Parse(code.InnerText),
                reMessage = message.InnerText,
                reMsgID = msgId.InnerText,
                reSign = sign.InnerText,
                reValue = value == null ? string.Empty : value.InnerText,
            };
        }

        /// <summary>
        /// 解析回复结果-竞彩
        /// </summary>
        public Dictionary<string, string> DecodeResult_JC(XmlDocument xmlDoc, Dictionary<string, string> ticket_BetContent = null)
        {
            Dictionary<string, string> resultList = new Dictionary<string, string>();
            if (xmlDoc == null)
                return null;
            var root = xmlDoc.SelectSingleNode("ActionResult");
            if (root == null)
                throw new Exception("找不到该节点：ActionResult");

            var code = root.SelectSingleNode("reCode");
            if (code.InnerText != "0")
                return resultList;
            var message = root.SelectSingleNode("reMessage");
            var value = root.SelectSingleNode("reValue");
            var msgId = root.SelectSingleNode("reMsgID");
            var sign = root.SelectSingleNode("reSign");
            var spValue = string.Empty;
            XmlElement xe = (XmlElement)value;
            XmlNodeList subList = xe.ChildNodes;
            var spTicketL = new List<string>();
            foreach (XmlNode xmlNode in subList)
            {
                var spL = new List<string>();
                var id = xmlNode.Attributes["OrderID"].Value;
                var statu = xmlNode.Attributes["Status"].Value;
                if (!(statu == "2" || statu == "4")) continue;

                //<Ticket Seq="1" Status="Y" Errmsg="出票成功" TicketID="9015062500000034" GroundID="71927742972271276729" Odds="3.20-4.10" />
                var blList = xmlNode.ChildNodes;
                //tt1214|JCZQ937590171|001      BRQSPF_150626002_1/BF_150626003_51/ZJQ_150626004_7
                var content = ticket_BetContent.Where(p => p.Key.Contains(id)).FirstOrDefault();
                if (string.IsNullOrEmpty(content.Key)) continue;

                var cl = content.Value.Split('/');

                var oddsL = xmlNode.FirstChild.Attributes["Odds"].Value.Split('-');
                //出票票号
                var groundID = xmlNode.FirstChild.Attributes["GroundID"].Value;
                var errmsg = xmlNode.FirstChild.Attributes["Errmsg"].Value;
                var strL = new List<string>();
                for (int i = 0; i < cl.Length; i++)
                {
                    if (statu == "4") continue;
                    var match = cl[i].Split('_');
                    if (match.Length < 2)
                        return resultList;
                    if (match.Length > 2)
                        strL.Add(Ticket_BetOdds(match[1], match[2], oddsL[i]));
                    else
                        strL.Add(Ticket_BetOdds(match[0], match[1], oddsL[i]));
                }
                resultList.Add(content.Key, string.Format("{0}^{1}^{2}^{3}", statu, string.Join("/", strL), groundID, errmsg));
            }
            return resultList;
        }

        /// <summary>
        /// 解析回复结果-数字彩
        /// </summary>
        public Dictionary<string, string> DecodeResult_SZC(XmlDocument xmlDoc)
        {
            Dictionary<string, string> resultList = new Dictionary<string, string>();
            if (xmlDoc == null)
                return null;
            var root = xmlDoc.SelectSingleNode("ActionResult");
            if (root == null)
                throw new Exception("找不到该节点：ActionResult");

            var code = root.SelectSingleNode("reCode");
            if (code.InnerText != "0")
                return resultList;
            var message = root.SelectSingleNode("reMessage");
            var value = root.SelectSingleNode("reValue");
            var msgId = root.SelectSingleNode("reMsgID");
            var sign = root.SelectSingleNode("reSign");
            XmlElement xe = (XmlElement)value;
            XmlNodeList subList = xe.ChildNodes;
            foreach (XmlNode xmlNode in subList)
            {
                var id = xmlNode.Attributes["OrderID"].Value;
                var statu = xmlNode.Attributes["Status"].Value;
                if (!(statu == "2" || statu == "4")) continue;

                //出票票号
                var groundID = xmlNode.FirstChild.Attributes["GroundID"].Value;
                var errmsg = xmlNode.FirstChild.Attributes["Errmsg"].Value;
              
                resultList.Add(id, string.Format("{0}^{1}^{2}", statu, groundID, errmsg));
            }
            return resultList;
        }

        public static string Ticket_BetOdds(string matchId, string betContent, string sp)
        {
            var strl = new List<string>();
            var bl = betContent.Split(',');
            sp = sp.Split('@')[0];
            var sl = sp.Split('A');
            if (bl.Length != sl.Length)
                return string.Empty;
            for (int i = 0; i < bl.Length; i++)
            {
                strl.Add(string.Format("{0}|{1}", bl[i], sl[i]));
            }
            return string.Format("{0}_{1}", matchId, string.Join(",", strl));
        }

        public static string CodeToMessage(string code)
        {
            switch (code)
            {
                case "0":
                    return "成功";
                case "101":
                    return "参数错误";
                case "102":
                    return "签名验证失败";
                case "103":
                    return "不支持的功能请求";
                case "104":
                    return "IP 限制";
                case "105":
                    return "商户被禁用";
                case "106":
                    return "系统繁忙，请再次尝试。";
                case "201":
                    return "查询订单不存在";
                case "202":
                    return "查询奖期不存在";
                case "301":
                    return "10秒内禁止重复发单";
                case "302":
                    return "投注格式不正确或玩法不支持";
                case "303":
                    return "订单投注倍数超最大限制";
                case "304":
                    return "投注票金额不正确 ";
                case "305":
                    return "投注期次过期或期次不存在";
                case "306":
                    return "订单号重复";
                case "307":
                    return "订单创建失败";
                case "308":
                    return "余额不足";
                case "401":
                    return "投注号码限号";
                case "402":
                    return "彩种暂停销售";
                case "403":
                    return "期次取消";
                default:
                    return string.Empty;
            }
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
            return Path.Combine(new string[] { _xmlDir, "HuAi", "Ticket", DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Now.Hour.ToString("D2"), string.Format("{0}-{1}-{2:D3}.xml", new object[] { action, _date, _index }) });
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
