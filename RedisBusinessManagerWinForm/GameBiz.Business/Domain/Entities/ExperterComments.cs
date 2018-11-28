using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Mappings;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 名家吐槽
    /// </summary>
    public class ExperterComments
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 专家编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 发送用户编号
        /// </summary>
        public virtual string SendUserId { get; set; }
        /// <summary>
        /// 推荐方案
        /// </summary>
        public virtual string RecommendSchemeId { get; set; }
        /// <summary>
        /// 分析方案
        /// </summary>
        public virtual string AnalyzeSchemeId { get; set; }
        /// <summary>
        /// 吐槽内容
        /// </summary>
        public virtual string Content { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public virtual DealWithType DealWithType { get; set; }
        /// <summary>
        /// 吐槽类别
        /// </summary>
        public virtual CommentsTpye CommentsTpye { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public virtual string DisposeOpinion { get; set; }
        /// <summary>
        /// 当前发布时间
        /// </summary>
        public virtual string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
