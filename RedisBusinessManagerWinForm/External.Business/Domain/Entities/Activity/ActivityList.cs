using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Domain.Entities;
using External.Core;

namespace External.Domain.Entities.Activity
{
    /// <summary>
    /// 活动列表
    /// </summary>
    public class ActivityList
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 活动Id
        /// </summary>
        public virtual int ActivityIndex { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public virtual string ActiveName { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 活动描述
        /// </summary>
        public virtual string Summary { get; set; }
        /// <summary>
        /// 活动地址
        /// </summary>
        public virtual string LinkUrl { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public virtual string ImageUrl { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public virtual bool IsShow { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public virtual DateTime BeginTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public virtual DateTime EndTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
