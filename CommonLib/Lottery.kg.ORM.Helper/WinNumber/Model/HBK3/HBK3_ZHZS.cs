using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 综合走势
    /// </summary>
    public class HBK3_ZHZS : ImportBase
    {
        public virtual int Red_1 { get; set; }
        public virtual int Red_2 { get; set; }
        public virtual int Red_3 { get; set; }
        public virtual int Red_4 { get; set; }
        public virtual int Red_5 { get; set; }
        public virtual int Red_6 { get; set; }

        public virtual int RedCan_1 { get; set; }
        public virtual int RedCan_2 { get; set; }
        public virtual int RedCan_3 { get; set; }
        public virtual int RedCan_4 { get; set; }
        public virtual int RedCan_5 { get; set; }
        public virtual int RedCan_6 { get; set; }

        /// <summary>
        ///百位
        /// </summary>
        public virtual int BW_1 { get; set; }
        public virtual int BW_2 { get; set; }
        public virtual int BW_3 { get; set; }
        public virtual int BW_4 { get; set; }
        public virtual int BW_5 { get; set; }
        public virtual int BW_6 { get; set; }

        /// <summary>
        ///十位
        /// </summary>
        public virtual int SW_1 { get; set; }
        public virtual int SW_2 { get; set; }
        public virtual int SW_3 { get; set; }
        public virtual int SW_4 { get; set; }
        public virtual int SW_5 { get; set; }
        public virtual int SW_6 { get; set; }

        /// <summary>
        ///个位
        /// </summary>
        public virtual int GW_1 { get; set; }
        public virtual int GW_2 { get; set; }
        public virtual int GW_3 { get; set; }
        public virtual int GW_4 { get; set; }
        public virtual int GW_5 { get; set; }
        public virtual int GW_6 { get; set; }

    }
}
