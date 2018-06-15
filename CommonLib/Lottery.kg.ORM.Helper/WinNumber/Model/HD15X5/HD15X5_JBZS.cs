using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 华东15选5基本走势
    /// </summary>
    public class HD15X5_JBZS : ImportBase
    {
        public virtual int Red01 { get; set; }
        public virtual int Red02 { get; set; }
        public virtual int Red03 { get; set; }
        public virtual int Red04 { get; set; }
        public virtual int Red05 { get; set; }
        public virtual int Red06 { get; set; }
        public virtual int Red07 { get; set; }
        public virtual int Red08 { get; set; }
        public virtual int Red09 { get; set; }
        public virtual int Red10 { get; set; }
        public virtual int Red11 { get; set; }
        public virtual int Red12 { get; set; }
        public virtual int Red13 { get; set; }
        public virtual int Red14 { get; set; }
        public virtual int Red15 { get; set; }

        public virtual int Hezhi { get; set; }
        public virtual int HW { get; set; }
        public virtual string DaXiaobi { get; set; }
        public virtual string Jobi { get; set; }
        public virtual string ZHbi { get; set; }
    }
}
