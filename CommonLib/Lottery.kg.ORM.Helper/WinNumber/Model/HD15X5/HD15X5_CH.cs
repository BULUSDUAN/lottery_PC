using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 重号走势
    /// </summary>
    public class HD15X5_CH : ImportBase
    {
        public virtual int Red_01 { get; set; }
        public virtual int Red_02 { get; set; }
        public virtual int Red_03 { get; set; }
        public virtual int Red_04 { get; set; }
        public virtual int Red_05 { get; set; }
        public virtual int Red_06 { get; set; }
        public virtual int Red_07 { get; set; }
        public virtual int Red_08 { get; set; }
        public virtual int Red_09 { get; set; }
        public virtual int Red_10 { get; set; }
        public virtual int Red_11 { get; set; }
        public virtual int Red_12 { get; set; }
        public virtual int Red_13 { get; set; }
        public virtual int Red_14 { get; set; }
        public virtual int Red_15 { get; set; }
        /// <summary>
        /// 重号
        /// </summary>
        public virtual int Duplicate { get; set; }
        public virtual int HeZhi { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Proportion { get; set; }

    }
}
