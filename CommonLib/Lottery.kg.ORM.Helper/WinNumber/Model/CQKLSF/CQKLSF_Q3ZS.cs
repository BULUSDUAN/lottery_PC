using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前3走势
    /// </summary>
    public class CQKLSF_Q3ZS : ImportBase
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

        public virtual int DDan2 { get; set; }
        public virtual int DS2 { get; set; }
        public virtual int XDan2 { get; set; }
        public virtual int XS2 { get; set; }

        public virtual int C32_0 { get; set; }
        public virtual int C32_1 { get; set; }
        public virtual int C32_2 { get; set; }
    }
}
