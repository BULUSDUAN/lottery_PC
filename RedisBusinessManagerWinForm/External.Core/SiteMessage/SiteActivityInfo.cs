using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;
using Common.Utilities;

namespace External.Core.SiteMessage
{
    /// <summary>
    /// 优惠活动
    /// </summary>
    [CommunicationObject]
    public class SiteActivityInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 图片Url
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 文章Url 
        /// </summary>
        public string ArticleUrl { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public string Titile { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }


    [CommunicationObject]
    public class SiteActivityInfo_Collection
    {
        public SiteActivityInfo_Collection()
        {
            ListInfo = new List<SiteActivityInfo>();
        }
        public int TotalCount { get; set; }
        public List<SiteActivityInfo> ListInfo { get; set; }
    }
}
