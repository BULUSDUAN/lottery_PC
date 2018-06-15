using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小走势
    /// </summary>
    public class LN11X5_DXZS : ImportBase
    {

        public virtual int D_Red1 { get; set; }
        public virtual int X_Red1 { get; set; }
        public virtual int D_Red2 { get; set; }
        public virtual int X_Red2 { get; set; }
        public virtual int D_Red3 { get; set; }
        public virtual int X_Red3 { get; set; }
        public virtual int D_Red4 { get; set; }
        public virtual int X_Red4 { get; set; }
        public virtual int D_Red5 { get; set; }
        public virtual int X_Red5 { get; set; }


        public virtual int DX_Q_D { get; set; }
        public virtual int DX_1D4X { get; set; }
        public virtual int DX_2D3X { get; set; }
        public virtual int DX_3D2X { get; set; }
        public virtual int DX_4D1X { get; set; }
        public virtual int DX_Q_X { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Proportion { get; set; }
    }
}
