using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.FriendLink
{
    /// <summary>
    /// 友情链接,热点链接
    /// </summary>
    public class FriendLinks
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 友情链接排序
        /// </summary>
        public virtual int IndexLink { get; set; }
        /// <summary>
        /// 网站名称
        /// </summary>
        public virtual string InnerText { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public virtual string LinkUrl { get; set; }
        /// <summary>
        /// 是否是友情链接
        /// </summary>
        public virtual bool Isfriendship { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

    }
}
