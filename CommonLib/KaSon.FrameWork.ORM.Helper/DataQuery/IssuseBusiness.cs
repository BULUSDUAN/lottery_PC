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
        public C_CTZQ_GameIssuse QueryWinNumberByIssuseNumber(string gameCode, string gameType, string issuseNumber)
        {
            var query = from b in DB.CreateQuery<C_CTZQ_GameIssuse>()
                        where b.GameCode == gameCode
                        && (gameType==null||gameType=="" || b.GameType == gameType)
                        && b.IssuseNumber == issuseNumber
                        select b;
                        //select new WinNumber_QueryInfo
                        //{
                        //    AwardTime = b.AwardTime,
                        //    GameCode = b.GameCode,
                        //    GameType = b.GameType,
                        //    IssuseNumber = b.IssuseNumber,
                        //    WinNumber = b.WinNumber,
                        //};
            return query.FirstOrDefault();
        }
    }
}
