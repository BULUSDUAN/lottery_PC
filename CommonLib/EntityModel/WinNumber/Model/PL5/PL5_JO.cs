using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class PL5_JO : ImportBase
    {
        /// <summary> 
        /// 奇偶走势分布
        /// </summary>
        public virtual int NO1_J { get; set; }
        public virtual int NO1_O { get; set; }
        public virtual int NO2_J { get; set; }
        public virtual int NO2_O { get; set; }
        public virtual int NO3_J { get; set; }
        public virtual int NO3_O { get; set; }
        public virtual int NO4_J { get; set; }
        public virtual int NO4_O { get; set; }
        public virtual int NO5_J { get; set; }
        public virtual int NO5_O { get; set; }

        public virtual string JOqualifying { get; set; }
        public virtual string JiOuBi { get; set; }

        public virtual int Bi5_0 { get; set; }
        public virtual int Bi4_1 { get; set; }
        public virtual int Bi3_2 { get; set; }
        public virtual int Bi2_3 { get; set; }
        public virtual int Bi1_4 { get; set; }
        public virtual int Bi0_5 { get; set; }

    }
}
