using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.XmlAnalyzer;
using Common.Lottery.Objects;

namespace Common.Lottery.Gateway.Shop
{
    public static class ObjectExtension
    {
        //数字彩
        public static string ToAnteString_Shop(this Ticket ticket, string gameCode)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.ToAnteString_Shop(gameCode));
            }
            return string.Join("", strList);
        }
        public static string ToAnteString_Shop(this Antecode ante, string gameCode)
        {
            if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("{0}^", ante.AnteNumber.Replace(",", ""));
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("*{0}^", ante.AnteNumber.Replace(",", "").Replace("|", "|*"));
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                //05,06,17,18|01,02,13,14|06|01,05
                //102035*182328|07*0102030405060809101112^
                var tmp = ante.AnteNumber.Split('|');
                if (tmp.Length != 4)
                {
                    throw new ArgumentException("大乐透胆拖玩法号码错误 - " + ante.AnteNumber);
                }
                var hongdan = tmp[0];
                var hongtuo = tmp[1];
                var landan = tmp[2];
                var lantuo = tmp[3];
                return string.Format("{0}*{1}|{2}*{3}^", hongdan, hongtuo, landan, lantuo).Replace(",", "");
            }
            else if (gameCode.Equals("CTZQ", StringComparison.OrdinalIgnoreCase))
            {
                if (ante.GameType.Equals("TR9", StringComparison.OrdinalIgnoreCase))
                    return ante.AnteNumber.Replace("*", "4").Replace(",", "*") + "^";
                else
                    return string.Format("{0}^", ante.AnteNumber.Replace(",", "*"));
            }
            else if (gameCode.Equals("PL3", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "ds":
                    case "fs":
                    case "zx3ds":
                    case "zx6ds":
                        return ante.AnteNumber.Replace(",", "*") + "^";
                    case "zx6":
                        //return ante.AnteNumber.Length > 5 ? string.Format("**{0}^", ante.AnteNumber.Replace(",", "")) : ante.AnteNumber.Replace(",", "*") + "^";// ante.AnteNumber.Replace(",", "*") + "^";
                        return string.Format("**{0}^", ante.AnteNumber.Replace(",", ""));
                    case "hz":
                    case "zxhz":
                        return string.Format("**{0}^", ante.AnteNumber.Length == 1 ? "0" + ante.AnteNumber : ante.AnteNumber);
                    case "zx3fs":
                        return string.Format("**{0}^", ante.AnteNumber.Replace(",", ""));
                    default:
                        break;
                }
                return ante.AnteNumber;
            }
            #region JX11X5
            //else if (gameCode.Equals("JX11X5", StringComparison.OrdinalIgnoreCase))
            //{
            //    switch (ante.GameType.ToLower())
            //    {
            //        case "rx2":
            //        case "RX3":
            //        case "RX4":
            //        case "RX5":
            //        case "RX6":
            //        case "RX7":
            //        case "RX8":
            //        case "q2zux":
            //        case "q3zux":
            //            return ante.AnteNumber;
            //        case "q2zhix":
            //            if (ante.AnteNumber.Length == "04|10".Length)
            //            {
            //                return ante.AnteNumber.Replace(",", "|");
            //            }
            //            else
            //            {
            //                ante.AnteNumber = ante.AnteNumber.Replace(",", "|");
            //                ante.AnteNumber = ante.AnteNumber.Replace(" ", ",");
            //                return ante.AnteNumber;
            //            }
            //        case "q3zhix":
            //            if (ante.AnteNumber.Length == "05|08|11".Length)
            //            {
            //                return ante.AnteNumber.Replace(",", "|");
            //            }
            //            else
            //            {
            //                ante.AnteNumber = ante.AnteNumber.Replace(",", "|");
            //                ante.AnteNumber = ante.AnteNumber.Replace(" ", ",");
            //                return ante.AnteNumber;
            //            }
            //        default:
            //            break;
            //    }

            //    return ante.AnteNumber;
            //}
            #endregion
            else
            {
                return ante.AnteNumber;
            }
            //todo:
        }
        //竞彩、北单
        public static string ToAnteString_Shop(this ITicket ticket)
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
                strList.Add(ante.ToAnteString_Shop(ante.GameType ?? ticket.GameType, withGameType));
            }
            return string.Join("", strList);
        }
        public static string ToAnteString_Shop(this IAntecode ante, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                // SF@1-001:[主胜,客胜]
                return string.Format("{0}{1}{2}^", ConverMatchId(ante.GameCode, ante.MatchId), ConvertGameCode(ante.GameCode, gameType), ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber));
            }
            else
            {
                // 71:[胜,负]
                return string.Format("{0}{1}^", ConverMatchId(ante.GameCode, ante.MatchId), ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber));
            }
        }




        /// <summary>
        /// 打票机玩法编码
        /// </summary>
        public static string ConvertGameType(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                    switch (gameType.ToUpper())
                    {
                        case "ZJQ":
                            return "Z53";
                        case "HH":
                            return "Z59";
                        case "SPF":
                            return "Z56";
                        case "BRQSPF":
                            return "Z51";
                        case "BF":
                            return "Z52";
                        case "BQC":
                            return "Z54";
                        default:
                            return "";
                    }
                case "jclq":
                    switch (gameType.ToUpper())
                    {
                        case "RFSF":
                            return "L61";
                        case "SF":
                            return "L62";
                        case "SFC":
                            return "L63";
                        case "DXF":
                            return "L64";
                        case "HH":
                            return "Z69";
                        default:
                            return "";
                    }
                case "ctzq":
                    switch (gameType.ToUpper())
                    {
                        case "T14C":
                            return "D14";
                        case "TR9":
                            return "D9";
                        case "T6BQC":
                            return "C12";
                        case "T4CJQ":
                            return "C4";
                        default:
                            return "";
                    }
                case "pl3":
                    return "D3";
                case "dlt":
                    return "T001";
                //case "jx11x5":
                //    return 112;
                default:
                    return "";
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
                            return 53;
                        case "SPF":
                            return 56;
                        case "BRQSPF":
                            return 51;
                        case "BF":
                            return 52;
                        case "BQC":
                            return 54;
                        default:
                            return 0;
                    }
                case "jclq":
                    switch (gameType.ToUpper())
                    {
                        case "RFSF":
                            return 61;
                        case "SF":
                            return 62;
                        case "SFC":
                            return 63;
                        case "DXF":
                            return 64;
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
                        return string.Format("{0}{1}", week, index);
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
                        case "dxf":
                            return ante.Replace("3", "1").Replace("0", "2").Replace(",", "");
                        case "sfc":
                            return ante.Replace(",", "");
                    }
                    return ante;
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                        case "brqspf":
                        case "bqc":
                        case "zjq":
                            return ante.Replace(",", "");
                        case "bf":
                            return ante.Replace("X0", "90").Replace("XX", "99").Replace("0X", "09").Replace(",", "");
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
        /// <summary>
        /// 解析一张票的所注内容，每注号以/分隔
        /// </summary>
        public static string ToAnteString_Localhost_TM(this Ticket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.AnteNumber);
            }
            return string.Join("/", strList);
        }
        public static string ToAnteString_Shop_Loc(this ITicket ticket)
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

        public static string ToAnteString_TMToMatchIdLoc(this ITicket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(ante.MatchId);
            }
            return string.Join(",", strList);
        }

        //竞彩、北单
        public static string ToAnteString_Shop(string gameCode, string gameType, string locBetContent)
        {
            var strList = new List<string>();
            var withGameType = false;
            if (!string.IsNullOrEmpty(gameType) && gameType.Equals("HH", StringComparison.OrdinalIgnoreCase))
            {
                withGameType = true;
            }
          
            foreach (var ante in locBetContent.Split('/'))
            {
                var array = ante.Split('_');
                strList.Add(ToAnteString_Shop(array.Length > 2 ? array[2] : array[1], array.Length > 2 ? array[1] : array[0], gameCode, array.Length > 2 ? array[0] : gameType, withGameType));
            }
            return string.Join("", strList);
        }
        public static string ToAnteString_Shop(string anteNumber, string matchId, string gameCode, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                // SF@1-001:[主胜,客胜]
                return string.Format("{0}{1}{2}^", ConverMatchId(gameCode, matchId), ConvertGameCode(gameCode, gameType), ConvertAnteCode(gameCode, gameType, anteNumber));
            }
            else
            {
                // 71:[胜,负]
                return string.Format("{0}{1}^", ConverMatchId(gameCode, matchId), ConvertAnteCode(gameCode, gameType, anteNumber));
            }
        }
    }
}
