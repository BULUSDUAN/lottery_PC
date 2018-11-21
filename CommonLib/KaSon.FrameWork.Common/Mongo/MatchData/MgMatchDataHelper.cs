using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Domain.Entities;
using EntityModel.Enum;
using EntityModel.Interface;
using EntityModel.LotteryJsonInfo;
using MongoDB.Bson;
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
        public static List<C_BJDC_Match> BJDC_Match_List_ByIssuse(string issuse) {
            var filter = Builders<C_BJDC_Match>.Filter.Eq(b => b.IssuseNumber, issuse) & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);
            
            return MgDB.GetCollection<C_BJDC_Match>("BJDC_Match_List").Find<C_BJDC_Match>(filter).ToList();

        }

        /// <summary>
        /// 赛事
        /// </summary>
        /// <param name="issuse"></param>
        /// <returns></returns>
        public static List<JCZQ_MatchInfo> JCZQ_Match_List_FB()
        {
            var filter = Builders<JCZQ_MatchInfo>.Filter.Empty;// & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);

            return MgDB.GetCollection<JCZQ_MatchInfo>("JCZQ_Match_List_FB").Find<JCZQ_MatchInfo>(filter).ToList();

        }
        public static List<JCZQ_MatchInfo> JCZQ_Match_List(string tableName= "JCZQ_Match_List", string MatchData = "")
        {
            FilterDefinition<JCZQ_MatchInfo> filter=  Builders<JCZQ_MatchInfo>.Filter.Empty; ;
            if (!string.IsNullOrEmpty(MatchData))
            {
                filter = Builders<JCZQ_MatchInfo>.Filter.Eq(b=>b.MatchData, MatchData); 

            }
            //var filter = Builders<JCLQ_MatchInfo>.Filter.Empty;// & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);

            return MgDB.GetCollection<JCZQ_MatchInfo>(tableName).Find<JCZQ_MatchInfo>(filter).ToList();

        }
        public static List<C_JCZQ_MatchResult> JCZQ_MatchResult(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return MgHelper.MgDB.GetCollection<C_JCZQ_MatchResult>("JCZQ_Match_Result_List").Find<C_JCZQ_MatchResult>(Builders<C_JCZQ_MatchResult>.Filter.Empty).ToList();

                //
            }
            else
            {
                return MgHelper.MgDB.GetCollection<C_JCZQ_MatchResult>("JCZQ_Match_Result_List").Find<C_JCZQ_MatchResult>(Builders<C_JCZQ_MatchResult>.Filter.Eq(b => b.MatchData, matchDate)).ToList();

            }
        }
                public static List<CTZQ_MatchInfo> CTZQ_Match_List(string type, string issuse)
        {
            var filter_ZJQ = Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.GameType, type) & Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
            return MgHelper.MgDB.GetCollection<CTZQ_MatchInfo>("CTZQ_MatchInfo").Find<CTZQ_MatchInfo>(filter_ZJQ).ToList();

        }
        /// <summary>
        /// 赛事
        /// </summary>
        /// <param name="issuse"></param>
        /// <returns></returns>
        public static List<C_BJDC_Match> BJDC_Match_List_ByIssuseSales(string issuse)
        {
            var filter = Builders<C_BJDC_Match>.Filter.Eq(b => b.IssuseNumber, issuse);// & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);

            return MgDB.GetCollection<C_BJDC_Match>("BJDC_Match_List").Find<C_BJDC_Match>(filter).ToList();

        }
        /// <summary>
        /// 赛果
        /// </summary>
        /// <param name="issuse"></param>
        /// <returns></returns>
        public static List<C_BJDC_MatchResult> BJDC_MatchResult_List_ByIssuse(string issuse)
        {
            var filter = Builders<C_BJDC_MatchResult>.Filter.Eq(b => b.IssuseNumber, issuse);// & Builders<C_BJDC_Match>.Filter.Eq(b => b.MatchState, (int)BJDCMatchState.Sales);

            return MgDB.GetCollection<C_BJDC_MatchResult>("BJDC_MatchResult_List").Find<C_BJDC_MatchResult>(filter).ToList();

        }

        /// <summary>
        /// 北京单场赔率
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="issuse"></param>
        /// <returns></returns>
        public static List<T> BJDC_SP<T>(string type, string issuse)
        {

            ///
            ///BJDC_SP_SPF  胜平负SPF
            ///BJDC_SP_ZJQ  总进球ZJQ
            ///BJDC_SP_SXDS  上下单双SXDS
            ///BJDC_SP_BF  比分BF
            ///BJDC_SP_BQC  半全场BJBQC
            ///
            object list = null;

            switch (type)
            {

                case "ZJQ":
                    var filter_ZJQ = Builders<BJDC_ZJQ_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_ZJQ_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_ZJQ_SpInfo>(filter_ZJQ).ToList();

                    break;
                case "SXDS":
                    var filter_SXDS = Builders<BJDC_SXDS_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_SXDS_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_SXDS_SpInfo>(filter_SXDS).ToList();

                    break;
                case "BF":
                    var filter_BF = Builders<BJDC_BF_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_BF_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_BF_SpInfo>(filter_BF).ToList();

                    break;
                case "BQC":
                    var filter_BQC = Builders<BJDC_BQC_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_BQC_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_BQC_SpInfo>(filter_BQC).ToList();

                    break;
                default://BJDC_SP_SPF
                    var filter = Builders<BJDC_SPF_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
                    list = MgHelper.MgDB.GetCollection<BJDC_SPF_SpInfo>("BJDC_SP_" + type.ToUpper()).Find<BJDC_SPF_SpInfo>(filter).ToList();
                    break;

            }


            return (List<T>)list;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">///JCLQ_SP ///JCLQ_SF_SPInfo,JCLQ_RFSF_SPInfo,JCLQ_SFC_SPInfo,JCLQ_DXF_SPInfo</typeparam>
        /// <param name="type"></param>
        /// <param name="matchdate"></param>
        /// <returns></returns>

        public static List<T> JCLQ_SP<T>(string type, string matchdate = null)where T: IMatchData
        { 
            if (type.ToLower() == "hh")
            {
                //  
                var filter = Builders<T>.Filter.Empty;
                return  MgHelper.MgDB.GetCollection<T>("JCLQ_SP").Find<T>(filter).ToList();
            }
            else if (string.IsNullOrEmpty(matchdate))
            {
                //return "/MatchData/" + "jclq/" + type + "_SP.json";

                List<T> obj = null;
                switch (type.ToUpper())
                {
                   

                    case "SF":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;
                    case "RFSF":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;
                    case "SFC":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;

                    default://DXF
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();

                        break;
                }
                return obj;
            }
            else
            {
               

                object obj = null;

                switch (type.ToUpper())
                {
                    case "SF":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        var filter_SF = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_SF).ToList();
                        break;
                    case "RFSF":
                        var filter_RFSF = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_RFSF).ToList();
                        break;
                    case "SFC":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        var filter_SFC = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_SFC).ToList();
                        break;

                    default://DXF
                        var filter = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter).ToList();

                        break;
                }
                return (List<T>)obj;

                // return "/MatchData/" + "/jclq/" + matchdate + "/" + type + "_SP.json";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">
        ///JCZQ_SP, JCZQ_SPF_SPInfo ,JCZQ_SPF_SPInfo,JCZQ_BF_SPInfo,JCZQ_ZJQ_SPInfo,JCZQ_BQC_SPInfo
        /// </typeparam>
        /// <param name="type"></param>
        /// <param name="matchdate"></param>
        /// <returns></returns>
        public static List<T> JCZQ_SP<T>(string type, string matchdate = null) where T : IMatchData
        {
            if (type.ToLower() == "hh")
            {
                //JCZQ_SP
                var filter = Builders<T>.Filter.Empty;
                return MgHelper.MgDB.GetCollection<T>("JCZQ_SP").Find<T>(filter).ToList();
            }
            else if (string.IsNullOrEmpty(matchdate))
            {
                //return "/MatchData/" + "jclq/" + type + "_SP.json";

                List<T> obj = null;
                switch (type.ToUpper())
                {
                    case "SPF":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;
                    case "BRQSPF":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;
                    case "BF":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;
                    case "ZJQ":
                        //   var filter = Builders<JCLQ_SP>.Filter.Empty;
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();
                        break;

                    default://DXF
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(Builders<T>.Filter.Empty).ToList();

                        break;
                }
                return obj;
            }
            else
            {

                object obj = null;

                switch (type.ToUpper())
                {
                    case "SPF":
                        var filter_SPF = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_SPF).ToList();
                        break;
                    case "BRQSPF":
                        var filter_BRQSPF = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_BRQSPF).ToList();
                        break;
                    case "BF":
                        var filter_BF = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_BF).ToList();
                        break;
                    case "ZJQ":
                        var filter_ZJQ = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_ZJQ).ToList();
                        break;

                    default://DXF
                        var filter_BQC = Builders<T>.Filter.Eq(b => b.MatchData, matchdate);
                        obj = MgHelper.MgDB.GetCollection<T>("JCLQ_" + type.ToUpper() + "_SP").Find<T>(filter_BQC).ToList();

                        break;
                }
                return (List<T>)obj;

                // return "/MatchData/" + "/jclq/" + matchdate + "/" + type + "_SP.json";
            }
        }

        public static List<CTZQ_OddInfo> CTZQ_Odd(string type, string issuse)
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter = Builders<BsonDocument>.Filter.Eq("GameType", type) & Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuse);
            var document = MgHelper.MgDB.GetCollection<BsonDocument>("C_CTZQ_Odds").Find(filter).FirstOrDefault();
            string text = "";
            if (document != null)
            {
                text = document["Content"].ToString().Trim();
                return JsonHelper.Deserialize<List<CTZQ_OddInfo>>(text);
            }
            return new List<CTZQ_OddInfo>();
        }


        public static string SZC_BonusLevelInfo(string type, string issuse)
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");

            var filter_BQC = Builders<BsonDocument>.Filter.Eq("GameCode", type) &
                  Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuseId);
            var doc = MgHelper.MgDB.GetCollection<BsonDocument>("SZCBonusPool").Find<BsonDocument>(filter_BQC).FirstOrDefault();

            if (doc != null)
            {
                return doc["Content"].ToString();
            }

            return "";
        }

        public static List<CTZQ_BonusLevelInfo> CTZQ_BonusLevelInfo(string type, string issuse)
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter_ZJQ = Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.GameType, type) & Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.IssuseNumber, issuse);
            return MgHelper.MgDB.GetCollection<CTZQ_BonusLevelInfo>("CTZQ_BonusLevelInfo").Find<CTZQ_BonusLevelInfo>(filter_ZJQ).ToList();
        }

        public static List<CTZQ_IssuseInfo> CTZQ_Issuse(string type,string issuseId="")
        {
            //  var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var filter_ZJQ = Builders<CTZQ_IssuseInfo>.Filter.Eq(b => b.GameType, type);
            if (string.IsNullOrEmpty(issuseId))
            {
                filter_ZJQ = Builders<CTZQ_IssuseInfo>.Filter.Eq(b => b.GameType, type) & Builders<CTZQ_IssuseInfo>.Filter.Eq(b => b.IssuseNumber, issuseId);
            }
            return MgHelper.MgDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo").Find<CTZQ_IssuseInfo>(filter_ZJQ).ToList();
        }
        public static List<JCLQ_MatchResult> JCLQ_MatchResult(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                var filter = Builders<JCLQ_MatchResult>.Filter.Empty;
                return MgHelper.MgDB.GetCollection<JCLQ_MatchResult>("JCLQ_Match_Result_List").Find<JCLQ_MatchResult>(filter).ToList();
            }
            else
            {
                var filter = Builders<JCLQ_MatchResult>.Filter.Eq(b => b.MatchData, matchDate);
                return MgHelper.MgDB.GetCollection<JCLQ_MatchResult>("JCLQ_Match_Result_List").Find<JCLQ_MatchResult>(filter).ToList();
            }
        }
        public static List<JCLQ_MatchInfo> JCLQ_MatchList(string type, string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {

                if (type.ToLower() == "sf")
                {
                     //var filter_ZJQ = Builders<JCLQ_Match_SF>.Filter.Eq(b => b., type) & Builders<JCLQ_Match_SF>.Filter.Eq(b => b.IssuseNumber, issuse);
                    return MgHelper.MgDB.GetCollection<JCLQ_MatchInfo>("JCLQ_Match_" + type.ToUpper() + "_List").Find<JCLQ_MatchInfo>(Builders<JCLQ_MatchInfo>.Filter.Empty).ToList();

                }
                return MgHelper.MgDB.GetCollection<JCLQ_MatchInfo>("JCLQ_Match_" + type.ToUpper() + "_List").Find<JCLQ_MatchInfo>(Builders<JCLQ_MatchInfo>.Filter.Empty).ToList();
                //return "/MatchData/" + "jclq/Match_" + (type.ToLower() == "sf" ? "sf" : "rfsf") + "_List.json";20150519 sf读取match_list文件
                // return "/MatchData/" + "jclq/Match_" + (type.ToLower() == "sf" ? "" : "rfsf_") + "List.json";
            }
            else
            {
                // return "/MatchData/" + "/jclq/" + matchDate + "/Match_List.json";
                var filter = Builders<JCLQ_MatchInfo>.Filter.Eq(b => b.MatchData, matchDate);
                return MgHelper.MgDB.GetCollection<JCLQ_MatchInfo>("JCLQ_Match_" + type.ToUpper() + "_List").Find<JCLQ_MatchInfo>(filter).ToList();

             //   return (List<T>)obj;
            }
        }
        public static List<JCLQ_Match_HHDG> JCLQ_Match_HHDG_List()
        {
            return MgHelper.MgDB.GetCollection<JCLQ_Match_HHDG>("JCLQ_Match_HHDG_List").Find<JCLQ_Match_HHDG>(Builders<JCLQ_Match_HHDG>.Filter.Empty).ToList();
        }
        public static List<C_JCZQ_Match_HH> JCZQ_Match_HH_List()
        {//JCZQ_Match_List_HH 
            var filter_BQC = Builders<C_JCZQ_Match_HH>.Filter.Empty;
            return MgHelper.MgDB.GetCollection<C_JCZQ_Match_HH>("JCZQ_Match_List_HH").Find<C_JCZQ_Match_HH>(filter_BQC).ToList();

        }
      
    }
}
