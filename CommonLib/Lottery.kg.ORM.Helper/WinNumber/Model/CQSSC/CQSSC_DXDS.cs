using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小单双
    /// </summary>
    public class CQSSC_DXDS : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }

        public virtual int D_Red_S { get; set; }
        public virtual int X_Red_S { get; set; }
        public virtual int Dan_Red_S { get; set; }
        public virtual int S_Red_S { get; set; }
        public virtual int D_Red_G { get; set; }
        public virtual int X_Red_G { get; set; }
        public virtual int Dan_Red_G { get; set; }
        public virtual int S_Red_G { get; set; }

        public virtual int DD { get; set; }
        public virtual int DX { get; set; }
        public virtual int DDan { get; set; }
        public virtual int DS { get; set; }
        public virtual int XD { get; set; }
        public virtual int XX { get; set; }
        public virtual int XDan { get; set; }
        public virtual int XS { get; set; }
        public virtual int DanD { get; set; }
        public virtual int DanX { get; set; }
        public virtual int DanDan { get; set; }
        public virtual int DanS { get; set; }
        public virtual int SD { get; set; }
        public virtual int SX { get; set; }
        public virtual int SDan { get; set; }
        public virtual int SS { get; set; }
    }
}
