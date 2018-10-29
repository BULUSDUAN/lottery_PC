using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class BDFXManager : DBbase
    {
        public void AddTotalSingleTreasure(C_TotalSingleTreasure entity)
        {
            DB.GetDal<C_TotalSingleTreasure>().Add(entity);
        }
        public void UpdateTotalSingleTreasure(C_TotalSingleTreasure entity)
        {
            DB.GetDal<C_TotalSingleTreasure>().Update(entity);
        }
        public C_TotalSingleTreasure QueryTotalSingleTreasureBySchemeId(string schemeId)
        {
           
            return DB.CreateQuery<C_TotalSingleTreasure>().Where(s => s.SchemeId == schemeId).FirstOrDefault();
        }
        public C_BDFX_RecordSingleCopy QueryBDFXRecordSingleCopyBySchemeId(string singleCopyId)
        {
          
            return DB.CreateQuery<C_BDFX_RecordSingleCopy>().Where(s => s.SingleCopySchemeId == singleCopyId).FirstOrDefault();
        }
        public void AddBDFXRecordSingleCopy(C_BDFX_RecordSingleCopy entity)
        {
            DB.GetDal<C_BDFX_RecordSingleCopy>().Add(entity);
        }
        public void AddSingleTreasureAttentionSummary(C_SingleTreasure_AttentionSummary entity)
        {
            DB.GetDal<C_SingleTreasure_AttentionSummary>().Add(entity);
        }
        public void UpdateSingleTreasureAttentionSummary(C_SingleTreasure_AttentionSummary entity)
        {
            DB.GetDal<C_SingleTreasure_AttentionSummary>().Update(entity);
        }
        public C_SingleTreasure_AttentionSummary QuerySingleTreasureAttentionSummaryByUserId(string userId)
        {
            
            return DB.CreateQuery<C_SingleTreasure_AttentionSummary>().Where(s => s.UserId == userId).FirstOrDefault();
        }

        public void AddSingleTreasureAttention(C_SingleTreasure_Attention entity)
        {
            DB.GetDal<C_SingleTreasure_Attention>().Add(entity);
        }
        public void UpdateSingleTreasureAttention(C_SingleTreasure_Attention entity)
        {
            DB.GetDal<C_SingleTreasure_Attention>().Update(entity);
        }
        public void DeleteSingleTreasureAttention(C_SingleTreasure_Attention entity)
        {
            DB.GetDal<C_SingleTreasure_Attention>().Delete(entity);
        }
        public C_SingleTreasure_Attention QuerySingleTreasureAttentionByUserId(string beConcernedUserId, string concernedUserId)
        {
          
            return DB.CreateQuery<C_SingleTreasure_Attention>().Where(s => s.BeConcernedUserId == beConcernedUserId && s.ConcernedUserId == concernedUserId).FirstOrDefault();
        }

        public List<C_SingleTreasure_Attention> QueryBDFXAllAttentionIList()
        {
          
            var query = from a in DB.CreateQuery<C_SingleTreasure_Attention>()
                        select new C_SingleTreasure_Attention
                        {
                            Id = a.Id,
                            BeConcernedUserId = a.BeConcernedUserId,
                            ConcernedUserId = a.ConcernedUserId,
                            CreateTime = a.CreateTime,
                        };
            if (query != null && query.Count() > 0) return query.ToList();
            return new List<C_SingleTreasure_Attention>();
        }
        public List<string> QueryBeConcernedUserIdList(string concernedUserId)
        {
           
            return (from o in DB.CreateQuery<C_SingleTreasure_Attention>().Where(s => s.ConcernedUserId == concernedUserId) select o.BeConcernedUserId).ToList();
        }
        public List<C_Sports_AnteCode> QueryAnteCodeList(string[] arrSchemeId)
        {
        
            var query = from a in DB.CreateQuery<C_Sports_AnteCode>()
                        where arrSchemeId.Contains(a.SchemeId)
                        select new C_Sports_AnteCode
                        {
                            AnteCode = a.AnteCode,
                            GameType = a.GameType,
                            GameCode = a.GameCode,
                            IsDan = a.IsDan,
                            IssuseNumber = a.IssuseNumber,
                            MatchId = a.MatchId,
                            PlayType = a.PlayType,
                            SchemeId = a.SchemeId,
                        };
            if (query != null && query.Count() > 0)
                return query.ToList();
            return new List<C_Sports_AnteCode>();
        }
        public List<AnteCodeInfo> QueryAnteCodeListBySchemeId(string schemeId)
        {
          
            var query = from a in DB.CreateQuery<C_Sports_AnteCode>()
                        where a.SchemeId == schemeId
                        select new AnteCodeInfo
                        {
                            AnteCode = a.AnteCode,
                            GameType = a.GameType,
                            GameCode = a.GameCode,
                            IsDan = a.IsDan,
                            IssuseNumber = a.IssuseNumber,
                            MatchId = a.MatchId,
                            PlayType = a.PlayType,
                            SchemeId = a.SchemeId,
                        };
            if (query != null && query.Count() > 0)
                return query.ToList();
            return new List<AnteCodeInfo>();
        }

        //public TotalSingleTreasure_Collection QueryTodayBDFXList(string userId, string userName, string gameCode, string orderBy, string desc, DateTime startTime, DateTime endTime, string isMyBD, int pageIndex, int pageSize)
        //{

        //    TotalSingleTreasure_Collection collection = new TotalSingleTreasure_Collection();
        //    collection.TotalCount = 0;

        //    //计算上周时间
        //    var currTime = DateTime.Now;
        //    int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
        //    if (currTime.DayOfWeek != 0)
        //        currTime = currTime.AddDays(-day);
        //    else
        //        currTime = currTime.AddDays(-6);
        //    var sTime = currTime.AddDays(-7).Date;
        //    var eTime = currTime.Date;
        //    Dictionary<string, object> outputs;
        //    var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryTodayBDFX"))
        //                                .AddInParameter("UserId", userId)
        //                                .AddInParameter("UserName", userName)
        //                                .AddInParameter("GameCode", gameCode)
        //                                .AddInParameter("OrderBy", orderBy)
        //                                .AddInParameter("Desc", desc)
        //                                .AddInParameter("PageIndex", pageIndex)
        //                                .AddInParameter("PageSize", pageSize)
        //                                .AddInParameter("StartTime", startTime)
        //                                .AddInParameter("EndTime", endTime)
        //                                .AddInParameter("LastweekStartTime", sTime)
        //                                .AddInParameter("LastweekEndTime", eTime)
        //                                .AddInParameter("IsMyBD", isMyBD)
        //                                .AddOutParameter("TotalCount", "Int32");
        //    var dt = query.GetDataTable(out outputs);
        //    collection.TotalCount = UsefullHelper.GetDbValue<int>(outputs["TotalCount"]);
        //    if (collection.TotalCount > 0)
        //    {
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            TotalSingleTreasureInfo info = new TotalSingleTreasureInfo();
        //            info.UserId = Convert.ToString(row["UserId"]);
        //            info.UserName = Convert.ToString(row["UserName"]);
        //            info.SingleTreasureDeclaration = Convert.ToString(row["SingleTreasureDeclaration"]);
        //            info.GameCode = Convert.ToString(row["GameCode"]);
        //            info.GameType = Convert.ToString(row["GameType"]);
        //            info.IssuseNumber = Convert.ToString(row["IssuseNumber"]);
        //            info.ExpectedReturnRate = Convert.ToDecimal(row["ExpectedReturnRate"]);
        //            info.Commission = Convert.ToDecimal(row["Commission"]);
        //            info.Security = (TogetherSchemeSecurity)Convert.ToInt32(row["Security"]);
        //            info.TotalBuyCount = Convert.ToInt32(row["TotalBuyCount"]);
        //            info.TotalBuyMoney = Convert.ToDecimal(row["TotalBuyMoney"]);
        //            info.AfterTaxBonusMoney = Convert.ToDecimal(row["AfterTaxBonusMoney"]);
        //            info.FirstMatchStopTime = Convert.ToDateTime(row["FirstMatchStopTime"]);
        //            info.LastMatchStopTime = Convert.ToDateTime(row["LastMatchStopTime"]);
        //            info.ProfitRate = Convert.ToDecimal(row["ProfitRate"]);
        //            info.SchemeId = Convert.ToString(row["SchemeId"]);
        //            info.TotalBonusMoney = Convert.ToDecimal(row["TotalBonusMoney"]);
        //            info.ExpectedBonusMoney = Convert.ToDecimal(row["ExpectedBonusMoney"]);
        //            info.BetCount = Convert.ToInt32(row["BetCount"]);
        //            info.TotalMatchCount = Convert.ToInt32(row["TotalMatchCount"]);
        //            info.IsComplate = Convert.ToBoolean(row["IsComplate"]);
        //            info.CurrentBetMoney = Convert.ToDecimal(row["CurrentBetMoney"]);
        //            info.ProfitRate = Convert.ToDecimal(row["CDProfitRate"]);
        //            info.LastweekProfitRate = row["LastweekProfitRate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["LastweekProfitRate"]);
        //            info.BDFXCreateTime = Convert.ToDateTime(row["BDFXCreateTime"]);
        //            info.CurrProfitRate = row["CurrProfitRate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurrProfitRate"]);
        //            collection.TotalSingleTreasureList.Add(info);
        //        }
        //        var arrSchemeId = from o in collection.TotalSingleTreasureList select o.SchemeId;
        //        var anteCodeList = this.QueryAnteCodeList(arrSchemeId.ToArray());
        //        collection.AnteCodeList.AddRange(anteCodeList);

        //    }
        //    return collection;
        //}
        public List<NearTimeProfitRateInfo> QueryNearTimeProfitRate(string bdfxUserId)
        {
            //查询近段时间盈利率
            List<NearTimeProfitRateInfo> ListInfo = new List<NearTimeProfitRateInfo>();
            var endTime = DateTime.Now.Date.AddDays(1);
            var startTime = endTime.AddDays(-7);
            //20151008
            //var strSql = "select tab.rowNumber,isnull(tab.currDay,'')CurrDay,isnull(tab.CurrProfitRate,0)CurrProfitRate from(select ROW_NUMBER() over(order by CONVERT(varchar(10),CreateTime,120)) rowNumber, CONVERT(varchar(10),CreateTime,120) currDay,(case t.IsBonus when 0 then 0 when 1 then((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) else 0 end) CurrProfitRate from C_TotalSingleTreasure t where CONVERT(varchar(10),CreateTime,120)>=:StartTime and CONVERT(varchar(10),CreateTime,120) <:EndTime and UserId=:BDFXUserId group by CONVERT(varchar(10),CreateTime,120) ,t.UserId,t.IsBonus) tab";
            var strSql = "select tab.rowNumber,isnull(tab.currDay,'')CurrDay,isnull(tab.CurrProfitRate,0)CurrProfitRate from ( select ROW_NUMBER() over(order by CONVERT(varchar(10),CreateTime,120)) rowNumber, CONVERT(varchar(10),CreateTime,120) currDay, (SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney) CurrProfitRate  from C_TotalSingleTreasure t where CONVERT(varchar(10),CreateTime,120)>=:StartTime and CONVERT(varchar(10),CreateTime,120) <:EndTime and UserId=:BDFXUserId and IsBonus=1 group by CONVERT(varchar(10),CreateTime,120) ,t.UserId ) tab";
            var query = DB.CreateSQLQuery(strSql)
                           .SetString("StartTime", startTime.ToString("yyyy-MM-dd"))
                           .SetString("EndTime", endTime.ToString("yyyy-MM-dd"))
                           .SetString("BDFXUserId", bdfxUserId)
                           .List<object>();
            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    NearTimeProfitRateInfo nInfo = new NearTimeProfitRateInfo();
                    nInfo.RowNumber = Convert.ToInt32(array[0]);
                    nInfo.CurrDate = Convert.ToString(array[1]);
                    nInfo.CurrProfitRate = Convert.ToDecimal(array[2]);
                    ListInfo.Add(nInfo);
                }
            }
            if (ListInfo == null || ListInfo.Count <= 0)
            {
                for (int i = 1; i <= 7; i++)
                {
                    NearTimeProfitRateInfo nInfo = new NearTimeProfitRateInfo();
                    nInfo.RowNumber = i;
                    nInfo.CurrDate = string.Empty;
                    nInfo.CurrProfitRate = 0;
                    ListInfo.Add(nInfo);
                }
            }
            //else if (ListInfo != null && ListInfo.Count < 7)
            //{
            //    var newListInfo = ListInfo.OrderByDescending(s => Convert.ToDateTime(s.CurrDate));
            //    for (int i = 0; i < ListInfo.Count; i++)
            //    {
            //        var currInfo=
            //    }
            //}
            return ListInfo;
        }
        public int QueryRankNumber(string bdfxUserId)
        {
            //查询上周排行,根据当前时间，计算出上个星期的时间段

            //计算上周时间
            var currTime = DateTime.Now;
            int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
            if (currTime.DayOfWeek != 0)
                currTime = currTime.AddDays(-day);
            else
                currTime = currTime.AddDays(-6);
            var sTime = currTime.AddDays(-7).Date;
            var eTime = currTime.Date;
            //var strSql = "select tt.rownumber from (select ROW_NUMBER() over(order by sum(ProfitRate) desc) rownumber,UserId from C_TotalSingleTreasure t where CreateTime>=:StartTime and CreateTime<:EndTime group by UserId)tt where UserId=:BDFXUserId";

            var strSql = "select tt.LastweekRank from (select ROW_NUMBER() over(order by sum(CurrProfitRate) desc) LastweekRank,lastTab.UserId from (select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,UserId from C_TotalSingleTreasure t where CreateTime>=:StartTime and CreateTime<:EndTime and t.IsBonus=1 group by UserId	)	lastTab group by UserId		)tt where tt.UserId=:BDFXUserId";
            var query = DB.CreateSQLQuery(strSql)
                          .SetString("StartTime", sTime.ToString("yyyy-MM-dd"))
                          .SetString("EndTime", eTime.ToString("yyyy-MM-dd"))
                          .SetString("BDFXUserId", bdfxUserId)
                          .List<object>();
            if (query != null && query.Count > 0)
                return Convert.ToInt32(query[0]);
            return 0;
        }
        public ConcernedInfo QueryConcernedByUserId(string bdfxUserId, string currUserId, DateTime startTime, DateTime endTime)
        {
           
            ConcernedInfo info = new ConcernedInfo();
            //查询关注信息
            string strSql = "select u.UserId,u.DisplayName,isnull(att.BeConcernedUserCount,0)BeConcernedUserCount,isnull(att.ConcernedUserCount,0)ConcernedUserCount,isnull(att.SingleTreasureCount,0)SingleTreasureCount from C_User_Register u left join C_SingleTreasure_AttentionSummary att on u.UserId=att.UserId  where u.UserId=:BDFXUserId";
            var query = DB.CreateSQLQuery(strSql)
                             .SetString("BDFXUserId", bdfxUserId).List<object>();
            if (query != null && query.Count > 0)
            {
                var array = query[0] as object[];
                info.UserId = Convert.ToString(array[0]);
                info.UserName = Convert.ToString(array[1]);
                info.BeConcernedUserCount = Convert.ToInt32(array[2]);
                info.ConcernedUserCount = Convert.ToInt32(array[3]);
                info.SingleTreasureCount = Convert.ToInt32(array[4]);
            }
            //查询是否已关注
            strSql = "select count(1) GZ from C_SingleTreasure_Attention at where at.BeConcernedUserId=:BDFXUserId and at.ConcernedUserId=:CurrUserId";
            query =DB.CreateSQLQuery(strSql)
                         .SetString("BDFXUserId", bdfxUserId)
                         .SetString("CurrUserId", currUserId)
                         .List<object>();
            if (query != null && query.Count > 0)
                info.IsGZ = Convert.ToInt32(query[0]) > 0;

            #region 暂时屏蔽
            ////查询近段时间盈利率
            //var endTime = DateTime.Now.Date;
            //var startTime = endTime.AddDays(-7);
            //strSql = "select tab.rowNumber,isnull(tab.currDay,'')CurrDay,isnull(tab.ProfitRate,0)ProfitRate from(select ROW_NUMBER() over(order by CONVERT(varchar(10),CreateTime,120)) rowNumber, CONVERT(varchar(10),CreateTime,120) currDay,t.ProfitRate from C_TotalSingleTreasure t where CONVERT(varchar(10),CreateTime,120)>=:StartTime and CONVERT(varchar(10),CreateTime,120) <:EndTime and UserId=:BDFXUserId group by CONVERT(varchar(10),CreateTime,120),t.ProfitRate ) tab";
            //query = Session.CreateSQLQuery(strSql)
            //             .SetDateTime("StartTime", startTime)
            //             .SetDateTime("EndTime", endTime)
            //             .SetString("BDFXUserId", bdfxUserId)
            //             .List();
            //if (query != null && query.Count > 0)
            //{
            //    foreach (var item in query)
            //    {
            //        var array = item as object[];
            //        NearTimeProfitRateInfo nInfo = new NearTimeProfitRateInfo();
            //        nInfo.RowNumber = Convert.ToInt32(array[0]);
            //        nInfo.CurrDate = Convert.ToString(array[1]);
            //        nInfo.ProfitRate = Convert.ToDecimal(array[2]);
            //        info.NearTimeProfitRateCollection.NearTimeProfitRateList.Add(nInfo);
            //    }
            //}

            ////查询上周排行,根据当前时间，计算出上个星期的时间段
            //var currTime = DateTime.Now;
            //int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
            //if (currTime.DayOfWeek != 0)
            //    currTime = currTime.AddDays(-day);
            //else
            //    currTime = currTime.AddDays(-6);
            //startTime = currTime.AddDays(-7).Date;
            //endTime = currTime.Date;
            //strSql = "select tt.rownumber from (select ROW_NUMBER() over(order by sum(ProfitRate) desc) rownumber,UserId from C_TotalSingleTreasure t where CreateTime>=:StartTime and CreateTime<:EndTime group by UserId)tt where UserId=:BDFXUserId";
            //query = Session.CreateSQLQuery(strSql)
            //             .SetDateTime("StartTime", startTime)
            //             .SetDateTime("EndTime", endTime)
            //             .SetString("BDFXUserId", bdfxUserId)
            //             .List();
            //if (query != null && query.Count > 0)
            //    info.RankNumber = Convert.ToInt32(query[0]); 
            #endregion

            //查询近段时间盈利率
            var nInfo = QueryNearTimeProfitRate(bdfxUserId);
            info.NearTimeProfitRateCollection = new NearTimeProfitRate_Collection();
            info.NearTimeProfitRateCollection.NearTimeProfitRateList.AddRange(nInfo);
            //查询上周排行,根据当前时间，计算出上个星期的时间段
            info.RankNumber = QueryRankNumber(bdfxUserId);
            return info;
        }
        public BDFXOrderDetailInfo QueryBDFXOrderDetailBySchemeId(string schemeId)
        {
           
            var orderRunning = DB.CreateQuery<C_Sports_Order_Running>().Where(s => s.SchemeId == schemeId).FirstOrDefault();
            if (orderRunning != null && !string.IsNullOrEmpty(orderRunning.SchemeId))
            {
                var query = from t in DB.CreateQuery<C_TotalSingleTreasure>()
                            join o in DB.CreateQuery<C_Sports_Order_Running>() on t.SchemeId equals o.SchemeId
                            join u in DB.CreateQuery<C_User_Register>() on t.UserId equals u.UserId
                            where t.SchemeId == schemeId
                            select new BDFXOrderDetailInfo
                            {
                                UserId = u.UserId,
                                UserName = u.DisplayName,
                                SingleTreasureDeclaration = t.SingleTreasureDeclaration,
                                TotalBuyCount = t.TotalBuyCount,
                                TotalBuyMoney = t.TotalBuyMoney,
                                AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                                TotalBonusMoney = t.TotalBonusMoney,
                                ProfitRate = t.ProfitRate,
                                GameCode = o.GameCode,
                                GameType = o.GameType,
                                IssuseNumber = o.IssuseNumber,
                                ExpectedReturnRate = t.ExpectedReturnRate,
                                Security = (TogetherSchemeSecurity)t.Security,
                                SchemeId = t.SchemeId,
                                Amount = o.Amount,
                                BetCount = o.BetCount,
                                FirstMatchStopTime = t.FirstMatchStopTime,
                                LastMatchStopTime = t.LastMatchStopTime,
                                PlayType = o.PlayType,
                                TotalMatchCount = o.TotalMatchCount,
                                SchemeBettingCategory = (SchemeBettingCategory)o.SchemeBettingCategory,
                                ExpectedBonusMoney = t.ExpectedBonusMoney,
                                IsComplate = t.IsComplate,
                                CurrentBetMoney = t.CurrentBetMoney,
                                CurrProfitRate = t.CurrentBetMoney > 0 ? (o.AfterTaxBonusMoney - t.CurrentBetMoney) / t.CurrentBetMoney : 0,
                                Commission = t.Commission,
                                TicketStatus = (TicketStatus)o.TicketStatus,
                            };
                if (query != null && query.Count() > 0)
                    return query.FirstOrDefault();
            }
            else
            {
                var query = from t in DB.CreateQuery<C_TotalSingleTreasure>()
                            join o in DB.CreateQuery<C_Sports_Order_Running>() on t.SchemeId equals o.SchemeId
                            join u in DB.CreateQuery<C_User_Register>() on t.UserId equals u.UserId
                            where t.SchemeId == schemeId
                            select new BDFXOrderDetailInfo
                            {
                                UserId = u.UserId,
                                UserName = u.DisplayName,
                                SingleTreasureDeclaration = t.SingleTreasureDeclaration,
                                TotalBuyCount = t.TotalBuyCount,
                                TotalBuyMoney = t.TotalBuyMoney,
                                AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                                TotalBonusMoney = t.TotalBonusMoney,
                                ProfitRate = t.ProfitRate,
                                GameCode = o.GameCode,
                                GameType = o.GameType,
                                IssuseNumber = o.IssuseNumber,
                                ExpectedReturnRate = t.ExpectedReturnRate,
                                Security = (TogetherSchemeSecurity)t.Security,
                                SchemeId = t.SchemeId,
                                Amount = o.Amount,
                                BetCount = o.BetCount,
                                FirstMatchStopTime = t.FirstMatchStopTime,
                                LastMatchStopTime = t.LastMatchStopTime,
                                PlayType = o.PlayType,
                                TotalMatchCount = o.TotalMatchCount,
                                SchemeBettingCategory = (SchemeBettingCategory)o.SchemeBettingCategory,
                                ExpectedBonusMoney = t.ExpectedBonusMoney,
                                IsComplate = t.IsComplate,
                                CurrentBetMoney = t.CurrentBetMoney,
                                CurrProfitRate = t.CurrentBetMoney > 0 ? (o.AfterTaxBonusMoney - t.CurrentBetMoney) / t.CurrentBetMoney : 0,
                                Commission = t.Commission,
                                TicketStatus = (TicketStatus)o.TicketStatus,
                            };
                if (query != null && query.Count() > 0)
                    return query.FirstOrDefault();
            }

            return new BDFXOrderDetailInfo();
        }
        //public BDFXGSRank_Collection QueryGSRankList(DateTime startTime, DateTime endTime, string currUserId, string isMyGZ)
        //{
        //    Session.Clear();
        //    //计算上周时间
        //    //var currTime = DateTime.Now;
        //    //int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
        //    //if (currTime.DayOfWeek != 0)
        //    //    currTime = currTime.AddDays(-day);
        //    //else
        //    //    currTime = currTime.AddDays(-6);
        //    var sTime = startTime.AddDays(-7).Date;
        //    var eTime = startTime.Date;
        //    BDFXGSRank_Collection collection = new BDFXGSRank_Collection();
        //    var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryBDFXGSRank"))
        //                                .AddInParameter("StartTime", startTime)
        //                                .AddInParameter("EndTime", endTime)
        //                                .AddInParameter("CurrUserId", currUserId)
        //                                .AddInParameter("IsMyGZ", isMyGZ)
        //                                .AddInParameter("LastweekStartTime", sTime)
        //                                .AddInParameter("LastweekEndTime", eTime);
        //    var dt = query.GetDataTable();
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow Row in dt.Rows)
        //        {
        //            BDFXGSRankInfo info = new BDFXGSRankInfo();

        //            info.BeConcernedUserCount = Convert.ToInt32(Row["BeConcernedUserCount"]);
        //            info.IsGZ = Row["IsGZ"] == string.Empty ? false : Convert.ToBoolean(Row["IsGZ"]);
        //            info.CurrProfitRate = Convert.ToDecimal(Row["CurrProfitRate"]);
        //            info.RankNumber = Convert.ToInt32(Row["RankNumber"]);
        //            info.SchemeId = Convert.ToString(Row["SchemeId"]);
        //            info.SingleTreasureCount = Convert.ToInt32(Row["SingleTreasureCount"]);
        //            info.UserId = Convert.ToString(Row["UserId"]);
        //            info.UserName = Convert.ToString(Row["UserName"]);
        //            info.LastweekRank = Row["LastweekRank"] == DBNull.Value ? 0 : Convert.ToInt32(Row["LastweekRank"]);

        //            collection.RankList.Add(info);
        //        }
        //    }
        //    return collection;
        //}
        //public TotalSingleTreasure_Collection QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize)
        //{
        //    Session.Clear();
        //    TotalSingleTreasure_Collection collection = new TotalSingleTreasure_Collection();
        //    collection.TotalCount = 0;
        //    Dictionary<string, object> outputs;
        //    var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryBDFXAutherHomePage"))
        //                                .AddInParameter("UserId", userId)
        //                                .AddInParameter("StrIsBonus", strIsBonus)
        //                                .AddInParameter("CurrentTime", currentTime)
        //                                .AddInParameter("PageIndex", pageIndex)
        //                                .AddInParameter("PageSize", pageSize)
        //                                .AddOutParameter("TotalCount", "Int32")
        //                                .AddOutParameter("AllTotalBuyCount", "Int32")
        //                                .AddOutParameter("AllTotalBonusMoney", "Int32");
        //    var dt = query.GetDataTable(out outputs);
        //    collection.TotalCount = UsefullHelper.GetDbValue<int>(outputs["TotalCount"]);
        //    collection.AllTotalBuyCount = UsefullHelper.GetDbValue<int>(outputs["AllTotalBuyCount"]);
        //    collection.AllTotalBonusMoney = UsefullHelper.GetDbValue<int>(outputs["AllTotalBonusMoney"]);
        //    if (collection.TotalCount > 0)
        //    {
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            TotalSingleTreasureInfo info = new TotalSingleTreasureInfo();
        //            info.UserId = Convert.ToString(row["UserId"]);
        //            info.UserName = Convert.ToString(row["UserName"]);
        //            info.SingleTreasureDeclaration = Convert.ToString(row["SingleTreasureDeclaration"]);
        //            info.GameCode = Convert.ToString(row["GameCode"]);
        //            info.GameType = Convert.ToString(row["GameType"]);
        //            info.IssuseNumber = Convert.ToString(row["IssuseNumber"]);
        //            info.ExpectedReturnRate = Convert.ToDecimal(row["ExpectedReturnRate"]);
        //            info.Commission = Convert.ToDecimal(row["Commission"]);
        //            info.Security = (TogetherSchemeSecurity)Convert.ToInt32(row["Security"]);
        //            info.TotalBuyCount = Convert.ToInt32(row["TotalBuyCount"]);
        //            info.TotalBuyMoney = Convert.ToDecimal(row["TotalBuyMoney"]);
        //            info.AfterTaxBonusMoney = Convert.ToDecimal(row["AfterTaxBonusMoney"]);
        //            info.FirstMatchStopTime = Convert.ToDateTime(row["FirstMatchStopTime"]);
        //            info.LastMatchStopTime = Convert.ToDateTime(row["LastMatchStopTime"]);
        //            info.ProfitRate = Convert.ToDecimal(row["ProfitRate"]);
        //            info.SchemeId = Convert.ToString(row["SchemeId"]);
        //            info.TotalBonusMoney = Convert.ToDecimal(row["TotalBonusMoney"]);
        //            info.ExpectedBonusMoney = Convert.ToDecimal(row["ExpectedBonusMoney"]);
        //            info.BetCount = Convert.ToInt32(row["BetCount"]);
        //            info.TotalMatchCount = Convert.ToInt32(row["TotalMatchCount"]);
        //            info.IsComplate = Convert.ToBoolean(row["IsComplate"]);
        //            info.CurrentBetMoney = Convert.ToDecimal(row["CurrentBetMoney"]);
        //            info.CurrProfitRate = row["CurrProfitRate"] == DBNull.Value ? 0M : Convert.ToDecimal(row["CurrProfitRate"]);
        //            collection.TotalSingleTreasureList.Add(info);
        //        }
        //        var arrSchemeId = from o in collection.TotalSingleTreasureList select o.SchemeId;
        //        var anteCodeList = this.QueryAnteCodeList(arrSchemeId.ToArray());
        //        collection.AnteCodeList.AddRange(anteCodeList);
        //    }
        //    return collection;
        //}
        //public BDFXCommisionInfo QueryBDFXCommision(string schemeId)
        //{
        //    Session.Clear();
        //    var query = from t in Session.Query<TotalSingleTreasure>()
        //                join u in Session.Query<UserRegister>() on t.UserId equals u.UserId
        //                join r in Session.Query<BDFXRecordSingleCopy>() on t.SchemeId equals r.BDXFSchemeId
        //                where r.SingleCopySchemeId == schemeId
        //                select new BDFXCommisionInfo
        //                {
        //                    UserId = u.UserId,
        //                    UserName = u.DisplayName,
        //                    Commission = t.Commission,
        //                };
        //    if (query != null)
        //        return query.FirstOrDefault();
        //    return new BDFXCommisionInfo();
        //}
        //public string QueryYesterdayNR(DateTime startTime, DateTime endTime, int count)
        //{
        //    Session.Clear();
        //    startTime = startTime.Date.AddDays(-1);
        //    endTime = endTime.Date;
        //    string strSql = "select top " + count + " t.UserId,t.DisplayName from(select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,u.UserId,u.DisplayName from C_TotalSingleTreasure t inner join C_User_Register u on t.UserId=u.UserId where  t.CreateTime>=:StartTime and t.CreateTime<:EndTime and t.IsBonus=1 group by u.UserId,u.DisplayName	)t where  t.CurrProfitRate>=0 order by t.CurrProfitRate desc";
        //    var query = Session.CreateSQLQuery(strSql)
        //        .SetDateTime("StartTime", startTime)
        //        .SetDateTime("EndTime", endTime)
        //        .List();
        //    string str = string.Empty;
        //    if (query != null && query.Count > 0)
        //    {
        //        foreach (var item in query)
        //        {
        //            var array = item as object[];
        //            str += array[0] + "|" + array[1] + "%";
        //            //str = array[0] + "|" + array[1];
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(str))
        //        str = str.TrimEnd('%');
        //    return str;
        //}
        //public BDFXNRRankList_Collection QueryNRRankList(DateTime startTime, DateTime endTime, int count)
        //{
        //    Session.Clear();
        //    BDFXNRRankList_Collection collection = new BDFXNRRankList_Collection();
        //    startTime = startTime.Date;
        //    var sTime = DateTime.Now.Date.AddDays(-1);
        //    var eTime = sTime.AddDays(1).Date;
        //    endTime = endTime.AddDays(1).Date;
        //    string strSql = "select nrtable.UserId,nrtable.DisplayName,isnull(nrtable.CurrProfitRate,0)CurrProfitRate,isnull(nrph.rankNumber,0)rankNumber  from  ( select top " + count + " t.UserId,t.DisplayName,CurrProfitRate from(select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,u.UserId,u.DisplayName from C_TotalSingleTreasure t inner join C_User_Register u on t.UserId=u.UserId where  t.IsBonus=1 and t.CreateTime>=:sTime and t.CreateTime<:eTime group by u.UserId,u.DisplayName	)t where  t.CurrProfitRate>=0 order by t.CurrProfitRate desc ) nrtable left join  ( select ROW_NUMBER() over(order by tabl.CurrProfitRate desc) rankNumber,tabl.UserId from  ( select (case SUM(CurrentBetMoney) when 0 then 0 else ((SUM(CurrBonusMoney)-SUM(CurrentBetMoney))/SUM(CurrentBetMoney)) end) CurrProfitRate,UserId from C_TotalSingleTreasure where CreateTime>=:StartTime and CreateTime<:EndTime and IsBonus=1   group by UserId ) tabl ) nrph on nrtable.UserId=nrph.UserId";
        //    var query = Session.CreateSQLQuery(strSql)
        //        .SetDateTime("sTime", sTime)
        //        .SetDateTime("eTime", eTime)
        //       .SetDateTime("StartTime", startTime)
        //       .SetDateTime("EndTime", endTime)
        //       .List();
        //    if (query != null && query.Count > 0)
        //    {
        //        collection.TotalCount = query.Count;
        //        foreach (var item in query)
        //        {
        //            var array = item as object[];
        //            BDFXNRRankListInfo info = new BDFXNRRankListInfo();
        //            info.UserId = Convert.ToString(array[0]);
        //            info.UserName = Convert.ToString(array[1]);
        //            info.CurrProfitRate = Convert.ToDecimal(array[2]);
        //            info.RankNumber = Convert.ToInt32(array[3]);
        //            collection.RanList.Add(info);
        //        }
        //    }
        //    return collection;
        //}


        //public WebUserSchemeShareExpert_Collection QueryWebUserSchemeShareExpertList(int queryCount, int expertType)
        //{
        //    Session.Clear();
        //    WebUserSchemeShareExpert_Collection collection = new WebUserSchemeShareExpert_Collection();
        //    var strSql = "select r.UserId,r.DisplayName,r.HideDisplayNameCount,br.TotalProfit,br.TotalFansCount from E_User_SchemeShareExpert se left join E_BDFX_ReportStatisticsData br on se.UserId=br.UserId inner join C_User_Register r on se.UserId=r.UserId where r.IsEnable=1 and se.ExpertType=:ExpertType order by se.ShowSort asc,br.TotalProfit desc";
        //    var result = Session.CreateSQLQuery(strSql)
        //                        .SetInt32("ExpertType", expertType)
        //                        .List();
        //    if (result != null && result.Count > 0)
        //    {
        //        foreach (var item in result)
        //        {
        //            var arr = item as object[];
        //            WebUserSchemeShareExpertInfo info = new WebUserSchemeShareExpertInfo();
        //            info.UserId = arr[0] == null ? string.Empty : arr[0].ToString();
        //            info.UserName = arr[1] == null ? string.Empty : arr[1].ToString();
        //            info.HideDisplayNameCount = arr[2] == null ? 0 : Convert.ToInt32(arr[2]);
        //            info.TotalProfit = arr[3] == null ? 0 : Convert.ToDecimal(arr[3]);
        //            info.TotalFansCount = arr[4] == null ? 0 : Convert.ToInt32(arr[4]);
        //            collection.UserSchemeShareExpertList.Add(info);
        //        }
        //        collection.TotalCount = queryCount;
        //        collection.UserSchemeShareExpertList = collection.UserSchemeShareExpertList.Take(queryCount).ToList(); ;
        //    }
        //    return collection;
        //}

        public void AddUserSchemeShareExpert(E_User_SchemeShareExpert entity)
        {
            DB.GetDal<E_User_SchemeShareExpert>().Add(entity);
        }
        public void UpdateUserSchemeShareExpert(E_User_SchemeShareExpert entity)
        {
            DB.GetDal<E_User_SchemeShareExpert>().Update(entity);
        }
        public E_User_SchemeShareExpert QueryUserSchemeShareExpertByUserId(string userId, CopyOrderSource source)
        {
            return DB.CreateQuery<E_User_SchemeShareExpert>().Where(s => s.UserId == userId && s.ExpertType == (int)source).FirstOrDefault();
        }
        public UserSchemeShareExpert_Collection QueryUserSchemeShareExpertList(string userKey, int source, int pageIndex, int pageSize)
        {
            UserSchemeShareExpert_Collection collection = new UserSchemeShareExpert_Collection();
            var query = from s in DB.CreateQuery<E_User_SchemeShareExpert>()
                        join u in DB.CreateQuery<C_User_Register>() on s.UserId equals u.UserId
                        where (string.IsNullOrEmpty(userKey) || u.UserId == userKey || u.DisplayName == userKey)
                        && (source == -1 || s.ExpertType == source)
                        orderby s.CreateTime descending
                        select new UserSchemeShareExpertInfo
                        {
                            CreateTime = s.CreateTime,
                            ExpertType = (CopyOrderSource)s.ExpertType,
                            Id = s.Id,
                            IsEnable = s.IsEnable,
                            ShowSort = s.ShowSort,
                            UserId = s.UserId,
                            UserName = u.DisplayName,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                collection.SchemeShareExpertList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }
        public void DeleteUserSchemeShareExpert(string id)
        {
            var strSql = "delete from E_User_SchemeShareExpert where Id=@id";
            DB.CreateSQLQuery(strSql)
                   .SetString("@id", id);
        }

        public BDFXCommisionInfo QueryBDFXCommision(string schemeId)
        {
          
            var query = from t in DB.CreateQuery<C_TotalSingleTreasure>()
                        join u in DB.CreateQuery<C_User_Register>() on t.UserId equals u.UserId
                        join r in DB.CreateQuery<C_BDFX_RecordSingleCopy>() on t.SchemeId equals r.BDXFSchemeId
                        where r.SingleCopySchemeId == schemeId
                        select new BDFXCommisionInfo
                        {
                            UserId = u.UserId,
                            UserName = u.DisplayName,
                            Commission = t.Commission,
                        };
            if (query != null)
                return query.FirstOrDefault();
            return new BDFXCommisionInfo();
        }

        public string QueryYesterdayNR(DateTime startTime, DateTime endTime, int count)
        {
         
            startTime = startTime.Date.AddDays(-1);
            endTime = endTime.Date;
            string strSql = "select top " + count + " t.UserId,t.DisplayName from(select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,u.UserId,u.DisplayName from C_TotalSingleTreasure t inner join C_User_Register u on t.UserId=u.UserId where  t.CreateTime>=:StartTime and t.CreateTime<:EndTime and t.IsBonus=1 group by u.UserId,u.DisplayName	)t where  t.CurrProfitRate>=0 order by t.CurrProfitRate desc";
            var query = DB.CreateSQLQuery(strSql)
                .SetString("StartTime", startTime.ToString("yyyy-MM-dd"))
                .SetString("EndTime", endTime.ToString("yyyy-MM-dd"))
                .List<object>();
            string str = string.Empty;
            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    str += array[0] + "|" + array[1] + "%";
                    //str = array[0] + "|" + array[1];
                }
            }
            if (!string.IsNullOrEmpty(str))
                str = str.TrimEnd('%');
            return str;
        }

        public BDFXNRRankList_Collection QueryNRRankList(DateTime startTime, DateTime endTime, int count)
        {
          
            BDFXNRRankList_Collection collection = new BDFXNRRankList_Collection();
            startTime = startTime.Date;
            var sTime = DateTime.Now.Date.AddDays(-1);
            var eTime = sTime.AddDays(1).Date;
            endTime = endTime.AddDays(1).Date;
            string strSql = "select nrtable.UserId,nrtable.DisplayName,isnull(nrtable.CurrProfitRate,0)CurrProfitRate,isnull(nrph.rankNumber,0)rankNumber  from  ( select top " + count + " t.UserId,t.DisplayName,CurrProfitRate from(select (case SUM(t.CurrentBetMoney) when 0 then 0 else ((SUM(t.CurrBonusMoney)-SUM(t.CurrentBetMoney))/SUM(t.CurrentBetMoney)) end) CurrProfitRate,u.UserId,u.DisplayName from C_TotalSingleTreasure t inner join C_User_Register u on t.UserId=u.UserId where  t.IsBonus=1 and t.CreateTime>=:sTime and t.CreateTime<:eTime group by u.UserId,u.DisplayName	)t where  t.CurrProfitRate>=0 order by t.CurrProfitRate desc ) nrtable left join  ( select ROW_NUMBER() over(order by tabl.CurrProfitRate desc) rankNumber,tabl.UserId from  ( select (case SUM(CurrentBetMoney) when 0 then 0 else ((SUM(CurrBonusMoney)-SUM(CurrentBetMoney))/SUM(CurrentBetMoney)) end) CurrProfitRate,UserId from C_TotalSingleTreasure where CreateTime>=:StartTime and CreateTime<:EndTime and IsBonus=1   group by UserId ) tabl ) nrph on nrtable.UserId=nrph.UserId";
            var query = DB.CreateSQLQuery(strSql)
                .SetString("sTime", sTime.ToString("yyyy-MM-dd"))
                .SetString("eTime", eTime.ToString("yyyy-MM-dd"))
               .SetString("StartTime", startTime.ToString("yyyy-MM-dd"))
               .SetString("EndTime", endTime.ToString("yyyy-MM-dd"))
                 .List<object>();
            if (query != null && query.Count > 0)
            {
                collection.TotalCount = query.Count;
                foreach (var item in query)
                {
                    var array = item as object[];
                    BDFXNRRankListInfo info = new BDFXNRRankListInfo();
                    info.UserId = Convert.ToString(array[0]);
                    info.UserName = Convert.ToString(array[1]);
                    info.CurrProfitRate = Convert.ToDecimal(array[2]);
                    info.RankNumber = Convert.ToInt32(array[3]);
                    collection.RanList.Add(info);
                }
            }
            return collection;
        }
        
        
    }
}
