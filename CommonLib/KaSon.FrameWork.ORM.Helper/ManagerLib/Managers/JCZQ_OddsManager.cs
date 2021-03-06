﻿using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// kason
    /// </summary>
   public class JCZQ_OddsManager : DBbase
    {
        /// <summary>
        /// 存错过程查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oddsType"></param>
        /// <param name="matchId"></param>
        /// <param name="IsDS"></param>
        /// <returns></returns>
        public T GetLastOdds<T>( string oddsType, string matchId, bool IsDS)
            where T :  new()
        {
          //  Session.Clear();
            var isDS = IsDS == true ? 1 : 0;
            var tp = typeof(T);
            //  Session.Clear();
            //var isDS = IsDS == true ? 1 : 0;
            //var tp = typeof(T);

            //string spf = "JCZQ_Odds_SPF".ToLower();
            //switch (tp.Name.ToLower())
            //{
            //    case "jczq_odds_spf":


            //    default:
            //        break;
            //}
            // 通过数据库存储过程进行查询
            SQLModel sqlmodel = new SQLModel();
            if (oddsType == "SPF" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCZQ_GetLastOddsByMatchId_SPF").FirstOrDefault();
            }
            else
            if (oddsType == "BRQSPF" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCZQ_GetLastOddsByMatchId_BRQSPF").FirstOrDefault();
            }
            else
            if (oddsType == "ZJQ" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCZQ_GetLastOddsByMatchId_ZJQ").FirstOrDefault();
            }
            else
            if (oddsType == "BQC" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCZQ_GetLastOddsByMatchId_BQC").FirstOrDefault();
            }
            else
            if (oddsType == "BF" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCZQ_GetLastOddsByMatchId_BF").FirstOrDefault();
            }
            else
            {
                throw new Exception("查询竞彩足球赔率出错，不支持的玩法 - " + oddsType);
            }
            //var query = CreateOutputQuery(Session.GetNamedQuery("P_JCZQ_GetLastOddsByMatchId"))
            //    .AddInParameter("MatchId", matchId)
            //    .AddInParameter("OddsType", oddsType)
            //    .AddInParameter("IsDS", isDS);

            //var dt = query.GetDataTable();
            //if (dt.Rows.Count == 0)
            //{
            //    return null;
            //}
            //return ORMHelper.ConvertDataRowToInfo<T>(dt.Rows[0]);
            return DB.CreateSQLQuery(sqlmodel.SQL).SetString("@MatchId", matchId).First<T>();
            //string spf = "JCZQ_Odds_SPF".ToLower();
            //switch (tp.Name.ToLower())
            //{
            //    case "jczq_odds_spf":


            //    default:
            //        break;
            //}
            // 通过数据库存储过程进行查询

            //var query = CreateOutputQuery(Session.GetNamedQuery("P_JCZQ_GetLastOddsByMatchId"))
            //    .AddInParameter("MatchId", matchId)
            //    .AddInParameter("OddsType", oddsType)
            //    .AddInParameter("IsDS", isDS);

            //var dt = query.GetDataTable();
            //if (dt.Rows.Count == 0)
            //{
            //    return null;
            //}
            //return ORMHelper.ConvertDataRowToInfo<T>(dt.Rows[0]);
           // return DB.CreateSQLQuery("").First<T>();
        }

        public bool CheckAllMatchUpdatedOdds(string gameCode, string gameType, string[] matchId)
        {
            if (gameCode.ToUpper() == "BJDC")
                return true;
            if (matchId == null || matchId.Length == 0)
            {
                throw new ArgumentException("未传入任何比赛");
            }
            var sql = string.Empty;
            switch (gameCode.ToUpper())
            {
                case "JCZQ":
                    //[MatchId], MAX(Id)
                    sql = string.Format("SELECT Count(Id)  FROM [T_JCZQ_Odds_{0}] WHERE [MatchId] IN (N'{1}') GROUP BY [MatchId]", gameType, string.Join("',N'", matchId));
                    break;
                case "JCLQ":
                    sql = string.Format("SELECT Count(Id)  FROM [T_JCLQ_Odds_{0}] WHERE [MatchId] IN (N'{1}') GROUP BY [MatchId]", gameType, string.Join("',N'", matchId));
                    break;
                default:
                    break;
            }
            var count= DB.CreateSQLQuery(sql).First<int>();
            return count == matchId.Length;
          //  var dt = this.Session.CreateSQLQuery(sql).List();
           // return (dt.Count == matchId.Length);
        }
    }
}
