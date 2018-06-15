using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 任选基本走势
    /// </summary>
    public class JX11X5_RX3 : ImportBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public virtual int NO_1 { get; set; }
        public virtual int NO_2 { get; set; }
        public virtual int NO_3 { get; set; }
        public virtual int NO_4 { get; set; }
        public virtual int NO_5 { get; set; }
        public virtual int NO_6 { get; set; }
        public virtual int NO_7 { get; set; }
        public virtual int NO_8 { get; set; }
        public virtual int NO_9 { get; set; }
        public virtual int NO_10 { get; set; }
        public virtual int NO_11 { get; set; }

        public virtual int DX_D { get; set; }
        public virtual int DX_X { get; set; }

        public virtual int JO_J { get; set; }
        public virtual int JO_O { get; set; }

        public virtual int ZH_Z { get; set; }
        public virtual int ZH_H { get; set; }

        public virtual int Yu_0 { get; set; }
        public virtual int Yu_1 { get; set; }
        public virtual int Yu_2 { get; set; }
    }
}
