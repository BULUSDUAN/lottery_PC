using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 前2形态走势
    /// </summary>
    public class GD11X5_Q2XTZS : ImportBase
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

        public virtual int DX_DD { set; get; }
        public virtual int DX_DX { set; get; }
        public virtual int DX_XD { set; get; }
        public virtual int DX_XX { set; get; }

        public virtual int DS_DD { set; get; }
        public virtual int DS_DS { set; get; }
        public virtual int DS_SD { set; get; }
        public virtual int DS_SS { set; get; }
    }
}
