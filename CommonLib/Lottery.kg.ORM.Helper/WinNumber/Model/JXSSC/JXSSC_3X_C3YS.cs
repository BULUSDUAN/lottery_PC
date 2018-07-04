using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 除3
    /// </summary>
    public class JXSSC_3X_C3YS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        public virtual int O_Red1_0 { get; set; }
        public virtual int O_Red1_1 { get; set; }
        public virtual int O_Red1_2 { get; set; }

        public virtual int O_Red2_0 { get; set; }
        public virtual int O_Red2_1 { get; set; }
        public virtual int O_Red2_2 { get; set; }

        public virtual int O_Red3_0 { get; set; }
        public virtual int O_Red3_1 { get; set; }
        public virtual int O_Red3_2 { get; set; }

        public virtual int Y0_Number { get; set; }
        public virtual int Y1_Number { get; set; }
        public virtual int Y2_Number { get; set; }

        /// <summary>
        /// 余X个数
        /// </summary>
        public virtual int Y0_Number0 { get; set; }
        public virtual int Y0_Number1 { get; set; }
        public virtual int Y0_Number2 { get; set; }
        public virtual int Y0_Number3 { get; set; }

        public virtual int Y1_Number0 { get; set; }
        public virtual int Y1_Number1 { get; set; }
        public virtual int Y1_Number2 { get; set; }
        public virtual int Y1_Number3 { get; set; }

        public virtual int Y2_Number0 { get; set; }
        public virtual int Y2_Number1 { get; set; }
        public virtual int Y2_Number2 { get; set; }
        public virtual int Y2_Number3 { get; set; }
        /// <summary>
        /// 余数比例
        /// </summary>
        public virtual string YS_Proportion { get; set; }
    }
}
