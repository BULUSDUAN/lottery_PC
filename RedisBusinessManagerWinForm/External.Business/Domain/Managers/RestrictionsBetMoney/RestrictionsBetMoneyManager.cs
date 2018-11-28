using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using External.Business.Domain.Entities.RestrictionsBetMoney;
using External.Core.RestrictionsBetMoney;
using GameBiz.Business;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using External.Domain.Entities.RestrictionsBetMoney;

namespace External.Business.Domain.Managers.RestrictionsBetMoney
{
    public class RestrictionsBetMoneyManager : GameBizEntityManagement
    {
        public void AddRestrictionsBetMoney(RestrictionsBetMoneys entity)
        {
            this.Add<RestrictionsBetMoneys>(entity);
        }

        public void DeleteRestrictionsBetMoney(RestrictionsBetMoneys entity)
        {
            this.Delete<RestrictionsBetMoneys>(entity);
        }

        public void UpdateRestrictionsBetMoney(RestrictionsBetMoneys entity)
        {
            this.Update<RestrictionsBetMoneys>(entity);
        }

        public void AddRestrictionsUsers(RestrictionsUsers entity)
        {
            this.Add<RestrictionsUsers>(entity);
        }
        public void DeleteRestrictionsUsers(RestrictionsUsers entity)
        {
            this.Delete<RestrictionsUsers>(entity);
        }

        public void UpdateRestrictionsUsers(RestrictionsUsers entity)
        {
            this.Update<RestrictionsUsers>(entity);
        }

        public RestrictionsBetMoneys QueryRestrictionsBetMoneyById(int id)
        {
            return this.Session.Query<RestrictionsBetMoneys>().FirstOrDefault(p => p.Id == id);
        }

        public RestrictionsBetMoneys QueryRestrictionsBetMoneyByUserId(string userId)
        {
            return this.Session.Query<RestrictionsBetMoneys>().OrderByDescending(p => p.CreateTime).FirstOrDefault(p => p.UserId == userId&&p.CreateTime.Date==DateTime.Now.Date);
        }

        public RestrictionsUsers QueryRestrictionsUsersByUserId(string userId)
        {
            return this.Session.Query<RestrictionsUsers>().FirstOrDefault(p => p.UserId == userId);
        }
        /// <summary>
        /// 查询限制投注金额列表
        /// </summary>
        public List<RestrictionsBetMoneyInfo> QueryRestrictionsBetMoneyList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            //var query = from r in this.Session.Query<>()where(userId == string.Empty || r.UserId == userId) && r.CreateTime > startTime && r.CreateTime < endTime
            //                select  new RestrictionsBetMoneyInfo
            //                {
            //                    Id=r.Id,
            //                    CreateTime = r.CreateTime,
            //                    MaxRestrictionsMoney = r.MaxMoney
            //                };



            var query = from r in this.Session.Query<RestrictionsBetMoneys>()
                        where r.UserId == userId && r.CreateTime > startTime && r.CreateTime < endTime
                        select new RestrictionsBetMoneyInfo
                            {

                                Id = r.Id,
                                CreateTime = r.CreateTime,
                                MaxRestrictionsMoney = r.MaxRestrictionsMoney,
                                TodayBetMoney = r.TodayBetMoney,
                                UserId = r.UserId
                            };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询限制投注金额用户列表
        /// </summary>
        public List<RestrictionsUsersInfo> QueryRestrictionsUsersList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {

            //Session.Clear();
            ////var aa = from r in this.Session.Query<RestrictionsUsers>()where(userId == string.Empty || r.UserId == userId) && r.CreateTime > startTime && r.CreateTime < endTime;
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append("where 1=1");
            //if (!string.IsNullOrEmpty(userId))
            //    strSql.Append(" and UserId='" + userId + "'");
            //strSql.Append(string.Format("AND CreateTime >=N'{0:yyyy-MM-dd}' AND CreateTime <=N'{1:yyyy-MM-dd}'", startTime, endTime.AddDays(1)));
            //string sql = "select Id,UserId,MaxMoney,CreateTime from E_RestrictionsUser " + strSql;
            //var result = Session.CreateSQLQuery(sql).List();
            //List<RestrictionsUsersInfo> list = new List<RestrictionsUsersInfo>();
            //if (result != null)
            //{
            //    foreach (var item in result)
            //    {
            //        var array = item as object[];
            //        RestrictionsUsersInfo info = new RestrictionsUsersInfo();
            //        info.Id = array[0] == null ? 0 : Convert.ToInt32(array[0]);
            //        info.UserId = array[1] == null ? string.Empty : array[1].ToString();
            //        info.MaxMoney = array[2] == null ? 0M : Convert.ToDecimal(array[2]);
            //        info.CreateTime = array[3] == null ? Convert.ToDateTime(null) : Convert.ToDateTime(array[3]);
            //        list.Add(info);
            //    }
            //}
            //totalCount = list.Count();
            //return list;


            Session.Clear();
            totalCount = 0;
            var query = from r in Session.Query<RestrictionsUsers>()
                        join u in Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where (userId == string.Empty || u.UserId == userId) && (u.CreateTime >= startTime.Date && u.CreateTime < endTime.AddDays(1).Date)
                        select new RestrictionsUsersInfo
                        {
                            Id = r.Id,
                            UserId = u.UserId,
                            MaxMoney = r.MaxMoney,
                            CreateTime = r.CreateTime,
                            RegistTime=u.CreateTime,
                            UserDisplayName=u.DisplayName,
                        };
            if (query != null && query.ToList().Count > 0)
            {
                totalCount = query.Count();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return new List<RestrictionsUsersInfo>();

        }
        public decimal GetTotalBetMoney()
        {
            Session.Clear();
            //var result = Session.Query<RestrictionsBetMoneys>().Where(s => s.CreateTime.Date == DateTime.Now.Date).ToList();
            var result = Session.Query<OrderDetail>().Where(s => s.CreateTime.Date == DateTime.Now.Date).ToList();
            if (result == null && result.Count <= 0)
                return 0M;
            return result.Sum(s => s.TotalMoney);

            //var result = Session.Query<RestrictionsBetMoneys>().Where(s => s.CreateTime.Date == DateTime.Now.Date).ToList();
            //if (result != null && result.Count > 0)
            //    return result.Sum(s => s.TodayBetMoney);
            //return 0;

        }
        public decimal GetCurrDayTotalBetMoneyByUserId(string userId)
        {
            Session.Clear();
            //var result = Session.Query<RestrictionsBetMoneys>().Where(s => s.CreateTime.Date == DateTime.Now.Date).ToList();
            var result = Session.Query<OrderDetail>().Where(s => s.CreateTime.Date == DateTime.Now.Date && s.UserId == userId).ToList();
            if (result == null && result.Count <= 0)
                return 0M;
            return result.Sum(s => s.TotalMoney);

            //var result = Session.Query<RestrictionsBetMoneys>().Where(s => s.UserId == userId && s.CreateTime.Date == DateTime.Now.Date).ToList();
            //if (result != null && result.Count > 0)
            //    return result.Sum(s => s.TodayBetMoney);
            //return 0;
        }
        public void BatchEditRestrictionsUser(string userIds, decimal money)
        {
            Session.Clear();
            var array = userIds.Split(',');
            string sql = "update E_RestrictionsUser set MaxMoney=:maxmoney where UserId in (:userIds)";
            Session.CreateSQLQuery(sql).SetDecimal("maxmoney", money).SetParameterList("userIds", array.ToArray()).UniqueResult();
        }

        #region 投注白名单

        public void AddBetWhiteList(BetWhiteList entity)
        {
            this.Add<BetWhiteList>(entity);
        }
        public void EditBetWhiteList(params BetWhiteList[] arrayEntity)
        {
            this.Update<BetWhiteList>(arrayEntity);
        }
        public void DeleteBetWhiteList(BetWhiteList entity)
        {
            this.Delete<BetWhiteList>(entity);
        }
        public BetWhiteList GetBetWhiteListByUserId(string userId)
        {
            return Session.Query<BetWhiteList>().FirstOrDefault(s => s.UserId == userId);
        }
        public bool IsBet(string userId)
        {
            Session.Clear();
            var res= Session.Query<BetWhiteList>().FirstOrDefault(s => s.UserId == userId&&s.IsEnable==true);
            if (res != null)
                return true;
            return false;
        }
        public BetWhiteListInfo_Collection QueryBetWhiteListCollection(string userId, string userDisplayName, string isHm, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
            BetWhiteListInfo_Collection collection = new BetWhiteListInfo_Collection();
            collection.TotalCount = 0;
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select WhiteLlistId,UserId,UserDisplayName,isnull(IsEnable,0),RegisterTime,isnull(ExpansionOne,'')as ExpansionOne,isnull(ExpansionTwo,0) as ExpansionTwo,CreateTime,isnull(IsOpenHeMai,0)as IsOpenHeMai,Isnull(IsOpenBDFX,0)IsOpenBDFX,isnull(IsSingleScheme,0) IsSingleScheme from E_Bet_WhiteList where 1=1");
            if (!string.IsNullOrEmpty(userId) || !string.IsNullOrEmpty(userDisplayName))
            {
                if (!string.IsNullOrEmpty(userId) && !BusinessHelper.CheckSQLCondition(userId))
                    strSql.Append(" and UserId='" + userId + "' ");
                if (!string.IsNullOrEmpty(userDisplayName) && !BusinessHelper.CheckSQLCondition(userDisplayName))
                    strSql.Append(" and UserDisplayName='" + userDisplayName + "' ");
            }
            if (!string.IsNullOrEmpty(isHm) && !BusinessHelper.CheckSQLCondition(isHm))
            {
                strSql.Append(" and IsOpenHeMai=" + isHm + " ");
            }
            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userDisplayName))
                strSql.Append(" and CreateTime>='" + startTime + "' and CreateTime<'" + endTime + "'");
            var result = Session.CreateSQLQuery(strSql.ToString()).List();
            if (result != null)
            {
                collection.TotalCount = result.Count;
                foreach (var item in result)
                {
                    var array = item as object[];
                    BetWhiteListInfo info = new BetWhiteListInfo();
                    info.WhiteLlistId = Convert.ToInt32(array[0]);
                    info.UserId = array[1].ToString();
                    info.UserDisplayName = array[2].ToString();
                    info.IsEnable = Convert.ToBoolean(array[3]);
                    info.RegisterTime = Convert.ToDateTime(array[4]);
                    info.ExpansionOne = array[5].ToString();
                    info.ExpansionTwo = Convert.ToDecimal(array[6]);
                    info.CreateTime = Convert.ToDateTime(array[7]);
                    info.IsOpenHeMai = Convert.ToBoolean(array[8]);
                    info.IsOpenBDFX = Convert.ToBoolean(array[9]);
                    info.IsSingleScheme = Convert.ToBoolean(array[10]);
                    collection.BetInfoList.Add(info);
                }
            }

            #region 暂时屏蔽
            //if (string.IsNullOrEmpty(isHm))
            //{
            //    var query = from b in Session.Query<BetWhiteList>()
            //                where (userId == string.Empty || b.UserId == userId) && (userDisplayName == string.Empty || b.UserDisplayName == userDisplayName) && (b.CreateTime >= startTime && b.CreateTime < endTime) && (b.IsOpenHeMai == false || b.IsOpenHeMai == true)
            //                select new BetWhiteListInfo
            //                {
            //                    WhiteLlistId = b.WhiteLlistId,
            //                    UserId = b.UserId,
            //                    UserDisplayName = b.UserDisplayName,
            //                    IsEnable = b.IsEnable,
            //                    RegisterTime = b.RegisterTime,
            //                    ExpansionOne = b.ExpansionOne,
            //                    ExpansionTwo = b.ExpansionTwo,
            //                    CreateTime = b.CreateTime,
            //                    IsOpenHeMai = b.IsOpenHeMai == null ? false : b.IsOpenHeMai,
            //                };
            //    if (query != null && query.ToList().Count > 0)
            //    {
            //        collection.TotalCount = query.Count();
            //        collection.BetInfoList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //    }
            //}
            //else if (isHm == "1")
            //{
            //    var query = from b in Session.Query<BetWhiteList>()
            //                where (userId == string.Empty || b.UserId == userId) && (userDisplayName == string.Empty || b.UserDisplayName == userDisplayName) && (b.CreateTime >= startTime && b.CreateTime < endTime) && (b.IsOpenHeMai == true)
            //                select new BetWhiteListInfo
            //                {
            //                    WhiteLlistId = b.WhiteLlistId,
            //                    UserId = b.UserId,
            //                    UserDisplayName = b.UserDisplayName,
            //                    IsEnable = b.IsEnable,
            //                    RegisterTime = b.RegisterTime,
            //                    ExpansionOne = b.ExpansionOne,
            //                    ExpansionTwo = b.ExpansionTwo,
            //                    CreateTime = b.CreateTime,
            //                    IsOpenHeMai = b.IsOpenHeMai == null ? false : b.IsOpenHeMai,
            //                };
            //    if (query != null && query.ToList().Count > 0)
            //    {
            //        collection.TotalCount = query.Count();
            //        collection.BetInfoList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //    }
            //}
            //else
            //{
            //    var query = from b in Session.Query<BetWhiteList>()
            //                where (userId == string.Empty || b.UserId == userId) && (userDisplayName == string.Empty || b.UserDisplayName == userDisplayName) && (b.CreateTime >= startTime && b.CreateTime < endTime) && b.IsOpenHeMai == false
            //                select new BetWhiteListInfo
            //                {
            //                    WhiteLlistId = b.WhiteLlistId,
            //                    UserId = b.UserId,
            //                    UserDisplayName = b.UserDisplayName,
            //                    IsEnable = b.IsEnable,
            //                    RegisterTime = b.RegisterTime,
            //                    ExpansionOne = b.ExpansionOne,
            //                    ExpansionTwo = b.ExpansionTwo,
            //                    CreateTime = b.CreateTime,
            //                    IsOpenHeMai = b.IsOpenHeMai == null ? false : b.IsOpenHeMai,
            //                };
            //    if (query != null && query.ToList().Count > 0)
            //    {
            //        collection.TotalCount = query.Count();
            //        collection.BetInfoList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //    }
            //} 
            #endregion

            collection.BetInfoList = collection.BetInfoList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return collection;
        }

        public List<UserRegister> QueryNotJoinBetWhiteUser()
        {
            Session.Clear();
            string strSql = "select utable.UserId,utable.DisplayName AS UserDisplayName,utable.CreateTime as RegisterTime from(select UserId,DisplayName ,CreateTime from C_User_Register where UserId not in (select UserId from E_Bet_WhiteList)) utable inner join E_Authentication_Mobile am on utable.UserId=am.UserId inner join E_Authentication_RealName ar on utable.UserId=ar.UserId  where am.IsSettedMobile=1 and ar.IsSettedRealName=1";
            var result = Session.CreateSQLQuery(strSql).List();
            List<UserRegister> userList = new List<UserRegister>();
            if (result != null)
            {
                foreach (var item in result)
                {
                    UserRegister info = new UserRegister();
                    //info.UserId = item.UserId;
                    //info.DisplayName = item.DisplayName;
                    //info.CreateTime = item.CreateTime;
                    //userList.Add(info);
                    var array = item as object[];
                    info.UserId = array[0] == null ? string.Empty : array[0].ToString();
                    info.DisplayName = array[1] == null ? string.Empty : array[1].ToString();
                    info.CreateTime = Convert.ToDateTime(array[2]);
                    userList.Add(info);
                }
            }
            return userList;
        }

        #endregion

        public decimal QueryBetMoneyByDay(string userId, int days)
        {
            Session.Clear();
            var startTime = DateTime.Now.AddDays(-days).Date;
            var endTime = DateTime.Now.AddDays(1).Date;
            var query = from o in Session.Query<OrderDetail>() where o.UserId == userId && o.CreateTime >= startTime && o.CreateTime < endTime select o.CurrentBettingMoney;
            if (query != null &&query.Count() > 0)
                return query.Sum(s => s);
            return 0M;
        }

    }
}
