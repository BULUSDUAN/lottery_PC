using EntityModel.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kason.Net.Common.ExtensionFn
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
                        case "bf":
                            return ante.Replace("10", "1:0").Replace("20", "2:0").Replace("21", "2:1").Replace("30", "3:0").Replace("31", "3:1").Replace("32", "3:2").Replace("40", "4:0").Replace("41", "4:1").Replace("42", "4:2").Replace("50", "5:0").Replace("51", "5:1").Replace("52", "5:2").Replace("X0", "胜其他")
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
        public static string ToAnteString_zhongminToMatchIdLoc(this ITicket ticket, string issuseNumber)
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
}
