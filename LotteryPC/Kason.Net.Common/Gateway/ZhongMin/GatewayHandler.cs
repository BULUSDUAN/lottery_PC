using EntityModel.Xml;
using Kason.Net.Common.Net;
using Kason.Net.Common.Xml;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace Kason.Net.Common.Gateway
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
                    return "JC" + gameType;

                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sfc":
                            return "JCFC";

                        case "hh":
                            return "JCLQFH";
                    }
                    return "JC" + gameType;

                case "bjdc":
                    string str4;
                    if ((str4 = gameType.ToLower()) == null || !(str4 == "zjq"))
                        return gameType;
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
            var str = gameCode.ToLower();
            if (str != null)
            {
                string str2;
                if (!(str == "jczq") && !(str == "bjdc"))
                {
                    if (str == "ctzq")
                        return betType;
                    if (str != "dlt")
                        return gameType;
                }
                else
                {
                    return "FS";
                }
                if ((str2 = gameType.ToLower()) != null && str2 == "dt")
                    return "FS";
            }
            return gameType;
        }

        private string ConvertIssuseNumber(string gameCode, string issuseNumber)
        {
            var str = gameCode.ToLower();
            if (str != null)
            {
                if (!(str == "ssq"))
                {
                    if (str == "dlt" && issuseNumber.Contains("-"))
                        return issuseNumber.Replace("-", "").Substring(2);
                    return issuseNumber;
                }
                if (issuseNumber.Contains("-"))
                    return issuseNumber.Replace("-", "");
            }
            return issuseNumber;
        }

        private static string GetLogFileName(XmlMappingObject messageObject)
        {
            var str = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (str == _date)
            {
                _index++;
            }
            else
            {
                _date = str;
                _index = 1;
            }
            var str2 = messageObject.GetPrimaryKey().Replace("<", ".").Replace(">", ".").Replace(":", ".")
                .Replace(@"\", ".").Replace("/", ".").Replace("*", ".").Replace("?", ".").Replace("|", ".")
                .Replace("\"", ".");
            return Path.Combine(_xmlDir, "ZhongMin", CommandDefinition.GetCommandCategory(messageObject.GetCode()),
                DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Now.Hour.ToString("D2"),
                string.Format("{0}-{1}-{2}-{3:D3}.xml", messageObject.GetCode(), str2, _date, _index));
        }

        public static string GetRequestMessageXml(XmlMappingObject messageObject)
        {
            var str = messageObject.ToInnerXmlString("body");
            var info2 = new MessageRequestInfo();
            var head = new MessageRequestInfo.MessageHead();
            head.Transcode = CommandDefinition.GetCommandByRequestObjectType(messageObject.GetType()).CommandCode;
            head.Partnerid = PartnerId;
            head.Version = Version;
            head.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            info2.Head = head;
            info2.Body = str;
            var info = info2;
            var str2 = info.ToXmlString("msg").Replace("&gt;", ">").Replace("&lt;", "<");
            return string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", info.Head.Transcode, str2,
                MessageHelper.GetMd5Body(info.Head.Transcode + str2 + Key), PartnerId);
        }

        private static string GetXMl(string xmlstr)
        {
            var match = new Regex("&msg=(?<value0>.*?)&key=").Match(xmlstr);
            if (match.Groups[0].Length > 0)
                return match.Groups[1].Value;
            return "";
        }

        public decimal QueryBalance()
        {
            var info3 = new BalanceRequestInfo();
            var account = new PartnerAccount();
            account.PartnerId = PartnerId;
            info3.PartnerAccount = account;
            var messageObject = info3;
            var info2 = RequestMessage<BalanceResponseInfo>(messageObject);
            if (info2.ErrInfo != null && info2.ErrInfo.TransCode != null)
                throw new GatewayException(info2.ErrInfo.Message);
            return info2.PartnerAccount.Balance;
        }

        public IssuseQueryResponseInfo QueryCurrentIssuse(string gameCode, string gameType)
        {
            var info3 = new IssuseQueryRequestInfo();
            var info4 = new IssuseQueryRequestInnerInfo();
            info4.LotteryId = ConvertGameCode(gameCode, gameType);
            info3.IssuseQueryInfo = info4;
            var messageObject = info3;
            var info2 = RequestMessage<IssuseQueryResponseInfo>(messageObject);
            if (info2.ErrInfo != null && info2.ErrInfo.TransCode != null)
                throw new GatewayException(info2.ErrInfo.Message);
            return info2;
        }

        public BJDC_GameQueryResponseInfo QueryMatchList_BJDC(string gameCode, string gameType, string issuseNumber)
        {
            var info3 = new BJDC_GameQueryRequestInfo();
            var info4 = new BJDC_GameQueryRequestInnerInfo();
            info4.LotteryId = ConvertGameCode(gameCode, gameType);
            info4.IssuseNumber = ConvertIssuseNumber(gameCode, issuseNumber);
            info3.GameQueryInfo = info4;
            var messageObject = info3;
            var info2 = RequestMessage<BJDC_GameQueryResponseInfo>(messageObject);
            if (info2.ErrInfo != null && info2.ErrInfo.TransCode != null)
                throw new GatewayException(info2.ErrInfo.Message);
            return info2;
        }

        public JC_GameQueryResponseInfo QueryMatchList_JC(string gameCode, string gameType, string issuseNumber)
        {
            var info3 = new JC_GameQueryRequestInfo();
            var info4 = new JC_GameQueryRequestInnerInfo();
            info4.LotteryId = ConvertGameCode(gameCode);
            info3.GameQueryInfo = info4;
            var messageObject = info3;
            var info2 = RequestMessage<JC_GameQueryResponseInfo>(messageObject);
            if (info2.ErrInfo != null && info2.ErrInfo.TransCode != null)
                throw new GatewayException(info2.ErrInfo.Message);
            return info2;
        }

        public GameResultQueryResponseInfo QueryMatchResult(string gameCode, string issuseNumber)
        {
            var info3 = new GameResultQueryRequestInfo();
            var info4 = new GameResultQueryRequestInnerInfo();
            info4.LotteryId = ConvertGameCode(gameCode);
            info4.IssuseNumber = ConvertIssuseNumber(gameCode, issuseNumber);
            info3.GameResultQueryInfo = info4;
            var messageObject = info3;
            var info2 = RequestMessage<GameResultQueryResponseInfo>(messageObject);
            if (info2.ErrInfo != null && info2.ErrInfo.TransCode != null)
                throw new GatewayException(info2.ErrInfo.Message);
            return info2;
        }

        public QueryPrizeResponseInfo QueryPrize(string gameCode, string gameType, string issuseNumber,
            string prevTicketId)
        {
            var info3 = new QueryPrizeRequestInfo();
            var info4 = new QueryPrizeRequestInnerInfo();
            info4.LotteryId = ConvertGameCode(gameCode, gameType);
            info4.IssueNumber = ConvertIssuseNumber(gameCode, issuseNumber);
            info4.PrevTicketId = prevTicketId;
            info4.Status = "0";
            info3.QueryPrizeInnerInfo = info4;
            var messageObject = info3;
            var info2 = RequestMessage<QueryPrizeResponseInfo>(messageObject);
            if (info2.ErrInfo != null && info2.ErrInfo.TransCode != null)
                throw new GatewayException(info2.ErrInfo.Message);
            return info2;
        }

        public QueryTicketResponseInfo QueryTicket(QueryTicketRequestInfo request)
        {
            var info = RequestMessage<QueryTicketResponseInfo>(request);
            if (info.ErrInfo != null && info.ErrInfo.TransCode != null)
                throw new GatewayException(info.ErrInfo.Message);
            return info;
        }

        public static string RequestMessage(XmlMappingObject messageObject)
        {
            var str = messageObject.ToInnerXmlString("body");
            var info2 = new MessageRequestInfo();
            var head = new MessageRequestInfo.MessageHead();
            head.Transcode = CommandDefinition.GetCommandByRequestObjectType(messageObject.GetType()).CommandCode;
            head.Partnerid = PartnerId;
            head.Version = Version;
            head.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            info2.Head = head;
            info2.Body = str;
            var info = info2;
            var str2 = info.ToXmlString("msg").Replace("&gt;", ">").Replace("&lt;", "<");
            var requestString = string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", info.Head.Transcode, str2,
                MessageHelper.GetMd5Body(info.Head.Transcode + str2 + Key), PartnerId);
            return GetXMl(PostManager.Post(ServiceUrl, requestString, Encoding.UTF8, 0, null, "text/xml"));
        }

        public static T RequestMessage<T>(XmlMappingObject messageObject) where T : XmlMappingObject, new()
        {
            var str = messageObject.ToInnerXmlString("body");
            var commandCode = CommandDefinition.GetCommandByRequestObjectType(messageObject.GetType()).CommandCode;
            var info2 = new MessageRequestInfo();
            var head = new MessageRequestInfo.MessageHead();
            head.Transcode = commandCode;
            head.Partnerid = PartnerId;
            head.Version = Version;
            head.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            info2.Head = head;
            info2.Body = str;
            var info = info2;
            var text = info.ToXmlString("msg").Replace("&gt;", ">").Replace("&lt;", "<");
            if (!string.IsNullOrEmpty(_xmlDir))
                TryAppendText(messageObject, text, 0, 3);
            var requestString = string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", info.Head.Transcode, text,
                MessageHelper.GetMd5Body(commandCode + text + Key), PartnerId);
            var xMl = GetXMl(PostManager.Post(ServiceUrl, requestString, Encoding.UTF8, 0, null, "text/xml"));
            var local = XmlAnalyzeHelper.AnalyseResponse<T>(xMl, "body");
            if (!string.IsNullOrEmpty(_xmlDir))
                TryAppendText(local, xMl, 0, 3);
            return local;
        }

        public TicketResponseInfo RequestTicket(TicketRequestInfo[] requestList)
        {
            var info4 = new TicketResponseInfo();
            var list = new TicketResponseList();
            list.InnerOrders = new XmlMappingList<InnerOrderResponseInfo>();
            info4.Tickets = list;
            var info = info4;
            foreach (var info2 in requestList)
            {
                if (info2 == null) continue;
                if (info2.ticketOrder == null) continue;
                if (string.IsNullOrEmpty(info2.ticketOrder.LotteryId)) continue;
                if (info2.ticketOrder.TicketsNum == 0) continue;
                if (info2.ticketOrder.TotalMoney == 0) continue;
                var info3 = RequestMessage<TicketResponseInfo>(info2);
                if (info3 == null) continue;
                if (info3.ErrInfo != null && info3.ErrInfo.TransCode != null)
                    throw new GatewayException(info3.ErrInfo.Message);
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
                var logFileName = GetLogFileName(messageObject);
                var directoryName = Path.GetDirectoryName(logFileName);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                using (var writer = File.AppendText(logFileName))
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
                GameCode = ConvertGameCode(gameCode, ""),
                IssueNumber = issueNumber
            };
            var info = new WinNumberRequestInfo
            {
                QueryResult = queryInfo
            };
            var info2 = RequestMessage<WinNumberResponseInfo>(info);
            return info2;
        }
    }
}