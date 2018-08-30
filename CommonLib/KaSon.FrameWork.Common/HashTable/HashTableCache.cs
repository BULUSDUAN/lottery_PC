﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.Common
{
  public   class HashTableCache
    {
        public static System.Collections.Hashtable _CTZQHt =  System.Collections.Hashtable.Synchronized(new Hashtable());
        public static Issuse_QueryInfoEX _Issuse_QueryInfo = new Issuse_QueryInfoEX();
        public static System.Collections.Hashtable _IssuseCTZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());

        public static System.Collections.Hashtable _BJDCHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        public static System.Collections.Hashtable _JCZQHt = System.Collections.Hashtable.Synchronized(new Hashtable());
        public static System.Collections.Hashtable _JCLQHt = System.Collections.Hashtable.Synchronized(new Hashtable());

        public static string[] _CTZQType = { "T14C", "T4CJQ", "TR9", "T6BQC" };
        public static string[] _JCZQType = { "SPF", "BRQSPF", "ZJQ", "BF", "BQC","HHDG" };
        public static string[] _JCLQType = { "SF", "RFSF", "DXF", "SFC",  "HHDG" };
        public static string[] _BJDCType = { "SPF"};

        public static void Set_Issuse_QueryInfo(Issuse_QueryInfoEX ex) {

            lock (_Issuse_QueryInfo) {
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
        public static void Init_CTZQ_Data(Issuse_QueryInfoEX ex) {
            //  Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
            string key = "";
            //_CTZQHt.Clear();
            ex = ex == null ? new Issuse_QueryInfoEX() : ex;
            foreach (var item in ex.CTZQ_IssuseNumber)
            {
                var type = item.GameCode_IssuseNumber.Split('|')[1];
                key = type + item.IssuseNumber;
                var result= Json_CTZQ.MatchList_WEB(item.IssuseNumber, type);
                var RemoveKeys = new List<string>();
                foreach (string _keyitem in _CTZQHt.Keys)
                {
                    if (_keyitem.StartsWith(type)) RemoveKeys.Add(_keyitem);
                        //_CTZQHt.Remove(_keyitem);
                }
                foreach (var str in RemoveKeys)
                {
                    _CTZQHt.Remove(str);
                }
                _CTZQHt[key] = result;
            }
          
        }


        public static void Init_CTZQ_Issuse_Data()
        {
            //  Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
            string key = "";
            foreach (var item in _CTZQType)
            {
                key = item ;
                _IssuseCTZQHt[item] = Json_CTZQ.IssuseList(item) ;
            }

        }
        /// <summary>
        /// 北京单场
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_BJDC_Data(string issuseNumber)
        {
            //  Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
            string key = "";
            //_BJDCHt.Clear();
            foreach (var item in _BJDCType)
            {
                key = item  + issuseNumber;//SF+期号
                var result = Json_BJDC.MatchList_WEB(issuseNumber, item);
                _BJDCHt.Clear();
                _BJDCHt[key] = result;
            }

        }
        /// <summary>
        /// 竞猜足球
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_JCZQ_Data(string newVerType=null)
        {
            //  Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
            string key = "";
            foreach (var item in _JCZQType)
            {
                key = item +(newVerType==null? "": newVerType);
             //   _JCZQHt[item] = Json_CTZQ.MatchList_WEB(issuseNumber, item);
                if (item.ToLower() == "hhdg")
                    _JCZQHt[key] = Json_JCZQ.GetJCZQHHDGList();
                else
                    _JCZQHt[key] = Json_JCZQ.MatchList_WEB(item, newVerType);
            }

        }
       
        /// <summary>
        /// 竞猜篮球
        /// </summary>
        /// <param name="issuseNumber"></param>
        public static void Init_JCLQ_Data()
        {
            //  Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
            string key = "";
            foreach (var item in _JCLQType)
            {
                key = item;
              //  _JCLQHt[item] = Json_CTZQ.MatchList_WEB(issuseNumber, item);
                if (item.ToLower() == "hhdg")
                    _JCLQHt[item] = Json_JCLQ.GetJCLQHHDGList();
                else
                    _JCLQHt[item] = Json_JCLQ.MatchList_WEB(item);
            }

        }

        /// <summary>
        /// 开奖信息
        /// </summary>
        public static void Init_Pool_Data() {
              

        }
    }
}
