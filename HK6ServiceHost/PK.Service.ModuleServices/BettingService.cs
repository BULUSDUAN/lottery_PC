using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;

using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Sport;
using System.Threading.Tasks;

using System.IO;
using System.Text;
using EntityModel.ExceptionExtend;

using System.Linq;
using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using KaSon.FrameWork.ORM;
using EntityModel;
using KaSon.FrameWork.Analyzer.Hk6Model;
using KaSon.FrameWork.Common.Utilities;
using PK.Service.IModuleServices;
using HK6.ModuleBaseServices;
using KaSon.FrameWork.ORM.Helper.BJPK;

namespace PK.Service.ModuleServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ModuleName("Betting")]
    public class BettingService : KgBaseService, IBettingService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<BettingService> _Log;
        private readonly Repository _rep;
        private IDbProvider DB = null;
        private IDbProvider LettoryDB = null;
        public BettingService(Repository repository, ILogger<BettingService> log)
        {
            _Log = log;
            this._rep = repository;
            DB = _rep.LDB.Init("MySql.Default", false);
            LettoryDB = _rep.DB.Init("SqlServer.Default", false);
        }
        /// <summary>
        /// 普通订单缓存数据
        /// </summary>
        private static Dictionary<string, HK6Sports_BetingInfo> _bettingListInfo = new Dictionary<string, HK6Sports_BetingInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="redBagMoney"></param>
        /// <param name="userid"></param>
        /// <returns></returns>

        public Task<CommonActionResult> Betting(HK6Sports_BetingInfo info)
        {
            string balancePassword = info.BalancePassword;
            CommonActionResult cresult = new CommonActionResult();
            cresult.IsSuccess = true;
            decimal totalmoney = 0M;
            decimal basemoney = 0M;
            List<blast_bet_orderdetail> orderDetailList = new List<blast_bet_orderdetail>();
            blast_member LoginUser = new blast_member();
            try
            {
                var userManager = new UserBalanceManager();
                var user = userManager.LoadUserRegister(info.userId);
                if (!user.IsEnable)
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "用户已禁用";
                    return Task.FromResult(cresult);
                }


                #region 校验是否重复购买
                HK6Sports_BetingInfo betinfo = null;
                _bettingListInfo.TryGetValue(info.userId, out betinfo);
                if (betinfo != null)
                {
                    var tf = DateTime.Now - betinfo.CurrentBetTime;
                    if (info.TotalMoney == betinfo.TotalMoney
                        && info.issueNo == betinfo.issueNo
                        && tf.Minutes<15
                        )
                    {
                        var isRePeat = true;
                        foreach (var item in info.orderList)
                        {
                            var p = (from b in betinfo.orderList
                                     where b.content == item.content
                                             && b.unitPrice == item.unitPrice
                                     select b).ToList();
                            if (p.Count == 0)
                            {
                                isRePeat = false;
                                break;
                            }
                        }

                        if (isRePeat)
                        {
                            cresult.IsSuccess = false;
                            cresult.Code = 300;
                            cresult.StatuCode = 300;
                            cresult.Message = "重复购买";
                            return Task.FromResult(cresult);
                        }

                    }

                }
                #endregion

                #region 校验追期期号是否合法，号码格式是否正确，号码是否合法，玩法是否可用
                int issueNo = info.issueNo ;
                CommonActionResult result = new DataHelper(DB).GetGames(2);
                if (!result.IsSuccess)
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "期号无效无法购买";
                    return Task.FromResult(cresult);
                }
                string  iNo = (result.Value as IssueReuslt).actionNo ;
                if (iNo !=issueNo+"")
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "期号无效无法购买";
                    return Task.FromResult(cresult);
                }
                var codeList = DB.CreateQuery<blast_antecode>().Where(b => b.typeid == 3).ToList();
                
                foreach (var item in info.orderList)
                {
                    blast_bet_orderdetail orderdetail = new blast_bet_orderdetail()
                    {
                        playId = item.playId
                    };
                    //BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(orderdetail, DB);
                    //var bol = winHelper.CheckCode(item.content, codeList, item.playId);
                    var p = codeList.Where(b => b.AnteCode == item.content).FirstOrDefault();
                    if (p == null)
                    {
                        cresult.IsSuccess = false;
                        cresult.Code = 300;
                        cresult.StatuCode = 300;
                        cresult.Message = "投注号码不正确";
                        return Task.FromResult(cresult);
                    }

                }


                #endregion




                #region 校验金额是否足够
                // decimal unitPrice = 0M;
                foreach (var item in info.orderList)
                {
                    totalmoney = totalmoney + item.unitPrice;



                }
                //追号期号
                //foreach (var item in info.planList)
                //{
                //    totalmoney = totalmoney + basemoney * (item.multiple == 0 ? 1 : item.multiple);
                //}
                //if (info.planList.Count <= 0)
                //{
                //    totalmoney = basemoney;
                //}
                //LoginUser = DB.CreateQuery<blast_member>().Where(b => b.userId == info.userId).FirstOrDefault();

                //if (LoginUser == null || LoginUser.gameMoney < totalmoney)
                //{
                //    cresult.IsSuccess = false;
                //    cresult.Message = "金额不足，请充值";
                //    cresult.Code = 300;
                //    cresult.StatuCode = 300;
                //    return Task.FromResult(cresult);
                //}


                //   var LoginUser = DB.LettoryDB<E_Login_Local>();
                #endregion
           
                
               
                //校验投注订单合法信息，包括金额 玩法，号码，
                DB.Begin();
                LettoryDB.Begin();
                #region 创建订单
                string prefix = "CHASE_PLV_" + "PK10_";
                var keyLine = prefix + UsefullHelper.UUID() + info.issueNo.ToString();// UsefullHelper.ConvertDateTimeToInt(DateTime.Now);

                //  string keyLine = info.IssuseNumberList.Count > 1 ? BusinessHelper.GetChaseLotterySchemeKeyLine(info.GameCode) : string.Empty;
                blast_bet_order border = new blast_bet_order()
                {
                    redBagMoney = info.redBagMoney,
                    SchemeId = keyLine,
                    issueNo = info.issueNo.ToString(),
                    betNum = info.orderList.Count(),
                    anteCodeNum = info.planList.Count(),
                    betTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    totalMoney = totalmoney,
                    userId = info.userId,
                    SchemeSource = info.SchemeSource,
                    displayName = LoginUser.displayName,
                    winAnteCodeStop = info.winStop,
                    typeid=3
                    


                };
              
                int pindex = 0;
                HK6Sports_PlanInfo plan = new HK6Sports_PlanInfo()
                {
                    multiple = 1
                };
                var playedList = DB.CreateQuery<blast_played>().Where(b => b.typeid == 3).ToList();
                foreach (var item in info.orderList)
                {

                    //是否包含多个号码
                    //if (item.content.Contains("|"))
                    //{
                    //    var arr = item.content.Split('|');
                    //    var t = codeList.Where(b => arr.Contains(b.AnteCode) && b.playid == item.playId);
                    //    // var oddslist = t.Select(b => b.odds).ToArray();
                    //   // OddsArr = String.Join("|", t.Select(b => b.odds).ToArray());

                    //   // codeName = String.Join(",", t.Select(b => b.displayName).ToArray());
                    //}

                    blast_played p = playedList.Where(b => b.playId == item.playId).FirstOrDefault();
                    string content = item.content;
                   var code= codeList.Where(b=>b.AnteCode== content).FirstOrDefault();
                    blast_bet_orderdetail orderDetail = new blast_bet_orderdetail()
                    {
                        SchemeId = keyLine,
                        AnteCodes = item.content,
                        playId = item.playId,
                        playName = p.name,
                       AnteCodesName = code.displayName,
                        SchemeSource = info.SchemeSource,
                        OddsArr = p.Odds+"",
                        typeid=3,
                        GameType="BJPK10",
                        userId = info.userId,
                        issueNo = info.issueNo.ToString(),
                      //  OddsArr = OddsArr,
                        ProgressStatus = 0,
                        BeiSu = 0,
                        unitPrices=item.unitPrices,
                        unitPrice = item.unitPrice,
                        CreateTime = DateTime.Now,
                        anteSchemeId =prefix+"Sub_" + UsefullHelper.UUID() + "_" + info.issueNo.ToString() + "_" + item.playId

                    };
                    //BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(orderDetail, DB);
                    //orderDetail.AnteCodesName = winHelper.BuildCodes(item.content);
                    //orderDetail.OddsArr = winHelper.GetOddsArr(item.content, codeList, item.playId);
                    orderDetailList.Add(orderDetail);

                }
                DB.GetDal<blast_bet_order>().Add(border);


                foreach (var orderdetail in orderDetailList)
                {
                    DB.GetDal<blast_bet_orderdetail>().Add(orderdetail);
                }
                #endregion

                #region 扣款

                // decimal gameMoney = (LoginUser.gameMoney - totalmoney);
                //DB.GetDal<blast_member>().Update(b => new blast_member()
                //{
                //    gameMoney = b.gameMoney - totalmoney
                //}, b => b.userId == info.userId);
                var msg = string.Format("{0}第{1}期投注", "PLVPK10", border.issueNo);
                if (border.redBagMoney > 0M)
                {
                    var fundManager = new FundManager();
                    var percent = fundManager.QueryRedBagUseConfig("PLVPK10");
                    //使用红包百分比
                    var maxUseMoney = totalmoney * percent / 100;
                    if (border.redBagMoney > maxUseMoney)
                        throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0:N2}%，即{1:N2}元", percent, maxUseMoney));
                    //红包支付
                    cresult= BusinessHelper.Payout_RedBag_To_EndByDb(LettoryDB, BusinessHelper.FundCategory_Betting, info.userId, keyLine, border.redBagMoney, msg, "Bet", balancePassword);

                    if (!cresult.IsSuccess)
                    {
                        throw new LogicException(cresult.Message);
                    }
                }
                //其它账户支付
                cresult= BusinessHelper.Payout_To_EndByDb(LettoryDB, BusinessHelper.FundCategory_Betting, info.userId, keyLine, totalmoney - border.redBagMoney
                    , msg, "Bet", balancePassword);
                if (!cresult.IsSuccess)
                {
                    throw new LogicException(cresult.Message);
                }

                #endregion
                DB.Commit();
                LettoryDB.Commit();
                #region 是否执行插件

                //不需要执行插件

                #endregion
                //返回
                cresult.IsSuccess = true;
                cresult.Message = "投注成功";
              
                cresult.ReturnValue = keyLine;
                cresult.Value = orderDetailList.OrderByDescending(b => b.CreateTime).Select(b => b.anteSchemeId).ToList();

                //清空重复验证缓存数据
                _bettingListInfo[info.userId] = info;
            }
            catch (LogicException ex)
            {
                cresult.IsSuccess = false;
                cresult.Code = 300;
                cresult.Message = ex.Message;
                cresult.StatuCode = 300;
                cresult.ReturnValue = ex.ToString();


                LettoryDB.Rollback();
                DB.Rollback();
                // throw new Exception("订单投注异常，请重试 ", ex);
            }
            catch (Exception ex)
            {
                cresult.Code = 500;
                cresult.Message = "系统错误";
                cresult.StatuCode = 500;
                cresult.ReturnValue = ex.ToString();


                LettoryDB.Rollback();
                DB.Rollback();
                // throw new Exception("订单投注异常，请重试 ", ex);
            }
        
            finally
            {
                //释放资源
                DB.Dispose();
                LettoryDB.Dispose();
            }



            return Task.FromResult(cresult);
        }
    }
}
