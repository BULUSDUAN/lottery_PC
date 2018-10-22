using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using EntityModel.CoreModel;

using Newtonsoft.Json;
using KaSon.FrameWork.Common.Redis;
using System.Linq;

namespace KaSon.FrameWork.Common
{
    public class HashTableCache
    {
        //public static System.Collections.Hashtable _CTZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        //public static Issuse_QueryInfoEX _Issuse_QueryInfo = new Issuse_QueryInfoEX();
        //public static System.Collections.Hashtable _IssuseCTZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());

        //public static System.Collections.Hashtable _BJDCHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        //public static System.Collections.Hashtable _JCZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        //public static System.Collections.Hashtable _JCLQHt = System.Collections.Hashtable.Synchronized(new Hashtable());

        public static string[] _CTZQType = { "T14C", "T4CJQ", "TR9", "T6BQC" };
        public static string[] _JCZQType = { "SPF", "BRQSPF", "ZJQ", "BF", "BQC", "HHDG","HH" };
        public static string[] _JCLQType = { "SF", "RFSF", "DXF", "SFC", "HHDG", "HH" };
        public static string[] _BJDCType = { "SPF" };

        //public static void Set_Issuse_QueryInfo(Issuse_QueryInfoEX ex)
        //{

        //    lock (_Issuse_QueryInfo)
        //    {
        //        _Issuse_QueryInfo = ex;
        //    }
        //}


        //public static void ClearHashTable()
        //{
        //    _CTZQHt.Clear();
        //    _BJDCHt.Clear();
        //}

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
               RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
            }

        }


        public static void Init_CTZQ_Issuse_Data()
        {
            string key = EntityModel.Redis.RedisKeys.Key_CTZQ_Issuse_List;
            foreach (var item in _CTZQType)//"T14C", "T4CJQ", "TR9", "T6BQC"
            {
                string reidskey = $"{key}_{item}";
                var result = Json_CTZQ.IssuseList(item);
                RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
            }

        }
        /// <summary>
        /// 北京单场
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_BJDC_Data(string issuseNumber)
        {
            string key = EntityModel.Redis.RedisKeys.Key_BJDC_Match_Odds_List;
            foreach (var item in _BJDCType)
            {
                string reidskey = $"{key}_{item}_{issuseNumber}";//SF+期号
                var result = Json_BJDC.MatchList_WEB(issuseNumber, item);
                RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
            }

        }
        /// <summary>
        /// 竞猜足球
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_JCZQ_Data(string newVerType = null)
        {
            string key = EntityModel.Redis.RedisKeys.Key_JCZQ_Match_Odds_List;
            foreach (var item in _JCZQType) //"SPF", "BRQSPF", "ZJQ", "BF", "BQC","HHDG"
            {
                string reidskey = key + "_" + item + (newVerType == null ? "" : newVerType);
                if (item.ToLower() == "hhdg")
                {
                    var result = Json_JCZQ.GetJCZQHHDGList();
                    RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var result = Json_JCZQ.MatchList_WEB(item, newVerType);
                    #region 新逻辑20181022
                    //如果gametype为让分胜负与大小分，则需要拼装他们的state_hhdg
                    if (item.ToLower() == "brqspf")
                    {
                        var oddlist_jczq_hhdg = Json_JCZQ.GetJCZQHHDGList();
                        if (result != null && oddlist_jczq_hhdg != null)
                        {
                            foreach (var brqitem in result)
                            {
                                var hhdgitem = oddlist_jczq_hhdg.FirstOrDefault(c => c.MatchId == brqitem.MatchId);
                                if (hhdgitem != null) brqitem.State_HHDG = hhdgitem.State_HHDG;
                            }
                        }
                    }
                    #endregion
                    RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
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
            foreach (var item in _JCLQType)
            {
                string reidskey = $"{key}_{item}";
                if (item.ToLower() == "hhdg")
                {
                    var result = Json_JCLQ.GetJCLQHHDGList();
                    RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
                }
                else
                {
                    var result = Json_JCLQ.MatchList_WEB(item);
                    #region 新逻辑20181022
                    //新逻辑20181022
                    //如果gametype为让分胜负与大小分，则需要拼装他们的state_hhdg
                    if (item.ToLower() == "rfsf" || item.ToLower() == "dxf")
                    {
                        var oddlist_jclq_hhdg = Json_JCLQ.GetJCLQHHDGList();
                        if (result != null && oddlist_jclq_hhdg != null)
                        {
                            foreach (var typeitem in result)
                            {
                                var hhdgitem = oddlist_jclq_hhdg.FirstOrDefault(c => c.MatchId == typeitem.MatchId);
                                if (hhdgitem != null) typeitem.State_HHDG = hhdgitem.State_HHDG;
                            }
                        }
                    }
                    #endregion
                    RedisHelperEx.DB_Match.SetObj(reidskey, result, TimeSpan.FromMinutes(30));
                }
            }

        }

       
    }
}
