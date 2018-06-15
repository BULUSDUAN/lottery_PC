using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class SSQ_JiOu : ImportBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public virtual string RedLotteryNumber { get; set; }

        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }
        public virtual string RedBall4 { get; set; }
        public virtual string RedBall5 { get; set; }
        public virtual string RedBall6 { get; set; }


        public virtual int O_RedBall1_J { get; set; }
        public virtual int O_RedBall2_J { get; set; }
        public virtual int O_RedBall3_J { get; set; }
        public virtual int O_RedBall4_J { get; set; }
        public virtual int O_RedBall5_J { get; set; }
        public virtual int O_RedBall6_J { get; set; }

        public virtual int O_RedBall1_O { get; set; }
        public virtual int O_RedBall2_O { get; set; }
        public virtual int O_RedBall3_O { get; set; }
        public virtual int O_RedBall4_O { get; set; }
        public virtual int O_RedBall5_O { get; set; }
        public virtual int O_RedBall6_O { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public virtual string JO_Qualifying { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }
        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public virtual int O_JO_Proportion60 { get; set; }
        public virtual int O_JO_Proportion51 { get; set; }
        public virtual int O_JO_Proportion42 { get; set; }
        public virtual int O_JO_Proportion33 { get; set; }
        public virtual int O_JO_Proportion24 { get; set; }
        public virtual int O_JO_Proportion15 { get; set; }
        public virtual int O_JO_Proportion06 { get; set; }
    }
}
