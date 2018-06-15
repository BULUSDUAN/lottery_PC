using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小走势
    /// </summary>
    public class DF6_1_DXZS : ImportBase
    {
        public virtual int DX_D { get; set; }
        public virtual int DX_X { get; set; }

        public virtual int DX1_D { get; set; }
        public virtual int DX1_X { get; set; }

        public virtual int DX2_D { get; set; }
        public virtual int DX2_X { get; set; }

        public virtual int DX3_D { get; set; }
        public virtual int DX3_X { get; set; }

        public virtual int DX4_D { get; set; }
        public virtual int DX4_X { get; set; }

        public virtual int DX5_D { get; set; }
        public virtual int DX5_X { get; set; }

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
