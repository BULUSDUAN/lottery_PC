using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using EntityModel.CoreModel;
using EntityModel.Communication;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public abstract class LotteryDataBusiness:DBbase
    {
        public T CreateNewEntity<T>(Dictionary<string, object> specialProperty, Func<PropertyInfo, object> callBack) //where T : ImportBase
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

        public static ILotteryDataBusiness GetTypeImport(string gameCode)
        {
            ILotteryDataBusiness biz = null;
            try
            {
             
                
                    switch (gameCode)
                    {
                        case "SSQ":
                            biz = new LotteryDataBusiness_SSQ();

                            break;
                           case "HK6":
                            biz = new LotteryDataBusiness_HK6();

                            break;
                        case "DLT":
                            biz = new LotteryDataBusiness_DLT();
                            break;
                        case "FC3D":
                            biz = new LotteryDataBusiness_FC3D();
                            break;
                        case "PL3":
                            biz = new LotteryDataBusiness_PL3();
                            break;
                        case "CQSSC":
                            biz = new LotteryDataBusiness_CQSSC();
                            break;
                        case "JX11X5":
                            biz = new LotteryDataBusiness_JX11X5();
                            break;
                        case "CQ11X5":
                            biz = new LotteryDataBusiness_CQ11X5();
                            break;
                        case "CQKLSF":
                            biz = new LotteryDataBusiness_CQKLSF();
                            break;
                        case "DF6J1":
                            biz = new LotteryDataBusiness_DF6_1();
                            break;
                        case "GD11X5":
                            biz = new LotteryDataBusiness_GD11X5();
                            break;
                        case "GDKLSF":
                            biz = new LotteryDataBusiness_GDKLSF();
                            break;
                        case "HBK3":
                            biz = new LotteryDataBusiness_HBK3();
                            break;
                        case "HC1":
                            biz = new LotteryDataBusiness_HC1();
                            break;
                        case "HD15X5":
                            biz = new LotteryDataBusiness_HD15X5();
                            break;
                        case "HNKLSF":
                            biz = new LotteryDataBusiness_HNKLSF();
                            break;
                        case "JLK3":
                            biz = new LotteryDataBusiness_JLK3();
                            break;
                        case "JSKS":
                            biz = new LotteryDataBusiness_JSK3();
                            break;
                        case "JXSSC":
                            biz = new LotteryDataBusiness_JXSSC();
                            break;
                        case "LN11X5":
                            biz = new LotteryDataBusiness_LN11X5();
                            break;
                        case "PL5":
                            biz = new LotteryDataBusiness_PL5();
                            break;
                        case "QLC":
                            biz = new LotteryDataBusiness_QLC();
                            break;
                        case "QXC":
                            biz = new LotteryDataBusiness_QXC();
                            break;
                        case "SDQYH":
                            biz = new LotteryDataBusiness_SDQYH();
                            break;
                        case "SD11X5":
                            biz = new LotteryDataBusiness_YDJ11();
                            break;
                        case "SDKLPK3":
                            biz = new LotteryDataBusiness_SDKLPK3();
                            break;
                        case "CTZQ_T14C":
                        case "CTZQ_TR9":
                        case "CTZQ_T6BQC":
                        case "CTZQ_T4CJQ":
                            biz = new LotteryDataBusiness_CTZQ(gameCode);
                            break;
                        default:
                            throw new Exception(string.Format("未找到匹配的接口：{0}", gameCode));
                    
                    
                }
              //  biz.ImportWinNumber(issuseNumber, winNumber);
              //  return new CommonActionResult(true, "导入成功");
            }
            catch (Exception ex)
            {
               // return new CommonActionResult(false, "导入失败 " + ex.ToString());
            }

            return biz;
        }

    }

    public interface ILotteryDataBusiness
    {
        string CurrentGameCode { get; }
        void ImportWinNumber(string issuseNumber, string winNumber);
    }

}
