using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using NHibernate.Criterion;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using Common.Business;
using NHibernate;
using GameBiz.Business;
using GameBiz.Core;
using System.Threading;
using Common;
using Common.Database.ORM;
using Common.Utilities;
using System.Data;

namespace GameBiz.Domain.Managers
{
    public class UserBalanceManager : GameBizEntityManagement
    {
        public void AddUserBalance(UserBalance entity)
        {
            this.Add<UserBalance>(entity);
        }
        public void AddUserRegister(UserRegister entity)
        {
            this.Add<UserRegister>(entity);
        }

        public void AddUserBalanceHistory(UserBalanceHistory entity)
        {
            this.Add<UserBalanceHistory>(entity);
        }

        public void AddUserBalanceReport(UserBalanceReport entity)
        {
            this.Add<UserBalanceReport>(entity);
        }

        //public void AddUserBalanceFreeze(UserBalanceFreeze entity)
        //{
        //    this.Add<UserBalanceFreeze>(entity);
        //}
        //public void DeleteUserBalanceFreeze(UserBalanceFreeze entity)
        //{
        //    this.Delete<UserBalanceFreeze>(entity);
        //}
        //public void UpdateUserBalanceFreeze(UserBalanceFreeze entity)
        //{
        //    this.Update<UserBalanceFreeze>(entity);
        //}
        //public UserBalanceFreeze GetUserBalanceFreezeByOrder(string userId, string orderId)
        //{
        //    Session.Clear();
        //    return this.Session.Query<UserBalanceFreeze>().FirstOrDefault(u => u.UserId == userId && u.OrderId == orderId);
        //}

        public IList<UserBalanceFreezeInfo> QueryUserBalanceFreezeListByUser(string userId, int pageIndex, int pageSize, out int totalCount, out decimal totalMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<UserBalanceFreeze>()
                        where t.UserId == userId
                        select new UserBalanceFreezeInfo
                        {
                            Id = t.Id,
                            UserId = t.UserId,
                            OrderId = t.OrderId,
                            FreezeMoney = t.FreezeMoney,
                            Category = t.Category,
                            Description = t.Description,
                            CreateTime = t.CreateTime,
                        };

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
                .Take(pageSize)
                .ToList();
        }
        public UserRegister LoadUserRegister(string userId)
        {
            return this.LoadByKey<UserRegister>(userId);
        }
        public UserRegister GetUserRegister(string userId)
        {
            Session.Clear();
            return this.Session.Query<UserRegister>().FirstOrDefault(u => u.UserId == userId);
        }
        public void UpdateUserRegister(UserRegister user)
        {
            Update<UserRegister>(user);
        }
        public UserRegister QueryUserRegister(string userId)
        {
            Session.Clear();
            return this.Session.Query<UserRegister>().FirstOrDefault(u => u.UserId == userId);
        }
        public UserRegister QueryUserRegisterByUserName(string userName)
        {
            Session.Clear();
            return this.Session.Query<UserRegister>().FirstOrDefault(u => u.DisplayName == userName);
        }

        public int QueryUserRegisterByIP(string registerIp)
        {
            Session.Clear();
            var query = from t in this.Session.Query<UserRegister>()
                        where t.RegisterIp == registerIp
                        select t;
            return query.Count();
        }
        public List<UserRegister> QueryEnableUserList(int pageIndex, int pageSize)
        {
            Session.Clear();
            var query = from t in this.Session.Query<UserRegister>()
                        where t.IsEnable == true
                        select t;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public void UpdateUserBalance(UserBalance entity)
        {
            var maxTime = 3;
            var currentTime = 0;
            while (currentTime < maxTime)
            {
                try
                {
                    this.Update<UserBalance>(entity);
                    break;
                }
                catch (StaleObjectStateException ex)
                {
                    //throw new Exception("资金处理错误，请重试", ex);
                }
                catch (Exception ex)
                {
                    var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    writer.Write("ERROR_UserBalanceManager", "_UpdateUserBalance", Common.Log.LogType.Error, "更新用户资金问题出错", ex.ToString());
                    //throw new Exception("资金处理错误，请重试", ex);
                }

                currentTime++;
                Thread.Sleep(1000);
            }

        }

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

            var sql = string.Format("update [C_User_Balance] set {0},[Version]+=1 where userid='{1}'", string.Join(",", setList), userId);
            this.Session.CreateSQLQuery(sql).ExecuteUpdate();
        }

        public UserBalance QueryUserBalance(string userId)
        {
            Session.Clear();
            return this.Session.Query<UserBalance>().FirstOrDefault(p => p.UserId == userId);
            //var hql = "FROM UserBalance WHERE UserId = ?";
            //var balance = Session.CreateQuery(hql)
            //    .SetString(0, userId)
            //    .SetCacheable(false)
            //    .SetCacheMode(NHibernate.CacheMode.Get)
            //    .UniqueResult<UserBalance>();

            //return balance;
        }

        public int QueryRegisterUserCount()
        {
            Session.Clear();
            return Session.Query<UserRegister>().Count(p => p.CreateTime >= DateTime.Today && p.CreateTime < DateTime.Today.AddDays(1));
        }

        public dynamic QueryCommonAndBonusMoney()
        {
            Session.Clear();
            //var query = from b in this.Session.Query<UserBalance>()
            //            join r in this.Session.Query<UserRegister>() on b.UserId equals r.UserId
            //            where (!r.IsIgnoreReport.HasValue || !r.IsIgnoreReport.Value) && r.IsEnable
            //            select new
            //            {
            //                CommonMoney = b.CommonBalance,
            //                BonusMoney = b.BonusBalance,
            //            };
            //if (query.Count() == 0)
            //    return new
            //    {

            //        TotalCommonMoney = 0M,
            //        TotalBonusMoney = 0M,
            //    };
            return new
            {
                TotalCommonMoney = 0M,// query.Sum(p => p.CommonMoney),
                TotalBonusMoney = 0M,//query.Sum(p => p.BonusMoney),
            };
        }
        public string GetUserEmailByUserId(string userId)
        {
            Session.Clear();
            var sql = "select Email from E_Authentication_Email where UserId=:userId";
            var query = Session.CreateSQLQuery(sql)
                    .SetString("userId", userId);
            if (query != null && query.List() != null)
                return query.List()[0].ToString();
            return string.Empty;
        }

        #region 财务人员设置

        public void AddFinanceSettings(FinanceSettings entity)
        {
            this.Add<FinanceSettings>(entity);
        }
        public void UpdateFinanceSettings(FinanceSettings entity)
        {
            FinanceSettings info = this.LoadByKey<FinanceSettings>(entity.FinanceId);
            info.UserId = entity.UserId;
            info.OperateType = entity.OperateType;
            info.OperateRank = entity.OperateRank;
            info.MinMoney = entity.MinMoney;
            info.MaxMoney = entity.MaxMoney;
            info.UserId = entity.UserId;
            info.CreateTime = DateTime.Now;
            this.Update<FinanceSettings>(info);
        }
        public void DeleteFinanceSettings(string financeId)
        {
            Session.Clear();
            Session.CreateSQLQuery("delete from C_FinanceSettings where FinanceId=:FinanceId").SetString("FinanceId", financeId).UniqueResult();

        }
        public List<FinanceSettings> GetFinanceUserByUserId(string userId)
        {
            Session.Clear();
            return Session.Query<FinanceSettings>().Where(f => f.UserId == userId).ToList();
            //FinanceSettingsInfo_Collection collection = new FinanceSettingsInfo_Collection();
            //string strSql = "select isnull(C_FinanceSettings.UserId,'')as UserId, isnull(C_FinanceSettings.OperateRank,'')as OperateRank, isnull(C_FinanceSettings.OperateType,'')as OperateType from C_FinanceSettings where UserId=:UserId ";
            //var result = Session.CreateSQLQuery(strSql)
            //                  .SetString("UserId", userId)
            //                  .List();
            //if (result != null && result.Count > 0)
            //{
            //    foreach (var item in result)
            //    {
            //        var array=item as object[];
            //        FinanceSettingsInfo info = new FinanceSettingsInfo();
            //        info.UserId = array[0].ToString();
            //        info.OperateRank = array[1].ToString();
            //        info.OperateType = array[2].ToString();
            //        collection.FinanceSettingsList.Add(info);
            //    }
            //}
            //return collection;
        }
        public bool IsExistFinance(string userId, string operateType)
        {
            Session.Clear();
            var query = from s in Session.Query<FinanceSettings>() where s.UserId == userId && s.OperateType == operateType select s;
            if (query != null && query.ToList().Count > 0)
                return true;
            return false;
        }
        public FinanceSettings GetFinanceSettingsByFinanceId(int financeId)
        {
            return this.LoadByKey<FinanceSettings>(financeId);
        }
        public FinanceSettingsInfo_Collection GetFinanceSettingsCollection(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
            string sTime = startTime.ToShortDateString() + " 00:00:00";
            string eTime = endTime.ToShortDateString() + " 23:59:59";
            StringBuilder strBud = new StringBuilder();
            if (!string.IsNullOrEmpty(userId))
            {
                strBud.Append(" where C_FinanceSettings.UserId='" + userId + "' ");
            }
            string strSql = "SELECT     isnull(C_FinanceSettings.FinanceId,0)as FinanceId, isnull(C_FinanceSettings.UserId,'')as UserId, isnull(C_FinanceSettings.OperateRank,'')as OperateRank, isnull(C_FinanceSettings.OperateType,'')as OperateType,cast(C_FinanceSettings.minmoney as nvarchar)+'—'+cast(C_FinanceSettings.Maxmoney as nvarchar) as FinanceMoney, isnull(E_Login_Local.LoginName,'')as LoginName, isnull(loc.LoginName,'') AS Operator, isnull(C_FinanceSettings.CreateTime,'')as CreateTime FROM C_FinanceSettings INNER JOIN E_Login_Local ON C_FinanceSettings.UserId = E_Login_Local.UserId INNER JOIN  E_Login_Local AS loc ON loc.UserId = C_FinanceSettings.OperatorId  " + strBud + "";
            int totalCount = 0;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Core_Pager"))
                      .AddInParameter("sqlStr", strSql)
                      .AddInParameter("currentPageIndex", pageIndex)
                      .AddInParameter("pageSize", pageSize);
            var result = query.ToListByPaging(out totalCount);
            FinanceSettingsInfo_Collection collection = new FinanceSettingsInfo_Collection();
            collection.TotalCount = totalCount;
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    FinanceSettingsInfo info = new FinanceSettingsInfo();
                    info.FinanceId = Convert.ToInt32(array[0]);
                    info.UserId = Convert.ToString(array[1]);
                    info.OperateRank = Convert.ToString(array[2]);
                    info.OperateType = Convert.ToString(array[3]);
                    info.FinanceMoney = Convert.ToString(array[4]);
                    info.UserName = Convert.ToString(array[5]);
                    info.OperatorName = Convert.ToString(array[6]);
                    info.CreateTime = Convert.ToDateTime(array[7]);
                    collection.FinanceSettingsList.Add(info);
                }
            }
            return collection;
        }

        /// <summary>
        /// 查询财务人员
        /// </summary>
        public string GetCaiWuOperator()
        {
            Session.Clear();
            string strSql = @"select isnull(u.UserId,'')as UserId,isnull(LoginName,'')as LoginName from C_Auth_Users u left 
                            join C_Auth_UserRole ur on ur.UserId=u.UserId inner join C_Auth_RoleFunction fr on fr.RoleId=ur.RoleId 
                            inner join E_Login_Local loc on loc.UserId=u.UserId where fr.FunctionId in ('C101')group by u.UserId,LoginName";
            var result = Session.CreateSQLQuery(strSql).List();
            StringBuilder strBud = new StringBuilder();
            string strResult = string.Empty;
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    strBud.AppendFormat("{0}-{1}", array[0], array[1]);
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
            Session.Clear();
            string strSql = "SELECT     isnull(C_FinanceSettings.FinanceId,0)as FinanceId, isnull(C_FinanceSettings.UserId,'')as UserId, isnull(C_FinanceSettings.OperateRank,'')as OperateRank, isnull(C_FinanceSettings.OperateType,'')as OperateType,isnull(C_FinanceSettings.Minmoney,0)as MinMoney,isnull(C_FinanceSettings.MaxMoney,0)as MaxMoney, isnull(E_Login_Local.LoginName,'')as LoginName, isnull(loc.LoginName,'') AS Operator, isnull(C_FinanceSettings.CreateTime,'')as CreateTime FROM C_FinanceSettings INNER JOIN E_Login_Local ON C_FinanceSettings.UserId = E_Login_Local.UserId INNER JOIN  E_Login_Local AS loc ON loc.UserId = C_FinanceSettings.OperatorId where C_FinanceSettings.FinanceId=:financeId";
            var result = Session.CreateSQLQuery(strSql)
                              .SetString("financeId", FinanceId)
                              .List();
            FinanceSettingsInfo info = new FinanceSettingsInfo();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    info.FinanceId = Convert.ToInt32(array[0]);
                    info.UserId = Convert.ToString(array[1]);
                    info.OperateRank = Convert.ToString(array[2]);
                    info.OperateType = Convert.ToString(array[3]);
                    info.MinMoney = Convert.ToDecimal(array[4]);
                    info.MaxMoney = Convert.ToDecimal(array[5]);
                    info.UserName = Convert.ToString(array[6]);
                    info.OperatorName = Convert.ToString(array[7]);
                    info.CreateTime = Convert.ToDateTime(array[8]);
                }
            }
            return info;
        }

        #endregion

        #region 首页发送短信

        public void AddSendMsgHistoryRecord(SendMsgHistoryRecord entity)
        {
            this.Add<SendMsgHistoryRecord>(entity);
        }
        public void UpdateMsgHistoryRecord(SendMsgHistoryRecord entity)
        {
            this.Update<SendMsgHistoryRecord>(entity);
        }
        public SendMsgHistoryRecord QueryMsgHistoryRecordByMsgId(long msgId)
        {
            return this.GetByKey<SendMsgHistoryRecord>(msgId);
        }
        public List<SendMsgHistoryRecordInfo> GetSendMsgHistoryRecord(string phoneNumber, string IP, int msgType)
        {
            var query = from s in Session.Query<SendMsgHistoryRecord>()
                        where s.PhoneNumber == phoneNumber && s.IP == IP && s.MsgType == msgType && (s.CreateTime >= DateTime.Now.Date && s.CreateTime < DateTime.Now.AddDays(1).Date)
                        select new SendMsgHistoryRecordInfo
                            {
                                IP = s.IP,
                                MsgId = s.MsgId,
                                PhoneNumber = s.PhoneNumber,
                                CreateTime = s.CreateTime,
                                MsgType = s.MsgType,
                            };
            if (query == null || query.ToList().Count <= 0)
                return new List<SendMsgHistoryRecordInfo>();
            return query.ToList<SendMsgHistoryRecordInfo>();
        }
        public string GetMobileNumber(string userId)
        {
            Session.Clear();
            string strSql = "select Mobile from E_Authentication_Mobile where UserId=:userId";
            var result = Session.CreateSQLQuery(strSql).SetString("userId", userId).List();
            if (result != null && result.Count > 0)
                return result[0].ToString();
            return string.Empty;
        }
        public SendMsgHistoryRecord_Collection QueryHistoryRecordCollection(string mobile, string status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
            SendMsgHistoryRecord_Collection collection = new SendMsgHistoryRecord_Collection();
            collection.TotalCount = 0;
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            var query = from h in Session.Query<SendMsgHistoryRecord>()
                        where (mobile == string.Empty || h.PhoneNumber == mobile) && (status == string.Empty || status == h.MsgResultStatus) && (h.SendTime >= startTime && h.SendTime < endTime)
                        select new SendMsgHistoryRecordInfo
                        {
                            CreateTime = h.CreateTime,
                            IP = h.IP,
                            MsgContent = h.MsgContent,
                            MsgId = h.MsgId,
                            MsgResultStatus = h.MsgResultStatus,
                            MsgStatusDesc = h.MsgStatusDesc,
                            MsgType = h.MsgType,
                            PhoneNumber = h.PhoneNumber,
                            SendTime = h.SendTime,
                            UserId = h.UserId,
                            SendNumber = h.SendNumber == null ? 0 : h.SendNumber
                        };
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.ListHistoryRecord = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }

        #endregion

        #region 网站服务项

        public UserSiteService QueryUserSiteServiceById(int Id)
        {
            Session.Clear();
            return Session.Query<UserSiteService>().Where(s => s.Id == Id).FirstOrDefault();
        }
        public void AddUserSiteService(UserSiteService entity)
        {
            Session.Clear();
            this.Add<UserSiteService>(entity);
        }
        public void UpdateUserSiteService(UserSiteService entity)
        {
            Session.Clear();
            this.Update<UserSiteService>(entity);
        }
        public UserSiteService QueryUserSiteServiceByUserId(string userId, ServiceType serviceType)
        {
            Session.Clear();
            return Session.Query<UserSiteService>().Where(s => s.UserId == userId && s.ServiceType == serviceType).FirstOrDefault();
        }

        #endregion

        #region 计算代理销量

        public void AddOCAgentReportSales(OCAgentReportSales entity)
        {
            Session.Clear();
            this.Add<OCAgentReportSales>(entity);
        }
        public void UpdateOCAgentReportSales(OCAgentReportSales entity)
        {
            Session.Clear();
            this.Update<OCAgentReportSales>(entity);
        }
        public OCAgentReportSales QueryOCAgentReportSales(string userId, string gameCode)
        {
            Session.Clear();
            return Session.Query<OCAgentReportSales>().FirstOrDefault(s => s.UserId == userId && s.GameCode == gameCode && s.CreateTime.Date == DateTime.Now.Date);
        }

        #endregion

        #region 用户结余报表

        public List<UserBalanceHistoryInfo> QueryAllUserBalance(string saveDate)
        {
            Session.Clear();
            var query = from b in this.Session.Query<UserBalance>()
                        join r in this.Session.Query<UserRegister>() on b.UserId equals r.UserId
                        where r.UserType == 0
                        orderby b.UserId
                        select new UserBalanceHistoryInfo
                        {
                            BonusBalance = b.BonusBalance,
                            CommissionBalance = b.CommissionBalance,
                            CreateTime = DateTime.Now,
                            CurrentDouDou = b.CurrentDouDou,
                            ExpertsBalance = b.ExpertsBalance,
                            FillMoneyBalance = b.FillMoneyBalance,
                            FreezeBalance = b.FreezeBalance,
                            SaveDateTime = saveDate,
                            RedBagBalance = b.RedBagBalance,
                            UserGrowth = b.UserGrowth,
                            UserId = b.UserId,
                        };
            return query.ToList();
        }

        public UserBalanceReport QueryUserBalanceReport(string saveDate)
        {
            Session.Clear();
            return this.Session.Query<UserBalanceReport>().FirstOrDefault(p => p.SaveDateTime == saveDate);
        }

        public UserBalanceHistoryInfoCollection QueryUserBalanceHistoryList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
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
            var strSql = "select BonusBalance,CommissionBalance,CreateTime,CurrentDouDou,ExpertsBalance,FillMoneyBalance,FreezeBalance,Id,RedBagBalance,UserGrowth,UserId,SaveDateTime from C_User_Balance_History where SaveDateTime>=:startTime and SaveDateTime<=:endTime";
            var query = Session.CreateSQLQuery(strSql)
                             .SetString("startTime", strStartTime)
                             .SetString("endTime", strEndTime).List();
            //var query = from h in Session.Query<UserBalanceHistory>()
            //            where (string.IsNullOrEmpty(userId) || h.UserId == userId) && (Convert.ToInt32(h.SaveDateTime) >= Convert.ToInt32(strStartTime) && Convert.ToInt32(h.SaveDateTime) <= Convert.ToInt32(strEndTime))
            //            select new UserBalanceHistoryInfo
            //            {
            //                BonusBalance = h.BonusBalance,
            //                CommissionBalance = h.CommissionBalance,
            //                CreateTime = h.CreateTime,
            //                CurrentDouDou = h.CurrentDouDou,
            //                ExpertsBalance = h.ExpertsBalance,
            //                FillMoneyBalance = h.FillMoneyBalance,
            //                FreezeBalance = h.FreezeBalance,
            //                Id = h.Id,
            //                RedBagBalance = h.RedBagBalance,
            //                UserGrowth = h.UserGrowth,
            //                UserId = h.UserId,
            //                SaveDateTime = h.SaveDateTime,
            //            };

            if (query != null)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    UserBalanceHistoryInfo info = new UserBalanceHistoryInfo();
                    info.BonusBalance = array[0]==null?0:Convert.ToDecimal(array[0]);
                    info.CommissionBalance = array[1]==null?0:Convert.ToDecimal(array[1]);
                    info.CreateTime = Convert.ToDateTime(array[2]);
                    info.CurrentDouDou = array[3]==null?0:Convert.ToInt32(array[3]);
                    info.ExpertsBalance = array[4]==null?0:Convert.ToDecimal(array[4]);
                    info.FillMoneyBalance = array[5]==null?0:Convert.ToDecimal(array[5]);
                    info.FreezeBalance = array[6]==null?0:Convert.ToDecimal(array[6]);
                    info.Id = Convert.ToInt32(array[7]);
                    info.RedBagBalance = array[8]==null?0:Convert.ToDecimal(array[8]);
                    info.UserGrowth = array[9]==null?0:Convert.ToInt32(array[9]);
                    info.UserId = array[10]==null?string.Empty: array[10].ToString();
                    info.SaveDateTime = array[10] == null ? string.Empty : array[10].ToString();
                    collection.InfoList.Add(info);
                }
                var list = collection.InfoList.Where(s=>(userId==string.Empty||s.UserId==userId));
                collection.TotalCount = list.Count();
                collection.TotalBonusBalance = list.Sum(s => s.BonusBalance);
                collection.TotalCommissionBalance = list.Sum(s => s.CommissionBalance);
                collection.TotalDouDou = list.Sum(s => s.CurrentDouDou);
                collection.TotalExpertsBalance = list.Sum(s => s.ExpertsBalance);
                collection.TotalFillMoneyBalance = list.Sum(s => s.FillMoneyBalance);
                collection.TotalFreezeBalance = list.Sum(s => s.FreezeBalance);
                collection.TotalRedBagBalance = list.Sum(s => s.RedBagBalance);
                collection.TotalUserGrowth = list.Sum(s => s.UserGrowth);
                collection.InfoList = list.Skip(pageIndex * pageSize).Take(pageSize).ToList();


                //var list = query.ToList();
                //collection.TotalCount = list.Count;
                //collection.TotalBonusBalance = list.Sum(s => s.BonusBalance);
                //collection.TotalCommissionBalance = list.Sum(s => s.CommissionBalance);
                //collection.TotalDouDou = list.Sum(s => s.CurrentDouDou);
                //collection.TotalExpertsBalance = list.Sum(s => s.ExpertsBalance);
                //collection.TotalFillMoneyBalance = list.Sum(s => s.FillMoneyBalance);
                //collection.TotalFreezeBalance = list.Sum(s => s.FreezeBalance);
                //collection.TotalRedBagBalance = list.Sum(s => s.RedBagBalance);
                //collection.TotalUserGrowth = list.Sum(s => s.UserGrowth);
                //collection.InfoList = list.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;

        }

        public UserBalanceReportInfoCollection QueryUserBalanceReportList(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
            var strStartTime = startTime.Date.ToString("yyyyMMdd");
            var strEndTime = endTime.Date.ToString("yyyyMMdd");
            UserBalanceReportInfoCollection collection = new UserBalanceReportInfoCollection();
            collection.TotalCount = 0;
            collection.SumBonusBalance = 0;
            collection.SumCommissionBalance = 0;
            collection.SumDouDou = 0;
            collection.SumExpertsBalance = 0;
            collection.SumFillMoneyBalance = 0;
            collection.SumFreezeBalance = 0;
            collection.SumRedBagBalance = 0;
            collection.SumUserGrowth = 0;

            var strSql = "select CreateTime,Id,TotalBonusBalance,TotalCommissionBalance,TotalDouDou,TotalExpertsBalance,TotalFillMoneyBalance,TotalFreezeBalance,TotalRedBagBalance,TotalUserGrowth,SaveDateTime from C_User_Balance_Report where SaveDateTime>=:startTime and SaveDateTime<=:endTime";
            var query = Session.CreateSQLQuery(strSql)
                             .SetString("startTime", strStartTime)
                             .SetString("endTime", strEndTime).List();
            if (query != null)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    UserBalanceReportInfo info = new UserBalanceReportInfo();

                    info.CreateTime = Convert.ToDateTime(array[0]);
                    info.Id = Convert.ToInt32(array[1]);
                    info.TotalBonusBalance = array[2] == null ? 0 : Convert.ToDecimal(array[2]);
                    info.TotalCommissionBalance = array[3] == null ? 0 : Convert.ToDecimal(array[3]);
                    info.TotalDouDou = array[4] == null ? 0 : Convert.ToInt32(array[4]);
                    info.TotalExpertsBalance = array[5] == null ? 0 : Convert.ToDecimal(array[5]);
                    info.TotalFillMoneyBalance = array[6] == null ? 0 : Convert.ToDecimal(array[6]);
                    info.TotalFreezeBalance = array[7] == null ? 0 : Convert.ToDecimal(array[7]);
                    info.TotalRedBagBalance = array[8] == null ? 0 : Convert.ToDecimal(array[8]);
                    info.TotalUserGrowth = array[9] == null ? 0 : Convert.ToInt32(array[9]);
                    info.SaveDateTime = array[10] == null ? string.Empty : array[10].ToString();

                    collection.InfoList.Add(info);
                }
            }

            //var query = from r in Session.Query<UserBalanceReport>()
            //            where (Convert.ToInt32(r.SaveDateTime) >= Convert.ToInt32(strStartTime) && Convert.ToInt32(r.SaveDateTime) <= Convert.ToInt32(strEndTime))
            //            select new UserBalanceReportInfo
            //            {
            //                CreateTime = r.CreateTime,
            //                Id = r.Id,
            //                TotalBonusBalance = r.TotalBonusBalance,
            //                TotalCommissionBalance = r.TotalCommissionBalance,
            //                TotalDouDou = r.TotalDouDou,
            //                TotalExpertsBalance = r.TotalExpertsBalance,
            //                TotalFillMoneyBalance = r.TotalFillMoneyBalance,
            //                TotalFreezeBalance = r.TotalFreezeBalance,
            //                TotalRedBagBalance = r.TotalRedBagBalance,
            //                TotalUserGrowth = r.TotalUserGrowth,
            //                SaveDateTime = r.SaveDateTime,
            //            };
            //if (query != null)
            //{
            //    var list = query.ToList();
            //    collection.TotalCount = list.Count;
            //    collection.SumBonusBalance = list.Sum(s => s.TotalBonusBalance);
            //    collection.SumCommissionBalance = list.Sum(s => s.TotalCommissionBalance);
            //    collection.SumDouDou = list.Sum(s => s.TotalDouDou);
            //    collection.SumExpertsBalance = list.Sum(s => s.TotalExpertsBalance);
            //    collection.SumFillMoneyBalance = list.Sum(s => s.TotalFillMoneyBalance);
            //    collection.SumFreezeBalance = list.Sum(s => s.TotalFreezeBalance);
            //    collection.SumRedBagBalance = list.Sum(s => s.TotalRedBagBalance);
            //    collection.SumUserGrowth = list.Sum(s => s.TotalUserGrowth);
            //    collection.InfoList = list.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //}
            return collection;
        }





        #endregion


        #region 报表
        /// <summary>
        ///  KPI报表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public NewKPIDetailInfoCollection NewKPIDetailList(DateTime startTime, DateTime endTime)
        {
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            NewKPIDetailInfoCollection collection = new NewKPIDetailInfoCollection();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Fund_QueryDayBusinessData"))
                         .AddInParameter("StartTime", startTime)
                         .AddInParameter("EndTime", endTime);
            var dt = query.GetDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    NewKPIDetailInfo info = new NewKPIDetailInfo();
                    info.CreateTime = row["aa0"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(row["aa0"]);
                    info.LoginCount = row["aa1"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa1"]);
                    info.OpenCoutn = row["aa2"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa2"]);
                    info.PalyGameCount = row["aa3"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa3"]);
                    info.WinthdrawCount = row["aa4"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa4"]);
                    info.RechargeCount = row["aa5"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa5"]);
                    info.ActiveCount = row["aa6"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa6"]);
                    info.FristPalyGameCount = row["aa7"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa7"]);
                    info.FristWinthdrawCount = row["aa8"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa8"]);
                    info.FristRechargeCount = row["aa9"] == DBNull.Value ? 0 : Convert.ToInt32(row["aa9"]);
                    collection.InfoList.Add(info);
                }
            }
            return collection;



        }
        /// <summary>
        /// 资金汇总报表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public NewSummaryReportInfoCollection NewSummaryReport(DateTime startTime, DateTime endTime)
        {
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            NewSummaryReportInfoCollection collection = new NewSummaryReportInfoCollection();




            var strSql = "select Category,accounttype,paytype ,SUM(PayMoney) as 'PayMoney'from C_Fund_Detail A ,C_User_Register B where A.CreateTime >=:startTime  and A.CreateTime<=:endTime and A.UserId = B.UserId and B.usertype = 0 group by Category,accounttype,paytype order by Category, accounttype, paytype";
            var query = Session.CreateSQLQuery(strSql)
                  .SetString("startTime", startTime.ToString())
                  .SetString("endTime", endTime.ToString()).List();
            if (query != null)
            {
                foreach (var item in query)
                {
                    var array = item as object[];                   
                    NewSummaryReportInfo info = new NewSummaryReportInfo();
                    info.Category = array[0] == null ? "" : (array[0].ToString());
                    info.accounttype = array[1] == null ? 0 : Convert.ToInt32(array[1]);
                    info.paytype = array[2] == null ? 0 : Convert.ToInt32(array[2]);
                    info.PayMoney = array[3] == null ? 0 : Convert.ToDecimal(array[3]);
                    collection.InfoList.Add(info);
                }
               

            }
            return collection;          


        }

        #endregion
    }
}