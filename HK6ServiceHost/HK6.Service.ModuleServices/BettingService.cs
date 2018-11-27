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


using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using KaSon.FrameWork.ORM;
using EntityModel;
using KaSon.FrameWork.Analyzer.Hk6Model;
using KaSon.FrameWork.Common.Utilities;

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
            DB = _rep.DB.Init("MySql.Default",true);
            LettoryDB = _rep.DB.Init("SQL SERVER.Default", true);
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
            try
            {
                var playedList = DB.CreateQuery<blast_played>().ToList();
                var codeList = DB.CreateQuery<blast_lhc_antecode>().ToList();
                //校验投注订单合法信息，包括金额 玩法，号码，
                #region 校验投注订单合法信息，包括金额 玩法，号码，
                //cresult = Hk6_BaseAnalyzer.BetingOrderCheck(info, playedList);
                //if (!cresult.IsSuccess)
                //{
                //    return Task.FromResult(cresult);
                //}
                #endregion

                #region 校验追期期号是否合法，加倍是否合法

                var issue = DB.CreateQuery<blast_lhc_time>().Where(b=>b.actionNo==info.issueNo).FirstOrDefault();
                if (issue==null || issue.actionTime < DateTime.Now)
                {
                    cresult.IsSuccess = false;
                    cresult.Message = "期号错误无法购买";
                    return Task.FromResult(cresult);
                }
                //追号期号 无效 请重新下注
                foreach (var item in info.planList)
                {

                    if (item.issueNo <= issue.actionNo)
                    {
                        cresult.IsSuccess = false;
                        cresult.Message = "期号错误无法购买";
                        return Task.FromResult(cresult);
                    }

                }
                //期号是否连续校验


                #endregion

                #region 校验金额是否足够
                decimal totalmoney = 0M;
                decimal basemoney = 0M;
                foreach (var item in info.orderList)
                {
                    basemoney = basemoney + item.unitPrice;
                }
                //追号期号
                foreach (var item in info.planList)
                {
                    totalmoney = totalmoney+ basemoney * (item.multiple==0?1: item.multiple);
                }
                var LoginUser = DB.CreateQuery<blast_lhc_member>().Where(b => b.userId == info.userId).FirstOrDefault();
               
                if (LoginUser == null || LoginUser.gameMoney<totalmoney)
                {
                    cresult.IsSuccess = false;
                    cresult.Message = "金额不足，请充值";
                    return Task.FromResult(cresult);
                }


              //   var LoginUser = DB.LettoryDB<E_Login_Local>();
                #endregion
                DB.Begin();
                #region 创建订单
                string prefix = "CHASE" + "HK6";
                var keyLine = prefix + UsefullHelper.UUID()+ info.issueNo.ToString()+ UsefullHelper.ConvertDateTimeToInt(DateTime.Now);
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
                    username = LoginUser.loginName,
                    winAnteCodeStop=info.winStop==true?1:0,
                  


                };
                List<blast_bet_orderdetail> orderDetailList = new List<blast_bet_orderdetail>();
                foreach (var item in info.orderList)
                {
                    blast_played p = playedList.Where(b => b.id == item.playedId).FirstOrDefault();
                    if (p==null)
                    {
                        cresult.IsSuccess = false;
                        cresult.Message = "不存在该玩法";
                        return Task.FromResult(cresult);
                    }
                    string AnteCodes = "";
                    //是否包含多个号码
                    if (item.content.Contains(","))
                    {
                        var arr = item.content.Split(',');
                        var oddslist=codeList.Where(b => arr.Contains(b.AnteCode) && b.playid == p.id).Select(b=>b.odds).ToArray();
                        AnteCodes = String.Join(",", oddslist);
                    }
                    blast_bet_orderdetail orderDetail = new blast_bet_orderdetail()
                    {
                        SchemeId = keyLine,
                        AnteCodes = item.content,
                        playId = p.id,
                        Odds = p.Odds,
                        issueNo = info.issueNo.ToString(),
                        OddsArr = AnteCodes,
                        ProgressStatus =0,

                        CreateTime = DateTime.Now

                    };
                    orderDetailList.Add(orderDetail);

                }
                DB.GetDal<blast_bet_order>().Add(border);

                DB.GetDal<blast_bet_orderdetail>().BulkAdd(orderDetailList);

                #endregion

                #region 扣款

                decimal gameMoney = (LoginUser.gameMoney - totalmoney);
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member()
                {
                    gameMoney = gameMoney
                }, b => b.userId == info.userId);

                #endregion
                DB.Commit();

                #region 是否执行插件

                #endregion
                //返回
                cresult.IsSuccess = true;
                cresult.Message = "投注成功";
                cresult.ReturnValue = keyLine;
            }
            catch (Exception ex)
            {
             
                DB.Rollback();
                throw new Exception("订单投注异常，请重试 ", ex);
            }
            finally {
                //释放资源
                DB.Dispose();
                LettoryDB.Dispose();
            }


            //校验追号是否合法，加倍是否合法



            //校验是否重复投注


            //获取用户信息

            //校验金额是否足够

            //生成订单

            //扣款


            //using (DB)
            //{
            //数据库处理
            //重复投注保护


            //}

            return Task.FromResult(cresult);
        }
    }
}
