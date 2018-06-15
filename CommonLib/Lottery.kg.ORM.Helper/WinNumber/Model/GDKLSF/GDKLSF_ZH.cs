using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质和走势
    /// </summary>
    public class GDKLSF_ZH : ImportBase
    {
        /// <summary>
        /// 质和遗漏
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
        public virtual int NO6Z { get; set; }
        public virtual int NO6H { get; set; }
        public virtual int NO7Z { get; set; }
        public virtual int NO7H { get; set; }
        public virtual int NO8Z { get; set; }
        public virtual int NO8H { get; set; }
        /// <summary>
        /// 质和比8比0
        /// </summary>
        public virtual int Bi8_0 { get; set; }
        public virtual int Bi7_1 { get; set; }
        public virtual int Bi6_2 { get; set; }
        public virtual int Bi5_3 { get; set; }
        public virtual int Bi4_4 { get; set; }
        public virtual int Bi3_5 { get; set; }
        public virtual int Bi2_6 { get; set; }
        public virtual int Bi1_7 { get; set; }
        public virtual int Bi0_8 { get; set; }

    }
}
