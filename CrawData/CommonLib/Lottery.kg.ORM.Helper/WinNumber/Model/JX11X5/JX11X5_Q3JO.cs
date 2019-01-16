using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前3奇偶走势
    /// </summary>
    public class JX11X5_Q3JO : ImportBase
    {
        /// <summary>
        ///位置奇偶
        /// </summary>
        public virtual int NO1_J { get; set; }
        public virtual int NO1_O { get; set; }
        public virtual int NO2_J { get; set; }
        public virtual int NO2_O { get; set; }
        public virtual int NO3_J { get; set; }
        public virtual int NO3_O { get; set; }
        /// <summary>
        /// 奇偶比
        /// </summary>
        public virtual int Bi3_0 { get; set; }
        public virtual int Bi2_1 { get; set; }
        public virtual int Bi1_2 { get; set; }
        public virtual int Bi0_3 { get; set; }
        /// <summary>
        /// 奇奇奇
        /// </summary>
        public virtual int JO_JJJ { get; set; }
        public virtual int JO_JJO { get; set; }
        public virtual int JO_JOJ { get; set; }
        public virtual int JO_OJJ { get; set; }
        public virtual int JO_JOO { get; set; }
        public virtual int JO_OJO { get; set; }
        public virtual int JO_OOJ { get; set; }
        public virtual int JO_OOO { get; set; }

    }
}
