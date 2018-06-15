using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前3直选走势
    /// </summary>
    public class JX11X5_Q3ZS : ImportBase
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
        /// <summary>
        /// 开奖号码第一位（百位）
        /// </summary>
        public virtual int BW_1 { get; set; }
        public virtual int BW_2 { get; set; }
        public virtual int BW_3 { get; set; }
        public virtual int BW_4 { get; set; }
        public virtual int BW_5 { get; set; }
        public virtual int BW_6 { get; set; }
        public virtual int BW_7 { get; set; }
        public virtual int BW_8 { get; set; }
        public virtual int BW_9 { get; set; }
        public virtual int BW_10 { get; set; }
        public virtual int BW_11 { get; set; }

        public virtual int DXBi3_0 { get; set; }
        public virtual int DXBi2_1 { get; set; }
        public virtual int DXBi1_2 { get; set; }
        public virtual int DXBi0_3 { get; set; }

        public virtual int JOBi3_0 { get; set; }
        public virtual int JOBi2_1 { get; set; }
        public virtual int JOBi1_2 { get; set; }
        public virtual int JOBi0_3 { get; set; }


    }
}
