using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    public class Ticket_BonusPool
    {
        public virtual string Id { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public virtual string BonusLevel { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public virtual string BonusLevelDisplayName { get; set; }
        /// <summary>
        /// 奖池金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 中奖数
        /// </summary>
        public virtual int BonusCount { get; set; }
        /// <summary>
        /// 比赛结果
        /// </summary>
        public virtual string WinNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
