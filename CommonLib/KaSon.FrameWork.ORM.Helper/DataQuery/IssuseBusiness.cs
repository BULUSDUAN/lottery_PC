using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class IssuseBusiness:DBbase
    {

        public C_Game_Issuse QueryWinNumberByIssuseNumber(string gameCode, string gameType, string issuseNumber)
        {
            var query = from b in DB.CreateQuery<C_Game_Issuse>()
                        where b.GameCode == gameCode
                        && (gameType == null || gameType == "" || b.GameType == gameType)
                        && b.IssuseNumber == issuseNumber
                        select b;
            return query.FirstOrDefault();
        }

        public WinNumber_QueryInfoCollection QueryWinNumber(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var gameType = string.Empty;
            if (gameCode.IndexOf("_") >= 0)
            {
                var array = gameCode.Split('_');
                gameCode = array[0].ToUpper();
                gameType = array[1].ToUpper();
            }
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from i in DB.CreateQuery<C_Game_Issuse>()
                        join g in DB.CreateQuery<C_Lottery_Game>() on i.GameCode equals g.GameCode
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
            var Result = new WinNumber_QueryInfoCollection();
            Result.TotalCount = query.Count();
            Result.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return Result;
        }

        public WinNumber_QueryInfoCollection QueryWinNumber(string gameCode, int count)
        {
            var query = from i in DB.CreateQuery<C_Game_Issuse>()
                        join g in DB.CreateQuery<C_Lottery_Game>() on i.GameCode equals g.GameCode
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
            var Result = new WinNumber_QueryInfoCollection();
            Result.List= query.Take(count).ToList();
            return Result;
        }
    }
}
