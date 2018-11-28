using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class SingleTreasureAttention
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 被关注用户编号
        /// </summary>
        public virtual string BeConcernedUserId { get; set; }
        /// <summary>
        /// 关注者用户编号
        /// </summary>
        public virtual string ConcernedUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
    public class SingleTreasureAttentionSummary
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 被关注总数
        /// </summary>
        public virtual int BeConcernedUserCount { get; set; }
        /// <summary>
        /// 关注总数
        /// </summary>
        public virtual int ConcernedUserCount { get; set; }
        /// <summary>
        /// 晒单总数
        /// </summary>
        public virtual int SingleTreasureCount { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }
}
