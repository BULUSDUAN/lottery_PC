using EntityModel.CoreModel.BetingEntities;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Text;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.UserHelper;
using EntityModel.ExceptionExtend;
using System.Diagnostics;
using EntityModel.Enum;
using System.Linq;
using KaSon.FrameWork.Common.Redis;
using EntityModel.Redis;
using EntityModel.Ticket;
using System.Threading;
using EntityModel.GameBiz.Core;
using System.IO;
using System.Data;
using EntityModel.CoreModel;
using KaSon.FrameWork.ORM.Helper.AnalyzerFactory;

namespace KaSon.FrameWork.ORM.Helper.BusinessLib
{
    public class Sports_Business: DBbase
    {
        private static Log4Log writerLog = new Log4Log();
        private void CheckSportAnteCode(string gameCode, string gameType, string[] anteCode)
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
        public int CheckBettingOrderMoney(Sports_AnteCodeInfoCollection codeList, string gameCode, string gameType, string playType, int amount, decimal schemeTotalMoney, DateTime stopTime, bool isAllow = false, string userId = "")
        {
            //验证投注号码
            if (stopTime < DateTime.Now)
                throw new Exception("投注结束时间不能小于当前时间");

            //验证投注内容是否合法，或是否重复
            foreach (var item in codeList)
            {
                var oneCodeArray = item.AnteCode.Split(',');
                if (oneCodeArray.Distinct().Count() != oneCodeArray.Length)
                    throw new Exception(string.Format("投注号码{0}中包括重复的内容", item.AnteCode));
                CheckSportAnteCode(gameCode, string.IsNullOrEmpty(item.GameType) ? gameType : item.GameType.ToUpper(), oneCodeArray);
            }

            var tmp = playType.Split('|');
            var totalMoney = 0M;
            var totalCount = 0;

            var c = new Combination();
            foreach (var chuan in tmp)
            {
                var chuanArray = chuan.Split('_');
                if (chuanArray.Length != 2) continue;

                var m = int.Parse(chuanArray[0]);
                var n = int.Parse(chuanArray[1]);

                //串关包括的真实串数
                var countList = SportAnalyzer.AnalyzeChuan(m, n);
                if (n > 1)
                {
                    //3_3类型
                    c.Calculate(codeList.ToArray(), m, (arr) =>
                    {
                        //m场比赛
                        if (arr.Select(p => p.MatchId).Distinct().Count() == m)
                        {
                            foreach (var count in countList)
                            {
                                //M串1
                                c.Calculate(arr, count, (a1) =>
                                {
                                    var cCount = 1;
                                    foreach (var t in a1)
                                    {
                                        cCount *= t.AnteCode.Split(',').Length;
                                    }
                                    totalCount += cCount;
                                });
                            }
                        }

                    });
                }
                else
                {
                    var ac = new ArrayCombination();
                    var danList = codeList.Where(a => a.IsDan).ToList();
                    //var tuoList = codeList.Where(a => !a.IsDan).ToList();
                    var totalCodeList = new List<Sports_AnteCodeInfo[]>();
                    foreach (var g in codeList.GroupBy(p => p.MatchId))
                    {
                        totalCodeList.Add(codeList.Where(p => p.MatchId == g.Key).ToArray());
                    }
                    //3_1类型
                    foreach (var count in countList)
                    {
                        //c.Calculate(totalCodeList.ToArray(), count - danList.Count, (arr2) =>
                        c.Calculate(totalCodeList.ToArray(), count, (arr2) =>
                        {
                            ac.Calculate(arr2, (tuoArr) =>
                            {
                                #region 拆分组合票

                                var isContainsDan = true;
                                foreach (var dan in danList)
                                {
                                    var con = tuoArr.FirstOrDefault(p => p.MatchId == dan.MatchId);
                                    if (con == null)
                                    {
                                        isContainsDan = false;
                                        break;
                                    }
                                }

                                if (isContainsDan)
                                {
                                    var cCount = 1;
                                    foreach (var t in tuoArr)
                                    {
                                        cCount *= t.AnteCode.Split(',').Length;
                                    }
                                    totalCount += cCount;
                                    //if (!isAllow && BusinessHelper.IsSiteLimit(gameCode))
                                    //{
                                    //    if (!string.IsNullOrEmpty(userId) && !BusinessHelper.IsSpecificUser(userId))//如果是特定用户，则不限制投注
                                    //    {
                                    //        if ((cCount * amount * 2M) < 50)
                                    //            throw new Exception("您好，根据您投注的内容将产生多张彩票，每张彩票金额不足50元，请您增加倍数以达到出票条件。");
                                    //    }
                                    //}
                                }

                                #endregion
                            });
                        });
                    }
                }
            }
            totalMoney = totalCount * amount * 2M;

            if (totalMoney != schemeTotalMoney) throw new ArgumentException(string.Format("订单金额不正确，应该为：{0}；实际为：{1}。", totalMoney, schemeTotalMoney));
            return totalCount;
        }

        private void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<C_JCLQ_Match> matchList)
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
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        private void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<C_JCZQ_Match> matchList)
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
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        private void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<C_BJDC_Match> matchList)
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
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, BusinessHelper.FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public DateTime CheckGeneralBettingMatch(Sports_Manager sportsManager, string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, string issuseNumber, SchemeBettingCategory? bettingCategory = null)
        {
            if (gameCode == "BJDC")
            {
                var matchIdArray = (from l in codeList select string.Format("{0}|{1}", issuseNumber, l.MatchId)).Distinct().ToArray();
                var matchList = sportsManager.QueryBJDCSaleMatchCount(matchIdArray);
                if (gameType.ToUpper() == "SF")
                {
                    var SFGGMatchList = sportsManager.QuerySFGGSaleMatchCount(matchIdArray);

                    if (SFGGMatchList.Count != matchIdArray.Length)
                        throw new LogicException("所选比赛中有停止销售的比赛。");
                    CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                    return SFGGMatchList.Min(m => m.BetStopTime);
                }
                else
                {

                    if (matchList.Count != matchIdArray.Length)
                        throw new LogicException("所选比赛中有停止销售的比赛。");
                    CheckPrivilegesType_BJDC(gameCode, gameType, playType, issuseNumber, codeList, matchList);
                    return matchList.Min(m => m.LocalStopTime);
                }
                //if (matchList.Count != matchIdArray.Length && gameType != "HH")
                //    throw new ArgumentException("所选比赛中有停止销售的比赛。");
            }
            if (gameCode == "JCZQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = sportsManager.QueryJCZQSaleMatchCount(matchIdArray);
                //if (matchList.Count != matchIdArray.Length && gameType != "HH")
                //    throw new ArgumentException("所选比赛中有停止销售的比赛。");
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                //var matchResultList = sportsManager.QueryJCZQMatchResult(matchIdArray);
                //if (matchResultList.Count > 0)
                //    throw new ArgumentException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

                CheckPrivilegesType_JCZQ(gameCode, gameType, playType, codeList, matchList);

                //if (playType == "1_1")
                if (bettingCategory != null && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "JCLQ")
            {
                var matchIdArray = (from l in codeList select l.MatchId).Distinct().ToArray();
                var matchList = sportsManager.QueryJCLQSaleMatchCount(matchIdArray);
                //if (matchList.Count != matchIdArray.Length && gameType != "HH")
                //    throw new ArgumentException("所选比赛中有停止销售的比赛。");
                if (matchList.Count != matchIdArray.Length)
                    throw new LogicException("所选比赛中有停止销售的比赛。");
                var matchResultList = sportsManager.QueryJCLQMatchResult(matchIdArray);
                if (matchResultList.Count > 0)
                    throw new LogicException(string.Format("所选比赛中包含结束的比赛：{0}", string.Join(",", matchResultList.Select(p => p.MatchId).ToArray())));

                CheckPrivilegesType_JCLQ(gameCode, gameType, playType, codeList, matchList);

                //if (playType == "1_1")
                if (bettingCategory != null && bettingCategory.Value == SchemeBettingCategory.SingleBetting)
                    return matchList.Min(m => m.DSStopBettingTime);
                return matchList.Min(m => m.FSStopBettingTime);
            }
            if (gameCode == "CTZQ")
            {
                var issuse = new LotteryGameManager().QueryGameIssuseByKey(gameCode, gameType, issuseNumber);
                if (issuse == null)
                    throw new LogicException(string.Format("{0},{1}奖期{2}不存在", gameCode, gameType, issuseNumber));
                if (issuse.LocalStopTime < DateTime.Now)
                    throw new LogicException(string.Format("{0},{1}奖期{2}结束时间为{3}", gameCode, gameType, issuseNumber, issuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
                return issuse.LocalStopTime;
            }
            //其它数字彩
            var currentIssuse = new LotteryGameManager().QueryGameIssuseByKey(gameCode, gameCode == "CTZQ" ? gameType : string.Empty, issuseNumber);
            if (currentIssuse == null)
                throw new LogicException(string.Format("{0},{1}奖期{2}不存在", gameCode, gameType, issuseNumber));
            if (currentIssuse.LocalStopTime < DateTime.Now)
                throw new LogicException(string.Format("{0},{1}奖期{2}结束时间为{3}", gameCode, gameType, issuseNumber, currentIssuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm")));
            return currentIssuse.LocalStopTime;
        }

        /// <summary>
        /// 保存用户未投注订单
        /// </summary>
        public string SaveOrderSportsBetting(Sports_BetingInfo info, string userId)
        {
            string schemeId = string.Empty;
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            var gameCode = info.GameCode;
            schemeId = BusinessHelper.GetSportsBettingSchemeId(gameCode);
            var sportsManager = new Sports_Manager();
            //验证比赛是否还可以投注
            var stopTime = CheckGeneralBettingMatch(sportsManager, gameCode, info.GameType, info.PlayType, info.AnteCodeList, info.IssuseNumber);
            // 检查订单金额是否匹配
            var betCount = CheckBettingOrderMoney(info.AnteCodeList, gameCode, info.GameType, info.PlayType, info.Amount, info.TotalMoney, stopTime, false, userId);
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            //开启事务
            using (DB)
            {
                DB.Begin();
                AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime,
                    info.SchemeSource, info.Security, SchemeType.SaveScheme, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                foreach (var item in info.AnteCodeList)
                {
                    //sportsManager.AddSports_AnteCode(new Sports_AnteCode
                    //{
                    //    SchemeId = schemeId,
                    //    AnteCode = item.AnteCode,
                    //    BonusStatus = BonusStatus.Waitting,
                    //    CreateTime = DateTime.Now,
                    //    GameCode = gameCode,
                    //    GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                    //    IsDan = item.IsDan,
                    //    IssuseNumber = info.IssuseNumber,
                    //    MatchId = item.MatchId,
                    //    PlayType = info.PlayType,
                    //    Odds = string.Empty,
                    //});
                    var entity = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    };
                    DB.GetDal<C_Sports_AnteCode>().Add(entity);

                }
                //用户的订单保存
                //sportsManager.AddUserSaveOrder(new UserSaveOrder
                //{
                //    SchemeId = schemeId,
                //    UserId = userId,
                //    GameCode = info.GameCode,
                //    GameType = info.GameType,
                //    PlayType = info.PlayType,
                //    SchemeType = SchemeType.SaveScheme,
                //    SchemeSource = info.SchemeSource,
                //    SchemeBettingCategory = info.BettingCategory,
                //    ProgressStatus = ProgressStatus.Waitting,
                //    IssuseNumber = info.IssuseNumber,
                //    Amount = info.Amount,
                //    BetCount = betCount,
                //    TotalMoney = info.TotalMoney,
                //    StopTime = stopTime,
                //    CreateTime = DateTime.Now,
                //    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                //});
                var entity_U = new C_UserSaveOrder
                {
                    SchemeId = schemeId,
                    UserId = userId,
                    GameCode = info.GameCode,
                    GameType = info.GameType,
                    PlayType = info.PlayType,
                    SchemeType = (int)SchemeType.SaveScheme,
                    SchemeSource = (int)info.SchemeSource,
                    SchemeBettingCategory = (int)info.BettingCategory,
                    ProgressStatus = (int)ProgressStatus.Waitting,
                    IssuseNumber = info.IssuseNumber,
                    Amount = info.Amount,
                    BetCount = betCount,
                    TotalMoney = info.TotalMoney,
                    StopTime = stopTime,
                    CreateTime = DateTime.Now,
                    StrStopTime = stopTime.AddMinutes(-5).ToString("yyyyMMddHHmm"),
                };
                DB.Commit();
               // biz.CommitTran();
            }
            return schemeId;
        }

        public string ConvertGameType(string gameCode, string gameType, string betType, int betCount)
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

        public List<Antecode> FomartTR9AnteCodeList(GatewayAnteCode code)
        {
            var list = new List<Antecode>();
            //var tr9 = "3,1,0,31,11,10,30,*,*,*,*,*,1,1|0,1,2,3,4";
            var source = code.AnteNumber.Split('|');
            var array = source[0].Split(',');
            var danArray = source.Length == 1 ? new string[] { } : source[1].Split(',');

            //找出数字的索引
            var numberIndexList = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != "*")
                    numberIndexList.Add(i);
            }
            if (numberIndexList.Count == 9)
            {
                if (danArray.Length > 0)
                    throw new Exception("任9玩法选9场时不能设胆");
                list.Add(new Antecode
                {
                    AnteNumber = code.AnteNumber,
                    GameType = code.GameType,
                });
                return list;
            }

            //计算应该替换为*的数组索引的组合
            var replaceIndexList = new List<int[]>();
            new Common.Algorithms.Combination().Calculate(numberIndexList.ToArray(), numberIndexList.Count - 9, (p) =>
            {
                var containDan = false;
                if (danArray.Length > 0)
                {
                    foreach (var item in p)
                    {
                        if (danArray.Contains(item.ToString()))
                        {
                            containDan = true;
                            break;
                        }
                    }
                }
                if (!containDan)
                {
                    replaceIndexList.Add(p);
                    var t = string.Join(",", p);
                }
            });

            //按索引替换
            foreach (var indexArray in replaceIndexList)
            {
                var tempCode = source[0].Split(',');
                foreach (var index in indexArray)
                {
                    tempCode[index] = "*";
                }
                var replaceCode = string.Join(",", tempCode);
                list.Add(new Antecode
                {
                    AnteNumber = replaceCode,
                    GameType = code.GameType,
                });
            }

            return list;
        }
        private int GetMaxAntecodeCountEachTicket(string gameCode, string gameType)
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
        public int GetMaxTicketAmount(string gameCode)
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
        private System.Data.DataTable GetNewTicketTable()
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

        private TicketCollection AnalyzeTickets(Order order)
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
        private IEnumerable<Ticket> GetTicketsByAntecodes(IEnumerable<Antecode> antecodeList, int maxCount, int maxAmount, Func<Ticket> createTicketHandler)
        {
            var ticketList = new List<Ticket>();
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

        public TicketCollection AnalyzeOrder(GatewayTicketOrder order)
        {
            Order o = new Order()
            {
                GameCode = order.GameCode,
                Amount = order.Amount,
            };
            foreach (var a in order.AnteCodeList)
            {
                if (order.GameCode == "CTZQ" && a.GameType == "TR9")
                {
                    o.AntecodeList.AddRange(FomartTR9AnteCodeList(a));
                }
                else
                {
                    o.AntecodeList.Add(new Antecode()
                    {
                        AnteNumber = a.AnteNumber,
                        GameType = a.GameType,
                    });
                }
            }
            var ticketList = AnalyzeTickets(o);
            ticketList.AnalyzeOrder(order.GameCode, order.Price);
            if (ticketList.TotalMoney != order.TotalMoney)
            {
                throw new Exception(string.Format("订单金额与投注号码不匹配。应为￥{0:N}，实际为￥{1:N}。", ticketList.TotalMoney, order.TotalMoney));
            }
            return ticketList;
        }
        /// <summary>
        /// 拆票，数字彩、传统足球
        /// </summary>
        public void RequestTicket2(GatewayTicketOrder orderInfo, string keyLine, bool stopAfterBonus, SchemeType schemeType)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                var redisTicketList = new List<RedisTicketInfo>();
                var gameType = string.Empty;
                var ticketList = AnalyzeOrder(orderInfo);
                var ticketTable = GetNewTicketTable();
                var index = 1;
                var locOdds = string.Empty;
                foreach (var ticket in ticketList)
                {
                    var locBetContent = ticket.ToAnteString_Localhost();
                    if (orderInfo.GameCode == "OZB")
                    {
                        //欧洲杯特殊处理
                        var locOddsList = new List<string>();
                        var betArray = locBetContent.Split(',');
                        var ozbMatchList = new JCZQMatchManager().QueryJCZQ_OZBMatchList(ticket.GameType, betArray);
                        foreach (var c in betArray)
                        {
                            var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                            if (m == null) continue;
                            locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                        }
                        locOdds = string.Join("/", locOddsList);
                    }
                    if (orderInfo.GameCode == "SJB")
                    {
                        //欧洲杯特殊处理
                        var locOddsList = new List<string>();
                        var betArray = locBetContent.Split(',');
                        var ozbMatchList = new JCZQMatchManager().QueryJCZQ_SJBMatchList(ticket.GameType, betArray);
                        foreach (var c in betArray)
                        {
                            var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                            if (m == null) continue;
                            locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                        }
                        locOdds = string.Join("/", locOddsList);
                    }

                    var betType = ticket.BetCount <= 1 ? "DS" : "FS";
                    var info = new 
                    {
                        OrderId = orderInfo.OrderId + "|" + (index++).ToString("D3"),
                        BetType = ConvertGameType(orderInfo.GameCode, ticket.GameType, betType, ticket.BetCount),
                        IssueNumber = orderInfo.IssuseNumber,
                        BetUnits = ticket.BetCount,
                        Multiple = ticket.Amount,
                        BetMoney = ticket.TicketMoney,
                        IsAppend = orderInfo.IsAppend ? 1 : 0,
                        BetContent = locBetContent,
                    };

                    //sql.Append(CreateInsertSql(orderInfo, ticket, info, locOdds));
                    System.Data.DataRow r = ticketTable.NewRow();
                    r["Id"] = index;
                    r["GameCode"] = orderInfo.GameCode.ToUpper();
                    r["GameType"] = ticket.GameType.ToUpper();
                    r["SchemeId"] = orderInfo.OrderId;
                    r["TicketId"] = info.OrderId;
                    r["IssuseNumber"] = orderInfo.IssuseNumber;
                    r["PlayType"] = info.BetType;
                    r["BetContent"] = info.BetContent;
                    r["TicketStatus"] = 90;
                    r["Amount"] = info.Multiple;
                    r["BetUnits"] = info.BetUnits;
                    r["BetMoney"] = info.BetMoney;
                    r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                    r["IsAppend"] = orderInfo.IsAppend;
                    r["BonusStatus"] = 0;
                    r["PreTaxBonusMoney"] = 0M;
                    r["AfterTaxBonusMoney"] = 0M;
                    r["PrintDateTime"] = DateTime.Now;
                    r["CreateTime"] = DateTime.Now;
                    r["LocOdds"] = locOdds;

                    if (string.IsNullOrEmpty(gameType))
                        gameType = ticket.GameType.ToUpper();
                    redisTicketList.Add(new RedisTicketInfo
                    {
                        AfterBonusMoney = 0M,
                        Amount = info.Multiple,
                        BetContent = info.BetContent,
                        BetMoney = info.BetMoney,
                        BetUnits = ticket.BetCount,
                        BonusStatus = BonusStatus.Waitting,
                        GameCode = orderInfo.GameCode.ToUpper(),
                        GameType = ticket.GameType.ToUpper(),
                        IsAppend = orderInfo.IsAppend,
                        IssuseNumber = orderInfo.IssuseNumber,
                        LocOdds = locOdds,
                        MatchIdList = ticket.ToAnteString_zhongminToMatchId(),
                        PlayType = "",
                        PreBonusMoney = 0M,
                        SchemeId = orderInfo.OrderId,
                        TicketId = info.OrderId,
                    });
                    ticketTable.Rows.Add(r);
                }

                //var watch = new Stopwatch();
                //watch.Start();
                var manager = new Sports_Manager();
                try
                {
                    //批量插入票表
                    manager.SqlBulkAddTable(ticketTable);
                }
                catch (Exception exp)
                {

                    writerLog.WriteLog("SqlBulkAddTableError", orderInfo.OrderId, (int)LogType.Information, "SqlBulkAddTable", exp.Message + "/r/n");
                    return;
                }
                //manager.ExecSql(sql.ToString());
                //Common.Database.DbDapper.DapperHelper.ExecuteSql(sql.ToString());
                if (orderInfo.GameCode == "OZB")
                {
                    //更新号码表比赛SP
                    var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                    foreach (var item in codeList)
                    {
                        if (!string.IsNullOrEmpty(item.Odds))
                            continue;

                        item.Odds = locOdds;
                        manager.UpdateSports_AnteCode(item);
                    }
                }
                if (orderInfo.GameCode == "SJB")
                {
                    //更新号码表比赛SP
                    var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                    foreach (var item in codeList)
                    {
                        if (!string.IsNullOrEmpty(item.Odds))
                            continue;

                        item.Odds = locOdds;
                        manager.UpdateSports_AnteCode(item);
                    }
                }

                //调用Redis保存
                if (RedisHelper.EnableRedis)
                    RedisOrderBusiness.AddToRunningOrder_SZC(schemeType, orderInfo.GameCode.ToUpper(), gameType, orderInfo.OrderId, keyLine, stopAfterBonus, orderInfo.IssuseNumber, redisTicketList);

            }
            catch (Exception exp)
            {
                writerLog.WriteLog("RequestTicketError", orderInfo.OrderId, (int)LogType.Information, "保存票数据报错", exp.Message + "/r/n" + sql.ToString());
            }
            //watch.Stop();
            //this.writer.Write("RequestTicket", orderInfo.OrderId, LogType.Information, "保存票数据计时", "用时 " + watch.Elapsed.TotalMilliseconds);
        }

        //根据playType 1_1判断是单关  区DS下面
        public string GetOddsToMatchId_New(string matchId, string gameCode, string gameType)
        {
            var strList = new List<string>();
            if (gameCode.ToUpper() == "JCZQ")
            {
                var oddManager = new JCZQ_OddsManager();
                switch (gameType.ToUpper())
                {
                    case "SPF":
                        var entity_SPF = oddManager.GetLastOdds<T_JCZQ_Odds_SPF>(gameType.ToUpper(), matchId, false);
                        if (entity_SPF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SPF.GetOddsString()));
                        break;
                    case "BRQSPF":
                        var entity_BRQSPF = oddManager.GetLastOdds<T_JCZQ_Odds_BRQSPF>(gameType.ToUpper(), matchId, false);
                        if (entity_BRQSPF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BRQSPF.GetOddsString()));
                        break;
                    case "BF":
                        var entity_BF = oddManager.GetLastOdds<T_JCZQ_Odds_BF>(gameType.ToUpper(), matchId, false);
                        if (entity_BF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BF.GetOddsString()));
                        break;
                    case "ZJQ":
                        var entity_ZJQ = oddManager.GetLastOdds<T_JCZQ_Odds_ZJQ>(gameType.ToUpper(), matchId, false);
                        if (entity_ZJQ == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_ZJQ.GetOddsString()));
                        break;
                    case "BQC":
                        var entity_BQC = oddManager.GetLastOdds<T_JCZQ_Odds_BQC>(gameType.ToUpper(), matchId, false);
                        if (entity_BQC == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_BQC.GetOddsString()));
                        break;
                    default:
                        break;
                }
            }
            if (gameCode.ToUpper() == "JCLQ")
            {
                var oddManager = new JCLQ_OddsManager();
                switch (gameType.ToUpper())
                {
                    case "SF":
                        var entity_SF = oddManager.GetLastOdds<T_JCLQ_Odds_SF>(gameType.ToUpper(), matchId, false);
                        if (entity_SF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SF.GetOddsString()));
                        break;
                    case "RFSF":
                        var entity_RFSF = oddManager.GetLastOdds<T_JCLQ_Odds_RFSF>(gameType.ToUpper(), matchId, false);
                        if (entity_RFSF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_RFSF.GetOddsString()));
                        break;
                    case "SFC":
                        var entity_SFC = oddManager.GetLastOdds<T_JCLQ_Odds_SFC>(gameType.ToUpper(), matchId, false);
                        if (entity_SFC == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_SFC.GetOddsString()));
                        break;
                    case "DXF":
                        var entity_DXF = oddManager.GetLastOdds<T_JCLQ_Odds_DXF>(gameType.ToUpper(), matchId, false);
                        if (entity_DXF == null)
                            throw new Exception("订单中有比赛未开出赔率" + matchId);
                        strList.Add(string.Format("{0}_{1}", matchId, entity_DXF.GetOddsString()));
                        break;
                    default:
                        break;
                }
            }
            return string.Join("/", strList);
        }

        /// <summary>
        /// 拆票，北单、竞彩
        /// </summary>
        public void RequestTicket_Sport(GatewayTicketOrder_Sport orderInfo)
        {
            var matchIdArray = new string[] { };
            var matchIdOddsList = new Dictionary<string, string>();
            if (orderInfo.GameCode.ToUpper() == "JCZQ" || orderInfo.GameCode.ToUpper() == "JCLQ" || orderInfo.GameCode.ToUpper() == "BJDC")
            {
                matchIdArray = orderInfo.AnteCodeList.Select(p => p.MatchId).Distinct().ToArray();

                foreach (var item in orderInfo.AnteCodeList)
                {
                    matchIdOddsList.Add(string.Format("{0}_{1}", item.GameType.ToUpper(), item.MatchId), GetOddsToMatchId_New(item.MatchId, orderInfo.GameCode.ToUpper(), item.GameType.ToUpper()));
                }
            }

            //var order = string.IsNullOrEmpty(orderInfo.Attach) ?
            //    AnalyzeOrder_Sport<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(orderInfo, "LOCAL")
            //   : AnalyzeOrder_Sport_YH<Ticket_Order_Running, Ticket_Ticket_Running, Ticket_AnteCode_Running>(orderInfo, "LOCAL", orderInfo.Attach);


            //RequestTicket_Sport(order, orderInfo.IsRunningTicket, matchIdOddsList, matchIdArray);
        }
        /// <summary>
        /// 足彩普通投注
        /// </summary>
        public string SportsBetting(Sports_BetingInfo info, string userId, string password, string place, int totalCount, DateTime stopTime, decimal redBagMoney)
        {
            string schemeId = info.SchemeId;
            var anteCodeList = new List<EntityModel.C_Sports_AnteCode>();
            C_Sports_Order_Running runningOrder = null;
            var sportsManager = new Sports_Manager();
            var userManager = new UserBalanceManager();
            var user = userManager.LoadUserRegister(userId);
            if (!user.IsEnable)
                throw new LogicException("用户已禁用");

            if (redBagMoney > 0M)
            {
                var fundManager = new FundManager();
                var percent = 0M;
                //JCZQ单关配置了单独的红包比例
                if (info.PlayType == "1_1" && info.GameCode.ToUpper() == "JCZQ")
                {
                    percent = fundManager.QueryRedBagUseConfig("jczqdg");
                }
                else
                {
                    percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                }
                //var percent = fundManager.QueryRedBagUseConfig(info.GameCode);

                //var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                var maxUseMoney = info.TotalMoney * percent / 100;
                if (redBagMoney > maxUseMoney)
                    throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
            }

            //投注流程计时
            var watch = new Stopwatch();
            watch.Start();
            info.GameCode = info.GameCode.ToUpper();
            info.GameType = info.GameType.ToUpper();
            if (string.IsNullOrEmpty(schemeId))
                schemeId = BusinessHelper.GetSportsBettingSchemeId(info.GameCode);
            var canTicket = BusinessHelper.CanRequestBet(info.GameCode);
            //开启事务
            using ( DB)
            {
                
                runningOrder = AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true,
                     info.IssuseNumber, info.Amount, totalCount, info.TotalMatchCount, info.TotalMoney, stopTime, info.SchemeSource, info.Security,
                     SchemeType.GeneralBetting, true, false, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, redBagMoney,
                     canTicket ? ProgressStatus.Running : ProgressStatus.Waitting, canTicket ? TicketStatus.Ticketed : TicketStatus.Waitting);

                foreach (var item in info.AnteCodeList)
                {
                    var entityAnteCode = new C_Sports_AnteCode
                    {
                        SchemeId = schemeId,
                        AnteCode = item.AnteCode,
                        BonusStatus = (int)BonusStatus.Waitting,
                        CreateTime = DateTime.Now,
                        GameCode = info.GameCode,
                        GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        IsDan = item.IsDan,
                        IssuseNumber = info.IssuseNumber,
                        MatchId = item.MatchId,
                        PlayType = info.PlayType,
                        Odds = string.Empty,
                    };
                    anteCodeList.Add(entityAnteCode);
                    DB.GetDal<C_Sports_AnteCode>().Add(entityAnteCode);
                    //sportsManager.AddSports_AnteCode(entityAnteCode);
                }


                // 消费资金
                string msg = info.GameCode == "BJDC" ? string.Format("{0}第{1}期投注", BusinessHelper.FormatGameCode(info.GameCode), info.IssuseNumber)
                                                    : string.Format("{0} 投注", BusinessHelper.FormatGameCode(info.GameCode));
                if (redBagMoney > 0M)
                {
                    //var fundManager = new FundManager();
                    //var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                    //var maxUseMoney = info.TotalMoney * percent / 100;
                    //if (redBagMoney > maxUseMoney)
                    //    throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0}%，即{1:N2}元", percent, maxUseMoney));
                    //红包支付
                    BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, redBagMoney, msg, "Bet", password);
                }
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, schemeId, info.TotalMoney - redBagMoney, msg, place, password);

                DB.Commit();
            }

            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writerLog.WriteLog("SportsBetting", "SQL", (int)LogType.Warning, "存入订单、号码、扣钱操作", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #region 拆票

            watch.Restart();
            if (RedisHelper.EnableRedis)
            {
                var reidsWaitOrder = new RedisWaitTicketOrder
                {
                    RunningOrder = runningOrder,
                    AnteCodeList = anteCodeList
                };
                RedisOrderBusiness.AddOrderToRedis(info.GameCode, reidsWaitOrder);
                //RedisOrderBusiness.AddOrderToWaitSplitList(reidsWaitOrder);
            }
            else
            {
                DoSplitOrderTickets(schemeId);
            }

            watch.Stop();
            if (watch.Elapsed.TotalMilliseconds > 1000)
                writerLog.WriteLog("SportsBetting", "Redis",(int) LogType.Warning, "拆票", string.Format("总用时：{0}毫秒", watch.Elapsed.TotalMilliseconds));

            #endregion

            //刷新用户在Redis中的余额
            BusinessHelper.RefreshRedisUserBalance(userId);

            return schemeId;
        }
        /// <summary>
        /// 拆票，单式上传
        /// </summary>
        public void RequestTicketByGateway_SingleScheme_New(GatewayTicketOrder_SingleScheme order)
        {
            List<string> codeList = new List<string>();
           // var admin = new TicketGatewayAdmin();
            var matchIdList = new List<string>();
            var matchIdOddsList = new Dictionary<string, string>();

            using (DB)
            {
                DB.Begin();

                #region 保存订单、票数据
                try
                {
                   
                    switch (order.GameCode.ToUpper())
                    {
                        case "BJDC":
                           // codeList = admin.RequestTicket_BJDCSingleScheme(order, out matchIdList);
                            break;
                        case "JCZQ":
                           // codeList = admin.RequestTicket_JCZQSingleScheme(order, out matchIdList);
                            break;
                        case "JCLQ":
                            //codeList = admin.RequestTicket_JCLQSingleScheme(order, out matchIdList);
                            break;
                        case "CTZQ":
                            //codeList = admin.RequestTicket_CTZQSingleScheme(order);
                            break;
                        default:
                            throw new ArgumentException("请求出票不支持的订单彩种 - " + order.GameCode);
                    }
                    throw new NotImplementedException("请求出票不支持的订单彩种 - " + order.GameCode);
                    if (order.GameCode.ToUpper() == "JCZQ" || order.GameCode.ToUpper() == "JCLQ")
                    {
                        foreach (var item in matchIdList)
                        {
                            matchIdOddsList.Add(string.Format("{0}_{1}", order.GameType.ToUpper(), item), GetOddsToMatchId_New(item, order.GameCode.ToUpper(), order.GameType.ToUpper()));
                        }
                    }
                }
                catch (Exception EX)
                {
                    DB.Rollback();
                    throw EX;
                }
               

                #endregion

                DB.Commit();
            }

            //拆票
            RequestTicket_SingleSport(order, codeList, order.Amount, matchIdOddsList, matchIdList.Distinct().ToList());

        }
        public string ConvertIssuseNumber(string gameCode, string issuseNumber)
        {
            return issuseNumber;

            //switch (gameCode.ToLower())
            //{
            //    case "ssq":
            //    case "fc3d":
            //    case "cqssc":
            //        return issuseNumber.Replace("-", "");
            //    case "dlt":
            //    case "pl3":
            //    case "jx11x5":
            //        return issuseNumber.Substring(2).Replace("-", "");
            //    default:
            //        return issuseNumber;
            //}
        }
        public void RequestTicket_SingleSport(GatewayTicketOrder_SingleScheme order, List<string> codeList, int amount, Dictionary<string, string> matchIdOddsList, List<string> orderMatchIdList)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var ticketNum = 0;
            var hisManager = new Sports_Manager();
            //key:比赛编号_玩法  value:赔率
            var oddDic = new Dictionary<string, string>();
            try
            {
                var ticketTable = GetNewTicketTable();
                var dictionary = new Dictionary<string, int>();
                foreach (var item in codeList)
                {
                    //[0]: "158|3|5_1#159|1|5_1#160|1|5_1#166|0|5_1#169|1|5_1"
                    //[1]: "158|0|5_1#159|1|5_1#160|1|5_1#166|0|5_1#169|3|5_1"
                    #region 组合一注号码

                    var matchIdList = new List<string>();
                    var betType = string.Empty;
                    //var strList = new List<string>();
                    var locBetContentList = new List<string>();
                    foreach (var matchCode in item.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        //158|0|5_1
                        var array = matchCode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length != 3) continue;
                        if (string.IsNullOrEmpty(array[1]) || array[1] == "*" || array[1] == "**")
                            continue;

                        betType = array[2];
                        var matchId = ObjectExtension.ConverMatchId(order.GameCode, array[0]);
                        matchIdList.Add(array[0]);
                        //strList.Add(string.Format("{0}{1}^", ObjectExtension.ConverMatchId(order.GameCode, array[0]), ObjectExtension.ConvertAnteCode(order.GameCode, order.GameType, array[1])));
                        if (order.GameCode.ToUpper() == "BJDC" && !string.IsNullOrEmpty(order.GameType) && order.GameType.ToUpper() == "SXDS")
                            locBetContentList.Add(string.Format("{0}_{1}", array[0], array[1].Replace("0", "SD").Replace("1", "SS").Replace("2", "XD").Replace("3", "XS")));
                        else
                            locBetContentList.Add(string.Format("{0}_{1}", array[0], array[1]));
                    }

                    var locOdds = new List<string>();
                    if (order.GameCode.ToUpper() == "JCZQ" || order.GameCode.ToUpper() == "JCLQ")
                    {
                        foreach (var match in matchIdList)
                        {
                            locOdds.Add(matchIdOddsList.FirstOrDefault(p => p.Key == string.Format("{0}_{1}", order.GameType.ToUpper(), match)).Value);
                        }
                    }
                    var locOddStr = string.Join("/", locOdds);

                    #endregion

                    //最大倍数
                    var maxAmount = GetMaxTicketAmount(order.GameCode);
                    var tmpAmount = amount;
                    while (tmpAmount > 0)
                    {
                        var currentAmount = maxAmount;
                        if (tmpAmount <= maxAmount)
                        {
                            currentAmount = tmpAmount;
                        }
                        tmpAmount -= maxAmount;
                        var ticketMoney = 2M * currentAmount;
                        ticketNum++;
                        var ticketId = string.Format("{0}|{1}", order.OrderId, ticketNum.ToString().PadLeft(3, '0'));
                        //var locOdds =GetOddsToMatchId(string.Join("/", locBetContentList), order.GameCode, order.GameType);
                        //var history = new Sports_Ticket
                        //{
                        //    GameCode = order.GameCode.ToUpper(),
                        //    GameType = order.GameType.ToUpper(),
                        //    SchemeId = order.OrderId,
                        //    TicketId = ticketId,
                        //    IssuseNumber = ConvertIssuseNumber(order.GameCode, order.IssuseNumber),
                        //    CreateTime = DateTime.Now,
                        //    TicketStatus = order.IsRunningTicket ? TicketStatus.Ticketing : TicketStatus.Waitting,
                        //    PlayType = string.Format("P{0}", betType),
                        //    Amount = currentAmount,
                        //    BetUnits = 1,
                        //    BetMoney = ticketMoney,
                        //    BetContent = string.Join("/", locBetContentList),
                        //    MatchIdList = string.Join(",", matchIdList.ToArray()),
                        //    LocOdds = GetOddsToMatchId(string.Join("/", locBetContentList), order.GameCode, order.GameType),
                        //    PrintNumber3 = Guid.NewGuid().ToString("N").ToUpper(),
                        //};
                        //hisManager.AddSports_Ticket(history);

                        DataRow r = ticketTable.NewRow();
                        r["Id"] = ticketNum;
                        r["GameCode"] = order.GameCode.ToUpper();
                        r["GameType"] = order.GameType.ToUpper();
                        r["SchemeId"] = order.OrderId;
                        r["TicketId"] = ticketId;
                        r["IssuseNumber"] = ConvertIssuseNumber(order.GameCode, order.IssuseNumber);
                        r["PlayType"] = string.Format("P{0}", betType);
                        r["BetContent"] = string.Join("/", locBetContentList);
                        r["TicketStatus"] = 90;
                        r["Amount"] = currentAmount;
                        r["BetUnits"] = 1;
                        r["BetMoney"] = ticketMoney;
                        r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                        r["MatchIdList"] = string.Join(",", matchIdList.ToArray());
                        r["LocOdds"] = locOddStr;
                        r["IsAppend"] = false;
                        r["BonusStatus"] = 0;
                        r["PreTaxBonusMoney"] = 0M;
                        r["AfterTaxBonusMoney"] = 0M;
                        r["PrintDateTime"] = DateTime.Now;
                        r["CreateTime"] = DateTime.Now;

                        redisTicketList.Add(new RedisTicketInfo
                        {
                            AfterBonusMoney = 0M,
                            Amount = currentAmount,
                            BetContent = string.Join("/", locBetContentList),
                            BetMoney = ticketMoney,
                            BetUnits = 1,
                            BonusStatus = BonusStatus.Waitting,
                            GameCode = order.GameCode.ToUpper(),
                            GameType = order.GameType.ToUpper(),
                            IsAppend = false,
                            IssuseNumber = ConvertIssuseNumber(order.GameCode, order.IssuseNumber),
                            LocOdds = locOddStr,
                            MatchIdList = string.Join(",", matchIdList.ToArray()),
                            PlayType = string.Format("P{0}", betType),
                            PreBonusMoney = 0M,
                            SchemeId = order.OrderId,
                            TicketId = ticketId,
                        });

                        ticketTable.Rows.Add(r);

                        #region 号码表更新对应比赛sp

                        var oddArray = locOddStr.Split('/');
                        foreach (var content in locBetContentList)
                        {
                            var contentArray = content.Split('_');
                            var gameType = string.Empty;
                            var matchId = string.Empty;
                            if (contentArray.Length == 3)
                            {
                                //混合
                                gameType = contentArray[0];
                                matchId = contentArray[1];
                            }
                            if (contentArray.Length == 2)
                            {
                                //普通
                                gameType = order.GameType.ToUpper();
                                matchId = contentArray[0];
                            }
                            if (string.IsNullOrEmpty(gameType) || string.IsNullOrEmpty(matchId)) continue;

                            var matchIdStr = string.Format("{0}_", matchId);
                            var oddItem = oddArray.FirstOrDefault(p => p.StartsWith(matchIdStr));
                            if (string.IsNullOrEmpty(oddItem)) continue;
                            var matchIdOdd = oddItem.Replace(matchIdStr, string.Empty);
                            var key = string.Format("{0}_{1}", matchId, gameType).ToUpper();
                            if (!oddDic.ContainsKey(key))
                                oddDic.Add(key, matchIdOdd);
                        }

                        #endregion
                    }
                }

                //批量插入票表
                hisManager.SqlBulkAddTable(ticketTable);
                //调用Redis保存
                if (RedisHelper.EnableRedis)
                {
                    if (new string[] { "JCZQ", "JCLQ" }.Contains(order.GameCode.ToUpper()))
                    {
                        RedisOrderBusiness.AddToRunningOrder_JC(order.GameCode, order.OrderId, orderMatchIdList.ToArray(), redisTicketList);
                    }
                    if (order.GameCode == "BJDC")
                    {
                        RedisOrderBusiness.AddToRunningOrder_BJDC(order.OrderId, orderMatchIdList.ToArray(), redisTicketList);
                    }
                    if (order.GameCode == "CTZQ")
                    {
                        RedisOrderBusiness.AddToRunningOrder_SZC(SchemeType.GeneralBetting, order.GameCode, order.GameType, order.OrderId, "", true, order.IssuseNumber, redisTicketList);
                    }
                }

                //更新号码表比赛SP
                var anteCodeList = hisManager.QuerySportsAnteCodeBySchemeId(order.OrderId);
                foreach (var item in anteCodeList)
                {
                    if (!string.IsNullOrEmpty(item.Odds))
                        continue;

                    var key = string.Format("{0}_{1}", item.MatchId, item.GameType).ToUpper();
                    if (oddDic.ContainsKey(key))
                    {
                        var odd = oddDic[key] + ",-1|1";
                        item.Odds = odd;
                        hisManager.UpdateSports_AnteCode(item);
                    }
                }
            }
            catch (Exception ex)
            {
                //Common.Log.LogWriterGetter.GetLogWriter().Write("GatewayHandler_Shop", "RequestTicket_SingleSport", Common.Log.LogType.Information, "test", ex.ToString());
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 拆票，数字彩、传统足球
        /// </summary>
        public void RequestTicket(GatewayTicketOrder orderInfo, string keyLine, bool stopAfterBonus, SchemeType schemeType)
        {
            var redisTicketList = new List<RedisTicketInfo>();
            var gameType = string.Empty;
            var ticketList = AnalyzeOrder(orderInfo);
            var ticketTable = GetNewTicketTable();
            var index = 1;
            var locOdds = string.Empty;
            foreach (var ticket in ticketList)
            {
                var locBetContent = ticket.ToAnteString_Localhost();
                if (orderInfo.GameCode == "OZB")
                {
                    //欧洲杯特殊处理
                    var locOddsList = new List<string>();
                    var betArray = locBetContent.Split(',');
                    var ozbMatchList = new JCZQMatchManager().QueryJCZQ_OZBMatchList(ticket.GameType, betArray);
                    foreach (var c in betArray)
                    {
                        var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                        if (m == null) continue;
                        locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                    }
                    locOdds = string.Join("/", locOddsList);
                }
                if (orderInfo.GameCode == "SJB")
                {
                    //欧洲杯特殊处理
                    var locOddsList = new List<string>();
                    var betArray = locBetContent.Split(',');
                    var ozbMatchList = new JCZQMatchManager().QueryJCZQ_SJBMatchList(ticket.GameType, betArray);
                    foreach (var c in betArray)
                    {
                        var m = ozbMatchList.FirstOrDefault(p => p.MatchId == c);
                        if (m == null) continue;
                        locOddsList.Add(string.Format("{0}|{1}", m.MatchId, m.BonusMoney));
                    }
                    locOdds = string.Join("/", locOddsList);
                }

                var betType = ticket.BetCount <= 1 ? "DS" : "FS";
                var info = new 
                {
                    OrderId = orderInfo.OrderId + "|" + (index++).ToString("D3"),
                    BetType = ConvertGameType(orderInfo.GameCode, ticket.GameType, betType, ticket.BetCount),
                    IssueNumber = orderInfo.IssuseNumber,
                    BetUnits = ticket.BetCount,
                    Multiple = ticket.Amount,
                    BetMoney = ticket.TicketMoney,
                    IsAppend = orderInfo.IsAppend ? 1 : 0,
                    BetContent = locBetContent,
                };

                DataRow r = ticketTable.NewRow();
                r["Id"] = index;
                r["GameCode"] = orderInfo.GameCode.ToUpper();
                r["GameType"] = ticket.GameType.ToUpper();
                r["SchemeId"] = orderInfo.OrderId;
                r["TicketId"] = info.OrderId;
                r["IssuseNumber"] = orderInfo.IssuseNumber;
                r["PlayType"] = info.BetType;
                r["BetContent"] = info.BetContent;
                r["TicketStatus"] = 90;
                r["Amount"] = info.Multiple;
                r["BetUnits"] = info.BetUnits;
                r["BetMoney"] = info.BetMoney;
                r["PrintNumber3"] = Guid.NewGuid().ToString("N").ToUpper();
                r["IsAppend"] = orderInfo.IsAppend;
                r["BonusStatus"] = 0;
                r["PreTaxBonusMoney"] = 0M;
                r["AfterTaxBonusMoney"] = 0M;
                r["PrintDateTime"] = DateTime.Now;
                r["CreateTime"] = DateTime.Now;
                r["LocOdds"] = locOdds;
                if (string.IsNullOrEmpty(gameType))
                    gameType = ticket.GameType.ToUpper();
                redisTicketList.Add(new RedisTicketInfo
                {
                    AfterBonusMoney = 0M,
                    Amount = info.Multiple,
                    BetContent = info.BetContent,
                    BetMoney = info.BetMoney,
                    BetUnits = ticket.BetCount,
                    BonusStatus = BonusStatus.Waitting,
                    GameCode = orderInfo.GameCode.ToUpper(),
                    GameType = ticket.GameType.ToUpper(),
                    IsAppend = orderInfo.IsAppend,
                    IssuseNumber = orderInfo.IssuseNumber,
                    LocOdds = locOdds,
                    MatchIdList = ticket.ToAnteString_zhongminToMatchId(),
                    PlayType = "",
                    PreBonusMoney = 0M,
                    SchemeId = orderInfo.OrderId,
                    TicketId = info.OrderId,
                });

                ticketTable.Rows.Add(r);
            }

            //var watch = new Stopwatch();
            //watch.Start();
            var manager = new Sports_Manager();
            //批量插入票表
            manager.SqlBulkAddTable(ticketTable);
            if (orderInfo.GameCode == "OZB")
            {
                //更新号码表比赛SP
                var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                foreach (var item in codeList)
                {
                    if (!string.IsNullOrEmpty(item.Odds))
                        continue;

                    item.Odds = locOdds;
                    manager.UpdateSports_AnteCode(item);
                }
            }
            if (orderInfo.GameCode == "SJB")
            {
                //更新号码表比赛SP
                var codeList = manager.QuerySportsAnteCodeBySchemeId(orderInfo.OrderId);
                foreach (var item in codeList)
                {
                    if (!string.IsNullOrEmpty(item.Odds))
                        continue;

                    item.Odds = locOdds;
                    manager.UpdateSports_AnteCode(item);
                }
            }

            //调用Redis保存
            if (RedisHelper.EnableRedis)
                RedisOrderBusiness.AddToRunningOrder_SZC(schemeType, orderInfo.GameCode.ToUpper(), gameType, orderInfo.OrderId, keyLine, stopAfterBonus, orderInfo.IssuseNumber, redisTicketList);

            //watch.Stop();
            //this.writer.Write("RequestTicket", orderInfo.OrderId, LogType.Information, "保存票数据计时", "用时 " + watch.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// 拆票（无线程）
        /// </summary>
        public string DoSplitOrderTicketsWithNoThread(string schemeId)
        {
            var log = new List<string>();
            log.Add(string.Format("----------->>>>>>开始对订单{0}做票拆分", schemeId));
            var watch = new Stopwatch();
            watch.Start();
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null)
                //return;
                return string.Format("找不到订单{0}的订单数据", schemeId);

            if (!BusinessHelper.CanRequestBet(order.GameCode))
                //return;
                return string.Format("彩种{0}暂时不能出票", order.GameCode);
            var anteCodeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            if (anteCodeList.Count <= 0)
                //return;
                return string.Format("订单{0}无投注内容", schemeId);

            if (order.SchemeType == (int)SchemeType.ChaseBetting)
            {
                //清理冻结
                BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.UserId, order.SchemeId, string.Format("订单{0}出票完成，扣除冻结{1:N2}元", order.SchemeId, order.TotalMoney), order.TotalMoney);
            }
            var oldCount = sportsManager.QueryTicketCount(schemeId);
            watch.Stop();
            log.Add("1)查询订单和号码用时 " + watch.Elapsed.TotalMilliseconds);

            watch.Restart();
            if (oldCount <= 0)
            {
                //保存投注内容到文件
                if (order.SchemeBettingCategory == (int)SchemeBettingCategory.SingleBetting || order.SchemeBettingCategory == (int)SchemeBettingCategory.XianFaQiHSC)
                {
                    //单式
                    var singleCode = sportsManager.QuerySingleScheme_AnteCode(schemeId);
                    if (singleCode == null)
                        //return;
                        return string.Format("单式订单{0}找不到上传的号码数据", schemeId);

                    #region 拆票

                    RequestTicketByGateway_SingleScheme_New(new GatewayTicketOrder_SingleScheme
                    {
                        AllowCodes = singleCode.AllowCodes,
                        Amount = order.Amount,
                        ContainsMatchId = singleCode.ContainsMatchId,
                        FileBuffer = singleCode.FileBuffer,
                        GameCode = order.GameCode,
                        GameType = order.GameType,
                        IsRunningTicket = true,
                        IssuseNumber = order.IssuseNumber,
                        IsVirtualOrder = false,
                        OrderId = schemeId,
                        PlayType = order.PlayType,
                        SelectMatchId = singleCode.SelectMatchId,
                        TotalMoney = order.TotalMoney,
                        UserId = order.UserId,
                    });


                    //new Thread(() =>
                    //{
                    try
                    {
                        //生成文件
                        var json = Encoding.UTF8.GetString(singleCode.FileBuffer);
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.GameCode, schemeId.Substring(0, 10));
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var fileName = Path.Combine(path, string.Format("{0}.json", schemeId));
                        File.WriteAllText(fileName, json, Encoding.UTF8);
                    }
                    catch (Exception)
                    {
                    }
                    //}).Start();


                    #endregion
                }
                else
                {
                    //普通投注
                    var jcGameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
                    if (jcGameCodeArray.Contains(order.GameCode))
                    {
                        //竞彩
                        #region 拆票

                        var betInfo = new GatewayTicketOrder_Sport
                        {
                            Amount = order.Amount,
                            Attach = order.Attach,
                            GameCode = order.GameCode,
                            GameType = order.GameType,
                            IssuseNumber = order.IssuseNumber,
                            IsVirtualOrder = order.IsVirtualOrder,
                            OrderId = schemeId,
                            PlayType = order.PlayType,
                            UserId = order.UserId,
                            TotalMoney = order.TotalMoney,
                            Price = 2M,
                            IsRunningTicket = true,
                        };
                        foreach (var code in anteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode_Sport
                            {
                                AnteCode = code.AnteCode,
                                GameType = code.GameType,
                                IsDan = code.IsDan,
                                MatchId = code.MatchId,
                            });
                        }
                        RequestTicket_Sport(betInfo);

                        //new Thread(() =>
                        //{
                        try
                        {
                            //生成文件
                            var json = JsonHelper.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.GameCode, schemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", schemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }
                        //}).Start();
                        #endregion
                    }
                    else
                    {
                        //数字彩、传统足球
                        #region 拆票

                        var betInfo = new GatewayTicketOrder
                        {
                            Amount = order.Amount,
                            GameCode = order.GameCode,
                            IssuseNumber = order.IssuseNumber,
                            OrderId = schemeId,
                            Price = ((order.IsAppend && order.GameCode == "DLT") ? 3M : 2M),
                            TotalMoney = order.TotalMoney,
                            IsVirtualOrder = false,
                            Attach = "",
                            IsAppend = order.IsAppend,
                            UserId = order.UserId,
                            IsRunningTicket = true,
                        };
                        foreach (var item in anteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode
                            {
                                AnteNumber = item.AnteCode,
                                GameType = item.GameType,
                            });
                        }

                        RequestTicket(betInfo, "", true, (SchemeType)order.SchemeType);

                        //new Thread(() =>
                        //{

                        try
                        {
                            //生成文件
                            var json = JsonHelper.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.GameCode, schemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", schemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }

                        //}).Start();

                        #endregion
                    }
                }

                //触发出票完成接口
                BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.UserId, order.SchemeId, order.TotalMoney, order.TotalMoney });
            }
            watch.Stop();
            log.Add("2)执行拆分用时 " + watch.Elapsed.TotalMilliseconds);
            //this.writer.Write("DoSplitOrderTickets", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()));

            watch.Restart();
            order.IsSplitTickets = true;
            if (!order.TicketTime.HasValue)
                order.TicketTime = DateTime.Now;
            order.TicketStatus = (int)TicketStatus.Ticketed;
            order.ProgressStatus = (int)ProgressStatus.Running;
            sportsManager.UpdateSports_Order_Running(order);

            var detailManager = new SchemeManager();
            var detail = detailManager.QueryOrderDetail(schemeId);
            if (detail != null)
            {
                if (!detail.TicketTime.HasValue)
                    detail.TicketTime = DateTime.Now;
                detail.TicketStatus = (int)TicketStatus.Ticketed;
                detail.ProgressStatus = (int)ProgressStatus.Running;
                detail.CurrentBettingMoney = detail.TotalMoney;
                detailManager.UpdateOrderDetail(detail);
            }
            watch.Stop();
            log.Add("3)拆分和修改订单数据 " + watch.Elapsed.TotalMilliseconds);
            //this.writer.Write("DoSplitOrderTickets", schemeId, LogType.Information, "拆票日志", string.Join(Environment.NewLine, log.ToArray()));
            return string.Join(Environment.NewLine, log);
        }

        /// <summary>
        /// 订单拆票
        /// </summary>
        public void DoSplitOrderTickets(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId))
                return;
            //return "订单号不能为空";
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    DoSplitOrderTicketsWithNoThread(o.ToString());
                }
                catch (Exception ex)
                {
                    writerLog.ErrrorLog("DoSplitOrderTickets-DpSplitOrderTicketsWithNoThread", ex);
                }
            }, schemeId);

            //new Thread(() =>
            //{


            //}).Start();

            //return "拆分票成功";
        }
        public C_Sports_Order_Running AddRunningOrderAndOrderDetail(string schemeId, SchemeBettingCategory category,
            string gameCode, string gameType, string playType, bool stopAfterBonus,
            string issuseNumber, int amount, int betCount, int totalMatchCount, decimal totalMoney, DateTime stopTime,
            SchemeSource schemeSource, TogetherSchemeSecurity security, SchemeType schemeType, bool canChase, bool isVirtualOrder,
            string userId, string userAgent, DateTime betTime, ActivityType activityType, string attach, bool isAppend, decimal redBagMoney, ProgressStatus progressStatus, TicketStatus ticketStatus)
        {
            //if (DateTime.Now >= stopTime)
            if (betTime >= stopTime)
                throw new LogicException(string.Format("订单结束时间是{0}，订单不能投注。", stopTime.ToString("yyyy-MM-dd HH:mm")));
            var sportsManager = new Sports_Manager();
            if (new string[] { "JCZQ", "JCLQ" }.Contains(gameCode))
                issuseNumber = DateTime.Now.ToString("yyyy-MM-dd");

            DateTime? ticketTime = null;
            if (ticketStatus == TicketStatus.Ticketed)
                ticketTime = DateTime.Now;
            DateTime createtime = DateTime.Now;
            if (betTime != null && betTime.ToString() != "0001/1/1 0:00:00")
            {
                createtime = betTime;
            }
            else
            {
                betTime = DateTime.Now;
            }
            betTime = createtime;
            var order = new C_Sports_Order_Running
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                Amount = amount,
                BonusStatus = (int)BonusStatus.Waitting,
                CanChase = canChase,
                IsVirtualOrder = isVirtualOrder,
                IsPayRebate = false,
                RealPayRebateMoney = 0M,
                TotalPayRebateMoney = 0M,
                CreateTime = createtime,
                GameCode = gameCode,
                GameType = gameType,
                IssuseNumber = issuseNumber,
                PlayType = playType,
                PreTaxBonusMoney = 0M,
                ProgressStatus =(int) progressStatus,
                SchemeId = schemeId,
                SchemeType = (int)schemeType,
                SchemeBettingCategory = (int)category,
                TicketId = string.Empty,
                TicketLog = string.Empty,
                TicketStatus = (int)ticketStatus,
                TotalMatchCount = totalMatchCount,
                TotalMoney = totalMoney,
                SuccessMoney = totalMoney,
                UserId = userId,
                StopTime = stopTime,
                SchemeSource = (int)schemeSource,
                BetCount = betCount,
                BonusCount = 0,
                HitMatchCount = 0,
                RightCount = 0,
                Error1Count = 0,
                Error2Count = 0,
                MaxBonusMoney = 0,
                MinBonusMoney = 0,
                Security = (int)security,
                TicketGateway = string.Empty,
                TicketProgress = 1M,
                BetTime = betTime,
                ExtensionOne = string.Format("{0}{1}", "3X1_", (int)activityType),
                Attach = string.IsNullOrEmpty(attach) ? string.Empty : attach.ToUpper(),
                QueryTicketStopTime = stopTime.AddMinutes(1).ToString("yyyyMMddHHmm"),
                IsAppend = isAppend,
                RedBagMoney = redBagMoney,
                IsSplitTickets = ticketStatus == TicketStatus.Ticketed,
                TicketTime = ticketTime,
            };
           // sportsManager.AddSports_Order_Running(order);
            DB.GetDal<C_Sports_Order_Running>().Add(order);
            //订单总表信息
            var orderDetail = new C_OrderDetail
            {
                AfterTaxBonusMoney = 0M,
                AgentId = userAgent,
                BonusStatus = (int)BonusStatus.Waitting,
              //  ComplateTime = null,
                CreateTime = createtime,
                CurrentBettingMoney = ticketStatus == TicketStatus.Ticketed ? totalMoney : 0M,
                GameCode = gameCode,
                GameType = gameType,
                GameTypeName = BusinessHelper.FormatGameType(gameCode, gameType),
                PreTaxBonusMoney = 0M,
                ProgressStatus = (int)progressStatus,
                SchemeId = schemeId,
                SchemeSource = (int)schemeSource,
                SchemeType = (int)schemeType,
                SchemeBettingCategory = (int)category,
                StartIssuseNumber = issuseNumber,
                StopAfterBonus = stopAfterBonus,
                TicketStatus = (int)ticketStatus,
                TotalIssuseCount = 1,
                TotalMoney = totalMoney,
                UserId = userId,
                CurrentIssuseNumber = issuseNumber,
                IsVirtualOrder = isVirtualOrder,
                PlayType = playType,
                Amount = amount,
                AddMoney = 0M,
                BetTime = betTime,
                IsAppend = isAppend,
                RedBagMoney = redBagMoney,
                BonusAwardsMoney = 0M,
                RealPayRebateMoney = 0M,
                RedBagAwardsMoney = 0M,
                TotalPayRebateMoney = 0M,
                TicketTime = ticketTime,
            };
            //   new SchemeManager().AddOrderDetail(orderDetail);
            DB.GetDal<C_OrderDetail>().Add(orderDetail);
            return order;
        }

    }
}
