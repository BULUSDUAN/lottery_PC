using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大乐透大小走势
    /// </summary>
    public class DLT_DX : ImportBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public virtual string QianQu { get; set; }
        /// <summary>
        /// 大小排位
        /// </summary>
        public virtual string DaoXiaoQualifying { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public virtual string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小遗漏
        /// </summary>
        public virtual int NO1D { get; set; }
        public virtual int NO1X { get; set; }
        public virtual int NO2D { get; set; }
        public virtual int NO2X { get; set; }
        public virtual int NO3D { get; set; }
        public virtual int NO3X { get; set; }
        public virtual int NO4D { get; set; }
        public virtual int NO4X { get; set; }
        public virtual int NO5D { get; set; }
        public virtual int NO5X { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public virtual int Bi0_5 { get; set; }
        public virtual int Bi1_4 { get; set; }
        public virtual int Bi2_3 { get; set; }
        public virtual int Bi3_2 { get; set; }
        public virtual int Bi4_1 { get; set; }
        public virtual int Bi5_0 { get; set; }

    }
}
