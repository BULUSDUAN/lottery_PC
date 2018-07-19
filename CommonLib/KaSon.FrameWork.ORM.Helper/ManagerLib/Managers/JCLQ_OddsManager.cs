using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// kason
    /// </summary>
   public class JCLQ_OddsManager : DBbase
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

            //string spf = "JCZQ_Odds_SPF".ToLower();
            //switch (tp.Name.ToLower())
            //{
            //    case "jczq_odds_spf":


            //    default:
            //        break;
            //}
            // 通过数据库存储过程进行查询
            SQLModel sqlmodel = new SQLModel();
            if (oddsType == "SF" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCLQ_GetLastOddsByMatchId_SF").FirstOrDefault();
            }
            else
            if (oddsType == "RFSF" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCLQ_GetLastOddsByMatchId_RFSF").FirstOrDefault();
            }
            else
            if (oddsType == "SFC" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCLQ_GetLastOddsByMatchId_SFC").FirstOrDefault();
            }
            else
            if (oddsType == "DXF" && !IsDS)
            {
                sqlmodel = SqlModule.BettiongModule.Where(b => b.Key == "P_JCLQ_GetLastOddsByMatchId_DXF").FirstOrDefault();
            }
            
            else {
                throw new Exception("查询竞彩篮球赔率出错，不支持的玩法 - " + oddsType);
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
        }
    }
}
