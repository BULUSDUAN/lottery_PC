using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 除3余数
    /// </summary>
    public class SSQ_C3 : ImportBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public virtual string RedLotteryNumber { get; set; }

        public virtual string Red1 { get; set; }
        public virtual string Red2 { get; set; }
        public virtual string Red3 { get; set; }
        public virtual string Red4 { get; set; }
        public virtual string Red5 { get; set; }
        public virtual string Red6 { get; set; }

        public virtual int O_Red1_0 { get; set; }
        public virtual int O_Red2_0 { get; set; }
        public virtual int O_Red3_0 { get; set; }
        public virtual int O_Red4_0 { get; set; }
        public virtual int O_Red5_0 { get; set; }
        public virtual int O_Red6_0 { get; set; }

        public virtual int O_Red1_1 { get; set; }
        public virtual int O_Red2_1 { get; set; }
        public virtual int O_Red3_1 { get; set; }
        public virtual int O_Red4_1 { get; set; }
        public virtual int O_Red5_1 { get; set; }
        public virtual int O_Red6_1 { get; set; }

        public virtual int O_Red1_2 { get; set; }
        public virtual int O_Red2_2 { get; set; }
        public virtual int O_Red3_2 { get; set; }
        public virtual int O_Red4_2 { get; set; }
        public virtual int O_Red5_2 { get; set; }
        public virtual int O_Red6_2 { get; set; }

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
        public virtual int Y0_Number4 { get; set; }
        public virtual int Y0_Number5 { get; set; }
        public virtual int Y0_Number6 { get; set; }

        public virtual int Y1_Number0 { get; set; }
        public virtual int Y1_Number1 { get; set; }
        public virtual int Y1_Number2 { get; set; }
        public virtual int Y1_Number3 { get; set; }
        public virtual int Y1_Number4 { get; set; }
        public virtual int Y1_Number5 { get; set; }
        public virtual int Y1_Number6 { get; set; }

        public virtual int Y2_Number0 { get; set; }
        public virtual int Y2_Number1 { get; set; }
        public virtual int Y2_Number2 { get; set; }
        public virtual int Y2_Number3 { get; set; }
        public virtual int Y2_Number4 { get; set; }
        public virtual int Y2_Number5 { get; set; }
        public virtual int Y2_Number6 { get; set; }
        /// <summary>
        /// 余数排位
        /// </summary>
        public virtual string YS_Qualifying { get; set; }
        /// <summary>
        /// 余数比例
        /// </summary>
        public virtual string YS_Proportion { get; set; }
    }
}
