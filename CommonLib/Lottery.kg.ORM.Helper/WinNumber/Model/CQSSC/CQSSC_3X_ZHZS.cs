using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质合走势
    /// </summary>
    public class CQSSC_3X_ZHZS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        public virtual int Z_Red1 { get; set; }
        public virtual int H_Red1 { get; set; }
        public virtual int Z_Red2 { get; set; }
        public virtual int H_Red2 { get; set; }
        public virtual int Z_Red3 { get; set; }
        public virtual int H_Red3 { get; set; }
        /// <summary>
        /// 质合形态
        /// </summary>
        public virtual string Zhxt { get; set; }

        public virtual int ZH_Q_Z { get; set; }
        public virtual int ZH_ZZH { get; set; }
        public virtual int ZH_ZHZ { get; set; }
        public virtual int ZH_HZZ { get; set; }
        public virtual int ZH_ZHH { get; set; }
        public virtual int ZH_HZH { get; set; }
        public virtual int ZH_HHZ { get; set; }
        public virtual int ZH_Q_H { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public virtual string ZH_Proportion { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public virtual int O_ZH_Proportion30 { get; set; }
        public virtual int O_ZH_Proportion21 { get; set; }
        public virtual int O_ZH_Proportion12 { get; set; }
        public virtual int O_ZH_Proportion03 { get; set; }
    }
}
