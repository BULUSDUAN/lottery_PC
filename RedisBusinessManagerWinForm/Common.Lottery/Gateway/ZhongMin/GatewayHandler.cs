using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.XmlAnalyzer;
using Common.Business;
using System.Text.RegularExpressions;
using Common.Net;
using Common.Algorithms;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Common.Lottery.Gateway.ZhongMin
{
    public class GatewayHandler
    {
        // Fields
        private static string _date;
        private static int _index = 1;
        private static string _xmlDir;

        public static string ServiceUrl { get; set; }
        public static string Key { get; set; }
        public static string PartnerId { get; set; }
        public static string Version { get; set; }


        // Methods
        private string ConvertGameCode(string gameCode)
        {
            gameCode = gameCode.ToLower();
            switch (gameCode)
            {
                case "bjdc":
                    return "bd";
                default:
                    return gameCode;
            }
        }

        private string ConvertGameCode(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "zjq":
                            return "JCJQS";

                        case "hh":
                            return "JCZQFH";
                    }
                    return ("JC" + gameType);

                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sfc":
                            return "JCFC";

                        case "hh":
                            return "JCLQFH";
                    }
                    return ("JC" + gameType);

                case "bjdc":
                    string str4;
                    if (((str4 = gameType.ToLower()) == null) || !(str4 == "zjq"))
                    {
                        return gameType;
                    }
                    return "JQS";

                case "ctzq":
                    switch (gameType.ToLower())
                    {
                        case "t14c":
                            return "14CSF";

                        case "tr9":
                            return "SFR9";

                        case "t6bqc":
                            return "6CBQ";

                        case "t4cjq":
                            return "4CJQ";
                    }
                    return "";

                case "fc3d":
                    return "3D";

                case "cqssc":
                    return "ZQSSC";
            }
            return gameCode;
        }

        private string ConvertGameType(string gameCode, string gameType, string betType)
        {
            string str = gameCode.ToLower();
            if (str != null)
            {
                string str2;
                if (!(str == "jczq") && !(str == "bjdc"))
                {
                    if (str == "ctzq")
                    {
                        return betType;
                    }
                    if (str != "dlt")
                    {
                        return gameType;
                    }
                }
                else
                {
                    return "FS";
                }
                if (((str2 = gameType.ToLower()) != null) && (str2 == "dt"))
                {
                    return "FS";
                }
            }
            return gameType;
        }

        private string ConvertIssuseNumber(string gameCode, string issuseNumber)
        {
            string str = gameCode.ToLower();
            if (str != null)
            {
                if (!(str == "ssq"))
                {
                    if ((str == "dlt") && issuseNumber.Contains("-"))
                    {
                        return issuseNumber.Replace("-", "").Substring(2);
                    }
                    return issuseNumber;
                }
                if (issuseNumber.Contains("-"))
                {
                    return issuseNumber.Replace("-", "");
                }
            }
            return issuseNumber;
        }

        private static string GetLogFileName(XmlMappingObject messageObject)
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
            string str2 = messageObject.GetPrimaryKey().Replace("<", ".").Replace(">", ".").Replace(":", ".").Replace(@"\", ".").Replace("/", ".").Replace("*", ".").Replace("?", ".").Replace("|", ".").Replace("\"", ".");
            return Path.Combine(new string[] { _xmlDir, "ZhongMin",  CommandDefinition.GetCommandCategory(messageObject.GetCode()), DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Now.Hour.ToString("D2"), string.Format("{0}-{1}-{2}-{3:D3}.xml", new object[] { messageObject.GetCode(), str2, _date, _index }) });
        }

        public static string GetRequestMessageXml(XmlMappingObject messageObject)
        {
            string str = messageObject.ToInnerXmlString("body");
            MessageRequestInfo info2 = new MessageRequestInfo();
            MessageRequestInfo.MessageHead head = new MessageRequestInfo.MessageHead();
            head.Transcode = CommandDefinition.GetCommandByRequestObjectType(messageObject.GetType()).CommandCode;
            head.Partnerid = PartnerId;
            head.Version = Version;
            head.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            info2.Head = head;
            info2.Body = str;
            MessageRequestInfo info = info2;
            string str2 = info.ToXmlString("msg").Replace("&gt;", ">").Replace("&lt;", "<");
            return string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", new object[] { info.Head.Transcode, str2, MessageHelper.GetMd5Body(info.Head.Transcode + str2 + Key), PartnerId });
        }

        private static string GetXMl(string xmlstr)
        {
            Match match = new Regex("&msg=(?<value0>.*?)&key=").Match(xmlstr);
            if (match.Groups[0].Length > 0)
            {
                return match.Groups[1].Value;
            }
            return "";
        }

        public decimal QueryBalance()
        {
            BalanceRequestInfo info3 = new BalanceRequestInfo();
            PartnerAccount account = new PartnerAccount();
            account.PartnerId = PartnerId;
            info3.PartnerAccount = account;
            BalanceRequestInfo messageObject = info3;
            BalanceResponseInfo info2 = RequestMessage<BalanceResponseInfo>(messageObject);
            if ((info2.ErrInfo != null) && (info2.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info2.ErrInfo.Message);
            }
            return info2.PartnerAccount.Balance;
        }

        public IssuseQueryResponseInfo QueryCurrentIssuse(string gameCode, string gameType)
        {
            IssuseQueryRequestInfo info3 = new IssuseQueryRequestInfo();
            IssuseQueryRequestInnerInfo info4 = new IssuseQueryRequestInnerInfo();
            info4.LotteryId = this.ConvertGameCode(gameCode, gameType);
            info3.IssuseQueryInfo = info4;
            IssuseQueryRequestInfo messageObject = info3;
            IssuseQueryResponseInfo info2 = RequestMessage<IssuseQueryResponseInfo>(messageObject);
            if ((info2.ErrInfo != null) && (info2.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info2.ErrInfo.Message);
            }
            return info2;
        }

        public BJDC_GameQueryResponseInfo QueryMatchList_BJDC(string gameCode, string gameType, string issuseNumber)
        {
            BJDC_GameQueryRequestInfo info3 = new BJDC_GameQueryRequestInfo();
            BJDC_GameQueryRequestInnerInfo info4 = new BJDC_GameQueryRequestInnerInfo();
            info4.LotteryId = this.ConvertGameCode(gameCode, gameType);
            info4.IssuseNumber = this.ConvertIssuseNumber(gameCode, issuseNumber);
            info3.GameQueryInfo = info4;
            BJDC_GameQueryRequestInfo messageObject = info3;
            BJDC_GameQueryResponseInfo info2 = RequestMessage<BJDC_GameQueryResponseInfo>(messageObject);
            if ((info2.ErrInfo != null) && (info2.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info2.ErrInfo.Message);
            }
            return info2;
        }

        public JC_GameQueryResponseInfo QueryMatchList_JC(string gameCode, string gameType, string issuseNumber)
        {
            JC_GameQueryRequestInfo info3 = new JC_GameQueryRequestInfo();
            JC_GameQueryRequestInnerInfo info4 = new JC_GameQueryRequestInnerInfo();
            info4.LotteryId = this.ConvertGameCode(gameCode);
            info3.GameQueryInfo = info4;
            JC_GameQueryRequestInfo messageObject = info3;
            JC_GameQueryResponseInfo info2 = RequestMessage<JC_GameQueryResponseInfo>(messageObject);
            if ((info2.ErrInfo != null) && (info2.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info2.ErrInfo.Message);
            }
            return info2;
        }

        public GameResultQueryResponseInfo QueryMatchResult(string gameCode, string issuseNumber)
        {
            GameResultQueryRequestInfo info3 = new GameResultQueryRequestInfo();
            GameResultQueryRequestInnerInfo info4 = new GameResultQueryRequestInnerInfo();
            info4.LotteryId = this.ConvertGameCode(gameCode);
            info4.IssuseNumber = this.ConvertIssuseNumber(gameCode, issuseNumber);
            info3.GameResultQueryInfo = info4;
            GameResultQueryRequestInfo messageObject = info3;
            GameResultQueryResponseInfo info2 = RequestMessage<GameResultQueryResponseInfo>(messageObject);
            if ((info2.ErrInfo != null) && (info2.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info2.ErrInfo.Message);
            }
            return info2;
        }

        public QueryPrizeResponseInfo QueryPrize(string gameCode, string gameType, string issuseNumber, string prevTicketId)
        {
            QueryPrizeRequestInfo info3 = new QueryPrizeRequestInfo();
            QueryPrizeRequestInnerInfo info4 = new QueryPrizeRequestInnerInfo();
            info4.LotteryId = this.ConvertGameCode(gameCode, gameType);
            info4.IssueNumber = this.ConvertIssuseNumber(gameCode, issuseNumber);
            info4.PrevTicketId = prevTicketId;
            info4.Status = "0";
            info3.QueryPrizeInnerInfo = info4;
            QueryPrizeRequestInfo messageObject = info3;
            QueryPrizeResponseInfo info2 = RequestMessage<QueryPrizeResponseInfo>(messageObject);
            if ((info2.ErrInfo != null) && (info2.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info2.ErrInfo.Message);
            }
            return info2;
        }

        public QueryTicketResponseInfo QueryTicket(QueryTicketRequestInfo request)
        {
            QueryTicketResponseInfo info = RequestMessage<QueryTicketResponseInfo>(request);
            if ((info.ErrInfo != null) && (info.ErrInfo.TransCode != null))
            {
                throw new GatewayException(info.ErrInfo.Message);
            }
            return info;
        }

        public static string RequestMessage(XmlMappingObject messageObject)
        {
            string str = messageObject.ToInnerXmlString("body");
            MessageRequestInfo info2 = new MessageRequestInfo();
            MessageRequestInfo.MessageHead head = new MessageRequestInfo.MessageHead();
            head.Transcode = CommandDefinition.GetCommandByRequestObjectType(messageObject.GetType()).CommandCode;
            head.Partnerid = PartnerId;
            head.Version = Version;
            head.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            info2.Head = head;
            info2.Body = str;
            MessageRequestInfo info = info2;
            string str2 = info.ToXmlString("msg").Replace("&gt;", ">").Replace("&lt;", "<");
            string requestString = string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", new object[] { info.Head.Transcode, str2, MessageHelper.GetMd5Body(info.Head.Transcode + str2 + Key), PartnerId });
            return GetXMl(PostManager.Post(ServiceUrl, requestString, Encoding.UTF8, 0, null, "text/xml"));
        }

        public static T RequestMessage<T>(XmlMappingObject messageObject) where T : XmlMappingObject, new()
        {
            string str = messageObject.ToInnerXmlString("body");
            string commandCode = CommandDefinition.GetCommandByRequestObjectType(messageObject.GetType()).CommandCode;
            MessageRequestInfo info2 = new MessageRequestInfo();
            MessageRequestInfo.MessageHead head = new MessageRequestInfo.MessageHead();
            head.Transcode = commandCode;
            head.Partnerid = PartnerId;
            head.Version = Version;
            head.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            info2.Head = head;
            info2.Body = str;
            MessageRequestInfo info = info2;
            string text = info.ToXmlString("msg").Replace("&gt;", ">").Replace("&lt;", "<");
            if (!string.IsNullOrEmpty(_xmlDir))
            {
                TryAppendText(messageObject, text, 0, 3);
            }
            string requestString = string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", new object[] { info.Head.Transcode, text, MessageHelper.GetMd5Body(commandCode + text + Key), PartnerId });
            string xMl = GetXMl(PostManager.Post(ServiceUrl, requestString, Encoding.UTF8, 0, null, "text/xml"));
            T local = XmlAnalyzeHelper.AnalyseResponse<T>(xMl, "body");
            if (!string.IsNullOrEmpty(_xmlDir))
            {
                TryAppendText(local, xMl, 0, 3);
            }
            return local;
        }

        public TicketResponseInfo RequestTicket(TicketRequestInfo[] requestList)
        {
            TicketResponseInfo info4 = new TicketResponseInfo();
            TicketResponseList list = new TicketResponseList();
            list.InnerOrders = new XmlMappingList<InnerOrderResponseInfo>();
            info4.Tickets = list;
            TicketResponseInfo info = info4;
            foreach (TicketRequestInfo info2 in requestList)
            {
                if (info2 == null) continue;
                if (info2.ticketOrder == null) continue;
                if (string.IsNullOrEmpty(info2.ticketOrder.LotteryId)) continue;
                if (info2.ticketOrder.TicketsNum == 0) continue;
                if (info2.ticketOrder.TotalMoney == 0) continue;
                TicketResponseInfo info3 = RequestMessage<TicketResponseInfo>(info2);
                if (info3 == null) continue;
                if ((info3.ErrInfo != null) && (info3.ErrInfo.TransCode != null))
                {
                    throw new GatewayException(info3.ErrInfo.Message);
                }
                info.Tickets.InnerOrders.AddRange(info3.Tickets.InnerOrders);
            }
            return info;
        }

        public static void SetXmlDir(string root)
        {
            _xmlDir = root;
        }

        private static void TryAppendText(XmlMappingObject messageObject, string text, int time, int max)
        {
            try
            {
                string logFileName = GetLogFileName(messageObject);
                string directoryName = Path.GetDirectoryName(logFileName);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                using (StreamWriter writer = File.AppendText(logFileName))
                {
                    writer.WriteLine(text);
                    writer.Flush();
                }
            }
            catch
            {
                if (time++ < max)
                {
                    Thread.Sleep(200);
                    TryAppendText(messageObject, text, time, max);
                }
            }
        }

        public WinNumberResponseInfo QueryWinNumber(string gameCode, string issueNumber)
        {
            var queryInfo = new WinNumberQueryInfo
            {
                GameCode = this.ConvertGameCode(gameCode, ""),
                IssueNumber = issueNumber,
            };
            var info = new WinNumberRequestInfo
            {
                QueryResult = queryInfo,
            };
            WinNumberResponseInfo info2 = RequestMessage<WinNumberResponseInfo>(info);
            return info2;
        }
    }
}
