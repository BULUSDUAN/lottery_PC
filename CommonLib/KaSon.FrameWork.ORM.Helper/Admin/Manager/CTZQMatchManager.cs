using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    public class CTZQMatchManager : DBbase
    {
        public void AddCTZQ_GameIssuse(C_CTZQ_GameIssuse entity)
        {
            DB.GetDal<C_CTZQ_GameIssuse>().Add(entity);
        }
        public void AddCTZQ_Match(C_CTZQ_Match entity)
        {
            DB.GetDal<C_CTZQ_Match>().Add(entity);
        }
        public void AddCTZQ_MatchPool(C_CTZQ_MatchPool entity)
        {
            DB.GetDal<C_CTZQ_MatchPool>().Add(entity);
        }
        public void UpdateCTZQ_GameIssuse(C_CTZQ_GameIssuse entity)
        {
            DB.GetDal<C_CTZQ_GameIssuse>().Update(entity);
        }
        public void UpdateCTZQ_Match(C_CTZQ_Match entity)
        {
            DB.GetDal<C_CTZQ_Match>().Update(entity);
        }
        public void UpdateCTZQ_MatchPool(C_CTZQ_MatchPool entity)
        {
            DB.GetDal<C_CTZQ_MatchPool>().Update(entity);
        }
        public void ExecUpdateCTZQMatch(string sql)
        {
            DB.CreateSQLQuery(sql);
        }
        public C_CTZQ_Match GetCTZQ_Match(string gameType, string issuseNumber, int orderNumber)
        {
            return DB.CreateQuery<C_CTZQ_Match>().FirstOrDefault(p => p.GameType == gameType && p.IssuseNumber == issuseNumber && p.OrderNumber == orderNumber);
        }
        public List<C_CTZQ_GameIssuse> QueryCTZQGameIssuseListById(string[] array)
        {
            return DB.CreateQuery<C_CTZQ_GameIssuse>().Where(p => array.Contains(p.Id)).ToList();
        }
        public List<C_CTZQ_Match> QueryCTZQMatchListById(string[] array)
        {
            return DB.CreateQuery<C_CTZQ_Match>().Where(p => array.Contains(p.Id)).ToList();
        }
        public List<C_CTZQ_MatchPool> QueryCTZQMatchPoolListById(string[] array)
        {
            return DB.CreateQuery<C_CTZQ_MatchPool>().Where(p => array.Contains(p.Id)).ToList();
        }
        public C_CTZQ_MatchPool QueryCTZQ_MatchPoolByKey(string key)
        {
            return DB.CreateQuery<C_CTZQ_MatchPool>().Where(p => p.Id == key).FirstOrDefault();
        }
        public C_CTZQ_MatchPool QueryCTZQ_MatchPool(string issuseNumber, string gameType)
        {
            return DB.CreateQuery<C_CTZQ_MatchPool>().Where(p => p.IssuseNumber == issuseNumber && p.GameType == gameType && p.BonusLevel == 1).FirstOrDefault();
        }
        public List<CTZQMatchInfo> QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber)
        {
            var query = from s in DB.CreateQuery<C_CTZQ_Match>()
                        where s.GameType == gameType && s.IssuseNumber == issuseNumber
                        select new CTZQMatchInfo
                        {
                            GameCode = s.GameCode,
                            GameType = s.GameType,
                            GuestTeamHalfScore = s.GuestTeamHalfScore,
                            GuestTeamId = s.GuestTeamId,
                            GuestTeamName = s.GuestTeamName,
                            GuestTeamScore = s.GuestTeamScore,
                            GuestTeamStanding = s.GuestTeamStanding.ToString(),
                            HomeTeamHalfScore = s.HomeTeamHalfScore,
                            HomeTeamId = s.HomeTeamId,
                            HomeTeamName = s.HomeTeamName,
                            HomeTeamScore = s.HomeTeamScore,
                            HomeTeamStanding = s.HomeTeamStanding.ToString(),
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
            var query = from i in DB.CreateQuery<C_CTZQ_GameIssuse>()
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
            var query = from i in DB.CreateQuery<C_CTZQ_GameIssuse>()
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
        public CTZQ_IssuseInfo QueryCTZQCurrentIssuse()
        {
            var query = from i in DB.CreateQuery<C_CTZQ_GameIssuse>()
                        where i.StopBettingTime >= DateTime.Now
                        orderby i.StopBettingTime ascending
                        select new CTZQ_IssuseInfo
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
            CTZQMatch_PoolInfo_Collection collection = new CTZQMatch_PoolInfo_Collection
            {
                TotalCount = 0
            };
            var query = from s in DB.CreateQuery<C_CTZQ_Match>()
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
                            GuestTeamStanding = s.GuestTeamStanding.ToString(),
                            HomeTeamHalfScore = s.HomeTeamHalfScore,
                            HomeTeamId = s.HomeTeamId,
                            HomeTeamName = s.HomeTeamName,
                            HomeTeamScore = s.HomeTeamScore,
                            HomeTeamStanding = s.HomeTeamStanding.ToString(),
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
            var query = from s in DB.CreateQuery<C_CTZQ_Match>()
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
