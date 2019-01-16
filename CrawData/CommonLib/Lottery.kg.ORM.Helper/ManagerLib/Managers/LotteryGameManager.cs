using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
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
       
        public void UpdateLotteryGame(C_Lottery_Game lotteryGame)
        {
            this.DB.GetDal<C_Lottery_Game>().Update(lotteryGame);
        }
        public C_Lottery_Game QueryLotteryGame(string gameCode)
        {
           // Session.Clear();
            return this.DB.CreateQuery<C_Lottery_Game>().FirstOrDefault(p => p.GameCode == gameCode);
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

        public C_Game_Issuse QueryGameIssuseByKey(string gameCode, string gameType, string issuseNumber)
        {
          //  Session.Clear();
            var query = from g in this.DB.CreateQuery<C_Game_Issuse>()
                        where g.GameCode == gameCode
                        && g.IssuseNumber == issuseNumber
                        && (gameType == string.Empty || g.GameType == gameType)
                        select g;
            return query.FirstOrDefault();
        }
    }
}
