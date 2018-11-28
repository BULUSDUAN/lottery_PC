using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;

namespace External.Domain.Entities.Authentication
{
    /// <summary>
    /// 球队点评
    /// </summary>
    public class TeamComment
    {
        public virtual int ID { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string MatchDate { get; set; }
        /// <summary>
        /// 场次信息
        /// </summary>
        public virtual string OrderNumber { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public virtual string LeagueName { get; set; }
        /// <summary>
        /// 主队信息
        /// </summary>
        public virtual string HomeTeamName { get; set; }
        /// <summary>
        /// 客队信息
        /// </summary>
        public virtual string GuestTeamName { get; set; }
        /// <summary>
        ///  比赛时间
        /// </summary>
        public virtual DateTime MatchTime { get; set; }
        /// <summary>
        /// 用户的发布文章
        /// </summary>
        public virtual string ArticleContent { get; set; }
        /// <summary>
        ///  被用户支持
        /// </summary>
        public virtual int ByTop { get; set; }
        /// <summary>
        /// 被用户鄙视
        /// </summary>
        public virtual int ByTrample { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public virtual DateTime PublishTime { get; set; }
    }

    public class TeamCommentRecored
    {
        public virtual int Id { get; set; }
        public virtual int TeamCommentId { get; set; }
        public virtual string UserId { get; set; }
        public virtual bool IsByTop { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
