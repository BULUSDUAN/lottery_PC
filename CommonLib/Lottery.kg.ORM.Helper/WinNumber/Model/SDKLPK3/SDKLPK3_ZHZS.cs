using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 质合走势 
    /// </summary>
    public class SDKLPK3_ZHZS : ImportBase
    {
        public virtual int D1_Z { get; set; }
        public virtual int D1_H { get; set; }
        public virtual int D2_Z { get; set; }
        public virtual int D2_H { get; set; }
        public virtual int D3_Z { get; set; }
        public virtual int D3_H { get; set; }

        public virtual int ZHB_30 { get; set; }
        public virtual int ZHB_21 { get; set; }
        public virtual int ZHB_12 { get; set; }
        public virtual int ZHB_03 { get; set; }

        public virtual int ZZZ { get; set; }
        public virtual int ZZH { get; set; }
        public virtual int ZHZ { get; set; }
        public virtual int HZZ { get; set; }
        public virtual int ZHH { get; set; }
        public virtual int HZH { get; set; }
        public virtual int HHZ { get; set; }
        public virtual int HHH { get; set; }
    }
}
