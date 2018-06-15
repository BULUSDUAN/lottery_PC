using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 基本走势
    /// </summary>
    public class PL3_DXHM : ImportBase
    {
        /// <summary>
        /// 百十个位置的遗漏
        /// </summary>
        public virtual int BW_0 { get; set; }
        public virtual int BW_1 { get; set; }
        public virtual int BW_2 { get; set; }
        public virtual int BW_3 { get; set; }
        public virtual int BW_4 { get; set; }
        public virtual int BW_5 { get; set; }
        public virtual int BW_6 { get; set; }
        public virtual int BW_7 { get; set; }
        public virtual int BW_8 { get; set; }
        public virtual int BW_9 { get; set; }
        public virtual int SW_0 { get; set; }
        public virtual int SW_1 { get; set; }
        public virtual int SW_2 { get; set; }
        public virtual int SW_3 { get; set; }
        public virtual int SW_4 { get; set; }
        public virtual int SW_5 { get; set; }
        public virtual int SW_6 { get; set; }
        public virtual int SW_7 { get; set; }
        public virtual int SW_8 { get; set; }
        public virtual int SW_9 { get; set; }
        public virtual int GW_0 { get; set; }
        public virtual int GW_1 { get; set; }
        public virtual int GW_2 { get; set; }
        public virtual int GW_3 { get; set; }
        public virtual int GW_4 { get; set; }
        public virtual int GW_5 { get; set; }
        public virtual int GW_6 { get; set; }
        public virtual int GW_7 { get; set; }
        public virtual int GW_8 { get; set; }
        public virtual int GW_9 { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public virtual string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public virtual int Bi3_0 { get; set; }
        public virtual int Bi2_1 { get; set; }
        public virtual int Bi1_2 { get; set; }
        public virtual int Bi0_3 { get; set; }


    }
}
