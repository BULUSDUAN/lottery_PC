using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class QLC_JO : ImportBase
    {
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
        public virtual int NO6J { get; set; }
        public virtual int NO6O { get; set; }
        public virtual int NO7J { get; set; }
        public virtual int NO7O { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public virtual string JiOuQualifying { get; set; }
        /// <summary>
        /// 奇偶比
        /// </summary>
        public virtual string JiOuBi { get; set; }
        /// <summary>
        /// 奇偶比7比0
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
