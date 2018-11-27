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
    [ModuleName("WinSum")]
    public class WinSumService : KgBaseService, IWinSumService
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
        public WinSumService(Repository repository, ILogger<BettingService> log)
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
        /// 开奖日期，开奖时间
        /// </summary>
        /// <param name="winDate"></param>
        /// <param name="winNum"></param>
        /// <returns></returns>
       public Task<CommonActionResult> Sum(string winDate, string winNum) {

            //
            try
            {

              var list=  DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.issueDate == winDate).ToList<blast_bet_orderdetail>();

                DB.Begin();
                foreach (blast_bet_orderdetail item in list)
                {
                    //开始结算
                    string playIdStr = "";
                    switch (playIdStr)
                    {
                        case "TM"://特码

                            string tm = winNum.Split('|')[1];
                            if (tm.Trim()==item.AnteCodes.Trim())
                            {
                                //故中奖
                                //计算中奖号码
                                decimal Odds = decimal.Parse(item.OddsArr);

                                decimal winMoney = item.unitPrice * Odds;

                                int orderDetailId = item.id;

                                DB.GetDal<blast_bet_orderdetail>().Update(b=>new blast_bet_orderdetail {
                                     winNumber=winNum,
                                     BonusAwardsMoney=winMoney,
                                     updateTime=DateTime.Now,
                                    

                                },b=>b.id== orderDetailId);

                                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                                {
                                    winNumber = winNum,
                                    BonusAwardsMoney = winMoney,
                                    updateTime = DateTime.Now,
                                    BonusStatus=2  //为中奖状态

                                }, b => b.id == orderDetailId);

                                //添加用户金币 加钱  blast_lhc_member

                                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                                {
                                  gameMoney=b.gameMoney+ winMoney


                                }, b => b.id == item.userId);

                            }



                            break;
                        case "ZM"://正码



                            break;

                        default:
                            break;
                    }




                }
                DB.Commit();


            }
            catch (Exception)
            {

                throw;
            }
            finally {

                DB.Dispose();
            }


           return Task.FromResult(new CommonActionResult());
        }
    }
}
