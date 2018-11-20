using EntityModel;
using EntityModel.Enum;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common
{
   public class MgMatchDataHelper: MgHelper
    {
       /// <summary>
       /// 赛事
       /// </summary>
       /// <param name="issuse"></param>
       /// <returns></returns>
        public static IList<C_BJDC_Match> BJDC_Match_List_ByIssuse(string issuse) {
            var filter = Builders<C_BJDC_Match>.Filter.Eq(b => b.IssuseNumber, issuse) & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);
            
            return MgDB.GetCollection<C_BJDC_Match>("BJDC_Match_List").Find<C_BJDC_Match>(filter).ToList();

        }
        /// <summary>
        /// 赛果
        /// </summary>
        /// <param name="issuse"></param>
        /// <returns></returns>
        public static IList<C_BJDC_MatchResult> BJDC_MatchResult_List_ByIssuse(string issuse)
        {
            var filter = Builders<C_BJDC_MatchResult>.Filter.Eq(b => b.IssuseNumber, issuse);// & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);

            return MgDB.GetCollection<C_BJDC_MatchResult>("BJDC_Match_List").Find<C_BJDC_MatchResult>(filter).ToList();

        }
    }
}
