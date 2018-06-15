using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 任选基本走势
    /// </summary>
    public class SDQYH_RXDX : ImportBase
    {
        /// <summary> 
        /// 任选大小走势分布
        /// </summary>
        public virtual int NO1_D { get; set; }
        public virtual int NO1_X { get; set; }
        public virtual int NO2_D { get; set; }
        public virtual int NO2_X { get; set; }
        public virtual int NO3_D { get; set; }
        public virtual int NO3_X { get; set; }
        public virtual int NO4_D { get; set; }
        public virtual int NO4_X { get; set; }
        public virtual int NO5_D { get; set; }
        public virtual int NO5_X { get; set; }

        public virtual string DXqualifying { get; set; }
        public virtual string DaoXiaoBi { get; set; }

        public virtual int Bi5_0 { get; set; }
        public virtual int Bi4_1 { get; set; }
        public virtual int Bi3_2 { get; set; }
        public virtual int Bi2_3 { get; set; }
        public virtual int Bi1_4 { get; set; }
        public virtual int Bi0_5 { get; set; }

    }
}
