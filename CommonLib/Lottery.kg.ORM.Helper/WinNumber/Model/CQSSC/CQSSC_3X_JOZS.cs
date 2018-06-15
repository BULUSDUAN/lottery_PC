using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class CQSSC_3X_JOZS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }


        public virtual int J_Red1 { get; set; }
        public virtual int O_Red1 { get; set; }
        public virtual int J_Red2 { get; set; }
        public virtual int O_Red2 { get; set; }
        public virtual int J_Red3 { get; set; }
        public virtual int O_Red3 { get; set; }
        /// <summary>
        /// 奇偶形态
        /// </summary>
        public virtual string Joxt { get; set; }

        public virtual int JO_Q_J { get; set; }
        public virtual int JO_JJO { get; set; }
        public virtual int JO_JOJ { get; set; }
        public virtual int JO_OJJ { get; set; }
        public virtual int JO_JOO { get; set; }
        public virtual int JO_OJO { get; set; }
        public virtual int JO_OOJ { get; set; }
        public virtual int JO_Q_O { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public virtual int O_JO_Proportion30 { get; set; }
        public virtual int O_JO_Proportion21 { get; set; }
        public virtual int O_JO_Proportion12 { get; set; }
        public virtual int O_JO_Proportion03 { get; set; }
    }
}
