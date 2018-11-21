using EntityModel;
using EntityModel.Domain.Entities;
using EntityModel.ExceptionExtend;
using EntityModel.GameBiz.Core;
using EntityModel.Ticket;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Net;
using System.Globalization;
using MongoDB.Driver;

namespace KaSon.FrameWork.Common.Sport
{
    public class BettingHelper
    {
        /// <summary>
        /// 申请提现编号
        /// </summary>
        public static string GetWithdrawId()
        {
            string prefix = "MWD";
            return prefix + UsefullHelper.UUID();
        }


        /// <summary>
        /// 第三方游戏交易订单号
        /// </summary>
        /// <returns></returns>
        public static string GetGameTransferId()
        {
            string prefix = "GTD";
            return prefix + UsefullHelper.UUID();
        }
        public static string ConvertGameType(string gameCode, string gameType, string betType, int betCount)
        {
            switch (gameCode.ToLower())
            {
                case "ctzq":
                    return betType;
                //case "dlt":
                //    return betCount > 1 ? "FS" : "DS";
                //switch (gameType.ToLower())
                //{
                //    case "dt":
                //        return "FS";
                //    default:
                //        return gameType;
                //}
                case "cqssc":
                    switch (gameType.ToLower())
                    {
                        case "1xdx":
                            return "ZQSSC_1X_DS";
                        case "2xdx":
                            return string.Format("ZQSSC_2X_{0}", betType == "DS" ? "DS" : "FS");
                        case "3xdx":
                            return string.Format("ZQSSC_3X_{0}", betType == "DS" ? "DS" : "FS");
                        case "5xdx":
                            return string.Format("ZQSSC_5X_{0}", betType == "DS" ? "DS" : "FS");
                        case "2xzxfs":
                            return string.Format("ZQSSC_2XZX{0}", betType == "DS" ? "_DS" : "ZH");
                        case "2xhz":
                            return "ZQSSC_2XHZ";
                        case "2xzxfw":
                            return "ZQSSC_2XZXFZ";
                        case "2xbaodan":
                            return "ZQSSC_2XZX_BD";
                        case "3xzxzh":
                            if (betCount == 6)
                            {
                                return string.Format("ZQSSC_3XZH");
                            }
                            else
                            {
                                return string.Format("ZQSSC_3XZH_FS");
                            }
                        case "3xbaodan":
                            return "ZQSSC_3XZX_BD";
                        case "3xhz":
                            return "ZQSSC_3XHZ";
                        case "zx3ds":
                            return "ZQSSC_3XZ3_DS";
                        case "zx3fs":
                            return "ZQSSC_3XZ3_FS";
                        case "3xzxhz":
                            return "ZQSSC_3XZXHZ";
                        case "5xtx":
                            return "ZQSSC_5XTX";
                        case "dxds":
                            return "ZQSSC_DXDS";
                        case "zx6":
                            return string.Format("ZQSSC_3XZ6_{0}", betType == "DS" ? "DS" : "FS");
                        default:
                            return gameType;
                    }
                case "jx11x5":
                    switch (gameType.ToLower())
                    {
                        case "rx1":
                            return "11_RX1";
                        case "rx2":
                            return "11_RX2";
                        case "rx3":
                            return "11_RX3";
                        case "rx4":
                            return "11_RX4";
                        case "rx5":
                            return "11_RX5";
                        case "rx6":
                            return "11_RX6";
                        case "rx7":
                            return "11_RX7";
                        case "rx8":
                            return "11_RX8";
                        case "q2zhix":
                            return string.Format("11_ZXQ2_{0}", betType == "DS" ? "D" : "F");
                        case "q3zhix":
                            return string.Format("11_ZXQ3_{0}", betType == "DS" ? "D" : "F");
                        case "q2zux":
                            return "11_ZXQ2";
                        case "q3zux":
                            return "11_ZXQ3";
                        default:
                            return gameType;
                    }
                case "fc3d":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "fs":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "hz":
                            return "ZXHZ";
                        case "zx3ds":
                            return "ZX_DS";
                        case "zx3fs":
                            return "Z3FS";
                        case "zx6":
                            return string.Format("Z{0}", betType == "DS" ? "X_DS" : "6FS"); ;

                        default:
                            return gameType;
                    }
                case "pl3":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "fs":
                            return string.Format("ZX{0}", betType == "DS" ? "DS" : "FS"); ;
                        case "hz":
                            return "ZXHZ";
                        case "zx3ds":
                            return "ZX_DS";
                        case "zx3fs":
                            return "ZXZ3";
                        case "zx6":
                            if (betType == "DS")
                            {
                                return "ZX_DS";
                            }
                            else
                            {
                                return "ZXZ6";
                            }
                        default:
                            return gameType;
                    }
                default:
                    return gameType;
            }
        }

        public static IEnumerable<EntityModel.Ticket.Ticket> GetTicketsByAntecodes(IEnumerable<Antecode> antecodeList, int maxCount, int maxAmount, Func<EntityModel.Ticket.Ticket> createTicketHandler)
        {
            var ticketList = new List<EntityModel.Ticket.Ticket>();
            var tmpTicket = createTicketHandler();
            var tmpAmount = tmpTicket.Amount;
            while (tmpAmount > 0)
            {
                var currentAmount = maxAmount;
                if (tmpAmount <= maxAmount)
                {
                    currentAmount = tmpAmount;
                }
                tmpAmount -= maxAmount;
                tmpTicket.Amount = currentAmount;
                foreach (var antecode in antecodeList)
                {
                    // 添加号码到票
                    tmpTicket.AnteCodeList.Add(antecode);
                    // 如果票包含的号码数量达到上限，则将票添加到列表，并重建票对象
                    if (tmpTicket.AnteCodeList.Count >= maxCount)
                    {
                        ticketList.Add(tmpTicket);
                        tmpTicket = createTicketHandler();
                        tmpTicket.Amount = currentAmount;
                    }
                }
                if (tmpTicket.AnteCodeList.Count > 0)
                {
                    ticketList.Add(tmpTicket);
                    tmpTicket = createTicketHandler();
                    tmpTicket.Amount = currentAmount;
                }
            }
            return ticketList.ToArray();
        }


        public static TicketCollection AnalyzeTickets(Order order)
        {
            // 将所有号码，按照玩法进行分组
            var groupAntecodes = order.AntecodeList.GroupBy((item) => item.GameType);
            var ticketList = new TicketCollection();
            foreach (var group in groupAntecodes)
            {
                var gameType = group.Key;
                // 获取玩法一张票最多可以携带的号码数量
                var maxCount = GetMaxAntecodeCountEachTicket(order.GameCode, gameType);
                // 解析此玩法所有号码，并返回多张票
                var innerTicketList = GetTicketsByAntecodes(group.ToArray(), maxCount, GetMaxTicketAmount(order.GameCode), () => new Ticket() { GameType = gameType, Amount = order.Amount, });
                ticketList.AddRange(innerTicketList);
            }
            return ticketList;
        }
        public static System.Data.DataTable GetNewTicketTable()
        {
            var ticketTable = new System.Data.DataTable("C_Sports_Ticket");
            ticketTable.Columns.Add("Id", typeof(long));
            ticketTable.Columns.Add("SchemeId", typeof(string));
            ticketTable.Columns.Add("TicketId", typeof(string));
            ticketTable.Columns.Add("GameCode", typeof(string));
            ticketTable.Columns.Add("GameType", typeof(string));
            ticketTable.Columns.Add("PlayType", typeof(string));
            ticketTable.Columns.Add("MatchIdList", typeof(string));
            ticketTable.Columns.Add("IssuseNumber", typeof(string));
            ticketTable.Columns.Add("BetUnits", typeof(int));
            ticketTable.Columns.Add("Amount", typeof(int));
            ticketTable.Columns.Add("BetMoney", typeof(decimal));
            ticketTable.Columns.Add("BetContent", typeof(string));
            ticketTable.Columns.Add("LocOdds", typeof(string));
            ticketTable.Columns.Add("TicketStatus", typeof(int));
            ticketTable.Columns.Add("TicketLog", typeof(string));
            ticketTable.Columns.Add("PartnerId", typeof(string));
            ticketTable.Columns.Add("Palmid", typeof(string));
            ticketTable.Columns.Add("PrintNumber1", typeof(string));
            ticketTable.Columns.Add("PrintNumber2", typeof(string));
            ticketTable.Columns.Add("PrintNumber3", typeof(string));
            ticketTable.Columns.Add("BarCode", typeof(string));
            ticketTable.Columns.Add("PrintOdd", typeof(string));
            ticketTable.Columns.Add("PrintUnOdd", typeof(string));
            ticketTable.Columns.Add("BonusStatus", typeof(int));
            ticketTable.Columns.Add("PreTaxBonusMoney", typeof(decimal));
            ticketTable.Columns.Add("AfterTaxBonusMoney", typeof(decimal));
            ticketTable.Columns.Add("PrintDateTime", typeof(DateTime));
            ticketTable.Columns.Add("Gateway", typeof(string));
            ticketTable.Columns.Add("CreateTime", typeof(DateTime));
            ticketTable.Columns.Add("IsAppend", typeof(bool));
            ticketTable.PrimaryKey = new System.Data.DataColumn[] { ticketTable.Columns["Id"] };
            return ticketTable;
        }

        public static int GetMaxTicketAmount(string gameCode)
        {
            switch (gameCode.ToLower())
            {
                case "ssq":
                case "fc3d":
                    return 50;
                default:
                    return 99;
            }
        }
        public static int GetMaxAntecodeCountEachTicket(string gameCode, string gameType)
        {
            switch (gameCode.ToLower())
            {
                case "ctzq":
                    return 1;
                case "ssq":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return 5;
                        default:
                            return 1;
                    }
                case "dlt":
                    switch (gameType.ToLower())
                    {
                        case "ds":
                            return 5;
                        default:
                            return 1;
                    }
                default:
                    return 1;
            }
        }

        //验证 不支持的玩法
        public static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> codeList, List<Cache_JCZQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 0：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        public static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> codeList, List<Cache_JCLQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        public static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, List<Sports_AnteCodeInfo> codeList, List<Cache_BJDC_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> codeList, List<C_JCLQ_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> codeList, List<C_JCZQ_Match> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 10：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, List<Sports_AnteCodeInfo> codeList, List<C_BJDC_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckSportAnteCode(string gameCode, string gameType, string[] anteCode)
        {
            var allowCodeList = new string[] { };
            switch (gameCode)
            {
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负
                        case "BRQSPF": //不让球胜平负
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,50,51,52,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,05,15,25,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":     // 胜负
                        case "RFSF":     // 让分胜负
                        case "DXF":     // 大小分
                            allowCodeList = "3,0".Split(',');
                            break;
                        case "SFC":      // 胜分差
                            allowCodeList = "01,02,03,04,05,06,11,12,13,14,15,16".Split(',');
                            break;
                    }
                    break;
                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负 
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "SXDS":    // 上下单双。上单 3；上双 2；下单 1；下双 0；
                            allowCodeList = "SD,SS,XD,XS".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                        case "SF":
                            allowCodeList = "3,0".Split(',');
                            break;
                    }
                    break;
                default:
                    break;
            }
            foreach (var item in anteCode)
            {
                if (!allowCodeList.Contains(item))
                    throw new Exception(string.Format("投注号码{0}为非法字符", item));
            }
        }

        // 检查订单基本信息
        public static void CheckSchemeOrder(Sports_BetingInfo info)
        {
            if (info.AnteCodeList.Count == 0)
                throw new ArgumentException("未选择任何比赛或者投注号码");
            if (info.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.GameType != null && info.GameType.ToUpper() != "HH")
            {
                if (info.AnteCodeList != null)
                {
                    foreach (var item in info.AnteCodeList)
                    {
                        if (item.GameType != null)
                        {
                            if (item.GameType.ToUpper() != info.GameType.ToUpper())
                                throw new Exception("彩种玩法有误，应该是:" + FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + FormatGameType(info.GameCode, item.GameType));
                        }
                    }
                }
            }
        }
        /// 解析玩法为中文名称
        /// </summary>
        public static string FormatGameType(string gameCode, string gameType)
        {
            var nameList = new List<string>();
            var typeList = gameType.Split(',', '|');
            foreach (var t in typeList)
            {
                nameList.Add(FormatGameType_Each(gameCode, t));
            }
            return string.Join(",", nameList.ToArray());
        }

        public static string FormatGameType_Each(string gameCode, string gameType)
        {
            switch (gameCode)
            {
                #region 足彩

                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":
                            return "胜平负";
                        case "ZJQ":
                            return "总进球";
                        case "SXDS":
                            return "上下单双";
                        case "BF":
                            return "比分";
                        case "BQC":
                            return "半全场";
                    }
                    break;
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":
                            return "让球胜平负";
                        case "BRQSPF":
                            return "胜平负";
                        case "BF":
                            return "比分";
                        case "ZJQ":
                            return "总进球";
                        case "BQC":
                            return "半全场";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":
                            return "胜负";
                        case "RFSF":
                            return "让分胜负";
                        case "SFC":
                            return "胜分差";
                        case "DXF":
                            return "大小分";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            return "胜负14场";
                        case "TR9":
                            return "任选9";
                        case "T6BQC":
                            return "6场半全场";
                        case "T4CJQ":
                            return "4场进球";
                    }
                    break;

                #endregion

                #region 重庆时时彩

                case "CQSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "3XHZ":    // 三星和值
                            return "三星和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XBAODAN":   // 二星组选包胆
                            return "二星组选包胆";
                        case "3XBAODAN":   // 三星组选包胆
                            return "三星组选包胆";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "3XBAODIAN":   // 三星组选包点
                            return "三星组选包点";
                        case "2XZXFS":   // 二星组选复式
                            return "二星组选复式";
                        case "2XZXFW":   // 二星组选分位
                            return "二星组选分位";
                        case "3XZXZH":   // 三星直选组合
                            return "三星直选组合";
                    }
                    break;

                #endregion

                #region 江西时时彩

                case "JXSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "4XDX":
                            return "四星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "2XZX":   // 二星组选
                            return "二星组选";
                        case "RX1":   // 任选一
                            return "任选一";
                        case "RX2":   // 任选二
                            return "任选二";
                    }
                    break;

                #endregion

                #region 山东十一选五、广东十一选五、江西十一选五

                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                    switch (gameType)
                    {
                        case "RX1":
                            return "前一直选";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "RX7":
                            return "任选七";
                        case "RX8":
                            return "任选八";
                        case "Q2ZHIX":
                            return "前二直选";
                        case "Q3ZHIX":
                            return "前三直选";
                        case "Q2ZUX":
                            return "前二组选";
                        case "Q3ZUX":
                            return "前三组选";
                    }
                    break;

                #endregion

                #region 广东快乐十分

                case "GDKLSF":
                    switch (gameType)
                    {
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "X1HT":
                            return "选一红投";
                        case "X1ST":
                            return "选一数投";
                        case "X2LZHI":
                            return "选二连直";
                        case "X2LZU":
                            return "选二连组";
                        case "X3QZHI":
                            return "选三连直";
                        case "X3QZU":
                            return "选三连组";
                    }
                    break;

                #endregion

                #region 江苏快三

                case "JSKS":
                    switch (gameType)
                    {
                        case "2BTH":
                            return "二不同号";
                        case "2BTHDT":
                            return "二不同号单选";
                        case "2THDX":
                            return "二同号单选";
                        case "2THFX":
                            return "二同号复选";
                        case "3BTH":
                            return "三不同号";
                        case "3BTHDT":
                            return "三不同号单选";
                        case "3LHTX":
                            return "三连号通选";
                        case "3THDX":
                            return "三同号单选";
                        case "3THTX":
                            return "三同号通选";
                        case "HZ":
                            return "和值";
                    }
                    break;

                #endregion

                #region 山东快乐扑克3

                case "SDKLPK3":
                    switch (gameType)
                    {
                        case "BZ":
                            return "豹子";
                        case "DZ":
                            return "对子";
                        case "RX1":
                            return "任选一";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "SZ":
                            return "顺子";
                        case "TH":
                            return "同花";
                        case "THS":
                            return "同花顺";
                    }
                    break;

                #endregion


                #region 福彩3D、排列三

                case "FC3D":
                case "PL3":
                    switch (gameType)
                    {
                        case "FS":
                            return "复式";
                        case "HZ":
                            return "和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                    }
                    break;

                #endregion

                #region 双色球

                case "SSQ":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                    }
                    break;

                #endregion

                #region 大乐透

                case "DLT":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                        case "12X2DS":
                            return "12生肖";
                        case "12X2FS":
                            return "12生肖";
                    }
                    break;


                #endregion

                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
            }
            return gameType;
        }


        /// <summary>
        /// 解析彩种为中文名称
        /// </summary>
        public static string FormatGameCode(string gameCode)
        {
            switch (gameCode)
            {
                case "BJDC":
                    return "北京单场";
                case "JCZQ":
                    return "竞彩足球";
                case "JCLQ":
                    return "竞彩篮球";
                case "CTZQ":
                    return "传统足球";
                case "SSQ":
                    return "双色球";
                case "DLT":
                    return "大乐透";
                case "FC3D":
                    return "福彩3D";
                case "PL3":
                    return "排列3";
                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
                case "CQSSC":
                    return "重庆时时彩";
                case "JX11X5":
                    return "江西11选五";
                case "SD11X5":
                    return "山东11选5";
                case "GD11X5":
                    return "广东11选5";
                case "GDKLSF":
                    return "广东快乐十分";
                case "JSKS":
                    return "江苏快三";
                case "SDKLPK3":
                    return "山东快乐扑克3";

            }
            return gameCode;
        }


        public static Dictionary<string, string> _stopTime = new Dictionary<string, string>();
        /// <summary>
        /// 查询指定彩是否可以出票
        /// </summary>
        /// <param name="AppSettings">配置文件</param>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public static bool CanRequestBet(string gameCode)
        {
            try
            {
                //
                var key = string.Format("{0}_{1}", gameCode.ToUpper(), "StopTicketing");
                string value = ConfigHelper.AllConfigInfo[key].ToString();// "";  // ConfigurationManager.AppSettings[key];
                                                                          // var key = string.Format("{0}_{1}", gameCode.ToUpper(), "StopTicketing");
                                                                          // string value = AppSettings; //DBbase.GlobalConfig[key].ToString();
                if (!_stopTime.ContainsKey(key))
                {
                    _stopTime.Add(key, value);
                }
                var stopTime = _stopTime[key];
                if (string.IsNullOrEmpty(stopTime))
                    return true;

                var szArray = new string[] { "SSQ", "DLT", "FC3D", "PL3" };
                if (szArray.Contains(gameCode))
                {
                    var szTimeArray = stopTime.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    var szStartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + szTimeArray[0]);
                    var szEndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + szTimeArray[1]);

                    if (DateTime.Now > szStartTime && DateTime.Now < szEndTime)
                    {
                        if (gameCode == "FC3D" || gameCode == "PL3")
                        {
                            return false;
                        }
                        var szDayIndex = (int)DateTime.Now.DayOfWeek;
                        switch (szDayIndex)
                        {
                            case 0:
                                return gameCode != "SSQ";
                            case 1:
                                return gameCode != "DLT";
                            case 2:
                                return gameCode != "SSQ";
                            case 3:
                                return gameCode != "DLT";
                            case 4:
                                return gameCode != "SSQ";
                            case 5:
                                break;
                            case 6:
                                return gameCode != "DLT";
                            default:
                                break;
                        }
                    }
                    return true;
                }

                var array = stopTime.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 7)
                    return true;
                var dayIndex = (int)DateTime.Now.DayOfWeek;
                var timeArray = array[dayIndex].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                var gameCodeArrary = new string[] { "JCZQ", "JCLQ", "OZB", "SJB" };
                var ozbTime = new string[] { "160611", "160612", "160613", "160614"
                                        , "160615", "160616", "160617", "160618", "160619", "160620" , "160621", "160622"
                                        , "160623", "160625", "160626", "160627", "160628", "160701", "160702", "160703",
                                        "160704", "160707", "160708", "160711"};
                var sjbTime = new string[] { "180614", "180615", "180616", "180617", "180618", "180619", "180620", "180621",
                                            "180621","180622","180623","180624","180625","180626","180627","180628","180629",
                                            "180630","180701","180702","180703","180704","180705","180706","180707","180708",
                                            "180709","180710","180711","180712","180713","180714","180715"};
                var date = DateTime.Now.ToString("yyyyMMdd").Substring(2);
                var isOzb = false;
                var isSJB = false;
                if (sjbTime.Contains(date))
                    isSJB = true;
                if (ozbTime.Contains(date))
                    isOzb = true;
                if (gameCodeArrary.Contains(gameCode) && isOzb)
                {

                    var ssTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "03:00");
                    var eTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "09:00");
                    if (ssTime > eTime)
                        eTime = eTime.AddDays(1);

                    if (DateTime.Now > ssTime && DateTime.Now < eTime)
                    {
                        return false;
                    }
                    return true;
                }
                if (gameCodeArrary.Contains(gameCode) && isSJB)
                {

                    var ssTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "03:00");
                    var eTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "09:00");
                    if (ssTime > eTime)
                        eTime = eTime.AddDays(1);

                    if (DateTime.Now > ssTime && DateTime.Now < eTime)
                    {
                        return false;
                    }
                    return true;
                }
                var startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + timeArray[0]);
                var endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + timeArray[1]);

                if (startTime > endTime)
                    endTime = endTime.AddDays(1);

                if (DateTime.Now > startTime && DateTime.Now < endTime)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static string GetSportsBettingSchemeId(string gameCode)
        {
            string prefix = gameCode;
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 检查串关方式
        /// </summary>
        public static void CheckPlayType(string gameCode, List<string> gameTypeList, int m)
        {
            if (gameCode.ToUpper() != "JCZQ" && gameCode.ToUpper() != "JCLQ")
                return;
            //竞彩足球 BF、BQC 最高串关4，ZJQ最高串关6
            //竞彩篮球 SFC 最高串关4
            if (gameTypeList.Contains("BF") && m > 4)
                throw new LogicException("比分过关方式最大为4串");
            if (gameTypeList.Contains("BQC") && m > 4)
                throw new LogicException("比分过关方式最大为4串");
            if (gameTypeList.Contains("ZJQ") && m > 6)
                throw new LogicException("总进球过关方式最大为6串");
            if (gameTypeList.Contains("SFC") && m > 4)
                throw new LogicException("胜分差过关方式最大为4串");
        }


        public static void CheckGameCodeAndType(string gameCode, string gameType)
        {
            if (gameCode.ToUpper() == "BJDC")
            {
                if (new string[] { "ZJQ", "SXDS", "BQC", "BF" }.Contains(gameType.ToUpper()))
                    throw new Exception("该玩法暂停销售");
            }
        }
        public static void CheckUserRealName(string idCardNumber)
        {
            if (string.IsNullOrEmpty(idCardNumber))
                throw new LogicException("用户身份证信息不完整，不能购买彩票");
            if (idCardNumber.Length < 18)
            {
                if (idCardNumber.Length != 15)
                    throw new LogicException("用户身份证号格式不正确，不能购买彩票");
            }
            else if (idCardNumber.Length > 15)
            {
                if (idCardNumber.Length != 18)
                    throw new LogicException("用户身份证号格式不正确，不能购买彩票");
            }

            var birth = string.Empty;
            int year = 0;
            int month = 0;
            int day = 0;
            if (idCardNumber.Length == 18)
                birth = idCardNumber.Substring(6, 8);
            if (idCardNumber.Length == 15)
                birth = string.Format("19{0}", idCardNumber.Substring(6, 6));
            if (birth.Length != 8)
                throw new LogicException("用户身份证号格式不正确，不能购买彩票");

            year = int.Parse(birth.Substring(0, 4));
            month = int.Parse(birth.Substring(4, 2));
            day = int.Parse(birth.Substring(6, 2));

            var diffYear = DateTime.Now.Year - year;
            if (diffYear > 18)
                return;
            if (diffYear < 18)
                throw new LogicException("用户未满18岁，不能购买彩票");
            if (diffYear == 18)
            {
                if (DateTime.Now.Month < month)
                    throw new LogicException("用户未满18岁，不能购买彩票");
                else if (DateTime.Now.Month == month)
                {
                    if (DateTime.Now.Day < day)
                        throw new LogicException("用户未满18岁，不能购买彩票");
                }
            }
        }

        public static bool CheckIDCard18(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }

        static string ResUrl = string.Empty;
        public BettingHelper()
        {
            ResUrl = ConfigHelper.AllConfigInfo["ResSiteUrl"].ToString();
        }

        public List<T> GetMatchInfoList<T>(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (result == null || string.IsNullOrEmpty(result))
                return new List<T>();
            return JsonHelper.Deserialize<List<T>>(result);
        }

        private static string ReadFileString(string fullUrl)
        {
            try
            {
                string strResult = PostManager.Get(fullUrl, Encoding.UTF8);
                if (strResult == "404") return string.Empty;

                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToLower().StartsWith("var"))
                    {
                        string[] strArray = strResult.Split('=');
                        if (strArray != null && strArray.Length == 2)
                        {
                            if (strArray[1].ToString().Trim().EndsWith(";"))
                            {
                                return strArray[1].ToString().Trim().TrimEnd(';');
                            }
                            return strArray[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取星期
        /// </summary>
        /// <returns></returns>
        public static string Week()
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            return week;
        }

        public static string GetWeek(DateTime now)
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(now.DayOfWeek)];
            return week;
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <returns></returns>
        public static string GetLeagueColor()
        {
            string[] colors = { "#385994", "#5b9999", "#67b9cb", "#ddab4a", "#ebeab4", "#5ea673", "#806362", "#a0a065", "#656598", "#562d81", "#484817", "#dd0000", "#577fb5", "#647897", "#396842" };
            Random random = new Random();
            int i = random.Next(0, 14);
            return colors[i];
        }

        /// <summary>
        /// 传统足球详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issuseId"></param>
        /// <returns></returns>
        public static List<Web_CTZQ_BonusPoolInfo> GetPoolInfo_CTZQ(string type, string issuseId)
        {
            var poolInfo = GetCTZQBonusPool(IssuseFile(type, issuseId));
          //  var poolInfo = GetCTZQBonusPool(IssuseFile(type, issuseId));

#if MGDB

#else
#endif

            return poolInfo;
        }

        /// <summary>
        /// 传统足球奖期详情
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Web_CTZQ_BonusPoolInfo> GetCTZQBonusPool(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (string.IsNullOrEmpty(result))
                return new List<Web_CTZQ_BonusPoolInfo>();
            return JsonHelper.Deserialize<List<Web_CTZQ_BonusPoolInfo>>(result);
        }

#region 文件路径
        /// <summary>
        /// 奖期数据文件
        /// </summary>
        private static string IssuseFile(string type, string issuseId)
        {
            if (type.StartsWith("CTZQ"))
            {
                var strs = type.Split('_');
                var gameCode = strs[0];
                //var gameType = strs[1];
                return string.Format("/matchdata/{0}/{1}/{2}_BonusLevel.json?_={3}", gameCode, issuseId, type, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            return string.Format("/matchdata/{0}/{0}_{1}.json?_={2}", type, issuseId, DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private static IList<CTZQ_BonusLevelInfo> IssuseCTZQ_Mg(string type, string issuseId)
        {
           
            if (type.StartsWith("CTZQ"))
            {
                var strs = type.Split('_');
                var gameCode = strs[0];

                var filter_BQC = Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b=>b.GameType, type) &
                    Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.IssuseNumber, issuseId);
                return MgHelper.MgDB.GetCollection<CTZQ_BonusLevelInfo>("CTZQ_BonusLevelInfo").Find<CTZQ_BonusLevelInfo>(filter_BQC).ToList();

                //var gameType = strs[1];
                //return string.Format("/matchdata/{0}/{1}/{2}_BonusLevel.json?_={3}", gameCode, issuseId, type, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            return null;
           // return string.Format("/matchdata/{0}/{0}_{1}.json?_={2}", type, issuseId, DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

#endregion

#region 数字彩详情
        /// <summary>
        /// 数字彩详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issuseId"></param>
        /// <returns></returns>
        public static Web_SZC_BonusPoolInfo GetPoolInfo(string type, string issuseId)
        {
            var poolInfo = GetSZCBonusPool(IssuseFile(type, issuseId));
            return poolInfo;
        }

        public static Web_SZC_BonusPoolInfo GetSZCBonusPool(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (string.IsNullOrEmpty(result))
                return new Web_SZC_BonusPoolInfo();
            return JsonHelper.Deserialize<Web_SZC_BonusPoolInfo>(result);
        }
#endregion

        public static string GetResult(int homeTeamScore, int guestTeamScore)
        {
            string flag = "[{0},{1},{2}-{3}]";
            if (homeTeamScore == guestTeamScore)
            {
                flag = string.Format(flag, "平", "2", homeTeamScore, guestTeamScore);
            }
            else if (homeTeamScore > guestTeamScore)
            {
                flag = string.Format(flag, "胜", "3", homeTeamScore, guestTeamScore);
            }
            else
            {
                flag = string.Format(flag, "负", "1", homeTeamScore, guestTeamScore);
            }
            return flag;
        }

        /// <summary>
        /// 合买投注方案编号
        /// </summary>
        public static string GetTogetherBettingSchemeId()
        {
            string prefix = "TSM";
            return prefix + UsefullHelper.UUID();
        }

        public static bool CheckAnteCode(string gameType, string anteCode)
        {
            if (string.IsNullOrEmpty(gameType))
                return false;
            switch (gameType.ToLower())
            {
                case "spf":
                case "brqspf":
                    if (!new string[] { "3", "1", "0" }.Contains(anteCode))
                        return false;
                    break;
                case "zjq":
                    if (!new string[] { "0", "1", "2", "3", "4", "5", "6", "7" }.Contains(anteCode))
                        return false;
                    break;
                case "bqc":
                    if (!new string[] { "33", "31", "30", "13", "11", "10", "03", "01", "00" }.Contains(anteCode))
                        return false;
                    break;
                case "bf":
                    if (!new string[] { "00", "01", "02", "03", "10", "11", "12", "13", "20", "21", "22", "23", "30", "31", "32", "33", "40", "41", "42", "04", "14", "24", "50", "51", "52", "05", "15", "25", "X0", "XX", "0X" }.Contains(anteCode))
                        return false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static void CheckHHPlayType(string gameCode, string gameType, string playType, List<Sports_AnteCodeInfo> anteCodeList)
        {
            if (anteCodeList != null && anteCodeList.Count > 0)
            {
                if (gameCode.ToLower() == "jczq" || gameCode.ToLower() == "jclq")
                {
                    var bfCount = 0;
                    var bqcCount = 0;
                    var sfcCount = 0;
                    var zjqCount = 0;
                    foreach (var itemCode in anteCodeList)
                    {
                        if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf" || gameType.ToLower() == "zjq"))
                        {
                            if (gameCode.ToLower() == "jczq")
                            {
                                if (itemCode.GameType.ToLower() == "bf")
                                {
                                    bfCount++;
                                }
                                else if (itemCode.GameType.ToLower() == "bqc")
                                {
                                    bqcCount++;
                                }
                                else if (itemCode.GameType.ToLower() == "zjq")
                                {
                                    zjqCount++;
                                }
                            }
                            else if (gameCode.ToLower() == "jclq")
                            {
                                if (itemCode.GameType.ToLower() == "sfc")
                                    sfcCount++;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(playType))
                    {
                        var tempMatchCount = playType.Split('|').Select(s => s.Split('_')[0]).Where(s => int.Parse(s) > 4).Count();
                        if (bfCount > 0 || bqcCount > 0)
                        {
                            if (tempMatchCount > 0)
                                throw new Exception("竞彩足球-包含比分或半全场玩法投注，串关方式最多不能超过四串!");
                        }
                        else if (sfcCount > 0)
                        {
                            if (tempMatchCount > 0)
                                throw new Exception("竞彩篮球-包含胜分差玩法投注，串关方式最多不能超过四串!");
                        }
                        else if (zjqCount > 0)
                        {
                            var tempMatchCount_zjq = playType.Split('|').Select(s => s.Split('_')[0]).Where(s => int.Parse(s) > 6).Count();
                            if (tempMatchCount_zjq > 0)
                                throw new Exception("竞彩足球-包含总进球玩法的投注，串关方式最多不能超过六串!");
                        }

                    }
                }
            }
        }

        public static List<string> BackToDetailByAnteCode(string GameCode, string GameType, string AnteCodes, string CurrentSp)
        {
            if (GameCode == "JCZQ")
            {
                switch (GameType)
                {
                    case "BRQSPF":
                        return JCZQ_BRQSPF_Detail(AnteCodes, CurrentSp);
                    case "SPF":
                        return JCZQ_SPF_Detail(AnteCodes, CurrentSp);
                    case "BF":
                        return JCZQ_BF_Detail(AnteCodes, CurrentSp);
                    case "ZJQ":
                        return JCZQ_ZJQ_Detail(AnteCodes, CurrentSp);
                    case "BQC":
                        return JCZQ_BQC_Detail(AnteCodes, CurrentSp);
                }
            }
            else if (GameCode == "JCLQ")
            {
                switch (GameType)
                {
                    case "SF":
                        return JCLQ_SF_Detail(AnteCodes, CurrentSp);
                    case "RFSF":
                        return JCLQ_RFSF_Detail(AnteCodes, CurrentSp);
                    case "DXF":
                        return JCLQ_DXF_Detail(AnteCodes, CurrentSp);
                    case "SFC":
                        return JCLQ_SFC_Detail(AnteCodes, CurrentSp);
                }
            }
            return new List<string>();
        }

        private static List<string> JCZQ_BRQSPF_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "3")
                {
                    var str = "主胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "1")
                {
                    var str = "平";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "0")
                {
                    var str = "客胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }


        private static List<string> JCZQ_SPF_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "3")
                {
                    var str = "主让胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "1")
                {
                    var str = "让平";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "0")
                {
                    var str = "客让胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }


        private static List<string> JCZQ_BF_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "X0")
                {
                    var str = "胜其他";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "XX")
                {
                    var str = "平其他";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "0X")
                {
                    var str = "负其他";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else
                {
                    var str = item[0] + ":" + item[1];
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }

        private static List<string> JCZQ_ZJQ_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "7")
                {
                    var str = item + "球+";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else
                {
                    var str = item + "球";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }

        private static List<string> JCZQ_BQC_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                var str1 = item[0] == '3' ? "胜" : item[0] == '1' ? "平" : "负";
                var str2 = item[1] == '3' ? "胜" : item[1] == '1' ? "平" : "负";
                var str = str1 + "-" + str2;
                if (CurrentSpList.ContainsKey(item))
                {
                    str += "(" + CurrentSpList[item] + ")";
                }
                result.Add(str);
            }
            return result;
        }


        private static List<string> JCLQ_SF_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "3")
                {
                    var str = "主胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "0")
                {
                    var str = "客胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }


        private static List<string> JCLQ_RFSF_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "3")
                {
                    var str = "主让胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "0")
                {
                    var str = "客让胜";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }

        private static List<string> JCLQ_DXF_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            foreach (var item in AnteCodeList)
            {
                if (item == "3")
                {
                    var str = "大分";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
                else if (item == "0")
                {
                    var str = "小分";
                    if (CurrentSpList.ContainsKey(item))
                    {
                        str += "(" + CurrentSpList[item] + ")";
                    }
                    result.Add(str);
                }
            }
            return result;
        }

        private static List<string> JCLQ_SFC_Detail(string AnteCodes, string CurrentSp)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(AnteCodes)) return result;
            var AnteCodeList = AnteCodes.Split(',');
            var CurrentSpList = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(CurrentSp))
            {
                var tempsp = CurrentSp.Split(',');
                foreach (var spitem in tempsp)
                {
                    var tempspitem = spitem.Split('|');
                    CurrentSpList.Add(tempspitem[0], tempspitem[1]);
                }
            }
            var SFCdic = new Dictionary<string, string>();
            SFCdic.Add("1", "1-5");
            SFCdic.Add("2", "6-10");
            SFCdic.Add("3", "11-15");
            SFCdic.Add("4", "16-20");
            SFCdic.Add("5", "21-25");
            SFCdic.Add("6", "26+");
            foreach (var item in AnteCodeList)
            {
                var str1 = item[0] == '1' ? "客胜" : "主胜";
                var str2 = "";
                if (SFCdic.ContainsKey(item[1].ToString()))
                {
                    str2 = SFCdic[item[1].ToString()];
                }
                var str = str1 + str2;
                if (CurrentSpList.ContainsKey(item))
                {
                    str += "(" + CurrentSpList[item] + ")";
                }
                result.Add(str);

            }
            return result;
        }

        public static string GetTeamName(string TeamName)
        {
            if (string.IsNullOrEmpty(TeamName)) return TeamName;
            TeamName = TeamName.Replace("&nbsp;", "");
            if(TeamName.Length<=4) return TeamName;
            return TeamName.Substring(0, 4);
        }


        public static string GetGameTypeDisplayName(string GameTypeName)
        {
          return  GameTypeName==null?"": GameTypeName.Replace("任选一", "前一直选")
    .Replace("胜负任9", "任选9")
    .Replace("14场胜负", "胜负14场")
    .Replace("6场半全场", "六场半全场")
    .Replace("4场进球", "四场进球彩");
        }
        /// <summary>
        /// 检查sql条件中特殊字符
        /// </summary>
        /// <param name="sqlCondition"></param>
        /// <returns></returns>
        public static bool CheckSQLCondition(string sqlCondition)
        {
            var charList = new List<string>();

#region 特殊字符

            charList.Add("'");
            charList.Add("\"");
            charList.Add("&");
            charList.Add("<");
            charList.Add(">");
            charList.Add("delete");
            charList.Add("update");
            charList.Add("insert");
            charList.Add("'");
            charList.Add(";");
            charList.Add("(");
            charList.Add(")");
            charList.Add("Exec");
            charList.Add("Execute");
            charList.Add("xp_");
            charList.Add("sp_");
            charList.Add("0x");
            charList.Add("?");
            charList.Add("<");
            charList.Add(">");
            charList.Add("(");
            charList.Add(")");
            charList.Add("@");
            charList.Add("=");
            charList.Add("+");
            charList.Add("*");
            charList.Add("&");
            charList.Add("#");
            charList.Add("%");
            charList.Add("$");
            charList.Add("and");
            charList.Add("net user");
            charList.Add("or");
            charList.Add("net");
            charList.Add("drop");
            charList.Add("script");
            charList.Add(";");
            charList.Add("*/");
            charList.Add("\r\n");

#endregion

            if (charList.Contains(sqlCondition))
                return true;
            return false;
        }

        /// <summary>
        /// 日期当天是否有奖期数据 （春节无奖期）
        /// </summary>
        public static bool CheckIsOpenDay(DateTime date)
        {
            var calendar = new ChineseLunisolarCalendar();
            var lMonth = calendar.GetMonth(date);
            var lDay = calendar.GetDayOfMonth(date);
            if (lMonth == 1 && lDay < 7)
            {
                return false;
            }
            else
            {
                date = date.AddDays(1);
                lMonth = calendar.GetMonth(date);
                lDay = calendar.GetDayOfMonth(date);
                if (lMonth == 1 && lDay == 1)
                {
                    return false;
                }
            }
            return true;
        }

        public static string BuildLastIssuseNumber(string gameName, string currentIssuseNumber)
        {
            int maxIssuseCount = MaxIssuseCount(gameName);
            string[] issuseNumberArray = currentIssuseNumber.Split('-');
            int index = int.Parse(issuseNumberArray[1]);
            DateTime dtPart = DateTime.Now;
            switch (gameName.ToUpper())
            {
                case "CQSSC":
                case "JXSSC":
                case "JX11X5":
                case "SD11X5":
                case "GD11X5":
                case "SDQYH":
                case "GDKLSF":
                case "JSKS":
                    dtPart = DateTime.ParseExact(issuseNumberArray[0], "yyyyMMdd", null);
                    if (index == 1)
                    {
                        dtPart = dtPart.AddDays(-1);
                        index = maxIssuseCount;
                    }
                    else
                    {
                        index = index - 1;
                    }

                    //屏蔽春节7天奖期数据
                    if (!CheckIsOpenDay(dtPart))
                    {
                        dtPart = dtPart.AddDays(-1);
                    }
                    //while (dtPart >= minDate && dtPart <= maxDate)
                    //{
                    //    dtPart = dtPart.AddDays(-1);
                    //}

                    return string.Format("{0}-{1}", dtPart.ToString("yyyyMMdd"), index.ToString().PadLeft(issuseNumberArray[1].Length, '0'));
                case "GXKLSF":
                    int fech = int.Parse(issuseNumberArray[0]);
                    if (index == 1)
                    {
                        fech = fech - 1;
                        index = maxIssuseCount;
                    }
                    else
                    {
                        index = index - 1;
                    }
                    return string.Format("{0}-{1}", fech.ToString(), index.ToString().PadLeft(issuseNumberArray[1].Length, '0'));
                case "FC3D":
                case "PL3":
                case "SSQ":
                case "DLT":
                    dtPart = DateTime.ParseExact(issuseNumberArray[0], "yyyy", null);
                    if (index == 1)
                    {
                        dtPart = dtPart.AddYears(-1);
                        index = maxIssuseCount;
                    }
                    else
                    {
                        index = index - 1;
                    }
                    return string.Format("{0}-{1}", dtPart.ToString("yyyy"), index.ToString().PadLeft(issuseNumberArray[1].Length, '0'));
                default:
                    return currentIssuseNumber;
            }
        }

        public static int MaxIssuseCount(string gameCode)
        {
            switch (gameCode.ToUpper())
            {
                case "CQSSC":
                    return 120;
                case "JXSSC":
                    return 84;
                case "JX11X5":
                    return 84;
                case "SD11X5":
                    return 87;
                case "GD11X5":
                    return 84;
                case "FC3D":
                case "PL3":
                case "PL5":
                    return 358;
                case "SDQYH":
                    return 40;
                case "GDKLSF":
                    return 84;
                case "GXKLSF":
                    return 50;
                case "SSQ":
                case "DLT":
                    return 156;
                case "JSKS":
                    return 82;
                case "CTZQ":
                    return 50;
                case "SDKLPK3":
                    return 88;
                default:
                    return 0;
            }
        }
    }
}
