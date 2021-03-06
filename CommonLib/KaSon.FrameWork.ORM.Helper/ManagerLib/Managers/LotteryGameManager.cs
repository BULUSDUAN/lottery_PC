﻿using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Redis;
using EntityModel.Redis;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// kason
    /// </summary>
    public class LotteryGameManager : DBbase
    {
        public void AddLotteryGame(C_Lottery_Game entity)
        {
            this.DB.GetDal<C_Lottery_Game>().Add(entity);
        }
        public void AddLotteryGameType(C_Lottery_GameType entity)
        {
            this.DB.GetDal<C_Lottery_GameType>().Add(entity);
        }
        public void UpdateGameIssuse(C_Game_Issuse issuse)
        {
           // this.Update<GameIssuse>(issuse);
            this.DB.GetDal<C_Game_Issuse>().Update(issuse);
        }
        public void UpdateLotteryGame(C_Lottery_Game lotteryGame)
        {
            this.DB.GetDal<C_Lottery_Game>().Update(lotteryGame);
        }
        public C_Lottery_Game QueryLotteryGame(string gameCode)
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Lottery_Game>().Where(p => p.GameCode == gameCode).FirstOrDefault();
        }
        public List<C_Lottery_Game> QueryAllGame()
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Lottery_Game>().ToList();
        }

        public List<C_Game_Issuse> QueryLastOpenIssuse(string gameCode, int count)
        {
            //Session.Clear();
            var query = from g in DB.CreateQuery<C_Game_Issuse>()
                        where g.GameCode == gameCode
                        && g.Status == 30

                        orderby g.AwardTime descending
                        select g;
            return query.Take(count).ToList();
        }
        /// <summary>
        /// 加载数字彩开奖号码
        /// </summary>
        public static void LoadSZCWinNumber(string gameCode)
        {
            var gameCodeArray = new string[] { "CQSSC", "JX11X5", "SSQ", "DLT", "FC3D", "PL3", "OZB", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3", "SJB" };
            if (!gameCodeArray.Contains(gameCode))
                return;

            if (gameCode == "OZB")
            {
              //  LoadOZBWinNumber();
            }
            else if (gameCode == "SJB")
            {
               // LoadSJBWinNumber();
            }
            else
            {
                var lastOpenIssuse = new LotteryGameManager().QueryLastOpenIssuse(gameCode, 100);
                var db = RedisHelperEx.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                //清空Key对应的value值
                db.Del(fullKey);

                //写入redis库
                //格式：期号^开奖号
                var array = lastOpenIssuse.Select(p =>string.Format("{0}^{1}", p.IssuseNumber, p.WinNumber)).ToArray();
                //  db.ListRightPushAsync(fullKey, array);
                db.RPush(fullKey, array);
            }
        }


        public IList<LotteryIssuse_QueryInfo> QueryAllGameCurrentIssuse(bool byOfficial)
        {
            //  Session.Clear();
            IList<LotteryIssuse_QueryInfo> list = new List<LotteryIssuse_QueryInfo>();
            var sql = string.Format(@"select g.GameCode,g.IssuseNumber,g.LocalStopTime,g.OfficialStopTime, convert(int, c.ConfigValue)ConfigValue
                        from (
                        SELECT GameCode,min(IssuseNumber)IssuseNumber,min(OfficialStopTime)OfficialStopTime,min(LocalStopTime)LocalStopTime
                          FROM [C_Game_Issuse]
                          where gamecode in ('ssq','dlt','fc3d','pl3','cqssc','jx11x5')
                          and {0}>getdate()
                          group by gamecode
                          ) as g
                          left join [C_Core_Config] c on 'Site.GameDelay.'+g.GameCode=c.configkey", byOfficial ? "OfficialStopTime" : "LocalStopTime");

             list = this.DB.CreateSQLQuery(sql).List<LotteryIssuse_QueryInfo>();
            //if (array == null)
            //    return list;
            //var schemeIdList = new List<string>();
            //foreach (var item in array)
            //{
            //    var row = item as object[];
            //    list.Add(new LotteryIssuse_QueryInfo
            //    {
            //        GameCode = UsefullHelper.GetDbValue<string>(row[0]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(row[1]),
            //        LocalStopTime = UsefullHelper.GetDbValue<DateTime>(row[2]),
            //        OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(row[3]),
            //        GameDelaySecond = UsefullHelper.GetDbValue<int>(row[4]),
            //    });
            //}
            return list;
        }
        public IList<LotteryIssuse_QueryInfo> QueryNextIssuseList(bool byOfficial, string gameCode, int count)
        {
           // Session.Clear();
           // var list = new List<LotteryIssuse_QueryInfo>();
            var sql = string.Format(@"declare  @v int
                                    select @v=convert(int, c.ConfigValue)
                                    from [C_Core_Config] c
                                    where c.configkey='Site.GameDelay.'+'{1}'

                                    select top {2} g.GameCode,g.IssuseNumber,g.LocalStopTime,g.OfficialStopTime,@v ConfigValue
                                     from [C_Game_Issuse]   g     
                                     where g.gameCode='{1}'
                                     and g.{0}> getdate()      
                                     order by g.{0} asc", byOfficial ? "OfficialStopTime" : "LocalStopTime", gameCode, count);

           // var array = this.Session.CreateSQLQuery(sql).List();
          var  list = this.DB.CreateSQLQuery(sql).List<LotteryIssuse_QueryInfo>();
            //if (array == null)
            //    return list;
            //var schemeIdList = new List<string>();
            //foreach (var item in array)
            //{
            //    var row = item as object[];
            //    list.Add(new LotteryIssuse_QueryInfo
            //    {
            //        GameCode = UsefullHelper.GetDbValue<string>(row[0]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(row[1]),
            //        LocalStopTime = UsefullHelper.GetDbValue<DateTime>(row[2]),
            //        OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(row[3]),
            //        GameDelaySecond = UsefullHelper.GetDbValue<int>(row[4]),
            //    });
            //}
            return list;
        }
        /// <summary>
        /// 内存期号
        /// </summary>
       // private static IList<C_Game_Issuse> C_Game_IssuseList = null;
        private static IList<GameTypeInfo> C_GameTypeInfoList = null;

        /// <summary>
        ///  初始化内存期号 k_todo
        /// </summary>
        public  void StartInitData() {
            //if (C_Game_IssuseList == null) {

            //    C_Game_IssuseList = this.DB.CreateQuery<C_Game_Issuse>().ToList();
            //}

            if (C_GameTypeInfoList == null) {

                C_GameTypeInfoList=(from g in DB.CreateQuery<C_Lottery_GameType>()
                 join f in DB.CreateQuery<C_Lottery_Game>() on g.GameCode equals f.GameCode
                 orderby g.GameType
                 select new { g, f }
                    ).ToList().Select(p => new GameTypeInfo
                    {
                        Game = new GameInfo
                        {
                            GameCode = p.g.GameCode,
                            DisplayName = p.f.DisplayName
                        },
                        GameType = p.g.GameType,
                        DisplayName = p.g.DisplayName,
                    }).ToList();
            }

           
        }

        public C_Game_Issuse QueryGameIssuseByKey(string gameCode, string gameType, string issuseNumber)
        {
            //if (C_Game_IssuseList != null)
            //{
            //    var query = from g in C_Game_IssuseList
            //                where g.GameCode == gameCode
            //                && g.IssuseNumber == issuseNumber
            //                && (gameType == "" || g.GameType == gameType)
            //                select g;
            //    return query.FirstOrDefault();
            //}
            //else {
                var query = from g in this.DB.CreateQuery<C_Game_Issuse>()
                            where g.GameCode == gameCode
                            && g.IssuseNumber == issuseNumber
                            && (gameType ==""|| g.GameType == gameType)
                            select g;
                return query.FirstOrDefault();
           // }
          //  Session.Clear();
            //var query = from g in this.DB.CreateQuery<C_Game_Issuse>()
            //            where g.GameCode == gameCode
            //            && g.IssuseNumber == issuseNumber
            //            && (gameType == "" || g.GameType == gameType)
            //            select g;
            //return query.FirstOrDefault();
        }
        /// <summary>
        /// 优化 彩种类型  k_todo
        /// </summary>
        /// <returns></returns>
        public IList<GameTypeInfo> QueryEnableGameTypes()
        {

            if (C_GameTypeInfoList != null)
            {
                return C_GameTypeInfoList;
            }
            else {
                C_GameTypeInfoList = (from g in DB.CreateQuery<C_Lottery_GameType>()
                                      join f in DB.CreateQuery<C_Lottery_Game>() on g.GameCode equals f.GameCode
                                      orderby g.GameType
                                      select new { g, f }
                    ).ToList().Select(p => new GameTypeInfo
                    {
                        Game = new GameInfo
                        {
                            GameCode = p.g.GameCode,
                            DisplayName = p.f.DisplayName
                        },
                        GameType = p.g.GameType,
                        DisplayName = p.g.DisplayName,
                    }).ToList();
            }
            return C_GameTypeInfoList;
        }

        public C_Lottery_Game LoadGame(string gameCode)
        {
            //延时加载
            return DB.CreateQuery<C_Lottery_Game>().Where(p=>p.GameCode==gameCode).FirstOrDefault();
        }

        public C_Game_Issuse QueryCurrentIssuse(string gameCode, string gameType = "")
        {
            var query =DB.CreateQuery<C_Game_Issuse>();
            if (string.IsNullOrEmpty(gameType))
            {
                query = query.Where(p => p.GameCode == gameCode && p.Status == (int)IssuseStatus.OnSale
                          && p.LocalStopTime > DateTime.Now);
            }
            else
            {
                query = query.Where(p => p.GameCode == gameCode && p.Status == (int)IssuseStatus.OnSale
                         && p.LocalStopTime > DateTime.Now && p.GameType == gameType);
            }
            query = query.OrderByDescending(p => p.LocalStopTime);
                //where i.GameCode == gameCode
                //        && (string.IsNullOrEmpty(gameType) || i.GameType == gameType)
                //        && i.Status == (int)IssuseStatus.OnSale
                //        && i.LocalStopTime > DateTime.Now
                //        orderby i.LocalStopTime ascending
                //        select i;
            return query.FirstOrDefault();
        }
        public C_Game_Issuse QueryGameIssuse(string gameCode, string issuseNumber)
        {
          
            return DB.CreateQuery<C_Game_Issuse>().Where(p => p.GameCode == gameCode && p.IssuseNumber == issuseNumber).FirstOrDefault();
        }
    }
}
