using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.XmlAnalyzer;
using Common;
using Common.Mappings;
using Common.Lottery.Objects;
using Common.Communication;

namespace Common.Lottery.Gateway.ZhongMin
{
    #region 订单扩展函数

    public static class ObjectExtension
    {
        public static string ToAnteString_ZhongMin(this Ticket ticket, string gameCode)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.ToAnteString_ZhongMin(gameCode));
            }
            switch (gameCode.ToLower())
            {
                case "bjdc":
                case "jczq":
                case "jclq":
                    return string.Join("/", strList);
                default:
                    return string.Join(";", strList);
            }
        }
        public static string ToAnteString_ZhongMin(this Antecode ante, string gameCode)
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
                return string.Format("({0}),{1}|{2}", dan, tuo, lan);
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && (ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase) || ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase)))
            {
                return ante.AnteNumber.Replace("|", "+");
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                var tmp = ante.AnteNumber.Split('|');
                if (tmp.Length != 4)
                {
                    throw new ArgumentException("大乐透胆拖玩法号码错误 - " + ante.AnteNumber);
                }
                var hongdan = tmp[0];
                var hongtuo = tmp[1];
                var landan = tmp[2];
                var lantuo = tmp[3];
                return string.Format("({0}),{1}+({2}),{3}", hongdan, hongtuo, landan, lantuo);
            }
            else if (gameCode.Equals("CTZQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("TR9", StringComparison.OrdinalIgnoreCase))
            {
                return ante.AnteNumber.Replace("*", "-");
            }
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
                        return ante.AnteNumber;
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
                        return ante.AnteNumber;
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
        
        public static string ToAnteString_ZhongMin(this ITicket ticket)
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
                strList.Add(ante.ToAnteString_ZhongMin(ante.GameType ?? ticket.GameType, withGameType));
            }
            return string.Join("/", strList);
        }
        public static string ToAnteString_ZhongMin(this IAntecode ante, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                // SF@1-001:[主胜,客胜]
                return string.Format("{2}@{0}:[{1}]", ConverMatchId(ante.GameCode, ante.MatchId), ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber), FomartHHGameType(gameType));
            }
            else
            {
                // 71:[胜,负]
                return string.Format("{0}:[{1}]", ConverMatchId(ante.GameCode, ante.MatchId), ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber));
            }
        }

        //public static string ToAnteString_LiangCai(this Ticket ticket, string gameCode)
        //{
        //    var strList = new List<string>();
        //    foreach (var ante in ticket.AnteCodeList)
        //    {
        //        strList.Add(ante.ToAnteString_LiangCai(gameCode,ante.GameType));
        //    }
        //    //switch (gameCode.ToLower())
        //    //{
        //    //    case "bjdc":
        //    //    case "jczq":
        //    //    case "jclq":
        //    //        return string.Join("/", strList);
        //    //    default:
        //    //        return string.Join(";", strList);
        //    //}
        //    return string.Join(";", strList);
        //}
        //public static string ToAnteString_LiangCai(this Antecode ante, string gameCode,string gameType)
        //{
        //    if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && (ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase) || ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase)))
        //    {
        //        return ante.AnteNumber.Replace("|", "-").Replace(",", " ");
        //    }
        //    else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var tmp = ante.AnteNumber.Split('|');
        //        if (tmp.Length != 4)
        //        {
        //            throw new ArgumentException("大乐透胆拖玩法号码错误 - " + ante.AnteNumber);
        //        }
        //        //05,06,07,08|01,02,03,04,09|01|02,03
        //        //(05,06,07,08),01,02,03,04,09+(01),02,03
        //        var hongdan = tmp[0];
        //        var hongtuo = tmp[1];
        //        var landan = tmp[2];
        //        var lantuo = tmp[3];
        //        return string.Format("{0}${1}-{2}${3}", hongdan, hongtuo, landan, lantuo);
        //    }
        //    else if (gameCode.Equals("CTZQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("TR9", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return ante.AnteNumber.Replace("*", "#");
        //    }
        //    else if (gameCode.Equals("JX11X5", StringComparison.OrdinalIgnoreCase))
        //    {
        //        switch (ante.GameType.ToLower())
        //        {
        //            case "rx2":
        //            case "RX3":
        //            case "RX4":
        //            case "RX5":
        //            case "RX6":
        //            case "RX7":
        //            case "RX8":
        //            case "q2zux":
        //            case "q3zux":
        //                return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber.Replace(",", " ");
        //            case "q2zhix":
        //            case "q3zhix":
        //                return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
        //            default:
        //                break;
        //        }
        //        return ante.AnteNumber;
        //    }
        //    else if (gameCode.Equals("PL3", StringComparison.OrdinalIgnoreCase))
        //    {
        //        switch (ante.GameType.ToLower())
        //        {
        //            case "ds":
        //            case "fs":
        //            case "hz":
        //            case "zx3ds":
        //            case "zx6":
        //                return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
        //            case "zx3fs":
        //                return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber.Replace(",", "");
        //            default:
        //                break;
        //        }
        //        return ante.AnteNumber;
        //    }
        //    else
        //    {
        //        return ante.AnteNumber;
        //    }
        //}
        //public static string ConvertGameType(string gameCode, string gameType)
        //{
        //    switch (gameCode.ToLower())
        //    {
        //        case "jx11x5":
        //            switch (gameType.ToLower())
        //            {
        //                case "rx1":
        //                    return "";
        //                case "rx2":
        //                    return "R2";
        //                case "rx3":
        //                    return "R3";
        //                case "rx4":
        //                    return "R4";
        //                case "rx5":
        //                    return "R5";
        //                case "rx6":
        //                    return "R6";
        //                case "rx7":
        //                    return "R7";
        //                case "rx8":
        //                    return "R8";
        //                case "q2zhix":
        //                    return "Q2";
        //                case "q3zhix":
        //                    return "Q3";
        //                case "q2zux":
        //                    return "Z2";
        //                case "q3zux":
        //                    return "Z3";
        //                default:
        //                    return "";
        //            }
        //        case "pl3":
        //            switch (gameType.ToLower())
        //            {
        //                case "fs":
        //                    return "1";
        //                case "hz":
        //                    return "S1";
        //                case "zx3ds":
        //                case "zx6":
        //                    return "6";
        //                case "zx3fs":
        //                    return "F3";
        //                default:
        //                    return "";
        //            }
        //        default:
        //            return "";
        //    }
        //}
        /// <summary>
        /// 混合过关 玩法格式化
        /// </summary>
        public static string FomartHHGameType(string gameType)
        {
            gameType = gameType.ToUpper();
            switch (gameType)
            {
                //篮球
                case "SF":
                    return "SF";
                case "RFSF":
                    return "RFSF";
                case "DXF":
                    return "DXF";
                case "SFC":
                    return "FC";

                //足球
                case "SPF":
                    return "SPF";
                case "BRQSPF":
                    return "BRQSPF";
                case "ZJQ":
                    return "JQS";
                case "BF":
                    return "BF";
                case "BQC":
                    return "BQC";
                default:
                    break;
            }
            return gameType;
        }
        public static string ConverMatchId(string gameCode, string matchId)
        {
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
                            return ante.Replace("3", "主胜").Replace("0", "客胜");
                        case "dxf":
                            return ante.Replace("3", "大").Replace("0", "小");
                        case "sfc":
                            return ante
                                .Replace("01", "胜A").Replace("02", "胜B").Replace("03", "胜C").Replace("04", "胜D").Replace("05", "胜E").Replace("06", "胜F")
                                .Replace("11", "负A").Replace("12", "负B").Replace("13", "负C").Replace("14", "负D").Replace("15", "负E").Replace("16", "负F")
                                .Replace("胜A", "胜1-5").Replace("胜B", "胜6-10").Replace("胜C", "胜11-15").Replace("胜D", "胜16-20").Replace("胜E", "胜21-25").Replace("胜F", "胜26+")
                                .Replace("负A", "负1-5").Replace("负B", "负6-10").Replace("负C", "负11-15").Replace("负D", "负16-20").Replace("负E", "负21-25").Replace("负F", "负26+");
                    }
                    return ante;
                case "jczq":
                case "bjdc":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                        case "brqspf":
                            return ante.Replace("3", "胜").Replace("1", "平").Replace("0", "负");
                        case "zjq":
                            return ante.Replace("7", "7+");
                        case "sxds":
                            return ante.Replace("SD", "上+单").Replace("SS", "上+双").Replace("XD", "下+单").Replace("XS", "下+双");
                        case "bf": return ante.Replace("10", "1:0").Replace("20", "2:0").Replace("21", "2:1").Replace("30", "3:0").Replace("31", "3:1").Replace("32", "3:2").Replace("40", "4:0").Replace("41", "4:1").Replace("42", "4:2").Replace("50", "5:0").Replace("51", "5:1").Replace("52", "5:2").Replace("X0", "胜其他")
                                .Replace("00", "0:0").Replace("11", "1:1").Replace("22", "2:2").Replace("33", "3:3").Replace("XX", "平其他")
                                .Replace("01", "0:1").Replace("02", "0:2").Replace("12", "1:2").Replace("03", "0:3").Replace("13", "1:3").Replace("23", "2:3").Replace("04", "0:4").Replace("14", "1:4").Replace("24", "2:4").Replace("05", "0:5").Replace("15", "1:5").Replace("25", "2:5").Replace("0X", "负其他");
                        case "bqc":
                            return ante.Replace("33", "胜-胜").Replace("31", "胜-平").Replace("30", "胜-负")
                                .Replace("13", "平-胜").Replace("11", "平-平").Replace("10", "平-负")
                                .Replace("03", "负-胜").Replace("01", "负-平").Replace("00", "负-负");
                        case "sf":
                            return ante.Replace("3", "胜").Replace("0", "负");
                    }
                    return ante;
            }
            return ante;
        }

        /// <summary>
        /// 解析一张票的所注内容，每注号以/分隔
        /// </summary>
        public static string ToAnteString_CQCenter(this Ticket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.ToAnteString_CQCenter());
            }
            return string.Join("#", strList);
        }
        /// <summary>
        /// 解析一注投注号码，如为复式且必须拆为单式的情况，以|分隔
        /// </summary>
        /// <param name="ante"></param>
        /// <returns></returns>
        public static string ToAnteString_CQCenter(this Antecode ante)
        {
            if (ante.GameCode.Equals("CQSSC", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToUpper())
                {
                    case "1XDX":
                        return ante.AnteNumber.Replace("-,-,-,-,", "");
                    case "2XDX":
                        return ante.AnteNumber.Replace("-,-,-,", "").Replace(",", ":");
                    case "3XDX":
                        return ante.AnteNumber.Replace("-,-,", "").Replace(",", ":");
                    case "5XDX":
                    case "2XZXDS":
                    case "2XBAODAN":
                    case "ZX3DS":
                    case "ZX3FS":
                    case "ZX6":
                    case "5XTX":
                    case "DXDS":
                        return ante.AnteNumber.Replace(",", ":");

                    default:
                        break;
                }

                return ante.AnteNumber;
            }
            if (ante.GameCode.Equals("FC3D", StringComparison.OrdinalIgnoreCase))
            {
                return ante.AnteNumber.Replace(",", ":");
            }
            if (ante.GameCode.Equals("SSQ", StringComparison.OrdinalIgnoreCase))
            {
                return ante.AnteNumber.Replace("|", ":");
            }

            return ante.AnteNumber;
        }
        /// <summary>
        /// 解析一张票的所注内容，每注号以/分隔
        /// </summary>
        public static string ToAnteString_Localhost(this Ticket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.AnteNumber);
            }
            return string.Join("/", strList);
        }

        public static string ToAnteString_LocalhostShop(this ITicket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                if (ticket.GameType == "HH")
                {
                    strList.Add(string.Format("{0}_{1}_{2}", ante.GameType, ante.MatchId, ante.AnteNumber));
                }
                else
                {
                    strList.Add(string.Format("{0}_{1}", ante.MatchId, ante.AnteNumber));
                }
            }
            return string.Join("/", strList);
        }
        public static int ReturnAmount(this ITicket ticket, string attch)
        {
            var str = string.Empty;
            foreach (var ante in ticket.GetAnteCodeList()) 
            {
                str += string.IsNullOrEmpty(str) ? string.Format("{0}_{1}_{2}", ante.MatchId, ante.GameType, ante.AnteNumber) : string.Format("|{0}_{1}_{2}", ante.MatchId, ante.GameType, ante.AnteNumber);
            }
            var num = 0;
            var arraryList = attch.Split(',');
            foreach (var item in arraryList)
            {
                var arrary = item.ToUpper().Split('^');
                if (arrary.Contains(str))
                {
                    num = int.Parse(arrary[1]);
                }
            }
            if (num == 0)
                return -1;
            return num;
        }
        ////根据playType 1_1判断是单关  区DS下面
        //public static string GetOddsToMatchId(this ITicket ticket,string playType)
        //{
        //    var strList = new List<string>();
        //    foreach (var ante in ticket.GetAnteCodeList())
        //    {
        //        if (playType.Trim()=="1_1")
        //        {

        //        }
        //        else
        //        {
        //            if (ante.GameCode.ToUpper() == "JCZQ")
        //            {
        //                switch (ante.GameType.ToUpper())
        //                {
        //                    case "BF":
        //                        var oddManager = new JCZQ_OddsManager(tran);
        //                        break;
        //                    case "BQC":
        //                        break;
        //                    case "BRQSPF":
        //                        break;
        //                    case "SPF":
        //                        break;
        //                    case "ZJQ":
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //            if (ante.GameCode.ToUpper() == "JCLQ")
        //            {
        //                switch (ante.GameType.ToUpper())
        //                {
        //                    case "DXF":
        //                        break;
        //                    case "RFSF":
        //                        break;
        //                    case "SF":
        //                        break;
        //                    case "SFC":
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //        }

        //        strList.Add(string.Format("{0}_{1}", ante.MatchId, ante.AnteNumber));
        //    }
        //    return string.Join("/", strList);
        //}
        public static string ToAnteString_zhongminToMatchId(this ITicket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(string.Format("{0}", ante.MatchId));
            }
            return string.Join(",", strList);
        }
        public static string ToAnteString_zhongminToMatchIdLoc(this ITicket ticket,string issuseNumber)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(string.Format("{0}{1}", issuseNumber, ante.MatchId));
            }
            return string.Join(",", strList);
        }
    }

    #endregion

    #region 奖期查询

    public class IssuseQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryissue", 0)]
        public IssuseQueryRequestInnerInfo IssuseQueryInfo { get; set; }
    }
    public class IssuseQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
    }
    public class IssuseQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("issueinfo", 0, MappingType = MappingType.Element)]
        public IssuseQueryResponseItem IssuseQueryResponseItem { get; set; }
        [XmlMapping("issueinfos", 0, MappingType = MappingType.Element)]
        public IssuseQueryResponseList IssuseQueryResponseList { get; set; }
        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
        /// <summary>
        /// 获取结果类型。item:单个奖期。list:多个奖期，特指高频。null：未解析到任何数据
        /// </summary>
        public string ResultType
        {
            get
            {
                if (IssuseQueryResponseItem != null && !string.IsNullOrEmpty(IssuseQueryResponseItem.LotteryId))
                {
                    return "item";
                }
                else if (IssuseQueryResponseList != null && IssuseQueryResponseList.InnerIssuseQueryResponseItems != null && IssuseQueryResponseList.InnerIssuseQueryResponseItems.Count > 0)
                {
                    return "list";
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class IssuseQueryResponseList : XmlMappingObject
    {
        [XmlMapping("issueinfo", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<IssuseQueryResponseItem> InnerIssuseQueryResponseItems { get; set; }
    }
    public class IssuseQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
        [XmlMapping("startTime", 2, MappingType = MappingType.Attribute)]
        public DateTime StartTime { get; set; }
        [XmlMapping("stopTime", 3, MappingType = MappingType.Attribute)]
        public DateTime StopTime { get; set; }
        [XmlMapping("closeTime", 4, MappingType = MappingType.Attribute)]
        public DateTime CloseTime { get; set; }
        [XmlMapping("prizeTime", 5, MappingType = MappingType.Attribute)]
        public DateTime PrizeTime { get; set; }
        [XmlMapping("status", 6, MappingType = MappingType.Attribute)]
        public int Status { get; set; }
        [XmlMapping("bonusCode", 7, MappingType = MappingType.Attribute)]
        public string BonusCode { get; set; }
        [XmlMapping("BonusInfo", 8, MappingType = MappingType.Attribute)]
        public string BonusInfo { get; set; }
    }

    #endregion

    #region 出票请求

    public class TicketRequestInfo : XmlMappingObject
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        [XmlMapping("ticketorder", 0)]
        public TicketOrder ticketOrder { get; set; }
        public override string GetCode()
        {
            return "002";
        }
    }
    public class TicketOrder : XmlMappingObject
    {
        /// <summary>
        /// 玩法编号
        /// </summary>
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        /// <summary>
        ///总注数
        /// </summary>
        [XmlMapping("ticketsnum", 1, MappingType = MappingType.Attribute)]
        public int TicketsNum { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [XmlMapping("totalmoney", 2, MappingType = MappingType.Attribute)]
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 订单信息集合
        /// </summary>
        [XmlMapping("tickets", 3, MappingType = MappingType.Element)]
        public TicketList tickets { get; set; }
    }
    public class TicketList : XmlMappingObject
    {
        [XmlMapping("ticket", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<InnerOrderInfo> InnerOrders { get; set; }
    }
    public class InnerOrderInfo : XmlMappingObject
    {
        /// <summary>
        /// 订单号（投注序列号）
        /// </summary>
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string OrderId { get; set; }
        /// <summary>
        /// 投注方式
        /// </summary>
        [XmlMapping("betType", 1, MappingType = MappingType.Attribute)]
        public string BetType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        [XmlMapping("issueNumber", 2, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        [XmlMapping("betUnits", 3, MappingType = MappingType.Attribute)]
        public int BetUnits { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        [XmlMapping("multiple", 4, MappingType = MappingType.Attribute)]
        public int Multiple { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [XmlMapping("betMoney", 5, MappingType = MappingType.Attribute)]
        public decimal BetMoney { get; set; }
        /// <summary>
        /// 是否追加
        /// </summary>
        [XmlMapping("isAppend", 6, MappingType = MappingType.Attribute)]
        public int IsAppend { get; set; }
        /// <summary>
        /// 投注字符串
        /// </summary>
        [XmlMapping("betContent", 7)]
        public string BetContent { get; set; }
    }

    #endregion

    #region 出票响应

    public class TicketResponseInfo : XmlMappingObject
    {
        /// <summary>
        /// 订单信息集合
        /// </summary>
        [XmlMapping("tickets", 0, MappingType = MappingType.Element)]
        public TicketResponseList Tickets { get; set; }
        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }

        public override string GetCode()
        {
            return "102";
        }
    }
    public class TicketResponseList : XmlMappingObject
    {
        [XmlMapping("ticket", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<InnerOrderResponseInfo> InnerOrders { get; set; }
    }
    public class InnerOrderResponseInfo : XmlMappingObject
    {
        //<ticket ticketId="1234567" multiple="1" issueNumber="201203" betType="P3_1" betUnits="9" betMoney="18" statusCode="909" message="投注订单ID重复" palmid="" />
        /// <summary>
        /// 订单号（投注序列号）
        /// </summary>
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        [XmlMapping("multiple", 1, MappingType = MappingType.Attribute)]
        public int Multiple { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        [XmlMapping("issueNumber", 2, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
        /// <summary>
        /// 投注方式
        /// </summary>
        [XmlMapping("betType", 3, MappingType = MappingType.Attribute)]
        public string BetType { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        [XmlMapping("betUnits", 4, MappingType = MappingType.Attribute)]
        public int BetUnits { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [XmlMapping("betMoney", 5, MappingType = MappingType.Attribute)]
        public decimal BetMoney { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [XmlMapping("statusCode", 6, MappingType = MappingType.Attribute)]
        public string StatusCode { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [XmlMapping("message", 7, MappingType = MappingType.Attribute)]
        public string Message { get; set; }
        /// <summary>
        /// 彩票平台序号
        /// </summary>
        [XmlMapping("palmid", 8, MappingType = MappingType.Attribute)]
        public string Palmid { get; set; }
        /// <summary>
        /// 详细消息
        /// </summary>
        [XmlMapping("detailmessage", 9, MappingType = MappingType.Attribute)]
        public string DetailMessage { get; set; }
    }

    #endregion

    #region 交易结果查询

    public class QueryTicketRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryticket", 0, Common.MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryTicketRequestInnerInfo> QueryTicketInnerInfo { get; set; }
        public override string GetCode()
        {
            return "003";
        }
    }
    public class QueryTicketRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }
        [XmlMapping("palmId", 1, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }
    }
    public class AutoTicketResultRequestInfo : XmlMappingObject
    {
        [XmlMapping("ticketresults", 0, MappingType = MappingType.Element)]
        public AutoTicketResultRequestList TicketResultList { get; set; }
        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }
    public class AutoTicketResultRequestList : XmlMappingObject
    {
        [XmlMapping("ticketresult", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryTicketResponseInnerInfo> InnerTicketResultItems { get; set; }
    }
    public class AutoTicketResultResponseInfo : XmlMappingObject
    {
        [XmlMapping("returnticketresults", 0, MappingType = MappingType.Element)]
        public AutoTicketResultResponseList TicketResultList { get; set; }
        public override string GetCode()
        {
            return "107";
        }
    }
    public class AutoTicketResultResponseList : XmlMappingObject
    {
        [XmlMapping("returnticketresult", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<AutoTicketResultResponseItem> InnerTicketResultItems { get; set; }
    }
    public class AutoTicketResultResponseItem : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("palmId", 1, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }
    }
    public class QueryTicketResponseInfo : XmlMappingObject
    {
        [XmlMapping("ticketresult", 0, Common.MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryTicketResponseInnerInfo> QueryTicketResponseInnerInfo { get; set; }
        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
        public override string GetCode()
        {
            return "103";
        }
    }
    public class QueryTicketResponseInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
        [XmlMapping("ticketId", 2, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }
        [XmlMapping("palmId", 3, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }
        [XmlMapping("statusCode", 4, MappingType = MappingType.Attribute)]
        public string StatusCode { get; set; }
        [XmlMapping("message", 5, MappingType = MappingType.Attribute)]
        public string Message { get; set; }
        [XmlMapping("printodd", 6, MappingType = MappingType.Attribute)]
        public string PrintOdd { get; set; }
        [XmlMapping("Unprintodd", 7, MappingType = MappingType.Attribute)]
        public string UnprintOdd { get; set; }
        [XmlMapping("maxBonus", 8, MappingType = MappingType.Attribute)]
        public string MaxBonus { get; set; }
        [XmlMapping("printNo", 9, MappingType = MappingType.Attribute)]
        public string PrintNo { get; set; }
    }

    #endregion

    #region 比赛赛果查询

    public class GameResultQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("querygameresult", 0)]
        public GameResultQueryRequestInnerInfo GameResultQueryInfo { get; set; }
    }
    public class GameResultQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
    }
    public class GameResultQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("results", 0, MappingType = MappingType.Element)]
        public GameResultQueryResponseList GameResultQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }
    public class GameResultQueryResponseList : XmlMappingObject
    {
        [XmlMapping("result", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<GameResultQueryResponseItem> GameResultQueryResponseItems { get; set; }
    }
    public class GameResultQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("matchId", 0, MappingType = MappingType.Attribute)]
        public string MatchId { get; set; }
        [XmlMapping("matchtime", 1, MappingType = MappingType.Attribute)]
        public DateTime MatchTime { get; set; }
        [XmlMapping("value", 2, MappingType = MappingType.Attribute)]
        public string ResultValue { get; set; }
        [XmlMapping("polygoal", 3, MappingType = MappingType.Attribute)]
        public string Goal_RQ { get; set; }
        [XmlMapping("goal", 4, MappingType = MappingType.Attribute)]
        public string Goal_RF { get; set; }
        [XmlMapping("ougoal", 5, MappingType = MappingType.Attribute)]
        public string Goal_ZF { get; set; }
        [XmlMapping("sp", 6, MappingType = MappingType.Attribute)]
        public string SP_BJDC { get; set; }
        [XmlMapping("dsp", 7, MappingType = MappingType.Attribute)]
        public string SP_JC_DG { get; set; }
        [XmlMapping("gsp", 8, MappingType = MappingType.Attribute)]
        public string SP_JC_GG { get; set; }
    }

    #endregion

    #region 奖金查询

    public class QueryPrizeRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryprize", 0, Common.MappingType.Element)]
        public QueryPrizeRequestInnerInfo QueryPrizeInnerInfo { get; set; }
    }
    public class QueryPrizeRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
        [XmlMapping("prevTicketId", 2, MappingType = MappingType.Attribute)]
        public string PrevTicketId { get; set; }
        [XmlMapping("status", 3, MappingType = MappingType.Attribute)]
        public string Status { get; set; }
    }
    public class QueryPrizeResponseInfo : XmlMappingObject
    {
        [XmlMapping("prizeresult", 0, Common.MappingType.Element)]
        public QueryPrizeResultInfo QueryPrizeResultInfo { get; set; }
        [XmlMapping("wontickets", 1, Common.MappingType.Element)]
        public QueryPrizeWinTicketListInfo QueryPrizeWinTicketList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }
    public class QueryPrizeResultInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
        [XmlMapping("winNum", 2, MappingType = MappingType.Attribute)]
        public int TotalWinCount { get; set; }
        [XmlMapping("totalPrize", 3, MappingType = MappingType.Attribute)]
        public decimal TotalPrizeMoney { get; set; }
        // 剩余的票数
        [XmlMapping("num", 4, MappingType = MappingType.Attribute)]
        public int Number { get; set; }
        [XmlMapping("lastTicketId", 5, MappingType = MappingType.Attribute)]
        public string LastTicketId { get; set; }
    }
    public class QueryPrizeWinTicketListInfo : XmlMappingObject
    {
        [XmlMapping("wonticket", 0, MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryPrizeWinTicketInnerInfo> QueryPrizeWinTicketList { get; set; }
    }
    public class QueryPrizeWinTicketInnerInfo : XmlMappingObject
    {
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }
        [XmlMapping("pretaxPrice", 1, MappingType = MappingType.Attribute)]
        public decimal BonusMoneyPrevTax { get; set; }
        [XmlMapping("prize", 2, MappingType = MappingType.Attribute)]
        public decimal BonusMoneyAfterTax { get; set; }
        [XmlMapping("palmId", 3, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }
        [XmlMapping("state", 4, MappingType = MappingType.Attribute)]
        public string State { get; set; }
        [XmlMapping("IsCancelGame", 5, MappingType = MappingType.Attribute)]
        public string IsCancelGame { get; set; }
        [XmlMapping("IsAwards", 6, MappingType = MappingType.Attribute)]
        public string IsAwards { get; set; }

        [XmlMapping("awardGradedetail", 7, Common.MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryPrizeAwardGradeDetailInfo> QueryPrizeAwardGradeDetailList { get; set; }
    }
    public class QueryPrizeAwardGradeDetailInfo : XmlMappingObject
    {
        [XmlMapping("gradid", 0, MappingType = MappingType.Attribute)]
        public string GradId { get; set; }
        [XmlMapping("awardCount", 1, MappingType = MappingType.Attribute)]
        public string AwardCount { get; set; }
        [XmlMapping("AwardMoney", 2, MappingType = MappingType.Attribute)]
        public string AwardMoney { get; set; }
    }

    #endregion

    #region 查询余额

    public class BalanceRequestInfo : XmlMappingObject
    {
        [XmlMapping("partneraccount", 0)]
        public PartnerAccount PartnerAccount { get; set; }
    }
    public class PartnerAccount : XmlMappingObject
    {
        [XmlMapping("partnerid", 0, MappingType = MappingType.Attribute)]
        public string PartnerId { get; set; }
        [XmlMapping("balance", 0, MappingType = MappingType.Attribute)]
        public decimal Balance { get; set; }
    }
    public class BalanceResponseInfo : XmlMappingObject
    {
        [XmlMapping("partneraccount", 0)]
        public PartnerAccount PartnerAccount { get; set; }
        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    #endregion

    #region 竞彩比赛列表查询

    public class JC_GameQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("querySchedule", 0)]
        public JC_GameQueryRequestInnerInfo GameQueryInfo { get; set; }
    }
    public class JC_GameQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("type", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
    }
    public class JC_GameQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("jcgames", 0, MappingType = MappingType.Element)]
        public JC_GameQueryResponseList GameQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }
    public class JC_GameQueryResponseList : XmlMappingObject
    {
        [XmlMapping("jcgame", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<JC_GameQueryResponseItem> InnerGameQueryResponseItems { get; set; }
    }
    public class JC_GameQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("name", 0, MappingType = MappingType.Attribute)]
        public string GameName { get; set; }
        [XmlMapping("matchID", 1, MappingType = MappingType.Attribute)]
        public string MatchID { get; set; }
        [XmlMapping("hometeam", 2, MappingType = MappingType.Attribute)]
        public string HomeTeam { get; set; }
        [XmlMapping("guestteam", 3, MappingType = MappingType.Attribute)]
        public string GuestTeam { get; set; }
        [XmlMapping("matchstate", 4, MappingType = MappingType.Attribute)]
        public string MatchState { get; set; }
        [XmlMapping("matchtime", 5, MappingType = MappingType.Attribute)]
        public DateTime MatchTime { get; set; }
        [XmlMapping("sellouttime", 6, MappingType = MappingType.Attribute)]
        public DateTime SelloutTime { get; set; }
        /// <summary>
        /// 让球
        /// </summary>
        [XmlMapping("polygonal", 7, MappingType = MappingType.Attribute)]
        public string Polygonal { get; set; }
        /// <summary>
        /// 让分盘口
        /// </summary>
        [XmlMapping("goal", 8, MappingType = MappingType.Attribute)]
        public string Goal { get; set; }
        /// <summary>
        /// 总分盘口
        /// </summary>
        [XmlMapping("ougoal", 9, MappingType = MappingType.Attribute)]
        public string Ougoal { get; set; }
    }

    #endregion

    #region 北单比赛列表查询

    public class BJDC_GameQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("querygames", 0)]
        public BJDC_GameQueryRequestInnerInfo GameQueryInfo { get; set; }
    }
    public class BJDC_GameQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
    }
    public class BJDC_GameQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("games", 0, MappingType = MappingType.Element)]
        public BJDC_GameQueryResponseList GameQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }
    public class BJDC_GameQueryResponseList : XmlMappingObject
    {
        [XmlMapping("game", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<BJDC_GameQueryResponseItem> InnerGameQueryResponseItems { get; set; }
    }
    public class BJDC_GameQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("name", 0, MappingType = MappingType.Attribute)]
        public string GameName { get; set; }
        [XmlMapping("matchID", 1, MappingType = MappingType.Attribute)]
        public string MatchID { get; set; }
        [XmlMapping("hometeam", 2, MappingType = MappingType.Attribute)]
        public string HomeTeam { get; set; }
        [XmlMapping("guestteam", 3, MappingType = MappingType.Attribute)]
        public string GuestTeam { get; set; }
        [XmlMapping("matchtime", 4, MappingType = MappingType.Attribute)]
        public string MatchTimeSrc { get; set; }
        public DateTime MatchTime
        {
            get
            {
                if (string.IsNullOrEmpty(MatchTimeSrc))
                {
                    return default(DateTime);
                }
                var year = MatchTimeSrc.Substring(0, 4);
                var month = MatchTimeSrc.Substring(4, 2);
                var day = MatchTimeSrc.Substring(6, 2);
                var hour = MatchTimeSrc.Substring(8, 2);
                var minute = MatchTimeSrc.Substring(10, 2);
                return DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", year, month, day, hour, minute));
            }
        }
        [XmlMapping("sellouttime", 5, MappingType = MappingType.Attribute)]
        public string SelloutTimeSrc { get; set; }
        public DateTime SelloutTime
        {
            get
            {
                if (string.IsNullOrEmpty(SelloutTimeSrc))
                {
                    return default(DateTime);
                }
                var year = SelloutTimeSrc.Substring(0, 4);
                var month = SelloutTimeSrc.Substring(4, 2);
                var day = SelloutTimeSrc.Substring(6, 2);
                var hour = SelloutTimeSrc.Substring(8, 2);
                var minute = SelloutTimeSrc.Substring(10, 2);
                return DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", year, month, day, hour, minute));
            }
        }
        [XmlMapping("matchstate", 6, MappingType = MappingType.Attribute)]
        public string MatchState { get; set; }
        [XmlMapping("remark", 7, MappingType = MappingType.Attribute)]
        public string Remark { get; set; }
    }

    #endregion

    public class ErrorInfo : XmlMappingObject
    {
        [XmlMapping("transcode", 0, MappingType = MappingType.Attribute)]
        public string TransCode { get; set; }
        [XmlMapping("message", 1, MappingType = MappingType.Attribute)]
        public string Message { get; set; }
    }

    /// <summary>
    /// 查询中奖请求对象
    /// </summary>
    public class WinNumberRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryresult", 0, MappingType = MappingType.Element)]
        public WinNumberQueryInfo QueryResult { get; set; }
    }

    public class WinNumberQueryInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string GameCode { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
    }

    /// <summary>
    /// 查询中奖返回对象
    /// </summary>
    public class WinNumberResponseInfo : XmlMappingObject
    {
        [XmlMapping("results", 0, Common.MappingType.Element)]
        public WinNumberResultsInfo Results { get; set; }
    }

    public class WinNumberResultsInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string GameCode { get; set; }
        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
        [XmlMapping("result", 2, MappingType = MappingType.Attribute)]
        public WinNumberResultInfo Result { get; set; }
    }

    public class WinNumberResultInfo : XmlMappingObject
    {
        [XmlMapping("value", 0, MappingType = MappingType.Attribute)]
        public string Winumber { get; set; }
    }

    public class LiangCaiWinNumberResultInfo : XmlMappingObject
    {
        [XmlMapping("xMsgID", 0, MappingType = MappingType.Attribute)]
        public string xMsgID { get; set; }
        [XmlMapping("xCode", 0, MappingType = MappingType.Attribute)]
        public string xCode { get; set; }
        [XmlMapping("xMessage", 0, MappingType = MappingType.Attribute)]
        public string xMessage { get; set; }
        [XmlMapping("xSign", 0, MappingType = MappingType.Attribute)]
        public string xSign { get; set; }
        [XmlMapping("xValue", 0, MappingType = MappingType.Attribute)]
        public string xValue { get; set; }
    }
}
