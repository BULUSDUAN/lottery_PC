using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质合走势
    /// </summary>
    public class DF6_1_ZHZS : ImportBase
    {
        public virtual int ZH_Z { get; set; }
        public virtual int ZH_H { get; set; }

        public virtual int ZH1_Z { get; set; }
        public virtual int ZH1_H { get; set; }

        public virtual int ZH2_Z { get; set; }
        public virtual int ZH2_H { get; set; }

        public virtual int ZH3_Z { get; set; }
        public virtual int ZH3_H { get; set; }

        public virtual int ZH4_Z { get; set; }
        public virtual int ZH4_H { get; set; }

        public virtual int ZH5_Z { get; set; }
        public virtual int ZH5_H { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public virtual string ZH_Proportion { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public virtual int O_ZH_Proportion06 { get; set; }
        public virtual int O_ZH_Proportion15 { get; set; }
        public virtual int O_ZH_Proportion24 { get; set; }
        public virtual int O_ZH_Proportion33 { get; set; }
        public virtual int O_ZH_Proportion42 { get; set; }
        public virtual int O_ZH_Proportion51 { get; set; }
        public virtual int O_ZH_Proportion60 { get; set; }
    }
}
