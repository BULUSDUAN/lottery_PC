using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using External.Core;

namespace External.Domain.Entities.Authentication
{
    /// <summary>
    /// 球队投票
    /// </summary>
    public class TeamVote
    {
        public virtual int ID { get; set; }
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string MatchDate { get; set; }
        /// <summary>
        /// 场次信息
        /// </summary>
        public virtual string OrderNumber { get; set; }
        public virtual DateTime MatchStartTime { get; set; }
        /// <summary>
        /// 球队斗志 赛前状态 对阵佳绩 战绩 欧赔取向 亚培取向 的主客分类
        /// </summary>
        public virtual VoteCategory Category { get; set; }
        /// <summary>
        /// 支持主队粉丝
        /// </summary>
        public virtual int HomeTeamFans { get; set; }
        /// <summary>
        ///  支持客队粉丝
        /// </summary>
        public virtual int GuestTeamNameFans { get; set; }

        public virtual DateTime CreateTime { get; set; }
    }

    public class TeamVoteRecord
    {
        public virtual int Id { get; set; }
        public virtual int TeamVoteId { get; set; }
        public virtual string UserId { get; set; }
        public virtual VoteCategory Category { get; set; }
        public virtual bool VoteToHomeTeam { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
