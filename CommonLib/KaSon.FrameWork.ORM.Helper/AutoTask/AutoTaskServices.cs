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
                        Init_Pool_Data()
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
    }
}
