using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 和值
    /// </summary>
    public class SSQ_HeZhi:ImportBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public virtual string RedLotteryNumber { get; set; }
        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }
        /// <summary>
        /// 和尾
        /// </summary>
        public virtual int HeWei { get; set; }

        public virtual string Red1 { get; set; }
        public virtual string Red2 { get; set; }
        public virtual string Red3 { get; set; }
        public virtual string Red4 { get; set; }
        public virtual string Red5 { get; set; }
        public virtual string Red6 { get; set; }
        /// <summary>
        /// 和值分布
        /// </summary>
        public virtual int HZ_21_49 { get; set; }
        public virtual int HZ_50_59 { get; set; }
        public virtual int HZ_60_69 { get; set; }
        public virtual int HZ_70_79 { get; set; }
        public virtual int HZ_80_89 { get; set; }
        public virtual int HZ_90_99 { get; set; }
        public virtual int HZ_100_109 { get; set; }
        public virtual int HZ_110_119 { get; set; }
        public virtual int HZ_120_129 { get; set; }
        public virtual int HZ_130_139 { get; set; }
        public virtual int HZ_140_183 { get; set; }
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
