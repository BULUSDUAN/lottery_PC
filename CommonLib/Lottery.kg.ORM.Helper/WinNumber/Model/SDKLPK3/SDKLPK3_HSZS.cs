using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 花色走势
    /// </summary>
    public class SDKLPK3_HSZS : ImportBase
    {
        public virtual int D1_1 { get; set; }
        public virtual int D1_2 { get; set; }
        public virtual int D1_3 { get; set; }
        public virtual int D1_4 { get; set; }

        public virtual int D2_1 { get; set; }
        public virtual int D2_2 { get; set; }
        public virtual int D2_3 { get; set; }
        public virtual int D2_4 { get; set; }

        public virtual int D3_1 { get; set; }
        public virtual int D3_2 { get; set; }
        public virtual int D3_3 { get; set; }
        public virtual int D3_4 { get; set; }

        public virtual int TH { get; set; }
        public virtual int THS { get; set; }
        public virtual int SZ { get; set; }
    }
}
