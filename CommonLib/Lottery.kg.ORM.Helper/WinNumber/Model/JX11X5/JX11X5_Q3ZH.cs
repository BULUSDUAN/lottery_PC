using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前3质和走势
    /// </summary>
    public class JX11X5_Q3ZH : ImportBase
    {
        /// <summary>
        ///位置质和
        /// </summary>
        public virtual int NO1_Z { get; set; }
        public virtual int NO1_H { get; set; }
        public virtual int NO2_Z { get; set; }
        public virtual int NO2_H { get; set; }
        public virtual int NO3_Z { get; set; }
        public virtual int NO3_H { get; set; }
        /// <summary>
        /// 质和比
        /// </summary>
        public virtual int Bi3_0 { get; set; }
        public virtual int Bi2_1 { get; set; }
        public virtual int Bi1_2 { get; set; }
        public virtual int Bi0_3 { get; set; }
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

    }
}
