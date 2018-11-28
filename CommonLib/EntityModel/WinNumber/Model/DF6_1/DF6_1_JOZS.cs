using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class DF6_1_JOZS : ImportBase
    {
        public virtual int JO_J { get; set; }
        public virtual int JO_O { get; set; }

        public virtual int JO1_J { get; set; }
        public virtual int JO1_O { get; set; }

        public virtual int JO2_J { get; set; }
        public virtual int JO2_O { get; set; }

        public virtual int JO3_J { get; set; }
        public virtual int JO3_O { get; set; }

        public virtual int JO4_J { get; set; }
        public virtual int JO4_O { get; set; }

        public virtual int JO5_J { get; set; }
        public virtual int JO5_O { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public virtual int O_JO_Proportion06 { get; set; }
        public virtual int O_JO_Proportion15 { get; set; }
        public virtual int O_JO_Proportion24 { get; set; }
        public virtual int O_JO_Proportion33 { get; set; }
        public virtual int O_JO_Proportion42 { get; set; }
        public virtual int O_JO_Proportion51 { get; set; }
        public virtual int O_JO_Proportion60 { get; set; }
    }
}
