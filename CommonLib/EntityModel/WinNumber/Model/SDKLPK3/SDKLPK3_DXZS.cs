using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小走势
    /// </summary>
    public class SDKLPK3_DXZS : ImportBase
    {
        public virtual int D1_D { get; set; }
        public virtual int D1_X { get; set; }
        public virtual int D2_D { get; set; }
        public virtual int D2_X { get; set; }
        public virtual int D3_D { get; set; }
        public virtual int D3_X { get; set; }

        public virtual int DXB_30 { get; set; }
        public virtual int DXB_21 { get; set; }
        public virtual int DXB_12 { get; set; }
        public virtual int DXB_03 { get; set; }

        public virtual int DDD { get; set; }
        public virtual int DDX { get; set; }
        public virtual int DXD { get; set; }
        public virtual int XDD { get; set; }
        public virtual int DXX { get; set; }
        public virtual int XDX { get; set; }
        public virtual int XXD { get; set; }
        public virtual int XXX { get; set; }
    }
}
