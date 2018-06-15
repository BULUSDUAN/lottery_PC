using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 重号走势
    /// </summary>
    public class YDJ11_CHZS : ImportBase
    {
        public virtual int Red_01 { get; set; }
        public virtual int Red_02 { get; set; }
        public virtual int Red_03 { get; set; }
        public virtual int Red_04 { get; set; }
        public virtual int Red_05 { get; set; }
        public virtual int Red_06 { get; set; }
        public virtual int Red_07 { get; set; }
        public virtual int Red_08 { get; set; }
        public virtual int Red_09 { get; set; }
        public virtual int Red_10 { get; set; }
        public virtual int Red_11 { get; set; }

        /// <summary>
        /// 重号
        /// </summary>
        public virtual int Duplicate { get; set; }

        public virtual int D_0 { get; set; }
        public virtual int D_1 { get; set; }
        public virtual int D_2 { get; set; }
        public virtual int D_3 { get; set; }
        public virtual int D_4 { get; set; }
        public virtual int D_5 { get; set; }

        public virtual int D1_01_04 { get; set; }
        public virtual int D2_05_08 { get; set; }
        public virtual int D3_09_11 { get; set; }

    }
}
