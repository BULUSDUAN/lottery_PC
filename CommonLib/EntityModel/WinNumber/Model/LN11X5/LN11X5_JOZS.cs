using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class LN11X5_JOZS : ImportBase
    {
        public virtual int J_Red1 { get; set; }
        public virtual int O_Red1 { get; set; }
        public virtual int J_Red2 { get; set; }
        public virtual int O_Red2 { get; set; }
        public virtual int J_Red3 { get; set; }
        public virtual int O_Red3 { get; set; }
        public virtual int J_Red4 { get; set; }
        public virtual int O_Red4 { get; set; }
        public virtual int J_Red5 { get; set; }
        public virtual int O_Red5 { get; set; }

        public virtual int JO_Q_J { get; set; }
        public virtual int JO_1J4O { get; set; }
        public virtual int JO_2J3O { get; set; }
        public virtual int JO_3J2O { get; set; }
        public virtual int JO_4J1O { get; set; }
        public virtual int JO_Q_O { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string JO_Proportion { get; set; }
    }
}
