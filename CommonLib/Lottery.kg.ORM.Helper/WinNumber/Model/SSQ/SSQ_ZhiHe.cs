using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质合走势
    /// </summary>
    public class SSQ_ZhiHe : ImportBase
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


        public virtual int O_RedBall1_Z { get; set; }
        public virtual int O_RedBall2_Z { get; set; }
        public virtual int O_RedBall3_Z { get; set; }
        public virtual int O_RedBall4_Z { get; set; }
        public virtual int O_RedBall5_Z { get; set; }
        public virtual int O_RedBall6_Z { get; set; }

        public virtual int O_RedBall1_H { get; set; }
        public virtual int O_RedBall2_H { get; set; }
        public virtual int O_RedBall3_H { get; set; }
        public virtual int O_RedBall4_H { get; set; }
        public virtual int O_RedBall5_H { get; set; }
        public virtual int O_RedBall6_H { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public virtual string ZH_Qualifying { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string ZH_Proportion { get; set; }
        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public virtual int O_ZH_Proportion60 { get; set; }
        public virtual int O_ZH_Proportion51 { get; set; }
        public virtual int O_ZH_Proportion42 { get; set; }
        public virtual int O_ZH_Proportion33 { get; set; }
        public virtual int O_ZH_Proportion24 { get; set; }
        public virtual int O_ZH_Proportion15 { get; set; }
        public virtual int O_ZH_Proportion06 { get; set; }
    }
}
