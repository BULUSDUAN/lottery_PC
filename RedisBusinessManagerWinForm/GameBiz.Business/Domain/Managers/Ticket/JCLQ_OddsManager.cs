using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business.Domain.Entities.Ticket;
using Common.Database.ORM;

namespace GameBiz.Business.Domain.Managers.Ticket
{
    public class JCLQ_OddsManager : GameBizEntityManagement
    {
        public void AddOdds<T>(T odds)
            where T : JingCai_Odds
        {
            this.Add(odds);
        }
        public void UpdateOdds<T>(T odds)
            where T : JingCai_Odds
        {
            this.Update(odds);
        }
        public void AddJCLQ_Odds_SF(JCLQ_Odds_SF entity)
        {
            this.Add<JCLQ_Odds_SF>(entity);
        }
       
        public void AddJCLQ_Odds_RFSF(JCLQ_Odds_RFSF entity)
        {
            this.Add<JCLQ_Odds_RFSF>(entity);
        }

        public void AddJCLQ_Odds_SFC(JCLQ_Odds_SFC entity)
        {
            this.Add<JCLQ_Odds_SFC>(entity);
        }
       
        public void AddJCLQ_Odds_DXF(JCLQ_Odds_DXF entity)
        {
            this.Add<JCLQ_Odds_DXF>(entity);
        }
     
        public T GetLastOdds<T>(string oddsType, string matchId, bool IsDS)
            where T : JingCai_Odds, new()
        {
            Session.Clear();
            var isDS = IsDS == true ? 1 : 0;
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_JCLQ_GetLastOddsByMatchId"))
                .AddInParameter("MatchId", matchId)
                .AddInParameter("OddsType", oddsType)
                .AddInParameter("IsDS", isDS);

            var dt = query.GetDataTable();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            //return ORMHelper.ConvertDataRowToEntity<T>(dt.Rows[0]);
            return ORMHelper.ConvertDataRowToInfo<T>(dt.Rows[0]);
        }
    }
}
