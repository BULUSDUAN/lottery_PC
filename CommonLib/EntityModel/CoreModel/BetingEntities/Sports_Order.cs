using EntityModel.Enum;
using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    public class Sports_Order_Base
    {
        public virtual string SchemeId { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual SchemeSource SchemeSource { get; set; }
        public virtual SchemeBettingCategory SchemeBettingCategory { get; set; }
        public virtual TogetherSchemeSecurity Security { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual int Amount { get; set; }
        public virtual int BetCount { get; set; }
        public virtual int TotalMatchCount { get; set; }
        public virtual decimal TotalMoney { get; set; }
        public virtual decimal SuccessMoney { get; set; }
        public virtual DateTime StopTime { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual string TicketGateway { get; set; }
        public virtual decimal TicketProgress { get; set; }
        public virtual string TicketId { get; set; }
        public virtual string TicketLog { get; set; }
        public virtual ProgressStatus ProgressStatus { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual int BonusCount { get; set; }
        public virtual int HitMatchCount { get; set; }
        public virtual int RightCount { get; set; }
        public virtual int Error1Count { get; set; }
        public virtual int Error2Count { get; set; }
        public virtual decimal MinBonusMoney { get; set; }
        public virtual decimal MaxBonusMoney { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual bool CanChase { get; set; }
        public virtual bool IsVirtualOrder { get; set; }
        /// <summary>
        /// 是否已返点
        /// </summary>
        public virtual bool IsPayRebate { get; set; }
        /// <summary>
        /// 实际计算返点的金额(享受返点的金额)
        /// </summary>
        public virtual decimal RealPayRebateMoney { get; set; }
        /// <summary>
        /// 总返点金额
        /// </summary>
        public virtual decimal TotalPayRebateMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual DateTime BetTime { get; set; }
        public virtual string AgentId { get; set; }
        /// <summary>
        /// 扩展字段，活动三选一
        /// </summary>
        public virtual string ExtensionOne { get; set; }
        public virtual string Attach { get; set; }
        public virtual string QueryTicketStopTime { get; set; }
        /// <summary>
        /// 是否追加投注
        /// </summary>
        public virtual bool IsAppend { get; set; }
        /// <summary>
        /// 出票时间
        /// </summary>
        public virtual DateTime? TicketTime { get; set; }
        /// <summary>
        /// 红包金额
        /// </summary>
        public virtual decimal RedBagMoney { get; set; }
        /// <summary>
        /// 是否已拆票
        /// </summary>
        public virtual bool IsSplitTickets { get; set; }
    }
    public class Sports_Order_Running : Sports_Order_Base
    {
    }

    /// <summary>
    /// 用户保存的订单
    /// </summary>
    public class UserSaveOrder
    {
        public virtual string SchemeId { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string StrStopTime { get; set; }
        /// <summary>
        /// 几串几
        /// </summary>
        public virtual string PlayType { get; set; }
        /// <summary>
        /// 投注方案类别
        /// </summary>
        public virtual SchemeType SchemeType { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        public virtual SchemeSource SchemeSource { get; set; }
        /// <summary>
        /// 方案投注类别
        /// </summary>
        public virtual SchemeBettingCategory SchemeBettingCategory { get; set; }
        /// <summary>
        /// 进行状态
        /// </summary>
        public virtual ProgressStatus ProgressStatus { get; set; }
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public virtual int Amount { get; set; }
        /// <summary>
        /// 柱数
        /// </summary>
        public virtual int BetCount { get; set; }
        /// <summary>
        /// 投注总金额
        /// </summary>
        public virtual decimal TotalMoney { get; set; }
        public virtual DateTime StopTime { get; set; }
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 宝单宣言
        /// </summary>
        public virtual string SingleTreasureDeclaration { get; set; }
        /// <summary>
        /// 宝单分享提成
        /// </summary>
        public virtual decimal BDFXCommission { get; set; }
    }

    public class Sports_Order_Complate : Sports_Order_Base
    {
        public virtual string BonusCountDescription { get; set; }
        public virtual string BonusCountDisplayName { get; set; }
        public virtual DateTime ComplateDateTime { get; set; }
        public virtual string ComplateDate { get; set; }
        public virtual bool IsPrizeMoney { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual AddMoneyDistributionWay DistributionWay { get; set; }
        public virtual string AddMoneyDescription { get; set; }
    }

    public class Sports_AnteCode : ISportAnteCode
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string MatchId { get; set; }
        public virtual string AnteCode { get; set; }
        public virtual bool IsDan { get; set; }
        public virtual string Odds { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual DateTime CreateTime { get; set; }

        public virtual int Length { get { return AnteCode.Split(',', '|').Length; } }

        public virtual string GetMatchResult(string gameCode, string gameType, string score)
        {
            if (gameCode.ToLower() != "jclq")
            {
                return "";
            }
            if (score == "-" || score == "*")
            {
                //return "*"; old
                return "";
            }
            var tmp = score.Split(':');
            if (tmp.Length != 2)
            {
                throw new ArgumentException("比分格式错误");
            }
            if (tmp[0] == "-" || tmp[1] == "-" || tmp[0] == "-1" || tmp[1] == "-1")
            {
                //return "*"; old
                return "";
            }
            var score1 = decimal.Parse(tmp[0]);
            var score2 = decimal.Parse(tmp[1]);
            var total = score1 + score2;
            if (gameType.ToLower() == "rfsf")
            {
                var rf = GetResultOdds("rf");
                if (score1 + rf > score2)
                {
                    return "3";
                }
                else if (score1 + rf < score2)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            else if (gameType.ToLower() == "dxf")
            {
                var yszf = GetResultOdds("yszf");
                if (total > yszf)
                {
                    return "3";
                }
                else if (total < yszf)
                {
                    return "0";
                }
                else
                {
                    return "1";
                }
            }
            else
            {
                return "";
            }
        }
        public virtual decimal GetResultOdds(string matchResult)
        {
            var tmp = Odds.Split(',');
            foreach (var item in tmp)
            {
                var p = item.Split('|');
                if (p[0].Equals(matchResult, StringComparison.OrdinalIgnoreCase))
                {
                    return decimal.Parse(p[1]);
                }
            }
            throw new Exception(string.Format("没找到比赛{0}结果对应的赔率 - {1}", MatchId, matchResult));
        }
    }

    public class Sports_AnteCode_History
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string MatchId { get; set; }
        public virtual string AnteCode { get; set; }
        public virtual bool IsDan { get; set; }
        public virtual string Odds { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class SingleScheme_AnteCode
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        //public virtual string AnteCodeFullFileName { get; set; }
        public virtual string AllowCodes { get; set; }
        public virtual string SelectMatchId { get; set; }
        public virtual bool ContainsMatchId { get; set; }
        public virtual byte[] FileBuffer { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class YouHuaScheme_AnteCode
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string OrderSign { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual int Amount { get; set; }
        public virtual string MatchId { get; set; }
        public virtual string AnteCode { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class Sports_Ticket
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string TicketId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string MatchIdList { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        public virtual int BetUnits { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public virtual int Amount { get; set; }
        /// <summary>
        /// 票金额
        /// </summary>
        public virtual decimal BetMoney { get; set; }
        public virtual string BetContent { get; set; }
        public virtual string LocOdds { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual string TicketLog { get; set; }
        public virtual bool IsAppend { get; set; }

        /// <summary>
        /// 票号 1
        /// </summary>
        public virtual string PrintNumber1 { get; set; }
        /// <summary>
        /// 票号 2
        /// </summary>
        public virtual string PrintNumber2 { get; set; }
        /// <summary>
        /// 票号 3
        /// </summary>
        public virtual string PrintNumber3 { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public virtual string BarCode { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual DateTime? PrizeDateTime { get; set; }

        public virtual DateTime CreateTime { get; set; }
        public virtual DateTime? PrintDateTime { get; set; }
        /// <summary>
        /// 标识票从哪一个接口处
        /// </summary>
        public virtual string Gateway { get; set; }

    }

    public class Sports_TicketComparer : IEqualityComparer<Sports_Ticket>
    {
        public bool Equals(Sports_Ticket x, Sports_Ticket y)
        {
            if (x == null)
                return y == null;
            return x.TicketId == y.TicketId;
        }

        public int GetHashCode(Sports_Ticket obj)
        {
            if (obj == null)
                return 0;
            return obj.TicketId.GetHashCode();
        }
    }

    public class Sports_Ticket_History
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string TicketId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string MatchIdList { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        public virtual int BetUnits { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public virtual int Amount { get; set; }
        /// <summary>
        /// 票金额
        /// </summary>
        public virtual decimal BetMoney { get; set; }
        public virtual string BetContent { get; set; }
        public virtual string LocOdds { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual string TicketLog { get; set; }
        public virtual bool IsAppend { get; set; }

        /// <summary>
        /// 票号 1
        /// </summary>
        public virtual string PrintNumber1 { get; set; }
        /// <summary>
        /// 票号 2
        /// </summary>
        public virtual string PrintNumber2 { get; set; }
        /// <summary>
        /// 票号 3
        /// </summary>
        public virtual string PrintNumber3 { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public virtual string BarCode { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual DateTime? PrizeDateTime { get; set; }

        public virtual DateTime CreateTime { get; set; }
        public virtual DateTime? PrintDateTime { get; set; }
        /// <summary>
        /// 标识票从哪一个接口处
        /// </summary>
        public virtual string Gateway { get; set; }

    }

    /// <summary>
    /// 单式上传的订单信息
    /// </summary>
    public class SingleSchemeOrder
    {
        public virtual string OrderId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        /// <summary>
        /// 串关方式。5_1|6_1
        /// </summary>
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string SelectMatchId { get; set; }
        /// <summary>
        /// 允许投注的号，如胜平负 只能投 3 1 0
        /// </summary>
        public virtual string AllowCodes { get; set; }
        /// <summary>
        /// 是否包括场次编号
        /// </summary>
        public virtual bool ContainsMatchId { get; set; }
        /// <summary>
        /// 是否虚拟订单（不用出票到外部）
        /// </summary>
        public virtual bool IsVirtualOrder { get; set; }
        public virtual int Amount { get; set; }
        public virtual decimal TotalMoney { get; set; }
        //public virtual string AnteCodeFullFileName { get; set; }
        public virtual string FileBuffer { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
