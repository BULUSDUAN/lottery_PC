using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Mappings;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 分析推荐
    /// </summary>
    public class ExperterAnalyzeScheme
    {
        public virtual string AnalyzeId { get; set; }
        /// <summary>
        /// 专家编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public virtual string Source { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Content { get; set; }
        /// <summary>
        /// 分析价格
        /// </summary>
        public virtual decimal Price { get; set; }
        /// <summary>
        /// 售出次数
        /// </summary>
        public virtual int SellCount { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public virtual DealWithType DealWithType { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public virtual string DisposeOpinion { get; set; }
        /// <summary>
        /// 购买时间戳
        /// </summary>
        public virtual string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
