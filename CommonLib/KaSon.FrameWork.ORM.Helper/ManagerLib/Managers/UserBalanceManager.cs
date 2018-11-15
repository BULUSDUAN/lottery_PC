using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.PayModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KaSon.FrameWork.ORM.Helper
{
     public class UserBalanceManager:DBbase
    {
        private string GetOperFun(PayType p)
        {
            switch (p)
            {
                case PayType.Payin:
                    return "+=";
                case PayType.Payout:
                    return "-=";
            }
            throw new Exception("PayType类型不正确");
        }
        /// <summary>
        /// 支付到用户余额
        /// </summary>
        public void PayToUserBalance(string userId, params PayDetail[] array)
        {
            if (array.Length <= 0)
                return;

            var setList = new List<string>();
            foreach (var item in array)
            {
                switch (item.AccountType)
                {
                    case AccountType.Bonus:
                        setList.Add(string.Format(" [BonusBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                      //  DB.GetDal<>
                        break;
                    case AccountType.Freeze:
                        setList.Add(string.Format(" [FreezeBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.Commission:
                        setList.Add(string.Format(" [CommissionBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.FillMoney:
                        setList.Add(string.Format(" [FillMoneyBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.Experts:
                        setList.Add(string.Format(" [ExpertsBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.RedBag:
                        setList.Add(string.Format(" [RedBagBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.UserGrowth:
                        setList.Add(string.Format(" [UserGrowth]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.DouDou:
                        setList.Add(string.Format(" [CurrentDouDou]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    default:
                        break;
                }
            }

            var sql = string.Format("update [C_User_Balance] set {0},[Version]+=1 FROM  C_User_Balance where userid='{1}'", string.Join(",", setList), userId);
            DB.CreateSQLQuery(sql).Excute();
        }

        /// <summary>
        /// 查询用户余额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public C_User_Balance QueryUserBalance(string userId)
        {

            return DB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();

        }
        public C_User_Register LoadUserRegister(string userId)
        {
            return DB.CreateQuery<C_User_Register>().Where(p => p.UserId == userId).FirstOrDefault();
        }
        public void UpdateUserBalance(C_User_Balance entity)
        {
            var maxTime = 3;
            var currentTime = 0;
            while (currentTime < maxTime)
            {
                try
                {
                    DB.GetDal<C_User_Balance>().Update(entity);
                    break;
                }
               
                catch (Exception ex)
                {
                    //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    //writer.Write("ERROR_UserBalanceManager", "_UpdateUserBalance", Common.Log.LogType.Error, "更新用户资金问题出错", ex.ToString());
                    //throw new Exception("资金处理错误，请重试", ex);
                }

                currentTime++;
                Thread.Sleep(1000);
            }

        }

        public C_User_Register QueryUserRegister(string userId)
        {
            
            return DB.CreateQuery<C_User_Register>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        public void UpdateUserRegister(C_User_Register user)
        {
            DB.GetDal<C_User_Register>().Update(user);
        }
        public IList<UserBalanceFreezeInfo> QueryUserBalanceFreezeListByUser(string userId, int pageIndex, int pageSize, out int totalCount, out decimal totalMoney)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in DB.CreateQuery<C_User_Balance_FreezeList>()
                        where t.UserId == userId
                        select t;

            totalCount = query.Count();
            if (totalCount > 0)
            {
                totalMoney = query.Sum(u => u.FreezeMoney);
            }
            else
            {
                totalMoney = 0M;
            }
            return query
                .OrderByDescending(u => u.CreateTime)
                .Skip(pageIndex * pageSize)
                .Take(pageSize).ToList().Select(t=> new UserBalanceFreezeInfo
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    OrderId = t.OrderId,
                    FreezeMoney = t.FreezeMoney,
                    Category = (FrozenCategory)Convert.ToInt32(t.Category),
                    Description = t.Description,
                    CreateTime = t.CreateTime,
                }).ToList();
        }
        public C_User_Register GetUserRegister(string userId)
        {
            return DB.CreateQuery<C_User_Register>().Where(u => u.UserId == userId).FirstOrDefault();
        }
        public E_SendMsg_HistoryRecord QueryMsgHistoryRecordByMsgId(long msgId)
        {
            return DB.CreateQuery<E_SendMsg_HistoryRecord>().Where(x=>x.MsgId==msgId).FirstOrDefault();
        }


        public FinanceSettingsInfo_Collection GetFinanceSettingsCollection(string userId, int pageIndex, int pageSize)
        {
            //string sTime = startTime.ToShortDateString() + " 00:00:00";
            //string eTime = endTime.ToShortDateString() + " 23:59:59";
            //StringBuilder strBud = new StringBuilder();
            var addSql = "";
            if (!string.IsNullOrEmpty(userId))
            {
                addSql =" where C_FinanceSettings.UserId='" + userId + "'";
            }
            var totalCount = DB.CreateQuery<C_FinanceSettings>().Count();
            string listSql = SqlModule.AdminModule.First(x => x.Key == "Admin_GetFinanceSettingsCollection").SQL;
            listSql = string.Format(listSql, addSql);
            //var query = CreateOutputQuery(Session.GetNamedQuery("P_Core_Pager"))
            //          .AddInParameter("sqlStr", strSql)
            //          .AddInParameter("currentPageIndex", pageIndex)
            //          .AddInParameter("pageSize", pageSize);
            //var result = query.ToListByPaging(out totalCount);
            var result = DB.CreateSQLQuery(listSql)
                    //.SetString("StartTime", fromDate.ToString("yyyy-MM-dd"))
                    //.SetString("EndTime", toDate.ToString("yyyy-MM-dd"))
                    .SetInt("PageIndex", pageIndex)
                    .SetInt("PageSize", pageSize)
                    .List<FinanceSettingsInfo>();
            FinanceSettingsInfo_Collection collection = new FinanceSettingsInfo_Collection();
            collection.TotalCount = totalCount;
            collection.FinanceSettingsList = result.ToList();
            return collection;
        }


        /// <summary>
        /// 查询财务人员
        /// </summary>
        public string GetCaiWuOperator()
        {
            //string strSql = @"select isnull(u.UserId,'')as UserId,isnull(LoginName,'')as LoginName from C_Auth_Users u left 
            //                join C_Auth_UserRole ur on ur.UserId=u.UserId inner join C_Auth_RoleFunction fr on fr.RoleId=ur.RoleId 
            //                inner join E_Login_Local loc on loc.UserId=u.UserId where fr.FunctionId in ('C101')group by u.UserId,LoginName";
            //var result = Session.CreateSQLQuery(strSql).List();
            StringBuilder strBud = new StringBuilder();
            string strResult = string.Empty;
            string listSql = SqlModule.AdminModule.First(x => x.Key == "Admin_GetCaiWuOperator").SQL;
            var result = DB.CreateSQLQuery(listSql)
                  .List<E_Login_Local>();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    strBud.AppendFormat("{0}-{1}", item.UserId, item.LoginName);
                    strBud.Append("|");
                }
                if (!string.IsNullOrEmpty(strBud.ToString()))
                {
                    strResult = strBud.ToString().Trim('|');
                }
            }
            return strResult;
        }

        /// <summary>
        /// 财务人员设置列表
        /// </summary>
        public FinanceSettingsInfo GetFinanceSettingsByFinanceId(string FinanceId)
        {
            var sql = SqlModule.AdminModule.First(x => x.Key == "Admin_GetFinanceSettingsByFinanceId").SQL;
            var result = DB.CreateSQLQuery(sql)
                              .SetString("FinanceId", FinanceId)
                              .First<FinanceSettingsInfo>();
            //FinanceSettingsInfo info = new FinanceSettingsInfo();
            //if (result != null && result.Count > 0)
            //{
            //    foreach (var item in result)
            //    {
            //        var array = item as object[];
            //        info.FinanceId = Convert.ToInt32(array[0]);
            //        info.UserId = Convert.ToString(array[1]);
            //        info.OperateRank = Convert.ToString(array[2]);
            //        info.OperateType = Convert.ToString(array[3]);
            //        info.MinMoney = Convert.ToDecimal(array[4]);
            //        info.MaxMoney = Convert.ToDecimal(array[5]);
            //        info.UserName = Convert.ToString(array[6]);
            //        info.OperatorName = Convert.ToString(array[7]);
            //        info.CreateTime = Convert.ToDateTime(array[8]);
            //    }
            //}
            return result;
        }

        #region 财务人员增删改
        public List<C_FinanceSettings> GetFinanceUserByUserId(string userId)
        {
            return DB.CreateQuery<C_FinanceSettings>().Where(f => f.UserId == userId).ToList();
        }

        public void AddFinanceSettings(C_FinanceSettings entity)
        {
            DB.GetDal<C_FinanceSettings>().Add(entity);
        }
        public void UpdateFinanceSettings(C_FinanceSettings entity)
        {
            var info= DB.CreateQuery<C_FinanceSettings>().Where(p => p.FinanceId == entity.FinanceId).FirstOrDefault();
            //FinanceSettings info = this.LoadByKey<FinanceSettings>(entity.FinanceId);
            info.UserId = entity.UserId;
            info.OperateType = entity.OperateType;
            info.OperateRank = entity.OperateRank;
            info.MinMoney = entity.MinMoney;
            info.MaxMoney = entity.MaxMoney;
            info.UserId = entity.UserId;
            info.CreateTime = DateTime.Now;
            DB.GetDal<C_FinanceSettings>().Update(info);
        }
        public void DeleteFinanceSettings(string financeId)
        {
            //Session.Clear();
            DB.CreateSQLQuery("delete from C_FinanceSettings where FinanceId=@FinanceId").SetString("FinanceId", financeId).Excute();
        }
        #endregion

        #region 结余
        public UserBalanceHistoryInfoCollection QueryUserBalanceHistoryList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var strStartTime = startTime.Date.ToString("yyyyMMdd");
            var strEndTime = endTime.Date.ToString("yyyyMMdd");
            UserBalanceHistoryInfoCollection collection = new UserBalanceHistoryInfoCollection();
            collection.TotalCount = 0;
            collection.TotalBonusBalance = 0;
            collection.TotalCommissionBalance = 0;
            collection.TotalDouDou = 0;
            collection.TotalExpertsBalance = 0;
            collection.TotalFillMoneyBalance = 0;
            collection.TotalFreezeBalance = 0;
            collection.TotalRedBagBalance = 0;
            collection.TotalUserGrowth = 0;
            var AddSql = "";
            if (!string.IsNullOrEmpty(userId))
            {
                AddSql = " and UserId='" + userId + "'";
            }
            string listSql = SqlModule.AdminModule.First(x => x.Key == "Admin_QueryUserBalanceHistoryList").SQL;
            listSql = string.Format(listSql, AddSql);
            //DB.CreateQuery<C_User_Balance_History>().Where(p=>p.SaveDateTime> strStartTime)
            //var strSql = "select BonusBalance,CommissionBalance,CreateTime,CurrentDouDou,ExpertsBalance,FillMoneyBalance,FreezeBalance,Id,RedBagBalance,UserGrowth,UserId,SaveDateTime from C_User_Balance_History where SaveDateTime>=:startTime and SaveDateTime<=:endTime";
            var list = DB.CreateSQLQuery(listSql)
                             .SetString("StartTime", strStartTime)
                             .SetString("EndTime", strEndTime)
                             .SetInt("PageIndex", pageIndex)
                             .SetInt("PageSize", pageSize)
                             .List<C_User_Balance_History>();
            
            var sumSql= SqlModule.AdminModule.First(x => x.Key == "Admin_QueryUserBalanceHistoryList_Sum").SQL;
            listSql = string.Format(listSql, AddSql);
            collection = DB.CreateSQLQuery(listSql)
                             .SetString("StartTime", strStartTime)
                             .SetString("EndTime", strEndTime)
                             .SetInt("PageIndex", pageIndex)
                             .SetInt("PageSize", pageSize)
                             .First<UserBalanceHistoryInfoCollection>();
            collection.InfoList = list.ToList();
            return collection;
        }
        #endregion

        #region 当天注册人数
        public int QueryRegisterUserCount()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1).Date;
            return DB.CreateQuery<C_User_Register>().Count(p => p.CreateTime >= today && p.CreateTime < tomorrow);
        }
        #endregion
    }
} 
