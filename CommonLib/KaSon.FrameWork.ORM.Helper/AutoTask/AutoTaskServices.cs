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
using EntityModel.Enum;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.Common.Expansion;

namespace KaSon.FrameWork.ORM.Helper.AutoTask
{
    public class AutoTaskServices
    {
        /// <summary>
        /// 缓存bjdc,jclq,jczq
        /// </summary>
        public static void AutoCaheData(int seconds)
        {
            Task.Run(() => StartTaskByWriteChaseOrderToDb(seconds));
            bool flag = ConfigHelper.AllConfigInfo["AutoTask"] == null ? false : Convert.ToBoolean(ConfigHelper.AllConfigInfo["AutoTask"].ToString());
            if (flag)
            {
                Task.WhenAll(new Task[] {
                     CTZQ_BJDC(),
                        JCLQ(),
                        JCZQ(),
                        Init_Pool_Data(),
                        Repair_SZCAddToRedis_dp(),
                        Repair_SZCAddToRedis_gp(),
                        GameRechargeRepair(),
                        GameWithdraw()
                });
            }
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



        public static async Task Repair_SZCAddToRedis_dp()
        {
            while (true)
            {
                try
                {
                    List<string> listRange = new List<string>();
                    var orderService = new Sports_Manager();
                    var list = orderService.QueryAllRunningOrder_dp();
                    if (list.Count > 0)
                    {
                        var max = RedisOrderBusiness.Max_PrizeListCount;
                        //var ctzqlist = from p in list
                        //               where p.GameCode == "CTZQ" || p.GameCode == "OZB" || p.GameCode == "SJB"
                        //               group p by new { p.GameCode, p.IssuseNumber, p.GameType } into g
                        //               select new
                        //               {
                        //                   g.Key
                        //               };
                        var ctzqlist = list.Where(p => p.GameCode == "CTZQ").GroupBy(p => new { p.GameCode, p.IssuseNumber, p.GameType }).Select(a => a.Key).ToList();
                        foreach (var item in ctzqlist)
                        {
                            var ctzq = GetRedisList_dp(item.GameCode, item.IssuseNumber, max, item.GameType);
                            if (ctzq != null)
                                listRange.AddRange(ctzq);
                        }
                        var ssqlist = list.Where(p => p.GameCode == "SSQ").GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in ssqlist)
                        {
                            var ssq = GetRedisList_dp(item.GameCode, item.IssuseNumber, max);
                            if (ssq != null)
                                listRange.AddRange(ssq);
                        }
                        var dltlist = list.Where(p => p.GameCode == "DLT").GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in dltlist)
                        {
                            var dlt = GetRedisList_dp(item.GameCode, item.IssuseNumber, max);
                            if (dlt != null)
                                listRange.AddRange(dlt);
                        }
                        var fc3dlist = list.Where(p => p.GameCode == "FC3D").GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in fc3dlist)
                        {
                            var fc3d = GetRedisList_dp(item.GameCode, item.IssuseNumber, max);
                            if (fc3d != null)
                                listRange.AddRange(fc3d);
                        }
                        var PL3list = list.Where(p => p.GameCode == "PL3").GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in fc3dlist)
                        {
                            var PL3 = GetRedisList_dp(item.GameCode, item.IssuseNumber, max);
                            if (PL3 != null)
                                listRange.AddRange(PL3);
                        }
                        foreach (var order in list)
                        {
                            if (listRange.Count(a => a.Contains(order.SchemeId)) == 0)
                                BusinessHelper.AddToRunningOrder(order);
                            //var flag = GetRedisOrKey(order, Max_PrizeListCount);
                            //if (flag)
                            //{
                            //    var count = orderInfoList.Count(a => a.SchemeId == order.SchemeId);
                            //    if (count == 0)
                            //        flag = true;
                            //    BusinessHelper.AddToRunningOrder(order);
                            //}

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(1 * 60 * 60 * 1000);
            }
        }

        public static async Task Repair_SZCAddToRedis_gp()
        {
            while (true)
            {
                try
                {
                    List<string> listRange = new List<string>();
                    var orderService = new Sports_Manager();
                    var list = orderService.QueryAllRunningOrder_gp();
                    if (list.Count > 0)
                    {
                        var max = RedisOrderBusiness.Max_PrizeListCount;
                        //var ctzqlist = from p in list
                        //               where p.GameCode == "CTZQ" || p.GameCode == "OZB" || p.GameCode == "SJB"
                        //               group p by new { p.GameCode, p.IssuseNumber, p.GameType } into g
                        //               select new
                        //               {
                        //                   g.Key
                        //               };//"CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3"
                        var gpclist = list.Where(p => p.GameCode == "CQSSC" || p.GameCode == "JX11X5" || p.GameCode == "SD11X5" ||
                                                       p.GameCode == "GD11X5" || p.GameCode == "GDKLSF" || p.GameCode == "JSKS" || p.GameCode == "SDKLPK3")
                            .GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in gpclist)
                        {
                            var gpc = GetRedisList_gp(item.GameCode, item.IssuseNumber, max);
                            if (gpc != null)
                                listRange.AddRange(gpc);
                        }
                        var BJDClist = list.Where(p => p.GameCode == "BJDC").GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in BJDClist)
                        {
                            var BJDC = GetRedisList_gp(item.GameCode, item.IssuseNumber, max);
                            if (BJDC != null)
                                listRange.AddRange(BJDC);
                        }
                        var jclist = list.Where(p => p.GameCode == "JCZQ" || p.GameCode == "JCLQ").GroupBy(p => new { p.GameCode, p.IssuseNumber }).Select(a => a.Key).ToList();
                        foreach (var item in jclist)
                        {
                            var jc = GetRedisList_gp(item.GameCode, item.IssuseNumber, max);
                            if (jc != null)
                                listRange.AddRange(jc);
                        }
                        foreach (var order in list)
                        {
                            if (listRange.Count(a => a.Contains(order.SchemeId)) == 0)
                                BusinessHelper.AddToRunningOrder(order);
                            //var flag = GetRedisOrKey(order, Max_PrizeListCount);
                            //if (flag)
                            //{
                            //    var count = orderInfoList.Count(a => a.SchemeId == order.SchemeId);
                            //    if (count == 0)
                            //        flag = true;
                            //    BusinessHelper.AddToRunningOrder(order);
                            //}

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(10 * 60 * 1000);
            }
        }



        private static List<string> GetRedisList_dp(string gameCode, string issuseNumber, int max, string gameType = "")
        {
            CSRedis.CSRedisClient db = null;
            string key = string.Empty;
            if (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB")
            {
                key = string.Format("{0}_{1}_{2}_{3}", gameCode, gameType, RedisKeys.Key_Running_Order_List, issuseNumber);
                db = RedisHelperEx.DB_Running_Order_CTZQ;
            }
            else if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
            {
                key = string.Format("{0}_{1}_{2}", gameCode, RedisKeys.Key_Running_Order_List, issuseNumber);
                db = RedisHelperEx.DB_Running_Order_SCZ_DP;
            }
            //if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
            //{
            //    key = string.Format("{0}_{1}_{2}", gameCode, RedisKeys.Key_Running_Order_List, issuseNumber);
            //    db = RedisHelperEx.DB_Running_Order_SCZ_GP;
            //}
            //if (gameCode == "BJDC")
            //{
            //    key = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, issuseNumber);
            //    db = RedisHelperEx.DB_Running_Order_BJDC;
            //}
            //if (gameCode == "JCZQ" || gameCode == "JCLQ")
            //{
            //    var fullKeyNew = string.Format("{0}_{1}", gameCode, RedisKeys.Key_Running_Order_List);
            //    db = RedisHelperEx.DB_Running_Order_JC;
            //}
            if (db != null && !string.IsNullOrEmpty(key))
            {
                var orderInfoList = new List<string>();
                if (gameCode == "JCZQ" || gameCode == "JCLQ")
                {
                    for (int i = 0; i < max; i++)
                    {
                        var fullKey = $"{key}_{i}";
                        var list = db.LRang(fullKey, 0, -1);
                        if (list.Length > 0)
                            orderInfoList.AddRange(list);
                    }
                    return orderInfoList;
                }
                else
                {
                    var list = db.LRang(key, 0, -1);
                    orderInfoList.AddRange(list);
                    return orderInfoList;
                }
            }
            return null;
        }

        private static List<string> GetRedisList_gp(string gameCode, string issuseNumber, int max, string gameType = "")
        {
            CSRedis.CSRedisClient db = null;
            string key = string.Empty;
            //if (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB")
            //{
            //    key = string.Format("{0}_{1}_{2}_{3}", gameCode, gameType, RedisKeys.Key_Running_Order_List, issuseNumber);
            //    db = RedisHelperEx.DB_Running_Order_CTZQ;
            //}
            //else if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
            //{
            //    key = string.Format("{0}_{1}_{2}", gameCode, RedisKeys.Key_Running_Order_List, issuseNumber);
            //    db = RedisHelperEx.DB_Running_Order_SCZ_DP;
            //}
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
            {
                key = string.Format("{0}_{1}_{2}", gameCode, RedisKeys.Key_Running_Order_List, issuseNumber);
                db = RedisHelperEx.DB_Running_Order_SCZ_GP;
            }
            if (gameCode == "BJDC")
            {
                key = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, issuseNumber);
                db = RedisHelperEx.DB_Running_Order_BJDC;
            }
            if (gameCode == "JCZQ" || gameCode == "JCLQ")
            {
                key = string.Format("{0}_{1}", gameCode, RedisKeys.Key_Running_Order_List);
                db = RedisHelperEx.DB_Running_Order_JC;
            }
            if (db != null && !string.IsNullOrEmpty(key))
            {
                var orderInfoList = new List<string>();
                if (gameCode == "JCZQ" || gameCode == "JCLQ")
                {
                    for (int i = 0; i < max; i++)
                    {
                        var fullKey = $"{key}_{i}";
                        var list = db.LRang(fullKey, 0, -1);
                        if (list.Length > 0)
                            orderInfoList.AddRange(list);
                    }
                    return orderInfoList;
                }
                else
                {
                    var list = db.LRang(key, 0, -1);
                    orderInfoList.AddRange(list);
                    return orderInfoList;
                }
            }
            return null;
        }


        private static List<string> GetRedisList(C_Sports_Order_Running order, int max)
        {
            CSRedis.CSRedisClient db = null;
            string key = string.Empty;
            string gameCode = order.GameCode.ToUpper();
            if (gameCode == "CTZQ" || gameCode == "OZB" || gameCode == "SJB")
            {
                key = string.Format("{0}_{1}_{2}_{3}", order.GameCode, order.GameType, RedisKeys.Key_Running_Order_List, order.IssuseNumber);
                db = RedisHelperEx.DB_Running_Order_CTZQ;
            }
            else if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(gameCode))
            {
                key = string.Format("{0}_{1}_{2}", order.GameCode, RedisKeys.Key_Running_Order_List, order.IssuseNumber);
                db = RedisHelperEx.DB_Running_Order_SCZ_DP;
            }
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
            {
                key = string.Format("{0}_{1}_{2}", order.GameCode, RedisKeys.Key_Running_Order_List, order.IssuseNumber);
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
                var orderInfoList = new List<string>();
                if (gameCode == "JCZQ" || gameCode == "JCLQ")
                {
                    for (int i = 0; i < max; i++)
                    {
                        var fullKey = $"{key}_{i}";
                        var list = db.LRang(fullKey, 0, -1);
                        if (list.Length > 0)
                            orderInfoList.AddRange(list);
                    }
                    return orderInfoList;
                }
                else
                {
                    var list = db.LRang(key, 0, -1);
                    orderInfoList.AddRange(list);
                    return orderInfoList;
                }
            }
            return null;
        }

        public static async Task GameRechargeRepair()
        {
            var min = 2;
            var OperatorCode = ConfigHelper.AllConfigInfo["GameApi"]["OperatorCode"].ToString();
            var SecretKey = ConfigHelper.AllConfigInfo["GameApi"]["SecretKey"].ToString();
            var PreName = ConfigHelper.AllConfigInfo["GameApi"]["PreName"].ToString();
            var GameUrl = ConfigHelper.AllConfigInfo["GameApi"]["URL"].ToString();
            var pwd = "DJW7389a9";
            var dataQuery = new DataQuery();
            while (true)
            {
                try
                {
                    //查找2分钟前未完成的交易
                    var NotFinishGameTransfer = dataQuery.QueryNotFinishGame(min);
                    var theList = NotFinishGameTransfer.Where(p => p.TransferType == (int)GameTransferType.Recharge).ToList();
                    if (theList != null && theList.Count > 0)
                    {
                        foreach (var item in theList)
                        {
                            string providerSerialNo = "";
                            try
                            {
                                var gameLoginName = PreName + item.UserId;
                                var sign = MD5Helper.UpperMD5($"{item.RequestMoney.ToString()}&{OperatorCode}&{pwd}&{item.OrderId}&{gameLoginName}&{SecretKey}");
                                var rechargeParam = new
                                {
                                    command = "DEPOSIT",
                                    gameprovider = "2",
                                    sign = sign,
                                    @params = new
                                    {
                                        username = gameLoginName,
                                        operatorcode = OperatorCode,
                                        password = pwd,
                                        serialNo = item.OrderId,
                                        amount = item.RequestMoney.ToString(),
                                        extraparameter = new
                                        {
                                            type = "SMG"
                                        }
                                    }
                                }.ToJson();
                                var result = PostManager.Post(GameUrl, rechargeParam, Encoding.UTF8, 45, null, "application/json");
                                var jsonResult = JsonHelper.Decode(result);
                                if (jsonResult.ErrorCode == 0)
                                {
                                    providerSerialNo = jsonResult.Params.providerSerialNo;
                                    var confirmSign = MD5Helper.UpperMD5($"{OperatorCode}&{pwd}&{providerSerialNo}&{gameLoginName}&{SecretKey}");
                                    var confirmParam = new
                                    {
                                        command = "CHECK_TRANSFER_STATUS",
                                        gameprovider = "2",
                                        sign = confirmSign,
                                        @params = new
                                        {
                                            username = gameLoginName,
                                            operatorcode = OperatorCode,
                                            password = pwd,
                                            serialNo = providerSerialNo,
                                        }
                                    }.ToJson();
                                    var confirmResult = PostManager.Post(GameUrl, confirmParam, Encoding.UTF8, 45, null, "application/json");
                                    var jsonConfirmResult = JsonHelper.Decode(confirmResult);
                                    if (jsonConfirmResult.ErrorCode == 0) //确认
                                    {
                                        dataQuery.EndFreezeGameRecharge(item.OrderId, true, providerSerialNo);
                                    }
                                    else
                                    {
                                        dataQuery.EndFreezeGameRecharge(item.OrderId, false, providerSerialNo);
                                    }
                                }
                                else
                                {
                                    dataQuery.EndFreezeGameRecharge(item.OrderId, false, providerSerialNo);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                await Task.Delay(30000);
            }
        }

        public static async Task GameWithdraw()
        {
            var min = 2;
            var OperatorCode = ConfigHelper.AllConfigInfo["GameApi"]["OperatorCode"].ToString();
            var SecretKey = ConfigHelper.AllConfigInfo["GameApi"]["SecretKey"].ToString();
            var PreName = ConfigHelper.AllConfigInfo["GameApi"]["PreName"].ToString();
            var GameUrl = ConfigHelper.AllConfigInfo["GameApi"]["URL"].ToString();
            var pwd = "DJW7389a9";
            var dataQuery = new DataQuery();
            while (true)
            {
                try
                {
                    //查找3分钟前未完成的交易
                    var NotFinishGameTransfer = dataQuery.QueryNotFinishGame(min);
                    var theList = NotFinishGameTransfer.Where(p => p.TransferType == (int)GameTransferType.Withdraw).ToList();
                    if (theList != null && theList.Count > 0)
                    {
                        var providerSerialNo = "";
                        foreach (var item in theList)
                        {
                            try
                            {
                                var gameLoginName = PreName + item.UserId;
                                var sign = MD5Helper.UpperMD5($"{item.RequestMoney.ToString()}&{OperatorCode}&{pwd}&{item.OrderId}&{gameLoginName}&{SecretKey}");
                                var withdrawParam = new
                                {
                                    command = "WITHDRAW",
                                    gameprovider = "2",
                                    sign = sign,
                                    @params = new
                                    {
                                        username = gameLoginName,
                                        operatorcode = OperatorCode,
                                        password = pwd,
                                        serialNo = item.OrderId,
                                        amount = item.RequestMoney.ToString(),
                                        extraparameter = new
                                        {
                                            type = "SMG"
                                        }
                                    }
                                }.ToJson();
                                var result = PostManager.Post(GameUrl, withdrawParam, Encoding.UTF8, 45, null, "application/json");
                                var jsonResult = JsonHelper.Decode(result);
                                if (jsonResult.ErrorCode == 0)
                                {
                                    providerSerialNo = jsonResult.Params.providerSerialNo;
                                    var confirmSign = MD5Helper.UpperMD5($"{OperatorCode}&{pwd}&{providerSerialNo}&{gameLoginName}&{SecretKey}");
                                    var confirmParam = new
                                    {
                                        command = "CHECK_TRANSFER_STATUS",
                                        gameprovider = "2",
                                        sign = confirmSign,
                                        @params = new
                                        {
                                            username = gameLoginName,
                                            operatorcode = OperatorCode,
                                            password = pwd,
                                            serialNo = providerSerialNo,
                                        }
                                    }.ToJson();
                                    var confirmResult = PostManager.Post(GameUrl, confirmParam, Encoding.UTF8, 45, null, "application/json");
                                    var jsonConfirmResult = JsonHelper.Decode(confirmResult);
                                    if (jsonConfirmResult.ErrorCode == 0) //确认
                                    {
                                        dataQuery.EndAddGameWithdraw(item.OrderId, true, providerSerialNo);
                                    }
                                    else
                                    {
                                        dataQuery.EndAddGameWithdraw(item.OrderId, false, providerSerialNo);
                                    }
                                }
                                else
                                {
                                    dataQuery.EndAddGameWithdraw(item.OrderId, false, providerSerialNo);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                await Task.Delay(30000);
            }
        }
    }
}

