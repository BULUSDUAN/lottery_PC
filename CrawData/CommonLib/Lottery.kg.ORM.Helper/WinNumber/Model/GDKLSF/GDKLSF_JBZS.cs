using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    public class GDKLSF_JBZS : ImportBase
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
        public virtual int Red_16 { get; set; }
        public virtual int Red_17 { get; set; }
        public virtual int Red_18 { get; set; }
        public virtual int Red_19 { get; set; }
        public virtual int Red_20 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }
        /// <summary>
        /// 合尾
        /// </summary>
        public virtual int HeWei { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public virtual string JO_Bi { get; set; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Bi { get; set; }
        /// <summary>
        /// 质数
        /// </summary>
        public virtual int ZhiCount { get; set; }

    }
}
