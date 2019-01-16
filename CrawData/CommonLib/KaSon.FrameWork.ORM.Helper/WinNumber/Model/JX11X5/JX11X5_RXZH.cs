using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 任选质和走势
    /// </summary>
    public class JX11X5_RXZH : ImportBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public virtual int NO1_Z { get; set; }
        public virtual int NO1_H { get; set; }
        public virtual int NO2_Z { get; set; }
        public virtual int NO2_H { get; set; }
        public virtual int NO3_Z { get; set; }
        public virtual int NO3_H { get; set; }
        public virtual int NO4_Z { get; set; }
        public virtual int NO4_H { get; set; }
        public virtual int NO5_Z { get; set; }
        public virtual int NO5_H { get; set; }

        public virtual string ZHqualifying { get; set; }
        public virtual string ZhiHeBi { get; set; }

        public virtual int Bi5_0 { get; set; }
        public virtual int Bi4_1 { get; set; }
        public virtual int Bi3_2 { get; set; }
        public virtual int Bi2_3 { get; set; }
        public virtual int Bi1_4 { get; set; }
        public virtual int Bi0_5 { get; set; }

    }
}
