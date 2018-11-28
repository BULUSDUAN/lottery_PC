using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.XmlAnalyzer;
using Common;
using Common.Mappings;
using Common.Lottery.Objects;
using Common.Communication;
using Common.Algorithms;

namespace Common.Lottery.Gateway.HuAi
{
    #region 订单扩展函数

    public static class ObjectExtension
    {
        public static string ToAnteString_HuAi(this ITicket ticket, string PlayType)
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
                strList.Add(ante.ToAnteString_HuAi(ante.GameType ?? ticket.GameType, withGameType));
            }
            return string.Join("/", strList);
        }
        public static string ToAnteString_HuAi(this IAntecode ante, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                return ConvertAnteCodeHH(ante.GameCode, gameType, ante.AnteNumber, ConverMatchId(ante.MatchId), ConvertGameType(ante.GameCode, gameType));
            }
            else
            {
                // 71:[胜,负]
                return ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber, ConverMatchId(ante.MatchId));
            }
        }
        public static string ToAnteString_HuAi(this Ticket ticket, string gameCode)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.ToAnteString_HuAi(gameCode, ante.GameType));
            }
            //switch (gameCode.ToLower())
            //{
            //    case "bjdc":
            //    case "jczq":
            //    case "jclq":
            //        return string.Join("/", strList);
            //    default:
            //        return string.Join(";", strList);
            //}
            return string.Join("~", strList);
        }
        public static string ToAnteString_HuAi(this Antecode ante, string gameCode, string gameType)
        {
            if (gameCode.Equals("SSQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase))
            {
                return ante.AnteNumber.Replace("|", "-").Replace(",", " ");
            }
            else if (gameCode.Equals("SSQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase))
            {
                var hong = ante.AnteNumber.Split('|')[0];
                var lan = ante.AnteNumber.Split('|')[1];
                if (lan.Length > 35)
                {
                    var lan1 = ante.AnteNumber.Split('|')[1].Substring(0, 17);
                    var lan2 = ante.AnteNumber.Split('|')[1].Substring(18);
                    return hong.Replace(",", " ") + "-" + lan1.Replace(",", " ") + ";" + hong.Replace(",", " ") + "-" + lan2.Replace(",", " ");
                }
                return ante.AnteNumber.Replace("|", "-").Replace(",", " ");
            }
            else if (gameCode.Equals("SSQ", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                var tmp = ante.AnteNumber.Split('|');
                if (tmp.Length != 3)
                {
                    throw new ArgumentException("大乐透胆拖玩法号码错误 - " + ante.AnteNumber);
                }
                var hongdan = tmp[0];
                var hongtuo = tmp[1];
                var lan = tmp[2];
                if (lan.Length > 35)
                {
                    var lan1 = lan.Substring(0, 17);
                    var lan2 = lan.Substring(18);
                    return hongdan.Replace(",", " ") + "$" + hongtuo.Replace(",", " ") + "-" + lan1.Replace(",", " ") + ";" + hongdan.Replace(",", " ") + "$" + hongtuo.Replace(",", " ") + "-" + lan2.Replace(",", " ");
                }
                return string.Format("{0}${1}-{2}", hongdan.Replace(",", " "), hongtuo.Replace(",", " "), lan.Replace(",", " "));
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && (ante.GameType.Equals("DS", StringComparison.OrdinalIgnoreCase) || ante.GameType.Equals("FS", StringComparison.OrdinalIgnoreCase)))
            {
                return ante.AnteNumber.Replace("|", "#");
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                var tmp = ante.AnteNumber.Split('|');
                if (tmp.Length != 4)
                {
                    throw new ArgumentException("大乐透胆拖玩法号码错误 - " + ante.AnteNumber);
                }
                //05,06,07,08|01,02,03,04,09|01|02,03
                //(05,06,07,08),01,02,03,04,09+(01),02,03
                var hongdan = tmp[0];
                var hongtuo = tmp[1];
                var landan = tmp[2];
                var lantuo = tmp[3];
                return string.Format("{0}@{1}#{2}@{3}", hongdan, hongtuo, landan, lantuo);
            }
            else if (gameCode.Equals("CTZQ", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToUpper())
                {
                    case "T14C":
                        if (ante.AnteNumber.Replace(",", "").Length == 14)
                        {
                            return ante.AnteNumber;
                        }
                        else
                        {
                            var strL = ante.AnteNumber.Split(',');
                            var list = new List<string>();
                            foreach (var item in strL)
                            {
                                list.Add(string.Join(" ", item.ToArray()));
                            }
                            return string.Join(",", list);
                        }
                    case "TR9":
                        if (ante.AnteNumber.Replace(",", "").Replace("*", "").Length == 9)
                        {
                            return ante.AnteNumber;
                        }
                        else
                        {
                            var strL = ante.AnteNumber.Split(',');
                            var list = new List<string>();
                            foreach (var item in strL)
                            {
                                list.Add(string.Join(" ", item.ToArray()));
                            }
                            return string.Join(",", list);
                        }
                    case "T6BQC":
                        if (ante.AnteNumber.Replace(",", "").Length == 12)
                        {
                            return ante.AnteNumber;
                        }
                        else
                        {
                            var strL = ante.AnteNumber.Split(',');
                            var list = new List<string>();
                            foreach (var item in strL)
                            {
                                list.Add(string.Join(" ", item.ToArray()));
                            }
                            return string.Join(",", list);
                        }
                    case "T4CJQ":
                        if (ante.AnteNumber.Replace(",", "").Length == 8)
                        {
                            return ante.AnteNumber;
                        }
                        else
                        {
                            var strL = ante.AnteNumber.Split(',');
                            var list = new List<string>();
                            foreach (var item in strL)
                            {
                                list.Add(string.Join(" ", item.ToArray()));
                            }
                            return string.Join(",", list);
                        }
                    default:
                        return ante.AnteNumber;
                }
            }
            #region 重庆时时彩
            else if (gameCode.Equals("CQSSC", StringComparison.OrdinalIgnoreCase))
            {
                var touzhu = string.Empty;
                switch (ante.GameType.ToLower())
                {
                    case "1xdx":
                        var dx1xL = ante.AnteNumber.Replace("-,-,-,-,", "");
                        if (dx1xL.Length > 5)
                        {
                            return ConvertGameType(gameCode, gameType) + "|" + "-,-,-,-," + dx1xL.Substring(0, 5) + ";" + ConvertGameType(gameCode, gameType) + "|" + "-,-,-,-," + dx1xL.Substring(5);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "2xdx":
                        var dx2xL = ante.AnteNumber.Replace("-,-,-,", "").Split(',');
                        if ((dx2xL[0].Length + dx2xL[1].Length) > 12)
                        {
                            var list = new List<char[]>();
                            list.Add(dx2xL[0].ToArray());
                            list.Add(dx2xL[1].ToArray());
                            var strL = new List<string>();
                            var c = new ArrayCombination();
                            c.Calculate(list.ToArray(), (a) =>
                            {
                                strL.Add(ConvertGameType(gameCode, gameType) + "|" + "-,-,-," + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "3xdx":
                        var dx3xL = ante.AnteNumber.Replace("-,-,", "").Split(',');
                        if ((dx3xL[0].Length + dx3xL[1].Length + dx3xL[2].Length) > 24)
                        {
                            var list = new List<char[]>();
                            list.Add(dx3xL[0].ToArray());
                            list.Add(dx3xL[1].ToArray());
                            list.Add(dx3xL[2].ToArray());
                            var strL = new List<string>();
                            var c = new ArrayCombination();
                            c.Calculate(list.ToArray(), (a) =>
                            {
                                strL.Add(ConvertGameType(gameCode, gameType) + "|" + "-,-," + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "5xdx":
                        var dx5xL = ante.AnteNumber.Split(',');
                        if (ante.AnteNumber.Replace(",", "").Length > 45)
                        {
                            var list = new List<char[]>();
                            list.Add(dx5xL[0].ToArray());
                            list.Add(dx5xL[1].ToArray());
                            list.Add(dx5xL[2].ToArray());
                            list.Add(dx5xL[3].ToArray());
                            list.Add(dx5xL[4].ToArray());
                            var strL = new List<string>();
                            var c = new ArrayCombination();
                            c.Calculate(list.ToArray(), (a) =>
                            {
                                strL.Add(ConvertGameType(gameCode, gameType) + "|" + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "2xzxfs":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber.Replace(",", "");
                    case "5xtx":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "dxds":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber.Replace(",", "");
                    default:
                        return ante.AnteNumber;
                }
            }
            #endregion

            #region 江西11选5
            else if (gameCode.Equals("JX11X5", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "rx2":
                    case "rx3":
                    case "rx4":
                    case "rx5":
                    case "rx6":
                    case "rx7":
                    case "q2zux":
                    case "q3zux":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber.Replace(",", " ");
                    case "q2zhix":
                        if (ante.AnteNumber.Replace(",", " ").Split(' ').Length > 11)
                        {
                            var q2zhix = ante.AnteNumber.Replace(" ", "-").Split(',');
                            var list = new List<string[]>();
                            list.Add(q2zhix[0].Split('-'));
                            list.Add(q2zhix[1].Split('-'));
                            var strL = new List<string>();
                            var c = new ArrayCombination();
                            c.Calculate(list.ToArray(), (a) =>
                            {
                                if (a[0] != a[1])
                                    strL.Add(ConvertGameType(gameCode, gameType) + "|" + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "q3zhix":
                        if (ante.AnteNumber.Replace(",", " ").Split(' ').Length > 3)
                        {
                            var q3zhix = ante.AnteNumber.Replace(" ", "-").Split(',');
                            var list = new List<string[]>();
                            list.Add(q3zhix[0].Split('-'));
                            list.Add(q3zhix[1].Split('-'));
                            list.Add(q3zhix[2].Split('-'));
                            var strL = new List<string>();
                            var c = new ArrayCombination();
                            c.Calculate(list.ToArray(), (a) =>
                            {
                                if (a.Distinct().ToArray().Length == 3)
                                    strL.Add(ConvertGameType(gameCode, gameType) + "|" + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    default:
                        return ante.AnteNumber;
                }
            }
            #endregion

            #region 排列三
            else if (gameCode.Equals("PL3", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "hz":
                    case "zxhz":
                        return ante.AnteNumber.Length == 1 ? string.Format("0{0}", ante.AnteNumber) : ante.AnteNumber;
                    case "ds":
                    case "zx3ds":
                    case "zx6":
                    case "zx3fs":
                    case "zx6ds":
                        return ante.AnteNumber;
                    case "fs":
                        return string.Join(" ", ante.AnteNumber.ToCharArray()).Replace(" , ", ",");
                    default:
                        return ante.AnteNumber;
                }
            }
            #endregion
            else
            {
                return ante.AnteNumber;
            }
        }
        public static string ConvertGameType(string gameCode, string gameType, string playType = "")
        {
            playType = playType.Replace("P", "");
            switch (gameCode.ToLower())
            {
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "zjq":
                            if (playType == "1_1")
                                return "4015";
                            else
                                return "4005";
                        case "hh":
                            return "4006";
                        case "bf":
                            if (playType == "1_1")
                                return "4014";
                            else
                                return "4004";
                        case "brqspf":
                            if (playType == "1_1")
                                return "4011";
                            else
                                return "4001";
                        case "spf":
                            if (playType == "1_1")
                                return "4012";
                            else
                                return "4002";
                        case "bqc":
                            if (playType == "1_1")
                                return "4013";
                            else
                                return "4003";
                        default:
                            return gameType;
                    }
                case "ctzq":
                    switch (gameType.ToLower())
                    {
                        case "t14c":
                            return "1105";
                        case "tr9":
                            return "1108";
                        case "t6bqc":
                            return "1106";
                        case "t4cjq":
                            return "1107";
                        default:
                            return gameType;
                    }
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sf":
                            if (playType == "1_1")
                                return "4111";
                            else
                                return "4101";
                        case "hh":
                            return "4105";
                        case "rfsf":
                            if (playType == "1_1")
                                return "4112";
                            else
                                return "4102";
                        case "sfc":
                            if (playType == "1_1")
                                return "4113";
                            else
                                return "4103";
                        case "dxf":
                            if (playType == "1_1")
                                return "4114";
                            else
                                return "4104";
                        default:
                            return gameType;
                    }
                case "dlt":
                    return "1101";
                case "pl3":
                    return "1103";
                default:
                    return "";
            }
        }
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
        public static string ConverMatchId(string matchId)
        {
            var year = "20" + matchId.Substring(0, 2);
            var month = matchId.Substring(2, 2);
            var day = matchId.Substring(4, 2);
            var index = matchId.Substring(6);
            var date = DateTime.Parse(string.Format("{0}-{1}-{2}", year, month, day));
            var week = (int)date.DayOfWeek;
            if (week == 0) week = 7;
            return string.Format("{0}{1}{2}", date.ToString("yyyyMMdd"), week, index);
        }
        public static string ConvertAnteCode(string gameCode, string gameType, string ante, string matchId)
        {
            switch (gameCode.ToLower())
            {
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sf":
                        case "rfsf":
                            return string.Format("{0}:[{1}]", matchId, ante.Replace("3", "1").Replace("0", "2"));
                        case "sfc":
                            return string.Format("{0}:[{1}]", matchId, ante);
                        case "dxf":
                            return string.Format("{0}:[{1}]", matchId, ante.Replace("3", "1").Replace("0", "2"));
                    }
                    return ante;

                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                        case "brqspf":
                            return string.Format("{0}:[{1}]", matchId, ante);
                        case "zjq":
                            return string.Format("{0}:[{1}]", matchId, ante);
                        case "bf":
                            return string.Format("{0}:[{1}]", matchId, ante.Replace("X0", "90").Replace("XX", "99").Replace("0X", "09"));
                        case "bqc":
                            return string.Format("{0}:[{1}]", matchId, ante);
                    }
                    return ante;
                //case "bjdc":
                //    switch (gameType.ToLower())
                //    {
                //        case "spf":
                //            return matchId + "=" + ante.Replace(",", "/");
                //        case "zjq":
                //            return matchId + "=" + ante.Replace(",", "/");
                //        case "sxds":
                //            return matchId + "=" + ante.Replace("SD", "上单").Replace("SS", "上双").Replace("XD", "下单").Replace("XS", "下双").Replace(",", "/");
                //        case "bf":
                //            return matchId + "=" + ante.Replace("10", "1:0").Replace("20", "2:0").Replace("21", "2:1").Replace("30", "3:0").Replace("31", "3:1").Replace("32", "3:2").Replace("40", "4:0").Replace("41", "4:1").Replace("42", "4:2").Replace("50", "5:0").Replace("51", "5:1").Replace("52", "5:2").Replace("X0", "胜其它")
                //                .Replace("00", "0:0").Replace("11", "1:1").Replace("22", "2:2").Replace("33", "3:3").Replace("XX", "平其它")
                //                .Replace("01", "0:1").Replace("02", "0:2").Replace("12", "1:2").Replace("03", "0:3").Replace("13", "1:3").Replace("23", "2:3").Replace("04", "0:4").Replace("14", "1:4").Replace("24", "2:4").Replace("05", "0:5").Replace("15", "1:5").Replace("25", "2:5").Replace("0X", "负其它").Replace(",", "/");
                //        case "bqc":
                //            return matchId + "=" + ante.Replace("33", "3-3").Replace("31", "3-1").Replace("30", "3-0")
                //                .Replace("13", "1-3").Replace("11", "1-1").Replace("10", "1-0")
                //                .Replace("03", "0-3").Replace("01", "0-1").Replace("00", "0-0").Replace(",", "/");
                //    }
                //    return ante;
            }
            return ante;
        }
        public static string ConvertAnteCodeHH(string gameCode, string gameType, string ante, string matchId, string gameTypeId)
        {
            switch (gameCode.ToLower())
            {
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                        case "brqspf":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante);
                        case "zjq":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante);
                        case "bf":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante.Replace("X0", "90").Replace("XX", "99").Replace("0X", "09"));
                        case "bqc":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante);
                    }
                    return ante;
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sf":
                        case "rfsf":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante.Replace("3", "1").Replace("0", "2"));
                        case "sfc":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante);
                        case "dxf":
                            return string.Format("{0}={1}:[{2}]", gameTypeId, matchId, ante.Replace("3", "1").Replace("0", "2"));
                    }
                    return ante;
            }
            return ante;
        }
        /// <summary>
        /// 解析一张票的所注内容，每注号以/分隔
        /// </summary>
        public static string ToAnteString_Localhost_HA(this Ticket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.AnteNumber);
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
        public static string ToAnteString_LocalhostShop_HA(this ITicket ticket)
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
        public static string ToAnteString_HuAiToMatchId(this ITicket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(string.Format("{0}", ante.MatchId));
            }
            return string.Join(",", strList);
        }
        public static string ToAnteString_HuAiToMatchIdLoc(this ITicket ticket, string issuseNumber)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(string.Format("{0}{1}", issuseNumber, ante.MatchId));
            }
            return string.Join(",", strList);
        }

        /// <summary>
        /// 子类玩法
        /// </summary>
        public static string ConvertGameType_SZC(this ITicket ticket, string gameCode, string gameType, bool isTrue = false)
        {
            switch (gameCode.ToLower())
            {
                case "ctzq":
                    return "00";
                case "dlt":
                    if (isTrue)
                        return "01";
                    else
                        return "00";
                case "pl3":
                    switch (gameType.ToUpper())
                    {
                        case "ZXHZ":
                            return "02";
                        case "ZX3DS":
                        case "ZX3FS":
                            return "03";
                        case "ZX6DS":
                        case "ZX6":
                            return "04";
                        default:
                            return "01";
                    }
                default:
                    return "";
            }
        }
        public static string ConvertGameTypeIsDan(this ITicket ticket, string gameCode, string gameType, string betContent)
        {
            switch (gameCode.ToLower())
            {
                case "ctzq":
                    switch (gameType.ToLower())
                    {
                        case "t14c":
                            if (betContent.Replace(",", "").Length == 14)
                                return "01";
                            else
                                return "02";
                        case "tr9":
                            if (betContent.Replace(",", "").Replace("*", "").Length == 9)
                                return "01";
                            else
                                return "02";
                        case "t6bqc":
                            if (betContent.Replace(",", "").Length == 12)
                                return "01";
                            else
                                return "02";
                        case "t4cjq":
                            if (betContent.Replace(",", "").Length == 8)
                                return "01";
                            else
                                return "02";
                        default:
                            return "00";
                    }
                case "dlt":
                    switch (gameType.ToUpper())
                    {
                        case "DS":
                            return "01";
                        case "FS":
                            return "02";
                        case "DT":
                            return "03";
                        default:
                            return "00";
                    }
                case "pl3":
                    switch (gameType.ToUpper())
                    {
                        case "DS":
                            return "01";
                        case "FS":
                            return "02";
                        case "HZ":
                            return "04";
                        case "ZXHZ":
                            return "04";
                        case "ZX3DS":
                            return "01";
                        case "ZX3FS":
                            return "02";
                        case "ZX6DS":
                            return "01";
                        case "ZX6":
                            return "02";
                        default:
                            return "00";
                    }
                default:
                    return "00";
            }
        }


        //串关方式转换
        public static string PlayTypeM_N(this ITicket ticket, string playType)
        {
            playType = playType.Replace("P", "");
            switch (playType)
            {
                case "1_1":
                    return "01";
                case "2_1":
                    return "02";
                case "3_1":
                    return "03";
                case "3_3":
                    return "04";
                case "3_4":
                    return "05";
                case "4_1":
                    return "06";
                case "4_4":
                    return "07";
                case "4_5":
                    return "08";
                case "4_6":
                    return "09";
                case "4_11":
                    return "10";
                case "5_1":
                    return "11";
                case "5_5":
                    return "12";
                case "5_6":
                    return "13";
                case "5_10":
                    return "14";
                case "5_16":
                    return "15";
                case "5_20":
                    return "16";
                case "5_26":
                    return "17";
                case "6_1":
                    return "18";
                case "6_6":
                    return "19";
                case "6_7":
                    return "20";
                case "6_15":
                    return "21";
                case "6_20":
                    return "22";
                case "6_22":
                    return "23";
                case "6_35":
                    return "24";
                case "6_42":
                    return "25";
                case "6_50":
                    return "26";
                case "6_57":
                    return "27";
                case "7_1":
                    return "28";
                case "7_7":
                    return "29";
                case "7_8":
                    return "30";
                case "7_21":
                    return "31";
                case "7_35":
                    return "32";
                case "7_120":
                    return "33";
                case "8_1":
                    return "34";
                case "8_8":
                    return "35";
                case "8_9":
                    return "36";
                case "8_28":
                    return "37";
                case "8_56":
                    return "38";
                case "8_70":
                    return "39";
                case "8_247":
                    return "40";
                default:
                    return string.Empty;
            }
        }

        //串关方式转换
        public static string PlayTypeM_N(string playType)
        {
            playType = playType.Replace("P", "");
            switch (playType)
            {
                case "1_1":
                    return "01";
                case "2_1":
                    return "02";
                case "3_1":
                    return "03";
                case "3_3":
                    return "04";
                case "3_4":
                    return "05";
                case "4_1":
                    return "06";
                case "4_4":
                    return "07";
                case "4_5":
                    return "08";
                case "4_6":
                    return "09";
                case "4_11":
                    return "10";
                case "5_1":
                    return "11";
                case "5_5":
                    return "12";
                case "5_6":
                    return "13";
                case "5_10":
                    return "14";
                case "5_16":
                    return "15";
                case "5_20":
                    return "16";
                case "5_26":
                    return "17";
                case "6_1":
                    return "18";
                case "6_6":
                    return "19";
                case "6_7":
                    return "20";
                case "6_15":
                    return "21";
                case "6_20":
                    return "22";
                case "6_22":
                    return "23";
                case "6_35":
                    return "24";
                case "6_42":
                    return "25";
                case "6_50":
                    return "26";
                case "6_57":
                    return "27";
                case "7_1":
                    return "28";
                case "7_7":
                    return "29";
                case "7_8":
                    return "30";
                case "7_21":
                    return "31";
                case "7_35":
                    return "32";
                case "7_120":
                    return "33";
                case "8_1":
                    return "34";
                case "8_8":
                    return "35";
                case "8_9":
                    return "36";
                case "8_28":
                    return "37";
                case "8_56":
                    return "38";
                case "8_70":
                    return "39";
                case "8_247":
                    return "40";
                default:
                    return string.Empty;
            }
        }

        public static string ToAnteString_HuAi(string gameCode, string gameType, string locBetContent)
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
                strList.Add(ToAnteString_HuAi(array.Length > 2 ? array[2] : array[1], array.Length > 2 ? array[1] : array[0], gameCode, array.Length > 2 ? array[0] : gameType, withGameType));
            }
            return string.Join("/", strList);
        }

        public static string ToAnteString_HuAi(string anteNumber, string matchId, string gameCode, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                return ConvertAnteCodeHH(gameCode, gameType, anteNumber, ConverMatchId(matchId), ConvertGameType(gameCode, gameType));
            }
            else
            {
                // 71:[胜,负]
                return ConvertAnteCode(gameCode, gameType, anteNumber, ConverMatchId(matchId));
            }
        }
    }

    #endregion

}
