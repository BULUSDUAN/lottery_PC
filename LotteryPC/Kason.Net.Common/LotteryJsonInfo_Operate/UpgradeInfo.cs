using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kason.Net.Common
{
    public class UpgradeInfo
    {
        /// <summary>
        /// 升级标识    
        /// </summary>
        public string Upgrade { get; set; }
        /// <summary>
        /// 距离升级截止秒数
        /// </summary>
        public DateTime UpgradeStopTime { get; set; }
        /// <summary>
        /// 维护页面地址
        /// </summary>
        public string MaintainPage { get; set; }
        /// <summary>
        /// 倒计时总秒数
        /// </summary>
        public double TotalSeconds { get; set; }
    }
}