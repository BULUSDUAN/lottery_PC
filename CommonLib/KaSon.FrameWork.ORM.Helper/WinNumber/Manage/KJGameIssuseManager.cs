﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class KJGameIssuseManager : DBbase
    {
        /// <summary>
        /// 添加奖期数据
        /// </summary>
        public void AddKJGameIssuse(params Common_Game_Issuse[] issuse)
        {
            LottertDataDB.GetDal<Common_Game_Issuse>().Add(issuse);
        }

        public void UpdateKJGameIssuse(Common_Game_Issuse entity)
        {
            LottertDataDB.GetDal<Common_Game_Issuse>().Update(entity);           
        }

        public Common_Game_Issuse QueryKJGameIssuse(string gameCode, string issuseNumber)
        {
           
            return LottertDataDB.CreateQuery<Common_Game_Issuse>().Where(p => p.GameCode == gameCode && p.IssuseNumber == issuseNumber).FirstOrDefault();
        }

        public KJGameIssuse QueryCurrentIssuseWithOffical(string gameCode)
        {            

            var sql = string.Format(@"select i.* 
                                    from Common_Game_Issuse i
                                    inner join(
                                    select min(GameCode_IssuseNumber) as GameCode_IssuseNumber
                                    from Common_Game_Issuse
                                    where GameCode='{0}'
                                    and Status=10
                                    and OfficialStopTime > GETDATE()
                                    ) b on i.GameCode_IssuseNumber = b.GameCode_IssuseNumber", gameCode);
            return LottertDataDB.CreateSQLQuery(sql).List<KJGameIssuse>().FirstOrDefault();
            //foreach (var item in list)
            //{
            //    var array = item as object[];
            //    return new KJGameIssuse()
            //    {
            //        GameCode_IssuseNumber = UsefullHelper.GetDbValue<string>(array[0]),
            //        GameCode = UsefullHelper.GetDbValue<string>(array[1]),
            //        GameType = UsefullHelper.GetDbValue<string>(array[2]),
            //        IssuseNumber = UsefullHelper.GetDbValue<string>(array[3]),
            //        StartTime = UsefullHelper.GetDbValue<DateTime>(array[4]),
            //        LocalStopTime = UsefullHelper.GetDbValue<DateTime>(array[5]),
            //        GatewayStopTime = UsefullHelper.GetDbValue<DateTime>(array[6]),
            //        OfficialStopTime = UsefullHelper.GetDbValue<DateTime>(array[7]),
            //        Status = UsefullHelper.GetDbValue<IssuseStatus>(array[8]),
            //        WinNumber = UsefullHelper.GetDbValue<string>(array[9]),
            //        AwardTime = UsefullHelper.GetDbValue<DateTime>(array[10]),
            //        CreateTime = UsefullHelper.GetDbValue<DateTime>(array[11]),
            //    };
            //}
            //return null;
        }

        public void DeleteIssuseData(string gameCode, string[] issuseNumber)
        {
            string strSql = "delete from Common_Game_Issuse where @GameCode=gameCode and IssuseNumber in ('{0}')";
            strSql = string.Format(strSql,string.Join(",",issuseNumber));
            var result = LottertDataDB.CreateSQLQuery(strSql)
                   .SetString("@gameCode", gameCode).Excute();                           
        }
    }
}
