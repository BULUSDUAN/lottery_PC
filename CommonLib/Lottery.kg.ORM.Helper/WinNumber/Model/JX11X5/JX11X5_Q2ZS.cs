using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前2直选走势
    /// </summary>
    public class JX11X5_Q2ZS : ImportBase
    {
        /// <summary>
        /// 开奖号码第一位（万位）
        /// </summary>
        public virtual int WW_1 { get; set; }
        public virtual int WW_2 { get; set; }
        public virtual int WW_3 { get; set; }
        public virtual int WW_4 { get; set; }
        public virtual int WW_5 { get; set; }
        public virtual int WW_6 { get; set; }
        public virtual int WW_7 { get; set; }
        public virtual int WW_8 { get; set; }
        public virtual int WW_9 { get; set; }
        public virtual int WW_10 { get; set; }
        public virtual int WW_11 { get; set; }
        /// <summary>
        /// 开奖号码第一位（千位）
        /// </summary>
        public virtual int QW_1 { get; set; }
        public virtual int QW_2 { get; set; }
        public virtual int QW_3 { get; set; }
        public virtual int QW_4 { get; set; }
        public virtual int QW_5 { get; set; }
        public virtual int QW_6 { get; set; }
        public virtual int QW_7 { get; set; }
        public virtual int QW_8 { get; set; }
        public virtual int QW_9 { get; set; }
        public virtual int QW_10 { get; set; }
        public virtual int QW_11 { get; set; }

        public virtual int DXBi2_0 { get; set; }
        public virtual int DXBi1_1 { get; set; }
        public virtual int DXBi0_2 { get; set; }

        public virtual int JOBi2_0 { get; set; }
        public virtual int JOBi1_1 { get; set; }
        public virtual int JOBi0_2 { get; set; }
    }
}
