using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 传统足球比赛信息
    /// </summary>
    public class CTZQ_IssuseInfo
    {
        public string Id { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 单试停止投注时间
        /// </summary>
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public string StopBettingTime { get; set; }
        /// <summary>
        /// 官网截止投注时间
        /// </summary>
        public string OfficialStopTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 比赛投注开始时间
        /// </summary>
        public string StartTime { get; set; }
    }
    /// <summary>
    /// 传统足球奖池信息
    /// </summary>
    public class CTZQ_BonusPoolInfo
    {
        public string Id { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public string BonusLevel { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public string BonusLevelDisplayName { get; set; }
        /// <summary>
        /// 奖池金额
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 中奖数
        /// </summary>
        public int BonusCount { get; set; }
        /// <summary>
        /// 比赛结果
        /// </summary>
        public string MatchResult { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
    public class CTZQ_BonusPoolCollection : List<CTZQ_BonusPoolInfo>
    {
    }
}
