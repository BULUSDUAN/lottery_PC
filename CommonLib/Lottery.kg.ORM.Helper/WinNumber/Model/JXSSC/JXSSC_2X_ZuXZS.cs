using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 重庆2星组选走势
    /// </summary>
    public class JXSSC_2X_ZuXZS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }

        public virtual string Type { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public virtual string ZH_Proportion { get; set; }

        public virtual int Red_0 { get; set; }
        public virtual int Red_1 { get; set; }
        public virtual int Red_2 { get; set; }
        public virtual int Red_3 { get; set; }
        public virtual int Red_4 { get; set; }
        public virtual int Red_5 { get; set; }
        public virtual int Red_6 { get; set; }
        public virtual int Red_7 { get; set; }
        public virtual int Red_8 { get; set; }
        public virtual int Red_9 { get; set; }

        public virtual int RedCan_0 { get; set; }
        public virtual int RedCan_1 { get; set; }
        public virtual int RedCan_2 { get; set; }
        public virtual int RedCan_3 { get; set; }
        public virtual int RedCan_4 { get; set; }
        public virtual int RedCan_5 { get; set; }
        public virtual int RedCan_6 { get; set; }
        public virtual int RedCan_7 { get; set; }
        public virtual int RedCan_8 { get; set; }
        public virtual int RedCan_9 { get; set; }

        public virtual int Type_DZ { set; get; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public virtual int O_DX_Proportion20 { get; set; }
        public virtual int O_DX_Proportion11 { get; set; }
        public virtual int O_DX_Proportion02 { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public virtual int O_JO_Proportion20 { get; set; }
        public virtual int O_JO_Proportion11 { get; set; }
        public virtual int O_JO_Proportion02 { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public virtual int O_ZH_Proportion20 { get; set; }
        public virtual int O_ZH_Proportion11 { get; set; }
        public virtual int O_ZH_Proportion02 { get; set; }
    }
}
