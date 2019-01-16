using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class SJB_MatchInfo
    {
        /// <summary>
        /// 比赛编号
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public string Team { get; set; }
        /// <summary>
        /// 世界杯类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string BetState { get; set; }
        /// <summary>
        /// 奖金
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public decimal Probadbility { get; set; }
    }
}
