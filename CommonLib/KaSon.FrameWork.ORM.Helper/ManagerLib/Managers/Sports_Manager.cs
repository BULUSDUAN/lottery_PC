using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.Sport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class Sports_Manager : DBbase
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

        public void UpdateTogetherFollowerRecord(C_Together_FollowerRecord entity)
        {
            DB.GetDal<C_Together_FollowerRecord>().Update(entity);
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
                        select b).ToList().Select(a => new C_Sports_AnteCode
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
                query = query.Where(p => p.UserId == userId && p.GameCode == gameCode && p.GameType == gameType);
            }
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 初始化用户战绩
        /// </summary>
        /// <param name="UserBeedings"></param>
        public void AddUserBeedings(C_User_Beedings UserBeedings)
        {

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
        public void UpdateUserAttentionSummary(C_User_Attention_Summary entity)
        {
            DB.GetDal<C_User_Attention_Summary>().Update(entity);
        }

        public void DeleteUserAttention(C_User_Attention entity)
        {
            DB.GetDal<C_User_Attention>().Delete(entity);
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
            DB.GetDal<C_Sports_TogetherJoin>().Add(entity,true);
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

        public List<C_Sports_Order_Running> QueryAllRunningOrder_dp()
        {

            var query = from o in DB.CreateQuery<C_Sports_Order_Running>()
                        where o.TicketStatus == (int)TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        //&& o.CreateTime < DateTime.Now.AddMinutes(-2)
                        && o.StopTime < DateTime.Now
                        && (o.GameCode== "CTZQ"|| o.GameCode == "SSQ" || o.GameCode == "DLT" || o.GameCode == "FC3D" || o.GameCode == "PL3")
                        orderby o.CreateTime ascending
                        select o;
            return query.ToList();
        }

        public List<C_Sports_Order_Running> QueryAllRunningOrder_gp()
        {

            var query = from o in DB.CreateQuery<C_Sports_Order_Running>()
                        where o.TicketStatus == (int)TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        //&& o.CreateTime < DateTime.Now.AddMinutes(-2)
                        && o.StopTime < DateTime.Now
                        && (o.GameCode == "CQSSC" || o.GameCode == "JX11X5" || o.GameCode == "SD11X5" || o.GameCode == "GD11X5" || o.GameCode == "GDKLSF"
                            || o.GameCode == "JSKS" || o.GameCode == "SDKLPK3" || o.GameCode == "BJDC" || o.GameCode == "JCZQ" || o.GameCode == "JCLQ")
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

        /// <summary>
        /// 查询被跟单人数
        /// </summary>
        public int QueryTogetherFollowerRecord(string userId, string gameCode, string gameType)
        {
          
            return DB.CreateQuery<C_Together_FollowerRule>().Where(p => p.CreaterUserId == userId && p.GameCode == gameCode && p.GameType == gameType).Count();
        }

        public List<Sports_TicketQueryInfo> QueryTicketInfoList(string schemeId, int pageIndex, int pageSize, out int totalCount)
        {
        
            var query = (from p in DB.CreateQuery<C_Sports_Ticket>()
                        where p.SchemeId == schemeId
                        orderby p.TicketId ascending
                        select p).ToList().Select(t=>new Sports_TicketQueryInfo
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = (BonusStatus)t.BonusStatus,
                            CreateTime = t.CreateTime,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            IssuseNumber = t.IssuseNumber,
                            PlayType = t.PlayType,
                            PreTaxBonusMoney = t.PreTaxBonusMoney,
                            BarCode = t.BarCode,
                            PrintNumber1 = t.PrintNumber1,
                            PrintNumber2 = t.PrintNumber2,
                            PrintNumber3 = t.PrintNumber3,
                            SchemeId = t.SchemeId,
                            TicketId = t.TicketId,
                            TicketStatus = (TicketStatus)t.TicketStatus,
                            BetContent = t.BetContent,
                            LocOdds = t.LocOdds,
                            PrintDateTime = t.PrintDateTime,
                        });

            totalCount = query.Count();
            if (pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<Sports_TicketQueryInfo> QueryTicketHisgoryInfoList(string schemeId, int pageIndex, int pageSize, out int totalCount)
        {
          
            var query = (from p in DB.CreateQuery<C_Sports_Ticket_History>()
                        where p.SchemeId == schemeId
                        orderby p.TicketId ascending
                        select p).ToList().Select(t=> new Sports_TicketQueryInfo
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = (BonusStatus)t.BonusStatus,
                            CreateTime = t.CreateTime,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            IssuseNumber = t.IssuseNumber,
                            PlayType = t.PlayType,
                            PreTaxBonusMoney = t.PreTaxBonusMoney,
                            BarCode = t.BarCode,
                            PrintNumber1 = t.PrintNumber1,
                            PrintNumber2 = t.PrintNumber2,
                            PrintNumber3 = t.PrintNumber3,
                            SchemeId = t.SchemeId,
                            TicketId = t.TicketId,
                            TicketStatus = (TicketStatus)t.TicketStatus,
                            BetContent = t.BetContent,
                            LocOdds = t.LocOdds,
                            PrintDateTime = t.PrintDateTime,
                        });

            totalCount = query.Count();
            if (pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public C_User_Attention QueryUserAttention(string currentUserId, string beAttentionUserId)
        {
        
            return DB.CreateQuery<C_User_Attention>().Where(p => p.BeAttentionUserId == beAttentionUserId
                && p.FollowerUserId == currentUserId).FirstOrDefault();
        }

        public void AddUserAttention(C_User_Attention entity)
        {
            DB.GetDal<C_User_Attention>().Add(entity);
        }

        public C_User_Attention_Summary QueryUserAttentionSummary(string currentUserId)
        {
       
            return DB.CreateQuery<C_User_Attention_Summary>().Where(p => p.UserId == currentUserId).FirstOrDefault();
        }

        public void DeleteUserSaveOrder_Sports(C_UserSaveOrder entity)
        {
            DB.GetDal<C_UserSaveOrder>().Delete(entity);
        }

        public List<C_Sports_TogetherJoin> QuerySports_JoinTogetherList(string schemeId)
        {
            return DB.CreateQuery<C_Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId).ToList();
        }

        public List<C_JCZQ_Match> QueryJCZQDSSaleMatchCount(string[] matchIdArray)
        {
            var query = from m in DB.CreateQuery<C_JCZQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        //&& m.DSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public List<TogetherFollowerRuleQueryInfo> QueryUserFollowRule(bool byFollower, string userId, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
           
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = byFollower ? (from f in DB.CreateQuery<C_Together_FollowerRule>()
                                      join u in DB.CreateQuery<C_User_Register>() on f.CreaterUserId equals u.UserId
                                      where (gameCode == "" || f.GameCode == gameCode)
                                      && (gameType == "" || f.GameType == gameType)
                                      && (userId == "" || f.FollowerUserId == userId)
                                      select new { f,u}).ToList().Select(b=> new TogetherFollowerRuleQueryInfo
                                      {
                                          RuleId = b.f.Id,
                                          BonusMoney = b.f.TotalBonusMoney,
                                          BuyMoney = b.f.TotalBetMoney,
                                          CancelNoBonusSchemeCount = b.f.CancelNoBonusSchemeCount,
                                          CancelWhenSurplusNotMatch = b.f.CancelWhenSurplusNotMatch,
                                          CreaterUserId = b.f.CreaterUserId,
                                          CreateTime = b.f.CreateTime,
                                          FollowerCount = b.f.FollowerCount,
                                          FollowerIndex = b.f.FollowerIndex,
                                          FollowerPercent = b.f.FollowerPercent,
                                          FollowerUserId = b.f.FollowerUserId,
                                          GameCode = b.f.GameCode,
                                          GameType = b.f.GameType,
                                          IsEnable = b.f.IsEnable,
                                          MaxSchemeMoney = b.f.MaxSchemeMoney,
                                          MinSchemeMoney = b.f.MinSchemeMoney,
                                          SchemeCount = b.f.SchemeCount,
                                          StopFollowerMinBalance = b.f.StopFollowerMinBalance,
                                          UserId = b.u.UserId,
                                          UserDisplayName = b.u.DisplayName,
                                          HideDisplayNameCount = b.u.HideDisplayNameCount,
                                      }) :
                                    (from f in DB.CreateQuery<C_Together_FollowerRule>()
                                     join u in DB.CreateQuery<C_User_Register>() on f.FollowerUserId equals u.UserId
                                     where (gameCode == "" || f.GameCode == gameCode)
                                     && (gameType == "" || f.GameType == gameType)
                                     && (userId == "" || f.CreaterUserId == userId)
                                     orderby f.FollowerIndex ascending
                                     select new { f, u }).ToList().Select(b => new TogetherFollowerRuleQueryInfo
                                     {
                                         RuleId = b.f.Id,
                                         BonusMoney = b.f.TotalBonusMoney,
                                         BuyMoney = b.f.TotalBetMoney,
                                         CancelNoBonusSchemeCount = b.f.CancelNoBonusSchemeCount,
                                         CancelWhenSurplusNotMatch = b.f.CancelWhenSurplusNotMatch,
                                         CreaterUserId = b.f.CreaterUserId,
                                         CreateTime = b.f.CreateTime,
                                         FollowerCount = b.f.FollowerCount,
                                         FollowerIndex = b.f.FollowerIndex,
                                         FollowerPercent = b.f.FollowerPercent,
                                         FollowerUserId = b.f.FollowerUserId,
                                         GameCode = b.f.GameCode,
                                         GameType = b.f.GameType,
                                         IsEnable = b.f.IsEnable,
                                         MaxSchemeMoney = b.f.MaxSchemeMoney,
                                         MinSchemeMoney = b.f.MinSchemeMoney,
                                         SchemeCount = b.f.SchemeCount,
                                         StopFollowerMinBalance = b.f.StopFollowerMinBalance,
                                         UserId = b.u.UserId,
                                         UserDisplayName = b.u.DisplayName,
                                         HideDisplayNameCount = b.u.HideDisplayNameCount,
                                     });

            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        }
        public List<TogetherFollowRecordInfo> QuerySucessFolloweRecord(string userId, long ruleId, string gameCode, int pageIndex, int pageSize, out int totalCount)
        {
          
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = (from r in DB.CreateQuery<C_Together_FollowerRecord>()
                        join u in DB.CreateQuery<C_User_Register>() on r.CreaterUserId equals u.UserId
                        join o in DB.CreateQuery<C_OrderDetail>() on r.SchemeId equals o.SchemeId
                        where r.FollowerUserId == userId && (ruleId < 1 || r.RuleId == ruleId)
                        && (gameCode == "" || r.GameCode == gameCode)
                        orderby r.CreateTime descending
                        select new {r,u,o }).ToList().Select(p=> new TogetherFollowRecordInfo
                        {
                            CreaterDisplayName = p.u.DisplayName,
                            CreaterUserId = p.u.UserId,
                            CreaterHideDisplayNameCount = p.u.HideDisplayNameCount,
                            CreateTime = p.r.CreateTime,
                            FollowBonusMoney = p.r.BonusMoney,
                            FollowMoney = p.r.BuyMoney,
                            GameCode = p.r.GameCode,
                            GameType = p.r.GameType,
                            GameCodeDisplayName = BettingHelper.FormatGameCode(p.r.GameCode),
                            GameTypeDisplayName = BettingHelper.FormatGameType(p.r.GameCode, p.r.GameType),
                            IssuseNumber = p.o.CurrentIssuseNumber,
                            ProgressStatus = (ProgressStatus)p.o.ProgressStatus,
                            SchemeId = p.o.SchemeId,
                            SchemeMoney = p.o.TotalMoney,
                            SchemeBonusMoney = p.o.AfterTaxBonusMoney,
                        });
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public ProfileUserInfo QueryProfileUserInfo(string userId)
        {
        
            var query = (from r in DB.CreateQuery<C_User_Register>()
                        join u in DB.CreateQuery<C_User_Attention_Summary>() on r.UserId equals u.UserId
                        where r.UserId == userId
                        select new {r,u }).ToList().Select(p=> new ProfileUserInfo
                        {
                            UserId = p.r.UserId,
                            AttentionCount = p.u.FollowerUserCount,
                            AttentionedCount = p.u.BeAttentionUserCount,
                            HideNameCount = p.r.HideDisplayNameCount,
                            UserDisplayName = p.r.DisplayName,
                            CreateTime = p.r.CreateTime,
                        });
            return query.FirstOrDefault();
        }

        public List<UserBeedingListInfo> QueryUserBeedingList(string gameCode, string gameType, string userId, string userDisplayName, int pageIndex, int pageSize,
      QueryUserBeedingListOrderByProperty orderByProperty, OrderByCategory orderByCategory, out int totalCount)
        {
           
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = (from b in DB.CreateQuery<C_User_Beedings>()
                        join u in DB.CreateQuery<C_User_Register>() on b.UserId equals u.UserId
                        where b.TotalBonusMoney > 0M && b.TotalBonusTimes > 0
                        && (string.Empty == gameCode || b.GameCode == gameCode)
                        && (string.Empty == gameType || b.GameType == gameType)
                        && (string.Empty == userId || u.UserId == userId)
                        && (string.Empty == userDisplayName || u.DisplayName == userDisplayName)
                        select new {b,u }).ToList().Select(p=>new UserBeedingListInfo
                        {
                            BeFollowedTotalMoney = p.b.BeFollowedTotalMoney,
                            BeFollowerUserCount = p.b.BeFollowerUserCount,
                            GameCode = p.b.GameCode,
                            GameType = p.b.GameType,
                            GoldCrownCount = p.b.GoldCrownCount,
                            GoldCupCount = p.b.GoldCupCount,
                            GoldDiamondsCount = p.b.GoldDiamondsCount,
                            GoldStarCount = p.b.GoldStarCount,
                            SilverCrownCount = p.b.SilverCrownCount,
                            SilverCupCount = p.b.SilverCupCount,
                            SilverDiamondsCount = p.b.SilverDiamondsCount,
                            SilverStarCount = p.b.SilverStarCount,
                            TotalBetMoney = p.b.TotalBetMoney,
                            TotalOrderCount = p.b.TotalOrderCount,
                            TotalBonusMoney = p.b.TotalBonusMoney,
                            TotalBonusTimes = p.b.TotalBonusTimes,
                            UserDisplayName = p.u.DisplayName,
                            UserHideDisplayNameCount = p.u.HideDisplayNameCount,
                            UserId = p.b.UserId,
                        });

            #region 排序

            switch (orderByProperty)
            {
                case QueryUserBeedingListOrderByProperty.TotalBonusMoney:
                    switch (orderByCategory)
                    {
                        case OrderByCategory.DESC:
                            query.OrderByDescending(p => p.TotalBonusMoney);
                            break;
                        case OrderByCategory.ASC:
                            query.OrderBy(p => p.TotalBonusMoney);
                            break;
                    }
                    break;
                case QueryUserBeedingListOrderByProperty.TotalBonusTimes:
                    switch (orderByCategory)
                    {
                        case OrderByCategory.DESC:
                            query.OrderByDescending(p => p.TotalBonusTimes);
                            break;
                        case OrderByCategory.ASC:
                            query.OrderBy(p => p.TotalBonusTimes);
                            break;
                    }
                    break;
                case QueryUserBeedingListOrderByProperty.BeFollowedTotalMoney:
                    switch (orderByCategory)
                    {
                        case OrderByCategory.DESC:
                            query.OrderByDescending(p => p.BeFollowedTotalMoney);
                            break;
                        case OrderByCategory.ASC:
                            query.OrderBy(p => p.BeFollowedTotalMoney);
                            break;
                    }
                    break;
                case QueryUserBeedingListOrderByProperty.BeFollowerUserCount:
                    switch (orderByCategory)
                    {
                        case OrderByCategory.DESC:
                            query.OrderByDescending(p => p.BeFollowerUserCount);
                            break;
                        case OrderByCategory.ASC:
                            query.OrderBy(p => p.BeFollowerUserCount);
                            break;
                    }
                    break;
            }

            #endregion

            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();


        }

        public List<C_JCLQ_Match> QueryJCLQDSSaleMatchCount(string[] matchIdArray)
        {
            var query = from m in DB.CreateQuery<C_JCLQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        //&& m.DSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public void AddSingleScheme_AnteCode(C_SingleScheme_AnteCode entity)
        {
            DB.GetDal<C_SingleScheme_AnteCode>().Add(entity);
        }

        public UserCurrentOrderInfoCollection QueryUserCurrentOrderList(string userId, string gameCode, int pageIndex, int pageSize)
        {

            var listCount = new UserCurrentOrderInfoCollection();

            var query = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Order_QueryUserCurrentOrderList").SQL;
            listCount.List = DB.CreateSQLQuery(query).SetString("UserId", userId)
                .SetString("GameCode", gameCode)
                .SetInt("PageIndex", pageIndex)
                .SetInt("PageSize", pageSize)
                .List<UserCurrentOrderInfo>().ToList();
        

            var queryCount= SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Order_QueryUserCurrentOrderListCount").SQL;
            listCount.TotalCount = DB.CreateSQLQuery(queryCount).SetString("UserId", userId)
                .SetString("GameCode", gameCode)
                .Excute();

            //.AddInParameter("UserId", userId)
            //.AddInParameter("GameCode", gameCode)
            //.AddInParameter("PageIndex", pageIndex)
            //.AddInParameter("PageSize", pageSize)
            //.AddOutParameter("TotalCount", "Int32");

            //var dt = query.GetDataTable(out outputs);
            //totalCount = (int)outputs["TotalCount"];
            //foreach (DataRow row in dt.Rows)
            //{
            //    list.Add(new UserCurrentOrderInfo
            //    {
            //        UserId = UsefullHelper.GetDbValue<string>(row[0]),
            //        UserDisplayName = UsefullHelper.GetDbValue<string>(row[1]),
            //        HideDisplayNameCount = UsefullHelper.GetDbValue<int>(row[2]),
            //        SchemeId = UsefullHelper.GetDbValue<string>(row[3]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(row[4]),
            //        GameCode = UsefullHelper.GetDbValue<string>(row[5]),
            //        GameCodeName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(row[5])),
            //        GameTypeName = UsefullHelper.GetDbValue<string>(row[6]),
            //        SchemeType = UsefullHelper.GetDbValue<SchemeType>(row[7]),
            //        TotalMoney = UsefullHelper.GetDbValue<decimal>(row[8]),
            //        Progress = row[9] == DBNull.Value ? 1M : UsefullHelper.GetDbValue<decimal>(row[9]),
            //        JoinType = TogetherJoinType.Join,
            //    });
            //}
            return listCount;
        }

        public C_Sports_AnteCode QueryAnteCode(string schemeId, string macthId, string gameType)
        {
            return DB.CreateQuery<C_Sports_AnteCode>().Where(s => s.SchemeId == schemeId && s.MatchId == macthId && s.GameType == gameType).FirstOrDefault();
        }

        /// <summary>
        /// 查询用户保存的订单
        /// </summary>
        public List<SaveOrder_LotteryBettingInfo> QuerySaveOrderLottery(string userId)
        {

            DateTime beginTime = DateTime.Today.AddDays(-30);
            DateTime endTime = DateTime.Today.AddDays(1);
            var query = from g in DB.CreateQuery<C_UserSaveOrder>()
                        join i in DB.CreateQuery<C_User_Register>() on g.UserId equals i.UserId
                        orderby g.CreateTime descending
                        where g.UserId == userId
                        && (g.CreateTime > beginTime && g.CreateTime <= endTime)
                        select new { g, i };
            return query.ToList().Select(p => new SaveOrder_LotteryBettingInfo
            {
                SchemeId = p.g.SchemeId,
                DisplayName = p.i.DisplayName,
                UserId = p.g.UserId,
                GameCode = p.g.GameCode,
                GameType = p.g.GameType,
                StrStopTime = p.g.StrStopTime,
                PlayType = p.g.PlayType,
                SchemeType = (SchemeType)p.g.SchemeType,
                SchemeSource = (SchemeSource)p.g.SchemeSource,
                SchemeBettingCategory = (SchemeBettingCategory)p.g.SchemeBettingCategory,
                ProgressStatus = (ProgressStatus)p.g.ProgressStatus,
                IssuseNumber = p.g.IssuseNumber,
                Amount = p.g.Amount,
                BetCount = p.g.BetCount,
                TotalMoney = p.g.TotalMoney,
                StopTime = p.g.StopTime,
                CreateTime = p.g.CreateTime,
            }).ToList();
        }

        public Sports_SchemeQueryInfo QuerySports_Order_ComplateInfo(string schemeId)
        {
           
            var query = from r in DB.CreateQuery<C_Sports_Order_Complate>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where r.SchemeId == schemeId
                        select new {r,u };
            var info = query.ToList().Select(p => new Sports_SchemeQueryInfo
            {
                UserId = p.u.UserId,
                UserDisplayName = p.u.DisplayName,
                HideDisplayNameCount = p.u.HideDisplayNameCount,
                GameDisplayName = BettingHelper.FormatGameCode(p.r.GameCode),
                GameCode = p.r.GameCode,
                Amount = p.r.Amount,
                BonusStatus = (BonusStatus)p.r.BonusStatus,
                CreateTime = p.r.CreateTime,
                GameType = p.r.GameType,
                GameTypeDisplayName = BettingHelper.FormatGameType(p.r.GameCode, p.r.GameType),
                IssuseNumber = p.r.IssuseNumber,
                PlayType = p.r.PlayType,
                ProgressStatus = (ProgressStatus)p.r.ProgressStatus,
                SchemeId = p.r.SchemeId,
                SchemeType = (SchemeType)p.r.SchemeType,
                TicketId = p.r.TicketId,
                TicketLog = p.r.TicketLog,
                TicketStatus = (TicketStatus)p.r.TicketStatus,
                TotalMatchCount = p.r.TotalMatchCount,
                TotalMoney = p.r.TotalMoney,
                BetCount = p.r.BetCount,
                PreTaxBonusMoney = p.r.PreTaxBonusMoney,
                AfterTaxBonusMoney = p.r.AfterTaxBonusMoney,
                BonusCount = p.r.BonusCount,
                IsPrizeMoney = p.r.IsPrizeMoney,
                Security = (TogetherSchemeSecurity)p.r.Security,
                IsVirtualOrder = p.r.IsVirtualOrder,
                StopTime = p.r.StopTime,
                HitMatchCount = p.r.HitMatchCount,
                AddMoney = p.r.AddMoney,
                AddMoneyDescription = p.r.AddMoneyDescription,
                SchemeBettingCategory = (SchemeBettingCategory)p.r.SchemeBettingCategory,
                TicketProgress = p.r.TicketProgress,
                DistributionWay = (AddMoneyDistributionWay)p.r.DistributionWay,
                Attach = p.r.Attach,
                MaxBonusMoney = p.r.MaxBonusMoney,
                MinBonusMoney = p.r.MinBonusMoney,
                ExtensionOne = p.r.ExtensionOne,
                IsAppend = p.r.IsAppend == false ? false : p.r.IsAppend,
                ComplateDateTime = p.r.ComplateDateTime,
                BetTime = p.r.BetTime,
                SchemeSource = (SchemeSource)p.r.SchemeSource,
                RedBagMoney = p.r.RedBagMoney,
                TicketTime = p.r.TicketTime,
                RedBagAwardsMoney = p.r.AddMoneyDescription == "70" ? p.r.AddMoney : 0,
                BonusAwardsMoney = p.r.AddMoneyDescription == "10" ? p.r.AddMoney : 0,
            }).FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                if (info.GameCode == "SJB")
                    key = string.Format("{0}|{1}|{2}", info.GameCode, info.GameType == "冠亚军" ? "GYJ" : "GJ", info.IssuseNumber);
                var gameIssuse = DB.CreateQuery<C_Game_Issuse>().Where(g => g.GameCode_IssuseNumber == key).FirstOrDefault();
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }

        public Sports_SchemeQueryInfo QuerySports_Order_RunningInfo(string schemeId)
        {
         
            var query = from r in DB.CreateQuery<C_Sports_Order_Running>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where r.SchemeId == schemeId
                        select new { r,u};
            var info = query.ToList().Select(p => new Sports_SchemeQueryInfo
            {
                UserId = p.u.UserId,
                UserDisplayName = p.u.DisplayName,
                HideDisplayNameCount = p.u.HideDisplayNameCount,
                GameCode = p.r.GameCode,
                Amount = p.r.Amount,
                BonusStatus = (BonusStatus)p.r.BonusStatus,
                CreateTime = p.r.CreateTime,
                GameType = p.r.GameType,
                IssuseNumber = p.r.IssuseNumber,
                PlayType = p.r.PlayType,
                ProgressStatus = (ProgressStatus)p.r.ProgressStatus,
                SchemeId = p.r.SchemeId,
                SchemeType = (SchemeType)p.r.SchemeType,
                TicketId = p.r.TicketId,
                TicketLog = p.r.TicketLog,
                TicketStatus = (TicketStatus)p.r.TicketStatus,
                TotalMatchCount = p.r.TotalMatchCount,
                TotalMoney = p.r.TotalMoney,
                BetCount = p.r.BetCount,
                GameDisplayName = BettingHelper.FormatGameCode(p.r.GameCode),
                GameTypeDisplayName = BettingHelper.FormatGameType(p.r.GameCode, p.r.GameType),
                AfterTaxBonusMoney = 0M,
                PreTaxBonusMoney = 0M,
                BonusCount = 0,
                WinNumber = string.Empty,
                IsPrizeMoney = false,
                Security = (TogetherSchemeSecurity)p.r.Security,
                IsVirtualOrder = p.r.IsVirtualOrder,
                StopTime = p.r.StopTime,
                HitMatchCount = p.r.HitMatchCount,
                AddMoney = 0M,
                AddMoneyDescription = string.Empty,
                SchemeBettingCategory = (SchemeBettingCategory)p.r.SchemeBettingCategory,
                TicketProgress = p.r.TicketProgress,
                DistributionWay = AddMoneyDistributionWay.Average,
                Attach = p.r.Attach,
                MaxBonusMoney = p.r.MaxBonusMoney,
                MinBonusMoney = p.r.MinBonusMoney,
                ExtensionOne = p.r.ExtensionOne,
                IsAppend = p.r.IsAppend == false ? false : p.r.IsAppend,
                BetTime = p.r.BetTime,
                SchemeSource = (SchemeSource)p.r.SchemeSource,
                TicketTime = p.r.TicketTime,
                RedBagMoney = p.r.RedBagMoney,
            }).FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                if (info.GameCode == "SJB")
                    key = string.Format("{0}|{1}|{2}", info.GameCode, info.GameType == "冠亚军" ? "GYJ" : "GJ", info.IssuseNumber);
                var gameIssuse = DB.CreateQuery<C_Game_Issuse>().Where(g => g.GameCode_IssuseNumber == key).FirstOrDefault();
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }

        public TicketId_QueryInfoCollection QueryPrizeTicket_OrderIdList(string gameCode, string schemeId)
        {

            // 通过数据库存储过程进行查询
            var query = SqlModule.AdminModule.FirstOrDefault(x => x.Key == "P_Prize_OrderTicketList").SQL;
            var MatchInfo = DB.CreateSQLQuery(query).SetString("GameCode", gameCode).SetString("SchemeId", schemeId).List<MatchInfo>().ToList();

            var result = new TicketId_QueryInfoCollection();
            result.SportsTicketList = DB.CreateQuery<C_Sports_Ticket>().Where(p => p.SchemeId == schemeId).ToList();
            result.MatchList = MatchInfo;
            result.TotalTicketCount = result.SportsTicketList.Count;
            result.TotalMatchCount = result.MatchList.Count;
            return result;
        }

        public List<C_Sports_Order_Running> QueryBrotherSports_Order_Running(string schemeId)
        {
           
            var result = new List<C_Sports_Order_Running>();
            var sql = string.Format(@"select r.SchemeId,r.UserId,r.GameCode,
                                    r.GameType,r.PlayType,r.SchemeType,
                                    r.SchemeSource,r.SchemeBettingCategory,
                                    r.IssuseNumber,r.Amount,r.BetCount,r.TotalMatchCount,
                                    r.TotalMoney,r.StopTime,r.TicketStatus,r.TicketId,
                                    r.TicketLog,r.ProgressStatus,r.BonusStatus,
                                    r.HitMatchCount,r.PreTaxBonusMoney,r.AfterTaxBonusMoney,
                                    r.CanChase,r.IsVirtualOrder,r.CreateTime,r.AgentId,r.Security
 
                                    from C_Sports_Order_Running r
                                    where r.SchemeId in (
                                    select t.SchemeId 
                                    from C_Lottery_Scheme t
                                    where t.KeyLine =(select s.KeyLine from C_Lottery_Scheme s where s.SchemeId='{0}')
                                    )
                                    order by r.IssuseNumber ASC", schemeId);
            var array = DB.CreateSQLQuery(sql).List<C_Sports_Order_Running>();
            //if (array == null)
            //    return result;
            //foreach (var item in array)
            //{
              
            //    result.Add(new Sports_Order_Running
            //    {
            //        SchemeId = item.,
            //        UserId = UsefullHelper.GetDbValue<string>(row[1]),
            //        GameCode = UsefullHelper.GetDbValue<string>(row[2]),
            //        GameType = UsefullHelper.GetDbValue<string>(row[3]),
            //        PlayType = UsefullHelper.GetDbValue<string>(row[4]),
            //        SchemeType = UsefullHelper.GetDbValue<SchemeType>(row[5]),
            //        SchemeSource = UsefullHelper.GetDbValue<SchemeSource>(row[6]),
            //        SchemeBettingCategory = UsefullHelper.GetDbValue<SchemeBettingCategory>(row[7]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(row[8]),
            //        Amount = UsefullHelper.GetDbValue<int>(row[9]),
            //        BetCount = UsefullHelper.GetDbValue<int>(row[10]),
            //        TotalMatchCount = UsefullHelper.GetDbValue<int>(row[11]),
            //        TotalMoney = UsefullHelper.GetDbValue<decimal>(row[12]),
            //        StopTime = UsefullHelper.GetDbValue<DateTime>(row[13]),
            //        TicketStatus = UsefullHelper.GetDbValue<TicketStatus>(row[14]),
            //        TicketId = UsefullHelper.GetDbValue<string>(row[15]),
            //        TicketLog = UsefullHelper.GetDbValue<string>(row[16]),
            //        ProgressStatus = UsefullHelper.GetDbValue<ProgressStatus>(row[17]),
            //        BonusStatus = UsefullHelper.GetDbValue<BonusStatus>(row[18]),
            //        HitMatchCount = UsefullHelper.GetDbValue<int>(row[19]),
            //        PreTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(row[20]),
            //        AfterTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(row[21]),
            //        CanChase = UsefullHelper.GetDbValue<bool>(row[22]),
            //        IsVirtualOrder = UsefullHelper.GetDbValue<bool>(row[23]),
            //        CreateTime = UsefullHelper.GetDbValue<DateTime>(row[24]),
            //        AgentId = UsefullHelper.GetDbValue<string>(row[25]),
            //        Security = UsefullHelper.GetDbValue<TogetherSchemeSecurity>(row[26]),
            //    });
            //}
            return result;
        }
        public C_Together_FollowerRecord QueryFollowerRecordBySchemeId(string schemeId, string followerUserId)
        {
         
            return DB.CreateQuery<C_Together_FollowerRecord>().Where(p => p.SchemeId == schemeId && p.FollowerUserId == followerUserId).FirstOrDefault();
        }

        public C_Temp_Together QueryTemp_Together(string schemeId)
        {
         
            return DB.CreateQuery<C_Temp_Together>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }

        public void DeleteTemp_Together(C_Temp_Together entity)
        {
            DB.GetDal<C_Temp_Together>().Delete(entity);
        }

        public void UpdateSports_Ticket(C_Sports_Ticket entity)
        {
            DB.GetDal<C_Sports_Ticket>().Update(entity);
        }

        public List<Sports_SchemeQueryInfo> QueryWaitForPrizeMoneyOrderList(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize, out int totalCount)
        {
         
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = (from r in DB.CreateQuery<C_Sports_Order_Complate>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        where (r.ComplateDateTime >= startTime && r.ComplateDateTime < endTime)
                        && (gameCode == "" || r.GameCode == gameCode)
                        && (r.AfterTaxBonusMoney + r.AddMoney) > 0M
                        //&& r.BonusStatus == BonusStatus.Win
                        && r.IsPrizeMoney == false
                        && r.IsVirtualOrder == false select new {r,u }
                         ).ToList().Select(p => new Sports_SchemeQueryInfo
                        {

                            UserId = p.u.UserId,
                            UserDisplayName = p.u.DisplayName,
                            HideDisplayNameCount = p.u.HideDisplayNameCount,
                            GameDisplayName = BettingHelper.FormatGameCode(p.r.GameCode),
                            GameCode = p.r.GameCode,
                            Amount = p.r.Amount,
                            BonusStatus = (BonusStatus)p.r.BonusStatus,
                            CreateTime = p.r.CreateTime,
                            GameType = p.r.GameType,
                            GameTypeDisplayName = p.r.SchemeBettingCategory == (int)SchemeBettingCategory.ErXuanYi ? "主客二选一" : BettingHelper.FormatGameType(p.r.GameCode, p.r.GameType),
                            IssuseNumber = p.r.IssuseNumber,
                            PlayType = p.r.PlayType,
                            Security = (TogetherSchemeSecurity)p.r.Security,
                            IsVirtualOrder = p.r.IsVirtualOrder,
                            ProgressStatus = (ProgressStatus)p.r.ProgressStatus,
                            SchemeId = p.r.SchemeId,
                            SchemeType = (SchemeType)p.r.SchemeType,
                            TicketId = p.r.TicketId,
                            TicketLog = p.r.TicketLog,
                            TicketStatus = (TicketStatus)p.r.TicketStatus,
                            TotalMatchCount = p.r.TotalMatchCount,
                            TotalMoney = p.r.TotalMoney,
                            BetCount = p.r.BetCount,
                            PreTaxBonusMoney = p.r.PreTaxBonusMoney,
                            AfterTaxBonusMoney = p.r.AfterTaxBonusMoney,
                            BonusCount = p.r.BonusCount,
                            IsPrizeMoney = p.r.IsPrizeMoney,
                            HitMatchCount = p.r.HitMatchCount,
                            StopTime = p.r.StopTime,
                            AddMoney = p.r.AddMoney,
                            AddMoneyDescription = p.r.AddMoneyDescription,
                        });
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<C_Sports_Order_Complate> QuerySports_Order_ComplateByComplateDate(string complateDate)
        {
            
            var query = from c in DB.CreateQuery<C_Sports_Order_Complate>()
                        where c.ComplateDate == complateDate
                        orderby c.ComplateDateTime ascending
                        select c;
            return query.ToList();
        }

        public List<C_User_Beedings> QueryUserBeedingsList(string gameCode, string gameType)
        {
          
            return DB.CreateQuery<C_User_Beedings>().Where(p => p.GameCode == gameCode && (gameType == "" || p.GameType == gameType)).ToList();
        }

        public List<C_Sports_Order_Complate> QuerySports_Order_ComplateByComplateTime(string userId, string gameCode, string gameType, DateTime startTime, DateTime endTime)
        {
           
            var query = from c in DB.CreateQuery<C_Sports_Order_Complate>()
                        where c.ComplateDateTime >= startTime && c.ComplateDateTime < endTime
                        && c.IsVirtualOrder == false
                        && c.UserId == userId && (gameCode == "" || c.GameCode == gameCode) && (gameType == "" || c.GameType == gameType)
                        orderby c.ComplateDateTime descending
                        select c;
            return query.ToList();
        }
        public List<C_Sports_Order_Complate> QuerySports_Order_ComplateByComplateTime(DateTime startTime, DateTime endTime)
        {
          
            var query = from c in DB.CreateQuery<C_Sports_Order_Complate>()
                        where c.ComplateDateTime >= startTime && c.ComplateDateTime < endTime
                        && c.IsVirtualOrder == false
                        orderby c.ComplateDateTime descending
                        select c;
            return query.ToList();
        }

        public void UpdateUserBonusPercent(C_User_BonusPercent entity)
        {
            DB.GetDal<C_User_BonusPercent>().Update(entity);
        }

        public List<C_Sports_Order_Complate> QueryWinSports_Order_ComplateByComplateTime(string userId, DateTime startTime, DateTime endTime)
        {
          
            var query = from c in DB.CreateQuery<C_Sports_Order_Complate>()
                        where c.ComplateDateTime >= startTime && c.ComplateDateTime < endTime
                        && c.IsVirtualOrder == false
                        && c.UserId == userId
                        && c.BonusStatus == (int)BonusStatus.Win
                        orderby c.ComplateDateTime descending
                        select c;
            return query.ToList();
        }

        public List<TicketPrizeInfo> QuerySZCUnPrizeTicket(string gameCode, int count)
        {
            if (count <= 0)
                count = 100;
          
            var sql = string.Format(@"select top {0} t.ticketId,t.gamecode,t.gametype,t.betcontent,t.amount,t.isappend,t.issusenumber,i.WinNumber,t.id
                                        from C_Sports_Ticket t
                                        left join C_Game_Issuse i on t.gamecode=i.gamecode and t.IssuseNumber=i.IssuseNumber
                                        where t.BonusStatus=0 and t.TicketStatus=90 
                                        and i.WinNumber is not null 
                                        and i.Status=30
                                        and  i.WinNumber<>''
                                        and i.gamecode='{1}'
                                        order by t.issusenumber asc", count, gameCode);
            var list = DB.CreateSQLQuery(sql).List<TicketPrizeInfo>().ToList();
            //var ticketList = new List<TicketPrizeInfo>();
            //foreach (var item in list)
            //{
            //    if (item == null)
            //        continue;
            //    var array = item as object[];
            //    ticketList.Add(new TicketPrizeInfo
            //    {
            //        TicketId = UsefullHelper.GetDbValue<string>(array[0]),
            //        GameCode = UsefullHelper.GetDbValue<string>(array[1]),
            //        GameType = UsefullHelper.GetDbValue<string>(array[2]),
            //        BetContent = UsefullHelper.GetDbValue<string>(array[3]),
            //        Amount = UsefullHelper.GetDbValue<int>(array[4]),
            //        IsAppend = UsefullHelper.GetDbValue<bool>(array[5]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(array[6]),
            //        WinNumber = UsefullHelper.GetDbValue<string>(array[7]),
            //        Id = UsefullHelper.GetDbValue<long>(array[8]),
            //    });
            //}
            return list;
        }
        public List<TicketPrizeInfo> QueryCTZQUnPrizeticket(string gameType, int count)
        {
            if (count <= 0)
                count = 100;
        
            var sql = string.Format(@"select top {0} t.ticketId,t.gamecode,t.gametype,t.betcontent,t.amount,t.isappend,t.issusenumber,p.WinNumber,t.id
                                    from C_Sports_Ticket t
                                    left join [T_Ticket_BonusPool] p on t.gamecode=p.gamecode and t.gametype=p.gametype and t.issusenumber =p.issusenumber
                                    where t.gamecode='ctzq' and t.gametype='{1}' and t.bonusstatus=0 and t.TicketStatus=90 
                                    and p.bonuslevel=1 and p.BonusCount>0 and p.BonusMoney>0
                                    and p.WinNumber<>''
                                    order by t.issusenumber asc ", count, gameType);
            var list = DB.CreateSQLQuery(sql).List<TicketPrizeInfo>().ToList();
            //var ticketList = new List<TicketPrizeInfo>();
            //foreach (var item in list)
            //{
            //    if (item == null)
            //        continue;
            //    var array = item as object[];
            //    ticketList.Add(new TicketPrizeInfo
            //    {
            //        TicketId = UsefullHelper.GetDbValue<string>(array[0]),
            //        GameCode = UsefullHelper.GetDbValue<string>(array[1]),
            //        GameType = UsefullHelper.GetDbValue<string>(array[2]),
            //        BetContent = UsefullHelper.GetDbValue<string>(array[3]),
            //        Amount = UsefullHelper.GetDbValue<int>(array[4]),
            //        IsAppend = UsefullHelper.GetDbValue<bool>(array[5]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(array[6]),
            //        WinNumber = UsefullHelper.GetDbValue<string>(array[7]),
            //        Id = UsefullHelper.GetDbValue<long>(array[8]),
            //    });
            //}
            return list;
        }

        public C_Sports_AnteCode QueryOneSportsAnteCodeBySchemeId(string schemeId)
        {
          
            return DB.CreateQuery<C_Sports_AnteCode>().Where(p => p.SchemeId == schemeId).FirstOrDefault();
        }
    }
}
