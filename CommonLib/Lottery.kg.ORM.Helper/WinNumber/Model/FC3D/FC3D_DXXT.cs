using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小形态
    /// </summary>
    public class FC3D_DXXT:ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        /// <summary>
        /// 大小形态
        /// </summary>
        public virtual string Dxxt { get; set; }

        public virtual int DX_Q_D { get; set; }
        public virtual int DX_DDX { get; set; }
        public virtual int DX_DXD { get; set; }
        public virtual int DX_XDD { get; set; }
        public virtual int DX_DXX { get; set; }
        public virtual int DX_XDX { get; set; }
        public virtual int DX_XXD { get; set; }
        public virtual int DX_Q_X { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public virtual string DX_Proportion { get; set; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public virtual int O_DX_Proportion30 { get; set; }
        public virtual int O_DX_Proportion21 { get; set; }
        public virtual int O_DX_Proportion12 { get; set; }
        public virtual int O_DX_Proportion03 { get; set; }
    }
}
