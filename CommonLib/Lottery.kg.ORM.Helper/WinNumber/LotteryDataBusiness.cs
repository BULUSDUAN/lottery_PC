using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using EntityModel.CoreModel;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber
{
    public abstract class LotteryDataBusiness:DBbase
    {
        public T CreateNewEntity<T>(Dictionary<string, object> specialProperty, Func<PropertyInfo, object> callBack) where T : ImportBase
        {
            var entity = Activator.CreateInstance<T>();
            var currentType = typeof(T);
            foreach (var p in currentType.GetProperties())
            {
                if (specialProperty.ContainsKey(p.Name))
                {
                    p.SetValue(entity, specialProperty[p.Name], null);
                    continue;
                }
                var newValue = callBack(p);
                p.SetValue(entity, newValue, null);
            }

            return entity;
        }

        private static Dictionary<string, object> _cacheGameChart = new Dictionary<string, object>();
        public List<T> QueryGameChart<T>(string key, Func<List<T>> doQuery) where T : class
        {
            var result = new List<T>();
            //不包括Key值
            if (!_cacheGameChart.ContainsKey(key))
            {
                result = doQuery();
                _cacheGameChart.Add(key, result);
            }
            //包括Key值
            result = _cacheGameChart[key] as List<T>;
            if (result == null || result.Count == 0)
            {
                result = doQuery();
                _cacheGameChart.Remove(key);
                _cacheGameChart.Add(key, result);
            }
            return result;
        }

        public void ClearGameChartCache(string key)
        {
            foreach (var k in _cacheGameChart.Keys.Where(p => p.Contains(key)).ToList())
            {
                if (_cacheGameChart.ContainsKey(k))
                    _cacheGameChart.Remove(k);
            }
        }

        private static Dictionary<string, GameWinNumber_InfoCollection> _cacheNewWinNumber = new Dictionary<string, GameWinNumber_InfoCollection>();
        /// <summary>
        /// 查询最新开奖号码并做缓存
        /// </summary>
        public GameWinNumber_InfoCollection QueryNewWinNumber(string key, Func<GameWinNumber_InfoCollection> doQuery)
        {
            if (!_cacheNewWinNumber.ContainsKey(key))
            {
                var collection = doQuery();
                _cacheNewWinNumber.Add(key, collection);
            }
            return _cacheNewWinNumber[key];
        }

        public void ClearNewWinNumberCache(string fuzzyKey)
        {
            foreach (var key in _cacheNewWinNumber.Keys.Where(p => p.Contains(fuzzyKey)).ToList())
            {
                if (_cacheNewWinNumber.ContainsKey(key))
                    _cacheNewWinNumber.Remove(key);
            }
        }
        private static Dictionary<string, object> _cacheOmissionData = new Dictionary<string, object>();
        public T QueryOmissionData<T>(string key, Func<T> doQuery) where T : new()
        {
            var result = new T();
            if (!_cacheOmissionData.ContainsKey(key))
            {
                result = doQuery();
                ClearCacheOmissionData(key);
                _cacheOmissionData.Add(key, result);
            }
            result = (T)_cacheOmissionData[key];
            if (result == null)
            {
                result = doQuery();
                ClearCacheOmissionData(key);
                _cacheOmissionData.Add(key, result);
            }
            return result;
        }
        private void ClearCacheOmissionData(string key)
        { 
            var array=key.Split('_');
            if(array==null||array.Length<=0)
                return;
            var query = _cacheOmissionData.Where(s => s.Key.StartsWith(array[0] + "_" + array[1]));
            if (query != null && query.ToList().Count > 0)
            {
                foreach (var item in query.ToList())
                {
                    _cacheOmissionData.Remove(item.Key);
                }
            }
        }
    }

    public interface ILotteryDataBusiness
    {
        string CurrentGameCode { get; }
        void ImportWinNumber(string issuseNumber, string winNumber);
    }

}
