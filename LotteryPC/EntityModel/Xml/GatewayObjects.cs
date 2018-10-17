using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Xml
{

    #region 奖期查询

    public class IssuseQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryissue", 0)]
        public IssuseQueryRequestInnerInfo IssuseQueryInfo { get; set; }
    }

    public class IssuseQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
    }

    public class IssuseQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("issueinfo", 0, MappingType = MappingType.Element)]
        public IssuseQueryResponseItem IssuseQueryResponseItem { get; set; }

        [XmlMapping("issueinfos", 0, MappingType = MappingType.Element)]
        public IssuseQueryResponseList IssuseQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }

        /// <summary>
        ///     获取结果类型。item:单个奖期。list:多个奖期，特指高频。null：未解析到任何数据
        /// </summary>
        public string ResultType
        {
            get
            {
                if (IssuseQueryResponseItem != null && !string.IsNullOrEmpty(IssuseQueryResponseItem.LotteryId))
                    return "item";
                if (IssuseQueryResponseList != null && IssuseQueryResponseList.InnerIssuseQueryResponseItems != null &&
                    IssuseQueryResponseList.InnerIssuseQueryResponseItems.Count > 0)
                    return "list";
                return null;
            }
        }
    }

    public class IssuseQueryResponseList : XmlMappingObject
    {
        [XmlMapping("issueinfo", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<IssuseQueryResponseItem> InnerIssuseQueryResponseItems { get; set; }
    }

    public class IssuseQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }

        [XmlMapping("startTime", 2, MappingType = MappingType.Attribute)]
        public DateTime StartTime { get; set; }

        [XmlMapping("stopTime", 3, MappingType = MappingType.Attribute)]
        public DateTime StopTime { get; set; }

        [XmlMapping("closeTime", 4, MappingType = MappingType.Attribute)]
        public DateTime CloseTime { get; set; }

        [XmlMapping("prizeTime", 5, MappingType = MappingType.Attribute)]
        public DateTime PrizeTime { get; set; }

        [XmlMapping("status", 6, MappingType = MappingType.Attribute)]
        public int Status { get; set; }

        [XmlMapping("bonusCode", 7, MappingType = MappingType.Attribute)]
        public string BonusCode { get; set; }

        [XmlMapping("BonusInfo", 8, MappingType = MappingType.Attribute)]
        public string BonusInfo { get; set; }
    }

    #endregion

    #region 出票请求

    public class TicketRequestInfo : XmlMappingObject
    {
        /// <summary>
        ///     请求参数
        /// </summary>
        [XmlMapping("ticketorder", 0)]
        public TicketOrder ticketOrder { get; set; }

        public override string GetCode()
        {
            return "002";
        }
    }

    public class TicketOrder : XmlMappingObject
    {
        /// <summary>
        ///     玩法编号
        /// </summary>
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        /// <summary>
        ///     总注数
        /// </summary>
        [XmlMapping("ticketsnum", 1, MappingType = MappingType.Attribute)]
        public int TicketsNum { get; set; }

        /// <summary>
        ///     总金额
        /// </summary>
        [XmlMapping("totalmoney", 2, MappingType = MappingType.Attribute)]
        public decimal TotalMoney { get; set; }

        /// <summary>
        ///     订单信息集合
        /// </summary>
        [XmlMapping("tickets", 3, MappingType = MappingType.Element)]
        public TicketList tickets { get; set; }
    }

    public class TicketList : XmlMappingObject
    {
        [XmlMapping("ticket", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<InnerOrderInfo> InnerOrders { get; set; }
    }

    public class InnerOrderInfo : XmlMappingObject
    {
        /// <summary>
        ///     订单号（投注序列号）
        /// </summary>
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string OrderId { get; set; }

        /// <summary>
        ///     投注方式
        /// </summary>
        [XmlMapping("betType", 1, MappingType = MappingType.Attribute)]
        public string BetType { get; set; }

        /// <summary>
        ///     期号
        /// </summary>
        [XmlMapping("issueNumber", 2, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }

        /// <summary>
        ///     注数
        /// </summary>
        [XmlMapping("betUnits", 3, MappingType = MappingType.Attribute)]
        public int BetUnits { get; set; }

        /// <summary>
        ///     倍数
        /// </summary>
        [XmlMapping("multiple", 4, MappingType = MappingType.Attribute)]
        public int Multiple { get; set; }

        /// <summary>
        ///     订单金额
        /// </summary>
        [XmlMapping("betMoney", 5, MappingType = MappingType.Attribute)]
        public decimal BetMoney { get; set; }

        /// <summary>
        ///     是否追加
        /// </summary>
        [XmlMapping("isAppend", 6, MappingType = MappingType.Attribute)]
        public int IsAppend { get; set; }

        /// <summary>
        ///     投注字符串
        /// </summary>
        [XmlMapping("betContent", 7)]
        public string BetContent { get; set; }
    }

    #endregion

    #region 出票响应

    public class TicketResponseInfo : XmlMappingObject
    {
        /// <summary>
        ///     订单信息集合
        /// </summary>
        [XmlMapping("tickets", 0, MappingType = MappingType.Element)]
        public TicketResponseList Tickets { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }

        public override string GetCode()
        {
            return "102";
        }
    }

    public class TicketResponseList : XmlMappingObject
    {
        [XmlMapping("ticket", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<InnerOrderResponseInfo> InnerOrders { get; set; }
    }

    public class InnerOrderResponseInfo : XmlMappingObject
    {
        //<ticket ticketId="1234567" multiple="1" issueNumber="201203" betType="P3_1" betUnits="9" betMoney="18" statusCode="909" message="投注订单ID重复" palmid="" />
        /// <summary>
        ///     订单号（投注序列号）
        /// </summary>
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }

        /// <summary>
        ///     倍数
        /// </summary>
        [XmlMapping("multiple", 1, MappingType = MappingType.Attribute)]
        public int Multiple { get; set; }

        /// <summary>
        ///     期号
        /// </summary>
        [XmlMapping("issueNumber", 2, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }

        /// <summary>
        ///     投注方式
        /// </summary>
        [XmlMapping("betType", 3, MappingType = MappingType.Attribute)]
        public string BetType { get; set; }

        /// <summary>
        ///     注数
        /// </summary>
        [XmlMapping("betUnits", 4, MappingType = MappingType.Attribute)]
        public int BetUnits { get; set; }

        /// <summary>
        ///     订单金额
        /// </summary>
        [XmlMapping("betMoney", 5, MappingType = MappingType.Attribute)]
        public decimal BetMoney { get; set; }

        /// <summary>
        ///     状态
        /// </summary>
        [XmlMapping("statusCode", 6, MappingType = MappingType.Attribute)]
        public string StatusCode { get; set; }

        /// <summary>
        ///     消息
        /// </summary>
        [XmlMapping("message", 7, MappingType = MappingType.Attribute)]
        public string Message { get; set; }

        /// <summary>
        ///     彩票平台序号
        /// </summary>
        [XmlMapping("palmid", 8, MappingType = MappingType.Attribute)]
        public string Palmid { get; set; }

        /// <summary>
        ///     详细消息
        /// </summary>
        [XmlMapping("detailmessage", 9, MappingType = MappingType.Attribute)]
        public string DetailMessage { get; set; }
    }

    #endregion

    #region 交易结果查询

    public class QueryTicketRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryticket", 0, MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryTicketRequestInnerInfo> QueryTicketInnerInfo { get; set; }

        public override string GetCode()
        {
            return "003";
        }
    }

    public class QueryTicketRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }

        [XmlMapping("palmId", 1, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }
    }

    public class AutoTicketResultRequestInfo : XmlMappingObject
    {
        [XmlMapping("ticketresults", 0, MappingType = MappingType.Element)]
        public AutoTicketResultRequestList TicketResultList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    public class AutoTicketResultRequestList : XmlMappingObject
    {
        [XmlMapping("ticketresult", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryTicketResponseInnerInfo> InnerTicketResultItems { get; set; }
    }

    public class AutoTicketResultResponseInfo : XmlMappingObject
    {
        [XmlMapping("returnticketresults", 0, MappingType = MappingType.Element)]
        public AutoTicketResultResponseList TicketResultList { get; set; }

        public override string GetCode()
        {
            return "107";
        }
    }

    public class AutoTicketResultResponseList : XmlMappingObject
    {
        [XmlMapping("returnticketresult", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<AutoTicketResultResponseItem> InnerTicketResultItems { get; set; }
    }

    public class AutoTicketResultResponseItem : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("palmId", 1, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }
    }

    public class QueryTicketResponseInfo : XmlMappingObject
    {
        [XmlMapping("ticketresult", 0, MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryTicketResponseInnerInfo> QueryTicketResponseInnerInfo { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }

        public override string GetCode()
        {
            return "103";
        }
    }

    public class QueryTicketResponseInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }

        [XmlMapping("ticketId", 2, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }

        [XmlMapping("palmId", 3, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }

        [XmlMapping("statusCode", 4, MappingType = MappingType.Attribute)]
        public string StatusCode { get; set; }

        [XmlMapping("message", 5, MappingType = MappingType.Attribute)]
        public string Message { get; set; }

        [XmlMapping("printodd", 6, MappingType = MappingType.Attribute)]
        public string PrintOdd { get; set; }

        [XmlMapping("Unprintodd", 7, MappingType = MappingType.Attribute)]
        public string UnprintOdd { get; set; }

        [XmlMapping("maxBonus", 8, MappingType = MappingType.Attribute)]
        public string MaxBonus { get; set; }

        [XmlMapping("printNo", 9, MappingType = MappingType.Attribute)]
        public string PrintNo { get; set; }
    }

    #endregion

    #region 比赛赛果查询

    public class GameResultQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("querygameresult", 0)]
        public GameResultQueryRequestInnerInfo GameResultQueryInfo { get; set; }
    }

    public class GameResultQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
    }

    public class GameResultQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("results", 0, MappingType = MappingType.Element)]
        public GameResultQueryResponseList GameResultQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    public class GameResultQueryResponseList : XmlMappingObject
    {
        [XmlMapping("result", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<GameResultQueryResponseItem> GameResultQueryResponseItems { get; set; }
    }

    public class GameResultQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("matchId", 0, MappingType = MappingType.Attribute)]
        public string MatchId { get; set; }

        [XmlMapping("matchtime", 1, MappingType = MappingType.Attribute)]
        public DateTime MatchTime { get; set; }

        [XmlMapping("value", 2, MappingType = MappingType.Attribute)]
        public string ResultValue { get; set; }

        [XmlMapping("polygoal", 3, MappingType = MappingType.Attribute)]
        public string Goal_RQ { get; set; }

        [XmlMapping("goal", 4, MappingType = MappingType.Attribute)]
        public string Goal_RF { get; set; }

        [XmlMapping("ougoal", 5, MappingType = MappingType.Attribute)]
        public string Goal_ZF { get; set; }

        [XmlMapping("sp", 6, MappingType = MappingType.Attribute)]
        public string SP_BJDC { get; set; }

        [XmlMapping("dsp", 7, MappingType = MappingType.Attribute)]
        public string SP_JC_DG { get; set; }

        [XmlMapping("gsp", 8, MappingType = MappingType.Attribute)]
        public string SP_JC_GG { get; set; }
    }

    #endregion

    #region 奖金查询

    public class QueryPrizeRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryprize", 0, MappingType.Element)]
        public QueryPrizeRequestInnerInfo QueryPrizeInnerInfo { get; set; }
    }

    public class QueryPrizeRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }

        [XmlMapping("prevTicketId", 2, MappingType = MappingType.Attribute)]
        public string PrevTicketId { get; set; }

        [XmlMapping("status", 3, MappingType = MappingType.Attribute)]
        public string Status { get; set; }
    }

    public class QueryPrizeResponseInfo : XmlMappingObject
    {
        [XmlMapping("prizeresult", 0, MappingType.Element)]
        public QueryPrizeResultInfo QueryPrizeResultInfo { get; set; }

        [XmlMapping("wontickets", 1, MappingType.Element)]
        public QueryPrizeWinTicketListInfo QueryPrizeWinTicketList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    public class QueryPrizeResultInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }

        [XmlMapping("winNum", 2, MappingType = MappingType.Attribute)]
        public int TotalWinCount { get; set; }

        [XmlMapping("totalPrize", 3, MappingType = MappingType.Attribute)]
        public decimal TotalPrizeMoney { get; set; }

        // 剩余的票数
        [XmlMapping("num", 4, MappingType = MappingType.Attribute)]
        public int Number { get; set; }

        [XmlMapping("lastTicketId", 5, MappingType = MappingType.Attribute)]
        public string LastTicketId { get; set; }
    }

    public class QueryPrizeWinTicketListInfo : XmlMappingObject
    {
        [XmlMapping("wonticket", 0, MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryPrizeWinTicketInnerInfo> QueryPrizeWinTicketList { get; set; }
    }

    public class QueryPrizeWinTicketInnerInfo : XmlMappingObject
    {
        [XmlMapping("ticketId", 0, MappingType = MappingType.Attribute)]
        public string TicketId { get; set; }

        [XmlMapping("pretaxPrice", 1, MappingType = MappingType.Attribute)]
        public decimal BonusMoneyPrevTax { get; set; }

        [XmlMapping("prize", 2, MappingType = MappingType.Attribute)]
        public decimal BonusMoneyAfterTax { get; set; }

        [XmlMapping("palmId", 3, MappingType = MappingType.Attribute)]
        public string PalmId { get; set; }

        [XmlMapping("state", 4, MappingType = MappingType.Attribute)]
        public string State { get; set; }

        [XmlMapping("IsCancelGame", 5, MappingType = MappingType.Attribute)]
        public string IsCancelGame { get; set; }

        [XmlMapping("IsAwards", 6, MappingType = MappingType.Attribute)]
        public string IsAwards { get; set; }

        [XmlMapping("awardGradedetail", 7, MappingType.Element, ObjectType = XmlObjectType.List)]
        public XmlMappingList<QueryPrizeAwardGradeDetailInfo> QueryPrizeAwardGradeDetailList { get; set; }
    }

    public class QueryPrizeAwardGradeDetailInfo : XmlMappingObject
    {
        [XmlMapping("gradid", 0, MappingType = MappingType.Attribute)]
        public string GradId { get; set; }

        [XmlMapping("awardCount", 1, MappingType = MappingType.Attribute)]
        public string AwardCount { get; set; }

        [XmlMapping("AwardMoney", 2, MappingType = MappingType.Attribute)]
        public string AwardMoney { get; set; }
    }

    #endregion

    #region 查询余额

    public class BalanceRequestInfo : XmlMappingObject
    {
        [XmlMapping("partneraccount", 0)]
        public PartnerAccount PartnerAccount { get; set; }
    }

    public class PartnerAccount : XmlMappingObject
    {
        [XmlMapping("partnerid", 0, MappingType = MappingType.Attribute)]
        public string PartnerId { get; set; }

        [XmlMapping("balance", 0, MappingType = MappingType.Attribute)]
        public decimal Balance { get; set; }
    }

    public class BalanceResponseInfo : XmlMappingObject
    {
        [XmlMapping("partneraccount", 0)]
        public PartnerAccount PartnerAccount { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    #endregion

    #region 竞彩比赛列表查询

    public class JC_GameQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("querySchedule", 0)]
        public JC_GameQueryRequestInnerInfo GameQueryInfo { get; set; }
    }

    public class JC_GameQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("type", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }
    }

    public class JC_GameQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("jcgames", 0, MappingType = MappingType.Element)]
        public JC_GameQueryResponseList GameQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    public class JC_GameQueryResponseList : XmlMappingObject
    {
        [XmlMapping("jcgame", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<JC_GameQueryResponseItem> InnerGameQueryResponseItems { get; set; }
    }

    public class JC_GameQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("name", 0, MappingType = MappingType.Attribute)]
        public string GameName { get; set; }

        [XmlMapping("matchID", 1, MappingType = MappingType.Attribute)]
        public string MatchID { get; set; }

        [XmlMapping("hometeam", 2, MappingType = MappingType.Attribute)]
        public string HomeTeam { get; set; }

        [XmlMapping("guestteam", 3, MappingType = MappingType.Attribute)]
        public string GuestTeam { get; set; }

        [XmlMapping("matchstate", 4, MappingType = MappingType.Attribute)]
        public string MatchState { get; set; }

        [XmlMapping("matchtime", 5, MappingType = MappingType.Attribute)]
        public DateTime MatchTime { get; set; }

        [XmlMapping("sellouttime", 6, MappingType = MappingType.Attribute)]
        public DateTime SelloutTime { get; set; }

        /// <summary>
        ///     让球
        /// </summary>
        [XmlMapping("polygonal", 7, MappingType = MappingType.Attribute)]
        public string Polygonal { get; set; }

        /// <summary>
        ///     让分盘口
        /// </summary>
        [XmlMapping("goal", 8, MappingType = MappingType.Attribute)]
        public string Goal { get; set; }

        /// <summary>
        ///     总分盘口
        /// </summary>
        [XmlMapping("ougoal", 9, MappingType = MappingType.Attribute)]
        public string Ougoal { get; set; }
    }

    #endregion

    #region 北单比赛列表查询

    public class BJDC_GameQueryRequestInfo : XmlMappingObject
    {
        [XmlMapping("querygames", 0)]
        public BJDC_GameQueryRequestInnerInfo GameQueryInfo { get; set; }
    }

    public class BJDC_GameQueryRequestInnerInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string LotteryId { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
    }

    public class BJDC_GameQueryResponseInfo : XmlMappingObject
    {
        [XmlMapping("games", 0, MappingType = MappingType.Element)]
        public BJDC_GameQueryResponseList GameQueryResponseList { get; set; }

        [XmlMapping("error", 0, MappingType = MappingType.Element)]
        public ErrorInfo ErrInfo { get; set; }
    }

    public class BJDC_GameQueryResponseList : XmlMappingObject
    {
        [XmlMapping("game", 0, ObjectType = XmlObjectType.List)]
        public XmlMappingList<BJDC_GameQueryResponseItem> InnerGameQueryResponseItems { get; set; }
    }

    public class BJDC_GameQueryResponseItem : XmlMappingObject
    {
        [XmlMapping("name", 0, MappingType = MappingType.Attribute)]
        public string GameName { get; set; }

        [XmlMapping("matchID", 1, MappingType = MappingType.Attribute)]
        public string MatchID { get; set; }

        [XmlMapping("hometeam", 2, MappingType = MappingType.Attribute)]
        public string HomeTeam { get; set; }

        [XmlMapping("guestteam", 3, MappingType = MappingType.Attribute)]
        public string GuestTeam { get; set; }

        [XmlMapping("matchtime", 4, MappingType = MappingType.Attribute)]
        public string MatchTimeSrc { get; set; }

        public DateTime MatchTime
        {
            get
            {
                if (string.IsNullOrEmpty(MatchTimeSrc))
                    return default(DateTime);
                var year = MatchTimeSrc.Substring(0, 4);
                var month = MatchTimeSrc.Substring(4, 2);
                var day = MatchTimeSrc.Substring(6, 2);
                var hour = MatchTimeSrc.Substring(8, 2);
                var minute = MatchTimeSrc.Substring(10, 2);
                return DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", year, month, day, hour, minute));
            }
        }

        [XmlMapping("sellouttime", 5, MappingType = MappingType.Attribute)]
        public string SelloutTimeSrc { get; set; }

        public DateTime SelloutTime
        {
            get
            {
                if (string.IsNullOrEmpty(SelloutTimeSrc))
                    return default(DateTime);
                var year = SelloutTimeSrc.Substring(0, 4);
                var month = SelloutTimeSrc.Substring(4, 2);
                var day = SelloutTimeSrc.Substring(6, 2);
                var hour = SelloutTimeSrc.Substring(8, 2);
                var minute = SelloutTimeSrc.Substring(10, 2);
                return DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}", year, month, day, hour, minute));
            }
        }

        [XmlMapping("matchstate", 6, MappingType = MappingType.Attribute)]
        public string MatchState { get; set; }

        [XmlMapping("remark", 7, MappingType = MappingType.Attribute)]
        public string Remark { get; set; }
    }

    #endregion


    /// <summary>
    ///     查询中奖请求对象
    /// </summary>
    public class WinNumberRequestInfo : XmlMappingObject
    {
        [XmlMapping("queryresult", 0, MappingType = MappingType.Element)]
        public WinNumberQueryInfo QueryResult { get; set; }
    }

    public class WinNumberQueryInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string GameCode { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }
    }

    /// <summary>
    ///     查询中奖返回对象
    /// </summary>
    public class WinNumberResponseInfo : XmlMappingObject
    {
        [XmlMapping("results", 0, MappingType.Element)]
        public WinNumberResultsInfo Results { get; set; }
    }

    public class WinNumberResultsInfo : XmlMappingObject
    {
        [XmlMapping("lotteryId", 0, MappingType = MappingType.Attribute)]
        public string GameCode { get; set; }

        [XmlMapping("issueNumber", 1, MappingType = MappingType.Attribute)]
        public string IssueNumber { get; set; }

        [XmlMapping("result", 2, MappingType = MappingType.Attribute)]
        public WinNumberResultInfo Result { get; set; }
    }

    public class WinNumberResultInfo : XmlMappingObject
    {
        [XmlMapping("value", 0, MappingType = MappingType.Attribute)]
        public string Winumber { get; set; }
    }

    public class LiangCaiWinNumberResultInfo : XmlMappingObject
    {
        [XmlMapping("xMsgID", 0, MappingType = MappingType.Attribute)]
        public string xMsgID { get; set; }

        [XmlMapping("xCode", 0, MappingType = MappingType.Attribute)]
        public string xCode { get; set; }

        [XmlMapping("xMessage", 0, MappingType = MappingType.Attribute)]
        public string xMessage { get; set; }

        [XmlMapping("xSign", 0, MappingType = MappingType.Attribute)]
        public string xSign { get; set; }

        [XmlMapping("xValue", 0, MappingType = MappingType.Attribute)]
        public string xValue { get; set; }
    }
}
