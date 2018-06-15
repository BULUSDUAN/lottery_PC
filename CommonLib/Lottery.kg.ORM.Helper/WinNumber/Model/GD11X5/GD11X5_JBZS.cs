using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    public class GD11X5_JBZS:ImportBase
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

        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Proportion { get; set; }
        /// <summary>
        /// 重号
        /// </summary>
        public virtual int Duplicate { get; set; }
        /// <summary>
        /// 连号个数
        /// </summary>
        public virtual int ContinuousNumber { get; set; }
    }
}
