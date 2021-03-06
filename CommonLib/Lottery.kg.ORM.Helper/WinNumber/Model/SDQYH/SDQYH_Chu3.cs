﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 任选除3走势
    /// </summary>
    public class SDQYH_Chu3 : ImportBase
    {
        /// <summary>
        /// 除3余数比
        /// </summary>
        public virtual string Chu3Bi { get; set; }
        /// <summary>
        /// 除3余数遗漏
        /// </summary>
        public virtual int NO1_0 { get; set; }
        public virtual int NO1_1 { get; set; }
        public virtual int NO1_2 { get; set; }
        public virtual int NO2_0 { get; set; }
        public virtual int NO2_1 { get; set; }
        public virtual int NO2_2 { get; set; }
        public virtual int NO3_0 { get; set; }
        public virtual int NO3_1 { get; set; }
        public virtual int NO3_2 { get; set; }
        public virtual int NO4_0 { get; set; }
        public virtual int NO4_1 { get; set; }
        public virtual int NO4_2 { get; set; }
        public virtual int NO5_0 { get; set; }
        public virtual int NO5_1 { get; set; }
        public virtual int NO5_2 { get; set; }
        /// <summary>
        /// 除3余数个数
        /// </summary>
        public virtual int Yu0_0 { get; set; }
        public virtual int Yu0_1 { get; set; }
        public virtual int Yu0_2 { get; set; }
        public virtual int Yu0_3 { get; set; }
        public virtual int Yu0_4 { get; set; }
        public virtual int Yu0_5 { get; set; }

        public virtual int Yu1_0 { get; set; }
        public virtual int Yu1_1 { get; set; }
        public virtual int Yu1_2 { get; set; }
        public virtual int Yu1_3 { get; set; }
        public virtual int Yu1_4 { get; set; }
        public virtual int Yu1_5 { get; set; }

        public virtual int Yu2_0 { get; set; }
        public virtual int Yu2_1 { get; set; }
        public virtual int Yu2_2 { get; set; }
        public virtual int Yu2_3 { get; set; }
        public virtual int Yu2_4 { get; set; }
        public virtual int Yu2_5 { get; set; }
    }
}
