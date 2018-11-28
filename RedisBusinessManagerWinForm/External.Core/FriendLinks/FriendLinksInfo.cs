using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.FriendLinks
{

    [CommunicationObject]
    public class FriendLinksInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 友情链接排序
        /// </summary>
        public int IndexLink { get; set; }
        /// <summary>
        /// 网站名称
        /// </summary>
        public string InnerText { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        public bool IsFriendShip { get; set; }
    }

    [CommunicationObject]
    public class FriendLinksInfooCollection : List<FriendLinksInfo>
    {
    }
}
