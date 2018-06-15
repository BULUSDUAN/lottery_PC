using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 和值特征
    /// </summary>
    public class FC3D_HZTZ : ImportBase
    {

        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }

        public virtual int DX_HeZhi_D { get; set; }
        public virtual int DX_HeZhi_X { get; set; }

        public virtual int JO_HeZhi_J { get; set; }
        public virtual int JO_HeZhi_O { get; set; }

        public virtual int ZH_HeZhi_Z { get; set; }
        public virtual int ZH_HeZhi_H { get; set; }

        public virtual int QS_HeZhi_Sheng { get; set; }
        public virtual int QS_HeZhi_Ping { get; set; }
        public virtual int QS_HeZhi_Jiang { get; set; }

        public virtual int O_C4_Y0 { set; get; }
        public virtual int O_C4_Y1 { set; get; }
        public virtual int O_C4_Y2 { set; get; }
        public virtual int O_C4_Y3 { set; get; }

        public virtual int O_C3_Y0 { set; get; }
        public virtual int O_C3_Y1 { set; get; }
        public virtual int O_C3_Y2 { set; get; }

        public virtual int O_C5_Y0 { set; get; }
        public virtual int O_C5_Y1 { set; get; }
        public virtual int O_C5_Y2 { set; get; }
        public virtual int O_C5_Y3 { set; get; }
        public virtual int O_C5_Y4 { set; get; }
    }
}
