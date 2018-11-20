using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using GameBiz.Business;

namespace GameBiz.Domain.Managers
{
    public class TogetherManager : GameBizEntityManagement
    {
        //public void UpdateTogetherSchemeMain(TogetherSchemeMain entity)
        //{
        //    this.Update<TogetherSchemeMain>(entity);
        //}
        //public void UpdateTogetherJoin(TogetherJoiner entity)
        //{
        //    this.Update<TogetherJoiner>(entity);
        //}
        //public TogetherSchemeIssuse QueryBettingIssuse(string schemeId, string issuseNumber)
        //{
        //    Session.Clear();
        //    return this.QueryBettingIssuseByKey(string.Format("{0}|{1}", schemeId, issuseNumber));
        //}
        //public TogetherSchemeIssuse QueryBettingIssuseByKey(string key)
        //{
        //    Session.Clear();
        //    return this.Session.Query<TogetherSchemeIssuse>().FirstOrDefault(t => t.TogetherIssuseId == key);
        //    //return this.SingleByKey<TempBettingIssuse>(key);
        //}
        //public TogetherSchemeProgress QueryTogetherSchemeProgressStatus(string schemeId)
        //{
        //    Session.Clear();
        //    return (from m in this.Session.Query<TogetherSchemeMain>() where m.SchemeId == schemeId select m.ProgressStatus).FirstOrDefault();
        //}
        //        #region 合买投注

        //        public void AddTogetherSchemeIssuse(TogetherSchemeIssuse[] issuse)
        //        {
        //            this.Add<TogetherSchemeIssuse>(issuse);
        //        }
        //        public void AddTogetherSchemeMain(TogetherSchemeMain main)
        //        {
        //            this.Add<TogetherSchemeMain>(main);
        //        }
        //        public void AddTogetherJoin(TogetherJoiner entity)
        //        {
        //            this.Add<TogetherJoiner>(entity);
        //        }
        //        public void AddTogetherJoiner(params TogetherJoiner[] array)
        //        {
        //            this.Add<TogetherJoiner>(array);
        //        }
        //        public TogetherSchemeMain QueryTogetherMain(string schemeId)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherSchemeMain>().FirstOrDefault(m => m.SchemeId == schemeId);
        //            //return this.SingleByKey<TogetherSchemeMain>(schemeId);
        //        }
        //        public TogetherQueryInfo QueryTogetherInfo(string schemeId)
        //        {
        //            Session.Clear();
        //            var query = from m in this.Session.Query<TogetherSchemeMain>()
        //                        join g in this.Session.Query<LotteryGame>() on m.Game.GameCode equals g.GameCode
        //                        join u in this.Session.Query<UserRegister>() on m.CreateUser.UserId equals u.UserId
        //                        join o in this.Session.Query<OrderDetail>() on m.SchemeId equals o.SchemeId
        //                        where m.SchemeId == schemeId
        //                        select new TogetherQueryInfo
        //                        {
        //                            AfterTaxBonusMoney = m.AfterTaxBonusMoney,
        //                            BonusStatus = m.BonusStatus,
        //                            CreateTime = m.CreateTime,
        //                            GameCode = g.GameCode,
        //                            GameDisplayName = g.DisplayName,
        //                            GameTypeListDisplayName = m.GameTypeListDisplayName,
        //                            PreTaxBonusMoney = m.PreTaxBonusMoney,
        //                            ProgressStatus = m.ProgressStatus,
        //                            SchemeId = m.SchemeId,
        //                            TotalBettingMoney = o.CurrentBettingMoney,
        //                            TotalMoney = m.TotalMoney,
        //                            UserDisplayName = u.DisplayName,
        //                            UserComeFrom = u.ComeFrom,
        //                            CancelAfterAward = m.CancelAfterAward,
        //                            StopAfterBonus = m.StopAfterBonus,
        //                            TotalIssuseCount = o.TotalIssuseCount,
        //                            DeductPercentage = m.DeductPercentage,
        //                            Description = m.Description,
        //                            //Guarantees = m.Guarantees,
        //                            Price = m.Price,
        //                            Progress = m.Progress,
        //                            Security = m.Security,
        //                            SoldCount = m.SoldCount,
        //                            Subscription = m.Subscription,
        //                            Title = m.Title,
        //                            TotalCount = m.TotalCount,
        //                            StartIssuseNumber = o.StartIssuseNumber,
        //                            CurrentIssuseNumber = o.CurrentIssuseNumber,
        //                            UserId = u.UserId,
        //                            GuaranteesCount = m.Guarantees,
        //                            SystemGuaranteesCount = m.SystemGuarantees,
        //                            JoinUserCount = m.JoinUserCount,
        //                            State = m.State,
        //                            SchemeSource = m.SchemeSource,
        //                            GameTypeList = m.GameTypeList,
        //                        };
        //            return query.FirstOrDefault();
        //        }
        //        public TogetherJoiner QueryTogetherJoiner(string joinId)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherJoiner>().FirstOrDefault(j => j.JoinId == joinId);
        //        }

        //        public bool IsUserJoinTogether(string schemeId, string userId)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherJoiner>().Count(j => j.TogetherSchemeId == schemeId && j.JoinUserId == userId) > 0;
        //        }

        //        public int QueryTogetherJoinUserLength(string schemeId)
        //        {
        //            Session.Clear();
        //            var query = from j in this.Session.Query<TogetherJoiner>()
        //                        where j.TogetherSchemeId == schemeId
        //                        group j by j.JoinUserId into u
        //                        select u.Key;
        //            return query.Count();
        //        }

        //        public TogetherJoiner[] QueryTogetherJoinerArray(string schemeId)
        //        {
        //            Session.Clear();
        //            return (from j in this.Session.Query<TogetherJoiner>() where j.TogetherSchemeId == schemeId select j).ToArray();
        //        }

        //        public TogetherJoinUserCollection QueryTogetherJoinUsers(string schemeId, int pageIndex, int pageSize)
        //        {
        //            Session.Clear();
        //            var query = from j in this.Session.Query<TogetherJoiner>()
        //                        join r in this.Session.Query<UserRegister>() on j.JoinUserId equals r.UserId
        //                        where j.TogetherSchemeId == schemeId && j.JoinType != TogetherJoinType.SystemGuarantees
        //                        orderby j.CreateTime descending
        //                        select new TogetherJoinUser
        //                        {
        //                            BuyCount = j.BuyCount,
        //                            JoinDateTime = j.CreateTime,
        //                            TotalMoney = j.TotalMoney,
        //                            UserName = r.DisplayName,
        //                            UserComeFrom = r.ComeFrom,
        //                            UserId = r.UserId,
        //                            AfterTaxBonusMoney = j.AfterTaxBonusMoney,
        //                        };

        //            var collection = new TogetherJoinUserCollection();
        //            collection.TotalCount = query.Cacheable().Count();
        //            collection.List = query.Skip(pageIndex * pageSize).Take(pageSize).Cacheable().ToList();
        //            return collection;
        //        }
        //        public string[] QueryTogetherSchemeId(string gameCode, string issuseNumber, TogetherState state)
        //        {
        //            Session.Clear();
        //            return (from t in this.Session.Query<TogetherSchemeMain>() where t.Game.GameCode == gameCode && t.StartIssuseNumber == issuseNumber && t.State == state select t.SchemeId).ToArray();
        //        }


        //        public List<TogetherJoin_QueryInfo> QueryMyJoinTogetherList(string userId, string gameCode, List<TogetherState> stateList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        //        {
        //            Session.Clear();
        //            var query = from t in this.Session.Query<TogetherSchemeMain>()
        //                        join j in this.Session.Query<TogetherJoiner>() on t.SchemeId equals j.TogetherSchemeId
        //                        join g in this.Session.Query<LotteryGame>() on t.Game.GameCode equals g.GameCode
        //                        where (string.IsNullOrEmpty(userId) || j.JoinUserId == userId)
        //                        && (string.IsNullOrEmpty(gameCode) || t.Game.GameCode == gameCode)
        //                        && (stateList.Count == 0 || stateList.Contains(t.State))
        //                        && (j.CreateTime >= startTime && j.CreateTime < endTime)
        //                        orderby j.CreateTime descending
        //                        select new TogetherJoin_QueryInfo
        //                        {
        //                            AfterTaxBonusMoney = j.AfterTaxBonusMoney,
        //                            GameDisplayName = g.DisplayName,
        //                            Guarantees = t.Guarantees,
        //                            JoinCount = j.BuyCount,
        //                            JoinDateTime = j.CreateTime,
        //                            JoinId = j.JoinId,
        //                            JoinMoney = j.TotalMoney,
        //                            JoinState = j.JoinState,
        //                            JoinType = j.JoinType,
        //                            Price = t.Price,
        //                            Progress = t.Progress,
        //                            SoldCount = t.SoldCount,
        //                            SystemGuarantees = t.SystemGuarantees,
        //                            Title = t.Title,
        //                            TogetherState = t.State,
        //                            TotalCount = t.TotalCount,
        //                            TotalMoney = t.TotalMoney,
        //                            TogetherId = t.SchemeId,
        //                        };
        //            totalCount = query.Count();
        //            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        //        }

        //        public SchemeIssuse_QueryInfo[] QueryTogetherSchemeIssuse(string schemeId, int pageIndex, int pageSize, out int totalCount)
        //        {
        //            Session.Clear();
        //            var query = from g in this.Session.Query<TogetherSchemeIssuse>()
        //                        join i in this.Session.Query<GameIssuse>() on g.CurrentIssuseNumber equals i.GameCode_IssuseNumber
        //                        where g.SchemeId == schemeId
        //                        orderby g.CurrentIssuseNumber ascending
        //                        select new SchemeIssuse_QueryInfo
        //                        {
        //                            AfterTaxBonusMoney = g.AfterTaxBonusMoney,
        //                            Amount = g.Amount,
        //                            BonusStatus = g.BonusStatus,
        //                            BonusTime = g.BonusTime,
        //                            CreateTime = g.CreateTime.Value,
        //                            IssuseMoney = g.IssuseMoney,
        //                            IssuseNumber = g.CurrentIssuseNumber.Split('|')[1],
        //                            PreTaxBonusMoney = g.PreTaxBonusMoney,
        //                            ProgressStatus = g.ProgressStatus,
        //                            SchemeId = g.SchemeId,
        //                            TicketIdList = g.TicketIdList,
        //                            TicketLog = g.TicketLog,
        //                            TicketStatus = g.TicketStatus,
        //                            WinNumber = i.WinNumber,
        //                        };

        //            totalCount = query.Count();
        //            return query.Skip(pageIndex * pageSize).Take(pageSize).ToArray();
        //        }

        //        public IList<TogetherQueryInfo> QueryTogetherList(string gameCode, TogetherSchemeProgress? progress, int isOther, decimal min, decimal max, string createrDisplayName, OrderByInfoList orderList
        //          , int pageIndex, int pageSize, out  int totalCount)
        //        {
        //            Session.Clear();
        //            var sql_length = @"select COUNT(t.SchemeId)
        //                                from C_Together_Scheme_Main t
        //                                join C_Lottery_Game g on t.GameCode = g.GameCode
        //                                join C_User_Register u on t.CreateUserId = u.UserId  ";
        //            var sql_list = @"SELECT t.*
        //                                        ,isnull([IsTop],0) isTop
        //                                        ,isnull([JoinUserCount],0) JoinCount
        //                                        ,isnull(Beedings,0) Beedings,isnull(FailBeedings,0) FailBeedings
        //                                FROM 
        //	                                (select 
        //                                        [SchemeId]
        //                                      ,[CreateUserId]
        //                                      ,t.[GameCode]
        //                                      ,[SchemeSource]
        //                                      ,[GameTypeList]
        //                                      ,[GameTypeListDisplayName]
        //                                      ,[TotalMoney]
        //                                      ,[BonusStatus]
        //                                      ,[PreTaxBonusMoney]
        //                                      ,[AfterTaxBonusMoney]
        //                                      ,[StopAfterBonus]
        //                                      ,[CancelAfterAward]
        //                                      ,[CreateTime]
        //                                      ,[Title]
        //                                      ,[Description]
        //                                      ,[Security]
        //                                      ,[StartIssuseNumber]
        //                                      ,[TotalCount]
        //                                      ,[Price]
        //                                      ,[SoldCount]
        //                                      ,[JoinUserCount]
        //                                      ,[DeductPercentage]
        //                                      ,[Subscription]
        //                                      ,[Guarantees]
        //                                      ,[SystemGuarantees]
        //                                      ,[ProgressStatus]
        //                                      ,[State]
        //                                      ,[Progress]
        //                                      ,[IsTop]
        //                                      ,1 as [JoinCount]
        //                                        ,u.DisplayName as UserDisplayName,u.UserKey,u.ComeFrom ,g.DisplayName as GameDisplayName
        //                                        FROM C_Together_Scheme_Main t
        //                                        join C_Lottery_Game g on t.GameCode = g.GameCode
        //                                        join C_User_Register u on t.CreateUserId = u.UserId 
        //	                                ) AS t
        //                                LEFT JOIN 
        //                                        (SELECT ctb.UserId,isnull(sum(ctb.Beedings),0) Beedings ,isnull(sum(ctb.FailBeedings),0) FailBeedings FROM C_Together_Beedings ctb GROUP BY ctb.UserId
        //	                                ) AS b 
        //                                ON b.UserId=t.CreateUserId  ";

        //            var condition = new List<string>();
        //            condition.Add(string.Format(" t.CreateTime > '{0}'", DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd")));
        //            //condition.Add("t.State=1 ");
        //            var order = new List<string>();
        //            if (progress == null)
        //            {
        //                condition.Add(" t.State=1 ");
        //                condition.Add(" t.ProgressStatus in (1,2) ");
        //            }
        //            else
        //            {
        //                if (progress.Value == TogetherSchemeProgress.SalesIn) condition.Add(" t.State=1 ");
        //                if (progress.Value == TogetherSchemeProgress.Standard) condition.Add(" t.State=1 ");
        //                if (progress.Value == TogetherSchemeProgress.Finish) condition.Add(" (t.State=2 or t.State=3) ");
        //                condition.Add(string.Format(" t.ProgressStatus={0} ", (int)progress));
        //            }

        //            if (isOther == 1)
        //            {
        //                if (!string.IsNullOrEmpty(gameCode)) condition.Add(string.Format(" t.GameCode!='{0}' and t.Progress!=1 ", gameCode));
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(gameCode)) condition.Add(string.Format(" t.GameCode='{0}' ", gameCode));
        //            }
        //            if (min > 0) condition.Add(string.Format(" t.TotalMoney >= {0}", min));
        //            if (max > 0) condition.Add(string.Format(" t.TotalMoney <= {0}", max));
        //            if (!string.IsNullOrEmpty(createrDisplayName)) condition.Add(string.Format(" u.DisplayName like '%{0}%' ", createrDisplayName));
        //            //condition.Add("  t.State=1");

        //            //期号查询条件
        //            //if (!string.IsNullOrEmpty(IssuseNumber)) condition.Add(string.Format(" t.StartIssuseNumber='{0}' ", IssuseNumber));

        //            if (condition.Count != 0)
        //            {
        //                sql_length = string.Format("{0} where {1}", sql_length, string.Join(" and ", condition.ToArray()));
        //                sql_list = string.Format("{0} where {1}", sql_list, string.Join(" and ", condition.ToArray()));
        //            }
        //            orderList.ForEach(item =>
        //            {
        //                order.Add(string.Format("t.{0} {1}", item.PropertyName, item.Order.ToString()));
        //            });
        //            if (order.Count == 0) order.Add("t.IsTop DESC,t.CreateTime desc");
        //            if (order.Count > 0) sql_list = string.Format("{0} order by {1}", sql_list, string.Join(" , ", order.ToArray()));

        //            totalCount = this.Session.CreateSQLQuery(sql_length).UniqueResult<int>();

        //            var entityList = this.Session.CreateSQLQuery(sql_list).SetFirstResult(pageIndex * pageSize).SetMaxResults(pageSize).List();
        //            var result = new List<TogetherQueryInfo>();
        //            foreach (var item in entityList)
        //            {
        //                var array = item as object[];
        //                result.Add(new TogetherQueryInfo
        //                {
        //                    SchemeId = array[0].ToString(),
        //                    UserId = array[1].ToString(),
        //                    GameCode = array[2].ToString(),
        //                    SchemeSource = (SchemeSource)int.Parse(array[3].ToString()),
        //                    GameTypeList = array[4].ToString(),

        //                    GameTypeListDisplayName = array[5].ToString(),
        //                    TotalMoney = decimal.Parse(array[6].ToString()),
        //                    BonusStatus = (BonusStatus)int.Parse(array[7].ToString()),
        //                    PreTaxBonusMoney = decimal.Parse(array[8].ToString()),
        //                    AfterTaxBonusMoney = decimal.Parse(array[9].ToString()),

        //                    StopAfterBonus = (bool)(array[10] == null ? 0 : array[10]),
        //                    CancelAfterAward = (bool)(array[11] == null ? 0 : array[11]),
        //                    CreateTime = DateTime.Parse(array[12].ToString()),
        //                    Title = array[13].ToString(),
        //                    Description = array[14].ToString(),

        //                    Security = (TogetherSchemeSecurity)int.Parse(array[15].ToString()),
        //                    StartIssuseNumber = array[16].ToString(),
        //                    TotalCount = int.Parse(array[17].ToString()),
        //                    Price = decimal.Parse(array[18].ToString()),
        //                    SoldCount = int.Parse(array[19].ToString()),

        //                    JoinUserCount = int.Parse(array[20].ToString()),
        //                    DeductPercentage = int.Parse(array[21].ToString()),
        //                    Subscription = int.Parse(array[22].ToString()),
        //                    GuaranteesCount = int.Parse(array[23].ToString()),
        //                    SystemGuaranteesCount = int.Parse(array[24].ToString()),


        //                    ProgressStatus = (TogetherSchemeProgress)int.Parse(array[25].ToString()),
        //                    State = (TogetherState)int.Parse(array[26].ToString()),
        //                    Progress = decimal.Parse(array[27].ToString()),
        //                    //istop..joincount跳过后面显示28,29

        //                    //UserDisplayName = array[29].ToString(),
        //                    //UserKey = array[30].ToString(),
        //                    //UserComeFrom = array[31].ToString(),
        //                    //GameDisplayName = array[32].ToString(),
        //                    //IsTop = (bool)(array[33] == null ? 0 : array[33]),

        //                    //JoinUserCount = array[34] == null ? 0 : int.Parse(array[34].ToString()),//int.Parse(array[35].ToString())
        //                    //Beedings = int.Parse(array[35].ToString()),
        //                    //FailBeedings = int.Parse(array[36].ToString()),

        //                    UserDisplayName = array[30].ToString(),
        //                    //UserKey = array[31].ToString(),
        //                    UserComeFrom = array[32].ToString(),
        //                    GameDisplayName = array[33].ToString(),
        //                    IsTop = (bool)(array[34] == null ? 0 : array[34]),

        //                    //JoinUserCount = array[35] == null ? 0 : int.Parse(array[35].ToString()),//int.Parse(array[35].ToString())
        //                    Beedings = int.Parse(array[36].ToString()),
        //                    FailBeedings = int.Parse(array[37].ToString()),


        //                    //GuaranteesRatio = (int)Math.Floor(decimal.Parse(array[23].ToString()) * 100 / int.Parse(array[17].ToString())),
        //                    //SystemGuaranteesRatio = (int)Math.Floor(decimal.Parse(array[24].ToString()) * 100 / int.Parse(array[17].ToString())),

        //                });
        //            }
        //            return result;
        //        }

        //        public TogetherBeedings QueryTogetherBeedings(string userKey, string gameCode)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherBeedings>().FirstOrDefault(t => t.UserKey == userKey && t.GameCode == gameCode);
        //        }

        //        public void AddTogetherBeedings(TogetherBeedings beedings)
        //        {
        //            this.Add<TogetherBeedings>(beedings);
        //        }
        //        public void UpdateTogetherBeedings(TogetherBeedings beedings)
        //        {
        //            this.Update<TogetherBeedings>(beedings);
        //        }
        //        /// <summary>
        //        /// 查询合买发起者某彩种战绩
        //        /// </summary>
        //        public TogetherBeedings QueryBeedingsByGame(string userKey, string gameCode)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherBeedings>().FirstOrDefault(t => (t.UserKey == userKey || t.UserId == userKey) && t.GameCode == gameCode);
        //        }
        //        /// <summary>
        //        /// 总战绩查询
        //        /// </summary>
        //        public List<TogetherBeedings> QueryBeedings(string userkey)
        //        {
        //            Session.Clear();
        //            var query = from t in Session.Query<TogetherBeedings>()
        //                        where t.UserKey == userkey || t.UserId == userkey
        //                        select t;
        //            return query.ToList();
        //        }

        //        public void AddTogetherFollowerRule(TogetherFollowerRule entity)
        //        {
        //            this.Add<TogetherFollowerRule>(entity);
        //        }

        //        public void DeleteTogetherFollowerRule(TogetherFollowerRule entity)
        //        {
        //            this.Delete<TogetherFollowerRule>(entity);
        //        }

        //        public TogetherFollowerRule QueryTogetherFollowerRule(string createrUserId, string followerUserId, string gameCode)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherFollowerRule>().FirstOrDefault(r => r.CreaterUserId == createrUserId && r.FollowerUserId == followerUserId && r.GameCode == gameCode);
        //        }
        //        public int QueryFollowerCount(string createrUserId, string gameCode)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherFollowerRule>().Count(r => r.CreaterUserId == createrUserId && r.GameCode == gameCode);
        //        }
        //        public List<TogetherFollowerRule> QueryTogetherFollowerList(string createrUserId, string gameCode)
        //        {
        //            Session.Clear();
        //            return (from r in this.Session.Query<TogetherFollowerRule>() where r.CreaterUserId == createrUserId && r.GameCode == gameCode orderby r.CreateTime ascending select r).ToList();
        //        }

        //        public void AddTogetherFollowerRecord(params TogetherFollowerRecord[] array)
        //        {
        //            this.Add<TogetherFollowerRecord>(array);
        //        }
        //        public int QueryTogetherFollowerRecordCount(string indexKey)
        //        {
        //            Session.Clear();
        //            return this.Session.Query<TogetherFollowerRecord>().Count(r => r.IndexKey == indexKey);
        //        }

        //        public List<TogetherFollowerInfo> QueryTogetherFollowerByFollowerUserId(string followerUserId, string gameCode, int pageIndex, int pageSize, out int totalCount)
        //        {
        //            Session.Clear();
        //            var query = from r in this.Session.Query<TogetherFollowerRule>()
        //                        join g in this.Session.Query<LotteryGame>() on r.GameCode equals g.GameCode
        //                        join creater in this.Session.Query<UserRegister>() on r.CreaterUserId equals creater.UserId
        //                        join follower in this.Session.Query<UserRegister>() on r.FollowerUserId equals follower.UserId
        //                        where r.FollowerUserId == followerUserId
        //                        && (string.IsNullOrEmpty(gameCode) || r.GameCode == gameCode)
        //                        select new TogetherFollowerInfo
        //                        {
        //                            CreaterUserId = r.CreaterUserId,
        //                            CreaterDisplayName = creater.DisplayName,
        //                            CreateTime = r.CreateTime,
        //                            FollowerDisplayName = follower.DisplayName,
        //                            FollowerUserId = follower.UserId,
        //                            FollowerMoney = r.FollowerMoney,
        //                            GameCode = r.GameCode,
        //                            SchemeCount = r.SchemeCount,
        //                            StopFollowerMinBalance = r.StopFollowerMinBalance,
        //                            GameDisPlayName = g.DisplayName,
        //                        };
        //            totalCount = query.Count();
        //            return query.Skip(pageSize * pageIndex).Take(pageSize).ToList();
        //        }
        //        public List<TogetherFollowerInfo> QueryTogetherFollowerByCreaterUserId(string createrUserId, string gameCode, int pageIndex, int pageSize, out int totalCount)
        //        {
        //            Session.Clear();
        //            var query = from r in this.Session.Query<TogetherFollowerRule>()
        //                        join g in this.Session.Query<LotteryGame>() on r.GameCode equals g.GameCode
        //                        join creater in this.Session.Query<UserRegister>() on r.CreaterUserId equals creater.UserId
        //                        join follower in this.Session.Query<UserRegister>() on r.FollowerUserId equals follower.UserId
        //                        where r.CreaterUserId == createrUserId
        //                        && (string.IsNullOrEmpty(gameCode) || r.GameCode == gameCode)
        //                        select new TogetherFollowerInfo
        //                        {
        //                            CreaterUserId = r.CreaterUserId,
        //                            CreaterDisplayName = creater.DisplayName,
        //                            CreateTime = r.CreateTime,
        //                            FollowerDisplayName = follower.DisplayName,
        //                            FollowerUserId = follower.UserId,
        //                            FollowerMoney = r.FollowerMoney,
        //                            GameCode = r.GameCode,
        //                            SchemeCount = r.SchemeCount,
        //                            StopFollowerMinBalance = r.StopFollowerMinBalance,
        //                            GameDisPlayName = g.DisplayName,
        //                        };
        //            totalCount = query.Count();
        //            return query.Skip(pageSize * pageIndex).Take(pageSize).ToList();
        //        }
        //        #endregion



        public void AddTogetherFollowerRule(TogetherFollowerRule entity)
        {
            this.Add<TogetherFollowerRule>(entity);
        }

        public void AddTogetherFollowerRecord(TogetherFollowerRecord entity)
        {
            this.Add<TogetherFollowerRecord>(entity);
        }

    }
}
