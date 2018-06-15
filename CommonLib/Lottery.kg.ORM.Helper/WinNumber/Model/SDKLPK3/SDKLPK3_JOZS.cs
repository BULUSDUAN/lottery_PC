using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class SDKLPK3_JOZS : ImportBase
    {
        public virtual int D1_J { get; set; }
        public virtual int D1_O { get; set; }
        public virtual int D2_J { get; set; }
        public virtual int D2_O { get; set; }
        public virtual int D3_J { get; set; }
        public virtual int D3_O { get; set; }

        public virtual int JOB_30 { get; set; }
        public virtual int JOB_21 { get; set; }
        public virtual int JOB_12 { get; set; }
        public virtual int JOB_03 { get; set; }

        public virtual int JJJ { get; set; }
        public virtual int JJO { get; set; }
        public virtual int JOJ { get; set; }
        public virtual int OJJ { get; set; }
        public virtual int JOO { get; set; }
        public virtual int OJO { get; set; }
        public virtual int OOJ { get; set; }
        public virtual int OOO { get; set; }
    }
}
