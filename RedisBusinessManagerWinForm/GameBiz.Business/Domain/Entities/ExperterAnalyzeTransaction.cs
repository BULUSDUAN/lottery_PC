using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Mappings;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 专家分析交易信息
    /// </summary>
    public class ExperterAnalyzeTransaction
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 分析Id
        /// </summary>
        public virtual string AnalyzeId { get; set; }
        /// <summary>
        /// 购买用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 专家Id
        /// </summary>
        public virtual string ExperterId { get; set; }
        /// <summary>
        /// 分析单价
        /// </summary>
        public virtual decimal Price { get; set; }
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
