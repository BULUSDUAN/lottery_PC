using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 除3 1走势
    /// </summary>
    public class PL3_Chu31 : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

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
        /// 012比例
        /// </summary>
        public virtual string P012_Proportion { set; get; }


        public virtual int O_P012_Proportion300 { set; get; }
        public virtual int O_P012_Proportion210 { set; get; }
        public virtual int O_P012_Proportion201 { set; get; }
        public virtual int O_P012_Proportion120 { set; get; }
        public virtual int O_P012_Proportion111 { set; get; }
        public virtual int O_P012_Proportion102 { set; get; }
        public virtual int O_P012_Proportion030 { set; get; }
        public virtual int O_P012_Proportion021 { set; get; }
        public virtual int O_P012_Proportion012 { set; get; }
        public virtual int O_P012_Proportion003 { set; get; }
    }
}
