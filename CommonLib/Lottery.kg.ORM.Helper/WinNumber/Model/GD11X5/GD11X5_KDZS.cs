using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 跨度走势
    /// </summary>
    public class GD11X5_KDZS : ImportBase
    {
        public virtual int KD_4 { get; set; }
        public virtual int KD_5 { get; set; }
        public virtual int KD_6 { get; set; }
        public virtual int KD_7 { get; set; }
        public virtual int KD_8 { get; set; }
        public virtual int KD_9 { get; set; }
        public virtual int KD_10 { get; set; }


        public virtual int KuaDu { get; set; }

        public virtual int JO_J { get; set; }
        public virtual int JO_O { get; set; }
        public virtual int ZH_Z { get; set; }
        public virtual int ZH_H { get; set; }


        public virtual int C3_0 { get; set; }
        public virtual int C3_1 { get; set; }
        public virtual int C3_2 { get; set; }
    }
}
