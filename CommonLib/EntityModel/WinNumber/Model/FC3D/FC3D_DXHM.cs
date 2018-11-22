using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小号码
    /// </summary>
    public class FC3D_DXHM:ImportBase
    {

        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        public virtual int Red_B0 { get; set; }
        public virtual int Red_B1 { get; set; }
        public virtual int Red_B2 { get; set; }
        public virtual int Red_B3 { get; set; }
        public virtual int Red_B4 { get; set; }
        public virtual int Red_B5 { get; set; }
        public virtual int Red_B6 { get; set; }
        public virtual int Red_B7 { get; set; }
        public virtual int Red_B8 { get; set; }
        public virtual int Red_B9 { get; set; }

        public virtual int Red_S0 { get; set; }
        public virtual int Red_S1 { get; set; }
        public virtual int Red_S2 { get; set; }
        public virtual int Red_S3 { get; set; }
        public virtual int Red_S4 { get; set; }
        public virtual int Red_S5 { get; set; }
        public virtual int Red_S6 { get; set; }
        public virtual int Red_S7 { get; set; }
        public virtual int Red_S8 { get; set; }
        public virtual int Red_S9 { get; set; }

        public virtual int Red_G0 { get; set; }
        public virtual int Red_G1 { get; set; }
        public virtual int Red_G2 { get; set; }
        public virtual int Red_G3 { get; set; }
        public virtual int Red_G4 { get; set; }
        public virtual int Red_G5 { get; set; }
        public virtual int Red_G6 { get; set; }
        public virtual int Red_G7 { get; set; }
        public virtual int Red_G8 { get; set; }
        public virtual int Red_G9 { get; set; }

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
