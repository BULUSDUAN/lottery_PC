using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.SiteMessage
{
    public class SiteActivity
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 图片Url
        /// </summary>
        public virtual string ImageUrl { get; set; }
        /// <summary>
        /// 文章Url 
        /// </summary>
        public virtual string ArticleUrl { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public virtual string Titile { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public virtual DateTime StartTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public virtual DateTime EndTime { get; set; }
    }
}
