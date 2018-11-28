using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common.Lottery.Objects;
using Common.Lottery;

namespace GameBiz.Business.Domain.Entities.Ticket
{
    /// <summary>
    /// 订单实体对象
    /// </summary>
    public class Ticket_Order_Running : I_Sport_Order<Ticket_Ticket_Running, Ticket_AnteCode_Running>
    {
        public virtual string CompositeId { get; set; }
        // 胆码场数
        public virtual int AnteDanCount { get; set; }
        // 拖码场数
        public virtual int AnteTuoCount { get; set; }
        // 总场数
        public virtual int AnteTotalCount { get; set; }
        // 命中胆码场数
        public virtual int HitDanCount { get; set; }
        // 命中拖码场数
        public virtual int HitTuoCount { get; set; }
        // 总命中场数
        public virtual int TotalHitCount { get; set; }
        // 命中胆码比赛列表
        public virtual string HitDanMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTuoMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTotalMatchIdList { get; set; }
        // 中奖状态
        public virtual BonusStatus BonusStatus { get; set; }
        // 中奖注数
        public virtual int BonusCount { get; set; }
        // 税前奖金
        public virtual decimal BonusMoneyBeforeTax { get; set; }
        // 税后奖金
        public virtual decimal BonusMoneyAfterTax { get; set; }
        // 响应时间
        public virtual DateTime? BonusTime { get; set; }
        // 用于测试的，对比比赛结果的字符串
        public virtual string MatchTestString { get; set; }

        public virtual int SuccessCount { get; set; }
        //总票数
        public virtual int TotalTicketCount { get; set; }
        public virtual bool IsSuccrssTicket { get; set; }
        public virtual DateTime Deadline { get; set; }

    }
    /// <summary>
    /// 订单实体对象
    /// </summary>
    public class Ticket_Order_Prized : I_Sport_Order<Ticket_Ticket_Prized, Ticket_AnteCode_Prized>
    {
        public virtual void LoadByRunning(Ticket_Order_Running running)
        {
            CompositeId = running.CompositeId;
            AgentId = running.AgentId;
            OrderId = running.OrderId;
            TicketGateway = running.TicketGateway;
            GameCode = running.GameCode;
            GameType = running.GameType;
            PlayType = running.PlayType;
            IssuseNumber = running.IssuseNumber;
            TotalMoney = running.TotalMoney;
            TotalBetCount = running.TotalBetCount;
            Amount = running.Amount;
            Attach = running.Attach;
            Price = running.Price;
            RequestTime = running.RequestTime;
            TotalMatchCount = running.TotalMatchCount;
            TicketStatus = running.TicketStatus;
            BettingCategory = running.BettingCategory;
            TicketTime = running.TicketTime;

            AnteDanCount = running.AnteDanCount;
            AnteTuoCount = running.AnteTuoCount;
            AnteTotalCount = running.AnteTotalCount;
            HitDanCount = running.HitDanCount;
            HitTuoCount = running.HitTuoCount;
            TotalHitCount = running.TotalHitCount;
            HitDanMatchIdList = running.HitDanMatchIdList;
            HitTuoMatchIdList = running.HitTuoMatchIdList;
            HitTotalMatchIdList = running.HitTotalMatchIdList;
            BonusStatus = running.BonusStatus;
            BonusCount = running.BonusCount;
            BonusMoneyBeforeTax = running.BonusMoneyBeforeTax;
            BonusMoneyAfterTax = running.BonusMoneyAfterTax;
            BonusTime = running.BonusTime.Value;

            MatchTestString = running.MatchTestString;

            TicketList = new List<Ticket_Ticket_Prized>();
            foreach (var ticket in running.TicketList)
            {
                var ticketPrized = new Ticket_Ticket_Prized();
                ticketPrized.LoadByRunning(ticket);
                TicketList.Add(ticketPrized);
            }
        }
        public virtual string CompositeId { get; set; }
        // 胆码场数
        public virtual int AnteDanCount { get; set; }
        // 拖码场数
        public virtual int AnteTuoCount { get; set; }
        // 总场数
        public virtual int AnteTotalCount { get; set; }
        // 命中胆码场数
        public virtual int HitDanCount { get; set; }
        // 命中拖码场数
        public virtual int HitTuoCount { get; set; }
        // 总命中场数
        public virtual int TotalHitCount { get; set; }
        // 命中胆码比赛列表
        public virtual string HitDanMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTuoMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTotalMatchIdList { get; set; }
        // 中奖状态
        public virtual BonusStatus BonusStatus { get; set; }
        // 中奖注数
        public virtual int BonusCount { get; set; }
        // 是否处理
        public virtual bool IsHandle { get; set; }
        // 税前奖金
        public virtual decimal BonusMoneyBeforeTax { get; set; }
        // 税后奖金
        public virtual decimal BonusMoneyAfterTax { get; set; }
        // 响应时间
        public virtual DateTime BonusTime { get; set; }
        // 用于测试的，对比比赛结果的字符串
        public virtual string MatchTestString { get; set; }
    }

    /// <summary>
    /// 订单实体对象
    /// </summary>
    public class Ticket_Ticket_Running : I_Sport_Ticket<Ticket_AnteCode_Running>, ITicket
    {
        // 胆码场数
        public virtual int AnteDanCount { get; set; }
        // 拖码场数
        public virtual int AnteTuoCount { get; set; }
        // 总场数
        public virtual int AnteTotalCount { get; set; }
        // 命中胆码场数
        public virtual int HitDanCount { get; set; }
        // 命中拖码场数
        public virtual int HitTuoCount { get; set; }
        // 总命中场数
        public virtual int TotalHitCount { get; set; }
        // 命中胆码比赛列表
        public virtual string HitDanMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTuoMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTotalMatchIdList { get; set; }
        // 中奖状态
        public virtual BonusStatus BonusStatus { get; set; }
        // 中奖注数
        public virtual int BonusCount { get; set; }
        // 税前奖金
        public virtual decimal BonusMoneyBeforeTax { get; set; }
        // 税后奖金
        public virtual decimal BonusMoneyAfterTax { get; set; }
        // 响应时间
        public virtual DateTime? BonusTime { get; set; }

        public virtual decimal TicketMoney { get { return TotalMoney; } }
        public List<IAntecode> GetAnteCodeList()
        {
            return AnteCodeList.Select(a => (IAntecode)a).ToList();
        }
    }
    /// <summary>
    /// 订单实体对象
    /// </summary>
    public class Ticket_Ticket_Prized : I_Sport_Ticket<Ticket_AnteCode_Prized>
    {
        public virtual void LoadByRunning(Ticket_Ticket_Running running)
        {
            Id = running.Id;
            AgentId = running.AgentId;
            OrderId = running.OrderId;
            TicketGateway = running.TicketGateway;
            GameCode = running.GameCode;
            GameType = running.GameType;
            PlayType = running.PlayType;
            IssuseNumber = running.IssuseNumber;
            TotalMoney = running.TotalMoney;
            Amount = running.Amount;
            Attach = running.Attach;
            Price = running.Price;
            RequestTime = running.RequestTime;
            TotalMatchCount = running.TotalMatchCount;
            TicketStatus = running.TicketStatus;
            TicketTime = running.TicketTime;

            AnteDanCount = running.AnteDanCount;
            AnteTuoCount = running.AnteTuoCount;
            AnteTotalCount = running.AnteTotalCount;
            HitDanCount = running.HitDanCount;
            HitTuoCount = running.HitTuoCount;
            TotalHitCount = running.TotalHitCount;
            HitDanMatchIdList = running.HitDanMatchIdList;
            HitTuoMatchIdList = running.HitTuoMatchIdList;
            HitTotalMatchIdList = running.HitTotalMatchIdList;
            BonusStatus = running.BonusStatus;
            BonusCount = running.BonusCount;
            BonusMoneyBeforeTax = running.BonusMoneyBeforeTax;
            BonusMoneyAfterTax = running.BonusMoneyAfterTax;
            BonusTime = running.BonusTime.Value;

            AnteCodeList = new List<Ticket_AnteCode_Prized>();
            foreach (var ante in running.AnteCodeList)
            {
                var antePrized = new Ticket_AnteCode_Prized();
                antePrized.LoadByRunning(ante);
                AnteCodeList.Add(antePrized);
            }
        }
        // 胆码场数
        public virtual int AnteDanCount { get; set; }
        // 拖码场数
        public virtual int AnteTuoCount { get; set; }
        // 总场数
        public virtual int AnteTotalCount { get; set; }
        // 命中胆码场数
        public virtual int HitDanCount { get; set; }
        // 命中拖码场数
        public virtual int HitTuoCount { get; set; }
        // 总命中场数
        public virtual int TotalHitCount { get; set; }
        // 命中胆码比赛列表
        public virtual string HitDanMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTuoMatchIdList { get; set; }
        // 命中胆码比赛列表
        public virtual string HitTotalMatchIdList { get; set; }
        // 出票状态
        public virtual BonusStatus BonusStatus { get; set; }
        // 中奖注数
        public virtual int BonusCount { get; set; }
        // 税前奖金
        public virtual decimal BonusMoneyBeforeTax { get; set; }
        // 税后奖金
        public virtual decimal BonusMoneyAfterTax { get; set; }
        // 响应时间
        public virtual DateTime BonusTime { get; set; }
    }

    /// <summary>
    /// 投注号码
    /// </summary>
    public class Ticket_AnteCode_Running : I_Sport_AnteCode, ISportAnteCode, IAntecode
    {
        public virtual int MatchIndex { get; set; }
        public override string MatchId
        {
            get { return MatchIndex.ToString(); }
            set { MatchIndex = int.Parse(value); }
        }
        public virtual string AnteCode
        {
            get { return AnteNumber; }
        }
        public virtual int Length
        {
            get { return AnteNumber.Split(',').Length; }
        }
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
    }
    /// <summary>
    /// 投注号码
    /// </summary>
    public class Ticket_AnteCode_Prized : I_Sport_AnteCode
    {
        public virtual int MatchIndex { get; set; }

        public virtual void LoadByRunning(Ticket_AnteCode_Running running)
        {
            Id = running.Id;
            AgentId = running.AgentId;
            TicketId = running.TicketId;
            OrderId = running.OrderId;
            GameCode = running.GameCode;
            GameType = running.GameType;
            IssuseNumber = running.IssuseNumber;
            MatchId = running.MatchId;
            MatchId = running.MatchId;
            AnteNumber = running.AnteNumber;
            IsDan = running.IsDan;
            BonusStatus = running.BonusStatus;
            CreateTime = running.CreateTime;
            UpdateTime = running.UpdateTime;
        }
    }
}
