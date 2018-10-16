using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Ticket
{
    public interface IAntecode
    {
        /// <summary>
        /// 所属游戏
        /// </summary>
        string GameCode { get; set; }
        /// <summary>
        /// 游戏玩法
        /// </summary>
        string GameType { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        string MatchId { get; }
        /// <summary>
        /// 投注号码
        /// </summary>
        string AnteNumber { get; set; }
    }
    /// <summary>
    /// 投注号码
    /// </summary>
    public class Antecode : IAntecode
    {
        /// <summary>
        /// 所属游戏
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 游戏玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        public string AnteNumber { get; set; }
        /// <summary>
        /// 获取投注号码包含注数。计算后有值
        /// </summary>
        public int BetCount { get; private set; }
        /// <summary>
        /// 获取投注号码金额。计算后有值
        /// </summary>
        public decimal AntecodeMoney { get; private set; }
        /// <summary>
        /// 分析号码。计算出包含注数以及号码金额
        /// </summary>
        public void AnalyzeAntecode(IAntecodeAnalyzable analyzer, decimal price = 2M)
        {
            BetCount = analyzer.AnalyzeAnteCode(AnteNumber);
            AntecodeMoney = BetCount * price;
        }
        /// <summary>
        /// 分析号码。计算出包含注数以及号码金额
        /// </summary>
        public virtual void AnalyzeAntecode(string GameCode, decimal price = 2M)
        {
            throw new NotImplementedException("没有实现该方法");
            //var analyzer = AnalyzerFactory.AnalyzerFactory.GetAntecodeAnalyzer(GameCode, GameType);
            //AnalyzeAntecode(analyzer, price);
        }
    }
}
