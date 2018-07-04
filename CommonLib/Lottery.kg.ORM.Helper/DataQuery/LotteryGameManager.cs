using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel.CoreModel;
using GameBiz.Domain.Entities;

namespace Lottery.Kg.ORM.Helper
{
    public class LotteryGameManager : GameBizEntityManagement
    {
        public void AddLotteryGame(LotteryGame entity)
        {
            this.Add<LotteryGame>(entity);
        }
        public void AddLotteryGameType(LotteryGameType entity)
        {
            this.Add<LotteryGameType>(entity);
        }
        public void AddLoginUser(SystemUser user)
        {
            this.Add<SystemUser>(user);
        }
        public void AddGameIssuse(params GameIssuse[] issuse)
        {
            this.Add<GameIssuse>(issuse);
        }
        public void UpdateLotteryGame(LotteryGame lotteryGame)
        {
            this.Update<LotteryGame>(lotteryGame);
        }
        public LotteryGame QueryLotteryGame(string gameCode)
        {
            Session.Clear();
            return this.Session.Query<LotteryGame>().FirstOrDefault(p => p.GameCode == gameCode);
        }
        public List<LotteryGame> QueryAllGame()
        {
            Session.Clear();
            return this.Session.Query<LotteryGame>().ToList();
        }

        public void UpdateGameIssuse(GameIssuse issuse)
        {
            this.Update<GameIssuse>(issuse);
        }
        public void AddOrUpdateGameIssuse(params GameIssuse[] issuse)
        {
            foreach (var i in issuse)
            {
                var entity = QueryGameIssuse(i.GameCode, i.IssuseNumber);
                if (entity == null)
                {
                    Add<GameIssuse>(i);
                }
            }
            //this.AddOrIgnore<GameIssuse>(issuse);
        }

        public GameIssuse GetGameIssuse(string key)
        {
            //Get 查不到返回空值
            return this.GetByKey<GameIssuse>(key);
        }

        public IssuseStatus QueryIssuseStatus(string key)
        {
            Session.Clear();
            return (from g in this.Session.Query<GameIssuse>() where g.GameCode_IssuseNumber == key select g.Status).FirstOrDefault();
        }
        public GameIssuse QueryGameIssuse(string gameCode, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<GameIssuse>().FirstOrDefault(p => p.GameCode == gameCode && p.IssuseNumber == issuseNumber);
        }
        public GameIssuse QueryGameIssuseByKey(string gameCode, string gameType, string issuseNumber)
        {
            Session.Clear();
            var query = from g in this.Session.Query<GameIssuse>()
                        where g.GameCode == gameCode
                        && g.IssuseNumber == issuseNumber
                        && (gameType == string.Empty || g.GameType == gameType)
                        select g;
            return query.FirstOrDefault();
        }
        public GameIssuse LoadGameIssuseByKey(string key)
        {
            Session.Clear();
            return Session.Load<GameIssuse>(key, NHibernate.LockMode.Read);
        }
        public GameIssuse QueryCurrentIssuse(string gameCode, string gameType = "")
        {
            Session.Clear();
            //var hql = "FROM GameIssuse WHERE GameCode = :GameCode AND Status = :Status AND LocalStopTime > :LocalStopTime";
            //return Session.CreateQuery(hql)
            //    .SetString("GameCode", gameCode)
            //    .SetInt32("Status", (int)IssuseStatus.OnSale)
            //    .SetDateTime("LocalStopTime", DateTime.Now)
            //    .SetFirstResult(0)
            //    .SetMaxResults(1)
            //    .SetCacheable(false)
            //    .SetCacheMode(NHibernate.CacheMode.Get)
            //    .UniqueResult<GameIssuse>();

            //Session.GetNamedQuery("")..ExecuteUpdate()

            var query = from i in this.Session.Query<GameIssuse>()
                        where i.GameCode == gameCode
                        && (string.IsNullOrEmpty(gameType) || i.GameType == gameType)
                        && i.Status == IssuseStatus.OnSale
                        && i.LocalStopTime > DateTime.Now

                        orderby i.LocalStopTime ascending
                        select i;
            return query.FirstOrDefault();
        }

        public GameIssuse QueryCTZQCurrentIssuse(string gameType)
        {
            Session.Clear();
            var query = from i in this.Session.Query<GameIssuse>()
                        where i.GameCode == "CTZQ" && i.GameType == gameType
                        && i.Status == IssuseStatus.OnSale
                        && i.LocalStopTime > DateTime.Now
                        orderby i.LocalStopTime ascending
                        select i;
            return query.FirstOrDefault();
        }

        public GameIssuse QueryCurrentIssuseWithOffical(string gameCode)
        {
            Session.Clear();

            var sql = string.Format(@"select i.* 
                                    from C_Game_Issuse i
                                    inner join(
                                    select min(GameCode_IssuseNumber) as GameCode_IssuseNumber
                                    from C_Game_Issuse
                                    where GameCode='{0}'
                                    and Status=10
                                    and OfficialStopTime > GETDATE()
                                    ) b on i.GameCode_IssuseNumber = b.GameCode_IssuseNumber", gameCode);
            var list = this.Session.CreateSQLQuery(sql).List();
            foreach (var item in list)
            {
                var array = item as object[];
                return new GameIssuse()
                {
                    GameCode_IssuseNumber = UsefullHelper.GetDbValue<string>(array[0]),
                    GameCode = UsefullHelper.GetDbValue<string>(array[1]),
                    GameType = UsefullHelper.GetDbValue<string>(array[2]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(array[3]),
                    StartTime = UsefullHelper.GetDbValue<DateTime>(array[4]),
                    LocalStopTime = UsefullHelper.GetDbValue<DateTime>(array[5]),
                    GatewayStopTime = UsefullHelper.GetDbValue<DateTime>(array[6]),
                    OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(array[7]),
                    Status = UsefullHelper.GetDbValue<IssuseStatus>(array[8]),
                    WinNumber = UsefullHelper.GetDbValue<string>(array[9]),
                    AwardTime = UsefullHelper.GetDbValue<DateTime>(array[10]),
                    CreateTime = UsefullHelper.GetDbValue<DateTime>(array[11]),
                };
            }
            return null;
        }

        public GameIssuse QueryIssuse(string gameCode, string gameType, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<GameIssuse>().FirstOrDefault(p => p.GameCode == gameCode && (gameType == "" || p.GameType == gameType) && p.IssuseNumber == issuseNumber);
        }

        public GameIssuse QueryCurrentNewIssuseInfo(string gameCode, string gameType)
        {
            Session.Clear();

            var sql = string.Format(@"select i.* 
                                    from C_Game_Issuse i
                                    inner join(
                                    select min(GameCode_IssuseNumber) as GameCode_IssuseNumber
                                    from C_Game_Issuse
                                    where GameCode='{0}' and GameType='{1}'
                                    and Status=10
                                    and OfficialStopTime > GETDATE()
                                    ) b on i.GameCode_IssuseNumber = b.GameCode_IssuseNumber", gameCode, gameType);
            var list = this.Session.CreateSQLQuery(sql).List();
            foreach (var item in list)
            {
                var array = item as object[];
                return new GameIssuse()
                {
                    GameCode_IssuseNumber = UsefullHelper.GetDbValue<string>(array[0]),
                    GameCode = UsefullHelper.GetDbValue<string>(array[1]),
                    GameType = UsefullHelper.GetDbValue<string>(array[2]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(array[3]),
                    StartTime = UsefullHelper.GetDbValue<DateTime>(array[4]),
                    LocalStopTime = UsefullHelper.GetDbValue<DateTime>(array[5]),
                    GatewayStopTime = UsefullHelper.GetDbValue<DateTime>(array[6]),
                    OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(array[7]),
                    Status = UsefullHelper.GetDbValue<IssuseStatus>(array[8]),
                    WinNumber = UsefullHelper.GetDbValue<string>(array[9]),
                    AwardTime = UsefullHelper.GetDbValue<DateTime>(array[10]),
                    CreateTime = UsefullHelper.GetDbValue<DateTime>(array[11]),
                };
            }
            return null;
        }
        public List<GameIssuse> QueryGameIssuseByKey(string[] array)
        {
            Session.Clear();
            return this.Session.Query<GameIssuse>().Where(p => array.Contains(p.GameCode_IssuseNumber)).ToList();
        }

        public GameIssuse QueryLastUnSaleGameIssuse(string gameCode, out int count)
        {
            Session.Clear();
            var query = from g in this.Session.Query<GameIssuse>()
                        where g.GameCode == gameCode && g.Status == IssuseStatus.OnSale
                        orderby g.IssuseNumber descending
                        select g;
            count = query.Count();
            return query.FirstOrDefault();
        }
        public List<LotteryGameInfo> QueryLotteryGameList()
        {
            Session.Clear();
            var query = from g in this.Session.Query<LotteryGame>()
                        orderby g.GameCode
                        select new LotteryGameInfo
                        {
                            DisplayName = g.DisplayName,
                            EnableStatus = g.EnableStatus,
                            GameCode = g.GameCode
                        };
            return query.ToList();
        }
        public GameInfo QueryGameInfo(string gameCode)
        {
            Session.Clear();
            return (from g in this.Session.Query<LotteryGame>()
                    where g.GameCode == gameCode
                    orderby g.GameCode
                    select new GameInfo
                    {
                        GameCode = g.GameCode,
                        DisplayName = g.DisplayName
                    }
             ).Cacheable().FirstOrDefault();
        }
        public IList<GameInfo> QueryEnableGame()
        {
            Session.Clear();
            return (from g in this.Session.Query<LotteryGame>()
                    where g.EnableStatus == Common.EnableStatus.Enable
                    orderby g.GameCode
                    select new GameInfo
                    {
                        GameCode = g.GameCode,
                        DisplayName = g.DisplayName
                    }
             ).Cacheable().ToList<GameInfo>();
        }

        public LotteryGame LoadGame(string gameCode)
        {
            Session.Clear();
            //延时加载
            return Session.Load<LotteryGame>(gameCode, NHibernate.LockMode.Read);
        }

        public IList<GameTypeInfo> QueryEnableGameType(string gameCode)
        {
            Session.Clear();
            return (from g in this.Session.Query<LotteryGameType>()
                    orderby g.GameType
                    where g.Game.GameCode == gameCode
                    select new GameTypeInfo
                    {
                        Game = new GameInfo
                        {
                            GameCode = g.Game.GameCode,
                            DisplayName = g.Game.DisplayName
                        },
                        GameType = g.GameType,
                        DisplayName = g.DisplayName,
                    }
                    ).Cacheable().ToList<GameTypeInfo>();
        }

        public IList<GameTypeInfo> QueryEnableGameTypes()
        {
            Session.Clear();
            return (from g in this.Session.Query<LotteryGameType>()
                    orderby g.GameType
                    select new GameTypeInfo
                    {
                        Game = new GameInfo
                        {
                            GameCode = g.Game.GameCode,
                            DisplayName = g.Game.DisplayName
                        },
                        GameType = g.GameType,
                        DisplayName = g.DisplayName,
                    }
                    ).Cacheable().ToList<GameTypeInfo>();
        }

        public GameTypeInfo QueryGameType(string gameCode, string gameType)
        {
            Session.Clear();
            return (from g in this.Session.Query<LotteryGameType>()
                    orderby g.GameType
                    where g.Game.GameCode == gameCode && g.GameType == gameType
                    select new GameTypeInfo
                    {
                        Game = new GameInfo
                        {
                            GameCode = g.Game.GameCode,
                            DisplayName = g.Game.DisplayName
                        },
                        GameType = g.GameType,
                        DisplayName = g.DisplayName,
                    }
                    ).Cacheable().First();
        }

        public List<WinNumber_QueryInfo> QueryWinNumber(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var gameType = string.Empty;
            if (gameCode.IndexOf("_") >= 0)
            {
                var array = gameCode.Split('_');
                gameCode = array[0].ToUpper();
                gameType = array[1].ToUpper();
            }
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from i in this.Session.Query<GameIssuse>()
                        join g in this.Session.Query<LotteryGame>() on i.GameCode equals g.GameCode
                        orderby i.IssuseNumber descending
                        where i.GameCode == gameCode && (gameType == string.Empty || i.GameType == gameType) && i.WinNumber != string.Empty && i.WinNumber != null
                        && i.AwardTime >= startTime && i.AwardTime < endTime
                        select new WinNumber_QueryInfo
                        {
                            AwardTime = i.AwardTime,
                            GameCode = g.GameCode,
                            DisplayName = g.DisplayName,
                            IssuseNumber = i.IssuseNumber,
                            WinNumber = i.WinNumber,
                            GameType = i.GameType,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<WinNumber_QueryInfo> QueryWinNumber(string gameCode, int count)
        {
            Session.Clear();
            var query = from i in this.Session.Query<GameIssuse>()
                        join g in this.Session.Query<LotteryGame>() on i.GameCode equals g.GameCode
                        orderby i.IssuseNumber descending
                        where i.GameCode == gameCode && i.WinNumber != string.Empty && i.WinNumber != null
                        select new WinNumber_QueryInfo
                        {
                            AwardTime = i.AwardTime,
                            GameCode = g.GameCode,
                            DisplayName = g.DisplayName,
                            IssuseNumber = i.IssuseNumber,
                            WinNumber = i.WinNumber,
                        };
            return query.Take(count).ToList();
        }

        public WinNumber_QueryInfo QueryWinNumber(string gameCode, string gameType)
        {
            Session.Clear();
            var query = from b in this.Session.Query<GameIssuse>()
                        where b.GameCode == gameCode && (string.IsNullOrEmpty(gameType) || b.GameType == gameType) && b.WinNumber != ""
                        orderby b.AwardTime descending
                        select new WinNumber_QueryInfo
                        {
                            AwardTime = b.AwardTime,
                            GameCode = b.GameCode,
                            GameType = b.GameType,
                            IssuseNumber = b.IssuseNumber,
                            WinNumber = b.WinNumber,
                        };

            return query.FirstOrDefault();
        }
        public WinNumber_QueryInfo QueryWinNumberByIssuseNumber(string gameCode, string gameType, string issuseNumber)
        {
            Session.Clear();
            var query = from b in this.Session.Query<GameIssuse>()
                        where b.GameCode == gameCode
                        && (string.IsNullOrEmpty(gameType) || b.GameType == gameType)
                        && b.IssuseNumber == issuseNumber
                        select new WinNumber_QueryInfo
                        {
                            AwardTime = b.AwardTime,
                            GameCode = b.GameCode,
                            GameType = b.GameType,
                            IssuseNumber = b.IssuseNumber,
                            WinNumber = b.WinNumber,
                        };
            return query.FirstOrDefault();
        }

        public UserRegInfo QueryUserRegister(string userId)
        {
            Session.Clear();
            return (from c in this.Session.Query<UserRegister>()
                    where c.UserId == userId
                    select new UserRegInfo
                    {
                        DisplayName = c.DisplayName,
                        AgentId = c.AgentId,
                        ComeFrom = c.ComeFrom,
                        Referrer = c.Referrer,
                        ReferrerUrl = c.ReferrerUrl,
                        RegisterIp = c.RegisterIp,
                        RegType = c.RegType,
                    }).FirstOrDefault();
        }

        public string QueryPrizedIssuseList(string gameCode, string gameType, int length)
        {
            Session.Clear();
            var query = from g in this.Session.Query<GameIssuse>()
                        where g.GameCode == gameCode
                        && (gameType == "" || gameType == g.GameType)
                        && g.Status == IssuseStatus.Stopped
                        orderby g.IssuseNumber descending
                        select g.IssuseNumber;
            return string.Join(",", query.Take(length).ToList());
        }

        public string QueryStopIssuseList(string gameCode, string gameType, int length)
        {
            Session.Clear();
            var query = from g in this.Session.Query<GameIssuse>()
                        where g.GameCode == gameCode
                        && (gameType == "" || gameType == g.GameType)
                        && g.OfficialStopTime < DateTime.Now.AddHours(-1)
                        orderby g.IssuseNumber descending
                        select g.IssuseNumber;
            return string.Join(",", query.Take(length).ToList());
        }

        public void DeleteIssuseData(string gameCode, string[] issuseNumber)
        {
            string strSql = "delete from C_Game_Issuse where GameCode=:gameCode and IssuseNumber in (:issuseNumber)";
            var result = Session.CreateSQLQuery(strSql)
                   .SetString("gameCode", gameCode)
                   .SetParameterList("issuseNumber", issuseNumber).ExecuteUpdate();
        }
        public List<GameIssuse> QueryGameIssuseListByGameCode(List<string> gameCodeList)
        {
            Session.Clear();
            return Session.Query<GameIssuse>().Where(s => gameCodeList.Contains(s.GameCode)).ToList();
        }

        public List<LotteryIssuse_QueryInfo> QueryAllGameCurrentIssuse(bool byOfficial)
        {
            Session.Clear();
            var list = new List<LotteryIssuse_QueryInfo>();
            var sql = string.Format(@"select g.GameCode,g.IssuseNumber,g.LocalStopTime,g.OfficialStopTime, convert(int, c.ConfigValue)ConfigValue
                        from (
                        SELECT GameCode,min(IssuseNumber)IssuseNumber,min(OfficialStopTime)OfficialStopTime,min(LocalStopTime)LocalStopTime
                          FROM [C_Game_Issuse]
                          where gamecode in ('ssq','dlt','fc3d','pl3','cqssc','jx11x5')
                          and {0}>getdate()
                          group by gamecode
                          ) as g
                          left join [C_Core_Config] c on 'Site.GameDelay.'+g.GameCode=c.configkey", byOfficial ? "OfficialStopTime" : "LocalStopTime");

            var array = this.Session.CreateSQLQuery(sql).List();
            if (array == null)
                return list;
            var schemeIdList = new List<string>();
            foreach (var item in array)
            {
                var row = item as object[];
                list.Add(new LotteryIssuse_QueryInfo
                {
                    GameCode = UsefullHelper.GetDbValue<string>(row[0]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(row[1]),
                    LocalStopTime = UsefullHelper.GetDbValue<DateTime>(row[2]),
                    OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(row[3]),
                    GameDelaySecond = UsefullHelper.GetDbValue<int>(row[4]),
                });
            }
            return list;
        }

        /// <summary>
        /// 查询未来的奖期信息
        /// </summary>
        public List<LotteryIssuse_QueryInfo> QueryNextIssuseList(bool byOfficial, string gameCode, int count)
        {
            Session.Clear();
            var list = new List<LotteryIssuse_QueryInfo>();
            var sql = string.Format(@"declare  @v int
                                    select @v=convert(int, c.ConfigValue)
                                    from [C_Core_Config] c
                                    where c.configkey='Site.GameDelay.'+'{1}'

                                    select top {2} g.GameCode,g.IssuseNumber,g.LocalStopTime,g.OfficialStopTime,@v ConfigValue
                                     from [C_Game_Issuse]   g     
                                     where g.gameCode='{1}'
                                     and g.{0}> getdate()      
                                     order by g.{0} asc", byOfficial ? "OfficialStopTime" : "LocalStopTime", gameCode, count);

            var array = this.Session.CreateSQLQuery(sql).List();
            if (array == null)
                return list;
            var schemeIdList = new List<string>();
            foreach (var item in array)
            {
                var row = item as object[];
                list.Add(new LotteryIssuse_QueryInfo
                {
                    GameCode = UsefullHelper.GetDbValue<string>(row[0]),
                    IssuseNumber = UsefullHelper.GetDbValue<string>(row[1]),
                    LocalStopTime = UsefullHelper.GetDbValue<DateTime>(row[2]),
                    OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(row[3]),
                    GameDelaySecond = UsefullHelper.GetDbValue<int>(row[4]),
                });
            }
            return list;
        }

        public List<GameIssuse> QueryLastOpenIssuse(string gameCode, int count)
        {
            Session.Clear();
            var query = from g in Session.Query<GameIssuse>()
                        where g.GameCode == gameCode
                        && g.Status == IssuseStatus.Stopped

                        orderby g.AwardTime descending
                        select g;
            return query.Take(count).ToList();
        }

    }
}