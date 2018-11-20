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

namespace Common.Lottery.Gateway.Liangcai
{
    #region 订单扩展函数

    public static class ObjectExtension
    {
        public static string ToAnteString_LiangCai(this ITicket ticket, string PlayType)
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
                strList.Add(ante.ToAnteString_LiangCai(ante.GameType ?? ticket.GameType, withGameType));
            }
            //return string.Join(";", strList);
            return string.Format("{0}|{1}|{2}", ConvertGameType(ticket.GameCode, ticket.GameType), string.Join(",", strList), ConvertPlayType(ticket.GameCode, PlayType));
        }
        public static string ToAnteString_LiangCai(this IAntecode ante, string gameType, bool withGameType = false)
        {
            if (withGameType)
            {
                //混合过关
                return ConvertAnteCodeHH(ante.GameCode, gameType, ante.AnteNumber, ante.MatchId);
            }
            else
            {
                // 71:[胜,负]
                return ConvertAnteCode(ante.GameCode, gameType, ante.AnteNumber, ante.MatchId);
            }
        }
        public static string ConvertPlayType(string gameCode, string playType)
        {
            switch (gameCode.ToUpper())
            {
                case "BJDC":
                    return playType.Replace("P", "").Replace("_", "*").Replace("11*1", "#").Replace("1*1", "单关").Replace("#", "11*1");
                case "JCZQ":
                case "JCLQ":
                    return playType.Replace("P", "").Replace("_", "*");
                default:
                    break;
            }
            return playType;
        }
        public static string ToAnteString_LiangCai(this Ticket ticket, string gameCode)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.AnteCodeList)
            {
                strList.Add(ante.ToAnteString_LiangCai(gameCode, ante.GameType));
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
            return string.Join(";", strList);
        }
        public static string ToAnteString_LiangCai(this Antecode ante, string gameCode, string gameType)
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
                return ante.AnteNumber.Replace("|", "-").Replace(",", " ");
            }
            else if (gameCode.Equals("DLT", StringComparison.OrdinalIgnoreCase) && ante.GameType.Equals("DT", StringComparison.OrdinalIgnoreCase))
            {
                var tmp = ante.AnteNumber.Replace(",", " ").Split('|');
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
                return string.Format("{0}${1}-{2}${3}", hongdan, hongtuo, landan, lantuo);
            }
            else if (gameCode.Equals("CTZQ", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToUpper())
                {
                    case "T14C":
                        if (ante.AnteNumber.Replace(",", "").Length > 38)
                        {
                            throw new ArgumentException("传统足球14场玩法号码错误 - " + ante.AnteNumber);
                        }
                        return ante.AnteNumber;
                    case "TR9":
                        if (ante.AnteNumber.Replace(",", "").Length > 27)
                        {
                            throw new ArgumentException("传统足球任选9场玩法号码错误 - " + ante.AnteNumber);
                        }
                        return ante.AnteNumber.Replace("*", "#");
                    case "T6BQC":
                        if (ante.AnteNumber.Replace(",", "").Length > 32)
                        {
                            throw new ArgumentException("传统足球6场半全场玩法号码错误 - " + ante.AnteNumber);
                        }
                        return ante.AnteNumber;
                    case "T4CJQ":
                        if (ante.AnteNumber.Replace(",", "").Length > 32)
                        {
                            throw new ArgumentException("传统足球4场进球玩法号码错误 - " + ante.AnteNumber);
                        }
                        return ante.AnteNumber;
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

            #region 排列三、福彩3D
            else if (gameCode.Equals("PL3", StringComparison.OrdinalIgnoreCase) || gameCode.Equals("FC3D", StringComparison.OrdinalIgnoreCase))
            {
                switch (ante.GameType.ToLower())
                {
                    case "ds":
                    case "fs":
                        var pl = ante.AnteNumber.Replace(",", "");
                        if (pl.Length > 24)
                        {
                            var pl3fs = ante.AnteNumber.Split(',');
                            var list = new List<char[]>();
                            list.Add(pl3fs[0].ToArray());
                            list.Add(pl3fs[1].ToArray());
                            list.Add(pl3fs[2].ToArray());
                            var strL = new List<string>();
                            var c = new ArrayCombination();
                            c.Calculate(list.ToArray(), (a) =>
                            {
                                strL.Add(ConvertGameType(gameCode, gameType) + "|" + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "hz":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "zx3ds":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "zx6":
                        if (ante.AnteNumber.Replace(",", "").Length > 3)
                        {
                            var num = ante.AnteNumber.Replace(",", "");
                            var c = new Combination();
                            var strL = new List<string>();
                            c.Calculate(num.ToArray(), 3, (a) =>
                            {
                                strL.Add(ConvertGameType(gameCode, gameType) + "|" + string.Join(",", a));
                            });
                            return string.Join(";", strL);
                        }
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber;
                    case "zx3fs":
                        return ConvertGameType(gameCode, gameType) + "|" + ante.AnteNumber.Replace(",", "");
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
        public static string ConvertGameType(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "jx11x5":
                    switch (gameType.ToLower())
                    {
                        case "rx1":
                            return "";
                        case "rx2":
                            return "R2";
                        case "rx3":
                            return "R3";
                        case "rx4":
                            return "R4";
                        case "rx5":
                            return "R5";
                        case "rx6":
                            return "R6";
                        case "rx7":
                            return "R7";
                        case "rx8":
                            return "R8";
                        case "q2zhix":
                            return "Q2";
                        case "q3zhix":
                            return "Q3";
                        case "q2zux":
                            return "Z2";
                        case "q3zux":
                            return "Z3";
                        default:
                            return "";
                    }
                case "fc3d":
                case "pl3":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                        case "fs":
                            return "1";
                        case "hz":
                            return "S1";
                        case "zx3ds":
                        case "zx6":
                            return "6";
                        case "zx3fs":
                            return "F3";
                        default:
                            return "";
                    }
                case "cqssc":
                    switch (gameType.ToLower())
                    {
                        case "1xdx":
                            return "1D";
                        case "2xdx":
                            return "2D";
                        case "3xdx":
                            return "3D";
                        case "5xdx":
                            return "5D";
                        case "2xzxfs":
                            return "F2";
                        case "5xtx":
                            return "5T";
                        case "dxds":
                            return "DD";
                        default:
                            return "";
                    }
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "zjq":
                            return "JQS";
                        case "hh":
                            return "HH";
                        case "bf":
                            return "CBF";
                        case "brqspf":
                            return "SPF";
                        case "spf":
                            return "RQSPF";
                        default:
                            return gameType;
                    }
                case "jclq":
                    return gameType;
                case "bjdc":
                    switch (gameType.ToLower())
                    {
                        case "zjq":
                            return "JQS";
                        case "sxds":
                            return "SXP";
                        case "bf":
                            return "CBF";
                        default:
                            return gameType;
                    }
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
        public static string ConvertAnteCode(string gameCode, string gameType, string ante, string matchId)
        {
            switch (gameCode.ToLower())
            {
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sf":
                        case "rfsf":
                        case "sfc":
                        case "dxf":
                            return matchId + "=" + ante.Replace(",", "/");
                    }
                    return ante;
                case "bjdc":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                            return matchId + "=" + ante.Replace(",", "/");
                        case "zjq":
                            return matchId + "=" + ante.Replace(",", "/");
                        case "sxds":
                            return matchId + "=" + ante.Replace("SD", "上单").Replace("SS", "上双").Replace("XD", "下单").Replace("XS", "下双").Replace(",", "/");
                        case "bf":
                            return matchId + "=" + ante.Replace("10", "1:0").Replace("20", "2:0").Replace("21", "2:1").Replace("30", "3:0").Replace("31", "3:1").Replace("32", "3:2").Replace("40", "4:0").Replace("41", "4:1").Replace("42", "4:2").Replace("50", "5:0").Replace("51", "5:1").Replace("52", "5:2").Replace("X0", "胜其它")
                                .Replace("00", "0:0").Replace("11", "1:1").Replace("22", "2:2").Replace("33", "3:3").Replace("XX", "平其它")
                                .Replace("01", "0:1").Replace("02", "0:2").Replace("12", "1:2").Replace("03", "0:3").Replace("13", "1:3").Replace("23", "2:3").Replace("04", "0:4").Replace("14", "1:4").Replace("24", "2:4").Replace("05", "0:5").Replace("15", "1:5").Replace("25", "2:5").Replace("0X", "负其它").Replace(",", "/");
                        case "bqc":
                            return matchId + "=" + ante.Replace("33", "3-3").Replace("31", "3-1").Replace("30", "3-0")
                                .Replace("13", "1-3").Replace("11", "1-1").Replace("10", "1-0")
                                .Replace("03", "0-3").Replace("01", "0-1").Replace("00", "0-0").Replace(",", "/");
                    }
                    return ante;
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                        case "brqspf":
                            return matchId + "=" + ante.Replace(",", "/");
                        case "zjq":
                            return matchId + "=" + ante.Replace(",", "/");
                        case "bf":
                            return matchId + "=" + ante.Replace("10", "1:0").Replace("20", "2:0").Replace("21", "2:1").Replace("30", "3:0").Replace("31", "3:1").Replace("32", "3:2").Replace("40", "4:0").Replace("41", "4:1").Replace("42", "4:2").Replace("50", "5:0").Replace("51", "5:1").Replace("52", "5:2").Replace("X0", "9:0")
                                .Replace("00", "0:0").Replace("11", "1:1").Replace("22", "2:2").Replace("33", "3:3").Replace("XX", "9:9")
                                .Replace("01", "0:1").Replace("02", "0:2").Replace("12", "1:2").Replace("03", "0:3").Replace("13", "1:3").Replace("23", "2:3").Replace("04", "0:4").Replace("14", "1:4").Replace("24", "2:4").Replace("05", "0:5").Replace("15", "1:5").Replace("25", "2:5").Replace("0X", "0:9").Replace(",", "/");
                        case "bqc":
                            return matchId + "=" + ante.Replace("33", "3-3").Replace("31", "3-1").Replace("30", "3-0")
                                .Replace("13", "1-3").Replace("11", "1-1").Replace("10", "1-0")
                                .Replace("03", "0-3").Replace("01", "0-1").Replace("00", "0-0").Replace(",", "/");
                    }
                    return ante;
            }
            return ante;
        }
        public static string ConvertAnteCodeHH(string gameCode, string gameType, string ante, string matchId)
        {
            switch (gameCode.ToLower())
            {
                case "jclq":
                    switch (gameType.ToLower())
                    {
                        case "sf":
                            return "SF>" + matchId + "=" + ante.Replace(",", "/");
                        case "rfsf":
                            return "RFSF>" + matchId + "=" + ante.Replace(",", "/");
                        case "sfc":
                            return "SFC>" + matchId + "=" + ante.Replace(",", "/");
                        case "dxf":
                            return "DXF>" + matchId + "=" + ante.Replace(",", "/");
                    }
                    return ante;
                case "jczq":
                    switch (gameType.ToLower())
                    {
                        case "spf":
                            return "RQSPF>" + matchId + "=" + ante.Replace(",", "/");
                        case "brqspf":
                            return "SPF>" + matchId + "=" + ante.Replace(",", "/");
                        case "zjq":
                            return "JQS>" + matchId + "=" + ante.Replace(",", "/");
                        case "bf":
                            return "CBF>" + matchId + "=" + ante.Replace("10", "1:0").Replace("20", "2:0").Replace("21", "2:1").Replace("30", "3:0").Replace("31", "3:1").Replace("32", "3:2").Replace("40", "4:0").Replace("41", "4:1").Replace("42", "4:2").Replace("50", "5:0").Replace("51", "5:1").Replace("52", "5:2").Replace("X0", "9:0")
                                .Replace("00", "0:0").Replace("11", "1:1").Replace("22", "2:2").Replace("33", "3:3").Replace("XX", "9:9")
                                .Replace("01", "0:1").Replace("02", "0:2").Replace("12", "1:2").Replace("03", "0:3").Replace("13", "1:3").Replace("23", "2:3").Replace("04", "0:4").Replace("14", "1:4").Replace("24", "2:4").Replace("05", "0:5").Replace("15", "1:5").Replace("25", "2:5").Replace("0X", "0:9").Replace(",", "/");
                        case "bqc":
                            return "BQC>" + matchId + "=" + ante.Replace("33", "3-3").Replace("31", "3-1").Replace("30", "3-0")
                                .Replace("13", "1-3").Replace("11", "1-1").Replace("10", "1-0")
                                .Replace("03", "0-3").Replace("01", "0-1").Replace("00", "0-0").Replace(",", "/");
                    }
                    return ante;
            }
            return ante;
        }
        /// <summary>
        /// 解析一张票的所注内容，每注号以/分隔
        /// </summary>
        public static string ToAnteString_Localhost_LC(this Ticket ticket)
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
        public static string ToAnteString_LocalhostShop_LC(this ITicket ticket)
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
        public static string ToAnteString_liangcaiToMatchId(this ITicket ticket)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(string.Format("{0}", ante.MatchId));
            }
            return string.Join(",", strList);
        }
        public static string ToAnteString_liangcaiToMatchIdLoc(this ITicket ticket, string issuseNumber)
        {
            var strList = new List<string>();
            foreach (var ante in ticket.GetAnteCodeList())
            {
                strList.Add(string.Format("{0}{1}", issuseNumber, ante.MatchId));
            }
            return string.Join(",", strList);
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
    }

    #endregion

}
