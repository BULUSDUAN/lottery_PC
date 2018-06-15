using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 和值走势
    /// </summary>
    public class JLK3_HZ : ImportBase
    {
        /// <summary>
        /// 和值
        /// </summary>
        public virtual int He_3 { get; set; }
        public virtual int He_4 { get; set; }
        public virtual int He_5 { get; set; }
        public virtual int He_6 { get; set; }
        public virtual int He_7 { get; set; }
        public virtual int He_8 { get; set; }
        public virtual int He_9 { get; set; }
        public virtual int He_10 { get; set; }
        public virtual int He_11 { get; set; }
        public virtual int He_12 { get; set; }
        public virtual int He_13 { get; set; }
        public virtual int He_14 { get; set; }
        public virtual int He_15 { get; set; }
        public virtual int He_16 { get; set; }
        public virtual int He_17 { get; set; }
        public virtual int He_18 { get; set; }
        /// <summary>
        /// 合尾
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

        /// <summary>
        ///除3余数
        /// </summary>
        public virtual int Yu_0 { get; set; }
        public virtual int Yu_1 { get; set; }
        public virtual int Yu_2 { get; set; }
    }
}
