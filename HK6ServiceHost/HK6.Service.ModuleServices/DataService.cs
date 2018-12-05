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

namespace HK6.ModuleBaseServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ModuleName("Data")]
    public class DataService : KgBaseService, IDataService
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
        public DataService(Repository repository)
        {
           // _Log = log;
            this._rep = repository;
            DB = _rep.LDB.Init("MySql.Default",true);
            LettoryDB = _rep.DB.Init("SqlServer.Default", true);
        }

        public Task<CommonActionResult> ReCharge(string userId, string userDisplayName, decimal Money)
        {
            CommonActionResult result = new CommonActionResult();
            // var omb = DB.CreateQuery<C_User_Balance>().Where(b => b.UserId == userId).FirstOrDefault();

            //if (omb.FillMoneyBalance < Money)
            //{
            //    result.Message = "余额不知请充值";
            //    result.IsSuccess = false;
            //    return Task.FromResult(result);
            //}
            var orderId = BettingHelper.GetGameTransferId();
            var msg = string.Format("游戏充值订单号{0}", orderId);
            var mb = DB.CreateQuery<blast_member>().Where(b => b.userId == userId).FirstOrDefault();
            DB.Begin();
            LettoryDB.Begin();

          

            try
            {
                BusinessHelper.Payout_To_FrozenByDB(LettoryDB, BusinessHelper.FundCategory_GameRecharge, userId, orderId, Money, msg, "GameTransfer", "");
                if (mb == null)
                {
                    //创建一个用户
                    blast_member tmb = new blast_member()
                    {
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        gameMoney = Money,
                        userId = userId,
                         


                    };

                    DB.GetDal<blast_member>().Add(tmb);
                }
                else {
                    DB.GetDal<blast_member>().Update(b=>new blast_member
                    {
                         gameMoney=b.gameMoney+Money,
                         updateTime=DateTime.Now
                    },b=>b.userId==userId);

                }
               
                
                C_Game_Transfer ctransfer = new C_Game_Transfer()
                {
                    TransferType = (int)GameTransferType.Recharge,
                    GameType = (int)MGGameType.LHC,
                    Status = (int)FillMoneyStatus.Success,
                    UserId = userId,
                    RequestTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    OrderId = orderId,
                    UserDisplayName = userDisplayName,
                    RequestMoney = Money

                };
                LettoryDB.GetDal<C_Game_Transfer>().Add(ctransfer);

                LettoryDB.Commit();
                DB.Commit();
                result.Message = "充值成功";
                result.IsSuccess = true;
              

            }
            catch (Exception ex)
            {
                LettoryDB.Rollback();
                DB.Rollback();
                result.Message = "系统错误";
                result.IsSuccess = false;
                result.ReturnValue = ex.ToString();
            }
            finally {
                DB.Dispose();
                LettoryDB.Dispose();

            }

            return Task.FromResult(result);
        }
    }
}
