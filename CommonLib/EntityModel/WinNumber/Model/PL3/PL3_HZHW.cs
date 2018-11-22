using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 和值合尾
    /// </summary>
    public class PL3_HZHW : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }

        public virtual int AVG_0 { get; set; }
        public virtual int AVG_1 { get; set; }
        public virtual int AVG_2 { get; set; }
        public virtual int AVG_3 { get; set; }
        public virtual int AVG_4 { get; set; }
        public virtual int AVG_5 { get; set; }
        public virtual int AVG_6 { get; set; }
        public virtual int AVG_7 { get; set; }
        public virtual int AVG_8 { get; set; }
        public virtual int AVG_9 { get; set; }

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
