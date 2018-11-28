using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大乐透奇偶走势
    /// </summary>
    public class DLT_JiOu : ImportBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public virtual string QianQu { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public virtual string JiOuQualifying { get; set; }
        /// <summary>
        /// 奇偶比
        /// </summary>
        public virtual string JiOuBi { get; set; }
        /// <summary>
        /// 奇偶遗漏
        /// </summary>
        public virtual int NO1J { get; set; }
        public virtual int NO1O { get; set; }
        public virtual int NO2J { get; set; }
        public virtual int NO2O { get; set; }
        public virtual int NO3J { get; set; }
        public virtual int NO3O { get; set; }
        public virtual int NO4J { get; set; }
        public virtual int NO4O { get; set; }
        public virtual int NO5J { get; set; }
        public virtual int NO5O { get; set; }
        ///
        /// 奇偶比0比5
        /// </summary>
        public virtual int Bi0_5 { get; set; }
        public virtual int Bi1_4 { get; set; }
        public virtual int Bi2_3 { get; set; }
        public virtual int Bi3_2 { get; set; }
        public virtual int Bi4_1 { get; set; }
        public virtual int Bi5_0 { get; set; }

    }
}
