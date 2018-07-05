using EntityModel;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
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
            return this.DB.CreateQuery<C_Sports_Ticket>().Count(p => p.SchemeId == schemeId);
        }
        public C_Sports_Order_Running QuerySports_Order_Running(string schemeId)
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Sports_Order_Running>().FirstOrDefault(p => p.SchemeId == schemeId);
        }
        public void AddSports_Order_Running(C_Sports_Order_Running entity)
        {
            DB.GetDal<C_Sports_Order_Running>().Add(entity);
        }
        public void SqlBulkAddTable(DataTable dt)
        {
            if (dt.Rows.Count <= 0) return;
            throw new NotImplementedException("SqlBulkAddTable");
            //GameBiz.Business.Domain.Managers.SqlBulkCopyHelper.WriteTableToDataBase(dt, Session.Connection as System.Data.SqlClient.SqlConnection, new System.Data.SqlClient.SqlRowsCopiedEventHandler((obj, arg) =>
            //{
            //    Console.WriteLine(arg.RowsCopied.ToString());
            //}));
        }
        public void ExecSql(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;
            //  Session.Clear();
            this.DB.CreateSQLQuery(sql);//.ExecuteUpdate();
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
            return DB.CreateQuery<C_User_Beedings>().FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && (string.Empty == gameType || p.GameType == gameType));
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
            //Session.Clear();
            return DB.CreateQuery<C_User_BonusPercent>().FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && (gameType == string.Empty || p.GameType == gameType));
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
            return this.DB.CreateQuery<C_SingleScheme_AnteCode>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public void UpdateSports_Order_Running(params C_Sports_Order_Running[] entity)
        {
            DB.GetDal<C_Sports_Order_Running>().Add(entity);
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
    }
}
