using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 和值走势
    /// </summary>
    public class JXSSC_2X_HZZS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }

        public virtual int HeZhi { set; get; }

        public virtual int HeWei { set; get; }

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
