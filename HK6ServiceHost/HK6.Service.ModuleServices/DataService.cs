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
using KaSon.FrameWork.Common.Hk6;
using KaSon.FrameWork.Services.ORM;
using KaSon.FrameWork.Services.Enum;
using KaSon.FrameWork.ORM.Helper.BJPK;

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
            DB = _rep.LDB.Init("MySql.Default", true);
            LettoryDB = _rep.DB.Init("SqlServer.Default", true);
        }

        public Task<CommonActionResult> PlayCategory()
        {
            CommonActionResult result = new CommonActionResult();

            try
            {
                var mblist = DB.CreateQuery<blast_played_group>().Where(b => b.enable == true && b.typeid==1).ToList();
                var mPlayedList = DB.CreateQuery<blast_played>().Where(b => b.enable == true && b.typeid == 1).ToList();

                foreach (var item in mblist)
                {
                    item.PlayedList = mPlayedList.Where(b => b.groupId == item.groupId).ToList();
                }
                result.IsSuccess = true;
                result.Value = mblist;
            }
            catch (Exception ex)
            {
                LettoryDB.Rollback();
                DB.Rollback();
                result.Message = "系统错误";
                result.IsSuccess = false;

                result.Code = 500;
                result.StatuCode = 500;
                result.ReturnValue = ex.ToString();
            }
            finally
            {
                DB.Dispose();
                LettoryDB.Dispose();

            }

            // LettoryDB.GetDal<C_Game_Transfer>().Add(ctransfer);

            return Task.FromResult(result);


        }
        public Task<CommonActionResult> ReChargeRecord(string userId, int sType)
        {
            CommonActionResult result = new CommonActionResult();
            int lhc = (int)MGGameType.LHC;
            int status = (int)FillMoneyStatus.Success;
            int Recharge = (int)GameTransferType.Recharge;
            if (sType != 0)
            {
                Recharge = (int)GameTransferType.Withdraw;
            }
            try
            {
                var mb = LettoryDB.CreateQuery<C_Game_Transfer>().Where(b => b.UserId == userId
           && b.GameType == lhc && b.Status == status && b.TransferType == Recharge).ToList();
                result.Value = mb.OrderByDescending(b => b.UpdateTime).ToList();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LettoryDB.Rollback();
                DB.Rollback();
                result.Message = "系统错误";
                result.IsSuccess = false;

                result.Code = 500;
                result.StatuCode = 500;
                result.ReturnValue = ex.ToString();
            }
            finally
            {
                DB.Dispose();
                LettoryDB.Dispose();

            }

            // LettoryDB.GetDal<C_Game_Transfer>().Add(ctransfer);

            return Task.FromResult(result);
        }
        public Task<CommonActionResult> ReCharge(string userId, string userDisplayName, decimal Money)
        {
            //  CommonActionResult result = new CommonActionResult();
            // var omb = DB.CreateQuery<C_User_Balance>().Where(b => b.UserId == userId).FirstOrDefault();

            //if (omb.FillMoneyBalance < Money)
            //{
            //    result.Message = "余额不知请充值";
            //    result.IsSuccess = false;
            //    return Task.FromResult(result);
            //}
            CommonActionResult result = new CommonActionResult();
            var orderId = BettingHelper.GetGameTransferId();
            var msg = string.Format("游戏充值订单号{0}", orderId);
            var mb = DB.CreateQuery<blast_member>().Where(b => b.userId == userId).FirstOrDefault();
            var userRegister = LettoryDB.CreateQuery<C_User_Register>().Where(p => p.UserId == userId).FirstOrDefault();// balanceManager.QueryUserBalance(userId);
            //var userBalance = LettoryDB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();// balanceManager.QueryUserBalance(userId);

            DB.Begin();
            LettoryDB.Begin();



            try
            {
                result = BusinessHelper.Payout_To_FrozenByDB(LettoryDB, BusinessHelper.FundCategory_GameRecharge, userId, orderId, Money, msg, "GameTransfer", "");
                if (!result.IsSuccess)
                {
                    LettoryDB.Rollback();
                    DB.Rollback();
                    DB.Dispose();
                    LettoryDB.Dispose();
                    return Task.FromResult(result);
                }
                if (mb == null)
                {
                    //创建一个用户
                    blast_member tmb = new blast_member()
                    {
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        gameMoney = Money,
                        userId = userId,

                        displayName = userRegister.DisplayName,


                    };

                    DB.GetDal<blast_member>().Add(tmb);
                }
                else
                {
                    DB.GetDal<blast_member>().Update(b => new blast_member
                    {
                        displayName = userRegister.DisplayName,
                        gameMoney = b.gameMoney + Money,
                        updateTime = DateTime.Now
                    }, b => b.userId == userId);

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

                result.Code = 500;
                result.StatuCode = 500;
                result.ReturnValue = ex.ToString();
            }
            finally
            {
                DB.Dispose();
                LettoryDB.Dispose();

            }

            return Task.FromResult(result);
        }
        public Task<CommonActionResult> GameWithdraw(string userId, string userDisplayName, decimal Money)
        {

            // var omb = DB.CreateQuery<C_User_Balance>().Where(b => b.UserId == userId).FirstOrDefault();

            CommonActionResult result = new CommonActionResult();

            var orderId = BettingHelper.GetGameTransferId();
            var mb = DB.CreateQuery<blast_member>().Where(b => b.userId == userId).FirstOrDefault();
            if (mb.gameMoney < Money)
            {
                result.Message = $"提款金币不足金额：{Money}";
                result.IsSuccess = false;

                result.Code = 300;
                result.StatuCode = 300;
                return Task.FromResult(result);
            }
            DB.Begin();
            LettoryDB.Begin();



            try
            {
                result = BusinessHelper.Payin_To_BalanceByDB(LettoryDB, AccountType.Bonus, BusinessHelper.FundCategory_GameWithdraw, userId, orderId, Money,
                string.Format("游戏提款成功，金额：{0:N2}元存入账号", Money));
                if (!result.IsSuccess)
                {
                    LettoryDB.Rollback();
                    DB.Rollback();
                    DB.Dispose();
                    LettoryDB.Dispose();
                    return Task.FromResult(result);
                }
                if (mb == null)
                {
                    //创建一个用户
                    result.Message = $"系统错误,用户不存在{userId}";
                    result.IsSuccess = false;
                    //result.Message = "系统错误";
                    //result.IsSuccess = false;
                    result.Code = 300;
                    result.StatuCode = 300;
                    return Task.FromResult(result);
                }
                else
                {
                    DB.GetDal<blast_member>().Update(b => new blast_member
                    {
                        gameMoney = b.gameMoney - Money,
                        updateTime = DateTime.Now
                    }, b => b.userId == userId);

                }


                C_Game_Transfer ctransfer = new C_Game_Transfer()
                {
                    OrderId = orderId,
                    RequestMoney = Money,
                    RequestTime = DateTime.Now,
                    Status = (int)FillMoneyStatus.Success,
                    UserId = userId,
                    TransferType = (int)GameTransferType.Withdraw,
                    UserDisplayName = userDisplayName,
                    GameType = (int)MGGameType.LHC
                };
                LettoryDB.GetDal<C_Game_Transfer>().Add(ctransfer);

                LettoryDB.Commit();
                DB.Commit();
                result.Message = "提款金币成功";
                result.IsSuccess = true;


            }
            catch (Exception ex)
            {
                LettoryDB.Rollback();
                DB.Rollback();
                result.Message = "系统错误";
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
                result.ReturnValue = ex.ToString();
            }
            finally
            {
                DB.Dispose();
                LettoryDB.Dispose();

            }

            return Task.FromResult(result);
        }
        public Task<CommonActionResult> GameTransfer(string userId, int PageIndex)
        {
            CommonActionResult result = new CommonActionResult();
            int lhc = (int)MGGameType.LHC;

            //PageIndex
            var query = LettoryDB.CreateQuery<C_Game_Transfer>();

            var wlist = new List<WhereField>();
            wlist.Add(new WhereField()
            {
                Field = "UserId",
                Value = userId,
                WhereType = WhereType.Equal

            });
            //wlist.Add(new WhereField()
            //{
            //    Field = "GameType",
            //    Value = lhc.ToString(),
            //    WhereType = WhereType.Equal

            //});

            var slist = new List<SortField>();
            slist.Add(new SortField()
            {
                Field = "UpdateTime",
                IsASC = false

            });
            QueryArgs qargs = new QueryArgs()
            {
                PageIndex = PageIndex,
                PageSize = 20,
                WhereFields = wlist,
                SortFields = slist

            };

            var data = LettoryDB.CreateComQuery().Query<C_Game_Transfer>(query, qargs);

            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = data.Data;
            result.IsSuccess = true;
            if (data.RowCount <= 0)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
            }


            return Task.FromResult(result);

        }

        public Task<CommonActionResult> GetCurrentIssuseNo()
        {
            CommonActionResult result = new CommonActionResult();
            result.IsSuccess = true;
            var mb = new DataServiceHelper(DB).GetissueNo();
           // CommonActionResult result = new DataHelper(DB).GetGames(20);
            result.Value = mb;
            return Task.FromResult(result);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<CommonActionResult> UserInfo(string userId)
        {
            CommonActionResult result = new CommonActionResult();
            var mb = DB.CreateQuery<blast_member>().Where(b => b.userId == userId).FirstOrDefault();
            result.Value = mb;
            result.IsSuccess = true;
            if (mb == null)
            {
                //result.IsSuccess = true;
                result.Value = new blast_member();
            }

            return Task.FromResult(result);
        }
        public Task<CommonActionResult> PlayInfo()
        {
            CommonActionResult result = new CommonActionResult();
            //playGroup
            var pmb = DB.CreateQuery<blast_played>().Where(b => b.typeid ==1).ToList();
            var mb = DB.CreateQuery<blast_antecode>().Where(b => b.typeid == 1).ToList();
            var q = from b in mb
                    group b by b.playid into g
                    select g;

            List<playGroup> pgroupList = new List<playGroup>();
            int pid = 0;
            var pp = new blast_played();
            var antecodeList = new List<blast_antecode>();
            foreach (var item in q)
            {
                pid = item.Key;
                pp = pmb.Where(b => b.playId == pid).FirstOrDefault();
                antecodeList = item.ToList<blast_antecode>().OrderBy(b => b.sort).ToList();
                switch (pp.name.Trim())
                {
                    case "正肖":
                    case "特肖":
                    case "一肖":
                        foreach (var item1 in antecodeList)
                        {
                            item1.CodeContent = string.Join(",", SXHelper.ScodeArr(int.Parse(item1.AnteCode)));
                        }
                        break;
                    default:
                        break;
                }
                playGroup pg = new playGroup()
                {
                    CodeList = antecodeList,
                    Key = item.Key + "",
                    Name = pp.name
                };
                pgroupList.Add(pg);
            }


            result.Value = pgroupList;
            result.IsSuccess = true;

            return Task.FromResult(result);
        }
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<CommonActionResult> OrderInfo(string userId, int PageIndex)
        {
            CommonActionResult result = new CommonActionResult();
            //PageIndex
            var query = DB.CreateQuery<blast_bet_orderdetail>();

            var wlist = new List<WhereField>();
            wlist.Add(new WhereField()
            {
                Field = "userId",
                Value = userId,
                WhereType = WhereType.Equal

            });

            var slist = new List<SortField>();
            slist.Add(new SortField()
            {
                Field = "Id",
                IsASC = false

            });
            QueryArgs qargs = new QueryArgs()
            {
                PageIndex = PageIndex,
                PageSize = 20,
                WhereFields = wlist,
                SortFields = slist

            };

            var data = DB.CreateComQuery().Query<blast_bet_orderdetail>(query, qargs);

            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = data.Data;
            result.IsSuccess = true;
            if (data.RowCount <= 0)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
            }
            //if (list)
            //{
            //    result.ReturnObj = new blast_member();
            //}


            return Task.FromResult(result);
        }


        public Task<CommonActionResult> OrderDetail(string oId)
        {
            CommonActionResult result = new CommonActionResult();
            //PageIndex
            var orderdetail = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.anteSchemeId == oId).FirstOrDefault();


            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = orderdetail;
            result.IsSuccess = true;

            //if (list)
            //{
            //    result.ReturnObj = new blast_member();
            //}


            return Task.FromResult(result);
        }
        public Task<CommonActionResult> HostoryData(string userId, int PageIndex)
        {
            CommonActionResult result = new CommonActionResult();
            int lhc = (int)MGGameType.LHC;

            //PageIndex
            var query = DB.CreateQuery<blast_data>();

            var wlist = new List<WhereField>();
            //wlist.Add(new WhereField()
            //{
            //    Field = "UserId",
            //    Value = userId,
            //    WhereType = WhereType.Equal

            //});
            wlist.Add(new WhereField()
            {
                Field = "typeid",
                Value = "1",
                WhereType = WhereType.Equal

            });

            var slist = new List<SortField>();
            slist.Add(new SortField()
            {
                Field = "kjtime",
                IsASC = false

            });
            QueryArgs qargs = new QueryArgs()
            {
                PageIndex = PageIndex,
                PageSize = 20,
                WhereFields = wlist,
                SortFields = slist

            };

            var data = DB.CreateComQuery().Query<blast_data>(query, qargs);

            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = data.Data;
            result.IsSuccess = true;
            if (data.RowCount <= 0)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
            }
            else
            {
                //生肖 
                List<blast_data> LIST = data.Data as List<blast_data>; ;
                foreach (var item in LIST)
                {
                    item.NameList = SXHelper.SCodeNameArr(item.kjdata);
                }
            }


            return Task.FromResult(result);

        }

    }
}
