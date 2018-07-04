using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper
{
    public class GameCacheBusiness
    {
        private const int _winNumberMaxCount = 100;
        public void Test()
        {
            Stack<string> s = new Stack<string>(100);
            var list = new List<string>();
            var dic = new Dictionary<string, Issuse_QueryInfo>();
            //dic.Remove(
        }

        private static Dictionary<string, GameTypeInfoCollection> _gameTypeDictionary = new Dictionary<string, GameTypeInfoCollection>();
        ///// <summary>
        ///// 查询彩种所有玩法信息
        ///// </summary>
        ///// <param name="gameCode">彩种编码</param>
        //internal static GameTypeInfoCollection GetGameType(string gameCode)
        //{
        //    if (!_gameTypeDictionary.ContainsKey(gameCode))
        //        _gameTypeDictionary.Add(gameCode, new GameBusiness().QueryGameTypeList(gameCode));
        //    return _gameTypeDictionary[gameCode];
        //}

        private static Dictionary<string, Issuse_QueryInfo> _issuseDictionary = new Dictionary<string, Issuse_QueryInfo>();

        public static void ClearIssuseCache()
        {
            if (_issuseDictionary != null)
                _issuseDictionary.Clear();
        }

        /// <summary>
        /// 获取当前奖期信息
        /// </summary>
        public static Issuse_QueryInfo GetCurrentIssuserInfo(string gameCode)
        {
            if (_issuseDictionary.Keys.Contains(gameCode))
            {
                var existIssuse = _issuseDictionary.FirstOrDefault(d => d.Key == gameCode);
                if (existIssuse.Value != null && existIssuse.Value.LocalStopTime > DateTime.Now)
                    return existIssuse.Value;
            }

            var issuse = new GameBusiness().QueryCurrentIssuseInfo(gameCode);
            _issuseDictionary.Remove(gameCode);
            _issuseDictionary.Add(gameCode, issuse);
            return issuse;
        }
        private static Dictionary<string, Issuse_QueryInfo> _issuseOficalDictionary = new Dictionary<string, Issuse_QueryInfo>();
        private static object _lckObj_IssuseOffical = new object();
        private static object _lckObj_IssuseOffical2 = new object();
        ///// <summary>
        ///// 以官方结束时间为准 获取当前奖期信息
        ///// </summary>
        //internal static Issuse_QueryInfo GetCurrentIssuseInfoWithOffical(string gameCode)
        //{
        //    lock (_lckObj_IssuseOffical)
        //    {
        //        if (!_issuseOficalDictionary.ContainsKey(gameCode)
        //            || _issuseOficalDictionary[gameCode] == null
        //            || _issuseOficalDictionary[gameCode].OfficialStopTime < DateTime.Now)
        //        {
        //            lock (_lckObj_IssuseOffical2)
        //            {
        //                _issuseOficalDictionary[gameCode] = new GameBusiness().QueryCurrentIssuseInfoWithOffical(gameCode);
        //            }
        //        }
        //        return _issuseOficalDictionary[gameCode];
        //    }
        //}

        //internal static Issuse_QueryInfo QueryCurretNewIssuseInfo(string gameCode, string gameType)
        //{
        //    lock (_lckObj_IssuseOffical)
        //    {
        //        if (!_issuseOficalDictionary.ContainsKey(gameCode)
        //            || _issuseOficalDictionary[gameCode] == null
        //            || _issuseOficalDictionary[gameCode].OfficialStopTime < DateTime.Now)
        //        {
        //            lock (_lckObj_IssuseOffical2)
        //            {
        //                _issuseOficalDictionary[gameCode] = new GameBusiness().QueryCurrentNewIssuseInfo(gameCode, gameType);
        //            }
        //        }
        //        return _issuseOficalDictionary[gameCode];
        //    }
        //}

        //private static Dictionary<string, WinNumber_QueryInfo> _newWinNumbers = new Dictionary<string, WinNumber_QueryInfo>();
        ///// <summary>
        ///// 查询各彩种最新开奖号码
        ///// </summary>
        //internal static WinNumber_QueryInfo GetNewWinNumber(string gameCode)
        //{
        //    lock (_newWinNumbers)
        //    {
        //        if (!_newWinNumbers.ContainsKey(gameCode))
        //        {
        //            var tmp = gameCode.Split('_');
        //            if (tmp.Length == 2)
        //            {
        //                var t = new IssuseBusiness().QueryNewWinNumber(tmp[0], tmp[1]);
        //                if (t == null) return null;
        //                _newWinNumbers.Add(gameCode, t);
        //            }
        //            else
        //            {
        //                var t = new IssuseBusiness().QueryNewWinNumber(tmp[0], "");
        //                if (t == null) return null;
        //                _newWinNumbers.Add(gameCode, t);
        //            }
        //        }
        //        return _newWinNumbers[gameCode];
        //    }
        //}
        //private static Dictionary<string, Stack<WinNumber_QueryInfo>> _winNumber = new Dictionary<string, Stack<WinNumber_QueryInfo>>();
        ///// <summary>
        ///// 按彩种、期号获取开奖信息
        ///// </summary>
        //internal static WinNumber_QueryInfo GetWinNumber(string gameCode, string issuseNumber)
        //{
        //    lock (_winNumber)
        //    {
        //        if (_winNumber.Count == 0)
        //        {
        //            //初始数据 
        //            GameBusiness.GameList.ForEach((g) =>
        //            {
        //                var list = new Stack<WinNumber_QueryInfo>(_winNumberMaxCount);
        //                var t = GetNewWinNumber(g.GameCode);
        //                if (t != null)
        //                {
        //                    list.Push(t);
        //                    _winNumber.Add(g.GameCode, list);
        //                }
        //            });
        //        }
        //        if (!_winNumber.ContainsKey(gameCode))
        //        {
        //            //初始数据 
        //            var list = new Stack<WinNumber_QueryInfo>(_winNumberMaxCount);
        //            var t = GetNewWinNumber(gameCode);
        //            if (t != null)
        //            {
        //                list.Push(t);
        //                _winNumber.Add(gameCode, list);
        //            }
        //        }
        //        if (_winNumber.ContainsKey(gameCode))
        //        {
        //            var info = _winNumber[gameCode].FirstOrDefault(w => w.IssuseNumber == issuseNumber);
        //            if (info == null)
        //            {
        //                var t_gameCode = gameCode;
        //                var t_gameType = string.Empty;
        //                var t = gameCode.Split('_');
        //                if (t.Length == 2)
        //                {
        //                    t_gameCode = t[0];
        //                    t_gameType = t[1];
        //                }
        //                var entity = new IssuseBusiness().QueryWinNumberByIssuseNumber(t_gameCode, t_gameType, issuseNumber);
        //                return new WinNumber_QueryInfo
        //                {
        //                    GameCode = gameCode,
        //                    IssuseNumber = issuseNumber,
        //                    WinNumber = entity == null ? string.Empty : entity.WinNumber,
        //                };
        //            }
        //            return info;
        //        }
        //        return new WinNumber_QueryInfo
        //        {
        //            GameCode = gameCode,
        //        };
        //    }
        //}
        ///// <summary>
        ///// 查询全部彩种最新开奖号码
        ///// </summary>
        //internal static WinNumber_QueryInfoCollection GetAllNewWinNumbers()
        //{
        //    var list = new WinNumber_QueryInfoCollection();
        //    list.TotalCount = GameBusiness.GameList.Count;
        //    GameBusiness.GameList.ForEach((g) =>
        //    {
        //        if (g.GameCode.Equals("CTZQ"))
        //        {
        //            list.List.Add(GetNewWinNumber(g.GameCode + "_T14C"));
        //            list.List.Add(GetNewWinNumber(g.GameCode + "_T4CJQ"));
        //            list.List.Add(GetNewWinNumber(g.GameCode + "_T6BQC"));
        //        }
        //        else
        //        {
        //            list.List.Add(GetNewWinNumber(g.GameCode));
        //        }
        //    });
        //    return list;
        //}
        ///// <summary>
        ///// 设置最新开奖号码
        ///// </summary>
        //internal static void SetNewWinNumber(string gameCode, string issuseNumber, string winNumber, DateTime awardTime)
        //{
        //    var gameInfo = GameBusiness.GameList.FirstOrDefault(g => g.GameCode == gameCode);
        //    if (gameInfo == null)
        //        throw new Exception(string.Format("找不到彩种：{0}", gameCode));

        //    //最新开奖号码
        //    if (_newWinNumbers.ContainsKey(gameCode))
        //        _newWinNumbers.Remove(gameCode);
        //    _newWinNumbers.Add(gameCode, new WinNumber_QueryInfo
        //    {
        //        AwardTime = awardTime,
        //        DisplayName = gameInfo.DisplayName,
        //        GameCode = gameCode,
        //        IssuseNumber = issuseNumber,
        //        WinNumber = winNumber,
        //    });

        //    //历史开奖号码
        //    if (!_winNumber.ContainsKey(gameCode))
        //    {
        //        //初始数据 
        //        var list = new Stack<WinNumber_QueryInfo>(_winNumberMaxCount);
        //        list.Push(GetNewWinNumber(gameCode));
        //        _winNumber.Add(gameCode, list);
        //    }
        //    if (_winNumber[gameCode].Count >= 100)
        //        _winNumber[gameCode].Pop();
        //    _winNumber[gameCode].Push(new WinNumber_QueryInfo
        //    {
        //        AwardTime = awardTime,
        //        DisplayName = gameInfo.DisplayName,
        //        GameCode = gameCode,
        //        IssuseNumber = issuseNumber,
        //        WinNumber = winNumber,
        //    });
        //}

        ///// <summary>
        ///// 及时查询最新开奖没缓存
        ///// </summary>
        //public WinNumber_QueryInfo GetNewWinNumberFirst(string gameCode, string gameType)
        //{
        //    return new IssuseBusiness().GetNewWinNumberFirst(gameCode, gameType);
        //}

        //#region 查询开奖历史

        //private static Dictionary<string, WinNumber_QueryInfoCollection> _winNumberHistory = new Dictionary<string, WinNumber_QueryInfoCollection>();
        //private static object _lckObj_winNumberHistory = new object();
        //public static WinNumber_QueryInfoCollection QueryWinNumberHistory(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    lock (_lckObj_winNumberHistory)
        //    {
        //        var key = gameCode + "-" + startTime.ToString("yyyyMMdd") + "-" + endTime.ToString("yyyyMMdd") + "-" + pageIndex + "-" + pageSize;
        //        if (!_winNumberHistory.ContainsKey(key))
        //        {
        //            _winNumberHistory.Add(key, new IssuseBusiness().QueryWinNumberHistory(gameCode, startTime, endTime, pageIndex, pageSize));
        //        }
        //        return _winNumberHistory[key];
        //    }
        //}
        //public static WinNumber_QueryInfoCollection QueryWinNumberHistoryByCount(string gameCode, int count)
        //{
        //    var key = string.Format("{0}-{1}", gameCode, count);
        //    if (!_winNumberHistory.ContainsKey(key))
        //    {
        //        _winNumberHistory.Add(key, new IssuseBusiness().QueryWinNumberHistoryByCount(gameCode, count));
        //    }
        //    return _winNumberHistory[key];
        //}
        //public static void RefreshWinNumberHistory(string gameCode)
        //{
        //    var tmpList = _winNumberHistory.Keys.Where((key) =>
        //    {
        //        return key.StartsWith(gameCode + "-");
        //    });
        //    var list = new List<string>(tmpList);
        //    for (var i = list.Count - 1; i >= 0; i--)
        //    {
        //        var key = list[i];
        //        _winNumberHistory.Remove(key);
        //    }
        //}

        //#endregion

        //public static LotteryIssuse_QueryInfoCollection QueryAllGameCurrentIssuse(bool byOfficial)
        //{
        //    var coll = new LotteryIssuse_QueryInfoCollection();
        //    var list = new LotteryGameManager().QueryAllGameCurrentIssuse(byOfficial);
        //    coll.AddRange(list);
        //    return coll;
        //}
    }
}
