﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大乐透和值走势
    /// </summary>
    public class DLT_HeZhi : ImportBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public virtual string QianQu { get; set; }
        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }
        /// <summary>
        /// 和尾
        /// </summary>
        public virtual int HeWei { get; set; }
        /// <summary>
        /// 和值分布
        /// </summary>
        public virtual int HZ_15_49 { get; set; }
        public virtual int HZ_50_59 { get; set; }
        public virtual int HZ_60_69 { get; set; }
        public virtual int HZ_70_79 { get; set; }
        public virtual int HZ_80_89 { get; set; }
        public virtual int HZ_90_99 { get; set; }
        public virtual int HZ_100_109 { get; set; }
        public virtual int HZ_110_119 { get; set; }
        public virtual int HZ_120_129 { get; set; }
        public virtual int HZ_130_165 { get; set; }
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
