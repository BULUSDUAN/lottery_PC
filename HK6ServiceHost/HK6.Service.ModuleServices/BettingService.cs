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

using KaSon.FrameWork.Common.Utilities;
using EntityModel.Domain.Entities.HK6;

namespace HK6.ModuleBaseServices
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
            DB = _rep.LDB.Init("MySql.Default", true);
            LettoryDB = _rep.DB.Init("SqlServer.Default", true);
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
            CommonActionResult cresult = new CommonActionResult();
            decimal totalmoney = 0M;
            decimal basemoney = 0M;
            List<blast_bet_orderdetail> orderDetailList = new List<blast_bet_orderdetail>();
            blast_member LoginUser = new blast_member();
            try
            {


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

                #region 校验追期期号是否合法，加倍是否合法

                int issueNo=info.issueNo ;
                var issue = DB.CreateQuery<blast_lhc_time>().Where(b => b.actionNo == issueNo).FirstOrDefault();

               
                if (issue == null || DateTime.Now> DateTime.Parse(issue.actionTime.ToShortDateString()).AddHours(21).AddMinutes(10))
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "期号无效无法购买.";
                    return Task.FromResult(cresult);
                }
                if ((info.issueNo+"").Length !=7)
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "期号格式无效无法购买.格式如:2019007";
                    return Task.FromResult(cresult);
                }

                var mb = new DataServiceHelper(DB).GetissueNo();

                if (mb == null)
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "无法查询期号无法购买";
                    return Task.FromResult(cresult);
                }
                else if (mb.actionNo != info.issueNo)
                {
                    cresult.IsSuccess = false;
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    cresult.Message = "期号无效无法购买";
                    return Task.FromResult(cresult);
                } 
                //追号期号 无效 请重新下注
                foreach (var item in info.planList)
                {

                    if (item.issueNo < int.Parse(issue.actionNo+""))
                    {
                        cresult.IsSuccess = false;
                        cresult.Code = 300;
                        cresult.StatuCode = 300;
                        cresult.Message = "期号错误无法购买";
                        return Task.FromResult(cresult);
                    }

                }
                //期号是否连续校验


                #endregion

                #region 校验金额是否足够

                foreach (var item in info.orderList)
                {
                    basemoney = basemoney + item.unitPrice;
                }
                //追号期号
                foreach (var item in info.planList)
                {
                    totalmoney = totalmoney + basemoney * (item.multiple == 0 ? 1 : item.multiple);
                }
                if (info.planList.Count <= 0)
                {
                    totalmoney = basemoney;
                }
                LoginUser = DB.CreateQuery<blast_member>().Where(b => b.userId == info.userId).FirstOrDefault();

                if (LoginUser == null || LoginUser.gameMoney < totalmoney)
                {
                    cresult.IsSuccess = false;
                    cresult.Message = "金额不足，请充值";
                    cresult.Code = 300;
                    cresult.StatuCode = 300;
                    return Task.FromResult(cresult);
                }


                //   var LoginUser = DB.LettoryDB<E_Login_Local>();
                #endregion
           
                var playedList = DB.CreateQuery<blast_played>().ToList();
                var codeList = DB.CreateQuery<blast_antecode>().ToList();
                //校验投注订单合法信息，包括金额 玩法，号码，
                DB.Begin();
                #region 创建订单
                string prefix = "CHASE" + "HK6";
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
                    typeid=1,
                    userId = info.userId,
                    SchemeSource = info.SchemeSource,
                    displayName = LoginUser.displayName,
                    winAnteCodeStop = info.winStop



                };
              
                int pindex = 0;
                HK6Sports_PlanInfo plan = new HK6Sports_PlanInfo()
                {
                    multiple = 1
                };
                foreach (var item in info.orderList)
                {

                    if (info.planList.Count > 0)
                    {
                        plan = info.planList[pindex];
                    }

                    pindex++;
                    blast_played p = playedList.Where(b => b.playId == item.playId).FirstOrDefault();
                    var tempp = codeList.Where(b => b.AnteCode == item.content && b.playid == item.playId).FirstOrDefault();
                    if (p == null || tempp==null)
                    {
                        DB.Rollback();
                        cresult.IsSuccess = false;
                        cresult.Message = "不存在该玩法";
                        cresult.Code = 300;
                        cresult.StatuCode = 300;
                        return Task.FromResult(cresult);
                    }
                  
                    string OddsArr = tempp.odds + "";
                    string codeName = tempp.displayName + "";
                    //是否包含多个号码
                    if (item.content.Contains("|"))
                    {
                        var arr = item.content.Split('|');
                        var t = codeList.Where(b => arr.Contains(b.AnteCode) && b.playid == item.playId);
                        // var oddslist = t.Select(b => b.odds).ToArray();
                        OddsArr = String.Join("|", t.Select(b => b.odds).ToArray());

                        codeName = String.Join(",", t.Select(b => b.displayName).ToArray());
                    }


                    blast_bet_orderdetail orderDetail = new blast_bet_orderdetail()
                    {
                        SchemeId = keyLine,
                        AnteCodes = item.content,
                        playId = item.playId,
                        playName = p.name,
                        AnteCodesName = codeName,
                        SchemeSource = info.SchemeSource,
                        typeid=1,
                        // Odds = p.Odds,
                        GameType="HK6",
                        userId = info.userId,
                        issueNo = info.issueNo.ToString(),
                        OddsArr = OddsArr,
                        ProgressStatus = 0,
                        BeiSu = plan.multiple,
                        unitPrice = item.unitPrice,
                        CreateTime = DateTime.Now,
                        anteSchemeId = "CDHK6" + UsefullHelper.UUID() + "_" + info.issueNo.ToString() + "_" + item.playId

                    };
                    BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(orderDetail, DB);
                    orderDetail.AnteCodes = winHelper.BuildCodes(item.content);

                    orderDetailList.Add(orderDetail);

                }
                DB.GetDal<blast_bet_order>().Add(border);


                foreach (var orderdetail in orderDetailList)
                {
                    DB.GetDal<blast_bet_orderdetail>().Add(orderdetail);
                }
                #endregion

                #region 扣款
                decimal aftermoney = LoginUser.gameMoney - totalmoney;
                blast_money_detail mdetail = new blast_money_detail()
                {
                    beforeMoney = LoginUser.gameMoney,
                    afterMoney = aftermoney,
                    totalMoney = totalmoney,
                    moneyType = (int)PayType.Payout,
                    category = moneyType.HK6_Buy,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    isAuto = 1,
                    orderId = border.SchemeId,
                    remark = "HK6投注订单号"+ border.SchemeId+",是主订单号",
                    userId= LoginUser.userId,
                    user_diaplayName= LoginUser.displayName
                };
                DB.GetDal<blast_money_detail>().Add(mdetail);
                // decimal gameMoney = (LoginUser.gameMoney - totalmoney);
                DB.GetDal<blast_member>().Update(b => new blast_member()
                {
                    gameMoney = b.gameMoney - totalmoney
                }, b => b.userId == info.userId);


               
                #endregion
                DB.Commit();

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
