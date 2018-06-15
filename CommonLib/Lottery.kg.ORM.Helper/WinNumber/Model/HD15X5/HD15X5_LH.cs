using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 连号走势
    /// </summary>
    public class HD15X5_LH : ImportBase
    {
        public virtual int Red_12 { get; set; }
        public virtual int Red_23 { get; set; }
        public virtual int Red_34 { get; set; }
        public virtual int Red_45 { get; set; }
        public virtual int Red_56 { get; set; }
        public virtual int Red_67 { get; set; }
        public virtual int Red_78 { get; set; }
        public virtual int Red_89 { get; set; }
        public virtual int Red_910 { get; set; }
        public virtual int Red_1011 { get; set; }
        public virtual int Red_1112 { get; set; }
        public virtual int Red_1213 { get; set; }
        public virtual int Red_1314 { get; set; }
        public virtual int Red_1415 { get; set; }

        public virtual int GHao { get; set; }

        public virtual string XLH { get; set; }
        public virtual string DLH { get; set; }

        public virtual int GH_0 { get; set; }
        public virtual int GH_1 { get; set; }
        public virtual int GH_2 { get; set; }
        public virtual int GH_3 { get; set; }
        public virtual int GH_4 { get; set; }

    }
}
