using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.SiteMessage
{
    /// <summary>
    /// 疑问
    /// </summary>
    public class Doubt
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 排序次序
        /// </summary>
        public virtual int ShowIndex { get; set; }
        /// <summary>
        /// 顶的次数统计
        /// </summary>
        public virtual int UpCount { get; set; }
        /// <summary>
        /// 踩的次数统计
        /// </summary>
        public virtual int DownCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public virtual string CreateUserKey { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public virtual string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新者编号
        /// </summary>
        public virtual string UpdateUserKey { get; set; }
        /// <summary>
        /// 更新者显示名称
        /// </summary>
        public virtual string UpdateUserDisplayName { get; set; }
    }
    /// <summary>
    /// 顶踩记录
    /// </summary>
    public class UpDownRecord
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public virtual string DoubtId { get; set; }
        /// <summary>
        /// 顶/踩
        /// </summary>
        public virtual string UpDown { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
