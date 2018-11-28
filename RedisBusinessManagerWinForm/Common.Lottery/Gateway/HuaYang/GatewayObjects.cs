using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.XmlAnalyzer;
using Common.Lottery.Objects;

namespace Common.Lottery.Gateway.HuaYang
{
    #region 订单扩展函数

    public static class ObjectExtension
    {
        //数字彩
        public static string ToAnteString_HuaYang(this Ticket ticket, string gameCode)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.ToAnteString_HuaYang(gameCode));
            }
            switch (gameCode.ToLower())
            {
                case "bjdc":
                case "jczq":
                case "jclq":
                    return string.Join("/", strList);
                default:
                    return string.Join("^", strList);
            }
        }
        public static string ToAnteString_HuaYang(this Antecode ante, string gameCode)
        {
            if (gameCode.Equals("SSQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                var tmp = ante.AnteNumber.Split('|');
                if (tmp.Length != 3)
                {
                    throw new ArgumentException("双色球胆拖玩法号码错误 - " + ante.AnteNumber);
                }
                var dan = tmp[0];
                var tuo = tmp[1];
                var lan = tmp[2];
                return string.Format("{0}*{1}|{2}", dan, tuo, lan).Replace(",", "");
            }
            else if (gameCode.Equals("SSQ", StringComparison.OrdinalIgnoreCase) && (ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase) || ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase)))
            {
                return ante.AnteNumber.Replace(",", "*");
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && (ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase) || ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase)))
            {
                return ante.AnteNumber.Replace(",", "*");
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                //05,06,17,18|01,02,13,14|06|01,05
                var tmp = ante.AnteNumber.Split('|');
                if (tmp.Length != 4)
                {
                    throw new ArgumentException("大乐透胆拖玩法号码错误 - " + ante.AnteNumber);
                }
                var hongdan = tmp[0];
                var hongtuo = tmp[1];
                var landan = tmp[2];
                var lantuo = tmp[3];
                return string.Format("{0}*{1}|{2}*{3}", hongdan, hongtuo, landan, lantuo).Replace(",","");
            }
            else if (gameCode.Equals("CTZQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("TR9", StringComparison.OrdinalIgnoreCase))
            {
                return ante.AnteNumber.Replace("*", "#").Replace(",","*");
            }
            #region CQSSC
            else if (gameCode.Equals("CQSSC", StringComparison.OrdinalIgnoreCase))
            {
                char[] array;
                var touzhu = string.Empty;
                switch (ante.GameType.ToLower())
                {
                    case "1xdx":
                        ante.AnteNumber = ante.AnteNumber.Replace("-,-,-,-,", "");
                        array = ante.AnteNumber.ToArray();
                        touzhu = string.Join(";", array);
                        return touzhu;
                    case "2xdx":
                        ante.AnteNumber = ante.AnteNumber.Replace("-,-,-,", "");
                        if (ante.AnteNumber.Length == 3)
                        {
                            return ante.AnteNumber.Replace(",", "");
                        }
                        else if (ante.AnteNumber.Length > 3)
                        {
                        }
                        return ante.AnteNumber;

                    case "3xdx":
                        ante.AnteNumber = ante.AnteNumber.Replace("-,-,", "");
                        if (ante.AnteNumber.Length == 5)
                        {
                            return ante.AnteNumber.Replace(",", "");
                        }
                        return ante.AnteNumber;
                    case "5xdx":
                        if (ante.AnteNumber.Length == 9)
                        {
                            return ante.AnteNumber.Replace(",", "");
                        }
                        else if (ante.AnteNumber.Length > 9)
                        {
                            return ante.AnteNumber;
                        }
                        return ante.AnteNumber;
                    case "2xzxfs":
                    case "2xbaodan":
                    case "3xzxzh":
                    case "zx3ds":
                    case "zx3fs":
                    case "3xzxhz":
                    case "3xbaodan":
                    case "2xzxfw":
                    case "5xtx":
                    case "zx6":
                        return ante.AnteNumber.Replace(",", "");
                    case "2xhz":
                    case "3xhz":
                        return ante.AnteNumber;
                    case "dxds":
                        return ante.AnteNumber.Replace("5", "3").Replace(",", "");
                    default:
                        break;
                }

                return ante.AnteNumber;
            }
            #endregion
            #region JX11X5
            else if (gameCode.Equals("JX11X5", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "rx2":
                    case "RX3":
                    case "RX4":
                    case "RX5":
                    case "RX6":
                    case "RX7":
                    case "RX8":
                    case "q2zux":
                    case "q3zux":
                        return ante.AnteNumber;
                    case "q2zhix":
                        if (ante.AnteNumber.Length == "04|10".Length)
                        {
                            return ante.AnteNumber.Replace(",", "|");
                        }
                        else
                        {
                            ante.AnteNumber = ante.AnteNumber.Replace(",", "|");
                            ante.AnteNumber = ante.AnteNumber.Replace(" ", ",");
                            return ante.AnteNumber;
                        }
                    case "q3zhix":
                        if (ante.AnteNumber.Length == "05|08|11".Length)
                        {
                            return ante.AnteNumber.Replace(",", "|");
                        }
                        else
                        {
                            ante.AnteNumber = ante.AnteNumber.Replace(",", "|");
                            ante.AnteNumber = ante.AnteNumber.Replace(" ", ",");
                            return ante.AnteNumber;
                        }
                    default:
                        break;
                }

                return ante.AnteNumber;
            }
            #endregion
            else if (gameCode.Equals("FC3D", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "ds":
                    case "fs":
                    case "hz":
                    case "zx3fs":
                    case "zx3ds":
                    case "zx6":
                        return ante.AnteNumber.Replace(",", "*");
                    default:
                        break;
                }

                return ante.AnteNumber;
            }
            else if (gameCode.Equals("PL3", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "ds":
                    case "fs":
                    case "hz":
                    case "zx3fs":
                    case "zx3ds":
                    case "zx6":
                        return ante.AnteNumber.Replace(",","*");
                    default:
                        break;
                }
                return ante.AnteNumber;
            }
            else
            {
                return ante.AnteNumber;
            }
            //todo:
        }
        //竞彩、北单
        public static string ToAnteString_HuaYang(this ITicket ticket)
        {
            var strList = new List<string>();
            var withGameType = false;
            if (!string.IsNullOrEmpty(ticket.GameType) && ticket.GameType.Equals("HH", StringComparison.OrdinalIgnoreCase))
            {
                if (ticket.GetAnteCodeList().GroupBy(a => a.GameType).Count() > 1)
                {
                    withGameType = true;
                }
            }
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(ante.ToAnteString_HuaYang(ante.GameType ?? ticket.GameType, withGameType));
            }
            return string.Join(";", strList);
        }
        public static string ToAnteString_HuaYang(this IAntecode ante, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                // SF@1-001:[主胜,客胜]
                return string.Format("{0}^{1}({2})", ConvertGameCode(ante.GameCode, gameType), ConverMatchId(ante.GameCode, ante.MatchId), ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber));
            }
            else
            {
                // 71:[胜,负]
                return string.Format("{0}({1})", ConverMatchId(ante.GameCode, ante.MatchId), ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber));
            }
        }

        public static int ConvertGameCode(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                    switch (gameType.ToUpper())
                    {
                        case "ZJQ":
                            return 212;
                        case "HH":
                            return 208;
                        case "SPF":
                            return 210;
                        case "BRQSPF":
                            return 209;
                        case "BF":
                            return 211;
                        case "BQC":
                            return 213;
                        default:
                            return 0;
                    }
                case "jclq":
                    switch (gameType.ToUpper())
                    {
                        case "RFSF":
                            return 214;
                        case "SF":
                            return 216;
                        case "SFC":
                            return 217;
                        case "DXF":
                            return 215;
                        case "HH":
                            return 218;
                        default:
                            return 0;
                    }
                case "bjdc":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                            return 200;
                        case "zjq":
                            return 202;
                        case "bf":
                            return 203;
                        case "sxds":
                            return 201;
                        case "bqc":
                            return 204;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
        public static string ConverMatchId(string gameCode, string matchId)
        {
            //150108  001
            switch (gameCode.ToLower())
            {
                case "jczq":
                case "jclq":
                    if (matchId.Length == 5)
                    {
                        var day = matchId.Substring(0, 2);
                        var index = matchId.Substring(2);
                        var week = "0";
                        if (day == "周一") { week = "1"; }
                        else if (day == "周二") { week = "2"; }
                        else if (day == "周三") { week = "3"; }
                        else if (day == "周四") { week = "4"; }
                        else if (day == "周五") { week = "5"; }
                        else if (day == "周六") { week = "6"; }
                        else if (day == "周日") { week = "7"; }
                        return string.Format("{0}-{1}", week, index);
                    }
                    else if (matchId.Length == 9)
                    {
                        var year = "20" + matchId.Substring(0, 2);
                        var month = matchId.Substring(2, 2);
                        var day = matchId.Substring(4, 2);
                        var index = matchId.Substring(6);
                        var date = DateTime.Parse(string.Format("{0}-{1}-{2}", year, month, day));
                        var week = (int)date.DayOfWeek;
                        if (week == 0) week = 7;
                        return string.Format("{0}-{1}", week, index);
                    }
                    else
                    {
                        return matchId;
                    }
                default:
                    return matchId;
            }
        }
        public static string ConvertAnteCode(string gameCode, string gameType, string ante)
        {
            switch (gameCode.ToLower())
            {
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sf":
                        case "rfsf":
                            return ante;
                        case "dxf":
                            return ante.Replace("3", "1").Replace("0", "2");
                        case "sfc":
                            return ante
                                .Replace("01", "1").Replace("02", "2").Replace("03", "3").Replace("04", "4").Replace("05", "5").Replace("06", "6")
                                .Replace("11", "7").Replace("12", "8").Replace("13", "9").Replace("14", "10").Replace("15", "11").Replace("16", "12");
                    }
                    return ante;
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                        case "brqspf":
                            return ante;
                        case "zjq":
                            return ante.Replace("7", "7+");
                        case "bf":
                            return ante.Replace("X0", "90").Replace("XX", "99").Replace("0X", "09");
                        case "bqc":
                            return ante;
                    }
                    return ante;
                case "bjdc":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                            return ante.Replace("3", "胜").Replace("1", "平").Replace("0", "负");
                        case "zjq":
                            return ante.Replace("0", "1").Replace("1", "2").Replace("2", "3").Replace("3", "4").Replace("4", "5").Replace("5", "6").Replace("6", "7").Replace("7", "8");
                        case "sxds":
                            return ante.Replace("SD", "1").Replace("SS", "2").Replace("XD", "3").Replace("XS", "4");
                        case "bqc":
                            return ante.Replace("33", "1").Replace("31", "2").Replace("30", "3")
                                .Replace("13", "4").Replace("11", "5").Replace("10", "6")
                                .Replace("03", "7").Replace("01", "8").Replace("00", "9");
                        case "sf":
                            return ante.Replace("3", "胜").Replace("0", "负");
                        case "bf": return ante.Replace("10", "2").Replace("20", "3").Replace("21", "4").Replace("30", "5").Replace("31", "6").Replace("32", "7").Replace("40", "8").Replace("41", "9").Replace("42", "10").Replace("X0", "1")
                            .Replace("00", "12").Replace("11", "13").Replace("22", "14").Replace("33", "15").Replace("XX", "11")
                            .Replace("01", "17").Replace("02", "18").Replace("12", "19").Replace("03", "20").Replace("13", "21").Replace("23", "22").Replace("04", "23").Replace("14", "24").Replace("24", "25").Replace("0X", "16");
                    }
                    return ante;
            }
            return ante;
        }

        public static string ConvertPlayType(string gameCode, string playType) 
        {
            switch (gameCode.ToUpper())
            {
                case "BJDC":
                    return playType.Replace("P", "").Replace("_", "*").Replace("11*1", "#").Replace("1*1", "单关").Replace("#", "11*1");
                case "JCZQ":
                    return playType.Replace("P", "").Replace("_", "").Replace("11", "101").Replace("21", "102").Replace("31", "103").Replace("41", "104").Replace("51", "105").Replace("61", "106").Replace("71", "107").Replace("81", "108");
                case "JCLQ":
                    return playType.Replace("P", "").Replace("_", "").Replace("11", "02").Replace("21", "03").Replace("31", "04").Replace("41", "05").Replace("51", "06").Replace("61", "07").Replace("71", "08").Replace("81", "09");
                default:
                    break;
            }
            return playType;
        }
    }

    #endregion





    #region 查询当前奖期

    public class IssuseQueryRequestInfo : XmlMappingObject
    {
        /// <summary>
        /// 订单信息集合
        /// </summary>
        [XmlMapping("elements", 0, MappingType = MappingType.Element)]
        public Element_QueryNowIssueRequestList Elements { get; set; }
    }
    public class Element_QueryNowIssueRequestList : XmlMappingObject
    {
        [XmlMapping("element", 0, MappingType = MappingType.Element)]
        public IssuseQueryRequestInnerInfo InnerOrders { get; set; }
    }
    public class IssuseQueryRequestInnerInfo : XmlMappingObject
    {
        /// <summary>
        /// 玩法编号
        /// </summary>
        [XmlMapping("lotteryid", 0)]
        public string Lotteryid { get; set; }
    }

    public class IssuseQueryResponseInfo : XmlMappingObject
    {
        /// <summary>
        /// 订单信息集合
        /// </summary>
        [XmlMapping("elements", 0, MappingType = MappingType.Element)]
        public Element_IssuseQueryResponseList Elements { get; set; }
        [XmlMapping("oelement", 0, MappingType = MappingType.Element)]
        public ErrorInfo Oelement { get; set; }
    }
    public class Element_IssuseQueryResponseList : XmlMappingObject
    {
        [XmlMapping("element", 0, MappingType = MappingType.Element)]
        public IssuseQueryResponseItem InnerOrders { get; set; }
    }
    public class IssuseQueryResponseItem : XmlMappingObject
    {
        /// <summary>
        /// 玩法编号
        /// </summary>
        [XmlMapping("lotteryid", 0)]
        public string Lotteryid { get; set; }
        /// <summary>
        /// 要查询的期号
        /// </summary>
        [XmlMapping("issue", 1)]
        public string Issue { get; set; }
        /// <summary>
        /// 开始销售时间，时间格式：yyyymmdd hh24miss
        /// </summary>
        [XmlMapping("startime", 2)]
        public string Startime { get; set; }
        /// <summary>
        /// 停止销售时间, 时间格式：yyyymmdd hh24miss
        /// </summary>
        [XmlMapping("endtime", 3)]
        public string Endtime { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        [XmlMapping("satus", 4)]
        public string Satus { get; set; }
    }

    #endregion

    #region 出票请求

    public class TicketRequestInfo_HY : XmlMappingObject
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        [XmlMapping("elements", 0)]
        public ElementList Elements { get; set; }

        public override string GetCode()
        {
            return "13005";
        }
    }
    public class ElementList : XmlMappingObject
    {
        [XmlMapping("element", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<InnerOrderInfo_HY> InnerOrders { get; set; }
    }
    public class InnerOrderInfo_HY : XmlMappingObject
    {
        /// <summary>
        /// 彩票持有人真实姓名
        /// </summary>
        [XmlMapping("ticketuser", 0)]
        public string Ticketuser { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [XmlMapping("identify", 1)]
        public string Identify { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [XmlMapping("phone", 2)]
        public string Phone { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [XmlMapping("email", 3)]
        public string Email { get; set; }
        /// <summary>
        /// 代理发起投注时产生的流水号，不要超过24位，唯一标识
        /// </summary>
        [XmlMapping("id", 4)]
        public string Id { get; set; }
        /// <summary>
        /// 玩法代码
        /// </summary>
        [XmlMapping("lotteryid", 5)]
        public string Lotteryid { get; set; }
        /// <summary>
        /// 指定查询的期号
        /// </summary>
        [XmlMapping("issue", 6)]
        public string Issue { get; set; }
        /// <summary>
        /// 子玩法代码
        /// </summary>
        [XmlMapping("childtype", 7)]
        public string Childtype { get; set; }
        /// <summary>
        /// 销售方式
        /// </summary>
        [XmlMapping("saletype", 8)]
        public string Saletype { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        [XmlMapping("lotterycode", 9)]
        public string Lotterycode { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        [XmlMapping("appnumbers", 10)]
        public string Appnumbers { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        [XmlMapping("lotterynumber", 11)]
        public string Lotterynumber { get; set; }
        /// <summary>
        /// 投注金额
        /// </summary>
        [XmlMapping("lotteryvalue", 12)]
        public string Lotteryvalue { get; set; }
    }

    #endregion

    #region 出票响应

    public class TicketResponseInfo : XmlMappingObject
    {
        /// <summary>
        /// 订单信息集合
        /// </summary>
        [XmlMapping("elements", 0, MappingType = MappingType.Element)]
        public ElementResponseList Elements { get; set; }
        [XmlMapping("oelement", 0, MappingType = MappingType.Element)]
        public ErrorInfo Oelement { get; set; }

        public override string GetCode()
        {
            return "102";
        }
    }
    public class ElementResponseList : XmlMappingObject
    {
        [XmlMapping("element", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<InnerOrderResponseInfo> InnerOrders { get; set; }
    }
    public class InnerOrderResponseInfo : XmlMappingObject
    {
        //<elements><element><id>8000012007051012345678</id><ltappid>8000012007051012345678</ltappid><actvalue>400000</actvalue><errorcode>0</errorcode><errormsg>操作成功</errormsg></element></elements>
        /// <summary>
        /// 代理发起投注时产生的流水号，不要超过24位，唯一标识
        /// </summary>
        [XmlMapping("id", 0)]
        public string Id { get; set; }
        /// <summary>
        /// ltapp流水（无纸化系统订单号）
        /// </summary>
        [XmlMapping("ltappid", 1)]
        public string Ltappid { get; set; }
        /// <summary>
        /// 账户金额
        /// </summary>
        [XmlMapping("actvalue", 2)]
        public string Actvalue { get; set; }
        /// <summary>
        /// 错误代码，0为成功完成操作，其它为错误编码，必返。具体错误代码代表什么错误
        /// </summary>
        [XmlMapping("errorcode", 3)]
        public string Errorcode { get; set; }
        /// <summary>
        /// 具体错误信息，明确告诉代理商错误内容
        /// </summary>
        [XmlMapping("errormsg", 4)]
        public string Errormsg { get; set; }
    }

    #endregion





    public class ErrorInfo : XmlMappingObject
    {
        [XmlMapping("errorcode", 0, MappingType = MappingType.Attribute)]
        public string Errorcode { get; set; }
        [XmlMapping("errormsg", 1, MappingType = MappingType.Attribute)]
        public string Errormsg { get; set; }
    }
}
