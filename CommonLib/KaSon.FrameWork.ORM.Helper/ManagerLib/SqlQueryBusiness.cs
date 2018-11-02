
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.RequestModel;
using System;
using System.Collections.Generic;
using System.Text;
using static EntityModel.CoreModel.ReportInfo;

namespace KaSon.FrameWork.ORM.Helper
{
   public class SqlQueryBusiness:DBbase
    {
        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {
            var manager = new SqlQueryManager();
            
             return manager.QueryYqidRegisterByAgentId(AgentId);
            
        }

        public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        {
           var manager = new SqlQueryManager();
            
          return manager.QueryOrderDetailBySchemeId(schemeId);
            
        }

        public int QueryTogetherFollowerCount(string createUserId)
        {
             var manager = new SqlQueryManager();
            
             return manager.QueryTogetherFollowerCount(createUserId);
            
        }

        #region 过关统计

        /// <summary>
        /// 查询过关统计
        /// </summary>
        public SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool? isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new SportsOrder_GuoGuanInfoCollection();
            var totalCount = 0;
            result.ReportItemList.AddRange(new SqlQueryManager().QueryReportInfoList_GuoGuan(isVirtualOrder, category, key, gameCode, gameType, issuseNumber, startTime, endTime, pageIndex, pageSize));
            result.TotalCount = totalCount;
            return result;
        }

        #endregion

        public OrderSingleSchemeCollection QuerySingSchemeDetail(string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var sqlQueryManager = new SqlQueryManager();
            var result = new OrderSingleSchemeCollection();
            var singCodeList = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            if (singCodeList == null || string.IsNullOrEmpty(singCodeList.SelectMatchId))
                throw new Exception("未查询到投注内容");
            var arrayMatchId = singCodeList.SelectMatchId.Split(',');
            foreach (var item in arrayMatchId)
            {

                var anteCode = sportsManager.QueryAnteCode(singCodeList.SchemeId, item, singCodeList.GameType);
                if (singCodeList.GameCode.ToUpper() == "BJDC")
                {
                    var match = sportsManager.QueryBJDC_Match(string.Format("{0}|{1}", singCodeList.IssuseNumber, item));
                    var matchResult = sportsManager.QueryBJDC_MatchResult(string.Format("{0}|{1}", singCodeList.IssuseNumber, item));
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        halfResult = string.Format("{0}:{1}", matchResult.HomeHalf_Result, matchResult.GuestHalf_Result);
                        fullResult = string.Format("{0}:{1}", matchResult.HomeFull_Result, matchResult.GuestFull_Result);
                        matchState = matchResult.MatchState;
                        switch (singCodeList.GameType)
                        {
                            case "SPF":
                                caiguo = matchResult.SPF_Result;
                                matchResultSp = matchResult.SPF_SP;
                                break;
                            case "ZJQ":
                                caiguo = matchResult.ZJQ_Result;
                                matchResultSp = matchResult.ZJQ_SP;
                                break;
                            case "SXDS":
                                caiguo = matchResult.SXDS_Result;
                                matchResultSp = matchResult.SXDS_SP;
                                break;
                            case "BF":
                                caiguo = matchResult.BF_Result;
                                matchResultSp = matchResult.BF_SP;
                                break;
                            case "BQC":
                                caiguo = matchResult.BQC_Result;
                                matchResultSp = matchResult.BQC_SP;
                                break;
                        }
                    }
                    result.AnteCodeList.Add(new OrderSingleScheme
                    {
                        IssuseNumber = match.IssuseNumber,
                        LeagueId = string.Empty,
                        LeagueName = match.MatchName,
                        LeagueColor = match.MatchColor,
                        MatchId = match.MatchOrderId.ToString(),
                        MatchIdName = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.GuestTeamName,
                        IsDan = anteCode == null ? false : anteCode.IsDan,
                        StartTime = match.MatchStartTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = anteCode == null ? "0" : anteCode.Odds,
                        LetBall = match.LetBall,
                        BonusStatus = anteCode == null ?BonusStatus.Waitting : (BonusStatus)anteCode.BonusStatus,
                        GameType = singCodeList.GameType,
                        MatchState = matchState,
                        FileBuffer = singCodeList.FileBuffer,
                        PlayType = singCodeList.PlayType,
                        ContainsMatchId = singCodeList.ContainsMatchId,
                    });
                    continue;
                }
                if (singCodeList.GameCode.ToUpper() == "JCZQ")
                {
                    var match = sportsManager.QueryJCZQ_Match(item);
                    var matchResult = sportsManager.QueryJCZQ_MatchResult(item);
                    if (match == null)
                        throw new Exception("未查询到比赛数据");
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        halfResult = string.Format("{0}:{1}", matchResult.HalfHomeTeamScore, matchResult.HalfGuestTeamScore);
                        fullResult = string.Format("{0}:{1}", matchResult.FullHomeTeamScore, matchResult.FullGuestTeamScore);
                        matchState = matchResult.MatchState;
                        switch (singCodeList.GameType.ToUpper())
                        {
                            case "SPF":
                                caiguo = matchResult.SPF_Result;
                                matchResultSp = matchResult.SPF_SP;
                                break;
                            case "BRQSPF":
                                caiguo = matchResult.BRQSPF_Result;
                                matchResultSp = matchResult.BRQSPF_SP;
                                break;
                            case "ZJQ":
                                caiguo = matchResult.ZJQ_Result;
                                matchResultSp = matchResult.ZJQ_SP;
                                break;
                            case "BF":
                                caiguo = matchResult.BF_Result;
                                matchResultSp = matchResult.BF_SP;
                                break;
                            case "BQC":
                                caiguo = matchResult.BQC_Result;
                                matchResultSp = matchResult.BQC_SP;
                                break;
                        }
                    }
                    result.AnteCodeList.Add(new OrderSingleScheme
                    {
                        IssuseNumber = string.Empty,
                        LeagueId = match.LeagueId.ToString(),
                        LeagueName = match.LeagueName,
                        LeagueColor = match.LeagueColor,
                        MatchId = match.MatchId,
                        MatchIdName = match.MatchIdName,
                        HomeTeamId = match.HomeTeamId.ToString(),
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = match.GuestTeamId.ToString(),
                        GuestTeamName = match.GuestTeamName,
                        IsDan = anteCode == null ? false : anteCode.IsDan,
                        StartTime = match.StartDateTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = anteCode == null ? "0" : anteCode.Odds,
                        LetBall = match.LetBall,
                        BonusStatus = anteCode == null ? BonusStatus.Waitting : (BonusStatus)anteCode.BonusStatus,
                        GameType = singCodeList.GameType,
                        MatchState = matchState,
                        FileBuffer = singCodeList.FileBuffer,
                        PlayType = singCodeList.PlayType,
                        ContainsMatchId = singCodeList.ContainsMatchId,
                    });
                    continue;
                }
                if (singCodeList.GameCode.ToUpper() == "JCLQ")
                {
                    var match = sportsManager.QueryJCLQ_Match(item);
                    var matchResult = sportsManager.QueryJCLQ_MatchResult(item);
                    var halfResult = string.Empty;
                    var fullResult = string.Empty;
                    var caiguo = string.Empty;
                    var matchResultSp = 0M;
                    var matchState = string.Empty;
                    if (matchResult != null)
                    {
                        //halfResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestHalf_Result);
                        fullResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestScore);
                        matchState = matchResult.MatchState;
                        switch (singCodeList.GameType.ToUpper())
                        {
                            case "SF":
                                caiguo = matchResult.SF_Result;
                                matchResultSp = matchResult.SF_SP;
                                break;
                            case "RFSF":
                                caiguo = matchResult.RFSF_Result;
                                matchResultSp = matchResult.RFSF_SP;
                                break;
                            case "SFC":
                                caiguo = matchResult.SFC_Result;
                                matchResultSp = matchResult.SFC_SP;
                                break;
                            case "DXF":
                                caiguo = matchResult.DXF_Result;
                                matchResultSp = matchResult.DXF_SP;
                                break;
                        }
                    }
                    result.AnteCodeList.Add(new OrderSingleScheme
                    {
                        IssuseNumber = string.Empty,
                        LeagueId = match.LeagueId.ToString(),
                        LeagueName = match.LeagueName,
                        LeagueColor = match.LeagueColor,
                        MatchId = match.MatchId,
                        MatchIdName = match.MatchIdName,
                        HomeTeamId = string.Empty,
                        HomeTeamName = match.HomeTeamName,
                        GuestTeamId = string.Empty,
                        GuestTeamName = match.GuestTeamName,
                        IsDan = anteCode == null ? false : anteCode.IsDan,
                        StartTime = match.StartDateTime,
                        HalfResult = halfResult,
                        FullResult = fullResult,
                        MatchResult = caiguo,
                        MatchResultSp = matchResultSp,
                        CurrentSp = anteCode == null ? "0" : anteCode.Odds,
                        BonusStatus = anteCode == null ? BonusStatus.Waitting : (BonusStatus)anteCode.BonusStatus,
                        GameType = singCodeList.GameType,
                        MatchState = matchState,
                        FileBuffer = singCodeList.FileBuffer,
                        PlayType = singCodeList.PlayType,
                        ContainsMatchId = singCodeList.ContainsMatchId,
                    });
                    continue;
                }
                if (singCodeList.GameCode.ToUpper() == "CTZQ")
                {
                    result.AnteCodeList.Add(new OrderSingleScheme
                    {
                        IssuseNumber = singCodeList.IssuseNumber,
                        LeagueId = string.Empty,
                        LeagueName = string.Empty,
                        LeagueColor = string.Empty,
                        MatchId = string.Empty,
                        MatchIdName = string.Empty,
                        HomeTeamId = string.Empty,
                        HomeTeamName = string.Empty,
                        GuestTeamId = string.Empty,
                        GuestTeamName = string.Empty,
                        IsDan = anteCode == null ? false : anteCode.IsDan,
                        StartTime = DateTime.Now,
                        HalfResult = string.Empty,
                        FullResult = string.Empty,
                        MatchResult = string.Empty,
                        MatchResultSp = 0M,
                        CurrentSp = anteCode == null ? "0" : anteCode.Odds,
                        BonusStatus = anteCode == null ? BonusStatus.Waitting : (BonusStatus)anteCode.BonusStatus,
                        GameType = singCodeList.GameType,
                        FileBuffer = singCodeList.FileBuffer,
                        PlayType = singCodeList.PlayType,
                        ContainsMatchId = singCodeList.ContainsMatchId,
                    });
                }
            }
            return result;
        }
        public UserFundDetailCollection QueryUserFundDetail(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        {
            QueryUserFundDetailParam param = new QueryUserFundDetailParam()
            {
                fromDate=fromDate,
                userid=userId,
                keyLine=keyLine,
                toDate=toDate,
                accountTypeList = accountTypeList,
                categoryList=categoryList,
                pageIndex=pageIndex,
                pageSize=pageSize
            };
            return new OrderQuery().QueryUserFundDetail(param, userId);
        }
        public BettingOrderInfoCollection QueryBettingOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, bool? isVirtual, string gameCode
           , DateTime startTime, DateTime endTime, int sortType, string agentId, int pageIndex, int pageSize, string fieldName, TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        {

            var collection = new BettingOrderInfoCollection();

            collection = new OrderQuery().QueryBettingOrderList(userIdOrName, schemeType, progressStatus, bonusStatus, betCategory, isVirtual, gameCode, startTime, endTime, sortType, agentId, pageIndex, pageSize , schemeSource);

            return collection;
        }
        public FillMoneyQueryInfoCollection QueryFillMoneyList(string userId, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId = "")
        {
            var collection = new FillMoneyQueryInfoCollection();
            
            collection = new OrderQuery().QueryFillMoneyList(userId, agentTypeList, statusList, sourceList, startTime, endTime
            , pageIndex, pageSize, orderId);
            return collection;
        }

        public UserFundDetailCollection QueryUserFundDetailListReport(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        {
            int totalPayinCount;
            decimal totalPayinMoney;
            int totalPayoutCount;
            decimal totalPayoutMoney;
            var collection = new UserFundDetailCollection();

            collection.FundDetailList = new SqlQueryManager().QueryUserFundDetailListReport(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize
                , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
            collection.TotalPayinCount = totalPayinCount;
            collection.TotalPayinMoney = totalPayinMoney;
            collection.TotalPayoutCount = totalPayoutCount;
            collection.TotalPayoutMoney = totalPayoutMoney;
            //collection.TotalBalanceMoney = new SqlQueryManager().GetAllUserBalanceMoney(userId);
            return collection;
        }

        public UserFundDetailCollection QueryUserFundDetail_Commission(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            int totalPayinCount;
            decimal totalPayinMoney;
            var collection = new UserFundDetailCollection();
            collection.FundDetailList = new SqlQueryManager().QueryUserFundDetail_Commission(userId, fromDate, toDate, pageIndex, pageSize
                , out totalPayinCount, out totalPayinMoney);
            collection.TotalPayinCount = totalPayinCount;
            collection.TotalPayinMoney = totalPayinMoney;
            return collection;
        }
    }
}
