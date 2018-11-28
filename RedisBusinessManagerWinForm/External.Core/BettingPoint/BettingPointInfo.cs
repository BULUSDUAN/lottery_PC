using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.Login
{
    [CommunicationObject]
    public class TeamCommentAddInfo
    {
        public string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string MatchDate { get; set; }
        /// <summary>
        /// 场次信息
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 用户的发布文章
        /// </summary>
        public string ArticleContent { get; set; }
    }
    [CommunicationObject]
    public class TeamCommentRankingInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 被支持次数
        /// </summary>
        public int ByTop { get; set; }
    }
    [CommunicationObject]
    public class TeamCommentRankingInfoColltion : List<TeamCommentRankingInfo>
    {
    }

    [CommunicationObject]
    public class TeamCommentInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string MatchDate { get; set; }
        /// <summary>
        /// 场次信息
        /// </summary>
        public string OrderNumber { get; set; }
        public string LeagueName { get; set; }
        /// <summary>
        /// 主队信息
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队信息
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        ///  比赛时间
        /// </summary>
        public DateTime MatchTime { get; set; }
        /// <summary>
        /// 用户的发布文章
        /// </summary>
        public string ArticleContent { get; set; }
        /// <summary>
        ///  被用户支持
        /// </summary>
        public int ByTop { get; set; }
        /// <summary>
        /// 被用户鄙视
        /// </summary>
        public int ByTrample { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
    }

    [CommunicationObject]
    public class TeamCommentInfoColltion
    {
        public int TotalCount { get; set; }
        public List<TeamCommentInfo> TeamCommentInfoList { get; set; }
    }
    /// <summary>
    /// 投票对象
    /// </summary>
    [CommunicationObject]
    public class TeamVoteInfo
    {
        public int ID { get; set; }
        public int HomeTeamFans { get; set; }
        public VoteCategory Category { get; set; }
        public int GuestTeamNameFans { get; set; }
    }

    [CommunicationObject]
    public class TeamVoteInfoColltion : List<TeamVoteInfo>
    {
    }
}
