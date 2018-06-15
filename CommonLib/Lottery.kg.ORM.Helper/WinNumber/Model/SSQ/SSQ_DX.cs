using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小走势
    /// </summary>
    public class SSQ_DX : ImportBase
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


        public virtual int O_RedBall1_D { get; set; }
        public virtual int O_RedBall2_D { get; set; }
        public virtual int O_RedBall3_D { get; set; }
        public virtual int O_RedBall4_D { get; set; }
        public virtual int O_RedBall5_D { get; set; }
        public virtual int O_RedBall6_D { get; set; }

        public virtual int O_RedBall1_X { get; set; }
        public virtual int O_RedBall2_X { get; set; }
        public virtual int O_RedBall3_X { get; set; }
        public virtual int O_RedBall4_X { get; set; }
        public virtual int O_RedBall5_X { get; set; }
        public virtual int O_RedBall6_X { get; set; }
        /// <summary>
        /// 大小排位
        /// </summary>
        public virtual string DX_Qualifying { get; set; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Proportion { get; set; }
        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public virtual int O_DX_Proportion06 { get; set; }
        public virtual int O_DX_Proportion15 { get; set; }
        public virtual int O_DX_Proportion24 { get; set; }
        public virtual int O_DX_Proportion33 { get; set; }
        public virtual int O_DX_Proportion42 { get; set; }
        public virtual int O_DX_Proportion51 { get; set; }
        public virtual int O_DX_Proportion60 { get; set; }
    }
}
