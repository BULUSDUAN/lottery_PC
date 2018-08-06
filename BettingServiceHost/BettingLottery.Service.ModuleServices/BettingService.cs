using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using BettingLottery.Service.ModuleBaseServices;
using EntityModel.Enum;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using BettingLottery.Service.ModuleServices.SportsBettionCore;
using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Sport;
using System.Threading.Tasks;
using BettingLottery.Service.IModuleServices;

namespace BettingLottery.Service.ModuleServices
{
    [ModuleName("Betting")]
    public class BettingService : KgBaseService, IBettingService
    {
       
        // private readonly UserRepository _repository;
        //public UserService(UserRepository repository)
        //{
        //    this._repository = repository;
        //}
        IKgLog log = null;
        public BettingService()
        {

            log = new Log4Log();

        }
        //public void PublicInfo(IntegrationEvent evt)
        //{
        //    Publish(evt);
        //}

        //Task IIntegrationEventHandler<EventModel>.Handle(EventModel @event)
        //{
        //    throw new NotImplementedException();
        //}

        // log demo
        /// <summary>
        /// 日志使用 demo
        /// </summary>
       
        

        //public Task<int> GetUserId(string userName)
        //{
        //    //var xid = RpcContext.GetContext().GetAttachment("xid");

        //    //throw new Exception("错误！");

        //    //测试容错
        //   // Thread.Sleep(200000);

        //    //var T1 = TTest1();
        //    //var T21 = Test21();
        //    //var T2 = Test2();
        //    //var T3 = Test3();

        //    return Task.FromResult(1);
        //}


        #region 普通投注
        /// <summary>
        /// 普通投注
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public object Betting(string Param, SchemeSource SourceCode, string MsgId)
        {

           var p= JsonHelper.Decode(Param);
            string userToken = p.UserToken;
            string balancePassword = p.BalancePassword;
            string gameCode = p.GameCode.ToUpper();
            string gameType = p.GameType.ToUpper();
            string playType = p.PlayType.ToUpper();
            int security = p.Security;
            decimal totalMoney = p.TotalMoney;
            bool stopAfterBonus = p.StopAfterBonus;
            string SavaOrder = p.SavaOrder;
            decimal redBagMoney = p.RedBagMoney;
            if (redBagMoney <= 0)
                redBagMoney = 0;

            var _issuseList = p.IssuseList;
            var _code = p.CodeList;

            if (string.IsNullOrEmpty(gameCode))
                throw new Exception("彩种不能为空");
            if (string.IsNullOrEmpty(totalMoney.ToString()))
                throw new Exception("投注金额不能为空");
            if (string.IsNullOrEmpty(userToken))
                throw new Exception("userToken不能为空");
            if (totalMoney > 200000)
                throw new Exception("您的购买金额不能超过20万");
            var IsSaveOrder = "0";//是否为保存订单，0：不是保存订单；1：保存订单；
            if (!string.IsNullOrEmpty(SavaOrder))
                IsSaveOrder = SavaOrder;
            string returnValue = string.Empty;
            var successCount = 0;
            var codeCount = 0;
            var sportArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
            var array_gameType = gameType.Split('_');//HH_HHDG
            if (sportArray.Contains(gameCode))
            {
                #region
                //足球和篮球
                _code = JsonHelper.Decode(_code);
                _issuseList = JsonHelper.Decode(_issuseList);
                var codeList = new Sports_AnteCodeInfoCollection();
                if (array_gameType != null && array_gameType.Length >= 2)
                {
                    if (array_gameType[1].ToLower() == "hhdg")//单关固定投注
                    {
                        var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                        var fund = new FundBusiness();
                        var userBalance = fund.QueryUserBalance(userId);
                       // var userBalance = new GameBizSportsBettion().QueryMyBalance(userToken);
                        if (userBalance == null)
                            throw new Exception("未查询到账户信息");
                        else if ((userBalance.BonusBalance + userBalance.ExpertsBalance + userBalance.FillMoneyBalance + userBalance.RedBagBalance) < totalMoney)
                            throw new Exception("您好，目前账户余额不足！");
                        else if ((userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance + userBalance.FillMoneyBalance + userBalance.RedBagBalance) < totalMoney)
                            throw new Exception("您好，目前账户余额不足！");
                        try
                        {
                            foreach (var item in _code)
                            {
                                codeList = new Sports_AnteCodeInfoCollection();
                                gameType = item.GameType;
                                var code = new Sports_AnteCodeInfo()
                                {
                                    IsDan = item.IsDan,
                                    MatchId = item.MatchId,
                                    AnteCode = item.AnteCode,
                                    GameType = gameType,
                                };
                                codeList.Add(code);
                                playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
                                var amount = item.CurrentMoney / 2;
                                if (amount <= 0)
                                    throw new Exception("投注信息错误");
                                var info = new Sports_BetingInfo
                                {
                                    AnteCodeList = codeList,
                                    Amount = amount,
                                    BettingCategory = SchemeBettingCategory.GeneralBetting,
                                    GameCode = gameCode,
                                    GameType = gameType,
                                    PlayType = playType,
                                    SchemeSource = SourceCode,
                                    Security = (TogetherSchemeSecurity)security,
                                    TotalMoney = item.CurrentMoney,
                                    TotalMatchCount = (int)codeList.Count,
                                    IssuseNumber = _issuseList[0].IssuseNumber,
                                    SchemeProgress = TogetherSchemeProgress.Finish,
                                    ActivityType = ActivityType.NoParticipate,
                                    IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                                };
                                var result =new GameBizSportsBettion().Sports_Betting(info, balancePassword, redBagMoney, userToken);
                                //if (!result.IsSuccess)
                                //    throw new Exception(result.Message);
                                if (result.IsSuccess)
                                {
                                    successCount++;
                                    returnValue += result.ReturnValue + "~";
                                }
                                codeCount++;
                            }
                        }
                        catch { }
                        finally
                        {

                        }
                        if (successCount <= 0)
                        {
                            return new
                            {
                                Code = ResponseCode.失败,
                                Message = "投注失败",
                                MsgId = MsgId,
                                Value = "投注失败",
                                //Value = returnValue.TrimEnd('~'),
                            };
                        }
                        else if (successCount > 0 && successCount != codeCount)
                        {
                            return new 
                            {
                                Code = ResponseCode.成功,
                                Message = "您本次投注成功" + successCount + "笔，失败" + (codeCount - successCount) + "笔。",
                                MsgId = MsgId,
                                Value = "您本次投注成功" + successCount + "笔，失败" + (codeCount - successCount) + "笔。",
                                //Value = returnValue.TrimEnd('~'),  
                            };
                        }
                    }
                }
                else
                {
                    foreach (var item in _code)
                    {
                        var code = new Sports_AnteCodeInfo()
                        {
                            IsDan = item.IsDan,
                            MatchId = item.MatchId,
                            AnteCode = item.AnteCode,
                        };

                        if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf"))
                        {
                            code.GameType = item.GameType;
                        }
                        else
                        {
                            code.GameType = gameType;
                        }
                        codeList.Add(code);

                        //codeList.Add(new Sports_AnteCodeInfo
                        //{
                        //    AnteCode = item.AnteCode,
                        //    GameType = gameType != "HH" ? gameType : item.GameType,
                        //    IsDan = item.IsDan,
                        //    MatchId = item.MatchId,
                        //    PlayType = playType,
                        //});
                    }
                    playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
                    var info = new Sports_BetingInfo
                    {
                        AnteCodeList = codeList,
                        Amount = _issuseList[0].Amount,
                        BettingCategory = SchemeBettingCategory.GeneralBetting,
                        GameCode = gameCode,
                        GameType = gameType,
                        PlayType = playType,
                        SchemeSource = SourceCode,
                        Security = (TogetherSchemeSecurity)security,
                        TotalMoney = totalMoney,
                        TotalMatchCount = (int)totalMoney,
                        IssuseNumber = _issuseList[0].IssuseNumber,
                        SchemeProgress = TogetherSchemeProgress.Finish,
                        ActivityType = ActivityType.NoParticipate,
                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                        CurrentBetTime = DateTime.Now
                    };
                    var bettion = new GameBizSportsBettion();
                   
                    var result = IsSaveOrder == "0" ? bettion.Sports_Betting(info, balancePassword, redBagMoney, userToken): bettion.SaveOrderSportsBettingByResult(info, userToken);
                    if (!result.IsSuccess)
                        throw new Exception(result.Message);
                    returnValue = result.ReturnValue;
                }

                #endregion
            }

            return null;
        }
        #endregion

        #region 合买投注
        public Task<CommonActionResult> CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userToken)
        {
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            BettingHelper.CheckGameCodeAndType(info.GameCode, info.GameType);
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            //栓查是否实名
            //if (!BusinessHelper.IsUserValidateRealName(userId))
            //    throw new LogicException("未实名认证用户不能购买彩票");
            //检查重复投注
            CheckTogetherRepeatBetting(userId, info);
            //CheckDisableGame(info.GameCode, info.GameType);

            // 检查订单基本信息
            CheckSchemeOrder(info);

            try
            {
                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));

                //var list = new CacheDataBusiness().QuerySupperCreatorCollection();
                //if (list != null && list.FirstOrDefault(p => p.UserId == userId) != null)
                //    isTop = true;


                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().CreateSportsTogether(info, 0, userId, balancePassword, sysGuarantees, isTop, out canChase, out stopTime, ref schemeInfo);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICreateTogether_AfterTranCommit>(new object[] { userId, schemeId, info.GameCode, info.GameType, info.IssuseNumber, info.TotalMoney, stopTime });

                //参与合买后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, schemeInfo.SoldCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });

                return Task.FromResult(new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                });
            }
            catch (Exception ex)
            {
                throw new Exception("发起合买异常，请重试 ", ex);
            }
            //catch (Exception ex)
            //{
            //    throw new Exception("发起合买异常，请重试 ", ex);
            //}
        }

        /// <summary>
        /// 发起合买_保存订单
        /// </summary>
        public Task<CommonActionResult> SaveOrder_CreateSportsTogether(Sports_TogetherSchemeInfo info, string balancePassword, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigByKey("Site.Together.SystemGuarantees").ConfigValue);

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();

                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().SaveCreateSportsTogether(info, 0, userId, balancePassword, sysGuarantees, isTop);

                return Task.FromResult(new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 参与合买
        /// </summary>
        public Task<CommonActionResult> JoinSportsTogether(string schemeId, int buyCount, string joinPwd, string balancePassword, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            //bool isBet = new Sports_Business().UserIsBetting(userId);
            //if (!isBet)
            //    throw new Exception("对不起，网站已暂停彩票代购业务");
            //栓查是否实名
            //if (!BusinessHelper.IsUserValidateRealName(userId))
            //    throw new LogicException("未实名认证用户不能购买彩票");
            try
            {
                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();
                var canChase = new Sports_Business().JoinSportsTogether(schemeId, buyCount, userId, joinPwd, balancePassword, ref schemeInfo);

                //生成JsonData文件(合买大厅)
                //BusinessHelper.BuildJsonDataNotice("500");

                BusinessHelper.ExecPlugin<IAgentPayIn>(new object[] { schemeId });

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, buyCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, schemeInfo.TotalMoney, schemeInfo.SchemeProgress });
                return Task.FromResult(new CommonActionResult(true, "参与合买成功"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 检查数字彩、竞彩、优化合买

        /// <summary>
        /// 合买订单缓存
        /// </summary>
        private static Dictionary<string, Sports_TogetherSchemeInfo> _togetherSchemeInfo = new Dictionary<string, Sports_TogetherSchemeInfo>();

        /// <summary>
        /// 检查合买订单频繁投注
        /// </summary>
        private void CheckTogetherRepeatBetting(string currUserId, Sports_TogetherSchemeInfo info, bool isYouHua = false)
        {
            if (_togetherSchemeInfo == null || !_togetherSchemeInfo.ContainsKey(currUserId))
            {
                info.CurrentBetTime = DateTime.Now;
                _togetherSchemeInfo.Add(currUserId, info);
                return;
            }

            if (isYouHua)//奖金优化
            {
                try
                {
                    var cacheInfo = _togetherSchemeInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.Amount == info.Amount && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.PlayType == info.PlayType && s.Value.TotalMoney == info.TotalMoney && s.Value.Attach == info.Attach);
                    if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    if (!info.Equals(cacheInfo.Value))
                    {
                        //不重复
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    //投注内容相同
                    if (info.IsRepeat)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                    if (timeSpan.TotalSeconds > 5)
                    {
                        //大于间隔时间
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                }
                catch (Exception)
                {
                    _togetherSchemeInfo.Clear();
                    return;
                }
                throw new Exception("Repeat");
            }
            else
            {
                try
                {
                    var cacheInfo = _togetherSchemeInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.SchemeSource == info.SchemeSource && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
                    if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    if (!info.Equals(cacheInfo.Value))
                    {
                        //不重复
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    //投注内容相同
                    if (info.IsRepeat)
                    {
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                    var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                    if (timeSpan.TotalSeconds > 5)
                    {
                        //大于间隔时间
                        _togetherSchemeInfo.Remove(currUserId);
                        info.CurrentBetTime = DateTime.Now;
                        _togetherSchemeInfo.Add(currUserId, info);
                        return;
                    }
                }
                catch (Exception)
                {
                    _togetherSchemeInfo.Clear();
                    return;
                }
                throw new Exception("Repeat");
            }
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
                                throw new Exception("彩种玩法有误，应该是:" + BettingHelper.FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + BettingHelper.FormatGameType(info.GameCode, item.GameType));
                        }
                    }
                }
            }
        }


        private void CheckSchemeOrder(Sports_TogetherSchemeInfo info)
        {
            if (info.AnteCodeList.Count == 0)
                throw new ArgumentException("未选择任何比赛或者投注号码");
            if (info.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.TotalCount <= 0)
                throw new ArgumentException("合买总份数不能小于0");
            var allowGameCodeArray = "SSQ,DLT,FC3D,PL3,CTZQ,BJDC,JCZQ,JCLQ".Split(',');
            if (!allowGameCodeArray.Contains(info.GameCode.ToUpper()))
                throw new Exception("当前彩种不支持合买投注");
            var maxDeduct = 10;
            if (info.BonusDeduct > maxDeduct || info.BonusDeduct < 0)
                throw new Exception("合买提成有误，请确认后重新提交");
            if (info.BettingCategory != SchemeBettingCategory.YouHua)
            {
                var minMoney = info.TotalMoney * 5 / 100;
                minMoney = Math.Ceiling(minMoney);
                if (info.Subscription * info.Price < minMoney)
                    throw new ArgumentException(string.Format("合买发起人认购金额必须大于等于{0}%，即：{1:N2}元。", 5, minMoney));
            }
        }
        #endregion

        #region 北单、竞彩投注

        public Task<CommonActionResult> Sports_Betting(Sports_BetingInfo info , string password, decimal redBagMoney, string userToken)
        {
            try
            {
              //  Sports_BetingInfo info = new Sports_BetingInfo();
                   //检查彩种是否暂停销售
                   BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                BettingHelper.CheckGameCodeAndType(info.GameCode, info.GameType);
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

                return Task.FromResult(new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "足彩投注成功",
                });
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
            }

        }

        private void CheckDisableGame(string gameCode, string gameType)
        {
            var status = new GameBusiness().LotteryGameToStatus(gameCode);
            if (status != EnableStatus.Enable)
                throw new Exception("彩种暂时不能投注");
        }

        /// <summary>
        /// 足彩投注,用户保存的订单
        /// </summary>
        public Task<CommonActionResult> SaveOrderSportsBetting(Sports_BetingInfo info, string userToken)
        {
            // 验证用户身份及权限    
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                CheckDisableGame(info.GameCode, info.GameType);
                BettingHelper.CheckGameCodeAndType(info.GameCode, info.GameType);

                // 检查订单基本信息
                CheckSchemeOrder(info);

                string schemeId = new Sports_Business().SaveOrderSportsBetting(info, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return Task.FromResult(new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "保存订单成功",
                });
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("保存订单异常，请重试 ", ex);
            }
        }
        /// <summary>
        /// 足彩投注和追号
        /// </summary>
        public CommonActionResult Sports_BettingAndChase(Sports_BetingInfo info, string password, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var isSuceess = true;
                //info,
                var t = this.Sports_Betting(info,password, redBagMoney, userToken);
                isSuceess = t.Result.IsSuccess;
                var schemeId = string.Empty;
                var money = 0M;
                var array = t.Result.ReturnValue.Split('|');
                if (array.Length == 2)
                {
                    schemeId = array[0];
                    money = decimal.Parse(array[1]);
                }
                if (isSuceess)
                {
                    SportsChase(schemeId);
                }
                return new CommonActionResult { IsSuccess = isSuceess, Message = "订单提交成功", ReturnValue = string.Format("{0}|{1}", schemeId, money) };
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 检查竞彩和优化

        /// <summary>
        /// 竞彩足球缓存数据
        /// </summary>
        //private static Dictionary<string, Sports_BetingInfo> _sportsBettingListInfo = new Dictionary<string, Sports_BetingInfo>();
        /// <summary>
        /// 竞彩足球缓存数据
        /// </summary>
        private static System.Collections.Concurrent.ConcurrentDictionary<string, Sports_BetingInfo> _sportsBettingListInfo = new System.Collections.Concurrent.ConcurrentDictionary<string, Sports_BetingInfo>();

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
                                throw new Exception("Repeat");
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
                                throw new Exception("Repeat");
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

        #endregion

        #region 数字彩投注
        /// <summary>
        /// 数字彩投注
        /// </summary>
        public Task<CommonActionResult> LotteryBetting(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            //检查彩种是否暂停销售
            //LotteryBettingInfo info = new LotteryBettingInfo();
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");
                //var log = new List<string>();
                //log.Add("开始计时：" + userId);
                //var watch = new Stopwatch();
                //watch.Start();
                var checkError = CheckGeneralRepeatBetting(userId, info);
                if (!string.IsNullOrEmpty(checkError))
                    throw new Exception(checkError);

                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().LotteryBetting(info, userId, balancePassword, "Bet", redBagMoney);
                //2017-12-4 更新用户推广
                BusinessHelper.ExecPlugin<IBettingLottery_AfterTranCommit>(new object[] { userId, info, info.SchemeId, keyLine });
                //}

                //watch.Stop();
                //log.Add("计时结束：" + keyLine);
                //log.Add("用时 " + watch.Elapsed.TotalMilliseconds);
                //logger.Write("LotteryBeting", userId + "-" + watch.Elapsed.TotalMilliseconds, Common.Log.LogType.Information, "投注", string.Join(Environment.NewLine, log.ToArray()));

                return Task.FromResult(new CommonActionResult(true, "数字彩投注方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
                });
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            //catch (LogicException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
            }
        }
        #endregion

        #region 检查普通订单

        /// <summary>
        /// 普通订单缓存数据
        /// </summary>
        private static Dictionary<string, LotteryBettingInfo> _bettingListInfo = new Dictionary<string, LotteryBettingInfo>();

        /// <summary>
        /// 检查普通订单频繁投注
        /// </summary>
        private string CheckGeneralRepeatBetting(string currUserId, LotteryBettingInfo info)
        {
            try
            {
                //todo:备用 info.IsSubmit = false;
                if (_bettingListInfo == null || !_bettingListInfo.ContainsKey(currUserId))
                {
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                var cacheInfo = _bettingListInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.SchemeSource == info.SchemeSource && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
                if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                {
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                if (!info.Equals(cacheInfo.Value))
                {
                    //不重复
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                //投注内容相同
                if (info.IsRepeat)
                {
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                if (timeSpan.TotalSeconds > 5)
                {
                    //大于间隔时间
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                return "Repeat";
            }
            catch (Exception)
            {
                _bettingListInfo.Clear();
                return string.Empty;
            }
        }

        #endregion

        /// <summary>
        /// 足彩追号
        /// </summary>
        public CommonActionResult SportsChase(string schemeId)
        {
            //lock (UsefullHelper.moneyLocker)
            //{
            var isRequestTicketSuccess = new Sports_Business().SportsChase(schemeId);
            BusinessHelper.ExecPlugin<IComplateBetting_AfterTranCommit>(new object[] { string.Empty, schemeId, isRequestTicketSuccess });
            //提交事务后
            return new CommonActionResult
            {
                IsSuccess = isRequestTicketSuccess,
                Message = isRequestTicketSuccess ? "请求出票成功" : "请求出票失败",
                ReturnValue = schemeId,
            };
            //}
        }

        /// <summary>
        /// 保存用户未购买订单
        /// </summary>
        public Task<CommonActionResult> SaveOrderLotteryBetting(LotteryBettingInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                //CheckDisableGame(info.GameCode, info.AnteCodeList[0].GameType);


                string keyLine;

                string schemeId = new Sports_Business().SaveOrderLotteryBetting(info, userId, out keyLine);

                //! 执行扩展功能代码 - 提交事务后
                //BusinessHelper.ExecPlugin<IBettingLottery_AfterTranCommit>(new object[] { userId, info, schemeId, keyLine });
                return Task.FromResult(new CommonActionResult(true, "保存订单成功")
                {
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 优化合买
        /// <summary>
        /// 奖金优化合买
        /// </summary>
        public Task<CommonActionResult> CreateYouHuaSchemeTogether(Sports_TogetherSchemeInfo info, string balancePassword, decimal realTotalMoney, string userToken)
        {
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");
                CheckTogetherRepeatBetting(userId, info, true);//检查重复投注

                //CheckDisableGame(info.GameCode, string.Empty);

                // 检查订单基本信息
                CheckSchemeOrder(info);
                //最少认购5%
                var minMoney = realTotalMoney * 5 / 100;
                minMoney = Math.Ceiling(minMoney);
                if (info.Subscription * info.Price < minMoney)
                    throw new ArgumentException(string.Format("合买发起人认购金额必须大于等于{0}%，即：{1:N2}元。", 5, minMoney));

                var isTop = false;
                var sysGuarantees = int.Parse(new CacheDataBusiness().QueryCoreConfigFromRedis("Site.Together.SystemGuarantees"));
                //var list = new CacheDataBusiness().QuerySupperCreatorCollection();
                //if (list != null && list.FirstOrDefault(p => p.UserId == userId) != null)
                //    isTop = true;

                Sports_BetingInfo schemeInfo = new Sports_BetingInfo();
                string schemeId;
                DateTime stopTime;
                var canChase = false;
                schemeId = new Sports_Business().CreateYouHuaTogether(info, 0, userId, balancePassword, sysGuarantees, isTop, realTotalMoney, out canChase, out stopTime, ref schemeInfo);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICreateTogether_AfterTranCommit>(new object[] { userId, schemeId, info.GameCode, info.GameType, info.IssuseNumber, realTotalMoney, stopTime });

                //参与合买后
                BusinessHelper.ExecPlugin<IJoinTogether_AfterTranCommit>(new object[] { userId, schemeId, schemeInfo.SoldCount, schemeInfo.GameCode, schemeInfo.GameType, schemeInfo.IssuseNumber, realTotalMoney, schemeInfo.SchemeProgress });

                return Task.FromResult(new CommonActionResult(true, "发起合买成功")
                {
                    ReturnValue = schemeId + "|" + realTotalMoney,
                });
            }
            //catch (LogicException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("发起合买异常，请重试 ", ex);
            }
        }


        /// <summary>
        /// 虚拟奖金优化投注
        /// </summary>
        public Task<CommonActionResult> VirtualOrderYouHuaBet(Sports_BetingInfo info, decimal realTotalMoney, string userToken)
        {
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());

            //对附件字符做验证
            new Sports_Business().CheckYouHuaBetAttach(info.Attach, realTotalMoney, info.BettingCategory);
            try
            {
                // 验证用户身份及权限
                BusinessHelper.ExecPlugin<ICheckUserIsBetting_BeforeTranBegin>(new object[] { userId, info.TotalMoney });//检查当前用户是否可投注

                //检查投注内容,并获取投注注数
                var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //检查投注的比赛，并获取最早结束时间
                var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);
                var schemeId = new Sports_Business().VirtualOrderYouHuaBet(info, userId, realTotalMoney, totalCount, stopTime);
                BusinessHelper.ExecPlugin<IBonusOptimize>(new object[] { userId, schemeId });
                return Task.FromResult(new CommonActionResult(true, "奖金优化投注方案提交成功")
                {
                    ReturnValue = schemeId + "|" + realTotalMoney,
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 奖金优化投注
        /// </summary>
        public Task<CommonActionResult> YouHuaBet(Sports_BetingInfo info, string password, decimal realTotalMoney, decimal redBagMoney, string userToken)
        {
            try
            {
                var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());

                CheckJCRepeatBetting(userId, info, true);
                new Sports_Business().CheckYouHuaBetAttach(info.Attach, realTotalMoney, info.BettingCategory);

                //检查投注内容,并获取投注注数
                var totalCount = BusinessHelper.CheckBetCode(userId, info.GameCode.ToUpper(), info.GameType.ToUpper(), info.SchemeSource, info.PlayType, info.Amount, info.TotalMoney, info.AnteCodeList);
                //检查投注的比赛，并获取最早结束时间
                var stopTime = RedisMatchBusiness.CheckGeneralBettingMatch(info.GameCode.ToUpper(), info.GameType.ToUpper(), info.PlayType, info.AnteCodeList, info.IssuseNumber, info.BettingCategory);
                var schemeId = new Sports_Business().YouHuaBet(info, userId, password, realTotalMoney, totalCount, stopTime, redBagMoney);
                BusinessHelper.ExecPlugin<IBonusOptimize>(new object[] { userId, schemeId });

                //return new CommonActionResult
                //{
                //    IsSuccess = true,
                //    Message = "投注成功",
                //    ReturnValue = schemeId + "|" + realTotalMoney,
                //};
                return Task.FromResult(new CommonActionResult(true, "足彩投注方案提交成功")
                {
                    ReturnValue = schemeId + "|" + realTotalMoney,
                });
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            //catch (LogicException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
            }
        }
        #endregion

        #region 编辑合买跟单
        /// <summary>
        /// 编辑合买跟单
        /// </summary>
        public Task<CommonActionResult> EditTogetherFollower(TogetherFollowerRuleInfo info, long ruleId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().EditTogetherFollower(info, ruleId);
                return Task.FromResult(new CommonActionResult(true, "编辑跟单成功"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 定制合买跟单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> CustomTogetherFollower(TogetherFollowerRuleInfo info, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().CustomTogetherFollower(info);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ITogetherFollow_AfterTranCommit>(new object[] { info });
                return Task.FromResult(new CommonActionResult(true, "订制合买跟单成功"));
            }
            //catch (LogicException ex)
            //{
            //    return new CommonActionResult(false, ex.Message);
            //}
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 退订跟单
        /// </summary>
        public Task<CommonActionResult> ExistTogetherFollower(long followerId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var rule = new Sports_Business().ExistTogetherFollower(followerId, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IExistTogetherFollow_AfterTranCommit>(new object[] { rule.CreaterUserId, rule.FollowerUserId, rule.GameCode, rule.GameType });
                return Task.FromResult(new CommonActionResult(true, "退订跟单成功"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 宝单分享-创建宝单
        /// </summary>
        public Task<CommonActionResult> SaveOrderSportsBetting_DBFX(Sports_BetingInfo info, string userId)
        {
            try
            {
                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());

                // 检查订单基本信息
                CheckSchemeOrder(info);

                string schemeId = new Sports_Business().SaveOrderSportsBetting_DBFX(info, userId);

                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });

                return Task.FromResult(new CommonActionResult
                {
                    IsSuccess = true,
                    ReturnValue = schemeId + "|" + info.TotalMoney,
                    Message = "保存订单成功",
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 宝单分享-抄单
        /// </summary>
        public Task<CommonActionResult> Sports_BettingAndChase_BDFX(Sports_BetingInfo info, string password, string userId)
        {
            try
            {
                //检查用户余额是否足够
                BusinessHelper.CheckBalance(userId);

                //检查彩种是否暂停销售
                BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
                //检查混合串关方式
                BettingHelper.CheckHHPlayType(info.GameCode, info.GameType, info.PlayType, info.AnteCodeList);

                CheckJCRepeatBetting(userId, info);
                BusinessHelper.ExecPlugin<ICheckUserIsBetting_BeforeTranBegin>(new object[] { userId, info.TotalMoney });//检查当前用户是否可投注

                string schemeId = string.Empty;

                // 检查订单基本信息
                CheckSchemeOrder(info);
                //lock (UsefullHelper.moneyLocker)
                //{
                schemeId = new Sports_Business().SportsBetting_BDFX(info, userId, password, "Bet");
                //}
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IBettingSport_AfterTranCommit>(new object[] { userId, info, schemeId });
                return Task.FromResult(new CommonActionResult { IsSuccess = true, Message = "订单提交成功", ReturnValue = string.Format("{0}|{1}", schemeId, info.TotalMoney) });
                //return new CommonActionResult
                //{
                //    IsSuccess = true,
                //    ReturnValue = schemeId + "|" + info.TotalMoney,
                //    Message = "足彩投注成功",
                //};
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            //catch (LogicException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
            }
        }

        /// <summary>
        /// 世界杯投注
        /// </summary>
        public Task<CommonActionResult> BetSJB(LotteryBettingInfo info, string balancePassword, decimal redBagMoney, string userToken)
        {
            // 验证用户身份及权限
            //检查彩种是否暂停销售
            BusinessHelper.CheckGameEnable(info.GameCode.ToUpper());
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //栓查是否实名
                //if (!BusinessHelper.IsUserValidateRealName(userId))
                //    throw new LogicException("未实名认证用户不能购买彩票");

                //var checkError = CheckGeneralRepeatBetting(userId, info);
                //if (!string.IsNullOrEmpty(checkError))
                //    throw new LogicException(checkError);

                var keyLine = string.Empty;
                //lock (UsefullHelper.moneyLocker)
                //{
                keyLine = new Sports_Business().BetSJB(info, userId, balancePassword, "Bet", redBagMoney);
                //}

                return Task.FromResult(new CommonActionResult(true, "方案提交成功")
                {
                    ReturnValue = keyLine + "|" + info.TotalMoney,
                });
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.Message);
            }
            //catch (LogicException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new Exception("订单投注异常，请重试 ", ex);
            }
        }
    }
}
