using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.ORM;

namespace GameBiz.Auth.Business.Domain.Managers.Ticket
{
    public class JCZQ_OddsManager : GameBizAuthEntityManagement
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
        public void AddJCZQ_Odds_SPF(JCZQ_Odds_SPF entity)
        {
            this.Add<JCZQ_Odds_SPF>(entity);
        }
        public void AddJCZQ_Odds_SPF_DS(JCZQ_Odds_SPF_DS entity)
        {
            this.Add<JCZQ_Odds_SPF_DS>(entity);
        }
        public void AddJCZQ_Odds_BRQSPF(JCZQ_Odds_BRQSPF entity)
        {
            this.Add<JCZQ_Odds_BRQSPF>(entity);
        }
        public void AddJCZQ_Odds_BRQSPF_DS(JCZQ_Odds_BRQSPF_DS entity)
        {
            this.Add<JCZQ_Odds_BRQSPF_DS>(entity);
        }

        public void AddJCZQ_Odds_BF(JCZQ_Odds_BF entity)
        {
            this.Add<JCZQ_Odds_BF>(entity);
        }
        public void AddJCZQ_Odds_BF_DS(JCZQ_Odds_BF_DS entity)
        {
            this.Add<JCZQ_Odds_BF_DS>(entity);
        }
        public void AddJCZQ_Odds_ZJQ(JCZQ_Odds_ZJQ entity)
        {
            this.Add<JCZQ_Odds_ZJQ>(entity);
        }
        public void AddJCZQ_Odds_ZJQ_DS(JCZQ_Odds_ZJQ_DS entity)
        {
            this.Add<JCZQ_Odds_ZJQ_DS>(entity);
        }
        public void AddJCZQ_Odds_BQC(JCZQ_Odds_BQC entity)
        {
            this.Add<JCZQ_Odds_BQC>(entity);
        }
        public void AddJCZQ_Odds_BQC_DS(JCZQ_Odds_BQC_DS entity)
        {
            this.Add<JCZQ_Odds_BQC_DS>(entity);
        }
        public T GetLastOdds<T>(string oddsType, string matchId, bool IsDS)
            where T : JingCai_Odds, new()
        {
            Session.Clear();
            var isDS = IsDS == true ? 1 : 0;
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_JCZQ_GetLastOddsByMatchId"))
                .AddInParameter("MatchId", matchId)
                .AddInParameter("OddsType", oddsType)
                .AddInParameter("IsDS", isDS);

            var dt = query.GetDataTable();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return ORMHelper.ConvertDataRowToInfo<T>(dt.Rows[0]);
        }
    }
}
