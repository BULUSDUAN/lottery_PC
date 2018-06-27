using KaSon.FrameWork.ORM.Provider;

using Kason.Sg.Core.Caching;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Routing.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Kason.Sg.Core.CPlatform.Support;
using Kason.Sg.Core.CPlatform.Support.Attributes;
using Kason.Sg.Core.CPlatform.Transport.Implementation;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using BettingLottery.Service.Model;
using KaSon.FrameWork.Services.ORM;
using KaSon.FrameWork.Services.Enum;
using EntityModel.ORM;
using KaSon.FrameWork.Helper;
using System.Threading;
using BettingLottery.Service.ModuleBaseServices;
using Lottery.Kg.ORM.Helper;
using BettingLottery.Service.IModuleServices;
using EntityModel.Enum;
using EntityModel.CoreModel.BetingEntities;
using EntityModel.CoreModel;

namespace BettingLottery.Service.ModuleServices
{
    [ModuleName("Betting")]
    public class BettingService : KgBaseService
    {
        #region Implementation of IUserService
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
       
        

        public Task<int> GetUserId(string userName)
        {
            //var xid = RpcContext.GetContext().GetAttachment("xid");

            //throw new Exception("错误！");

            //测试容错
           // Thread.Sleep(200000);

            //var T1 = TTest1();
            //var T21 = Test21();
            //var T2 = Test2();
            //var T3 = Test3();

            return Task.FromResult(1);
        }



        /// <summary>
        /// 普通投注
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public LotteryServiceResponse Betting(string Param, SchemeSource SourceCode, string MsgId)
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

                        //var userBalance = WCFClients.GameFundClient.QueryMyBalance(userToken);
                        //if (userBalance == null)
                        //    throw new Exception("未查询到账户信息");
                        //else if ((userBalance.BonusBalance + userBalance.ExpertsBalance + userBalance.FillMoneyBalance + userBalance.RedBagBalance) < totalMoney)
                        //    throw new Exception("您好，目前账户余额不足！");
                        //else if ((userBalance.BonusBalance + userBalance.CommissionBalance + userBalance.ExpertsBalance + userBalance.FillMoneyBalance + userBalance.RedBagBalance) < totalMoney)
                        //    throw new Exception("您好，目前账户余额不足！");
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
                                var result = WCFClients.GameClient.Sports_Betting(info, balancePassword, redBagMoney, userToken);
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
                            return new LotteryServiceResponse
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
                            return new LotteryServiceResponse
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
                    var result = IsSaveOrder == "0" ? WCFClients.GameClient.Sports_Betting(info, balancePassword, redBagMoney, userToken) : WCFClients.GameClient.SaveOrderSportsBetting(info, userToken);
                    if (!result.IsSuccess)
                        throw new Exception(result.Message);
                    returnValue = result.ReturnValue;
                }

                #endregion
            }

            return null;
        }




        #endregion Implementation of IUserService
    }
}
