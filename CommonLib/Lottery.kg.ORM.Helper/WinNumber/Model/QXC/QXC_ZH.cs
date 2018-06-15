using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质和走势
    /// </summary>
    public class QXC_ZH : ImportBase
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
        /// <summary>
        /// 质和比
        /// </summary>
        public virtual string ZhiHeBi { get; set; }
        /// <summary>
        /// 质和比7比0
        /// </summary>
        public virtual int Bi7_0 { get; set; }
        public virtual int Bi6_1 { get; set; }
        public virtual int Bi5_2 { get; set; }
        public virtual int Bi4_3 { get; set; }
        public virtual int Bi3_4 { get; set; }
        public virtual int Bi2_5 { get; set; }
        public virtual int Bi1_6 { get; set; }
        public virtual int Bi0_7 { get; set; }

    }
}
