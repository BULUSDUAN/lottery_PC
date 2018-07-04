using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 组合走势
    /// </summary>
    public class JLK3_ZH : ImportBase
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

        public virtual int AA11 { get; set; }
        public virtual int AA22 { get; set; }
        public virtual int AA33 { get; set; }
        public virtual int AA44 { get; set; }
        public virtual int AA55 { get; set; }
        public virtual int AA66 { get; set; }


        public virtual int AB12 { get; set; }
        public virtual int AB13 { get; set; }
        public virtual int AB14 { get; set; }
        public virtual int AB15 { get; set; }
        public virtual int AB16 { get; set; }
        public virtual int AB23 { get; set; }
        public virtual int AB24 { get; set; }
        public virtual int AB25 { get; set; }
        public virtual int AB26 { get; set; }
        public virtual int AB34 { get; set; }
        public virtual int AB35 { get; set; }
        public virtual int AB36 { get; set; }
        public virtual int AB45 { get; set; }
        public virtual int AB46 { get; set; }
        public virtual int AB56 { get; set; }

        /// <summary>
        /// 跨度
        /// </summary>
        public virtual int KD_0 { get; set; }
        public virtual int KD_1 { get; set; }
        public virtual int KD_2 { get; set; }
        public virtual int KD_3 { get; set; }
        public virtual int KD_4 { get; set; }
        public virtual int KD_5 { get; set; }

        public virtual int HZ { get; set; }


    }
}
