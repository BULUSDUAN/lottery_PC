using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质和走势
    /// </summary>
    public class PL3_ZhiHe : ImportBase
    {
        /// <summary>
        /// 质和形态
        /// </summary>
        public virtual string ZHType { get; set; }
        /// <summary>
        /// 质质质
        /// </summary>
        public virtual int ZH_ZZZ { get; set; }
        public virtual int ZH_ZZH { get; set; }
        public virtual int ZH_ZHZ { get; set; }
        public virtual int ZH_HZZ { get; set; }
        public virtual int ZH_ZHH { get; set; }
        public virtual int ZH_HZH { get; set; }
        public virtual int ZH_HHZ { get; set; }
        public virtual int ZH_HHH { get; set; }
        /// <summary> 
        /// 质和比
        /// </summary>
        public virtual string ZHBi { get; set; }
        /// <summary>
        /// 质和比0比5 
        /// </summary>
        public virtual int Bi3_0 { get; set; }
        public virtual int Bi2_1 { get; set; }
        public virtual int Bi1_2 { get; set; }
        public virtual int Bi0_3 { get; set; }
    }
}
