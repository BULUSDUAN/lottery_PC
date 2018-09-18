using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.Redis;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using Lottery.Api.Controllers.CommonFilterActtribute;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaSon.FrameWork.Common.ExceptionEx;
using System.Diagnostics;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class BettingController : BaseController
    {
        #region 普通投注,世界杯投注(104,210)
        /// <summary>
        /// 普通投注_104
        /// </summary>
        public async Task<IActionResult> Betting([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string balancePassword = p.BalancePassword;
                string gameCode = p.GameCode == null ? null : ((string)p.GameCode).ToUpper();
                string gameType = p.GameType == null ? null : ((string)p.GameType).ToUpper();
                string playType = p.PlayType == null ? null : ((string)p.PlayType).ToUpper();
                int security = p.Security;
                decimal totalMoney = p.TotalMoney;
                bool stopAfterBonus = p.StopAfterBonus;
                string SavaOrder = p.SavaOrder;
                decimal redBagMoney = p.RedBagMoney;
                if (redBagMoney <= 0)
                    redBagMoney = 0;
                bool? isExy = p.IsExy;
                bool? isAppend = p.IsAppend;

                string _issuseList = p.IssuseList;
                string _code = p.CodeList;

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
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                string returnValue = string.Empty;
                var successCount = 0;
                var codeCount = 0;
                var sportArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
                var array_gameType = gameType.Split('_');//HH_HHDG
                if (sportArray.Contains(gameCode))
                {
                    #region
                    //足球和篮球
                    var _codeList = JsonHelper.Decode(_code);
                    var _theissuseList = JsonHelper.Decode(_issuseList);
                    var codeList = new List<Sports_AnteCodeInfo>();
                    if (array_gameType != null && array_gameType.Length >= 2)
                    {
                        if (array_gameType[1].ToLower() == "hhdg")//单关固定投注
                        {
                            try
                            {
                                foreach (var item in _codeList)
                                {
                                    codeList = new List<Sports_AnteCodeInfo>();
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
                                        BettingCategory = isExy==true? SchemeBettingCategory.ErXuanYi:SchemeBettingCategory.GeneralBetting,
                                        GameCode = gameCode,
                                        GameType = gameType,
                                        PlayType = playType,
                                        SchemeSource = entity.SourceCode,
                                        Security = (TogetherSchemeSecurity)security,
                                        TotalMoney = item.CurrentMoney,
                                        TotalMatchCount = codeList.GroupBy(a=>a.MatchId).ToList().Count,
                                        IssuseNumber = _theissuseList[0].IssuseNumber,
                                        SchemeProgress = TogetherSchemeProgress.Finish,
                                        ActivityType = ActivityType.NoParticipate,
                                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                                    };
                                    var param = new Dictionary<string, object>();
                                    param.Add("info", info);
                                    param.Add("password", balancePassword);
                                    param.Add("redBagMoney", redBagMoney);
                                    param.Add("userid", userid);
#if LogInfo
                                    var st = new Stopwatch();
                                    st.Start();
#endif
                                    var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/Sports_Betting");
#if LogInfo
                                    st.Stop();
                                    Log4Log.LogEX(KLogLevel.TimeInfo, "投注足彩", "用时：" + st.Elapsed.TotalMilliseconds.ToString() + "毫秒");
#endif
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
                                return Json(new LotteryServiceResponse
                                {
                                    Code = ResponseCode.失败,
                                    Message = "投注失败",
                                    MsgId = entity.MsgId,
                                    Value = "投注失败",
                                    //Value = returnValue.TrimEnd('~'),
                                });
                            }
                            else if (successCount > 0 && successCount != codeCount)
                            {
                                return Json(new LotteryServiceResponse
                                {
                                    Code = ResponseCode.成功,
                                    Message = "您本次投注成功" + successCount + "笔，失败" + (codeCount - successCount) + "笔。",
                                    MsgId = entity.MsgId,
                                    Value = "您本次投注成功" + successCount + "笔，失败" + (codeCount - successCount) + "笔。",
                                    //Value = returnValue.TrimEnd('~'),
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in _codeList)
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
                            Amount = _theissuseList[0].Amount,
                            BettingCategory = isExy==true? SchemeBettingCategory.ErXuanYi: SchemeBettingCategory.GeneralBetting,
                            GameCode = gameCode,
                            GameType = gameType,
                            PlayType = playType,
                            SchemeSource = entity.SourceCode,
                            Security = (TogetherSchemeSecurity)security,
                            TotalMoney = totalMoney,
                            TotalMatchCount = codeList.GroupBy(a => a.MatchId).ToList().Count,
                            IssuseNumber = _theissuseList[0].IssuseNumber,
                            SchemeProgress = TogetherSchemeProgress.Finish,
                            ActivityType = ActivityType.NoParticipate,
                            IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                            CurrentBetTime = DateTime.Now
                        };
                        var param = new Dictionary<string, object>();
                        param.Add("info", info);
                        param.Add("password", balancePassword);
                        param.Add("redBagMoney", redBagMoney);
                        param.Add("userid", userid);
                        var saveparam = new Dictionary<string, object>();
                        saveparam.Add("info", info);
                        saveparam.Add("userid", userid);
#if LogInfo
                        var st = new Stopwatch();
                        st.Start();
#endif
                        var result = IsSaveOrder == "0" ? await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/Sports_Betting") :
                            await _serviceProxyProvider.Invoke<CommonActionResult>(saveparam, "api/Betting/SaveOrderSportsBetting");
#if LogInfo
                        st.Stop();
                        Log4Log.LogEX(KLogLevel.TimeInfo, "投注竞技足彩", "用时：" + st.Elapsed.TotalMilliseconds.ToString() + "毫秒");
#endif
                        if (!result.IsSuccess)
                            throw new Exception(result.Message);
                        returnValue = result.ReturnValue;
                    }

                    #endregion
                }
                else
                {
                    //数字彩和传统足球
                    var codeList = new List<LotteryAnteCodeInfo>(); //new LotteryAnteCodeInfoCollection();
                    var _codeList = JsonHelper.Decode(_code);
                    foreach (var item in _codeList)
                    {
                        codeList.Add(new LotteryAnteCodeInfo
                        {
                            AnteCode = Convert.ToString(item.AnteCode),
                            GameType = gameType != "HH" ? gameType : item.GameType,
                            IsDan = item.IsDan,
                        });
                    }

                    var issuseList = new List<LotteryBettingIssuseInfo>();// new LotteryBettingIssuseInfoCollection();
                    var list = JsonHelper.Decode(_issuseList);
                    foreach (var item in list)
                    {
                        issuseList.Add(new LotteryBettingIssuseInfo
                        {
                            Amount = item.Amount,
                            IssuseNumber = item.IssuseNumber,
                            IssuseTotalMoney = item.IssuseTotalMoney,
                        });
                    }

                    var info = new LotteryBettingInfo
                    {
                        BettingCategory = SchemeBettingCategory.GeneralBetting,
                        GameCode = gameCode,
                        SchemeSource = entity.SourceCode,
                        Security = (TogetherSchemeSecurity)security,
                        TotalMoney = totalMoney,
                        StopAfterBonus = stopAfterBonus,
                        AnteCodeList = codeList,
                        IssuseNumberList = issuseList,
                        ActivityType = ActivityType.NoParticipate,
                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                        CurrentBetTime = DateTime.Now,
                        IsAppend= isAppend==true?true:false
                    };
                    var param = new Dictionary<string, object>();
                    param.Add("info", info);
                    param.Add("balancePassword", balancePassword);
                    param.Add("redBagMoney", redBagMoney);
                    param.Add("userid", userid);
                    var saveparam = new Dictionary<string, object>();
                    saveparam.Add("info", info);
                    saveparam.Add("userid", userid);
#if LogInfo
                    var st = new Stopwatch();
                    st.Start();
#endif
                    var result = IsSaveOrder == "0" ? await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/LotteryBetting") :
                           await _serviceProxyProvider.Invoke<CommonActionResult>(saveparam, "api/Betting/SaveOrderLotteryBetting");
#if LogInfo
                    st.Stop();
                    Log4Log.LogEX(KLogLevel.TimeInfo, "投注数字彩或传统足球", "用时：" + st.Elapsed.TotalMilliseconds.ToString() + "毫秒");
#endif
                    if (!result.IsSuccess)
                        throw new Exception(result.Message);
                    returnValue = result.ReturnValue;
                }

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "投注成功",
                    MsgId = entity.MsgId,
                    Value = returnValue.TrimEnd('~'),
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 世界杯_210
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> bet_sjb([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string balancePassword = p.BalancePassword;
                string gameCode = p.GameCode.ToUpper();
                string gameType = p.GameType.ToUpper();
                string playType = p.PlayType.ToUpper();
                int amount = p.Amount;//倍数
                string number = p.AnteCode;
                //int security = p.Security;
                decimal totalMoney = p.TotalMoney;
                //bool stopAfterBonus = p.StopAfterBonus;
                string SavaOrder = p.SavaOrder;
                decimal redBagMoney = p.RedBagMoney;
                if (redBagMoney <= 0)
                    redBagMoney = 0;

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
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                string returnValue = string.Empty;
                if (gameCode == "SJB")
                {
                    //投注信息
                    var info = new LotteryBettingInfo
                    {
                        ActivityType = ActivityType.NoParticipate,
                        BettingCategory = SchemeBettingCategory.GeneralBetting,
                        CurrentBetTime = DateTime.Now,
                        GameCode = "SJB",
                        IsAppend = false,
                        IsRepeat = false,
                        IsSubmit = false,
                        SchemeSource = entity.SourceCode
,
                        Security = TogetherSchemeSecurity.Public,
                        StopAfterBonus = false,
                        TicketTime = DateTime.Now,
                        TotalMoney = totalMoney,
                    };
                    info.AnteCodeList.Add(new LotteryAnteCodeInfo
                    {
                        GameType = gameType,
                        IsDan = false,
                        AnteCode = number,
                    });
                    info.IssuseNumberList.Add(new LotteryBettingIssuseInfo
                    {
                        IssuseNumber = "2018",
                        Amount = amount,
                        IssuseTotalMoney = totalMoney,
                    });
                    var result = new CommonActionResult();
                    var param = new Dictionary<string, object>();
                    param.Add("info", info);
                    param.Add("password", balancePassword);
                    param.Add("redBagMoney", redBagMoney);
                    param.Add("userid", userid);
                    result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/BetSJB");
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "投注成功",
                        MsgId = entity.MsgId,
                        Value = result.ReturnValue.TrimEnd('~'),
                    });
                }
                else
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = "投注类型错误",
                        MsgId = entity.MsgId,
                        Value = returnValue.TrimEnd('~'),
                    });
                }

            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "投注失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region 合买投注、参与合买投注(134,148)
        /// <summary>
        /// 发起合买_134
        /// </summary>
        public async Task<IActionResult> TogetherBetting([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string balancePassword = p.BalancePassword;
                string gameCode = p.GameCode == null ? null : ((string)p.GameCode).ToUpper();
                string gameType = p.GameType == null ? null : ((string)p.GameType).ToUpper();
                string playType = p.PlayType == null ? null : ((string)p.PlayType).ToUpper();
                string issuseNumber = p.IssuseNumber;
                int amount = p.Amount;
                decimal totalMoney = p.TotalMoney;
                int matchCount = p.TotalMatchCount;
                int security = p.Security;
                string joinpwd = p.JoinPwd;
                int bonusdeduct = p.BonusDeduct;//中奖提成
                int guarantees = p.Guarantees;//保底份数
                int subscription = p.Subscription;//认购
                string title = p.Title;
                string description = p.Description;
                bool? isExy = p.IsExy;
                //如果为1 则为保存订单，用于ios
                string isSaveOrder = p.SavaOrder;
                //if (entity.SourceCode == SchemeSource.New_Android || string.IsNullOrEmpty(isSaveOrder))//注意：安卓最新版本发布后，此处判断可以去掉
                //    isSaveOrder = "0";


                string _codeStr = p.CodeList;
                //var _code = System.Web.Helpers.Json.Decode(p.CodeList);// System.Web.Helpers.Json.Decode(_codeStr);
                var _code = JsonHelper.Decode(_codeStr);
                // CodeList结构：AnteCode,IsDan,MatchId

                if (string.IsNullOrEmpty(gameCode))
                    throw new Exception("彩种不能为空");
                if (string.IsNullOrEmpty(gameType))
                    throw new Exception("玩法不能为空");
                if (string.IsNullOrEmpty(amount.ToString()))
                    throw new Exception("倍数不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("标识不能为空");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                string returnValue = string.Empty;
                var sportArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
                if (sportArray.Contains(gameCode))
                {
                    var array_gameType = gameType.Split('_');
                    if (array_gameType != null && array_gameType.Length >= 2)
                    {
                        if (array_gameType[1].ToLower() == "hhdg")//单关固定
                        {
                        }
                    }
                    else
                    {
                        //足球和篮球
                        var codeList = new List<Sports_AnteCodeInfo>();
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
                        Sports_TogetherSchemeInfo togInfo = new Sports_TogetherSchemeInfo()
                        {
                            GameCode = gameCode,
                            GameType = gameType,
                            IssuseNumber = issuseNumber,
                            Amount = amount,
                            AnteCodeList = codeList,
                            PlayType = playType,
                            SchemeSource = entity.SourceCode,
                            TotalMoney = totalMoney,
                            TotalMatchCount = matchCount,
                            Title = title,
                            Description = description,
                            TotalCount = (int)totalMoney,
                            Security = (TogetherSchemeSecurity)security,
                            Price = 1M,
                            BonusDeduct = bonusdeduct,
                            Guarantees = guarantees,
                            JoinPwd = joinpwd,
                            Subscription = subscription,
                            ActivityType = ActivityType.NoParticipate,
                            IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                            BettingCategory= isExy==true ? SchemeBettingCategory.ErXuanYi : SchemeBettingCategory.GeneralBetting,
                        };
                        var param = new Dictionary<string, object>();
                        param.Add("info", togInfo);
                        param.Add("balancePassword", balancePassword);
                        param.Add("userid", userid);
                        var hmResult = isSaveOrder == "0" ? await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/CreateSportsTogether") :
                            await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/SaveOrder_CreateSportsTogether");
                        if (!hmResult.IsSuccess)
                            throw new Exception(hmResult.Message);
                        returnValue = hmResult.ReturnValue;
                    }
                }
                else
                {
                    //数字彩和传统足球
                    var codeList = new Sports_AnteCodeInfoCollection();
                    foreach (var item in _code)
                    {
                        codeList.Add(new Sports_AnteCodeInfo
                        {
                            AnteCode = item.AnteCode,
                            GameType = gameType != "HH" ? gameType : item.GameType,
                            //GameType = gameType,
                            IsDan = item.IsDan,
                            MatchId = item.MatchId,
                            PlayType = playType,
                        });
                    }
                    Sports_TogetherSchemeInfo togInfo = new Sports_TogetherSchemeInfo()
                    {
                        GameCode = gameCode,
                        GameType = gameType,
                        IssuseNumber = issuseNumber,
                        Amount = amount,
                        AnteCodeList = codeList,
                        PlayType = playType,
                        SchemeSource = entity.SourceCode,
                        TotalMoney = totalMoney,
                        TotalMatchCount = matchCount,
                        Title = title,
                        Description = description,
                        TotalCount = (int)totalMoney,
                        Security = (TogetherSchemeSecurity)security,
                        Price = 1M,
                        BonusDeduct = bonusdeduct,
                        Guarantees = guarantees,
                        JoinPwd = joinpwd,
                        Subscription = subscription,
                        ActivityType = ActivityType.NoParticipate,
                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                    };
                    var param = new Dictionary<string, object>();
                    param.Add("info", togInfo);
                    param.Add("balancePassword", balancePassword);
                    param.Add("userid", userid);
                    var hmResult = isSaveOrder == "0" ? await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/CreateSportsTogether") :
                        await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/SaveOrder_CreateSportsTogether");
                    if (!hmResult.IsSuccess)
                        throw new Exception(hmResult.Message);
                    returnValue = hmResult.ReturnValue;
                }

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "合买投注成功",
                    MsgId = entity.MsgId,
                    Value = returnValue,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "发起合买失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 参与合买_148
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> BetJoinTogether([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string schemeId = p.SchemeId;
                int buycount = p.BuyCount;
                string joinpwd = p.JoinPwd;
                string balancepwd = p.BalancePwd;
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("你还未登录");
                else if (string.IsNullOrEmpty(schemeId))
                    throw new Exception("订单号不能为空");
                else if (buycount <= 0)
                    throw new Exception("购买份数不能小于1份");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var param = new Dictionary<string, object>();
                param.Add("schemeId", schemeId);
                param.Add("buyCount", buycount);
                param.Add("joinPwd", joinpwd);
                param.Add("balancePassword", balancepwd);
                param.Add("userid", userid);
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/JoinSportsTogether");
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.IsSuccess ? "参与合买成功" : "参与合买失败",
                    MsgId = entity.MsgId,
                    Value = result.IsSuccess ? "参与合买成功" : "参与合买失败",
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "参与合买失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 网站优化投注(142)
        /// <summary>
        /// 优化投注_142
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> OptimizationBetting([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {

                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string gameCode = p.GameCode == null ? null : ((string)p.GameCode).ToUpper();
                string gameType = p.GameType == null ? null : ((string)p.GameType).ToUpper();
                string playType = p.PlayType == null ? null : ((string)p.PlayType).ToUpper();
                string issuseNumber = p.IssuseNumber;
                //string anteCode = p.AnteCode;
                var _code = p.CodeList;
                int amount = p.Amount;
                decimal totalMoney = p.TotalMoney;
                decimal org = p.OrgMoney;
                int matchCount = p.MatchCount;
                string balancePwd = p.BalancePwd;
                int sercu = p.Sercurity;
                string attach = p.Attach;
                bool isHemai = p.IsHemai;
                int bettingCategory = p.BettingCategory;
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var anteCodeList = new Sports_AnteCodeInfoCollection();
                var codeArray = JsonHelper.Decode(_code);
                foreach (var item in codeArray)
                {
                    var code = new Sports_AnteCodeInfo()
                    {
                        IsDan = item.IsDan,
                        MatchId = item.MatchId,
                        AnteCode = item.AnteCode,
                    };
                    //if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf") && cods.Length > 3)
                    //if (item.Length > 3)
                    //{
                    //code.GameType = item.GameType;
                    //}
                    if ((gameType.ToLower() == "hh" || gameType.ToLower() == "spf"))
                    {
                        code.GameType = item.GameType;
                    }
                    else
                    {
                        code.GameType = gameType;
                    }
                    anteCodeList.Add(code);
                }
                SchemeSource schemeSource = entity.SourceCode;
                //if (entity.SourceCode == SchemeSource.Iphone)
                //    schemeSource = SchemeSource.Iphone;
                //投注过关方式
                playType = playType.Replace("P0_1", "").Replace("P", "").Replace(",", "|");
                if (isHemai)
                {
                    string title = p.Title;
                    string desc = Convert.ToString(p.Description) == string.Empty ? amount + "倍，共" + totalMoney + "元" : p.Description;
                    int totalCount = p.TotalCount; // 默认份数为方案金额
                    decimal price = string.IsNullOrEmpty(Convert.ToString(p.Price)) ? 1 : p.Price; // 默认每份单价为1元
                    int guarantees = Convert.ToUInt32(p.Guarantees) <= 0 ? 0 : p.Guarantees; //我要保底份数
                    string joinpwd = string.IsNullOrEmpty(Convert.ToString(p.Joinpwd)) ? "" : p.Joinpwd; //认购密码
                    int subscription = Convert.ToUInt32(p.Subscription) <= 0 ? 0 : p.Subscription; //我要认购份数
                    int bonusdeduct = Convert.ToUInt32(p.Bonusdeduct) <= 0 ? 0 : p.Bonusdeduct; //提成比例
                    var togInfo = new Sports_TogetherSchemeInfo()
                    {
                        GameCode = gameCode,
                        GameType = gameType,
                        IssuseNumber = issuseNumber,
                        Amount = amount,
                        AnteCodeList = anteCodeList,
                        BettingCategory = (SchemeBettingCategory)bettingCategory,
                        Attach = attach,
                        PlayType = playType,
                        SchemeSource = schemeSource,
                        TotalMoney = org,
                        TotalMatchCount = matchCount,
                        Title = title,
                        Description = desc,
                        TotalCount = totalCount,
                        Security = (TogetherSchemeSecurity)sercu,
                        Price = price,
                        BonusDeduct = bonusdeduct,
                        Guarantees = guarantees,
                        JoinPwd = joinpwd,
                        Subscription = subscription,
                        ActivityType = ActivityType.NoParticipate,
                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat
                    };
                    var param = new Dictionary<string, object>();
                    param.Add("info", togInfo);
                    param.Add("balancePassword", balancePwd);
                    param.Add("realTotalMoney", totalMoney);
                    param.Add("userid", userid);
                    var hmResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/CreateYouHuaSchemeTogether");
                    return Json(new LotteryServiceResponse
                    {
                        Code = hmResult.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                        Message = hmResult.Message,
                        MsgId = entity.MsgId,
                        Value = hmResult.ReturnValue,
                    });
                }
                else
                {
                    decimal redBagMoney = p.RedBagMoney;
                    if (redBagMoney <= 0)
                        redBagMoney = 0;

                    var opt = new Sports_BetingInfo
                    {
                        ActivityType = ActivityType.NoParticipate,
                        Amount = amount,
                        AnteCodeList = anteCodeList,
                        Attach = attach,
                        BettingCategory = (SchemeBettingCategory)bettingCategory,
                        GameCode = gameCode,
                        GameType = gameType,
                        IssuseNumber = issuseNumber,
                        PlayType = playType,
                        SchemeProgress = TogetherSchemeProgress.Standard,
                        SchemeSource = schemeSource,
                        Security = TogetherSchemeSecurity.Public,
                        SoldCount = 0,
                        TotalMatchCount = matchCount,
                        TotalMoney = org,
                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat
                    };
                    var result = new CommonActionResult();
                    //if (entity.SourceCode == SchemeSource.Iphone)//IOS先是以虚拟订单的方式保存，然后再跳转到网页购买保存订单
                    //{
                    //    var param = new Dictionary<string, object>();
                    //    param.Add("info", opt);
                    //    param.Add("realTotalMoney", totalMoney);
                    //    param.Add("userid", userid);
                    //    result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/VirtualOrderYouHuaBet");
                    //}
                    //else
                    //{
                        var param = new Dictionary<string, object>();
                        param.Add("info", opt);
                        param.Add("password", balancePwd);
                        param.Add("realTotalMoney", totalMoney);
                        param.Add("redBagMoney", redBagMoney);
                        param.Add("userid", userid);
                        result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/YouHuaBet");
                    //}
                    return Json(new LotteryServiceResponse
                    {
                        Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                        Message = result.Message,
                        MsgId = entity.MsgId,
                        Value = result.ReturnValue,
                    });
                }
            }

            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "优化投注失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region 定制跟单(149,151)
        /// <summary>
        /// 定制跟单_149
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> CustomizedTogetherFollower([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string currentUserId = p.CurrentUserId;
                string createUserId = p.CreateUserId;
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                int followerCount = p.FollowerCount;
                decimal followerPercent = p.FollowerPercent;
                int schemeCount = p.SchemeCount;
                decimal maxSchemeMoney = p.MaxSchemeMoney;
                decimal minSchemeMoney = p.MinSchemeMoney;
                decimal stopFollowerMinBalance = p.StopFollowerMinBalance;
                bool cancelWhenSurplusNotMatch = p.CancelWhenSurplusNotMatch;
                bool isEnable = p.IsEnable;
                int cancelNoBonusSchemeCount = p.CancelNoBonusSchemeCount;
                int ruleId = p.RuleId;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("您还未登录，请登录！");
                else if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(createUserId))
                    throw new Exception("用户编号不能为空！");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                TogetherFollowerRuleInfo info = new TogetherFollowerRuleInfo()
                {
                    CreaterUserId = createUserId,
                    FollowerUserId = currentUserId,
                    GameCode = gameCode,
                    GameType = gameType,
                    MaxSchemeMoney = maxSchemeMoney,
                    MinSchemeMoney = minSchemeMoney,
                    SchemeCount = schemeCount,
                    StopFollowerMinBalance = stopFollowerMinBalance,
                    FollowerCount = followerCount,
                    FollowerPercent = followerPercent,
                    CancelNoBonusSchemeCount = cancelNoBonusSchemeCount,
                    CancelWhenSurplusNotMatch = cancelWhenSurplusNotMatch,
                    IsEnable = isEnable,
                };
                var result = new CommonActionResult();
                if (ruleId > 0)
                {
                    var param = new Dictionary<string, object>();
                    param.Add("info", info);
                    param.Add("ruleId", ruleId);
                    param.Add("userid", userid);
                    result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/EditTogetherFollower");
                }
                else
                {
                    var param = new Dictionary<string, object>();
                    param.Add("info", info);
                    param.Add("userid", userid);
                    result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/CustomTogetherFollower");
                }
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = result.ReturnValue,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "定制跟单失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 取消定制跟单_151
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> CancelTogetherFollower([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int ruleId = p.RuleId;
                string userToken = p.UserToken;
                if (ruleId <= 0)
                    throw new Exception("定制跟单编号不能为空");
                else if (string.IsNullOrEmpty(userToken))
                    throw new Exception("您还未登录，请登录！");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var param = new Dictionary<string, object>();
                param.Add("followerId", ruleId);
                param.Add("userid", userid);
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/ExistTogetherFollower");
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = result.ReturnValue,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "取消失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 抄单投注(159)
        /// <summary>
        /// 抄单投注_159
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> Sports_BettingAndChase_BDFX([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string balancePassword = p.BalancePassword;
                string gameCode = p.GameCode == null ? null : ((string)p.GameCode).ToUpper();
                string gameType = p.GameType == null ? null : ((string)p.GameType).ToUpper();
                string playType = p.PlayType == null ? null : ((string)p.PlayType).ToUpper();
                int security = p.Security;
                decimal totalMoney = p.TotalMoney;
                int totalMatchCount = p.TotalMatchCount;
                string userId = p.UserId;//当前登录用户
                decimal commission = p.Commission;//提成比例
                string singleTreasureDeclaration = p.SingleTreasureDeclaration;//宣言
                string schemeId = p.SchemeId;//订单号
                string isChaoDan = p.IsChaoDan;//0：宝单分享；1：抄单；

                var _issuseList = p.IssuseList;
                var _code = p.CodeList;

                if (string.IsNullOrEmpty(gameCode))
                    throw new Exception("彩种不能为空");
                else if (string.IsNullOrEmpty(totalMoney.ToString()))
                    throw new Exception("投注金额不能为空");
                else if (string.IsNullOrEmpty(userId))
                    throw new Exception("你还未登录，请登录");
                else if (string.IsNullOrEmpty(isChaoDan))
                    throw new Exception("投注类型错误");
                string returnValue = string.Empty;
                var sportArray = new string[] { "JCZQ" };
                if (sportArray.Contains(gameCode))
                {
                    #region
                    //足球
                    _code = JsonHelper.Decode(_code);
                    _issuseList = JsonHelper.Decode(_issuseList);
                    if (isChaoDan == "0")
                    {
                        if (_issuseList[0].Amount > 1)
                            throw new Exception("宝单分享只能投注一倍");
                    }
                    var codeList = new Sports_AnteCodeInfoCollection();
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
                        SchemeSource = entity.SourceCode,
                        Security = (TogetherSchemeSecurity)security,
                        TotalMoney = totalMoney,
                        TotalMatchCount = totalMatchCount,
                        IssuseNumber = _issuseList[0].IssuseNumber,
                        SchemeProgress = TogetherSchemeProgress.Finish,
                        ActivityType = ActivityType.NoParticipate,
                        BDFXCommission = commission,
                        SingleTreasureDeclaration = singleTreasureDeclaration,
                        BDFXSchemeId = schemeId,
                        IsRepeat = p.IsRepeat == null ? false : p.IsRepeat,
                    };
                    var param = new Dictionary<string, object>();
                    param.Add("info", info);
                    param.Add("userId", userId);
                    var chaseparam = new Dictionary<string, object>();
                    chaseparam.Add("info", info);
                    chaseparam.Add("password", balancePassword);
                    chaseparam.Add("userId", userId);
                    var result = isChaoDan == "0" ? await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Betting/SaveOrderSportsBetting_DBFX") :
                        await _serviceProxyProvider.Invoke<CommonActionResult>(chaseparam, "api/Betting/Sports_BettingAndChase_BDFX");
                    //var result = isChaoDan == "0" ? WCFClients.GameClient.SaveOrderSportsBetting_DBFX(info, userId) : WCFClients.GameClient.Sports_BettingAndChase_BDFX(info, balancePassword, userId);
                    if (!result.IsSuccess)
                        throw new Exception(result.Message);
                    returnValue = result.ReturnValue;
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = isChaoDan == "0" ? "分享宝单成功" : "抄单成功",
                        MsgId = entity.MsgId,
                        Value = returnValue,
                    });
                }

                #endregion

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = isChaoDan == "0" ? "分享宝单失败" : "抄单失败",
                    MsgId = entity.MsgId,
                    Value = returnValue.TrimEnd('~'),
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 查询合买大厅(202)
        /// <summary>
        /// 从Redis查询出合买订单数据_202
        /// </summary>
        public async Task<IActionResult> QueryTogetherHall([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            
            //改成用接口获取
            var list = await _serviceProxyProvider.Invoke<List<Sports_TogetherSchemeQueryInfo>>(new Dictionary<string, object>(), "api/Betting/QueryTogetherHall");
            //foreach (var item in redisList)
            //{
            //    try
            //    {
            //        if (string.IsNullOrEmpty(item.Value)) continue;
            //        var t = JsonHelper.Deserialize<Sports_TogetherSchemeQueryInfo>(item.ToString());
            //        list.Add(t);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int pageIndex = p.PageIndex;
                int PageSize = p.PageSize;
                string key = p.Key;
                string OrderBy = p.OrderBy;

                //查询列表
                var strPro = "10|20|30";
                var arrProg = strPro.Split('|');
                var query = from s in list
                            where arrProg.Contains(Convert.ToInt32(s.ProgressStatus).ToString())
                              && (s.StopTime >= DateTime.Now)
                              && (string.IsNullOrEmpty(key) || (s.CreateUserId != null && s.CreateUserId.Contains(key)) || (s.SchemeId != null && s.SchemeId.Contains(key)) || (s.CreaterDisplayName != null && s.CreaterDisplayName.Contains(key)) || (s.GameDisplayName != null && s.GameDisplayName.Contains(key)))
                            select s;
                var result = new List<Sports_TogetherSchemeQueryInfo>();
                if (string.IsNullOrEmpty(OrderBy))
                {
                    result = query.Skip(pageIndex * PageSize).Take(PageSize).ToList();
                }
                else if (!string.IsNullOrEmpty(OrderBy) && OrderBy.ToLower() == "masc")
                {
                    result = query.OrderBy(c => c.TotalMoney).Skip(pageIndex * PageSize).Take(PageSize).ToList();
                }
                else if (!string.IsNullOrEmpty(OrderBy) && OrderBy.ToLower() == "mdesc")
                {
                    result = query.OrderByDescending(c => c.TotalMoney).Skip(pageIndex * PageSize).Take(PageSize).ToList();
                }
                else if (!string.IsNullOrEmpty(OrderBy) && OrderBy.ToLower() == "pasc")
                {
                    result = query.OrderBy(c => c.Progress).Skip(pageIndex * PageSize).Take(PageSize).ToList();
                }
                else if (!string.IsNullOrEmpty(OrderBy) && OrderBy.ToLower() == "pdesc")
                {
                    result = query.OrderByDescending(c => c.Progress).Skip(pageIndex * PageSize).Take(PageSize).ToList();
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单明细成功",
                    MsgId = entity.MsgId,
                    Value = result,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        //private static string _cacheRedisHost = string.Empty;
        ///// <summary>
        ///// 缓存Redis的ip
        ///// </summary>
        //public static string CacheRedisHost
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(_cacheRedisHost))
        //                return _cacheRedisHost;
        //            _cacheRedisHost = ConfigHelper.ConfigInfo["CacheRedisHost"].ToString();
        //            return _cacheRedisHost;
        //        }
        //        catch (Exception)
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}

        //private static int _cacheRedisPort = 0;
        ///// <summary>
        ///// 缓存Redis的端口
        ///// </summary>
        //public static int CacheRedisPost
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (_cacheRedisPort > 0)
        //                return _cacheRedisPort;
        //            _cacheRedisPort = int.Parse(ConfigHelper.ConfigInfo["CacheRedisPost"].ToString());
        //            return _cacheRedisPort;
        //        }
        //        catch (Exception)
        //        {
        //            return 6379;
        //        }
        //    }
        //}

        //private static string _cacheRedisPassword = string.Empty;
        ///// <summary>
        ///// 缓存Redis的密码
        ///// </summary>
        //public static string CacheRedisPassword
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(_cacheRedisPassword))
        //                return _cacheRedisPassword;
        //            _cacheRedisPassword = ConfigHelper.ConfigInfo["CacheRedisPassword"].ToString();
        //            return _cacheRedisPassword;
        //        }
        //        catch (Exception)
        //        {
        //            return "123456";
        //        }
        //    }
        //}
        #endregion


    }
}
