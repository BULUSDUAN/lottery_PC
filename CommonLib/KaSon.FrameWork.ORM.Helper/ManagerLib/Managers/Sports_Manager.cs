using EntityModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class Sports_Manager:DBbase
    {
        public T_SingleScheme_Order QuerySingleSchemeOrder(string schemeId)
        {
          //  Session.Clear();
            return this.DB.CreateQuery<T_SingleScheme_Order>().Where(p => p.OrderId == schemeId).FirstOrDefault();
        }

        public int QueryTicketCount(string schemeId)
        {
            //Session.Clear();
            return this.DB.CreateQuery<C_Sports_Ticket>().Where(p => p.SchemeId == schemeId).Count();
        }
        public C_Sports_Order_Running QuerySports_Order_Running(string schemeId)
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Sports_Order_Running>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }
        public void AddSports_Order_Running(C_Sports_Order_Running entity)
        {
            DB.GetDal<C_Sports_Order_Running>().Add(entity);
        }
        //public void SqlBulkAddTable(DataTable dt)
        //{
        //    if (dt.Rows.Count <= 0) return;
        //    GameBiz.Business.Domain.Managers.SqlBulkCopyHelper.WriteTableToDataBase(dt, Session.Connection as System.Data.SqlClient.SqlConnection, new System.Data.SqlClient.SqlRowsCopiedEventHandler((obj, arg) =>
        //    {
        //        Console.WriteLine(arg.RowsCopied.ToString());
        //    }));
        //}

        public void SqlBulkAddTable(List<C_Sports_Ticket> list)
        {
            if (list.Count <= 0) return;
            DB.GetDal<C_Sports_Ticket>().BulkAdd(list);
        }

        public void ExecSql(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;
            //  Session.Clear();
            DB.CreateSQLQuery(sql).Excute();//.ExecuteUpdate();
        }
        public void AddSingleSchemeOrder(T_SingleScheme_Order entity)
        {
            DB.GetDal<T_SingleScheme_Order>().Add(entity);
        }
        public void UpdateSports_AnteCode(C_Sports_AnteCode entity)
        {
            DB.GetDal<C_Sports_AnteCode>().Update(entity);
        }
        public List<C_Sports_AnteCode> QuerySportsAnteCodeBySchemeId(string schemeId)
        {
            //Session.Clear();
            var list = (from a in this.DB.CreateQuery<C_Sports_AnteCode>()
                        where a.SchemeId == schemeId
                        select a
                        ).ToList();
            if (list == null || list.Count <= 0)
                list = (from b in this.DB.CreateQuery<C_Sports_AnteCode_History>()
                        where b.SchemeId == schemeId
                        select b).ToList().Select(a=> new C_Sports_AnteCode
                        {
                            AnteCode = a.AnteCode,
                            BonusStatus = a.BonusStatus,
                            CreateTime = a.CreateTime,
                            GameCode = a.GameCode,
                            GameType = a.GameType,
                            Id = a.Id,
                            IsDan = a.IsDan,
                            IssuseNumber = a.IssuseNumber,
                            MatchId = a.MatchId,
                            Odds = a.Odds,
                            PlayType = a.PlayType,
                            SchemeId = a.SchemeId,

                        }).ToList();
            return list;
        }
        /// <summary>
        /// kason  重新加载比赛数据
        /// </summary>
        /// <returns></returns>
        public List<Cache_JCZQ_MatchInfo> QueryJCZQ_Current_CacheMatchList()
        {
           // Session.Clear();
            var query = from m in DB.CreateQuery<C_JCZQ_Match>()
                        where m.FSStopBettingTime > DateTime.Now
                        select new Cache_JCZQ_MatchInfo
                        {
                            DSStopBettingTime = m.DSStopBettingTime,
                            FSStopBettingTime = m.FSStopBettingTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueName = m.LeagueName,
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            MatchStopDesc = m.MatchStopDesc,
                            PrivilegesType = m.PrivilegesType,
                        };
            return query.ToList();
        }

        public List<C_SFGG_Match> QuerySFGGSaleMatchCount(string[] matchArray)
        {
            return DB.CreateQuery<C_SFGG_Match>().Where(s => matchArray.Contains(s.MatchId)).ToList();
        }

        public List<C_BJDC_Match> QueryBJDCSaleMatchCount(string[] matchIdArray)
        {
            //Session.Clear();
            var query = from m in DB.CreateQuery<C_BJDC_Match>()
                        where matchIdArray.Contains(m.Id)
                        && m.LocalStopTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        /// <summary>
        /// kason 用matchId查询缓存中的比赛
        /// </summary>
        /// <returns></returns>
        public List<Cache_BJDC_MatchInfo> QueryBJDC_Current_CacheMatchList()
        {
           // Session.Clear();
            var query = from m in DB.CreateQuery<C_BJDC_Match>()   //this.Session.Query<C_BJDC_Match>()
                        where m.LocalStopTime > DateTime.Now
                        select new Cache_BJDC_MatchInfo
                        {
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            Id = m.Id,
                            IssuseNumber = m.IssuseNumber,
                            LocalStopTime = m.LocalStopTime,
                            MatchName = m.MatchName,
                            MatchOrderId = m.MatchOrderId,
                            MatchStartTime = m.MatchStartTime,
                            PrivilegesType = m.PrivilegesType
                        };
            return query.ToList();
        }

        /// <summary>
        ///  kason 重新加载比赛数据
        /// </summary>
        /// <returns></returns>
        public List<Cache_JCLQ_MatchInfo> QueryJCLQ_Current_CacheMatchList()
        {
          //  Session.Clear();
            var query = from m in DB.CreateQuery<C_JCLQ_Match>()
                        where m.FSStopBettingTime > DateTime.Now
                        select new Cache_JCLQ_MatchInfo
                        {
                            DSStopBettingTime = m.DSStopBettingTime,
                            FSStopBettingTime = m.FSStopBettingTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueName = m.LeagueName,
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            PrivilegesType = m.PrivilegesType,
                        };
            return query.ToList();
        }

        public List<C_JCZQ_Match> QueryJCZQSaleMatchCount(string[] matchIdArray)
        {
            //Session.Clear();
            var query = from m in DB.CreateQuery<C_JCZQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        /// <summary>
        /// 查询用户战绩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public C_User_Beedings QueryUserBeedings(string userId, string gameCode, string gameType)
        {
            //Session.Clear();
            var query = DB.CreateQuery<C_User_Beedings>();
            if (string.IsNullOrEmpty(gameType))
            {
                query = query.Where(p => p.UserId == userId && p.GameCode == gameCode);
            }
            else
            {
                query = query.Where(p => p.UserId == userId && p.GameCode == gameCode &&  p.GameType == gameType);
            }
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 初始化用户战绩
        /// </summary>
        /// <param name="UserBeedings"></param>
        public void AddUserBeedings(C_User_Beedings UserBeedings) {

            DB.GetDal<C_User_Beedings>().Add(UserBeedings);
        }

        /// <summary>
        /// 用户中奖概率
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public C_User_BonusPercent QueryUserBonusPercent(string userId, string gameCode, string gameType)
        {
            var query = DB.CreateQuery<C_User_BonusPercent>();
            if (string.IsNullOrEmpty(gameType))
            {
                query = query.Where(p => p.UserId == userId && p.GameCode == gameCode);
            }
            else
            {
                query = query.Where(p => p.UserId == userId && p.GameCode == gameCode && p.GameType == gameType);
            }
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 初始化用户中奖概率
        /// </summary>
        /// <param name="UserBeedings"></param>
        public void AddUserBonusPercent(C_User_BonusPercent UserBonusPercent)
        {

            DB.GetDal<C_User_BonusPercent>().Add(UserBonusPercent);
        }

        /// <summary>
        /// 用户关注汇总
        /// </summary>
        /// <param name="c_User_Attention_Summary"></param>
        public void AddUserAttentionSummary(C_User_Attention_Summary UserAttentionSummary)
        {
            DB.GetDal<C_User_Attention_Summary>().Add(UserAttentionSummary);

        }

        public C_SingleScheme_AnteCode QuerySingleScheme_AnteCode(string schemeId)
        {
            return this.DB.CreateQuery<C_SingleScheme_AnteCode>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }

        public void UpdateSports_Order_Running(params C_Sports_Order_Running[] entity)
        {
            DB.GetDal<C_Sports_Order_Running>().Update(entity);
        }

        public List<C_JCLQ_Match> QueryJCLQSaleMatchCount(string[] matchIdArray)
        {
           // Session.Clear();
            var query = from m in this.DB.CreateQuery<C_JCLQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public List<C_JCLQ_MatchResult> QueryJCLQMatchResult(string[] matchIdArray)
        {
           // Session.Clear();
            var query = from m in this.DB.CreateQuery<C_JCLQ_MatchResult>()
                        where matchIdArray.Contains(m.MatchId)
                        select m;
            return query.ToList();
        }

        public void AddSports_Order_Complate(C_Sports_Order_Complate entity)
        {
            DB.GetDal<C_Sports_Order_Complate>().Add(entity);
        }

        public void DeleteSports_Order_Running(C_Sports_Order_Running entity)
        {
            DB.GetDal<C_Sports_Order_Running>().Delete(entity);
        }

        public C_Sports_Together QuerySports_Together(string schemeId)
        {
            return DB.CreateQuery<C_Sports_Together>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }

        public C_Sports_TogetherJoin QuerySports_TogetherJoin(string schemeId, int joinId)
        {
            return DB.CreateQuery<C_Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId && p.Id == joinId).FirstOrDefault();
        }
        public C_Sports_TogetherJoin QuerySports_TogetherJoin(string schemeId, TogetherJoinType joinType)
        {
            return DB.CreateQuery<C_Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId && p.JoinType == (int)joinType).FirstOrDefault();
        }

        public void UpdateSports_Together(C_Sports_Together entity)
        {
            DB.GetDal<C_Sports_Together>().Update(entity);
        }
        public void UpdateSports_TogetherJoin(C_Sports_TogetherJoin entity)
        {
            DB.GetDal<C_Sports_TogetherJoin>().Update(entity);
        }

        public void AddSports_AnteCode(C_Sports_AnteCode entity)
        {
            DB.GetDal<C_Sports_AnteCode>().Add(entity);
        }

        public void AddTemp_Together(C_Temp_Together entity)
        {
            DB.GetDal<C_Temp_Together>().Add(entity);
        }

        public void AddSports_TogetherJoin(C_Sports_TogetherJoin entity)
        {
            DB.GetDal<C_Sports_TogetherJoin>().Add(entity);
        }

        public List<C_Together_FollowerRule> QuerySportsTogetherFollowerList(string createrUserId, string gameCode, string gameType)
        {
            return (from r in DB.CreateQuery<C_Together_FollowerRule>() where r.CreaterUserId == createrUserId && r.GameCode == gameCode && r.GameType == gameType orderby r.FollowerIndex ascending select r).ToList();
        }

        public void AddTogetherFollowerRecord(C_Together_FollowerRecord entity)
        {
            DB.GetDal<C_Together_FollowerRecord>().Add(entity);
        }

        public void UpdateTogetherFollowerRule(C_Together_FollowerRule entity)
        {
            DB.GetDal<C_Together_FollowerRule>().Update(entity);
        }

        public void UpdateUserBeedings(params C_User_Beedings[] entity)
        {
            DB.GetDal<C_User_Beedings>().Update(entity);
        }

        public void AddSports_Together(C_Sports_Together entity)
        {
            DB.GetDal<C_Sports_Together>().Add(entity);
        }

        public C_Lottery_Scheme QueryLotteryScheme(string schemeId)
        {
            return DB.CreateQuery<C_Lottery_Scheme>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }

        public List<C_Sports_TogetherJoin> QuerySports_TogetherSucessJoin(string schemeId)
        {
            return DB.CreateQuery<C_Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId && p.JoinSucess == true).ToList();
        }

        public int QueryKeyLineCount(string keyLine)
        {
            return DB.CreateQuery<C_Lottery_Scheme>().Where(p => p.KeyLine == keyLine).Count();
        }

        public void AddLotteryScheme(C_Lottery_Scheme entity)
        {
            DB.GetDal<C_Lottery_Scheme>().Add(entity);
        }

        public List<C_Lottery_Scheme> QueryLotterySchemeByKeyLine(string keyLine)
        {
            return DB.CreateQuery<C_Lottery_Scheme>().Where(p => p.KeyLine == keyLine && p.IsComplate == false).OrderBy(p => p.OrderIndex).ToList();
        }

        public void AddUserSaveOrder(C_UserSaveOrder entity)
        {
            DB.GetDal<C_UserSaveOrder>().Add(entity);
        }
        public bool IsUserJoinTogether(string schemeId, string userId)
        {
            var count = DB.CreateQuery<C_Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId && p.JoinUserId == userId && p.JoinSucess == true).Count();
            return count > 0;
        }

        public C_Together_FollowerRule QueryTogetherFollowerRule(long Id)
        {
            return DB.CreateQuery<C_Together_FollowerRule>().Where(p => p.Id == Id).FirstOrDefault();
        }

        public C_Together_FollowerRule QueryTogetherFollowerRule(string createrUserId, string followerUserId, string gameCode, string gameType)
        {
            return DB.CreateQuery<C_Together_FollowerRule>().Where(p => p.CreaterUserId == createrUserId && p.FollowerUserId == followerUserId && p.GameCode == gameCode && p.GameType == gameType).FirstOrDefault();
        }

        public int QueryTogetherFollowerRuleCount(string createUserId, string gameCode, string gameType)
        {
            return DB.CreateQuery<C_Together_FollowerRule>().Where(p => p.CreaterUserId == createUserId && p.GameCode == gameCode && p.GameType == gameType).Count();
        }

        public void AddTogetherFollowerRule(C_Together_FollowerRule entity)
        {
            DB.GetDal<C_Together_FollowerRule>().Add(entity);
        }

        public void DeleteTogetherFollowerRule(C_Together_FollowerRule entity)
        {
            DB.GetDal<C_Together_FollowerRule>().Delete(entity);
        }

        public C_Sports_Order_Complate QuerySports_Order_Complate(string schemeId)
        {
         
            return DB.CreateQuery<C_Sports_Order_Complate>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }

        public List<C_Sports_Ticket> QueryTicketList(string gameCode, string issuse)
        {
           
            return DB.CreateQuery<C_Sports_Ticket>().Where(p => p.GameCode == gameCode && p.IssuseNumber == issuse
                        && p.BonusStatus == (int)BonusStatus.Waitting
                        && p.TicketStatus == (int)TicketStatus.Ticketed).ToList();
        }

        public List<C_Sports_Ticket> QueryTicketList(string schemeId)
        {
          
            return DB.CreateQuery<C_Sports_Ticket>().Where(p => p.SchemeId == schemeId).ToList();
        }

        public void UpdateSports_Order_Complate(params C_Sports_Order_Complate[] entity)
        {
            DB.GetDal<C_Sports_Order_Complate>().Update(entity);
        }

        public decimal GetUserMaxBonusMoney(string userId)
        {
           
            string strSql = "select isnull(max(AfterTaxBonusMoney),0) maxBonusMoney from C_Sports_Order_Complate where UserId=:UserId and BonusStatus=20 and IsVirtualOrder=0";
            var maxBonusMoney = DB.CreateSQLQuery(strSql).SetString("UserId", userId).First<decimal>();

            return maxBonusMoney;
        }

        public C_OrderDetail QueryOrderDetailBySchemeId(string schemeId)
        {
           
            return DB.CreateQuery<C_OrderDetail>().Where(s => s.SchemeId == schemeId).FirstOrDefault();
        }

        public C_Sports_Ticket QueryTicket(string ticketId)
        {
         
            return DB.CreateQuery<C_Sports_Ticket>().Where(p => p.TicketId == ticketId).FirstOrDefault();
        }

        public List<string> QueryWaitPayRebateRunningOrder()
        {
           
            var query = from o in DB.CreateQuery<C_Sports_Order_Running>()
                        where o.IsPayRebate == false
                        && o.CanChase == true
                        && o.TicketStatus == (int)TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        orderby o.CreateTime ascending
                        select o.SchemeId;
            return query.ToList();
        }

        public List<C_Sports_Order_Running> QueryAllRunningOrder()
        {

            var query = from o in DB.CreateQuery<C_Sports_Order_Running>()
                        where o.TicketStatus == (int)TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        orderby o.CreateTime ascending
                        select o;
            return query.ToList();
        }


        public List<string> QueryWaitPayRebateComplateOrder()
        {
           
            var query = from o in DB.CreateQuery<C_Sports_Order_Complate>()
                        where o.IsPayRebate == false
                        && o.CanChase == true
                        && o.TicketStatus == (int)TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        && o.CreateTime > DateTime.Parse("2016-07-11")
                        orderby o.CreateTime ascending
                        select o.SchemeId;
            return query.ToList();
        }

        public C_UserSaveOrder QuerySaveOrder(string schemeId)
        {
           
            return DB.CreateQuery<C_UserSaveOrder>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }

        public C_BJDC_Match QueryBJDC_Match(string id)
        {
          
            return DB.CreateQuery<C_BJDC_Match>().Where(p => p.Id == id).FirstOrDefault();
        }
        public C_BJDC_MatchResult QueryBJDC_MatchResult(string id)
        {
           
            return DB.CreateQuery<C_BJDC_MatchResult>().Where(p => p.Id == id).FirstOrDefault();
        }

        public C_JCZQ_Match QueryJCZQ_Match(string matchId)
        {
          
            return DB.CreateQuery<C_JCZQ_Match>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        public C_JCZQ_MatchResult QueryJCZQ_MatchResult(string matchId)
        {
         
            return DB.CreateQuery<C_JCZQ_MatchResult>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        public C_JCLQ_Match QueryJCLQ_Match(string matchId)
        {
          
            return DB.CreateQuery<C_JCLQ_Match>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        public C_JCLQ_MatchResult QueryJCLQ_MatchResult(string matchId)
        {
           
            return DB.CreateQuery<C_JCLQ_MatchResult>().Where(p => p.MatchId == matchId).FirstOrDefault();
        }
        /// <summary>
        /// 查询追号
        /// </summary>
        /// <param name="keyLine"></param>
        /// <returns></returns>
        public List<C_Lottery_Scheme> QueryAllLotterySchemeByKeyLine(string keyLine)
        {

            return DB.CreateQuery<C_Lottery_Scheme>().Where(p => p.KeyLine == keyLine).OrderBy(p => p.OrderIndex).ToList();
           // return this.Session.Query<C_Lottery_Scheme>().Where(p => p.KeyLine == keyLine).OrderBy(p => p.OrderIndex).ToList();
        }

        public List<C_Sports_Order_Running> QueryOrderRunningBySchemeIdArray(string[] schemeIdArray)
        {
          //  Session.Clear();
            return DB.CreateQuery<C_Sports_Order_Running>().Where(p => schemeIdArray.Contains(p.SchemeId)).ToList();
        }
        public List<C_Sports_Order_Complate> QueryOrderComplateBySchemeIdArray(string[] schemeIdArray)
        {
            //   Session.Clear();
            return DB.CreateQuery<C_Sports_Order_Complate>().Where(p => schemeIdArray.Contains(p.SchemeId)).ToList();
        }
    }
}
