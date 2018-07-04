using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 任选基本走势
    /// </summary>
    public class JX11X5_RXJBZS : ImportBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public virtual int RXJB_01 { get; set; }
        public virtual int RXJB_02 { get; set; }
        public virtual int RXJB_03 { get; set; }
        public virtual int RXJB_04 { get; set; }
        public virtual int RXJB_05 { get; set; }
        public virtual int RXJB_06 { get; set; }
        public virtual int RXJB_07 { get; set; }
        public virtual int RXJB_08 { get; set; }
        public virtual int RXJB_09 { get; set; }
        public virtual int RXJB_10 { get; set; }
        public virtual int RXJB_11 { get; set; }

        /// <summary>
        /// 遗漏奇数的个数
        /// </summary>
        public virtual int J_0 { get; set; }
        public virtual int J_1 { get; set; }
        public virtual int J_2 { get; set; }
        public virtual int J_3 { get; set; }
        public virtual int J_4 { get; set; }
        public virtual int J_5 { get; set; }

        /// <summary>
        /// 遗漏小的个数
        /// </summary>
        public virtual int X_0 { get; set; }
        public virtual int X_1 { get; set; }
        public virtual int X_2 { get; set; }
        public virtual int X_3 { get; set; }
        public virtual int X_4 { get; set; }
        public virtual int X_5 { get; set; }

        /// <summary>
        /// 遗漏质数的个数
        /// </summary>
        public virtual int Z_0 { get; set; }
        public virtual int Z_1 { get; set; }
        public virtual int Z_2 { get; set; }
        public virtual int Z_3 { get; set; }
        public virtual int Z_4 { get; set; }
        public virtual int Z_5 { get; set; }

    }
}
