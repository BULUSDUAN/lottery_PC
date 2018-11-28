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
        /// 检查普通订单频繁投注
        /// </summary>
        //private string CheckGeneralRepeatBetting(string currUserId, HK6Sports_BetingInfo info)
        //{
        //    try
        //    {
        //        //todo:备用 info.IsSubmit = false;
        //        if (_bettingListInfo == null || !_bettingListInfo.ContainsKey(currUserId))
        //        {
        //            info.CurrentBetTime = DateTime.Now;
        //            _bettingListInfo.Add(currUserId, info);
        //            return string.Empty;
        //        }
        //        var cacheInfo = _bettingListInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.SchemeSource == info.SchemeSource && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
        //        if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
        //        {
        //            _bettingListInfo.Remove(currUserId);
        //            info.CurrentBetTime = DateTime.Now;
        //            _bettingListInfo.Add(currUserId, info);
        //            return string.Empty;
        //        }
        //        if (!info.Equals(cacheInfo.Value))
        //        {
        //            //不重复
        //            _bettingListInfo.Remove(currUserId);
        //            info.CurrentBetTime = DateTime.Now;
        //            _bettingListInfo.Add(currUserId, info);
        //            return string.Empty;
        //        }
        //        //投注内容相同
        //        if (info.IsRepeat)
        //        {
        //            _bettingListInfo.Remove(currUserId);
        //            info.CurrentBetTime = DateTime.Now;
        //            _bettingListInfo.Add(currUserId, info);
        //            return string.Empty;
        //        }
        //        var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
        //        if (timeSpan.TotalSeconds > 5)
        //        {
        //            //大于间隔时间
        //            _bettingListInfo.Remove(currUserId);
        //            info.CurrentBetTime = DateTime.Now;
        //            _bettingListInfo.Add(currUserId, info);
        //            return string.Empty;
        //        }
        //        return "Repeat";
        //    }
        //    catch (Exception)
        //    {
        //        _bettingListInfo.Clear();
        //        return string.Empty;
        //    }
        //}


        public Task<CommonActionResult> Betting(HK6Sports_BetingInfo info, string password, decimal redBagMoney, string userid)
        {
            CommonActionResult cresult = new CommonActionResult();
            try
            {
                var playedList = DB.CreateQuery<blast_played>().ToList();
                //校验投注订单合法信息，包括金额 玩法，号码，
                #region 校验投注订单合法信息，包括金额 玩法，号码，
                cresult = Hk6_BaseAnalyzer.BetingOrderCheck(info, playedList);
                if (!cresult.IsSuccess)
                {
                    return Task.FromResult(cresult);
                }
                #endregion

                #region 校验追期期号是否合法，加倍是否合法

                var issue = DB.CreateQuery<blast_lhc_time>().Where(b=>b.actionNo==info.issueNo).FirstOrDefault();
                if (issue==null || issue.actionTime < DateTime.Now)
                {
                    cresult.IsSuccess = false;
                    cresult.Message = "期号错误无法购买";
                    return Task.FromResult(cresult);
                }
                //
                foreach (var item in info.planList)
                {

                    if (item.issueNo <= issue.actionNo)
                    {
                        cresult.IsSuccess = false;
                        cresult.Message = "期号错误无法购买";
                        return Task.FromResult(cresult);
                    }

                }

                #endregion

                #region 校验金额是否足够

                var LoginUser = LettoryDB.CreateQuery<E_Login_Local>();

                decimal totalMoney = 0M;
                foreach (var item in info.orderList)
                {
                    totalMoney = totalMoney + item.unitPrice;

                }

                foreach (var item in info.planList)
                {
                    totalMoney++;//加上追期钱
                    totalMoney = totalMoney * item.multiple;//加倍钱
                }
                totalMoney = totalMoney + redBagMoney;



                #endregion

                //if (redBagMoney > 0M)
                //{
                //    var fundManager = new FundManager();
                //    var percent = fundManager.QueryRedBagUseConfig(info.GameCode);
                //    var maxUseMoney = info.TotalMoney * percent / 100;
                //    if (redBagMoney > maxUseMoney)
                //        throw new LogicException(string.Format("本彩种只允许使用红包为订单总金额的{0:N2}%，即{1:N2}元", percent, maxUseMoney));
                //    //红包支付
                //    BusinessHelper.Payout_RedBag_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine, redBagMoney, msg, "Bet", balancePassword);
                //}
                ////其它账户支付
                //BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_Betting, userId, keyLine, totalBetMoney - redBagMoney
                //    , msg, "Bet", balancePassword);

            }
            catch (Exception)
            {

                throw;
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
