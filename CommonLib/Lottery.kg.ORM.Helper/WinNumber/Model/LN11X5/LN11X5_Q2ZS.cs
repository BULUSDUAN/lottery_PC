using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前2走势
    /// </summary>
    public class LN11X5_Q2ZS : ImportBase
    {
        public virtual int DDan { get; set; }
        public virtual int DS { get; set; }
        public virtual int XDan { get; set; }
        public virtual int XS { get; set; }

        public virtual int C3_0 { get; set; }
        public virtual int C3_1 { get; set; }
        public virtual int C3_2 { get; set; }

        public virtual int DDan1 { get; set; }
        public virtual int DS1 { get; set; }
        public virtual int XDan1 { get; set; }
        public virtual int XS1 { get; set; }

        public virtual int C31_0 { get; set; }
        public virtual int C31_1 { get; set; }
        public virtual int C31_2 { get; set; }
    }
}
