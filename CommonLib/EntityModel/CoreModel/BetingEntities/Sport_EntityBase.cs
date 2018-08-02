using EntityModel.Enum;
using EntityModel.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    public interface IEntityOrder
    {
        string OrderId { get; }
        string GameCode { get; }
        string GameType { get; }
        string PlayType { get; }
        decimal TotalMoney { get; }
        int TotalBetCount { get; }
        int Amount { get; }
        decimal Price { get; }
        int TotalMatchCount { get; }

        IList<IEntityTicket> GetTicketList();
    }
    public interface IEntityTicket : ITicket
    {
        string Id { get; }
        string OrderId { get; }
        string AgentId { get; }
        string IssuseNumber { get; }
        int BaseCount { get; }
        string PlayType { get; }
        decimal TotalMoney { get; }
        int Amount { get; }
        int BetCount { get; }
        decimal Price { get; }
        int TotalMatchCount { get; }

        IList<IEntityAnteCode> GetAnteCodeList();
    }
    public interface IEntityAnteCode : IAntecode
    {
        string Id { get; }
        string TicketId { get; }
        string OrderId { get; }
        string GameCode { get; }
        string GameType { get; }
        string MatchId { get; }
        string AnteNumber { get; }
        bool IsDan { get; }
    }
    /// <summary>
    /// 投注号码
    /// </summary>
    public class I_Sport_AnteCode : IEntityAnteCode
    {
        // 自增编号
        public virtual string Id { get; set; }
        // 用户
        public virtual string AgentId { get; set; }
        // 所属票
        public virtual string TicketId { get; set; }
        // 所属订单
        public virtual string OrderId { get; set; }
        // 彩种
        public virtual string GameCode { get; set; }
        // 游戏玩法
        public virtual string GameType { get; set; }
        // 奖期
        public virtual string IssuseNumber { get; set; }
        // 比赛场次
        public virtual string MatchId { get; set; }
        // 投注号码
        public virtual string AnteNumber { get; set; }
        // 赔率编号
        public virtual long OddsId { get; set; }
        // 赔率
        public virtual string Odds { get; set; }
        // 是否是胆码
        public virtual bool IsDan { get; set; }
        // 中奖状态
        public virtual BonusStatus BonusStatus { get; set; }
        // 请求时间
        public virtual DateTime CreateTime { get; set; }
        // 更新时间
        public virtual DateTime UpdateTime { get; set; }

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
            if (matchResult.Equals("-1", StringComparison.OrdinalIgnoreCase))
                return 1;
            throw new Exception(string.Format("没找到比赛{0}结果对应的赔率 - {1}", MatchId, matchResult));
        }
        public virtual void LoadByObject(I_Sport_AnteCode running)
        {
            Id = running.Id;
            AgentId = running.AgentId;
            TicketId = running.TicketId;
            OrderId = running.OrderId;
            GameCode = running.GameCode;
            GameType = running.GameType;
            MatchId = running.MatchId;
            AnteNumber = running.AnteNumber;
            OddsId = running.OddsId;
            Odds = running.Odds;
            IsDan = running.IsDan;
            BonusStatus = running.BonusStatus;
            CreateTime = running.CreateTime;
            UpdateTime = running.UpdateTime;
        }
    }
    /// <summary>
    /// 订单实体对象
    /// </summary>
    public class I_Sport_Order<TTicket, TAnteCode> : IEntityOrder
        where TTicket : I_Sport_Ticket<TAnteCode>, new()
        where TAnteCode : I_Sport_AnteCode, new()
    {

        // Guid 自动编码
        public virtual string AgentId { get; set; }
        // 订单编号。由代理商提供
        public virtual string OrderId { get; set; }
        // 出票目标
        public virtual string TicketGateway { get; set; }
        // 彩种
        public virtual string GameCode { get; set; }
        // 玩法
        public virtual string GameType { get; set; }
        // 串关方式
        public virtual string PlayType { get; set; }
        // 彩票期号
        public virtual string IssuseNumber { get; set; }
        // 投注总金额 （包含多倍多期的总金额）
        public virtual decimal TotalMoney { get; set; }
        // 投注总注数
        public virtual int TotalBetCount { get; set; }
        // 投注倍数 1--99
        public virtual int Amount { get; set; }
        // 附加信息 （不能包含特殊符号）
        public virtual string Attach { get; set; }
        // 单注投注金额
        public virtual decimal Price { get; set; }
        // 请求时间
        public virtual DateTime RequestTime { get; set; }
        // 比赛场数
        public virtual int TotalMatchCount { get; set; }
        // 出票状态
        public virtual ProgressStatus TicketStatus { get; set; }
        public virtual SchemeBettingCategory BettingCategory { get; set; }
        // 出票时间
        public virtual DateTime? TicketTime { get; set; }

        public virtual IList<TTicket> TicketList { get; set; }

        public virtual IList<IEntityTicket> GetTicketList()
        {
            return TicketList.Select(t => (IEntityTicket)t).ToList();
        }

        public virtual void LoadByObject<TTicket1, TAnteCode1>(I_Sport_Order<TTicket1, TAnteCode1> running)
            where TTicket1 : I_Sport_Ticket<TAnteCode1>, new()
            where TAnteCode1 : I_Sport_AnteCode, new()
        {
            AgentId = running.AgentId;
            OrderId = running.OrderId;
            TicketGateway = running.TicketGateway;
            GameCode = running.GameCode;
            GameType = running.GameType;
            PlayType = running.PlayType;
            TotalMoney = running.TotalMoney;
            TotalBetCount = running.TotalBetCount;
            Amount = running.Amount;
            Attach = running.Attach;
            Price = running.Price;
            RequestTime = running.RequestTime;
            TotalMatchCount = running.TotalMatchCount;
            TicketStatus = running.TicketStatus;
            TicketTime = running.TicketTime;

            TicketList = new List<TTicket>();
            foreach (var ticket in running.TicketList)
            {
                var ticketPrized = new TTicket();
                ticketPrized.LoadByObject(ticket);
                TicketList.Add(ticketPrized);
            }
        }
    }
    /// <summary>
    /// 订单实体对象
    /// </summary>
    public class I_Sport_Ticket<T> : IEntityTicket
        where T : I_Sport_AnteCode, new()
    {
        // Guid 自动编码
        public virtual string Id { get; set; }
        // 经销商
        public virtual string AgentId { get; set; }
        // 订单编号。由代理商提供
        public virtual string OrderId { get; set; }
        // 出票目标
        public virtual string TicketGateway { get; set; }
        // 彩种
        public virtual string GameCode { get; set; }
        // 玩法
        public virtual string GameType { get; set; }
        // 串关基数
        public virtual int BaseCount { get; set; }
        // 串关方式
        public virtual string PlayType { get; set; }
        // 彩票期号
        public virtual string IssuseNumber { get; set; }
        // 投注总金额
        public virtual decimal TotalMoney { get; set; }
        // 投注倍数 1--99
        public virtual int Amount { get; set; }
        // 注数
        public virtual int BetCount { get; set; }
        // 附加信息 （不能包含特殊符号）
        public virtual string Attach { get; set; }
        // 单注投注金额
        public virtual decimal Price { get; set; }
        // 请求时间
        public virtual DateTime RequestTime { get; set; }
        // 比赛场数
        public virtual int TotalMatchCount { get; set; }
        // 出票状态
        public virtual TicketStatus TicketStatus { get; set; }
        // 出票时间
        public virtual DateTime? TicketTime { get; set; }

        public virtual IList<T> AnteCodeList { get; set; }

        public virtual IList<IEntityAnteCode> GetAnteCodeList()
        {
            return AnteCodeList.Select(t => (IEntityAnteCode)t).ToList();
        }
        List<IAntecode> ITicket.GetAnteCodeList()
        {
            return AnteCodeList.Select(t => (IAntecode)t).ToList();
        }

        public virtual void LoadByObject<T1>(I_Sport_Ticket<T1> running)
            where T1 : I_Sport_AnteCode, new()
        {
            Id = running.Id;
            AgentId = running.AgentId;
            OrderId = running.OrderId;
            TicketGateway = running.TicketGateway;
            GameCode = running.GameCode;
            GameType = running.GameType;
            PlayType = running.PlayType;
            TotalMoney = running.TotalMoney;
            Amount = running.Amount;
            Attach = running.Attach;
            Price = running.Price;
            RequestTime = running.RequestTime;
            TotalMatchCount = running.TotalMatchCount;
            TicketStatus = running.TicketStatus;
            TicketTime = running.TicketTime;

            AnteCodeList = new List<T>();
            foreach (var ante in running.AnteCodeList)
            {
                var antePrized = new T();
                antePrized.LoadByObject(ante);
                AnteCodeList.Add(antePrized);
            }
        }
    }
}
