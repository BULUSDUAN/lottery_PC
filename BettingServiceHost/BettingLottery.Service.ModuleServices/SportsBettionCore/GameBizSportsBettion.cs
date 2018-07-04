using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.CoreModel.BetingEntities;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.ORM.Helper.BusinessLib;
using KaSon.FrameWork.ORM.Helper.UserHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel;
using KaSon.FrameWork.Common.Algorithms;
using KaSon.FrameWork.ORM.Helper.AnalyzerFactory;

namespace BettingLottery.Service.ModuleServices.SportsBettionCore
{
    public class GameBizSportsBettion:DBbase
    {
        private void CheckDisableGame(string gameCode, string gameType)
        {
            var manager = new LotteryGameManager();
            var entity = manager.QueryLotteryGame(gameCode);
            if (entity == null)
                throw new Exception("没有查到该彩种！");
            var status= entity.EnableStatus;

           // var status = new GameBusiness().LotteryGameToStatus(gameCode);
            if (status !=(int)EnableStatus.Enable)
                throw new Exception("彩种暂时不能投注");
        }
        private void CheckSchemeOrder(Sports_BetingInfo info)
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
                                throw new Exception("彩种玩法有误，应该是:" + BusinessHelper.FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + BusinessHelper.FormatGameType(info.GameCode, item.GameType));
                        }
                    }
                }
            }
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
                //biz.BeginTran();
                DB.Begin();
                try
                {


                    AddRunningOrderAndOrderDetail(schemeId, info.BettingCategory, info.GameCode, info.GameType, info.PlayType, true, info.IssuseNumber, info.Amount, betCount, info.TotalMatchCount, info.TotalMoney, stopTime,
                        info.SchemeSource, info.Security, SchemeType.SaveScheme, false, true, user.UserId, user.AgentId, info.CurrentBetTime, info.ActivityType, info.Attach, false, 0M, ProgressStatus.Waitting, TicketStatus.Waitting);
                    foreach (var item in info.AnteCodeList)
                    {
                        //sportsManager.AddSports_AnteCode(new C_Sports_AnteCode
                        //{
                        //    SchemeId = schemeId,
                        //    AnteCode = item.AnteCode,
                        //    BonusStatus = (int)BonusStatus.Waitting,
                        //    CreateTime = DateTime.Now,
                        //    GameCode = gameCode,
                        //    GameType = string.IsNullOrEmpty(item.GameType) ? info.GameType.ToUpper() : item.GameType.ToUpper(),
                        //    IsDan = item.IsDan,
                        //    IssuseNumber = info.IssuseNumber,
                        //    MatchId = item.MatchId,
                        //    PlayType = info.PlayType,
                        //    Odds = string.Empty,
                        //});
                        var c_entity = new C_Sports_AnteCode
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

                        DB.GetDal<C_Sports_AnteCode>().Add(c_entity);

                    }
                    //C_UserSaveOrder
                    var C_UserSaveOrderEntity = new C_UserSaveOrder
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
                    //用户的订单保存
                    //sportsManager.AddUserSaveOrder(new C_UserSaveOrder
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

                    DB.Commit();
                }
                catch (Exception EX)
                {
                    DB.Rollback();
                    throw EX;
                }
            }
            return schemeId;
        }

        /// <summary>
        /// 足彩投注,用户保存的订单
        /// </summary>
        public CommonActionResult SaveOrderSportsBettingByResult(Sports_BetingInfo info, string userToken)
        {
            // 验证用户身份及权限    
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckDisableGame(info.GameCode, info.GameType);
                BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);

                // 检查订单基本信息
                CheckSchemeOrder(info);

                string schemeId = new Sports_Business().SaveOrderSportsBetting(info, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "保存订单成功",
                };
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("保存订单异常，请重试 ", ex);
            }
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


        //验证 不支持的玩法
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

        /// <summary>
        /// 竞彩足球缓存数据
        /// </summary>
        private static System.Collections.Concurrent.ConcurrentDictionary<string, Sports_BetingInfo> _sportsBettingListInfo = new System.Collections.Concurrent.ConcurrentDictionary<string, Sports_BetingInfo>();

        public CommonActionResult Sports_Betting(Sports_BetingInfo info, string password, decimal redBagMoney, string userToken)
        {
            try
            {
                //检查彩种是否暂停销售
                KaSon.FrameWork.ORM.Helper.BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
               BusinessHelper.CheckGameCodeAndType(info.GameCode, info.GameType);
                // 验证用户身份及权限
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckJCRepeatBetting(userId, info);
                //检查投注内容,并获取投注注数
                var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //检查投注的比赛，并获取最早结束时间
                var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);

                string schemeId = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                schemeId = new Sports_Business().SportsBetting(info, userId, password, "Bet", totalCount, stopTime, redBagMoney);
                //}
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "足彩投注成功",
                };
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
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

            var c = new KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Combination();
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
                    var ac = new KaSon.FrameWork.ORM.Helper.AnalyzerFactory.ArrayCombination();
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
                ProgressStatus = (int)progressStatus,
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
            sportsManager.AddSports_Order_Running(order);

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
            new SchemeManager().AddOrderDetail(orderDetail);
            return order;
        }

      
        /// <summary>
        /// 检查竞彩订单频繁投注
        /// </summary>
        private void CheckJCRepeatBetting(string currUserId, Sports_BetingInfo info, bool isYouHua = false)
        {
            try
            {
                if (!_sportsBettingListInfo.ContainsKey(currUserId))
                {
                    info.CurrentBetTime = DateTime.Now;
                    _sportsBettingListInfo.TryAdd(currUserId, info);
                    return;
                }
            }
            catch (Exception)
            {

            }
            lock (_sportsBettingListInfo)
            {
                try
                {
                    Sports_BetingInfo value = _sportsBettingListInfo[currUserId];
                    if (isYouHua)//奖金优化
                    {
                        //不重复
                        if (!info.Equals(value))
                        {
                            _sportsBettingListInfo.TryRemove(currUserId, out value);
                            info.CurrentBetTime = DateTime.Now;
                            _sportsBettingListInfo.TryAdd(currUserId, info);
                            return;
                        }
                        //重复投注
                        if (value.Amount == info.Amount && value.GameCode.ToUpper() == info.GameCode.ToUpper() && value.PlayType == info.PlayType && value.TotalMoney == info.TotalMoney && value.Attach == info.Attach)
                        {
                            info.IsRepeat = true;
                        }
                        //重复投注
                        if (info.IsRepeat)
                        {
                            var timeSpan = DateTime.Now - value.CurrentBetTime;
                            if (timeSpan.TotalSeconds > 5)
                            {
                                //大于间隔时间
                                _sportsBettingListInfo.TryRemove(currUserId, out value);
                                info.CurrentBetTime = DateTime.Now;
                                _sportsBettingListInfo.TryAdd(currUserId, info);
                                return;
                            }
                            else
                            {
                                throw new LogicException("Repeat");
                            }
                        }
                    }
                    else
                    {
                        //不重复
                        if (!info.Equals(value))
                        {
                            _sportsBettingListInfo.TryRemove(currUserId, out value);
                            info.CurrentBetTime = DateTime.Now;
                            _sportsBettingListInfo.TryAdd(currUserId, info);
                            return;
                        }
                        //重复投注
                        if (value.Amount == info.Amount && value.GameCode.ToUpper() == info.GameCode.ToUpper() && value.PlayType == info.PlayType && value.TotalMoney == info.TotalMoney)
                        {
                            info.IsRepeat = true;
                        }
                        //重复投注
                        if (info.IsRepeat)
                        {
                            var timeSpan = DateTime.Now - value.CurrentBetTime;
                            if (timeSpan.TotalSeconds > 5)
                            {
                                //大于间隔时间
                                _sportsBettingListInfo.TryRemove(currUserId, out value);
                                info.CurrentBetTime = DateTime.Now;
                                _sportsBettingListInfo.TryAdd(currUserId, info);
                                return;
                            }
                            else
                            {
                                throw new LogicException("Repeat");
                            }
                        }
                    }
                }
                catch
                {
                    _sportsBettingListInfo.Clear();
                    return;
                }
            }
        }


    }
}
