using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 重庆2星直选走势
    /// </summary>
    public class JXSSC_2X_ZXZS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }

        public virtual string Type { get; set; }

        public virtual int Red_S0 { get; set; }
        public virtual int Red_S1 { get; set; }
        public virtual int Red_S2 { get; set; }
        public virtual int Red_S3 { get; set; }
        public virtual int Red_S4 { get; set; }
        public virtual int Red_S5 { get; set; }
        public virtual int Red_S6 { get; set; }
        public virtual int Red_S7 { get; set; }
        public virtual int Red_S8 { get; set; }
        public virtual int Red_S9 { get; set; }

        public virtual int Red_G0 { get; set; }
        public virtual int Red_G1 { get; set; }
        public virtual int Red_G2 { get; set; }
        public virtual int Red_G3 { get; set; }
        public virtual int Red_G4 { get; set; }
        public virtual int Red_G5 { get; set; }
        public virtual int Red_G6 { get; set; }
        public virtual int Red_G7 { get; set; }
        public virtual int Red_G8 { get; set; }
        public virtual int Red_G9 { get; set; }

        public virtual int Type_DZ { set; get; }
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
