using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 大小走势
    /// </summary>
    public class GDKLSF_DX : ImportBase
    {
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
        public virtual int NO6D { get; set; }
        public virtual int NO6X { get; set; }
        public virtual int NO7D { get; set; }
        public virtual int NO7X { get; set; }
        public virtual int NO8D { get; set; }
        public virtual int NO8X { get; set; }
        /// <summary>
        /// 大小比8比0
        /// </summary>
        public virtual int Bi8_0 { get; set; }
        public virtual int Bi7_1 { get; set; }
        public virtual int Bi6_2 { get; set; }
        public virtual int Bi5_3 { get; set; }
        public virtual int Bi4_4 { get; set; }
        public virtual int Bi3_5 { get; set; }
        public virtual int Bi2_6 { get; set; }
        public virtual int Bi1_7 { get; set; }
        public virtual int Bi0_8 { get; set; }

    }
}
