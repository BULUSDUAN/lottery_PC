using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatchBiz.Core;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 传统足球14场赛事信息
    /// </summary>
    public class SJB_Match
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public virtual int MatchId { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public virtual string Team { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 世界杯类型
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual string BetState { get; set; }
        /// <summary>
        /// 奖金
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public virtual decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public virtual decimal Probadbility { get; set; }
    }
}
