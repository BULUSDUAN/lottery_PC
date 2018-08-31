using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common.Redis;
using Newtonsoft.Json;

namespace KaSon.FrameWork.Common
{
    public class HashTableCache
    {
        public static System.Collections.Hashtable _CTZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        public static Issuse_QueryInfoEX _Issuse_QueryInfo = new Issuse_QueryInfoEX();
        public static System.Collections.Hashtable _IssuseCTZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());

        public static System.Collections.Hashtable _BJDCHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        public static System.Collections.Hashtable _JCZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        public static System.Collections.Hashtable _JCLQHt = System.Collections.Hashtable.Synchronized(new Hashtable());

        public static string[] _CTZQType = { "T14C", "T4CJQ", "TR9", "T6BQC" };
        public static string[] _JCZQType = { "SPF", "BRQSPF", "ZJQ", "BF", "BQC", "HHDG" };
        public static string[] _JCLQType = { "SF", "RFSF", "DXF", "SFC", "HHDG" };
        public static string[] _BJDCType = { "SPF" };

        public static void Set_Issuse_QueryInfo(Issuse_QueryInfoEX ex)
        {

            lock (_Issuse_QueryInfo)
            {
                _Issuse_QueryInfo = ex;
            }
        }


        public static void ClearHashTable()
        {
            _CTZQHt.Clear();
            _BJDCHt.Clear();
        }

        /// <summary>
        /// 传统足球
        /// </summary>
        /// <param name="IssuseNumber"></param>
        public static void Init_CTZQ_Data(Issuse_QueryInfoEX ex)
        {
            string key = EntityModel.Redis.RedisKeys.Key_CTZQ_Match_Odds_List;
            ex = ex == null ? new Issuse_QueryInfoEX() : ex;
            foreach (var item in ex.CTZQ_IssuseNumber)
            {
                var type = item.GameCode_IssuseNumber.Split('|')[1];
                string reidskey = $"{key}_{type}_{item.IssuseNumber}";
                var result = Json_CTZQ.MatchList_WEB(item.IssuseNumber, type);
                RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                //if (result != null && result.Count > 0)
                //{
                //    string data = JsonConvert.SerializeObject(result);
                //    db.StringSet(reidskey, data, TimeSpan.FromMinutes(30));
                //}
                //else
                //{
                //    db.StringSet(reidskey, "", TimeSpan.FromMinutes(30));
                //}
            }

        }


        public static void Init_CTZQ_Issuse_Data()
        {
            var db = RedisHelper.DB_Match;
            string key = EntityModel.Redis.RedisKeys.Key_CTZQ_Issuse_List;
            foreach (var item in _CTZQType)//"T14C", "T4CJQ", "TR9", "T6BQC"
            {
                string reidskey = $"{key}_{item}";
                var result = Json_CTZQ.IssuseList(item);
                RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                //var obj = RedisHelper.DB_Match.Get(key) as List<EntityModel.LotteryJsonInfo.CtzqIssuesWeb>;
                //if (result != null && result.Count > 0)
                //{
                //    string data = JsonConvert.SerializeObject(result);
                //    db.StringSet(reidskey, data, TimeSpan.FromMinutes(30));
                //}
                //else
                //{
                //    db.StringSet(reidskey, "", TimeSpan.FromMinutes(30));
                //}
            }

        }
        /// <summary>
        /// 北京单场
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_BJDC_Data(string issuseNumber)
        {
            var db = RedisHelper.DB_Match;
            string key = EntityModel.Redis.RedisKeys.Key_BJDC_Match_Odds_List;
            foreach (var item in _BJDCType)
            {
                string reidskey = $"{key}_{item}_{issuseNumber}";//SF+期号
                var result = Json_BJDC.MatchList_WEB(issuseNumber, item);
                RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                //if (result != null && result.Count > 0)
                //{
                //    string data = JsonConvert.SerializeObject(result);
                //    db.StringSet(reidskey, data, TimeSpan.FromMinutes(30));
                //}
                //else
                //{
                //    db.StringSet(reidskey, "", TimeSpan.FromMinutes(30));
                //}
            }

        }
        /// <summary>
        /// 竞猜足球
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_JCZQ_Data(string newVerType = null)
        {
            string key = EntityModel.Redis.RedisKeys.Key_JCZQ_Match_Odds_List;
            var db = RedisHelper.DB_Match;
            foreach (var item in _JCZQType) //"SPF", "BRQSPF", "ZJQ", "BF", "BQC","HHDG"
            {
                string reidskey = key + "_" + item + (newVerType == null ? "" : newVerType);
                if (item.ToLower() == "hhdg")
                {
                    var result = Json_JCZQ.GetJCZQHHDGList();
                    RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var result = Json_JCZQ.MatchList_WEB(item, newVerType);
                    RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                }
            }

        }

        /// <summary>
        /// 竞猜篮球
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_JCLQ_Data()
        {
            string key = EntityModel.Redis.RedisKeys.Key_JCLQ_Match_Odds_List;
            var db = RedisHelper.DB_Match;
            foreach (var item in _JCLQType)
            {
                string reidskey = $"{key}_{item}";
                if (item.ToLower() == "hhdg")
                {
                    var result = Json_JCLQ.GetJCLQHHDGList();
                    RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var result = Json_JCLQ.MatchList_WEB(item);
                    RedisHelper.DB_Match.Set(reidskey, result, TimeSpan.FromMinutes(30));
                }
            }

        }

        /// <summary>
        /// 开奖信息
        /// </summary>
        public static void Init_Pool_Data()
        {


        }
    }
}
