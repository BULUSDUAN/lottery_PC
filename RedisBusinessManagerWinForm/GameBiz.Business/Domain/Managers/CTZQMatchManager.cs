using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Core;

namespace GameBiz.Domain.Managers
{
    public class CTZQMatchManager : GameBizEntityManagement
    {
        public void AddCTZQ_GameIssuse(CTZQ_GameIssuse entity)
        {
            this.Add<CTZQ_GameIssuse>(entity);
        }
        public void AddCTZQ_Match(CTZQ_Match entity)
        {
            this.Add<CTZQ_Match>(entity);
        }
        public void AddCTZQ_MatchPool(CTZQ_MatchPool entity)
        {
            this.Add<CTZQ_MatchPool>(entity);
        }

        public void UpdateCTZQ_GameIssuse(CTZQ_GameIssuse entity)
        {
            this.Update<CTZQ_GameIssuse>(entity);
        }
        public void UpdateCTZQ_Match(CTZQ_Match entity)
        {
            this.Update<CTZQ_Match>(entity);
        }
        public void UpdateCTZQ_MatchPool(CTZQ_MatchPool entity)
        {
            this.Update<CTZQ_MatchPool>(entity);
        }

        public void ExecUpdateCTZQMatch(string sql)
        {
            this.Session.CreateSQLQuery(sql).ExecuteUpdate();
        }

        public CTZQ_Match GetCTZQ_Match(string gameType, string issuseNumber, int orderNumber)
        {
            Session.Clear();
            return this.Session.Query<CTZQ_Match>().FirstOrDefault(p => p.GameType == gameType && p.IssuseNumber == issuseNumber && p.OrderNumber == orderNumber);
        }
        public List<CTZQ_GameIssuse> QueryCTZQGameIssuseListById(string[] array)
        {
            Session.Clear();
            return this.Session.Query<CTZQ_GameIssuse>().Where(p => array.Contains(p.Id)).ToList();
        }
        public List<CTZQ_Match> QueryCTZQMatchListById(string[] array)
        {
            Session.Clear();
            return this.Session.Query<CTZQ_Match>().Where(p => array.Contains(p.Id)).ToList();
        }
        public List<CTZQ_MatchPool> QueryCTZQMatchPoolListById(string[] array)
        {
            Session.Clear();
            return this.Session.Query<CTZQ_MatchPool>().Where(p => array.Contains(p.Id)).ToList();
        }
        public CTZQ_MatchPool QueryCTZQ_MatchPoolByKey(string key)
        {
            Session.Clear();
            return this.Session.Query<CTZQ_MatchPool>().Where(p => p.Id == key).FirstOrDefault();
        }

        public CTZQ_MatchPool QueryCTZQ_MatchPool(string issuseNumber, string gameType)
        {
            this.Session.Clear();
            return this.Session.Query<CTZQ_MatchPool>().FirstOrDefault(p => p.IssuseNumber == issuseNumber && p.GameType == gameType && p.BonusLevel == 1);
        }

        public List<CTZQMatchInfo> QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber)
        {
            Session.Clear();
            var query = from s in Session.Query<CTZQ_Match>()
                        where s.GameType == gameType && s.IssuseNumber == issuseNumber
                        select new CTZQMatchInfo
                        {
                            GameCode = s.GameCode,
                            GameType = s.GameType,
                            GuestTeamHalfScore = s.GuestTeamHalfScore,
                            GuestTeamId = s.GuestTeamId,
                            GuestTeamName = s.GuestTeamName,
                            GuestTeamScore = s.GuestTeamScore,
                            GuestTeamStanding = s.GuestTeamStanding,
                            HomeTeamHalfScore = s.HomeTeamHalfScore,
                            HomeTeamId = s.HomeTeamId,
                            HomeTeamName = s.HomeTeamName,
                            HomeTeamScore = s.HomeTeamScore,
                            HomeTeamStanding = s.HomeTeamStanding,
                            Id = s.Id,
                            IssuseNumber = s.IssuseNumber,
                            MatchId = s.MatchId,
                            MatchName = s.MatchName,
                            MatchResult = s.MatchResult == null ? "" : s.MatchResult,
                            MatchStartTime = s.MatchStartTime,
                            Mid = s.Mid,
                            OrderNumber = s.OrderNumber,
                            UpdateTime = s.UpdateTime,
                        };
            if (query != null)
                return query.ToList();
            return null;
        }

        public List<Issuse_QueryInfo> QueryCTZQCanBetIssuse(string gameType)
        {
            this.Session.Clear();
            var query = from i in this.Session.Query<CTZQ_GameIssuse>()
                        where i.StopBettingTime > DateTime.Now && i.GameType == gameType
                        select new Issuse_QueryInfo
                        {
                            CreateTime = i.CreateTime,
                            Game = new GameInfo
                            {
                                DisplayName = "传统足球",
                                GameCode = "CTZQ",
                            },
                            GameCode_IssuseNumber = string.Format("CTZQ|{0}|{1}", gameType, i.IssuseNumber),
                            WinNumber = i.WinNumber,
                            IssuseNumber = i.IssuseNumber,
                            Status = IssuseStatus.OnSale,
                            GatewayStopTime = i.StopBettingTime,
                            LocalStopTime = i.StopBettingTime,
                            OfficialStopTime = i.StopBettingTime,
                            StartTime = DateTime.Now
                        };
            return query.ToList();
        }

        public Issuse_QueryInfo QueryCTZQCurrentIssuse(string gameType)
        {
            this.Session.Clear();
            var query = from i in this.Session.Query<CTZQ_GameIssuse>()
                        where i.StopBettingTime >= DateTime.Now && i.GameType == gameType
                        orderby i.StopBettingTime ascending
                        select new Issuse_QueryInfo
                        {
                            CreateTime = i.CreateTime,
                            Game = new GameInfo
                            {
                                DisplayName = "传统足球",
                                GameCode = "CTZQ",
                            },
                            GameCode_IssuseNumber = string.Format("CTZQ|{0}|{1}", gameType, i.IssuseNumber),
                            WinNumber = i.WinNumber,
                            IssuseNumber = i.IssuseNumber,
                            Status = IssuseStatus.OnSale,
                            GatewayStopTime = i.StopBettingTime,
                            LocalStopTime = i.StopBettingTime,
                            OfficialStopTime = i.StopBettingTime,
                            StartTime = DateTime.Now
                        };
            return query.FirstOrDefault();
        }

        public MatchBiz.Core.CTZQ_IssuseInfo QueryCTZQCurrentIssuse()
        {
            this.Session.Clear();
            var query = from i in this.Session.Query<CTZQ_GameIssuse>()
                        where i.StopBettingTime >= DateTime.Now
                        orderby i.StopBettingTime ascending
                        select new MatchBiz.Core.CTZQ_IssuseInfo
                        {
                            CreateTime = i.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            GameCode = i.GameCode,
                            GameType = i.GameType,
                            Id = i.Id,
                            IssuseNumber = i.IssuseNumber,
                            StopBettingTime = i.StopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            WinNumber = i.WinNumber,
                        };
            return query.FirstOrDefault();
        }

        public CTZQMatch_PoolInfo_Collection QueryCTZQMatch_PoolCollection(string gameType, string issuseNumber)
        {
            Session.Clear();
            CTZQMatch_PoolInfo_Collection collection = new CTZQMatch_PoolInfo_Collection();
            collection.TotalCount = 0;
            var query = from s in Session.Query<CTZQ_Match>()
                        //join g in Session.Query<GameIssuse>() on s.IssuseNumber equals g.IssuseNumber
                        where s.GameType == gameType
                        && s.GameCode == "CTZQ"
                        && s.IssuseNumber == issuseNumber
                        orderby s.OrderNumber ascending
                        select new CTZQMatch_PoolInfo
                        {
                            GameCode = s.GameCode,
                            GameType = s.GameType,
                            GuestTeamHalfScore = s.GuestTeamHalfScore,
                            GuestTeamId = s.GuestTeamId,
                            GuestTeamName = s.GuestTeamName,
                            GuestTeamScore = s.GuestTeamScore,
                            GuestTeamStanding = s.GuestTeamStanding,
                            HomeTeamHalfScore = s.HomeTeamHalfScore,
                            HomeTeamId = s.HomeTeamId,
                            HomeTeamName = s.HomeTeamName,
                            HomeTeamScore = s.HomeTeamScore,
                            HomeTeamStanding = s.HomeTeamStanding,
                            Id = s.Id,
                            IssuseNumber = s.IssuseNumber,
                            MatchId = s.MatchId,
                            MatchName = s.MatchName,
                            MatchResult = s.MatchResult == null ? "" : s.MatchResult,
                            MatchStartTime = s.MatchStartTime,
                            Mid = s.Mid,
                            OrderNumber = s.OrderNumber,
                            UpdateTime = s.UpdateTime,
                            BounsTime = s.UpdateTime,
                        };
            if (query != null)
            {
                collection.ListInfo.AddRange(query.ToList());
                collection.TotalCount = query.ToList().Count;
            }
            return collection;
        }
        public CTZQMatch_PoolInfo_Collection GetCTZQIssuse(string gameType, int count)
        {
            CTZQMatch_PoolInfo_Collection collection = new CTZQMatch_PoolInfo_Collection();
            var query = from s in Session.Query<CTZQ_Match>()
                        where s.GameType == gameType
                        group s by s.IssuseNumber into g
                        select new CTZQMatch_PoolInfo
                        {
                            IssuseNumber = g.Key,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.ListInfo = query.ToList().OrderByDescending(s => s.IssuseNumber).Take(count).ToList();
                return collection;
            }
            return new CTZQMatch_PoolInfo_Collection();
        }
    }
}
