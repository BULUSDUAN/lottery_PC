using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 奇偶走势
    /// </summary>
    public class PL3_JIOU : ImportBase
    {
        /// <summary>
        /// 奇偶形态
        /// </summary>
        public virtual string JOType { get; set; }
        /// <summary>
        /// 奇奇奇
        /// </summary>
        public virtual int JO_JJJ { get; set; }
        public virtual int JO_JJO { get; set; }
        public virtual int JO_JOJ { get; set; }
        public virtual int JO_OJJ { get; set; }
        public virtual int JO_JOO { get; set; }
        public virtual int JO_OJO { get; set; }
        public virtual int JO_OOJ { get; set; }
        public virtual int JO_OOO { get; set; }
        /// <summary> 
        /// 奇偶比
        /// </summary>
        public virtual string JOBi { get; set; }
        /// <summary>
        /// 奇偶比0比3
        /// </summary>
        public virtual int Bi3_0 { get; set; }
        public virtual int Bi2_1 { get; set; }
        public virtual int Bi1_2 { get; set; }
        public virtual int Bi0_3 { get; set; }
    }
}
