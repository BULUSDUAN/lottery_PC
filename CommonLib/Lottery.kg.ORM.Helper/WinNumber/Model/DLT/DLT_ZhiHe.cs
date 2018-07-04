using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大乐透质合走势
    /// </summary>
    public class DLT_ZhiHe : ImportBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public virtual string QianQu { get; set; }
        /// <summary>
        /// 质合排位
        /// </summary>
        public virtual string ZhiHeQualifying { get; set; }
        /// <summary>
        /// 质合比
        /// </summary>
        public virtual string ZhiHeBi { get; set; }
        /// <summary>
        /// 质合遗漏
        /// </summary>
        public virtual int NO1Z { get; set; }
        public virtual int NO1H { get; set; }
        public virtual int NO2Z { get; set; }
        public virtual int NO2H { get; set; }
        public virtual int NO3Z { get; set; }
        public virtual int NO3H { get; set; }
        public virtual int NO4Z { get; set; }
        public virtual int NO4H { get; set; }
        public virtual int NO5Z { get; set; }
        public virtual int NO5H { get; set; }
        /// <summary>
        /// 质合比0比5
        /// </summary> 
        public virtual int Bi0_5 { get; set; }
        public virtual int Bi1_4 { get; set; }
        public virtual int Bi2_3 { get; set; }
        public virtual int Bi3_2 { get; set; }
        public virtual int Bi4_1 { get; set; }
        public virtual int Bi5_0 { get; set; }
    }
}
