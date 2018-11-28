using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using Common.Business;
using NHibernate.Criterion;
using External.Domain.Entities.Authentication;
using GameBiz.Business;
using NHibernate.Linq;
using External.Core.Login;
using GameBiz.Domain.Entities;
using Common.Utilities;
using External.Core;

namespace External.Domain.Managers.Authentication
{
    public class BettingPointTeamManager : GameBizEntityManagement
    {
        public void AddTeamComment(TeamComment entity)
        {
            this.Add<TeamComment>(entity);
        }
        public void AddTeamCommentRecored(TeamCommentRecored entity)
        {
            this.Add<TeamCommentRecored>(entity);
        }

        public void AddTeamVote(TeamVote entity)
        {
            this.Add<TeamVote>(entity);
        }
        public void AddTeamVoteRecord(TeamVoteRecord entity)
        {
            this.Add<TeamVoteRecord>(entity);
        }

        public void UpdateTeamComment(TeamComment entity)
        {
            this.Update<TeamComment>(entity);
        }

        public void UpdateTeamVote(TeamVote entity)
        {
            this.Update<TeamVote>(entity);
        }

        public TeamComment QueryTeamComment(int id)
        {
            Session.Clear();
            return this.Session.Query<TeamComment>().FirstOrDefault(p => p.ID == id);
        }
        public TeamCommentRecored QueryTeamCommentRecored(int teamCommentId, string userId)
        {
            Session.Clear();
            return this.Session.Query<TeamCommentRecored>().FirstOrDefault(p => p.TeamCommentId == teamCommentId && p.UserId == userId);
        }

        public TeamVote QueryTeamVote(int id)
        {
            Session.Clear();
            return this.Session.Query<TeamVote>().FirstOrDefault(p => p.ID == id);
        }
        public TeamVote QueryTeamVoteByMatchId(string matchDate, string orderNumber, VoteCategory category)
        {
            Session.Clear();
            return this.Session.Query<TeamVote>().FirstOrDefault(p => p.MatchDate == matchDate && p.OrderNumber == orderNumber && p.Category == category);
        }
        public TeamVoteRecord QueryTeamVoteRecord(int voteId, string userId)
        {
            Session.Clear();
            return this.Session.Query<TeamVoteRecord>().FirstOrDefault(p => p.TeamVoteId == voteId && p.UserId == userId);
        }

        public List<TeamVoteInfo> QueryTeamVoteFansList(string matchData, string orderNumber)
        {
            Session.Clear();
            var query = from t in this.Session.Query<TeamVote>()
                        where t.MatchDate == matchData && t.OrderNumber == orderNumber
                        select new TeamVoteInfo
                        {
                            ID = t.ID,
                            Category = t.Category,
                            GuestTeamNameFans = t.GuestTeamNameFans,
                            HomeTeamFans = t.HomeTeamFans
                        };

            return query.ToList();
        }

        public List<TeamCommentRankingInfo> QueryTeamCommentRanking(DateTime currentDate, DateTime nextDate, int length)
        {
            Session.Clear();

            var sql = string.Format(@"select top {2} c.UserId,u.DisplayName,sum(c.ByTop)ByTop
                        from [E_TeamComment_Article] c
                        left join  C_User_Register u on c.UserId=u.UserId

                        where c.PublishTime>'{0}' and c.PublishTime<='{1}'
                        group by c.UserId,u.DisplayName

                        order by ByTop desc", currentDate.ToString("yyyy-MM-dd"), nextDate.ToString("yyyy-MM-dd"), length);

            var result = new List<TeamCommentRankingInfo>();
            var list = this.Session.CreateSQLQuery(sql).List();
            foreach (var item in list)
            {
                var array = item as object[];
                result.Add(new TeamCommentRankingInfo()
                {
                    UserId = UsefullHelper.GetDbValue<string>(array[0]),
                    DisplayName = UsefullHelper.GetDbValue<string>(array[1]),
                    ByTop = UsefullHelper.GetDbValue<int>(array[2]),
                });
            }
            return result;


            //var query = from c in this.Session.Query<TeamComment>()
            //            join u in this.Session.Query<UserRegister>() on c.UserId equals u.UserId
            //            where c.PublishTime >= new DateTime(currentDate.Year, currentDate.Month, currentDate.Day) && c.PublishTime < new DateTime(nextDate.Year, nextDate.Month, nextDate.Day)
            //            orderby c.ByTop descending
            //            group c by new { UserId = c.UserId, DisplayName = u.DisplayName } into t
            //            select new TeamCommentRankingInfo
            //            {
            //                UserId = t.Key.UserId,
            //                DisplayName = t.Key.DisplayName,
            //                ByTop = t.Sum(p => p.ByTop),
            //            };
            //return query.Take(length).ToList();
        }

        public List<TeamCommentRankingInfo> QueryTeamCommentDayRanking(DateTime currentDate, int length)
        {
            return QueryTeamCommentRanking(currentDate, currentDate.AddDays(1), length);
        }

        public List<TeamCommentRankingInfo> QueryTeamCommentWeekRanking(DateTime currentDate, int length)
        {
            return QueryTeamCommentRanking(currentDate.AddDays(-7), currentDate, length);
        }

        public List<TeamCommentRankingInfo> QueryTeamCommentMonthRanking(DateTime currentDate, int length)
        {
            return QueryTeamCommentRanking(currentDate.AddMonths(-1), currentDate, length);
        }

        public List<TeamCommentInfo> QueryTeamArticle(string gameCode, string matchDate, string orderNumber, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from c in this.Session.Query<TeamComment>()
                        join u in this.Session.Query<UserRegister>() on c.UserId equals u.UserId
                        where (gameCode == string.Empty || c.GameCode == gameCode)
                        && (matchDate == string.Empty || c.MatchDate == matchDate)
                        && (orderNumber == string.Empty || c.OrderNumber == orderNumber)
                        orderby c.PublishTime descending
                        select new TeamCommentInfo
                        {
                            UserId = c.UserId,
                            DisplayName = u.DisplayName,
                            GameCode = c.GameCode,
                            MatchDate = c.MatchDate,
                            OrderNumber = c.OrderNumber,
                            HomeTeamName = c.HomeTeamName,
                            GuestTeamName = c.GuestTeamName,
                            MatchTime = c.MatchTime,
                            ArticleContent = c.ArticleContent,
                            ByTop = c.ByTop,
                            ByTrample = c.ByTrample,
                            PublishTime = c.PublishTime,
                            Id = c.ID,
                            LeagueName = c.LeagueName,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<TeamCommentInfo> QueryUserTeamArticle(string userId, int length)
        {
            Session.Clear();
            var query = from c in this.Session.Query<TeamComment>()
                        join u in this.Session.Query<UserRegister>() on c.UserId equals u.UserId
                        where c.UserId == userId
                        orderby c.PublishTime descending
                        select new TeamCommentInfo
                        {
                            UserId = c.UserId,
                            DisplayName = u.DisplayName,
                            GameCode = c.GameCode,
                            MatchDate = c.MatchDate,
                            OrderNumber = c.OrderNumber,
                            HomeTeamName = c.HomeTeamName,
                            GuestTeamName = c.GuestTeamName,
                            MatchTime = c.MatchTime,
                            ArticleContent = c.ArticleContent,
                            ByTop = c.ByTop,
                            ByTrample = c.ByTrample,
                            PublishTime = c.PublishTime,
                            Id = c.ID,
                            LeagueName = c.LeagueName,
                        };
            return query.Take(length).ToList();
        }
    }
}
