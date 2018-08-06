using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace EntityModel.Ticket
{
    /// <summary>
    /// 票
    /// </summary>
    public interface ITicket
    {
        /// <summary>
        /// 所属游戏
        /// </summary>
        string GameCode { get; set; }
        /// <summary>
        /// 所属游戏玩法
        /// </summary>
        string GameType { get; set; }
        /// <summary>
        /// 号码列表
        /// </summary>
        List<IAntecode> GetAnteCodeList();
    }
    /// <summary>
    /// 票
    /// </summary>
    public class Ticket : ITicket
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Ticket()
        {
            AnteCodeList = new List<Antecode>();
            Amount = 1;
        }
        /// <summary>
        /// 所属游戏
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 所属游戏玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 获取票包含的总注数。计算后有值
        /// </summary>
        public int BetCount { get;  set; }
        /// <summary>
        /// 获取票的总金额。计算后有值
        /// </summary>
        public decimal TicketMoney { get;  set; }
        /// <summary>
        /// 号码列表
        /// </summary>
        public List<Antecode> AnteCodeList { get;  set; }
        public List<IAntecode> GetAnteCodeList()
        {
            return AnteCodeList.Select(a => (IAntecode)a).ToList();
        }
        /// <summary>
        /// 分析票。计算出包含注数以及金额
        /// </summary>
        //public void AnalyzeTicket(string GameCode, decimal price = 2M)
        //{
        //    throw new NotImplementedException("NotImplementedException");
        //    //var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(GameCode, GameType);
        //    //BetCount = 0;
        //    //TicketMoney = 0;
        //    //AnteCodeList.ForEach(c =>
        //    //{
        //    //    c.AnalyzeAntecode(analyzer, price);
        //    //    BetCount += c.BetCount;
        //    //    TicketMoney += c.AntecodeMoney * Amount;
        //    //});
        //}
    }
    /// <summary>
    /// 订单中的票列表
    /// </summary>
    public class TicketCollection : List<Ticket>
    {
        /// <summary>
        /// 获取订单包含的总注数。计算后有值
        /// </summary>
        public int BetCount { get;  set; }
        /// <summary>
        /// 获取订单的总金额。计算后有值
        /// </summary>
        public decimal TotalMoney { get;  set; }
        /// <summary>
        /// 分析订单。计算出包含注数以及金额
        /// </summary>
        //public void AnalyzeOrder(string GameCode, decimal price = 2M)
        //{
        //    BetCount = 0;
        //    TotalMoney = 0;
        //    ForEach(t =>
        //    {
        //        t.AnalyzeTicket(GameCode, price);
        //        BetCount += t.BetCount;
        //        TotalMoney += t.TicketMoney;
        //    });
        //}
    }
}
