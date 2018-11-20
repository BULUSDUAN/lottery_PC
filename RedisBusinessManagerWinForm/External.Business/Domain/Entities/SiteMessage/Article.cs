using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.SiteMessage
{
    /// <summary>
    /// 文章
    /// </summary>
    public class Article
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 对应彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string KeyWords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string DescContent { get; set; }
        /// <summary>
        /// 是否标红
        /// </summary>
        public virtual bool IsRedTitle { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 排序次序
        /// </summary>
        public virtual int ShowIndex { get; set; }
        /// <summary>
        /// 阅读次数统计
        /// </summary>
        public virtual int ReadCount { get; set; }
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

        /// <summary>
        /// 静态文件地址
        /// </summary>
        public virtual string StaticPath { get; set; }

        /// <summary>
        /// 上一条
        /// </summary>
        public virtual string PreId { get; set; }
        public virtual string PreTitle { get; set; }
        public virtual string PreStaticPath { get; set; }
        /// <summary>
        /// 下一条
        /// </summary>
        public virtual string NextId { get; set; }
        public virtual string NextTitle { get; set; }
        public virtual string NextStaticPath { get; set; }
    }

    /// <summary>
    /// 内容关键字
    /// </summary>
    public class KeywordOfArticle
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string KeyWords { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public virtual string Link { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
    }
}
