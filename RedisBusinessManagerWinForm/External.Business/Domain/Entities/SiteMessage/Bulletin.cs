using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using GameBiz.Core;

namespace External.Domain.Entities.SiteMessage
{
    /// <summary>
    /// 公告
    /// </summary>
    public class Bulletin
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Content { get; set; }
        /// <summary>
        /// 公告状态：正常，取消
        /// </summary>
        public virtual EnableStatus Status { get; set; }
        /// <summary>
        /// 公告代理商
        /// </summary>
        public virtual BulletinAgent BulletinAgent { get; set; }
        /// <summary>
        /// 有效期从。如为null表示及时启用
        /// </summary>
        public virtual DateTime? EffectiveFrom { get; set; }
        /// <summary>
        /// 有效期至。如为null表示不过期
        /// </summary>
        public virtual DateTime? EffectiveTo { get; set; }
        /// <summary>
        /// 优先级别（数字越小，表示级别越高）
        /// </summary>
        public virtual int Priority { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public virtual string CreateBy { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 最后修改者
        /// </summary>
        public virtual string UpdateBy { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public virtual int IsPutTop { get; set; }
    }
}
