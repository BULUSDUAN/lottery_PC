using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper.BusinessLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EntityModel.Redis;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.AutoTask
{
    public class AutoTaskServices
    {
        /// <summary>
        /// 缓存bjdc,jclq,jczq
        /// </summary>
        public static void AutoCaheData(int seconds)
        {
            Task.WhenAll(new Task[] {
                     CTZQ_BJDC(),
                        JCLQ(),
                        JCZQ(),
                        StartTaskByWriteChaseOrderToDb(seconds),
                        Init_Pool_Data(),
                        Repair_SZCAddToRedis()
            });
        }

        public static async Task CTZQ_BJDC()
        {
            while (true)
            {
                try
                {
                    EntityModel.CoreModel.Issuse_QueryInfoEX val = GameServiceCache.QueryCurretNewIssuseInfoByList();
                    try
                    {
                        HashTableCache.Init_CTZQ_Issuse_Data();
                    }
                    catch (Exception ex)
                    {
                        //获取期号出错

                    }

                    HashTableCache.Init_CTZQ_Data(val);
                    HashTableCache.Init_BJDC_Data(val.BJDC_IssuseNumber.IssuseNumber);
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(30000);
            }
        }

        public static async Task JCLQ()
        {
            while (true)
            {
                try
                {
                    HashTableCache.Init_JCLQ_Data();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(30000);
            }
        }

        public static async Task JCZQ()
        {
            while (true)
            {
                try
                {
                    HashTableCache.Init_JCZQ_Data("1");
                    HashTableCache.Init_JCZQ_Data();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(30000);
            }
        }

        /// <summary>
        /// 处理追号订单
        /// </summary>
        /// <param name="Sports_SchemeJobSeconds"></param>
        public static async Task StartTaskByWriteChaseOrderToDb(int seconds)
        {
            while (true)
            {
                try
                {
                    //Console.WriteLine(string.Format("追号作业启动...每{0}秒执行一次", seconds));
                    Sports_BusinessBy.WriteChaseOrderToDb();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(10000 * seconds);
            }
        }

        /// <summary>
        /// 开奖信息
        /// </summary>
        public static async Task Init_Pool_Data()
        {
            while (true)
            {
                try
                {
                    string key = EntityModel.Redis.RedisKeys.KaiJiang_Key;

                    var orderService = new OrderQuery();
                    var gameString = "JX11X5|GD11X5|SD11X5|CQSSC|SSQ|DLT|FC3D|PL3|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ|CTZQ_TR9";
                    var result = orderService.QueryAllGameNewWinNumber(gameString);
                    var list = new List<KaiJiang>();
                    if (result != null && result.List.Count > 0)
                    {
                        foreach (var item in result.List)
                        {
                            var poolInfo = BettingHelper.GetPoolInfo(item.GameCode, item.IssuseNumber);
                            list.Add(new KaiJiang()
                            {
                                result = item.WinNumber,
                                prizepool = poolInfo != null ? poolInfo.TotalPrizePoolMoney.ToString("###,##0.00") : "",
                                nums = ConvertHelper.Getnums(poolInfo),
                                name = item.GameCode.ToUpper() == "CTZQ" ? item.GameType : item.GameCode,
                                termNo = item.IssuseNumber,
                                ver = "1",
                                grades = ConvertHelper.Getgrades(poolInfo),
                                date = item.CreateTime.ToString("yyyy-MM-dd"),
                                type = ConvertHelper.GetGameName(item.GameCode, item.GameType),
                                sale = poolInfo != null ? poolInfo.TotalSellMoney.ToString("###,##0.00") : ""
                            });
                        }
                        list[list.Count - 1].name = "TR9";
                        list[list.Count - 1].type = "任选9";
                    }
                    RedisHelperEx.DB_Match.SetObj(key, list, TimeSpan.FromSeconds(30 * 60));
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(60 * 1000);
            }
        }

        /// <summary>
        /// 修复未开奖数字彩订单无法加入redis
        /// </summary>
        /// <returns></returns>
        //public static async Task Repair_SZCAddToRedis()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            //1.DLT|SSQ|FC3D|PL3|CQSSC|GD11X5|SD11X5|JX11X5
        //            var GameCodes = "DLT|SSQ|FC3D|PL3|CQSSC|GD11X5|SD11X5|JX11X5";
        //            var orderService = new Sports_Manager();
        //            var list = orderService.QueryAllRunningOrder();
        //            CSRedis.CSRedisClient db = null;
        //            foreach (var item in GameCodes.Split('|', StringSplitOptions.None))
        //            {
        //                if (item == "CTZQ")
        //                    db = RedisHelperEx.DB_Running_Order_CTZQ;
        //                if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "OZB" }.Contains(item))
        //                    db = RedisHelperEx.DB_Running_Order_SCZ_DP;
        //                if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(item))
        //                    db = RedisHelperEx.DB_Running_Order_SCZ_GP;
        //                //if (db == null)
        //                //    throw new Exception(string.Format("找不到{0}对应的库", item));
        //                var gameList = list.Where(p => p.GameCode == item).ToList();
        //                foreach (var issuseNumber in gameList.GroupBy(p => p.IssuseNumber))
        //                {
        //                    var issuseNumberList = list.Where(p => p.IssuseNumber == issuseNumber.Key).ToList();
        //                    Dictionary<string, List<RedisOrderInfo>> redisIList = new Dictionary<string, List<RedisOrderInfo>>();
        //                    foreach (var runningItem in issuseNumberList)
        //                    {
        //                        var fullKey = (item.ToUpper() == "CTZQ" || item.ToUpper() == "OZB") ? string.Format("{0}_{1}_{2}_{3}", runningItem.GameCode, runningItem.GameType, RedisKeys.Key_Running_Order_List, runningItem.IssuseNumber) :
        //                                            string.Format("{0}_{1}_{2}", runningItem.GameCode, RedisKeys.Key_Running_Order_List, runningItem.IssuseNumber);
        //                        //var keycount = db.LLen(fullKey);
        //                        List<RedisOrderInfo> redisFullList = new List<RedisOrderInfo>();
        //                        if (!redisIList.Keys.Contains(fullKey))
        //                        {
        //                            var redisList = db.LRang(fullKey, 0, -1);
        //                            if (redisList != null)
        //                            {
        //                                foreach (var redisListItem in redisList)
        //                                {
        //                                    var jsonItem = JsonHelper.Deserialize<RedisOrderInfo>(redisListItem);
        //                                    redisFullList.Add(jsonItem);
        //                                }
        //                                //redisFullList = redisList;
        //                                redisIList.Add(fullKey, redisFullList);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            redisFullList = redisIList[fullKey];
        //                        }
        //                        //判断redis中是否存在
        //                        if (redisFullList.Count > 0)
        //                        {
        //                            var oldRedisItem = redisFullList.FirstOrDefault(p => p.SchemeId == runningItem.SchemeId);
        //                            //不存在，则需要加入redis
        //                            if (oldRedisItem == null)
        //                            {
        //                                BusinessHelper.AddToRunningOrder(runningItem.SchemeId);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            BusinessHelper.AddToRunningOrder(runningItem.SchemeId);
        //                        }
        //                    }
        //                }
        //            }
        //            //2.竞彩与北单的加入redis
        //            #region 北单与竞彩
        //            var bdList = list.Where(p => p.GameCode == "BJDC").ToList();
        //            var beiDb = RedisHelperEx.DB_Running_Order_BJDC;
        //            foreach (var bdItem in bdList.GroupBy(p => p.IssuseNumber))
        //            {
        //                var fullKey = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, bdItem.Key);
        //                var redisFullList = beiDb.LRang(fullKey, 0, -1);
        //                //判断北单是否存在于redis
        //                foreach (var bItem in bdList)
        //                {
        //                    if (redisFullList != null)
        //                    {
        //                        var hasOldModel = redisFullList.FirstOrDefault(p => p.EndsWith(bItem.SchemeId));
        //                        if (hasOldModel == null) BusinessHelper.AddToRunningOrder(bItem.SchemeId);
        //                    }
        //                    else
        //                    {
        //                        BusinessHelper.AddToRunningOrder(bItem.SchemeId);
        //                    }
        //                }
        //            }
        //            //JCZQ
        //            var Max_PrizeListCount = RedisOrderBusiness.Max_PrizeListCount;
        //            var jcDb = RedisHelperEx.DB_Running_Order_JC;
        //            var allJCZQKeys = new List<string>();
        //            for (int i = 0; i < Max_PrizeListCount; i++)
        //            {
        //                var fullKeyNew = string.Format("{0}_{1}_{2}", "JCZQ", RedisKeys.Key_Running_Order_List, i);
        //                allJCZQKeys.Add(fullKeyNew);
        //            }
        //            var allRedisJCZQList = new List<string>();
        //            foreach (var item in allJCZQKeys)
        //            {
        //                var jclist = jcDb.LRang(item, 0, -1);
        //                if (jclist != null)
        //                {
        //                    allRedisJCZQList.AddRange(jclist);
        //                }
        //            }

        //            var jczqList = list.Where(p => p.GameCode == "JCZQ").ToList();
        //            //判断JCZQ数据是否在redis存在
        //            foreach (var item in jczqList)
        //            {
        //                if (allRedisJCZQList.Count > 0)
        //                {
        //                    var oldmodel = allJCZQKeys.FirstOrDefault(p => p.EndsWith(item.SchemeId));
        //                    if (oldmodel == null) BusinessHelper.AddToRunningOrder(item.SchemeId);
        //                }
        //                else
        //                {
        //                    BusinessHelper.AddToRunningOrder(item.SchemeId);
        //                }
        //            }
        //            //JCLQ
        //            var allJCLQKeys = new List<string>();
        //            for (int i = 0; i < Max_PrizeListCount; i++)
        //            {
        //                var fullKeyNew = string.Format("{0}_{1}_{2}", "JCLQ", RedisKeys.Key_Running_Order_List, i);
        //                allJCLQKeys.Add(fullKeyNew);
        //            }
        //            var allRedisJCLQList = new List<string>();
        //            foreach (var item in allJCLQKeys)
        //            {
        //                var jclist = jcDb.LRang(item, 0, -1);
        //                if (jclist != null)
        //                {
        //                    allRedisJCLQList.AddRange(jclist);
        //                }
        //            }
        //            var jclqList = list.Where(p => p.GameCode == "JCLQ").ToList();
        //            //判断JCLQ数据是否在redis存在
        //            foreach (var item in jclqList)
        //            {
        //                if (allRedisJCLQList.Count > 0)
        //                {
        //                    var oldmodel = allJCLQKeys.FirstOrDefault(p => p.EndsWith(item.SchemeId));
        //                    if (oldmodel == null) BusinessHelper.AddToRunningOrder(item.SchemeId);
        //                }
        //                else
        //                {
        //                    BusinessHelper.AddToRunningOrder(item.SchemeId);
        //                }
        //            }
        //            #endregion
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        await Task.Delay(10 * 60 * 1000);
        //    }
        //}



        public static async Task Repair_SZCAddToRedis()
        {
            while (true)
            {
                try
                {
                    var orderService = new Sports_Manager();
                    var list = orderService.QueryAllRunningOrder();
                    var Max_PrizeListCount = RedisOrderBusiness.Max_PrizeListCount;
                    foreach (var order in list)
                    {
                        var flag = GetRedisOrKey(order, Max_PrizeListCount);
                        if (flag)
                        {
                            BusinessHelper.AddToRunningOrder(order);
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(10 * 60 * 1000);
            }
        }


        private static bool GetRedisOrKey(C_Sports_Order_Running order, int max)
        {
            CSRedis.CSRedisClient db = null;
            string key = string.Empty;
            bool flag = false;
            string gameCode = order.GameCode.ToUpper();
            if (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB")
            {
                key = (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB") ? string.Format("{0}_{1}_{2}_{3}", order.GameCode, order.GameType, RedisKeys.Key_Running_Order_List, order.IssuseNumber) :
                                                  string.Format("{0}_{1}_{2}", order.GameCode, RedisKeys.Key_Running_Order_List, order.IssuseNumber);
                db = RedisHelperEx.DB_Running_Order_CTZQ;
            }
            else if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
            {
                key = (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB") ? string.Format("{0}_{1}_{2}_{3}", order.GameCode, order.GameType, RedisKeys.Key_Running_Order_List, order.IssuseNumber) :
                                                  string.Format("{0}_{1}_{2}", order.GameCode, RedisKeys.Key_Running_Order_List, order.IssuseNumber);
                db = RedisHelperEx.DB_Running_Order_SCZ_DP;
            }
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
            {
                key = (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB") ? string.Format("{0}_{1}_{2}_{3}", order.GameCode, order.GameType, RedisKeys.Key_Running_Order_List, order.IssuseNumber) :
                                                 string.Format("{0}_{1}_{2}", order.GameCode, RedisKeys.Key_Running_Order_List, order.IssuseNumber);
                db = RedisHelperEx.DB_Running_Order_SCZ_GP;
            }
            if (gameCode == "BJDC")
            {
                key = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, order.IssuseNumber);
                db = RedisHelperEx.DB_Running_Order_BJDC;
            }
            if (gameCode == "JCZQ" || gameCode == "JCLQ")
            {
                var fullKeyNew = string.Format("{0}_{1}", gameCode, RedisKeys.Key_Running_Order_List);
                db = RedisHelperEx.DB_Running_Order_JC;
            }
            if (db != null && !string.IsNullOrEmpty(key))
            {
                var orderInfoList = new List<RedisOrderInfo>();
                if (gameCode == "JCZQ" || gameCode == "JCLQ")
                {
                    for (int i = 0; i < max; i++)
                    {
                        var fullKey = $"{key}_{i}";
                        var list = db.GetRange<RedisOrderInfo>(fullKey, 0, -1);
                        if (list.Count > 0)
                            orderInfoList.AddRange(list);
                    }
                    var count = orderInfoList.Count(a => a.SchemeId == order.SchemeId);
                    if (count == 0)
                        flag = true;
                }
                else
                {
                    var list = db.GetRange<RedisOrderInfo>(key, 0, -1);
                    var count = list.Count(a => a.SchemeId == order.SchemeId);
                    if (count == 0)
                        flag = true;
                }
            }
            return flag;
        }


    }
}

