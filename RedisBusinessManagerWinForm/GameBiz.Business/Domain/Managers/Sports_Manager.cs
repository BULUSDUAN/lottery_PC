using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Core;
using GameBiz.Business;
using Common.Expansion;
using Common.Utilities;
using System.Data;
using System.Linq.Expressions;
using GameBiz.Core.Ticket;

namespace GameBiz.Domain.Managers
{
    public class Sports_Manager : GameBizEntityManagement
    {
        public void AddSports_Order_Running(Sports_Order_Running entity)
        {
            this.Add<Sports_Order_Running>(entity);
        }
        public void AddSports_Order_Complate(Sports_Order_Complate entity)
        {
            this.Add<Sports_Order_Complate>(entity);
        }
        public void AddSports_AnteCode(Sports_AnteCode entity)
        {
            this.Add<Sports_AnteCode>(entity);
        }
        public void AddSports_Together(Sports_Together entity)
        {
            this.Add<Sports_Together>(entity);
        }
        public void AddSports_TogetherJoin(Sports_TogetherJoin entity)
        {
            this.Add<Sports_TogetherJoin>(entity);
        }
        public void AddTemp_Together(Temp_Together entity)
        {
            this.Add<Temp_Together>(entity);
        }
        public void AddTogetherFollowerRecord(TogetherFollowerRecord entity)
        {
            this.Add<TogetherFollowerRecord>(entity);
        }
        public void AddTogetherFollowerRule(TogetherFollowerRule entity)
        {
            this.Add<TogetherFollowerRule>(entity);
        }
        public void AddUserBeedings(UserBeedings entity)
        {
            this.Add<UserBeedings>(entity);
        }
        public void AddUserAttentionSummary(UserAttentionSummary entity)
        {
            this.Add<UserAttentionSummary>(entity);
        }
        public void AddUserAttention(UserAttention entity)
        {
            this.Add<UserAttention>(entity);
        }
        public void AddUserBonusPercent(UserBonusPercent entity)
        {
            this.Add<UserBonusPercent>(entity);
        }
        public void AddLotteryScheme(LotteryScheme entity)
        {
            this.Add<LotteryScheme>(entity);
        }
        public void AddSingleScheme_AnteCode(SingleScheme_AnteCode entity)
        {
            this.Add<SingleScheme_AnteCode>(entity);
        }
        public void AddUserSaveOrder(UserSaveOrder entity)
        {
            this.Add<UserSaveOrder>(entity);
        }
        public void AddSports_Ticket(Sports_Ticket entity)
        {
            this.Add<Sports_Ticket>(entity);
        }
        public void AddSports_Ticket_History(Sports_Ticket_History entity)
        {
            this.Add<Sports_Ticket_History>(entity);
        }
        public void AddYouHuaScheme_AnteCode(YouHuaScheme_AnteCode entity)
        {
            this.Add<YouHuaScheme_AnteCode>(entity);
        }
        public void AddSingleSchemeOrder(SingleSchemeOrder entity)
        {
            this.Add<SingleSchemeOrder>(entity);
        }
        public void AddIndexMatch(IndexMatch entity)
        {
            this.Add<IndexMatch>(entity);
        }

        public void DeleteSports_Order_Running(Sports_Order_Running entity)
        {
            this.Delete(entity);
        }
        public void DeleteTogetherFollowerRule(TogetherFollowerRule entity)
        {
            this.Delete(entity);
        }
        public void DeleteTemp_Together(Temp_Together entity)
        {
            this.Delete(entity);
        }
        public void DeleteUserAttention(UserAttention entity)
        {
            this.Delete<UserAttention>(entity);
        }
        public void DeleteUserSaveOrder_Sports(UserSaveOrder entity)
        {
            this.Delete<UserSaveOrder>(entity);
        }

        public void UpdateSports_AnteCode(Sports_AnteCode entity)
        {
            this.Update<Sports_AnteCode>(entity);
        }
        public void UpdateSports_Order_Running(params Sports_Order_Running[] entity)
        {
            this.Update<Sports_Order_Running>(entity);
        }
        public void UpdateSports_Order_Complate(params Sports_Order_Complate[] entity)
        {
            this.Update<Sports_Order_Complate>(entity);
        }
        public void UpdateSports_Together(Sports_Together entity)
        {
            this.Update<Sports_Together>(entity);
        }
        public void UpdateSports_TogetherJoin(Sports_TogetherJoin entity)
        {
            this.Update<Sports_TogetherJoin>(entity);
        }
        public void UpdateUserBeedings(params UserBeedings[] entity)
        {
            this.Update<UserBeedings>(entity);
        }
        public void UpdateUserAttentionSummary(UserAttentionSummary entity)
        {
            this.Update<UserAttentionSummary>(entity);
        }
        public void UpdateUserBonusPercent(UserBonusPercent entity)
        {
            this.Update<UserBonusPercent>(entity);
        }
        public void UpdateTogetherFollowerRule(TogetherFollowerRule entity)
        {
            this.Update<TogetherFollowerRule>(entity);
        }
        public void UpdateTogetherFollowerRecord(TogetherFollowerRecord entity)
        {
            this.Update<TogetherFollowerRecord>(entity);
        }
        public void UpdateUserSaveOrder(UserSaveOrder entity)
        {
            this.Update<UserSaveOrder>(entity);
        }
        public void UpdateSports_Ticket(Sports_Ticket entity)
        {
            this.Update<Sports_Ticket>(entity);
        }
        public void UpdateSports_TicketList(params Sports_Ticket[] entity)
        {
            this.Update<Sports_Ticket>(entity);
        }
        public void UpdateTempTogether(Temp_Together entity)
        {
            this.Update<Temp_Together>(entity);
        }
        public void UpdateLotteryScheme(LotteryScheme entity)
        {
            this.Update(entity);
        }
        public void UpdateIndexMatch(IndexMatch entity)
        {
            this.Update<IndexMatch>(entity);
        }

        public SingleSchemeOrder QuerySingleSchemeOrder(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<SingleSchemeOrder>().FirstOrDefault(p => p.OrderId == schemeId);
        }

        public List<BJDC_Match> QueryBJDCSaleMatchCount(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<BJDC_Match>()
                        where matchIdArray.Contains(m.Id)
                        && m.LocalStopTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public List<BJDC_Match> QueryBJDCCurrentMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<BJDC_Match>()
                        where m.LocalStopTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public List<Cache_BJDC_MatchInfo> QueryBJDC_Current_CacheMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<BJDC_Match>()
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

        public List<JCZQ_Match> QueryJCZQSaleMatchCount(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public List<Cache_JCZQ_MatchInfo> QueryJCZQ_Current_CacheMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_Match>()
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

        public List<JCZQ_Match> QueryJCZQCurrentMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_Match>()
                        where m.FSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }
        public List<JCZQ_OZBMatch> QueryJCZQ_OZBCurrentMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_OZBMatch>()
                        where m.BetState == "开售"
                        select m;
            return query.ToList();
        }

        public List<JCZQ_MatchResult> QueryJCZQMatchResult(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_MatchResult>()
                        where matchIdArray.Contains(m.MatchId)
                        select m;
            return query.ToList();
        }
        public List<JCLQ_Match> QueryJCLQSaleMatchCount(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }
        public List<JCLQ_Match> QueryJCLQCurrentMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_Match>()
                        where m.FSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }
        public List<Cache_JCLQ_MatchInfo> QueryJCLQ_Current_CacheMatchList()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_Match>()
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

        public List<JCLQ_MatchResult> QueryJCLQMatchResult(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_MatchResult>()
                        where matchIdArray.Contains(m.MatchId)
                        select m;
            return query.ToList();
        }
        public List<JCZQ_Match> QueryJCZQDSSaleMatchCount(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        //&& m.DSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }
        public List<JCLQ_Match> QueryJCLQDSSaleMatchCount(string[] matchIdArray)
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_Match>()
                        where matchIdArray.Contains(m.MatchId)
                        && m.FSStopBettingTime > DateTime.Now
                        //&& m.DSStopBettingTime > DateTime.Now
                        select m;
            return query.ToList();
        }

        public UserSaveOrder QuerySaveOrder(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<UserSaveOrder>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public BJDC_Match QueryBJDC_Match(string id)
        {
            Session.Clear();
            return this.Session.Query<BJDC_Match>().FirstOrDefault(p => p.Id == id);
        }
        public BJDC_MatchResult QueryBJDC_MatchResult(string id)
        {
            Session.Clear();
            return this.Session.Query<BJDC_MatchResult>().FirstOrDefault(p => p.Id == id);
        }
        public JCZQ_Match QueryJCZQ_Match(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_Match>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public JCZQ_MatchResult QueryJCZQ_MatchResult(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCZQ_MatchResult>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public JCLQ_Match QueryJCLQ_Match(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_Match>().FirstOrDefault(p => p.MatchId == matchId);
        }
        public JCLQ_MatchResult QueryJCLQ_MatchResult(string matchId)
        {
            Session.Clear();
            return this.Session.Query<JCLQ_MatchResult>().FirstOrDefault(p => p.MatchId == matchId);
        }

        public TicketId_QueryInfoCollection QueryPrizeTicketList(string gameCode, int num)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_PrizeTicketList"))
               .AddInParameter("GameCode", gameCode)
               .AddInParameter("Num", num);
            var ds = query.GetDataSet();
            var result = new TicketId_QueryInfoCollection();
            result.TicketList = Common.Database.ORM.ORMHelper.DataTableToInfoList<Sports_TicketQueryInfo>(ds.Tables[0]);
            result.MatchList = Common.Database.ORM.ORMHelper.DataTableToInfoList<MatchInfo>(ds.Tables[1]);
            result.TotalTicketCount = result.TicketList.Count;
            result.TotalMatchCount = result.MatchList.Count;
            return result;
        }

        public TicketId_QueryInfoCollection QueryPrizeTicket_OrderIdList(string gameCode, string schemeId)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Prize_OrderTicketList"))
               .AddInParameter("GameCode", gameCode)
               .AddInParameter("SchemeId", schemeId);
            var ds = query.GetDataSet();
            var result = new TicketId_QueryInfoCollection();
            result.TicketList = Common.Database.ORM.ORMHelper.DataTableToInfoList<Sports_TicketQueryInfo>(ds.Tables[0]);
            result.MatchList = Common.Database.ORM.ORMHelper.DataTableToInfoList<MatchInfo>(ds.Tables[1]);
            result.TotalTicketCount = result.TicketList.Count;
            result.TotalMatchCount = result.MatchList.Count;
            return result;
        }

        public Sports_Order_Running QuerySports_Order_Running(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Running>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        /// <summary>
        /// 查询用户今日购买2串1的条数
        /// </summary>
        public int QueryUserByP2_1Count(string userId)
        {
            Session.Clear();
            return this.Session.Query<OrderDetail>().Where(f => f.UserId == userId && f.PlayType == "2_1"
                && f.TicketStatus == TicketStatus.Ticketed
                && (f.GameCode == "JCLQ" || f.GameCode == "JCZQ")
                && (f.CreateTime > DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59")) && f.CreateTime <= DateTime.Now)).Count();
        }

        public SingleScheme_AnteCode QuerySingleScheme_AnteCode(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<SingleScheme_AnteCode>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public List<SingleScheme_AnteCode> QuerySingleScheme_AnteCode(string gameCode, string gameType, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<SingleScheme_AnteCode>().Where(p => p.GameCode == gameCode && p.GameType == gameType && p.IssuseNumber == issuseNumber).ToList();
        }

        public List<string> QueryBJDCSingleCodeByIsuseNumber(string issuseNumber, string[] matchResultIdArray)
        {
            if (string.IsNullOrEmpty(issuseNumber) || matchResultIdArray.Length == 0)
                return new List<string>();
            Session.Clear();
            var sql = string.Format(@"SELECT [SchemeId]+'|'+[GameType]
                                    FROM [C_SingleScheme_AnteCode]
                                    where IssuseNumber='{0}' and GameCode='BJDC' 
                                          and  exists(select a from F_Common_SplitString(SelectMatchId,',') where a in (select a from F_Common_SplitString('{1}',',')))
                                    order by CreateTime desc", issuseNumber, string.Join(",", matchResultIdArray));
            var result = new List<string>();
            var list = this.Session.CreateSQLQuery(sql).List();
            foreach (var item in list)
            {
                if (item == null) continue;
                if (string.IsNullOrEmpty(item.ToString())) continue;
                result.Add(item.ToString());
            }
            return result;
        }
        public List<string> QueryJCZQSingleCodeByIsuseNumber(string[] matchResultIdArray)
        {
            if (matchResultIdArray.Length == 0)
                return new List<string>();
            Session.Clear();
            var sql = string.Format(@"SELECT [SchemeId]+'|'+[GameType]
                                    FROM [C_SingleScheme_AnteCode]
                                    where GameCode='JCZQ' 
                                          and  exists(select a from F_Common_SplitString(SelectMatchId,',') where a in (select a from F_Common_SplitString('{0}',',')))
                                    order by CreateTime desc", string.Join(",", matchResultIdArray));
            var list = this.Session.CreateSQLQuery(sql).List();
            var result = new List<string>();
            foreach (var item in list)
            {
                if (item == null) continue;
                if (string.IsNullOrEmpty(item.ToString())) continue;
                result.Add(item.ToString());
            }
            return result;
        }
        public List<string> QueryJCLQSingleCodeByIsuseNumber(string[] matchResultIdArray)
        {
            if (matchResultIdArray.Length == 0)
                return new List<string>();
            Session.Clear();
            var sql = string.Format(@"SELECT [SchemeId]+'|'+[GameType]
                                    FROM [C_SingleScheme_AnteCode]
                                    where GameCode='JCLQ' 
                                          and  exists(select a from F_Common_SplitString(SelectMatchId,',') where a in (select a from F_Common_SplitString('{0}',',')))
                                    order by CreateTime desc", string.Join(",", matchResultIdArray));
            var list = this.Session.CreateSQLQuery(sql).List();
            var result = new List<string>();
            foreach (var item in list)
            {
                if (item == null) continue;
                if (string.IsNullOrEmpty(item.ToString())) continue;
                result.Add(item.ToString());
            }
            return result;
        }

        public List<Sports_Order_Running> QueryWaitForPrizeRunningOrder(string gameCode, string gameType, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Running>().Where(p => p.GameCode == gameCode
                //&& (gameCode == "CTZQ" || p.BonusStatus == BonusStatus.Awarding)
                && (gameType == string.Empty || p.GameType == gameType)
                && (issuseNumber == string.Empty || p.IssuseNumber == issuseNumber)).ToList();
        }

        public List<Sports_Order_Running> QueryWaitForOpenRunningOrder(string gameCode, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Running>().Where(p => p.GameCode == gameCode
                && p.BonusStatus == BonusStatus.Waitting
                && p.IssuseNumber == issuseNumber).ToList();
        }

        public List<Sports_Order_Running> QueryOrderRunningBySchemeIdArray(string[] schemeIdArray)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Running>().Where(p => schemeIdArray.Contains(p.SchemeId)).ToList();
        }

        public List<Sports_Order_Running> QueryErrorChaseOrderRunning(string gameCode, string issuseNumber, int maxCount)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Running>().Where(p => p.GameCode == gameCode
                && p.IssuseNumber == issuseNumber
                && p.SchemeType == SchemeType.ChaseBetting
                && p.IsVirtualOrder == false)
                .OrderBy(p => p.CreateTime)
                .Take(maxCount).ToList();
        }

        public List<Sports_Order_Complate> QueryOrderComplateBySchemeIdArray(string[] schemeIdArray)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Complate>().Where(p => schemeIdArray.Contains(p.SchemeId)).ToList();
        }

        public string QueryWaitForTicketOrderId(string gameCode, string dateTime)
        {
            Session.Clear();
            var query = from s in Session.Query<Sports_Order_Running>()
                        where (s.TicketStatus == TicketStatus.Ticketing || s.TicketStatus == TicketStatus.PrintTicket) && (s.IsVirtualOrder == false) && (s.ProgressStatus == ProgressStatus.Running)
                        && (gameCode == string.Empty || s.GameCode == gameCode)
                        && s.QueryTicketStopTime == dateTime
                        orderby s.CreateTime ascending
                        select s.SchemeId;

            return string.Join(",", query.Take(50).ToArray());
        }

        public List<Sports_Order_Running> QueryWaitForPrizeLotteryOrder(string gameCode, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Running>().Where(p => p.GameCode == gameCode
                && p.IssuseNumber == issuseNumber).ToList();
        }

        public LotteryScheme QueryLotteryScheme(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<LotteryScheme>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public List<LotteryScheme> QueryLotterySchemeByKeyLine(string keyLine)
        {
            Session.Clear();
            return this.Session.Query<LotteryScheme>().Where(p => p.KeyLine == keyLine && p.IsComplate == false).OrderBy(p => p.OrderIndex).ToList();
        }

        public List<LotteryScheme> QueryAllLotterySchemeByKeyLine(string keyLine)
        {
            Session.Clear();
            return this.Session.Query<LotteryScheme>().Where(p => p.KeyLine == keyLine).OrderBy(p => p.OrderIndex).ToList();
        }

        public int QueryKeyLineCount(string keyLine)
        {
            Session.Clear();
            return this.Session.Query<LotteryScheme>().Where(p => p.KeyLine == keyLine).Count();
        }

        public List<Sports_Order_Running> QueryBrotherSports_Order_Running(string schemeId)
        {
            Session.Clear();
            var result = new List<Sports_Order_Running>();
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
            var array = this.Session.CreateSQLQuery(sql).List();
            if (array == null)
                return result;
            foreach (var item in array)
            {
                var row = item as object[];
                result.Add(new Sports_Order_Running
                {
                    SchemeId = UsefullHelper.GetDbValue<string>(row[0]),
                    UserId = UsefullHelper.GetDbValue<string>(row[1]),
                    GameCode = UsefullHelper.GetDbValue<string>(row[2]),
                    GameType = UsefullHelper.GetDbValue<string>(row[3]),
                    PlayType = UsefullHelper.GetDbValue<string>(row[4]),
                    SchemeType = UsefullHelper.GetDbValue<SchemeType>(row[5]),
                    SchemeSource = UsefullHelper.GetDbValue<SchemeSource>(row[6]),
                    SchemeBettingCategory = UsefullHelper.GetDbValue<SchemeBettingCategory>(row[7]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(row[8]),
                    Amount = UsefullHelper.GetDbValue<int>(row[9]),
                    BetCount = UsefullHelper.GetDbValue<int>(row[10]),
                    TotalMatchCount = UsefullHelper.GetDbValue<int>(row[11]),
                    TotalMoney = UsefullHelper.GetDbValue<decimal>(row[12]),
                    StopTime = UsefullHelper.GetDbValue<DateTime>(row[13]),
                    TicketStatus = UsefullHelper.GetDbValue<TicketStatus>(row[14]),
                    TicketId = UsefullHelper.GetDbValue<string>(row[15]),
                    TicketLog = UsefullHelper.GetDbValue<string>(row[16]),
                    ProgressStatus = UsefullHelper.GetDbValue<ProgressStatus>(row[17]),
                    BonusStatus = UsefullHelper.GetDbValue<BonusStatus>(row[18]),
                    HitMatchCount = UsefullHelper.GetDbValue<int>(row[19]),
                    PreTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(row[20]),
                    AfterTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(row[21]),
                    CanChase = UsefullHelper.GetDbValue<bool>(row[22]),
                    IsVirtualOrder = UsefullHelper.GetDbValue<bool>(row[23]),
                    CreateTime = UsefullHelper.GetDbValue<DateTime>(row[24]),
                    AgentId = UsefullHelper.GetDbValue<string>(row[25]),
                    Security = UsefullHelper.GetDbValue<TogetherSchemeSecurity>(row[26]),
                });
            }
            return result;
        }

        public Sports_Order_Complate QuerySports_Order_Complate(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Order_Complate>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public Sports_SchemeQueryInfo QuerySports_Order_RunningInfo(string schemeId)
        {
            Session.Clear();
            var query = from r in this.Session.Query<Sports_Order_Running>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where r.SchemeId == schemeId
                        select new Sports_SchemeQueryInfo
                        {
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            GameCode = r.GameCode,
                            Amount = r.Amount,
                            BonusStatus = r.BonusStatus,
                            CreateTime = r.CreateTime,
                            GameType = r.GameType,
                            IssuseNumber = r.IssuseNumber,
                            PlayType = r.PlayType,
                            ProgressStatus = r.ProgressStatus,
                            SchemeId = r.SchemeId,
                            SchemeType = r.SchemeType,
                            TicketId = r.TicketId,
                            TicketLog = r.TicketLog,
                            TicketStatus = r.TicketStatus,
                            TotalMatchCount = r.TotalMatchCount,
                            TotalMoney = r.TotalMoney,
                            BetCount = r.BetCount,
                            GameDisplayName = BusinessHelper.FormatGameCode(r.GameCode),
                            GameTypeDisplayName = BusinessHelper.FormatGameType(r.GameCode, r.GameType),
                            AfterTaxBonusMoney = 0M,
                            PreTaxBonusMoney = 0M,
                            BonusCount = 0,
                            WinNumber = string.Empty,
                            IsPrizeMoney = false,
                            Security = r.Security,
                            IsVirtualOrder = r.IsVirtualOrder,
                            StopTime = r.StopTime,
                            HitMatchCount = r.HitMatchCount,
                            AddMoney = 0M,
                            AddMoneyDescription = string.Empty,
                            SchemeBettingCategory = r.SchemeBettingCategory,
                            TicketProgress = r.TicketProgress,
                            DistributionWay = AddMoneyDistributionWay.Average,
                            Attach = r.Attach,
                            MaxBonusMoney = r.MaxBonusMoney,
                            MinBonusMoney = r.MinBonusMoney,
                            ExtensionOne = r.ExtensionOne,
                            IsAppend = r.IsAppend == null ? false : r.IsAppend,
                            BetTime = r.BetTime,
                            SchemeSource = r.SchemeSource,
                            TicketTime = r.TicketTime,
                            RedBagMoney = r.RedBagMoney,
                        };
            var info = query.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = this.Session.Query<GameIssuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }

        public Sports_SchemeQueryInfo QuerySports_Order_ComplateInfo(string schemeId)
        {
            Session.Clear();
            var query = from r in this.Session.Query<Sports_Order_Complate>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where r.SchemeId == schemeId
                        select new Sports_SchemeQueryInfo
                        {
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            GameDisplayName = BusinessHelper.FormatGameCode(r.GameCode),
                            GameCode = r.GameCode,
                            Amount = r.Amount,
                            BonusStatus = r.BonusStatus,
                            CreateTime = r.CreateTime,
                            GameType = r.GameType,
                            GameTypeDisplayName = BusinessHelper.FormatGameType(r.GameCode, r.GameType),
                            IssuseNumber = r.IssuseNumber,
                            PlayType = r.PlayType,
                            ProgressStatus = r.ProgressStatus,
                            SchemeId = r.SchemeId,
                            SchemeType = r.SchemeType,
                            TicketId = r.TicketId,
                            TicketLog = r.TicketLog,
                            TicketStatus = r.TicketStatus,
                            TotalMatchCount = r.TotalMatchCount,
                            TotalMoney = r.TotalMoney,
                            BetCount = r.BetCount,
                            PreTaxBonusMoney = r.PreTaxBonusMoney,
                            AfterTaxBonusMoney = r.AfterTaxBonusMoney,
                            BonusCount = r.BonusCount,
                            IsPrizeMoney = r.IsPrizeMoney,
                            Security = r.Security,
                            IsVirtualOrder = r.IsVirtualOrder,
                            StopTime = r.StopTime,
                            HitMatchCount = r.HitMatchCount,
                            AddMoney = r.AddMoney,
                            AddMoneyDescription = r.AddMoneyDescription,
                            SchemeBettingCategory = r.SchemeBettingCategory,
                            TicketProgress = r.TicketProgress,
                            DistributionWay = r.DistributionWay,
                            Attach = r.Attach,
                            MaxBonusMoney = r.MaxBonusMoney,
                            MinBonusMoney = r.MinBonusMoney,
                            ExtensionOne = r.ExtensionOne,
                            IsAppend = r.IsAppend == null ? false : r.IsAppend,
                            ComplateDateTime = r.ComplateDateTime,
                            BetTime = r.BetTime,
                            SchemeSource = r.SchemeSource,
                            RedBagMoney = r.RedBagMoney,
                            TicketTime = r.TicketTime,
                            RedBagAwardsMoney = r.AddMoneyDescription == "70" ? r.AddMoney : 0,
                            BonusAwardsMoney = r.AddMoneyDescription == "10" ? r.AddMoney : 0,
                        };
            var info = query.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = this.Session.Query<GameIssuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }

        public List<string> QuerySports_Order_Running_SchemeId_List(string gameCode, string issuseNumber, int returnCount)
        {
            Session.Clear();
            if (returnCount <= 0)
                returnCount = 500;
            return (from o in this.Session.Query<Sports_Order_Running>()
                    where o.TicketStatus == Core.TicketStatus.Waitting
                        && o.CanChase == true
                    //&& o.GameCode == gameCode
                    //&& (issuseNumber == string.Empty || o.IssuseNumber == issuseNumber)
                    orderby o.StopTime ascending
                    select o.SchemeId).Take(returnCount).ToList();
        }

        public Sports_AnteCode QueryOneSportsAnteCodeBySchemeId(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_AnteCode>().FirstOrDefault(p => p.SchemeId == schemeId);
        }
        public List<Sports_AnteCode> QuerySportsAnteCodeBySchemeId(string schemeId)
        {
            Session.Clear();
            var list = (from a in this.Session.Query<Sports_AnteCode>()
                        where a.SchemeId == schemeId
                        select a
                        ).ToList();
            if (list == null || list.Count <= 0)
                list = (from a in this.Session.Query<Sports_AnteCode_History>()
                        where a.SchemeId == schemeId
                        select new Sports_AnteCode
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
        public List<Sports_AnteCode> QueryCTZQSportsAnteCodeByIssuseNumber(string gameType, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<Sports_AnteCode>().Where(p => p.GameCode == "CTZQ" && p.GameType == gameType && p.IssuseNumber == issuseNumber).ToList();
        }
        public List<Sports_AnteCode> QueryBJDCSportsAnteCodeByIsuseNumber(string issuseNumber, string[] matchResultIdArray)
        {
            Session.Clear();
            return this.Session.Query<Sports_AnteCode>().Where(p => p.GameCode == "BJDC" && p.IssuseNumber == issuseNumber && matchResultIdArray.Contains(p.MatchId)).ToList();
        }
        public List<Sports_AnteCode> QueryJCZQSportsAnteCodeByIsuseNumber(string[] matchResultIdArray)
        {
            Session.Clear();
            return this.Session.Query<Sports_AnteCode>().Where(p => p.GameCode == "JCZQ" && matchResultIdArray.Contains(p.MatchId)).ToList();
        }
        public List<Sports_AnteCode> QueryJCLQSportsAnteCodeByIsuseNumber(string[] matchResultIdArray)
        {
            Session.Clear();
            return this.Session.Query<Sports_AnteCode>().Where(p => p.GameCode == "JCLQ" && matchResultIdArray.Contains(p.MatchId)).ToList();
        }

        public List<YouHuaScheme_AnteCode> QueryYouHuaScheme_AnteCode(string schemeId)
        {
            Session.Clear();
            return (from a in this.Session.Query<YouHuaScheme_AnteCode>() where a.SchemeId == schemeId select a).ToList();
        }

        public Sports_Together QuerySports_Together(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Together>().FirstOrDefault(p => p.SchemeId == schemeId);
        }
        public List<Sports_TogetherJoin> QuerySports_JoinTogetherList(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId).ToList();
        }

        public List<Sports_Together> QueryFinishTogether(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Together>()
                        where t.IsPayBackGuarantees == false
                        && t.ProgressStatus == TogetherSchemeProgress.Finish
                        select t;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<Sports_Ticket> QueryTicketList(string gameCode, string issuse)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Where(p => p.GameCode == gameCode && p.IssuseNumber == issuse
                        && p.BonusStatus == BonusStatus.Waitting
                        && p.TicketStatus == TicketStatus.Ticketed).ToList();
        }

        public bool IsUserJoinTogether(string schemeId, string userId)
        {
            Session.Clear();
            var count = this.Session.Query<Sports_TogetherJoin>().Count(p => p.SchemeId == schemeId && p.JoinUserId == userId && p.JoinSucess);
            return count > 0;
        }

        public int QueryTogetherJoinUserCount(string schemeId)
        {
            Session.Clear();
            var query = from j in this.Session.Query<Sports_TogetherJoin>()
                        where j.JoinSucess == true && j.SchemeId == schemeId
                        group j by j.JoinUserId into t
                        select new { JoinUserId = t.Key, Count = t.Count() };
            return query.ToList().Count;
        }

        public Sports_TogetherJoin QuerySports_TogetherJoin(string schemeId, int joinId)
        {
            Session.Clear();
            return this.Session.Query<Sports_TogetherJoin>().FirstOrDefault(p => p.SchemeId == schemeId && p.Id == joinId);
        }
        public Sports_TogetherJoin QuerySports_TogetherJoin(string schemeId, TogetherJoinType joinType)
        {
            Session.Clear();
            return this.Session.Query<Sports_TogetherJoin>().FirstOrDefault(p => p.SchemeId == schemeId && p.JoinType == joinType);
        }
        public List<Sports_TogetherJoin> QuerySports_TogetherSucessJoin(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_TogetherJoin>().Where(p => p.SchemeId == schemeId && p.JoinSucess == true).ToList();
        }

        public string[] QueryWaitToProcessingTogetherIdList(string gameCode, string stopTime)
        {
            Session.Clear();
            //var query = from t in this.Session.Query<Temp_Together>()
            //            where t.GameCode == gameCode && t.StopTime == stopTime
            //            select t.SchemeId;

            //var query = from t in this.Session.Query<Temp_Together>()
            //            join o in this.Session.Query<Sports_Order_Running>() on t.SchemeId equals o.SchemeId
            //            where t.GameCode == gameCode && t.StopTime == stopTime && o.SchemeBettingCategory != SchemeBettingCategory.XianFaQiHSC
            //            select t.SchemeId;


            //and r.SchemeBettingCategory<>4 

            string strSql = "select t.SchemeId from C_Temp_Together t where t.GameCode=:gameCode and t.StopTime<=:stopTime";
            var query = Session.CreateSQLQuery(strSql)
                        .SetString("gameCode", gameCode)
                        .SetString("stopTime", stopTime)
                        .List<string>();
            return query.ToArray();
        }
        public string[] QueryWaitToProcessingXianFaQiHSCOrderList(string gameCode, string stopTime)
        {
            Session.Clear();
            //var query = from t in this.Session.Query<Temp_Together>()
            //            where t.GameCode == gameCode && t.StopTime == stopTime
            //            select t.SchemeId;

            //var query = from t in this.Session.Query<Temp_Together>()
            //            join o in this.Session.Query<Sports_Order_Running>() on t.SchemeId equals o.SchemeId
            //            where t.GameCode == gameCode && t.StopTime == stopTime && o.SchemeBettingCategory == SchemeBettingCategory.XianFaQiHSC
            //            select t.SchemeId;

            string strSql = "select t.SchemeId from C_Temp_Together t inner join C_Sports_Order_Running r on r.SchemeId=t.SchemeId where t.GameCode=:gameCode and r.SchemeBettingCategory=4 and t.StopTime<=:stopTime";
            var query = Session.CreateSQLQuery(strSql)
                        .SetString("gameCode", gameCode)
                        .SetString("stopTime", stopTime)
                        .List<string>();
            return query.ToArray();
        }
        public Temp_Together QueryTemp_Together(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Temp_Together>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public List<UserBeedingListInfo> QueryUserBeedingList(string gameCode, string gameType, string userId, string userDisplayName, int pageIndex, int pageSize,
            QueryUserBeedingListOrderByProperty orderByProperty, OrderByCategory orderByCategory, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from b in this.Session.Query<UserBeedings>()
                        join u in this.Session.Query<UserRegister>() on b.UserId equals u.UserId
                        where b.TotalBonusMoney > 0M && b.TotalBonusTimes > 0
                        && (string.Empty == gameCode || b.GameCode == gameCode)
                        && (string.Empty == gameType || b.GameType == gameType)
                        && (string.Empty == userId || u.UserId == userId)
                        && (string.Empty == userDisplayName || u.DisplayName == userDisplayName)
                        select new UserBeedingListInfo
                        {
                            BeFollowedTotalMoney = b.BeFollowedTotalMoney,
                            BeFollowerUserCount = b.BeFollowerUserCount,
                            GameCode = b.GameCode,
                            GameType = b.GameType,
                            GoldCrownCount = b.GoldCrownCount,
                            GoldCupCount = b.GoldCupCount,
                            GoldDiamondsCount = b.GoldDiamondsCount,
                            GoldStarCount = b.GoldStarCount,
                            SilverCrownCount = b.SilverCrownCount,
                            SilverCupCount = b.SilverCupCount,
                            SilverDiamondsCount = b.SilverDiamondsCount,
                            SilverStarCount = b.SilverStarCount,
                            TotalBetMoney = b.TotalBetMoney,
                            TotalOrderCount = b.TotalOrderCount,
                            TotalBonusMoney = b.TotalBonusMoney,
                            TotalBonusTimes = b.TotalBonusTimes,
                            UserDisplayName = u.DisplayName,
                            UserHideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = b.UserId,
                        };

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


            //Dictionary<string, object> outputs;
            //var query = CreateOutputQuery(Session.GetNamedQuery("P_User_QueryUserBeedingList"))
            //    .AddInParameter("gameCode", gameCode).AddInParameter("gameType", gameType).AddInParameter("userDisplayName", userDisplayName)
            //    .AddInParameter("pageIndex", pageIndex).AddInParameter("pageSize", pageSize)
            //    .AddInParameter("orderByProperty", (int)orderByProperty).AddInParameter("orderByCategory", (int)orderByCategory)
            //    .AddOutParameter("totalCount", "Int32");

            //var list = query.List(out outputs);
            //totalCount = (int)outputs["totalCount"];
            //var result = new List<UserBeedingListInfo>();
            //foreach (var item in list)
            //{
            //    var array = item as object[];
            //    result.Add(new UserBeedingListInfo
            //    {
            //        GameCode = UsefullHelper.GetDbValue<string>(array[1]),
            //        GameType = UsefullHelper.GetDbValue<string>(array[2]),
            //        UserId = UsefullHelper.GetDbValue<string>(array[3]),
            //        UserDisplayName = UsefullHelper.GetDbValue<string>(array[4]),
            //        TogetherSchemeSuccessGainMoney = UsefullHelper.GetDbValue<decimal>(array[5]),
            //        TogetherSchemeSuccessAndBonusCount = UsefullHelper.GetDbValue<int>(array[6]),
            //        TogetherSchemeSuccessAndBonusMoney = UsefullHelper.GetDbValue<decimal>(array[7]),
            //        BeFollowerUserCount = UsefullHelper.GetDbValue<int>(array[8]),
            //        BeFollowedTotalMoney = UsefullHelper.GetDbValue<decimal>(array[9]),
            //        TotalOrderCount = UsefullHelper.GetDbValue<int>(array[10]),
            //        BonusOrderCount = UsefullHelper.GetDbValue<int>(array[11]),
            //        BonusPercent = UsefullHelper.GetDbValue<decimal>(array[12]),
            //        HideDisplayNameCount = UsefullHelper.GetDbValue<int>(array[13]),
            //    });
            //}

            //return result;
        }

        public List<UserBeedingListInfo> QueryBonusUserBeedingList(string userId)
        {
            Session.Clear();
            var query = from b in this.Session.Query<UserBeedings>()
                        join u in this.Session.Query<UserRegister>() on b.UserId equals u.UserId
                        where b.TotalBonusMoney > 0M && b.TotalBonusTimes > 0
                        && b.UserId == userId
                        orderby b.TotalBonusMoney descending
                        select new UserBeedingListInfo
                        {
                            BeFollowedTotalMoney = b.BeFollowedTotalMoney,
                            BeFollowerUserCount = b.BeFollowerUserCount,
                            GameCode = b.GameCode,
                            GameType = b.GameType,
                            GoldCrownCount = b.GoldCrownCount,
                            GoldCupCount = b.GoldCupCount,
                            GoldDiamondsCount = b.GoldDiamondsCount,
                            GoldStarCount = b.GoldStarCount,
                            SilverCrownCount = b.SilverCrownCount,
                            SilverCupCount = b.SilverCupCount,
                            SilverDiamondsCount = b.SilverDiamondsCount,
                            SilverStarCount = b.SilverStarCount,
                            TotalBetMoney = b.TotalBetMoney,
                            TotalOrderCount = b.TotalOrderCount,
                            TotalBonusMoney = b.TotalBonusMoney,
                            TotalBonusTimes = b.TotalBonusTimes,
                            UserDisplayName = u.DisplayName,
                            UserHideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = b.UserId,
                        };
            return query.ToList();
        }

        public UserBeedingListInfo QueryUserBeedingListInfo(string userId, string gameCode, string gameType)
        {
            this.Session.Clear();
            var query = from b in this.Session.Query<UserBeedings>()
                        join u in this.Session.Query<UserRegister>() on b.UserId equals u.UserId
                        where b.UserId == userId
                        && (string.Empty == gameCode || b.GameCode == gameCode)
                        && (string.Empty == gameType || b.GameType == gameType)
                        select new UserBeedingListInfo
                        {
                            BeFollowedTotalMoney = b.BeFollowedTotalMoney,
                            BeFollowerUserCount = b.BeFollowerUserCount,
                            GameCode = b.GameCode,
                            GameType = b.GameType,
                            GoldCrownCount = b.GoldCrownCount,
                            GoldCupCount = b.GoldCupCount,
                            GoldDiamondsCount = b.GoldDiamondsCount,
                            GoldStarCount = b.GoldStarCount,
                            SilverCrownCount = b.SilverCrownCount,
                            SilverCupCount = b.SilverCupCount,
                            SilverDiamondsCount = b.SilverDiamondsCount,
                            SilverStarCount = b.SilverStarCount,
                            TotalBetMoney = b.TotalBetMoney,
                            TotalOrderCount = b.TotalOrderCount,
                            TotalBonusMoney = b.TotalBonusMoney,
                            TotalBonusTimes = b.TotalBonusTimes,
                            UserDisplayName = u.DisplayName,
                            UserHideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = b.UserId,
                        };

            return query.FirstOrDefault();
        }

        public TogetherFollowerRule QueryTogetherFollowerRule(string createrUserId, string followerUserId, string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<TogetherFollowerRule>().FirstOrDefault(p => p.CreaterUserId == createrUserId && p.FollowerUserId == followerUserId && p.GameCode == gameCode && p.GameType == gameType);
        }
        public TogetherFollowerRuleQueryInfo QueryTogetherFollowerRuleInfo(string createrUserId, string followerUserId, string gameCode, string gameType)
        {
            Session.Clear();
            var query = from t in Session.Query<TogetherFollowerRule>()
                        join u in Session.Query<UserRegister>() on t.CreaterUserId equals u.UserId
                        where t.CreaterUserId == createrUserId && t.FollowerUserId == followerUserId && t.GameCode == gameCode && t.GameType == gameType
                        select new TogetherFollowerRuleQueryInfo
                        {
                            CancelNoBonusSchemeCount = t.CancelNoBonusSchemeCount,
                            CancelWhenSurplusNotMatch = t.CancelWhenSurplusNotMatch,
                            CreaterUserId = t.CreaterUserId,
                            FollowerCount = t.FollowerCount,
                            FollowerPercent = t.FollowerPercent,
                            FollowerUserId = t.FollowerUserId,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            IsEnable = t.IsEnable,
                            MaxSchemeMoney = t.MaxSchemeMoney,
                            MinSchemeMoney = t.MinSchemeMoney,
                            SchemeCount = t.SchemeCount,
                            StopFollowerMinBalance = t.StopFollowerMinBalance,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            RuleId = t.Id,
                        };
            if (query != null) return query.FirstOrDefault();
            return new TogetherFollowerRuleQueryInfo();
        }
        public TogetherFollowerRule QueryTogetherFollowerRule(long Id)
        {
            Session.Clear();
            return this.Session.Query<TogetherFollowerRule>().FirstOrDefault(p => p.Id == Id);
        }

        public int QueryTogetherFollowerRuleCount(string createUserId, string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<TogetherFollowerRule>().Count(p => p.CreaterUserId == createUserId && p.GameCode == gameCode && p.GameType == gameType);
        }

        public List<TogetherFollowerRule> QuerySportsTogetherFollowerList(string createrUserId, string gameCode, string gameType)
        {
            Session.Clear();
            return (from r in this.Session.Query<TogetherFollowerRule>() where r.CreaterUserId == createrUserId && r.GameCode == gameCode && r.GameType == gameType orderby r.FollowerIndex ascending select r).ToList();
        }

        public List<TogetherFollowerRule> QuerySportsTogetherFollowerListByFollowerUserId(string followerUserId, string gameCode, string gameType)
        {
            Session.Clear();
            return (from r in this.Session.Query<TogetherFollowerRule>() where r.FollowerUserId == followerUserId && r.GameCode == gameCode && r.GameType == gameType orderby r.FollowerIndex ascending select r).ToList();
        }

        public int QueryTogetherFollowerRecordCount(string key)
        {
            Session.Clear();
            return this.Session.Query<TogetherFollowerRecord>().Count(r => r.RecordKey == key);
        }

        public TogetherFollowerRecord QueryFollowerRecordBySchemeId(string schemeId, string followerUserId)
        {
            Session.Clear();
            return this.Session.Query<TogetherFollowerRecord>().FirstOrDefault(p => p.SchemeId == schemeId && p.FollowerUserId == followerUserId);
        }
        public List<TogetherFollowRecordInfo> QuerySucessFolloweRecord(string userId, long ruleId, string gameCode, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<TogetherFollowerRecord>()
                        join u in this.Session.Query<UserRegister>() on r.CreaterUserId equals u.UserId
                        join o in this.Session.Query<OrderDetail>() on r.SchemeId equals o.SchemeId
                        where r.FollowerUserId == userId && (ruleId < 1 || r.RuleId == ruleId)
                        && (gameCode == string.Empty || r.GameCode == gameCode)
                        orderby r.CreateTime descending
                        select new TogetherFollowRecordInfo
                        {
                            CreaterDisplayName = u.DisplayName,
                            CreaterUserId = u.UserId,
                            CreaterHideDisplayNameCount = u.HideDisplayNameCount,
                            CreateTime = r.CreateTime,
                            FollowBonusMoney = r.BonusMoney,
                            FollowMoney = r.BuyMoney,
                            GameCode = r.GameCode,
                            GameType = r.GameType,
                            GameCodeDisplayName = BusinessHelper.FormatGameCode(r.GameCode),
                            GameTypeDisplayName = BusinessHelper.FormatGameType(r.GameCode, r.GameType),
                            IssuseNumber = o.CurrentIssuseNumber,
                            ProgressStatus = o.ProgressStatus,
                            SchemeId = o.SchemeId,
                            SchemeMoney = o.TotalMoney,
                            SchemeBonusMoney = o.AfterTaxBonusMoney,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询被跟单人数
        /// </summary>
        public int QueryTogetherFollowerRecord(string userId, string gameCode, string gameType)
        {
            this.Session.Clear();
            return this.Session.Query<TogetherFollowerRule>().Where(p => p.CreaterUserId == userId && p.GameCode == gameCode && p.GameType == gameType).Count();
        }


        public List<TogetherFollowerRuleQueryInfo> QueryUserFollowRule(bool byFollower, string userId, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = byFollower ? (from f in this.Session.Query<TogetherFollowerRule>()
                                      join u in this.Session.Query<UserRegister>() on f.CreaterUserId equals u.UserId
                                      where (gameCode == string.Empty || f.GameCode == gameCode)
                                      && (gameType == string.Empty || f.GameType == gameType)
                                      && (userId == string.Empty || f.FollowerUserId == userId)
                                      select new TogetherFollowerRuleQueryInfo
                                      {
                                          RuleId = f.Id,
                                          BonusMoney = f.TotalBonusMoney,
                                          BuyMoney = f.TotalBetMoney,
                                          CancelNoBonusSchemeCount = f.CancelNoBonusSchemeCount,
                                          CancelWhenSurplusNotMatch = f.CancelWhenSurplusNotMatch,
                                          CreaterUserId = f.CreaterUserId,
                                          CreateTime = f.CreateTime,
                                          FollowerCount = f.FollowerCount,
                                          FollowerIndex = f.FollowerIndex,
                                          FollowerPercent = f.FollowerPercent,
                                          FollowerUserId = f.FollowerUserId,
                                          GameCode = f.GameCode,
                                          GameType = f.GameType,
                                          IsEnable = f.IsEnable,
                                          MaxSchemeMoney = f.MaxSchemeMoney,
                                          MinSchemeMoney = f.MinSchemeMoney,
                                          SchemeCount = f.SchemeCount,
                                          StopFollowerMinBalance = f.StopFollowerMinBalance,
                                          UserId = u.UserId,
                                          UserDisplayName = u.DisplayName,
                                          HideDisplayNameCount = u.HideDisplayNameCount,
                                      }) :
                                    (from f in this.Session.Query<TogetherFollowerRule>()
                                     join u in this.Session.Query<UserRegister>() on f.FollowerUserId equals u.UserId
                                     where (gameCode == string.Empty || f.GameCode == gameCode)
                                     && (gameType == string.Empty || f.GameType == gameType)
                                     && (userId == string.Empty || f.CreaterUserId == userId)
                                     orderby f.FollowerIndex ascending
                                     select new TogetherFollowerRuleQueryInfo
                                     {
                                         RuleId = f.Id,
                                         BonusMoney = f.TotalBonusMoney,
                                         BuyMoney = f.TotalBetMoney,
                                         CancelNoBonusSchemeCount = f.CancelNoBonusSchemeCount,
                                         CancelWhenSurplusNotMatch = f.CancelWhenSurplusNotMatch,
                                         CreaterUserId = f.CreaterUserId,
                                         CreateTime = f.CreateTime,
                                         FollowerCount = f.FollowerCount,
                                         FollowerIndex = f.FollowerIndex,
                                         FollowerPercent = f.FollowerPercent,
                                         FollowerUserId = f.FollowerUserId,
                                         GameCode = f.GameCode,
                                         GameType = f.GameType,
                                         IsEnable = f.IsEnable,
                                         MaxSchemeMoney = f.MaxSchemeMoney,
                                         MinSchemeMoney = f.MinSchemeMoney,
                                         SchemeCount = f.SchemeCount,
                                         StopFollowerMinBalance = f.StopFollowerMinBalance,
                                         UserId = u.UserId,
                                         UserDisplayName = u.DisplayName,
                                         HideDisplayNameCount = u.HideDisplayNameCount,
                                     });

            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            //Dictionary<string, object> outputs;
            //var query = byFollower ?
            //    (CreateOutputQuery(Session.GetNamedQuery("P_User_QueryUserFollowRule_ByFollower"))
            //    .AddInParameter("userId", userId).AddInParameter("gameCode", gameCode)
            //    .AddInParameter("pageIndex", pageIndex).AddInParameter("pageSize", pageSize)
            //    .AddOutParameter("totalCount", "Int32"))
            //    : (CreateOutputQuery(Session.GetNamedQuery("P_User_QueryUserFollowRule_ByCreater"))
            //    .AddInParameter("userId", userId).AddInParameter("gameCode", gameCode)
            //    .AddInParameter("pageIndex", pageIndex).AddInParameter("pageSize", pageSize)
            //    .AddOutParameter("totalCount", "Int32"));

            //var list = query.List(out outputs);
            //totalCount = (int)outputs["totalCount"];
            //var result = new List<TogetherFollowerRuleQueryInfo>();
            //foreach (var item in list)
            //{
            //    var array = item as object[];
            //    result.Add(new TogetherFollowerRuleQueryInfo
            //    {
            //        UserId = UsefullHelper.GetDbValue<string>(array[1]),
            //        UserDisplayName = UsefullHelper.GetDbValue<string>(array[2]),
            //        SuccessGainMoney = UsefullHelper.GetDbValue<decimal>(array[3]),
            //        BuyMoney = UsefullHelper.GetDbValue<decimal>(array[4]),
            //        BonusMoney = UsefullHelper.GetDbValue<decimal>(array[5]),
            //        FollowerId = UsefullHelper.GetDbValue<long>(array[6]),
            //        CreateTime = UsefullHelper.GetDbValue<DateTime>(array[7]),
            //        IsEnable = UsefullHelper.GetDbValue<bool>(array[8]),
            //        CreaterUserId = UsefullHelper.GetDbValue<string>(array[9]),
            //        FollowerUserId = UsefullHelper.GetDbValue<string>(array[10]),
            //        GameCode = UsefullHelper.GetDbValue<string>(array[11]),
            //        GameType = UsefullHelper.GetDbValue<string>(array[12]),
            //        SchemeCount = UsefullHelper.GetDbValue<int>(array[13]),
            //        MinSchemeMoney = UsefullHelper.GetDbValue<decimal>(array[14]),
            //        MaxSchemeMoney = UsefullHelper.GetDbValue<decimal>(array[15]),
            //        FollowerCount = UsefullHelper.GetDbValue<int>(array[16]),
            //        FollowerPercent = UsefullHelper.GetDbValue<decimal>(array[17]),
            //        CancelWhenSurplusNotMatch = UsefullHelper.GetDbValue<bool>(array[18]),
            //        CancelNoBonusSchemeCount = UsefullHelper.GetDbValue<int>(array[19]),
            //        StopFollowerMinBalance = UsefullHelper.GetDbValue<decimal>(array[20]),
            //        HideDisplayNameCount = UsefullHelper.GetDbValue<int>(array[21]),

            //        GameCodeDisplayName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(array[11])),
            //        GameTypeDisplayName = BusinessHelper.FormatGameType(UsefullHelper.GetDbValue<string>(array[11]), UsefullHelper.GetDbValue<string>(array[12])),
            //    });
            //}
            //return result;
        }
        public List<TogetherFollowMeInfo> QueryUserBeFollowedReport(string userId, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from f in this.Session.Query<TogetherFollowerRule>()
                        where f.CreaterUserId == userId
                        && (string.Empty == gameCode || f.GameCode == gameCode)
                        && (string.Empty == gameType || f.GameType == gameType)
                        select new TogetherFollowMeInfo
                        {
                            CreaterUserId = f.CreaterUserId,
                            GameCode = f.GameCode,
                            GameType = f.GameType,
                            TotalBetMoney = f.TotalBetMoney,
                            TotalBonusMoney = f.TotalBonusMoney,
                            TotalOrderBonusCount = f.TotalBonusOrderCount,
                            TotalOrderCount = f.TotalBetOrderCount,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            //Dictionary<string, object> outputs;
            //var query = CreateOutputQuery(Session.GetNamedQuery("P_User_QueryUserBeFollowedReport"))
            //    .AddInParameter("userId", userId)
            //    .AddInParameter("pageIndex", pageIndex).AddInParameter("pageSize", pageSize)
            //    .AddOutParameter("totalCount", "Int32");

            //var list = query.List(out outputs);
            //totalCount = (int)outputs["totalCount"];
            //var result = new List<TogetherFollowMeInfo>();
            //foreach (var item in list)
            //{
            //    var array = item as object[];
            //    result.Add(new TogetherFollowMeInfo
            //    {
            //        CreaterUserId = UsefullHelper.GetDbValue<string>(array[1]),
            //        GameCode = UsefullHelper.GetDbValue<string>(array[2]),
            //        GameType = UsefullHelper.GetDbValue<string>(array[3]),
            //        FollowUserCount = UsefullHelper.GetDbValue<int>(array[4]),
            //        SuccessCount = UsefullHelper.GetDbValue<int>(array[5]),
            //        BonusCount = UsefullHelper.GetDbValue<int>(array[6]),
            //        GameCodeDisplayName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(array[2])),
            //        GameTypeDisplayName = BusinessHelper.FormatGameType(UsefullHelper.GetDbValue<string>(array[2]), UsefullHelper.GetDbValue<string>(array[3])),
            //    });
            //}
            //return result;
        }

        /// <summary>
        /// 查询合买列表
        /// </summary>
        public List<Sports_TogetherSchemeQueryInfo> QuerySportsTogetherList(string key, string issuseNumber, string gameCode, string gameType,
            TogetherSchemeSecurity? security, SchemeBettingCategory? betCategory, TogetherSchemeProgress? progressState,
            decimal minMoney, decimal maxMoney, decimal minProgress, decimal maxProgress, string orderBy, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            //pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryTogetherList"))
                .AddInParameter("gameCode", gameCode).AddInParameter("gameType", gameType)
                .AddInParameter("minMoney", minMoney).AddInParameter("maxMoney", maxMoney)
                .AddInParameter("minProgress", minProgress).AddInParameter("maxProgress", maxProgress)
                .AddInParameter("security", !security.HasValue ? -1 : (int)security.Value)
                .AddInParameter("bettingCategory", !betCategory.HasValue ? -1 : (int)betCategory.Value)
                .AddInParameter("progressStatus", !progressState.HasValue ? "10|20|30" : ((int)progressState.Value).ToString())
                .AddInParameter("issuseNumber", issuseNumber)
                .AddInParameter("Key_UID_UName_SchemeId", key)
                .AddInParameter("pageIndex", pageIndex).AddInParameter("pageSize", pageSize)
                .AddInParameter("orderBy", orderBy)
                .AddOutParameter("totalCount", "Int32");

            var list = query.List(out outputs);
            totalCount = (int)outputs["totalCount"];
            var result = new List<Sports_TogetherSchemeQueryInfo>();
            foreach (var item in list)
            {
                var array = item as object[];
                Sports_TogetherSchemeQueryInfo info = new Sports_TogetherSchemeQueryInfo();
                info.IsTop = UsefullHelper.GetDbValue<bool>(array[1]);
                info.BonusDeduct = UsefullHelper.GetDbValue<int>(array[2]);
                info.CreateUserId = UsefullHelper.GetDbValue<string>(array[3]);
                info.CreaterDisplayName = UsefullHelper.GetDbValue<string>(array[4]);
                info.Description = UsefullHelper.GetDbValue<string>(array[5]);
                info.GameCode = UsefullHelper.GetDbValue<string>(array[6]);
                info.GameDisplayName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(array[6]));
                info.GameType = UsefullHelper.GetDbValue<string>(array[7]);
                if (array[38] == DBNull.Value)
                    info.GameTypeDisplayName = BusinessHelper.FormatGameType(UsefullHelper.GetDbValue<string>(array[6]), UsefullHelper.GetDbValue<string>(array[7]));
                else
                    info.GameTypeDisplayName = (SchemeBettingCategory)array[38] == SchemeBettingCategory.ErXuanYi ? "主客二选一" : BusinessHelper.FormatGameType(UsefullHelper.GetDbValue<string>(array[6]), UsefullHelper.GetDbValue<string>(array[7]));
                info.PlayType = UsefullHelper.GetDbValue<string>(array[8]);
                info.Guarantees = UsefullHelper.GetDbValue<int>(array[9]);
                info.Price = UsefullHelper.GetDbValue<decimal>(array[10]);
                info.SchemeDeduct = UsefullHelper.GetDbValue<decimal>(array[11]);
                info.SchemeSource = UsefullHelper.GetDbValue<SchemeSource>(array[12]);
                info.Security = UsefullHelper.GetDbValue<TogetherSchemeSecurity>(array[13]);
                info.StopTime = UsefullHelper.GetDbValue<DateTime>(array[14]);
                info.Subscription = UsefullHelper.GetDbValue<int>(array[15]);
                info.Title = UsefullHelper.GetDbValue<string>(array[16]);
                info.TotalCount = UsefullHelper.GetDbValue<int>(array[17]);
                info.TotalMoney = UsefullHelper.GetDbValue<decimal>(array[18]);
                info.SchemeId = UsefullHelper.GetDbValue<string>(array[19]);
                info.JoinPwd = UsefullHelper.GetDbValue<string>(array[20]);
                info.Progress = UsefullHelper.GetDbValue<decimal>(array[21]);
                info.ProgressStatus = UsefullHelper.GetDbValue<TogetherSchemeProgress>(array[22]);
                info.SystemGuarantees = UsefullHelper.GetDbValue<int>(array[23]);
                info.SoldCount = UsefullHelper.GetDbValue<int>(array[24]);
                info.TotalMatchCount = UsefullHelper.GetDbValue<int>(array[25]);
                info.SurplusCount = UsefullHelper.GetDbValue<int>(array[26]);
                info.CreaterHideDisplayNameCount = UsefullHelper.GetDbValue<int>(array[27]);
                info.GoldCrownCount = UsefullHelper.GetDbValue<int>(array[28]);
                info.GoldCupCount = UsefullHelper.GetDbValue<int>(array[29]);
                info.GoldDiamondsCount = UsefullHelper.GetDbValue<int>(array[30]);
                info.GoldStarCount = UsefullHelper.GetDbValue<int>(array[31]);
                info.SilverCrownCount = UsefullHelper.GetDbValue<int>(array[32]);
                info.SilverCupCount = UsefullHelper.GetDbValue<int>(array[33]);
                info.SilverDiamondsCount = UsefullHelper.GetDbValue<int>(array[34]);
                info.SilverStarCount = UsefullHelper.GetDbValue<int>(array[35]);
                info.JoinUserCount = UsefullHelper.GetDbValue<int>(array[36]);
                info.TicketStatus = UsefullHelper.GetDbValue<TicketStatus>(array[37]);
                info.SchemeBettingCategory = UsefullHelper.GetDbValue<SchemeBettingCategory>(array[38]);
                info.CreateTime = Convert.ToDateTime(array[39]);
                result.Add(info);



                //result.Add(new Sports_TogetherSchemeQueryInfo
                //{
                //    IsTop = UsefullHelper.GetDbValue<bool>(array[1]),
                //    BonusDeduct = UsefullHelper.GetDbValue<int>(array[2]),
                //    CreateUserId = UsefullHelper.GetDbValue<string>(array[3]),
                //    CreaterDisplayName = UsefullHelper.GetDbValue<string>(array[4]),
                //    Description = UsefullHelper.GetDbValue<string>(array[5]),
                //    GameCode = UsefullHelper.GetDbValue<string>(array[6]),
                //    GameDisplayName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(array[6])),
                //    GameType = UsefullHelper.GetDbValue<string>(array[7]),
                //    GameTypeDisplayName = (SchemeBettingCategory)array[38] == SchemeBettingCategory.ErXuanYi ? "主客二选一" : BusinessHelper.FormatGameType(UsefullHelper.GetDbValue<string>(array[6]), UsefullHelper.GetDbValue<string>(array[7])),
                //    PlayType = UsefullHelper.GetDbValue<string>(array[8]),
                //    Guarantees = UsefullHelper.GetDbValue<int>(array[9]),
                //    Price = UsefullHelper.GetDbValue<decimal>(array[10]),
                //    SchemeDeduct = UsefullHelper.GetDbValue<decimal>(array[11]),
                //    SchemeSource = UsefullHelper.GetDbValue<SchemeSource>(array[12]),
                //    Security = UsefullHelper.GetDbValue<TogetherSchemeSecurity>(array[13]),
                //    StopTime = UsefullHelper.GetDbValue<DateTime>(array[14]),
                //    Subscription = UsefullHelper.GetDbValue<int>(array[15]),
                //    Title = UsefullHelper.GetDbValue<string>(array[16]),
                //    TotalCount = UsefullHelper.GetDbValue<int>(array[17]),
                //    TotalMoney = UsefullHelper.GetDbValue<decimal>(array[18]),
                //    SchemeId = UsefullHelper.GetDbValue<string>(array[19]),
                //    JoinPwd = UsefullHelper.GetDbValue<string>(array[20]),
                //    Progress = UsefullHelper.GetDbValue<decimal>(array[21]),
                //    ProgressStatus = UsefullHelper.GetDbValue<TogetherSchemeProgress>(array[22]),
                //    SystemGuarantees = UsefullHelper.GetDbValue<int>(array[23]),
                //    SoldCount = UsefullHelper.GetDbValue<int>(array[24]),
                //    TotalMatchCount = UsefullHelper.GetDbValue<int>(array[25]),
                //    SurplusCount = UsefullHelper.GetDbValue<int>(array[26]),
                //    CreaterHideDisplayNameCount = UsefullHelper.GetDbValue<int>(array[27]),
                //    GoldCrownCount = UsefullHelper.GetDbValue<int>(array[28]),
                //    GoldCupCount = UsefullHelper.GetDbValue<int>(array[29]),
                //    GoldDiamondsCount = UsefullHelper.GetDbValue<int>(array[30]),
                //    GoldStarCount = UsefullHelper.GetDbValue<int>(array[31]),
                //    SilverCrownCount = UsefullHelper.GetDbValue<int>(array[32]),
                //    SilverCupCount = UsefullHelper.GetDbValue<int>(array[33]),
                //    SilverDiamondsCount = UsefullHelper.GetDbValue<int>(array[34]),
                //    SilverStarCount = UsefullHelper.GetDbValue<int>(array[35]),
                //    JoinUserCount = UsefullHelper.GetDbValue<int>(array[36]),
                //    TicketStatus = UsefullHelper.GetDbValue<TicketStatus>(array[37]),
                //    SchemeBettingCategory = UsefullHelper.GetDbValue<SchemeBettingCategory>(array[38])
                //});
            }
            return result;
        }

        public Sports_TogetherSchemeQueryInfo QueryRunningSportsTogetherDetail(string schemeId)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Together>()
                        join u in this.Session.Query<UserRegister>() on t.CreateUserId equals u.UserId
                        join r in this.Session.Query<Sports_Order_Running>() on t.SchemeId equals r.SchemeId
                        join b in this.Session.Query<UserBeedings>() on t.CreateUserId equals b.UserId
                        where t.SchemeId == schemeId && t.GameCode == b.GameCode && t.GameType == b.GameType
                        select new Sports_TogetherSchemeQueryInfo
                        {
                            BonusDeduct = t.BonusDeduct,
                            CreateUserId = t.CreateUserId,
                            CreaterDisplayName = u.DisplayName,
                            CreaterHideDisplayNameCount = u.HideDisplayNameCount,
                            Description = t.Description,
                            GameDisplayName = BusinessHelper.FormatGameCode(t.GameCode),
                            GameTypeDisplayName = BusinessHelper.FormatGameType(t.GameCode, t.GameType),
                            Guarantees = t.Guarantees,
                            PlayType = t.PlayType,
                            Price = t.Price,
                            SchemeDeduct = t.SchemeDeduct,
                            SchemeSource = t.SchemeSource,
                            Security = t.Security,
                            StopTime = t.StopTime,
                            Subscription = t.Subscription,
                            Title = t.Title,
                            TotalCount = t.TotalCount,
                            TotalMoney = t.TotalMoney,
                            SchemeId = t.SchemeId,
                            JoinPwd = t.JoinPwd,
                            Progress = t.Progress,
                            ProgressStatus = t.ProgressStatus,
                            SystemGuarantees = t.SystemGuarantees,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            SoldCount = t.SoldCount,
                            TotalMatchCount = t.TotalMatchCount,
                            Amount = r.Amount,
                            BetCount = r.BetCount,
                            PreTaxBonusMoney = r.PreTaxBonusMoney,
                            AfterTaxBonusMoney = r.AfterTaxBonusMoney,
                            WinNumber = string.Empty,
                            BonusStatus = r.BonusStatus,
                            BonusCount = 0,
                            CreateTime = t.CreateTime,
                            IsPrizeMoney = false,
                            TicketStatus = r.TicketStatus,
                            IssuseNumber = r.IssuseNumber,
                            AddMoney = 0M,
                            AddMoneyDescription = string.Empty,
                            IsVirtualOrder = r.IsVirtualOrder,
                            HitMatchCount = r.HitMatchCount,
                            SchemeBettingCategory = r.SchemeBettingCategory,
                            JoinUserCount = t.JoinUserCount,
                            Attach = r.Attach,
                            MinBonusMoney = r.MinBonusMoney,
                            MaxBonusMoney = r.MaxBonusMoney,
                            ExtensionOne = r.ExtensionOne,
                            GoldCrownCount = b.GoldCrownCount,
                            GoldCupCount = b.GoldCupCount,
                            GoldDiamondsCount = b.GoldDiamondsCount,
                            GoldStarCount = b.GoldStarCount,
                            SilverCrownCount = b.SilverCrownCount,
                            SilverCupCount = b.SilverCupCount,
                            SilverDiamondsCount = b.SilverDiamondsCount,
                            SilverStarCount = b.SilverStarCount,
                            IsAppend = r.IsAppend == null ? false : r.IsAppend,
                            TicketTime = r.TicketTime,

                        };
            var info = query.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = this.Session.Query<GameIssuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }

        public Sports_TogetherSchemeQueryInfo QueryComplateSportsTogetherDetail(string schemeId)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Together>()
                        join u in this.Session.Query<UserRegister>() on t.CreateUserId equals u.UserId
                        join r in this.Session.Query<Sports_Order_Complate>() on t.SchemeId equals r.SchemeId
                        join b in this.Session.Query<UserBeedings>() on t.CreateUserId equals b.UserId
                        where t.SchemeId == schemeId && t.GameCode == b.GameCode && t.GameType == b.GameType
                        select new Sports_TogetherSchemeQueryInfo
                        {
                            BonusDeduct = t.BonusDeduct,
                            CreateUserId = t.CreateUserId,
                            CreaterDisplayName = u.DisplayName,
                            CreaterHideDisplayNameCount = u.HideDisplayNameCount,
                            Description = t.Description,
                            GameDisplayName = BusinessHelper.FormatGameCode(t.GameCode),
                            GameTypeDisplayName = BusinessHelper.FormatGameType(t.GameCode, t.GameType),
                            Guarantees = t.Guarantees,
                            PlayType = t.PlayType,
                            Price = t.Price,
                            SchemeDeduct = t.SchemeDeduct,
                            SchemeSource = t.SchemeSource,
                            Security = t.Security,
                            StopTime = t.StopTime,
                            Subscription = t.Subscription,
                            Title = t.Title,
                            TotalCount = t.TotalCount,
                            TotalMoney = t.TotalMoney,
                            SchemeId = t.SchemeId,
                            JoinPwd = t.JoinPwd,
                            Progress = t.Progress,
                            ProgressStatus = t.ProgressStatus,
                            SystemGuarantees = t.SystemGuarantees,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            SoldCount = t.SoldCount,
                            TotalMatchCount = t.TotalMatchCount,
                            Amount = r.Amount,
                            BetCount = r.BetCount,
                            PreTaxBonusMoney = r.PreTaxBonusMoney,
                            AfterTaxBonusMoney = r.AfterTaxBonusMoney,
                            BonusStatus = r.BonusStatus,
                            BonusCount = r.BonusCount,
                            CreateTime = t.CreateTime,
                            IsPrizeMoney = r.IsPrizeMoney,
                            TicketStatus = r.TicketStatus,
                            IssuseNumber = r.IssuseNumber,
                            AddMoney = r.AddMoney,
                            AddMoneyDescription = r.AddMoneyDescription,
                            IsVirtualOrder = r.IsVirtualOrder,
                            HitMatchCount = r.HitMatchCount,
                            SchemeBettingCategory = r.SchemeBettingCategory,
                            JoinUserCount = t.JoinUserCount,
                            Attach = r.Attach,
                            MinBonusMoney = r.MinBonusMoney,
                            MaxBonusMoney = r.MaxBonusMoney,
                            ExtensionOne = r.ExtensionOne,
                            GoldCrownCount = b.GoldCrownCount,
                            GoldCupCount = b.GoldCupCount,
                            GoldDiamondsCount = b.GoldDiamondsCount,
                            GoldStarCount = b.GoldStarCount,
                            SilverCrownCount = b.SilverCrownCount,
                            SilverCupCount = b.SilverCupCount,
                            SilverDiamondsCount = b.SilverDiamondsCount,
                            SilverStarCount = b.SilverStarCount,
                            IsAppend = r.IsAppend == null ? false : r.IsAppend,
                            TicketTime = r.TicketTime,
                        };
            var info = query.FirstOrDefault();
            if (info != null && info.GameCode != "JCZQ" && info.GameCode != "JCLQ" && info.GameCode != "BJDC")
            {
                var key = info.GameCode == "CTZQ" ? string.Format("{0}|{1}|{2}", info.GameCode, info.GameType, info.IssuseNumber) : string.Format("{0}|{1}", info.GameCode, info.IssuseNumber);
                var gameIssuse = this.Session.Query<GameIssuse>().FirstOrDefault(g => g.GameCode_IssuseNumber == key);
                if (gameIssuse != null)
                    info.WinNumber = gameIssuse.WinNumber;
            }
            return info;
        }

        public List<Sports_TogetherJoinInfo> QuerySportsTogetherJoinList(string schemeId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            //pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from j in this.Session.Query<Sports_TogetherJoin>()
                        join u in this.Session.Query<UserRegister>() on j.JoinUserId equals u.UserId
                        where j.SchemeId == schemeId && j.JoinSucess == true
                        orderby j.JoinType ascending
                        select new Sports_TogetherJoinInfo
                        {
                            BuyCount = j.BuyCount,
                            RealBuyCount = j.RealBuyCount,
                            IsSucess = j.JoinSucess,
                            JoinDateTime = j.CreateTime,
                            JoinType = j.JoinType,
                            Price = j.Price,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = u.UserId,
                            JoinId = j.Id,
                            SchemeId = j.SchemeId,
                            BonusMoney = j.PreTaxBonusMoney,
                        };
            totalCount = query.Count();
            if (pageIndex == -1 && pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<Sports_TogetherJoinInfo> QueryUserSportsTogetherJoinList(string schemeId, string userId)
        {
            Session.Clear();
            var query = from j in this.Session.Query<Sports_TogetherJoin>()
                        join u in this.Session.Query<UserRegister>() on j.JoinUserId equals u.UserId
                        where j.SchemeId == schemeId && j.JoinUserId == userId
                        orderby j.JoinType ascending
                        select new Sports_TogetherJoinInfo
                        {
                            BuyCount = j.BuyCount,
                            RealBuyCount = j.RealBuyCount,
                            IsSucess = j.JoinSucess,
                            JoinDateTime = j.CreateTime,
                            JoinType = j.JoinType,
                            Price = j.Price,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = u.UserId,
                            JoinId = j.Id,
                            SchemeId = j.SchemeId,
                            BonusMoney = j.PreTaxBonusMoney,
                        };
            return query.ToList();
        }

        public List<Sports_TogetherJoinInfo> QueryNewBonusTogetherJoiner(int count)
        {
            Session.Clear();
            var query = from j in this.Session.Query<Sports_TogetherJoin>()
                        join u in this.Session.Query<UserRegister>() on j.JoinUserId equals u.UserId
                        where (j.JoinType == TogetherJoinType.FollowerJoin || j.JoinType == TogetherJoinType.Join)
                        && j.JoinSucess == true
                        && j.PreTaxBonusMoney > 0M
                        orderby j.CreateTime descending
                        select new Sports_TogetherJoinInfo
                        {
                            BuyCount = j.BuyCount,
                            RealBuyCount = j.RealBuyCount,
                            IsSucess = j.JoinSucess,
                            JoinDateTime = j.CreateTime,
                            JoinType = j.JoinType,
                            Price = j.Price,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = u.UserId,
                            JoinId = j.Id,
                            SchemeId = j.SchemeId,
                            BonusMoney = j.PreTaxBonusMoney,
                        };
            return query.Take(count).ToList();
        }

        public UserBeedings QueryUserBeedings(string userId, string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<UserBeedings>().FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && (string.Empty == gameType || p.GameType == gameType));
        }

        public List<Sports_Order_Complate> QuerySports_Order_ComplateByComplateDate(string complateDate)
        {
            Session.Clear();
            var query = from c in this.Session.Query<Sports_Order_Complate>()
                        where c.ComplateDate == complateDate
                        orderby c.ComplateDateTime ascending
                        select c;
            return query.ToList();
        }

        public List<UserBeedings> QueryUserBeedingsList(string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<UserBeedings>().Where(p => p.GameCode == gameCode && (gameType == string.Empty || p.GameType == gameType)).ToList();
        }

        public List<Sports_Order_Complate> QuerySports_Order_ComplateByComplateTime(string userId, string gameCode, string gameType, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            var query = from c in this.Session.Query<Sports_Order_Complate>()
                        where c.ComplateDateTime >= startTime && c.ComplateDateTime < endTime
                        && c.IsVirtualOrder == false
                        && c.UserId == userId && (gameCode == string.Empty || c.GameCode == gameCode) && (gameType == string.Empty || c.GameType == gameType)
                        orderby c.ComplateDateTime descending
                        select c;
            return query.ToList();
        }

        public List<Sports_Order_Complate> QueryWinSports_Order_ComplateByComplateTime(string userId, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            var query = from c in this.Session.Query<Sports_Order_Complate>()
                        where c.ComplateDateTime >= startTime && c.ComplateDateTime < endTime
                        && c.IsVirtualOrder == false
                        && c.UserId == userId
                        && c.BonusStatus == BonusStatus.Win
                        orderby c.ComplateDateTime descending
                        select c;
            return query.ToList();
        }

        public List<Sports_Order_Complate> QuerySports_Order_ComplateByComplateTime(DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            var query = from c in this.Session.Query<Sports_Order_Complate>()
                        where c.ComplateDateTime >= startTime && c.ComplateDateTime < endTime
                        && c.IsVirtualOrder == false
                        orderby c.ComplateDateTime descending
                        select c;
            return query.ToList();
        }

        public void ExecSql(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;
            Session.Clear();
            this.Session.CreateSQLQuery(sql).ExecuteUpdate();
        }

        public void ExecSql2(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;
            Session.Clear();
            this.Session.CreateQuery(sql).ExecuteUpdate();
        }

        public List<Sports_SchemeQueryInfo> QueryWaitForPrizeMoneyOrderList(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<Sports_Order_Complate>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where (r.ComplateDateTime >= startTime && r.ComplateDateTime < endTime)
                        && (gameCode == string.Empty || r.GameCode == gameCode)
                        && (r.AfterTaxBonusMoney + r.AddMoney) > 0M
                            //&& r.BonusStatus == BonusStatus.Win
                        && r.IsPrizeMoney == false
                        && r.IsVirtualOrder == false
                        select new Sports_SchemeQueryInfo
                        {
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            GameDisplayName = BusinessHelper.FormatGameCode(r.GameCode),
                            GameCode = r.GameCode,
                            Amount = r.Amount,
                            BonusStatus = r.BonusStatus,
                            CreateTime = r.CreateTime,
                            GameType = r.GameType,
                            GameTypeDisplayName = r.SchemeBettingCategory == SchemeBettingCategory.ErXuanYi ? "主客二选一" : BusinessHelper.FormatGameType(r.GameCode, r.GameType),
                            IssuseNumber = r.IssuseNumber,
                            PlayType = r.PlayType,
                            Security = r.Security,
                            IsVirtualOrder = r.IsVirtualOrder,
                            ProgressStatus = r.ProgressStatus,
                            SchemeId = r.SchemeId,
                            SchemeType = r.SchemeType,
                            TicketId = r.TicketId,
                            TicketLog = r.TicketLog,
                            TicketStatus = r.TicketStatus,
                            TotalMatchCount = r.TotalMatchCount,
                            TotalMoney = r.TotalMoney,
                            BetCount = r.BetCount,
                            PreTaxBonusMoney = r.PreTaxBonusMoney,
                            AfterTaxBonusMoney = r.AfterTaxBonusMoney,
                            BonusCount = r.BonusCount,
                            IsPrizeMoney = r.IsPrizeMoney,
                            HitMatchCount = r.HitMatchCount,
                            StopTime = r.StopTime,
                            AddMoney = r.AddMoney,
                            AddMoneyDescription = r.AddMoneyDescription,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public UserAttention QueryUserAttention(string currentUserId, string beAttentionUserId)
        {
            Session.Clear();
            return this.Session.Query<UserAttention>().FirstOrDefault(p => p.BeAttentionUserId == beAttentionUserId
                && p.FollowerUserId == currentUserId);
        }
        public UserAttention_Collection QueryMyAttentionListByUserId(string userId, int pageIndex, int pageSize)
        {
            Session.Clear();
            UserAttention_Collection collection = new UserAttention_Collection();
            collection.TotalCount = 0;
            var query = from a in Session.Query<UserAttention>()
                        where a.FollowerUserId == userId
                        select new UserAttentionInfo
                        {
                            BeAttentionUserId = a.BeAttentionUserId,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                if (pageSize == -1)
                    collection.AttentionList = query.ToList();
                else
                    collection.AttentionList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }

        public UserAttentionSummary QueryUserAttentionSummary(string currentUserId)
        {
            Session.Clear();
            return this.Session.Query<UserAttentionSummary>().FirstOrDefault(p => p.UserId == currentUserId);
        }

        public ProfileUserInfo QueryProfileUserInfo(string userId)
        {
            Session.Clear();
            var query = from r in this.Session.Query<UserRegister>()
                        join u in this.Session.Query<UserAttentionSummary>() on r.UserId equals u.UserId
                        where r.UserId == userId
                        select new ProfileUserInfo
                        {
                            UserId = r.UserId,
                            AttentionCount = u.FollowerUserCount,
                            AttentionedCount = u.BeAttentionUserCount,
                            HideNameCount = r.HideDisplayNameCount,
                            UserDisplayName = r.DisplayName,
                            CreateTime = r.CreateTime,
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 查询被关注的用户列表
        /// </summary>
        public List<ProfileAttentionInfo> QueryProfileAttentionInfoList(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from s in this.Session.Query<UserAttention>()
                        join u in this.Session.Query<UserRegister>() on s.FollowerUserId equals u.UserId
                        join b in this.Session.Query<Blog_ProfileBonusLevel>() on s.FollowerUserId equals b.UserId
                        where s.BeAttentionUserId == userId
                        orderby s.CreateTime descending
                        select new ProfileAttentionInfo
                        {
                            UserId = s.FollowerUserId,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            MaxLevelName = b.MaxLevelName,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        public List<UserAttentionSummaryInfo> QueryUserAttentionSummaryRank(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from s in this.Session.Query<UserAttentionSummary>()
                        join u in this.Session.Query<UserRegister>() on s.UserId equals u.UserId
                        orderby s.BeAttentionUserCount descending
                        select new UserAttentionSummaryInfo
                        {
                            BeAttentionUserCount = s.BeAttentionUserCount,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<UserAttentionSummaryInfo> QueryUserAttentionList(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from a in this.Session.Query<UserAttention>()
                        join u in this.Session.Query<UserRegister>() on a.BeAttentionUserId equals u.UserId
                        join b in this.Session.Query<Blog_ProfileBonusLevel>() on a.BeAttentionUserId equals b.UserId
                        where a.FollowerUserId == userId
                        select new UserAttentionSummaryInfo
                        {
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            MaxLevelName = b.MaxLevelName,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<UserAttentionSummaryInfo> QueryAttentionUserList(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from a in this.Session.Query<UserAttention>()
                        join u in this.Session.Query<UserRegister>() on a.FollowerUserId equals u.UserId
                        join b in this.Session.Query<Blog_ProfileBonusLevel>() on a.FollowerUserId equals b.UserId
                        where a.BeAttentionUserId == userId
                        select new UserAttentionSummaryInfo
                        {
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            MaxLevelName = b.MaxLevelName,
                        };
            totalCount = query.Count();
            if (pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public int QueryUserBeAttentionCount(string beAttentionUserId)
        {
            Session.Clear();
            return this.Session.Query<UserAttention>().Count(p => p.BeAttentionUserId == beAttentionUserId);
        }
        public string[] QueryBonusPercentUserIdArray()
        {
            Session.Clear();
            var query = from p in this.Session.Query<UserBonusPercent>()
                        group p by p.UserId into t
                        select t.Key;
            return query.ToArray();
        }
        public UserBonusPercent QueryUserBonusPercent(string userId, string gameCode, string gameType)
        {
            Session.Clear();
            return this.Session.Query<UserBonusPercent>().FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && (gameType == string.Empty || p.GameType == gameType));
        }
        public List<UserCurrentOrderInfo> QueryUserCurrentOrderList(string userId, string gameCode, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var list = new List<UserCurrentOrderInfo>();
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryUserCurrentOrderList"))
                .AddInParameter("UserId", userId)
                .AddInParameter("GameCode", gameCode)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            totalCount = (int)outputs["TotalCount"];
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new UserCurrentOrderInfo
                {
                    UserId = UsefullHelper.GetDbValue<string>(row[0]),
                    UserDisplayName = UsefullHelper.GetDbValue<string>(row[1]),
                    HideDisplayNameCount = UsefullHelper.GetDbValue<int>(row[2]),
                    SchemeId = UsefullHelper.GetDbValue<string>(row[3]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(row[4]),
                    GameCode = UsefullHelper.GetDbValue<string>(row[5]),
                    GameCodeName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(row[5])),
                    GameTypeName = UsefullHelper.GetDbValue<string>(row[6]),
                    SchemeType = UsefullHelper.GetDbValue<SchemeType>(row[7]),
                    TotalMoney = UsefullHelper.GetDbValue<decimal>(row[8]),
                    Progress = row[9] == DBNull.Value ? 1M : UsefullHelper.GetDbValue<decimal>(row[9]),
                    JoinType = TogetherJoinType.Join,
                });
            }
            return list;
        }
        public List<UserCurrentOrderInfo> QueryUserCurrentTogetherOrderList(string userId, string gameCode, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var list = new List<UserCurrentOrderInfo>();
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryUserCurrentTogetherOrderList"))
                .AddInParameter("UserId", userId)
                .AddInParameter("GameCode", gameCode)
                //.AddInParameter("Category", isCreateByUser ? 0 : 1)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            totalCount = (int)outputs["TotalCount"];

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new UserCurrentOrderInfo
                {
                    UserId = UsefullHelper.GetDbValue<string>(row[0]),
                    UserDisplayName = UsefullHelper.GetDbValue<string>(row[1]),
                    HideDisplayNameCount = UsefullHelper.GetDbValue<int>(row[2]),
                    SchemeId = UsefullHelper.GetDbValue<string>(row[3]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(row[4]),
                    GameCodeName = BusinessHelper.FormatGameCode(UsefullHelper.GetDbValue<string>(row[5])),
                    GameTypeName = UsefullHelper.GetDbValue<string>(row[6]),
                    SchemeType = UsefullHelper.GetDbValue<SchemeType>(row[7]),
                    JoinType = UsefullHelper.GetDbValue<TogetherJoinType>(row[8]),
                    TotalMoney = UsefullHelper.GetDbValue<decimal>(row[9]),
                    Progress = row[9] == DBNull.Value ? 1M : UsefullHelper.GetDbValue<decimal>(row[10])
                });
            }
            return list;
        }

        /// <summary>
        /// 查询用户保存的订单
        /// </summary>
        public List<SaveOrder_LotteryBettingInfo> QuerySaveOrderLottery(string userId)
        {
            Session.Clear();
            var query = from g in this.Session.Query<UserSaveOrder>()
                        join i in this.Session.Query<UserRegister>() on g.UserId equals i.UserId
                        orderby g.CreateTime descending
                        where g.UserId == userId
                        && (g.CreateTime > DateTime.Today.AddDays(-30) && g.CreateTime <= DateTime.Today.AddDays(1))
                        select new SaveOrder_LotteryBettingInfo
                        {
                            SchemeId = g.SchemeId,
                            DisplayName = i.DisplayName,
                            UserId = g.UserId,
                            GameCode = g.GameCode,
                            GameType = g.GameType,
                            StrStopTime = g.StrStopTime,
                            PlayType = g.PlayType,
                            SchemeType = g.SchemeType,
                            SchemeSource = g.SchemeSource,
                            SchemeBettingCategory = g.SchemeBettingCategory,
                            ProgressStatus = g.ProgressStatus,
                            IssuseNumber = g.IssuseNumber,
                            Amount = g.Amount,
                            BetCount = g.BetCount,
                            TotalMoney = g.TotalMoney,
                            StopTime = g.StopTime,
                            CreateTime = g.CreateTime,
                        };
            return query.ToList();
        }

        public List<UserSaveOrder> QueryUnBetOrder(string time)
        {
            Session.Clear();
            return this.Session.Query<UserSaveOrder>().Where(p => p.StrStopTime == time).ToList();
        }

        public string QueryUnBetOrderByTime(string time)
        {
            Session.Clear();
            var sql = string.Format(@"select schemeid from [C_UserSaveOrder] where [ProgressStatus]=0 and strStoptime <='{0}'", time);
            var array = this.Session.CreateSQLQuery(sql).List();
            var schemeIdList = new List<string>();
            foreach (var schemeId in array)
            {
                schemeIdList.Add(schemeId.ToString());
            }
            return string.Join("|", schemeIdList);
        }

        public List<string> QueryUnSplitTicketsOrder(List<string> unTicketGameCode, int count)
        {
            Session.Clear();
            var unTicketSql = unTicketGameCode.Count <= 0 ? "" : string.Format("and o.gamecode not in ({0}) ", string.Join(",", unTicketGameCode.Select(p => string.Format("'{0}'", p)).ToArray()));
            var sql = string.Format(@"select top {0} o.SchemeId --,o.CreateTime --,t.SchemeId,t.TicketId
                                    from C_Sports_Order_Running o with(nolock)
                                    left join C_Sports_Ticket t with(nolock) on o.SchemeId=t.SchemeId
                                    where o.CanChase=1  {1} and t.SchemeId is null
                                    order by o.CreateTime asc ", count, unTicketSql);
            var array = this.Session.CreateSQLQuery(sql).List();
            var schemeIdList = new List<string>();
            foreach (var schemeId in array)
            {
                schemeIdList.Add(schemeId.ToString());
            }
            return schemeIdList;
            //return string.Join("|", schemeIdList);
        }

        public List<string> QueryUnSplitTicketsOrder()
        {
            Session.Clear();
            var query = from o in this.Session.Query<Sports_Order_Running>()
                        where
                            //o.CanChase == true && 
                        o.IsSplitTickets == false
                        orderby o.CreateTime ascending
                        select o.SchemeId;
            return query.ToList();
        }

        public List<string> QueryUnPrizeOrder()
        {
            Session.Clear();
            var query = from o in this.Session.Query<Sports_Order_Running>()
                        where o.TicketStatus == TicketStatus.Ticketed
                            //&& o.IsSplitTickets == true
                        && o.CreateTime > DateTime.Parse("2016-3-1")
                        orderby o.CreateTime ascending
                        select o.SchemeId;
            return query.ToList();
        }

        public List<Sports_Ticket> QueryTicketList(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Where(p => p.SchemeId == schemeId).ToList();
        }

        public List<Sports_Ticket> QueryComplateTicketListTop(int count)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Where(p => p.BonusStatus != BonusStatus.Waitting).OrderBy(p => p.Id).Take(count).ToList();
        }

        public List<Sports_Ticket> QueryTicketListHistory(string schemeId)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Ticket_History>()
                        where t.SchemeId == schemeId
                        orderby t.TicketId ascending
                        select new Sports_Ticket
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = t.BonusStatus,
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
                            TicketStatus = t.TicketStatus,
                            BetContent = t.BetContent,
                            LocOdds = t.LocOdds,
                            PrintDateTime = t.PrintDateTime,
                        };
            return query.ToList();
        }



        public int QueryTicketCount(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Count(p => p.SchemeId == schemeId);
        }
        public int QuerySuccessTicketCount(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Count(p => p.SchemeId == schemeId && p.TicketStatus == TicketStatus.Ticketed);
        }

        public Sports_Ticket QueryTicket(string ticketId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Where(p => p.TicketId == ticketId).FirstOrDefault();
        }

        public Sports_Ticket QueryFirstTicket(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public Sports_Ticket QueryFirstTicketHistory(string schemeId)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Ticket_History>()
                        where t.SchemeId == schemeId
                        select new Sports_Ticket
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BarCode = t.BarCode,
                            BetContent = t.BetContent,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = t.BonusStatus,
                            CreateTime = t.CreateTime,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            Gateway = t.Gateway,
                            Id = t.Id,
                            IsAppend = t.IsAppend,
                            IssuseNumber = t.IssuseNumber,
                            LocOdds = t.LocOdds,
                            MatchIdList = t.MatchIdList,
                            PlayType = t.PlayType,
                            PreTaxBonusMoney = t.PreTaxBonusMoney,
                            PrintDateTime = t.PrintDateTime,
                            PrintNumber1 = t.PrintNumber1,
                            PrintNumber2 = t.PrintNumber2,
                            PrintNumber3 = t.PrintNumber3,
                            PrizeDateTime = t.PrizeDateTime,
                            SchemeId = t.SchemeId,
                            TicketId = t.TicketId,
                            TicketLog = t.TicketLog,
                            TicketStatus = t.TicketStatus,
                        };
            return query.FirstOrDefault();
        }

        public List<Sports_Ticket> QuerySuccessTicketList(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<Sports_Ticket>().Where(p => p.SchemeId == schemeId && p.TicketStatus == TicketStatus.Ticketed).ToList();
        }

        public List<Sports_TicketQueryInfo> QueryTicketInfoList(string schemeId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Ticket>()
                        where t.SchemeId == schemeId
                        orderby t.TicketId ascending
                        select new Sports_TicketQueryInfo
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = t.BonusStatus,
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
                            TicketStatus = t.TicketStatus,
                            BetContent = t.BetContent,
                            LocOdds = t.LocOdds,
                            PrintDateTime = t.PrintDateTime,
                        };

            totalCount = query.Count();
            if (pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<Sports_TicketQueryInfo> QueryTicketHisgoryInfoList(string schemeId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Ticket_History>()
                        where t.SchemeId == schemeId
                        orderby t.TicketId ascending
                        select new Sports_TicketQueryInfo
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = t.BonusStatus,
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
                            TicketStatus = t.TicketStatus,
                            BetContent = t.BetContent,
                            LocOdds = t.LocOdds,
                            PrintDateTime = t.PrintDateTime,
                        };

            totalCount = query.Count();
            if (pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public void AddReceiveNoticeLog(ReceiveNoticeLog entity)
        {
            this.Add<ReceiveNoticeLog>(entity);
        }
        public void UpdateReceiveNoticeLog(ReceiveNoticeLog entity)
        {
            this.Update<ReceiveNoticeLog>(entity);
        }
        public void DeleteReceiveNoticeLog(ReceiveNoticeLog entity)
        {
            this.Delete<ReceiveNoticeLog>(entity);
        }

        public void AddReceiveNoticeLog_Complate(ReceiveNoticeLog_Complate entity)
        {
            this.Add<ReceiveNoticeLog_Complate>(entity);
        }
        public ReceiveNoticeLog_Complate QueryReceiveNoticeLog_Complate(long noticeId)
        {
            Session.Clear();
            return Session.Get<ReceiveNoticeLog_Complate>(noticeId);
        }

        public List<ReceiveNoticeLogInfo> QueryReceiveNoticeLogList(int noticeType, DateTime startTiem, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTiem.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var result = from s in Session.Query<ReceiveNoticeLog>()
                         where (noticeType == 0 || s.NoticeType == noticeType) && (s.CreateTime >= startTiem && s.CreateTime < endTime.AddDays(1))
                         select new ReceiveNoticeLogInfo
                         {
                             ReceiveNoticeId = s.ReceiveNoticeId,
                             NoticeType = s.NoticeType,
                             ReceiveUrlRoot = s.ReceiveUrlRoot,
                             ReceiveDataString = s.ReceiveDataString,
                             CreateTime = s.CreateTime,
                             Remark = s.Remark,
                             AgentId = s.AgentId,
                             SendTimes = s.SendTimes,
                             Sign = s.Sign,
                             ComplateTime = DateTime.Now,
                         };
            totalCount = result.ToList().Count();
            return result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<ReceiveNoticeLogInfo> QueryComplateReceiveNoticeLogList(int noticeType, DateTime startTiem, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTiem.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var result = from s in Session.Query<ReceiveNoticeLog_Complate>()
                         where (noticeType == -1 || s.NoticeType == noticeType) && (s.ComplateTime >= startTiem && s.ComplateTime < endTime.AddDays(1))
                         select new ReceiveNoticeLogInfo
                         {
                             ReceiveNoticeId = s.ReceiveNoticeId,
                             NoticeType = s.NoticeType,
                             ReceiveUrlRoot = s.ReceiveUrlRoot,
                             ReceiveDataString = s.ReceiveDataString,
                             CreateTime = s.CreateTime,
                             Remark = s.Remark,
                             AgentId = s.AgentId,
                             SendTimes = s.SendTimes,
                             Sign = s.Sign,
                             ComplateTime = s.ComplateTime,
                         };
            totalCount = result.ToList().Count();
            return result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public string QueryReceiveNoticeList(int returnRecord = 0)
        {
            Session.Clear();
            if (returnRecord == 0)
                returnRecord = 500;
            var query = from s in Session.Query<ReceiveNoticeLog>() orderby s.CreateTime ascending select s.ReceiveNoticeId;
            return string.Join("|", query.Take(returnRecord).ToArray());
        }

        public ReceiveNoticeLog QueryReceiveNoticeByReceiveId(string receiveId)
        {
            return Session.Get<ReceiveNoticeLog>(Convert.ToInt64(receiveId));
        }

        public string QueryWaitingTicket(int returnRecord)
        {
            Session.Clear();
            if (returnRecord <= 0)
                returnRecord = 200;
            var query = from s in Session.Query<Sports_Order_Running>()
                        where (s.TicketStatus == TicketStatus.Waitting) && (s.IsVirtualOrder == false) && (s.ProgressStatus == ProgressStatus.Running)
                        orderby s.CreateTime ascending
                        select s.SchemeId;

            return string.Join("|", query.Take(returnRecord).ToArray());
        }

        #region 方案快照查询 2014.11.24 dj

        public OrderSnapshotDetailInfo_JC_Collection QueryJCBettingSnapshotInfo(string schemeId, string gameCode)
        {
            Session.Clear();
            OrderSnapshotDetailInfo_JC_Collection collection = new OrderSnapshotDetailInfo_JC_Collection();
            //var queryHeadInfo = from o in Session.Query<Sports_Order_Running>()
            //                    join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
            //                    where o.SchemeId == schemeId
            //                    select new OrderSnapshotHeadInfo
            //                        {
            //                            UserName = u.DisplayName,
            //                            IssuseNumber = o.IssuseNumber,
            //                            SchemeId = o.SchemeId,
            //                            TotalMoney = o.TotalMoney,
            //                            CreateTime = o.CreateTime,
            //                            Amount=o.Amount,
            //                            GameCode=o.GameCode,
            //                            GameType=o.GameType,
            //                        };
            //if (queryHeadInfo == null)
            //    return collection;
            if (gameCode.ToUpper() == "JCLQ")
            {
                var queryDetailInfo = from ac in Session.Query<Sports_AnteCode>()
                                      join jm in Session.Query<JCLQ_Match>() on ac.MatchId equals jm.MatchId
                                      join o in Session.Query<Sports_Order_Running>() on ac.SchemeId equals o.SchemeId
                                      join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                                      where ac.SchemeId == schemeId
                                      orderby jm.FSStopBettingTime ascending
                                      select new OrderSnapshotDetailInfo_JC
                                      {
                                          MatchIdName = jm.MatchIdName,
                                          UserName = u.DisplayName,
                                          IssuseNumber = o.IssuseNumber,
                                          TotalMoney = o.TotalMoney,
                                          Amount = o.Amount,
                                          SchemeId = o.SchemeId,
                                          AnteCode = ac.AnteCode,
                                          CreateTime = ac.CreateTime,
                                          FSStopBettingTime = jm.FSStopBettingTime,
                                          HomeTeamName = jm.HomeTeamName,
                                          GuestTeamName = jm.GuestTeamName,
                                          GameCode = ac.GameCode,
                                          GameType = ac.GameType,
                                          OrderGameType = o.GameType,
                                      };
                if (queryDetailInfo == null || queryDetailInfo.Count() <= 0)
                    return collection;
                //collection.HeadInfo = queryHeadInfo.FirstOrDefault();
                collection.ListInfo = queryDetailInfo.ToList<OrderSnapshotDetailInfo_JC>();
            }
            else if (gameCode.ToUpper() == "JCZQ")
            {
                var queryDetailInfo = from ac in Session.Query<Sports_AnteCode>()
                                      join jm in Session.Query<JCZQ_Match>() on ac.MatchId equals jm.MatchId
                                      join o in Session.Query<Sports_Order_Running>() on ac.SchemeId equals o.SchemeId
                                      join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                                      where ac.SchemeId == schemeId
                                      orderby jm.FSStopBettingTime ascending
                                      select new OrderSnapshotDetailInfo_JC
                                      {
                                          MatchIdName = jm.MatchIdName,
                                          UserName = u.DisplayName,
                                          IssuseNumber = o.IssuseNumber,
                                          TotalMoney = o.TotalMoney,
                                          Amount = o.Amount,
                                          SchemeId = o.SchemeId,
                                          AnteCode = ac.AnteCode,
                                          CreateTime = ac.CreateTime,
                                          FSStopBettingTime = jm.FSStopBettingTime,
                                          HomeTeamName = jm.HomeTeamName,
                                          GuestTeamName = jm.GuestTeamName,
                                          GameCode = ac.GameCode,
                                          GameType = ac.GameType,
                                          OrderGameType = o.GameType,
                                      };
                if (queryDetailInfo == null || queryDetailInfo.Count() <= 0)
                    return collection;
                //collection.HeadInfo = queryHeadInfo.FirstOrDefault();
                collection.ListInfo = queryDetailInfo.ToList<OrderSnapshotDetailInfo_JC>();
            }
            else if (gameCode.ToUpper() == "BJDC")
            {
                var queryDetailInfo = from ac in Session.Query<Sports_AnteCode>()
                                      join bj in Session.Query<BJDC_Match>() on ac.IssuseNumber + "|" + ac.MatchId equals bj.Id
                                      join o in Session.Query<Sports_Order_Running>() on ac.SchemeId equals o.SchemeId
                                      join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                                      where ac.SchemeId == schemeId
                                      orderby bj.LocalStopTime ascending
                                      select new OrderSnapshotDetailInfo_JC
                                      {
                                          MatchIdName = bj.MatchOrderId != null ? bj.MatchOrderId.ToString().Trim() : "",
                                          UserName = u.DisplayName,
                                          IssuseNumber = o.IssuseNumber,
                                          TotalMoney = o.TotalMoney,
                                          Amount = o.Amount,
                                          SchemeId = o.SchemeId,
                                          AnteCode = ac.AnteCode,
                                          CreateTime = ac.CreateTime,
                                          FSStopBettingTime = bj.LocalStopTime,
                                          HomeTeamName = bj.HomeTeamName,
                                          GuestTeamName = bj.GuestTeamName,
                                          GameCode = ac.GameCode,
                                          GameType = ac.GameType,
                                          OrderGameType = o.GameType,
                                      };
                if (queryDetailInfo == null || queryDetailInfo.Count() <= 0)
                    return collection;
                //collection.HeadInfo = queryHeadInfo.FirstOrDefault();
                collection.ListInfo = queryDetailInfo.ToList<OrderSnapshotDetailInfo_JC>();
            }
            return collection;
        }
        public OrderSnapshotDetailInfo_PT_Collection QueryPTBettingSnapshotInfo(string schemeId)
        {
            Session.Clear();
            OrderSnapshotDetailInfo_PT_Collection collection = new OrderSnapshotDetailInfo_PT_Collection();
            var queryDetailInfo = from o in Session.Query<Sports_Order_Running>()
                                  join ac in Session.Query<Sports_AnteCode>() on o.SchemeId equals ac.SchemeId
                                  join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                                  where o.SchemeId == schemeId
                                  select new OrderSnapshotDetailInfo_PT
                                  {
                                      TotalMoney = o.TotalMoney,
                                      UserName = u.DisplayName,
                                      KeyLine = "",
                                      SchemeId = o.SchemeId,
                                      Amount = o.Amount,
                                      AnteCode = ac.AnteCode,
                                      BetCount = o.BetCount,
                                      IssuseNumber = o.IssuseNumber,
                                      GameCode = ac.GameCode,
                                      GameType = ac.GameType,
                                      CreateTime = o.CreateTime,
                                      OrderGameType = o.GameType,

                                  };
            if (queryDetailInfo == null)
                return collection;
            collection.ListInfo = queryDetailInfo.ToList<OrderSnapshotDetailInfo_PT>();
            return collection;
        }
        public OrderSnapshotDetailInfo_PT_Collection QueryChaseBettingSnapshotInfo(string KeyLine)
        {
            Session.Clear();
            OrderSnapshotDetailInfo_PT_Collection collection = new OrderSnapshotDetailInfo_PT_Collection();
            var queryHeadInfo = from ac in Session.Query<Sports_AnteCode>().ToList()
                                join s in (Session.Query<LotteryScheme>().Where(l => l.KeyLine == KeyLine && l.OrderIndex == 1).ToList()) on ac.SchemeId equals s.SchemeId
                                select new OrderSnapshotHeadInfo
                                    {
                                        AnteCode = ac.AnteCode,
                                        GameCode = ac.GameCode,
                                        GameType = ac.GameType,
                                    };
            if (queryHeadInfo == null || queryHeadInfo.Count() <= 0)
                return collection;
            var queryDetailInfo = from o in Session.Query<OrderDetail>()
                                  join s in Session.Query<LotteryScheme>() on o.SchemeId equals s.SchemeId
                                  join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                                  where s.KeyLine == KeyLine
                                  orderby s.OrderIndex ascending
                                  select new OrderSnapshotDetailInfo_PT
                                  {
                                      IsBonusStop = o.StopAfterBonus,
                                      TotalMoney = o.TotalMoney,
                                      UserName = u.DisplayName,
                                      KeyLine = KeyLine,
                                      SchemeId = o.SchemeId,
                                      Amount = o.Amount,
                                      IssuseNumber = o.CurrentIssuseNumber,
                                      CreateTime = o.CreateTime,
                                      GameCode = o.GameCode,
                                      GameType = o.GameType
                                  };
            if (queryDetailInfo == null || queryDetailInfo.Count() <= 0) return collection;
            collection.ListInfo = queryDetailInfo.ToList<OrderSnapshotDetailInfo_PT>();
            collection.HeadListInfo = queryHeadInfo.ToList<OrderSnapshotHeadInfo>();
            return collection;
        }
        public OrderSnapshotDetailInfo_Together_Collection QueryTogetherBettingSnapshotInfo(string schemeId)
        {
            Session.Clear();
            OrderSnapshotDetailInfo_Together_Collection collection = new OrderSnapshotDetailInfo_Together_Collection();
            //var queryHeadInfo = from o in Session.Query<Sports_Order_Running>()
            //                    join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
            //                    where o.SchemeId == schemeId
            //                    select new OrderSnapshotHeadInfo
            //                    {
            //                        UserName = u.DisplayName,
            //                        IssuseNumber = o.IssuseNumber,
            //                        SchemeId = o.SchemeId,
            //                        TotalMoney = o.TotalMoney,
            //                        CreateTime = o.CreateTime,
            //                        Amount = o.Amount,
            //                        GameCode = o.GameCode,
            //                        GameType = o.GameType,
            //                    };
            //if (queryHeadInfo == null)
            //    return collection;
            var queryDetailInfo = from o in Session.Query<Sports_Order_Running>()
                                  join t in Session.Query<Sports_Together>() on o.SchemeId equals t.SchemeId
                                  join ac in Session.Query<Sports_AnteCode>() on t.SchemeId equals ac.SchemeId
                                  join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                                  where t.SchemeId == schemeId
                                  select new OrderSnapshotDetailInfo_Together
                                  {
                                      BetCount = o.BetCount,
                                      UserName = u.DisplayName,
                                      IssuseNumber = o.IssuseNumber,
                                      SchemeId = o.SchemeId,
                                      Amount = o.Amount,
                                      AnteCode = ac.AnteCode,
                                      Guarantees = t.Guarantees,
                                      Price = t.Price,
                                      Subscription = t.Subscription,
                                      TotalCount = t.TotalCount,
                                      TotalMoney = t.TotalMoney,
                                      CreateTime = o.CreateTime,
                                      GameCode = o.GameCode,
                                      GameType = o.GameType,
                                  };
            if (queryDetailInfo == null || queryDetailInfo.Count() <= 0)
                return collection;
            collection.ListInfo = queryDetailInfo.ToList<OrderSnapshotDetailInfo_Together>();
            return collection;
        }
        public List<Sports_Order_Complate> GetHitMatchCount(string gameCode, string gameType, string issuseNumber, int hitMatch)
        {
            return Session.Query<Sports_Order_Complate>().Where(o => o.GameCode == gameCode && o.GameType == gameType && o.IssuseNumber == issuseNumber && o.HitMatchCount == hitMatch).ToList();
        }

        public Sports_AnteCode QueryAnteCode(string schemeId, string macthId, string gameType)
        {
            return Session.Query<Sports_AnteCode>().FirstOrDefault(s => s.SchemeId == schemeId && s.MatchId == macthId && s.GameType == gameType);
        }
        public List<SFGG_Match> QuerySFGGSaleMatchCount(string[] matchArray)
        {
            return Session.Query<SFGG_Match>().Where(s => matchArray.Contains(s.MatchId)).ToList();
        }

        #endregion

        #region APP相关函数

        public BettingOrderInfoCollection QueryOrderListByBonusState(string strSate, string userId, string strSchemeType, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();

            var param = DynamicLinqExpressions.True<OrderDetail>();
            switch (strSate)
            {
                case "0":
                    param = param.And(o => o.BonusStatus == BonusStatus.Waitting || o.BonusStatus == BonusStatus.Awarding);
                    break;
                case "20":
                    param = param.And(o => o.BonusStatus == BonusStatus.Win);
                    break;
                case "30":
                    param = param.And(o => o.BonusStatus == BonusStatus.Lose || o.BonusStatus == BonusStatus.Error);
                    break;
            }
            switch (strSchemeType)
            {
                case "0":
                    param = param.And(o => o.SchemeType == SchemeType.GeneralBetting || o.SchemeType == SchemeType.TogetherBetting);
                    break;
                case "1":
                    param = param.And(o => o.SchemeType == SchemeType.GeneralBetting);
                    break;
                case "3":
                    param = param.And(o => o.SchemeType == SchemeType.TogetherBetting);
                    break;
            }
            BettingOrderInfoCollection collection = new BettingOrderInfoCollection();
            collection.TotalCount = 0;
            var query = from o in Session.Query<OrderDetail>().Where(param)
                        join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                        where u.UserId == userId && (o.BetTime >= startTime.Date && o.BetTime < endTime.AddDays(1).Date)
                        orderby o.BetTime descending
                        select new BettingOrderInfo
                            {
                                SchemeId = o.SchemeId,
                                GameCode = o.GameCode,
                                GameName = BusinessHelper.FormatGameCode(o.GameCode.ToUpper()),
                                GameTypeName = BusinessHelper.FormatGameType_Each(o.GameCode.ToUpper(), o.GameType.ToUpper()),
                                CreatorDisplayName = u.DisplayName,
                                IssuseNumber = o.CurrentIssuseNumber,
                                CurrentBettingMoney = o.CurrentBettingMoney,
                                BetTime = o.BetTime,
                                SchemeType = o.SchemeType,
                                TicketStatus = o.TicketStatus,
                                ProgressStatus = o.ProgressStatus,
                                PreTaxBonusMoney = o.PreTaxBonusMoney,
                                AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                                BonusStatus = o.BonusStatus,
                                TotalMoney = o.TotalMoney,
                                CreateTime = o.CreateTime
                            };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                if (pageSize == -1)
                    collection.OrderList = query.ToList();
                collection.OrderList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }

        #endregion


        public string QueryTicketAbnormalOrderId()
        {
            Session.Clear();
            //var query = from s in Session.Query<Sports_Order_Running>() where (s.CreateTime >= DateTime.Now.Date && s.CreateTime <= DateTime.Now.Date.AddHours(9)) && s.GameCode != "CTZQ" && s.IsVirtualOrder == false && (s.TicketStatus == TicketStatus.Waitting) select s.SchemeId;
            //return string.Join(",", query.ToArray());
            List<string> ListSchemeId = new List<string>();
            var query = from s in Session.Query<Sports_Order_Running>()
                        where (s.CreateTime >= DateTime.Now.Date && s.CreateTime <= DateTime.Now.Date.AddHours(9))
                        && s.GameCode != "CTZQ" && s.IsVirtualOrder == false
                        && (s.TicketStatus == TicketStatus.Waitting && s.CanChase == true)
                        && (s.SchemeType == SchemeType.GeneralBetting || s.SchemeType == SchemeType.TogetherBetting || s.SchemeType == SchemeType.SingleCopy)
                        select s.SchemeId;
            if (query != null)
                ListSchemeId.AddRange(query.ToList());

            //查询当前时间以前，并且满足条件的合买订单
            var togetherQuery = from s in Session.Query<Sports_Order_Running>()
                                where (s.CreateTime < DateTime.Now.Date) && s.GameCode != "CTZQ" && s.IsVirtualOrder == false
                                && (s.TicketStatus == TicketStatus.Waitting && s.CanChase == true)
                                && (s.SchemeType == SchemeType.TogetherBetting)
                                select s.SchemeId;
            if (togetherQuery != null)
                ListSchemeId.AddRange(togetherQuery.ToList());

            if (ListSchemeId != null && ListSchemeId.Count > 0)
                return string.Join(",", ListSchemeId.ToArray());
            else return string.Empty;
        }

        public List<Sports_Ticket> QueryAllOrderTicket(string schemeId, string gameCode, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            return Session.Query<Sports_Ticket>().Where(s => (s.SchemeId == schemeId || schemeId == string.Empty) && (gameCode == string.Empty || s.GameCode == gameCode) && (s.CreateTime >= startTime && s.CreateTime < endTime)).OrderBy(s => s.CreateTime).ToList();
        }
        public OrderDetail QueryOrderDetailBySchemeId(string schemeId)
        {
            Session.Clear();
            return Session.Query<OrderDetail>().FirstOrDefault(s => s.SchemeId == schemeId);
        }
        public decimal QueryCurrBetMoney()
        {
            Session.Clear();
            var result = Session.Query<Sports_Order_Running>().Where(o => (o.TicketStatus == TicketStatus.Ticketing || o.TicketStatus == TicketStatus.PrintTicket) && o.ProgressStatus == ProgressStatus.Running).ToList();
            if (result != null && result.Count > 0)
                return result.Sum(o => o.TotalMoney);
            return 0;
        }
        public TogetherFollowerRuleQueryInfo QueryMyTogetherFollowerRuleById(long ruleId)
        {
            Session.Clear();
            var query = from t in Session.Query<TogetherFollowerRule>()
                        join u in Session.Query<UserRegister>() on t.CreaterUserId equals u.UserId
                        where t.Id == ruleId
                        select new TogetherFollowerRuleQueryInfo
                        {
                            RuleId = t.Id,
                            CreaterUserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            FollowerPercent = t.FollowerPercent,
                            FollowerCount = t.FollowerCount,
                            SchemeCount = t.SchemeCount,
                            MinSchemeMoney = t.MinSchemeMoney,
                            MaxSchemeMoney = t.MaxSchemeMoney,
                            CancelWhenSurplusNotMatch = t.CancelWhenSurplusNotMatch,
                            StopFollowerMinBalance = t.StopFollowerMinBalance,
                            IsEnable = t.IsEnable,
                        };
            if (query != null)
                return query.FirstOrDefault();
            return new TogetherFollowerRuleQueryInfo();
        }
        public decimal GetUserMaxBonusMoney(string userId)
        {
            Session.Clear();
            string strSql = "select isnull(max(AfterTaxBonusMoney),0) maxBonusMoney from C_Sports_Order_Complate where UserId=:UserId and BonusStatus=20 and IsVirtualOrder=0";
            var query = Session.CreateSQLQuery(strSql).SetString("UserId", userId).List();
            return Convert.ToDecimal(query[0]);
        }

        public string QueryNoChaseOrder()
        {
            Session.Clear();
            var sql = @"select s.KeyLine, s.SchemeId,s.IssuseNumber,i.WinNumber
                        from [C_Lottery_Scheme] s 
                        left join [C_OrderDetail] o on s.SchemeId=o.SchemeId
                        left join C_Game_Issuse i on s.IssuseNumber=i.IssuseNumber and o.GameCode=i.GameCode
                        where s.IsComplate=0 and o.TicketStatus=0 and i.WinNumber<>''
                        order by s.OrderIndex asc";

            var array = this.Session.CreateSQLQuery(sql).List();
            if (array == null)
                return string.Empty;
            var schemeIdList = new List<string>();
            foreach (var item in array)
            {
                var row = item as object[];
                schemeIdList.Add(UsefullHelper.GetDbValue<string>(row[1]));
            }
            return string.Join("|", schemeIdList.ToArray());
        }

        #region 优化函数


        public List<Sports_Order_Complate> GetHitMatchCount_YouHua(string gameCode, string gameType, string issuseNumber)
        {
            return Session.Query<Sports_Order_Complate>().Where(o => o.GameCode == gameCode && (o.GameType == gameType || gameType == string.Empty) && o.IssuseNumber == issuseNumber).ToList();
        }

        #endregion

        public List<Sports_Ticket> QueryUnPrizeTicket(string gameCode, string gameType, string issuseNumber, int count)
        {
            Session.Clear();
            var query = from t in this.Session.Query<Sports_Ticket>()
                        where t.GameCode == gameCode
                        && (gameType == "" || t.GameType == gameType)
                        && (issuseNumber == "" || t.IssuseNumber == issuseNumber)
                        && t.BonusStatus == BonusStatus.Waitting
                        && t.TicketStatus == TicketStatus.Ticketed
                        select t;
            if (count < 0)
                return query.ToList();
            return query.Take(count).ToList();
        }

        //public List<Sports_Ticket> QuerySZCUnPrizeTicket(string gameCode, int count)
        //{
        //    Session.Clear();
        //    var query = from t in this.Session.Query<Sports_Ticket>()
        //                join i in this.Session.Query<GameIssuse>() on new { GameCode = t.GameCode, IssuseNumber = t.IssuseNumber } equals new { GameCode = i.GameCode, IssuseNumber = i.IssuseNumber }
        //                where t.GameCode == gameCode
        //                && t.BonusStatus == BonusStatus.Waitting
        //                && t.TicketStatus == TicketStatus.Ticketed
        //                && i.Status == IssuseStatus.Stopped
        //                && i.WinNumber != ""
        //                select t;
        //    if (count < 0)
        //        return query.ToList();
        //    return query.Take(count).ToList();
        //}

        public List<TicketPrizeInfo> QuerySZCUnPrizeTicket(string gameCode, int count)
        {
            if (count <= 0)
                count = 100;
            Session.Clear();
            var sql = string.Format(@"select top {0} t.ticketId,t.gamecode,t.gametype,t.betcontent,t.amount,t.isappend,t.issusenumber,i.WinNumber,t.id
                                        from C_Sports_Ticket t
                                        left join C_Game_Issuse i on t.gamecode=i.gamecode and t.IssuseNumber=i.IssuseNumber
                                        where t.BonusStatus=0 and t.TicketStatus=90 
                                        and i.WinNumber is not null 
                                        and i.Status=30
                                        and  i.WinNumber<>''
                                        and i.gamecode='{1}'
                                        order by t.issusenumber asc", count, gameCode);
            var list = this.Session.CreateSQLQuery(sql).List();
            var ticketList = new List<TicketPrizeInfo>();
            foreach (var item in list)
            {
                if (item == null)
                    continue;
                var array = item as object[];
                ticketList.Add(new TicketPrizeInfo
                {
                    TicketId = UsefullHelper.GetDbValue<string>(array[0]),
                    GameCode = UsefullHelper.GetDbValue<string>(array[1]),
                    GameType = UsefullHelper.GetDbValue<string>(array[2]),
                    BetContent = UsefullHelper.GetDbValue<string>(array[3]),
                    Amount = UsefullHelper.GetDbValue<int>(array[4]),
                    IsAppend = UsefullHelper.GetDbValue<bool>(array[5]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(array[6]),
                    WinNumber = UsefullHelper.GetDbValue<string>(array[7]),
                    Id = UsefullHelper.GetDbValue<long>(array[8]),
                });
            }
            return ticketList;
        }

        //public List<Sports_Ticket> QueryCTZQUnPrizeticket(string gameType, int count)
        //{
        //    Session.Clear();
        //    var query = from t in this.Session.Query<Sports_Ticket>()
        //                join p in this.Session.Query<GameBiz.Business.Domain.Entities.Ticket.Ticket_BonusPool>() on new { GameCode = t.GameCode, GameType = t.GameType, IssuseNumber = t.IssuseNumber } equals new { GameCode = p.GameCode, GameType = p.GameType, IssuseNumber = p.IssuseNumber }
        //                where t.GameCode == "CTZQ" && t.GameType == gameType
        //                && t.BonusStatus == BonusStatus.Waitting
        //                && t.TicketStatus == TicketStatus.Ticketed
        //                && p.BonusLevel == "1"
        //                && p.WinNumber != ""
        //                select t;

        //    if (count < 0)
        //        return query.ToList();
        //    return query.Take(count).ToList();
        //}

        public List<TicketPrizeInfo> QueryCTZQUnPrizeticket(string gameType, int count)
        {
            if (count <= 0)
                count = 100;
            Session.Clear();
            var sql = string.Format(@"select top {0} t.ticketId,t.gamecode,t.gametype,t.betcontent,t.amount,t.isappend,t.issusenumber,p.WinNumber,t.id
                                    from C_Sports_Ticket t
                                    left join [T_Ticket_BonusPool] p on t.gamecode=p.gamecode and t.gametype=p.gametype and t.issusenumber =p.issusenumber
                                    where t.gamecode='ctzq' and t.gametype='{1}' and t.bonusstatus=0 and t.TicketStatus=90 
                                    and p.bonuslevel=1 and p.BonusCount>0 and p.BonusMoney>0
                                    and p.WinNumber<>''
                                    order by t.issusenumber asc ", count, gameType);
            var list = this.Session.CreateSQLQuery(sql).List();
            var ticketList = new List<TicketPrizeInfo>();
            foreach (var item in list)
            {
                if (item == null)
                    continue;
                var array = item as object[];
                ticketList.Add(new TicketPrizeInfo
                {
                    TicketId = UsefullHelper.GetDbValue<string>(array[0]),
                    GameCode = UsefullHelper.GetDbValue<string>(array[1]),
                    GameType = UsefullHelper.GetDbValue<string>(array[2]),
                    BetContent = UsefullHelper.GetDbValue<string>(array[3]),
                    Amount = UsefullHelper.GetDbValue<int>(array[4]),
                    IsAppend = UsefullHelper.GetDbValue<bool>(array[5]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(array[6]),
                    WinNumber = UsefullHelper.GetDbValue<string>(array[7]),
                    Id = UsefullHelper.GetDbValue<long>(array[8]),
                });
            }
            return ticketList;
        }


        public List<string> QueryUnPrizeOrder(string gameCode, int count)
        {
            Session.Clear();
            var sql = string.Format(@"select top {1} o.SchemeId 
                                    from C_Sports_Order_Running o
                                    inner join C_Sports_Ticket t on o.SchemeId=t.SchemeId
                                    where t.BonusStatus in (20,30) 
                                    and t.gamecode='{0}' 
                                    group by o.SchemeId having min(t.BonusStatus)>=20 ", gameCode, count);
            var list = this.Session.CreateSQLQuery(sql).List();
            var schemeIdList = new List<string>();
            foreach (var item in list)
            {
                if (item == null)
                    continue;
                schemeIdList.Add(item.ToString());
            }
            return schemeIdList;
        }

        public List<string> QueryWaitPayRebateRunningOrder()
        {
            Session.Clear();
            var query = from o in this.Session.Query<Sports_Order_Running>()
                        where o.IsPayRebate == false
                        && o.CanChase == true
                        && o.TicketStatus == TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        orderby o.CreateTime ascending
                        select o.SchemeId;
            return query.ToList();
        }
        public List<string> QueryWaitPayRebateComplateOrder()
        {
            Session.Clear();
            var query = from o in this.Session.Query<Sports_Order_Complate>()
                        where o.IsPayRebate == false
                        && o.CanChase == true
                        && o.TicketStatus == TicketStatus.Ticketed
                        && o.IsVirtualOrder == false
                        && o.CreateTime > DateTime.Parse("2016-07-11")
                        orderby o.CreateTime ascending
                        select o.SchemeId;
            return query.ToList();
        }

        public List<Sports_Order_Running> QueryWaitTicketOrder(string[] noChaseGameCodeArray, int afterSeconds)
        {
            Session.Clear();
            var query = from o in this.Session.Query<Sports_Order_Running>()
                        where o.CanChase == true && o.TicketStatus == TicketStatus.Waitting
                        && (noChaseGameCodeArray.Length == 0 || !noChaseGameCodeArray.Contains(o.GameCode))
                        && o.CreateTime < DateTime.Now.AddSeconds(-afterSeconds)
                        select o;
            return query.ToList();
        }

        public void UpdateTicketList(string[] ticketList)
        {
            Session.Clear();
            var sql = string.Format("update C_Sports_Ticket set BonusStatus=30,PreTaxBonusMoney=-1,AfterTaxBonusMoney=-1 where TicketId in ({0})"
                , string.Join(",", ticketList.Select(p => string.Format("'{0}'", p))));
            Session.CreateSQLQuery(sql).ExecuteUpdate();

            //string strSql = "update C_Sports_Ticket set BonusStatus=:bonusStatus,PreTaxBonusMoney=-1,AfterTaxBonusMoney=-1 where TicketId in (:ticketId)";
            //var result = Session.CreateSQLQuery(strSql)
            //       .SetInt32("bonusStatus", (int)BonusStatus.Lose)
            //       .SetParameterList("ticketId", ticketList).UniqueResult();
        }

        public void SetNotOpenTickets(string gameCode, string issuseNumber)
        {
            Session.Clear();
            var sql = string.Format(@"update [C_Sports_Ticket] set [BonusStatus]=30,[PreTaxBonusMoney]=-1,[AfterTaxBonusMoney]=-1 where [GameCode]='{0}' and [IssuseNumber]='{1}' ", gameCode, issuseNumber);
            Session.CreateSQLQuery(sql).ExecuteUpdate();
        }


        public IndexMatch QueryIndexMatchById(int id)
        {
            Session.Clear();
            return Session.Query<IndexMatch>().FirstOrDefault(s => s.Id == id);
        }
        public IndexMatch QueryIndexMatchByMatchId(string matchId)
        {
            Session.Clear();
            return Session.Query<IndexMatch>().FirstOrDefault(s => s.MatchId == matchId);
        }
        public IndexMatch_Collection QueryIndexMatchCollection(string matchId, string hasImg, int pageIndex, int pageSize)
        {
            Session.Clear();
            IndexMatch_Collection collection = new IndexMatch_Collection();
            collection.TotalCount = 0;
            var query = from s in Session.Query<IndexMatch>()
                        where (matchId == string.Empty || s.MatchId == matchId || s.MatchName == matchId) && (hasImg == "-1" || s.ImgPath == string.Empty)
                        select new IndexMatchInfo
                        {
                            Id = s.Id,
                            ImgPath = s.ImgPath,
                            MatchId = s.MatchId,
                            MatchName = s.MatchName,
                            CreateTime = s.CreateTime,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                collection.IndexMatchList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }

        public void SqlBulkAddTable(DataTable dt)
        {
            if (dt.Rows.Count <= 0) return;
            GameBiz.Business.Domain.Managers.SqlBulkCopyHelper.WriteTableToDataBase(dt, Session.Connection as System.Data.SqlClient.SqlConnection, new System.Data.SqlClient.SqlRowsCopiedEventHandler((obj, arg) =>
            {
                Console.WriteLine(arg.RowsCopied.ToString());
            }));
        }

        public void SqlBulkAddTable(DataTable dt, params string[] columns)
        {
            if (dt.Rows.Count <= 0) return;
            GameBiz.Business.Domain.Managers.SqlBulkCopyHelper.WriteTableToDataBase(dt, Session.Connection as System.Data.SqlClient.SqlConnection, new System.Data.SqlClient.SqlRowsCopiedEventHandler((obj, arg) =>
            {
                Console.WriteLine(arg.RowsCopied.ToString());
            }), columns);
        }

        public void UpdateTable(DataTable dt, string strTblName)
        {
            GameBiz.Business.Domain.Managers.SqlBulkCopyHelper.UpdateTable(dt, strTblName, Session.Connection as System.Data.SqlClient.SqlConnection);
        }

        public string QueryErrorTicketOrder(int count)
        {
            if (count <= 0)
                return string.Empty;
            Session.Clear();
            var sql = string.Format(@"select top {0} r.SchemeId,r.CreateTime,t.Id
                                        from C_Sports_Order_Running r
                                        left join   C_Sports_Ticket t  on r.SchemeId=t.SchemeId
                                        where 1=1
                                         and r.CreateTime>'2017-1-1'
                                         and r.TicketStatus=90
                                         and r.ProgressStatus=10
                                         and r.IsVirtualOrder=0
                                         and (t.Id='' or t.Id is null)
                                         order by r.CreateTime desc", count);

            var result = new List<string>();
            var list = this.Session.CreateSQLQuery(sql).List();
            foreach (var item in list)
            {
                if (item == null)
                    continue;
                var array = item as object[];
                if (array.Length <= 0)
                    continue;
                var schemeId = array[0].ToString();
                result.Add(schemeId);
            }
            return string.Join("|", result.ToArray());
        }


    }
    /// <summary>
    /// linq动态表达式
    /// </summary>
    public static class DynamicLinqExpressions
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }
        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.Or(expression1.Body, invokedExpression), expression1.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1.Body, invokedExpression), expression1.Parameters);
        }

    }
}
