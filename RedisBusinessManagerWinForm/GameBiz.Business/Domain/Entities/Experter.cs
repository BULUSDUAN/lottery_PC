using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Mappings;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 专家
    /// </summary>
    public class Experter
    {
        /// <summary>
        /// 专家编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public virtual string ExperterSummary { get; set; }
        /// <summary>
        /// 专家头像
        /// </summary>
        public virtual string ExperterHeadImage { get; set; }
        /// <summary>
        /// 擅长彩种
        /// </summary>
        public virtual string AdeptGameCode { get; set; }
        /// <summary>
        /// 最近发单数
        /// </summary>
        public virtual int RecentlyOrderCount { get; set; }
        /// <summary>
        /// 专家类别
        /// </summary>
        public virtual ExperterType ExperterType { get; set; }
        /// <summary>
        /// 周命中率
        /// </summary>
        public virtual decimal WeekShooting { get; set; }
        /// <summary>
        /// 月命中率
        /// </summary>
        public virtual decimal MonthShooting { get; set; }
        /// <summary>
        /// 总和命中率
        /// </summary>
        public virtual decimal TotalShooting { get; set; }
        /// <summary>
        /// 周回报率
        /// </summary>
        public virtual decimal WeekRate { get; set; }
        /// <summary>
        /// 月回报率
        /// </summary>
        public virtual decimal MonthRate { get; set; }
        /// <summary>
        /// 总回报率
        /// </summary>
        public virtual decimal TotalRate { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public virtual DealWithType DealWithType { get; set; }
        /// <summary>
        /// 名家启用，禁用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public virtual string DisposeOpinion { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 专家修改资料历史
    /// </summary>
    public class ExperterUpdateHitstroy
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 专家编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public virtual string ExperterSummary { get; set; }
        /// <summary>
        /// 专家头像
        /// </summary>
        public virtual string ExperterHeadImage { get; set; }
        /// <summary>
        /// 擅长彩种
        /// </summary>
        public virtual string AdeptGameCode { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public virtual DealWithType DealWithType { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public virtual string DisposeOpinion { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
