using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 和值分布
    /// </summary>
    public class FC3D_HZFB : ImportBase
    {

        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }
        /// <summary>
        /// 和尾
        /// </summary>
        public virtual int HeWei { get; set; }


        public virtual int HZ_0 { get; set; }
        public virtual int HZ_1 { get; set; }
        public virtual int HZ_2 { get; set; }
        public virtual int HZ_3 { get; set; }
        public virtual int HZ_4 { get; set; }
        public virtual int HZ_5 { get; set; }
        public virtual int HZ_6 { get; set; }
        public virtual int HZ_7 { get; set; }
        public virtual int HZ_8 { get; set; }
        public virtual int HZ_9 { get; set; }
        public virtual int HZ_10 { get; set; }
        public virtual int HZ_11 { get; set; }
        public virtual int HZ_12 { get; set; }
        public virtual int HZ_13 { get; set; }
        public virtual int HZ_14 { get; set; }
        public virtual int HZ_15 { get; set; }
        public virtual int HZ_16 { get; set; }
        public virtual int HZ_17 { get; set; }
        public virtual int HZ_18 { get; set; }
        public virtual int HZ_19 { get; set; }
        public virtual int HZ_20 { get; set; }
        public virtual int HZ_21 { get; set; }
        public virtual int HZ_22 { get; set; }
        public virtual int HZ_23 { get; set; }
        public virtual int HZ_24 { get; set; }
        public virtual int HZ_25 { get; set; }
        public virtual int HZ_26 { get; set; }
        public virtual int HZ_27 { get; set; }

        /// <summary>
        /// 和尾分布
        /// </summary> 
        public virtual int HW_0 { get; set; }
        public virtual int HW_1 { get; set; }
        public virtual int HW_2 { get; set; }
        public virtual int HW_3 { get; set; }
        public virtual int HW_4 { get; set; }
        public virtual int HW_5 { get; set; }
        public virtual int HW_6 { get; set; }
        public virtual int HW_7 { get; set; }
        public virtual int HW_8 { get; set; }
        public virtual int HW_9 { get; set; }
    }
}
