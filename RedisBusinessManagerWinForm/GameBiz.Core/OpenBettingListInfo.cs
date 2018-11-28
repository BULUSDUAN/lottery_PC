using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{

    [CommunicationObject]
    public class OpenBettingListInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否开通合买(是否允许发布合买订单)
        /// </summary>
        public bool IsOpenHeMai { get; set; }
        /// <summary>
        /// Web端是否开通合买
        /// </summary>
        public bool IsWebOpenHM { get; set; }
        /// <summary>
        /// M端是否开通合买
        /// </summary>
        public bool ISMOpenHM { get; set; }
        /// <summary>
        /// APP端是否开通合买
        /// </summary>
        public bool IsAppOpenHM { get; set; }
        /// <summary>
        /// IOS端是否开通合买
        /// </summary>
        public bool IsIOSOpenHM { get; set; }
        /// <summary>
        /// 合买已开通站点列表:web|m|app|ios
        /// </summary>
        public string SiteList { get; set; }
    }
}
