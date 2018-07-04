using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 隔号走势
    /// </summary>
    public class LN11X5_GHZS : ImportBase
    {
        public virtual int Red_13 { get; set; }
        public virtual int Red_24 { get; set; }
        public virtual int Red_35 { get; set; }
        public virtual int Red_46 { get; set; }
        public virtual int Red_57 { get; set; }
        public virtual int Red_68 { get; set; }
        public virtual int Red_79 { get; set; }
        public virtual int Red_810 { get; set; }
        public virtual int Red_911 { get; set; }

        public virtual int GHao { get; set; }

        public virtual int GH_0 { get; set; }
        public virtual int GH_1 { get; set; }
        public virtual int GH_2 { get; set; }
        public virtual int GH_3 { get; set; }
        public virtual int GH_4 { get; set; }
    }
}
