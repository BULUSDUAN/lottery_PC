using EntityModel.CoreModel;
using KaSon.FrameWork.ORM.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.BusinessLib.Hk6
{
   public class HK6BetingSevice
    {
        private IDbProvider DB;
        public HK6BetingSevice(IDbProvider db) {
            DB = db;
        }

        /// <summary>
        /// 普通订单缓存数据
        /// </summary>
        private static Dictionary<string, LotteryBettingInfo> _bettingListInfo = new Dictionary<string, LotteryBettingInfo>();

        /// <summary>
        /// 检查普通订单频繁投注
        /// </summary>
        public string CheckGeneralRepeatBetting(string currUserId, LotteryBettingInfo info)
        {
            try
            {
                //todo:备用 info.IsSubmit = false;
                if (_bettingListInfo == null || !_bettingListInfo.ContainsKey(currUserId))
                {
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                var cacheInfo = _bettingListInfo.FirstOrDefault(s => s.Key == currUserId && s.Value.GameCode == info.GameCode.ToUpper() && s.Value.SchemeSource == info.SchemeSource && s.Value.BettingCategory == info.BettingCategory && s.Value.TotalMoney == info.TotalMoney);
                if (string.IsNullOrEmpty(cacheInfo.Key) || cacheInfo.Value == null)
                {
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                if (!info.Equals(cacheInfo.Value))
                {
                    //不重复
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                //投注内容相同
                if (info.IsRepeat)
                {
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                var timeSpan = DateTime.Now - cacheInfo.Value.CurrentBetTime;
                if (timeSpan.TotalSeconds > 5)
                {
                    //大于间隔时间
                    _bettingListInfo.Remove(currUserId);
                    info.CurrentBetTime = DateTime.Now;
                    _bettingListInfo.Add(currUserId, info);
                    return string.Empty;
                }
                return "Repeat";
            }
            catch (Exception)
            {
                _bettingListInfo.Clear();
                return string.Empty;
            }
        }

    }
}
