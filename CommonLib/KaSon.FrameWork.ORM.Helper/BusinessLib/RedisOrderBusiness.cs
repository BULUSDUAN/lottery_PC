using EntityModel.ExceptionExtend;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using EntityModel;
using StackExchange.Redis;
using EntityModel.CoreModel;
using EntityModel.Redis;
using System.Threading;
using EntityModel.Ticket;
using System.IO;
using KaSon.FrameWork.Common.Sport;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// Redis比赛相关业务
    /// </summary>
    public static class RedisOrderBusiness
    {
        private static Log4Log writerLog =  new Log4Log();

        /// <summary>
        /// 拆票后,保存数字彩订单到Redis库中
        /// </summary>
        public static void AddToRunningOrder_SZC(SchemeType schemeType, string gameCode, string gameType, string orderId, string keyLine, bool stopAfterBonus, string issuseNumber, List<RedisTicketInfo> ticketList)
        {
            if (ticketList.Count <= 0)
                return;
            //把彩种、玩法、期号为Key,订单json存入List
            IDatabase db = null;// RedisHelper.DB_Running_Order;
            if (gameCode == "CTZQ")
                db = RedisHelper.DB_Running_Order_CTZQ;
            if (new string[] { "SSQ", "DLT", "FC3D", "PL3", "OZB" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_DP;
            if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(gameCode))
                db = RedisHelper.DB_Running_Order_SCZ_GP;

            if (db == null)
                throw new Exception(string.Format("找不到{0}对应的库", gameCode));

            var fullKey = (gameCode.ToUpper() == "CTZQ" || gameCode.ToUpper() == "OZB") ? string.Format("{0}_{1}_{2}_{3}", gameCode, gameType, RedisKeys.Key_Running_Order_List, issuseNumber) :
                                                    string.Format("{0}_{1}_{2}", gameCode, RedisKeys.Key_Running_Order_List, issuseNumber);
            //fullKey = GetUsableList(fullKey);

            var orderInfo = new RedisOrderInfo
            {
                SchemeId = orderId,
                KeyLine = keyLine,
                StopAfterBonus = stopAfterBonus,
                TicketList = ticketList,
                SchemeType = schemeType,
            };
            var json = JsonHelper.Serialize<RedisOrderInfo>(orderInfo);
            db.ListRightPushAsync(fullKey, json);
        }
        public static void AddOrderToRedis(string gameCode, RedisWaitTicketOrder order)
        {
            //读取配置文件
           
            if (BettingHelper.CanRequestBet(gameCode))
            {
                //可以拆票
                DoSplitOrderTicketWithThread(order);
            }
            else
            {
                //不能拆票
                AddOrderToWaitSplitList(order);
            }
        }

        /// <summary>
        /// 添加订单(单式)到Redis库，本方法决定拆票或不拆票
        /// </summary>
        public static void AddOrderToRedis(string gameCode, RedisWaitTicketOrderSingle order)
        {
            if (BettingHelper.CanRequestBet(gameCode))
            {
                //可以拆票
                DoSplitOrderTicketWithThread_Single(order);
            }
            else
            {
                //不能拆票
                AddOrderToWaitSplitList(order);
            }
        }
        /// <summary>
        /// 多线程执行拆票（单式投注）
        /// </summary>
        public static void DoSplitOrderTicketWithThread_Single(RedisWaitTicketOrderSingle order)
        {
            if (order == null || order.AnteCode == null)
                return;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    DoSplitOrderTicket_Single(o as RedisWaitTicketOrderSingle);
                }
                catch (Exception ex)
                {
                    writerLog.ErrrorLog("Redis_DoSplitOrderTicket_Single-DoSplitOrderTicketWithThread_Single", ex);
                }
            }, order);
        }

        /// <summary>
        /// 多线程执行拆票（普通投注）
        /// </summary>
        public static void DoSplitOrderTicketWithThread(RedisWaitTicketOrder order)
        {
            if (order == null || order.RunningOrder == null || order.AnteCodeList == null || order.AnteCodeList.Count <= 0)
                return;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    DoSplitOrderTicket(o as RedisWaitTicketOrder);
                }
                catch (Exception ex)
                {
                    writerLog.ErrrorLog("Redis_DoSplitOrderTicket-DoSplitOrderTicketWithThread", ex);
                }
            }, order);
        }

        /// <summary>
        /// 执行拆票（单式投注）
        /// </summary>
        public static void DoSplitOrderTicket_Single(RedisWaitTicketOrderSingle order)
        {
            //if (!BusinessHelper.CanRequestBet(order.RunningOrder.GameCode))
            //    return;

            var sportsManager = new Sports_Manager();
            var oldCount = sportsManager.QueryTicketCount(order.RunningOrder.SchemeId);
            if (oldCount <= 0)
            {
                //清理冻结
                if (order.RunningOrder.SchemeType == (int)SchemeType.ChaseBetting)
                    BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.RunningOrder.UserId, order.RunningOrder.SchemeId, string.Format("订单{0}出票完成，扣除冻结{1:N2}元", order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney), order.RunningOrder.TotalMoney);

                #region 拆票

                new Sports_Business().RequestTicketByGateway_SingleScheme_New(new GatewayTicketOrder_SingleScheme
                {
                    AllowCodes = order.AnteCode.AllowCodes,
                    Amount = order.RunningOrder.Amount,
                    ContainsMatchId = order.AnteCode.ContainsMatchId,
                    FileBuffer = order.AnteCode.FileBuffer,
                    GameCode = order.RunningOrder.GameCode,
                    GameType = order.RunningOrder.GameType,
                    IsRunningTicket = true,
                    IssuseNumber = order.RunningOrder.IssuseNumber,
                    IsVirtualOrder = false,
                    OrderId = order.RunningOrder.SchemeId,
                    PlayType = order.RunningOrder.PlayType,
                    SelectMatchId = order.AnteCode.SelectMatchId,
                    TotalMoney = order.RunningOrder.TotalMoney,
                    UserId = order.RunningOrder.UserId,
                });

                //new Thread(() =>
                //{
                try
                {
                    //生成文件
                    var json = Encoding.UTF8.GetString(order.AnteCode.FileBuffer);
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.RunningOrder.GameCode, order.RunningOrder.SchemeId.Substring(0, 10));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var fileName = Path.Combine(path, string.Format("{0}.json", order.RunningOrder.SchemeId));
                    File.WriteAllText(fileName, json, Encoding.UTF8);
                }
                catch (Exception)
                {
                }
                //}).Start();


                #endregion

                //更新订单状态
                UpdateOrderTicketStatus(order.RunningOrder.SchemeId);

                //触发出票完成接口
                BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.RunningOrder.UserId, order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney, order.RunningOrder.TotalMoney });
            }
        }
        /// <summary>
        /// 执行拆票(普通投注)
        /// </summary>
        public static void DoSplitOrderTicket(RedisWaitTicketOrder order)
        {
            //if (!BusinessHelper.CanRequestBet(order.RunningOrder.GameCode))
            //    return;
            var DB = new DBbase().DB;// DBbase
            try
            {
                var sportsManager = new Sports_Manager();
                var oldCount = DB.CreateQuery<C_Sports_Ticket>().Where(p => p.SchemeId == order.RunningOrder.SchemeId).Count(); //sportsManager.QueryTicketCount(order.RunningOrder.SchemeId);
                if (oldCount <= 0)
                {
                    //清理冻结
                    if (order.RunningOrder.SchemeType == (int)SchemeType.ChaseBetting)
                        BusinessHelper.Payout_Frozen_To_End(BusinessHelper.FundCategory_Betting, order.RunningOrder.UserId, order.RunningOrder.SchemeId, string.Format("订单{0}出票完成，扣除冻结{1:N2}元", order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney), order.RunningOrder.TotalMoney);

                    //普通投注
                    var jcGameCodeArray = new string[] { "BJDC", "JCZQ", "JCLQ" };
                    if (jcGameCodeArray.Contains(order.RunningOrder.GameCode))
                    {
                        //竞彩
                        #region 拆票

                        var betInfo = new GatewayTicketOrder_Sport
                        {
                            Amount = order.RunningOrder.Amount,
                            Attach = order.RunningOrder.Attach,
                            GameCode = order.RunningOrder.GameCode,
                            GameType = order.RunningOrder.GameType,
                            IssuseNumber = order.RunningOrder.IssuseNumber,
                            IsVirtualOrder = order.RunningOrder.IsVirtualOrder,
                            OrderId = order.RunningOrder.SchemeId,
                            PlayType = order.RunningOrder.PlayType,
                            UserId = order.RunningOrder.UserId,
                            TotalMoney = order.RunningOrder.TotalMoney,
                            Price = 2M,
                            IsRunningTicket = true,
                        };
                        foreach (var code in order.AnteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode_Sport
                            {
                                AnteCode = code.AnteCode,
                                GameType = code.GameType,
                                IsDan = code.IsDan,
                                MatchId = code.MatchId,
                            });
                        }
                        new Sports_Business().RequestTicket_Sport(betInfo);

                        //new Thread(() =>
                        //{
                        try
                        {
                            //生成文件
                            var json = JsonHelper.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.RunningOrder.GameCode, order.RunningOrder.SchemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", order.RunningOrder.SchemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }
                        //}).Start();
                        #endregion
                    }
                    else
                    {
                        //数字彩、传统足球
                        #region 拆票

                        var betInfo = new GatewayTicketOrder
                        {
                            Amount = order.RunningOrder.Amount,
                            GameCode = order.RunningOrder.GameCode,
                            IssuseNumber = order.RunningOrder.IssuseNumber,
                            OrderId = order.RunningOrder.SchemeId,
                            Price = ((order.RunningOrder.IsAppend && order.RunningOrder.GameCode == "DLT") ? 3M : 2M),
                            TotalMoney = order.RunningOrder.TotalMoney,
                            IsVirtualOrder = false,
                            Attach = "",
                            IsAppend = order.RunningOrder.IsAppend,
                            UserId = order.RunningOrder.UserId,
                            IsRunningTicket = true,
                        };
                        foreach (var item in order.AnteCodeList)
                        {
                            betInfo.AnteCodeList.Add(new GatewayAnteCode
                            {
                                AnteNumber = item.AnteCode,
                                GameType = item.GameType,
                            });
                        }

                        //new Sports_Business().RequestTicket(betInfo, order.KeyLine, order.StopAfterBonus, order.SchemeType);

                        new Sports_Business().RequestTicket2(betInfo, order.KeyLine, order.StopAfterBonus, order.SchemeType);
                        //new Thread(() =>
                        //{

                        try
                        {
                            //生成文件
                            var json = JsonHelper.Serialize(betInfo);
                            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "Orders", DateTime.Today.ToString("yyyyMMdd"), order.RunningOrder.GameCode, order.RunningOrder.SchemeId.Substring(0, 10));
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var fileName = Path.Combine(path, string.Format("{0}.json", order.RunningOrder.SchemeId));
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception)
                        {
                        }

                        //}).Start();

                        #endregion
                    }

                    //更新订单状态
                    UpdateOrderTicketStatus(order.RunningOrder.SchemeId);

                    //触发出票完成接口
                    BusinessHelper.ExecPlugin<IComplateTicket>(new object[] { order.RunningOrder.UserId, order.RunningOrder.SchemeId, order.RunningOrder.TotalMoney, order.RunningOrder.TotalMoney });


                }
            }
            catch (Exception exp)
            {
               // writerLog("Redis_DoSplitOrderTicket-DoSplitOrderTicketWithThread", ex);
                writerLog.WriteLog("追号订单自动拆票任务", "DoSplitOrderTicket",(int) LogType.Information, "追号订单自动拆票任务日志", exp.Message);

            }
        }
        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_PrizeListCount
        {
            get
            {
                try
                {
                    //ConfigurationManager.AppSettings["Max_PrizeListCount"]
                   
                    string _Max_PrizeListCount = ConfigHelper.ConfigInfo["Max_PrizeListCount"].ToString();

                    return int.Parse(_Max_PrizeListCount);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }
        /// <summary>
        /// 获取可用的队列Key
        /// </summary>
        public static string GetUsableList(string key)
        {
            try
            {
                var db = RedisHelper.DB_NoTicket_Order;
                var currentIndexKey = string.Format("{0}_Current", key);
                var indexValue = db.StringGetAsync(currentIndexKey).Result;
                var index = 0;
                if (indexValue.HasValue)
                {
                    //获取索引
                    index = int.Parse(indexValue.ToString());
                    index = index >= Max_PrizeListCount ? 0 : index + 1;
                }
                db.StringSetAsync(currentIndexKey, index);
                return string.Format("{0}_{1}", key, index);
            }
            catch (Exception)
            {
                return key;
            }
        }
        /// <summary>
        /// 拆票后,保存竞彩、北单订单到Redis库中
        /// </summary>
        public static void AddToRunningOrder_JC(string gameCode, string orderId, string[] matchIdArray, List<RedisTicketInfo> ticketList)
        {
            if (ticketList.Count <= 0)
                return;

            //把订单号和订单的比赛编号存入List
            var db = RedisHelper.DB_Running_Order_JC;
            //竞彩足球、竞彩篮球格式：比赛编号1,比赛编号2_订单号
            //北京单场格式：期号|比赛编号,期号|比赛编号_订单号
            var orderValue = string.Format("{0}_{1}", string.Join(",", matchIdArray), orderId);
            var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_Running_Order_List);
            fullKey = GetUsableList(fullKey);
            db.ListRightPushAsync(fullKey, orderValue);

            var orderInfo = new RedisOrderInfo
            {
                SchemeId = orderId,
                TicketList = ticketList,
            };
            var json = JsonHelper.Serialize<RedisOrderInfo>(orderInfo);
            //以订单号为key 订单内容为value保存
            db.StringSetAsync(orderId, json);
        }

        /// <summary>
        /// 拆票后,北单订单到Redis库中
        /// </summary>
        public static void AddToRunningOrder_BJDC(string orderId, string[] matchIdArray, List<RedisTicketInfo> ticketList)
        {
            if (ticketList.Count <= 0)
                return;
            var db = RedisHelper.DB_Running_Order_BJDC;
            string issuseNumber = ticketList[0].IssuseNumber;
            var fullKey = string.Format("{0}_{1}_{2}", "BJDC", RedisKeys.Key_Running_Order_List, issuseNumber);
            //fullKey = GetUsableList(fullKey);
            var orderValue = string.Format("{0}_{1}", string.Join(",", matchIdArray), orderId);
            db.ListRightPushAsync(fullKey, orderValue);

            var orderInfo = new RedisOrderInfo
            {
                SchemeId = orderId,
                TicketList = ticketList,
            };
            var json = JsonHelper.Serialize<RedisOrderInfo>(orderInfo);
            //以订单号为key 订单内容为value保存
            db.StringSetAsync(orderId, json);
        }

     
        private static string GetWaitingOrderUsableKey(string gameCode)
        {
            try
            {
                //    ConfigurationManager.AppSettings["WaitingOrderListCount"]
             
                string WaitingOrderListCount = ConfigHelper.ConfigInfo["WaitingOrderListCount"].ToString();// DBbase.GlobalConfig["WaitingOrderListCount"].ToString();

                var count = int.Parse(WaitingOrderListCount);
                var db = RedisHelper.DB_NoTicket_Order;
                var key = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "General", gameCode.ToUpper());
                var currentIndexKey = string.Format("{0}_Current", key);
                var indexValue = db.StringGetAsync(currentIndexKey).Result;
                var index = 0;
                if (indexValue.HasValue)
                {
                    //获取索引
                    index = int.Parse(indexValue.ToString());
                    index = index >= count ? 0 : index + 1;
                }
                db.StringSetAsync(currentIndexKey, index);
                return string.Format("{0}_{1}", key, index);
            }
            catch (Exception)
            {
                return string.Format("{0}_{1}_{2}_0", RedisKeys.Key_Waiting_Order_List, "General", gameCode.ToUpper());
            }
        }

        /// <summary>
        /// 更新订单状态为已出票
        /// </summary>
        private static void UpdateOrderTicketStatus(string schemeId)
        {
            //DBbase dBbase = new DBbase();
            var sql = string.Format(@"update [C_Sports_Order_Running] set TicketStatus=90,ProgressStatus=10,TicketTime=getdate() where [SchemeId]='{0}'
                                    update C_OrderDetail set CurrentBettingMoney=TotalMoney,TicketStatus=90,ProgressStatus=10,TicketTime=getdate() where [SchemeId]='{0}'", schemeId);
            var sportsManager = new Sports_Manager();
            sportsManager.ExecSql(sql);

        }
        /// <summary>
        /// 订单投注后加入Redis待拆票列表(普通投注)
        /// </summary>
        public static void AddOrderToWaitSplitList(RedisWaitTicketOrder order)
        {
            if (order == null || order.RunningOrder == null || order.AnteCodeList.Count <= 0)
                return;

            //var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "General", order.RunningOrder.GameCode.ToUpper());
            var fullKey = GetWaitingOrderUsableKey(order.RunningOrder.GameCode);
            var json = JsonHelper.Serialize<RedisWaitTicketOrder>(order);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
        }

        /// <summary>
        /// 把SQL中的订单加入Redis待拆票库中
        /// </summary>
        public static string AddOrderToWaitSplitList(string schemeId)
        {
            var logList = new List<string>();
            try
            {
                logList.Add(string.Format("开始处理订单{0}", schemeId));
                var manager = new Sports_Manager();
                var order = manager.QuerySports_Order_Running(schemeId);
                if (order == null)
                    throw new Exception("订单数据为空");
                var codeList = manager.QuerySportsAnteCodeBySchemeId(schemeId);
                if (codeList == null || codeList.Count <= 0)
                    throw new Exception("订单投注号码为空");

                if (order.SchemeType == (int)SchemeType.ChaseBetting)
                {
                    logList.Add("订单是【追号订单】");

                    var detail = new SchemeManager().QueryOrderDetail(schemeId);
                    if (detail == null)
                        throw new Exception("OrderDetail数据为空");
                    var scheme = manager.QueryLotteryScheme(schemeId);
                    if (scheme == null)
                        throw new Exception("LotteryScheme数据为空");

                    //检查keyline在Redis库中存不存在
                    var db = RedisHelper.DB_Chase_Order;
                    var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Waiting_Chase_Order_List, order.GameCode);
                    var chaseKeyLineArray = db.ListRangeAsync(fullKey).Result;
                    foreach (var k in chaseKeyLineArray)
                    {
                        if (!k.HasValue)
                            continue;
                        if (k.ToString() == scheme.KeyLine)
                        {
                            logList.Add("追号列表中已存在KeyLine.");
                            //取出所有追号列表
                            var chaseList = db.ListRangeAsync(scheme.KeyLine).Result;
                            //清空key
                            db.KeyDeleteAsync(scheme.KeyLine);
                            //修改canchase为true后，添加key
                            var orderList2 = new RedisWaitTicketOrderList();
                            orderList2.KeyLine = scheme.KeyLine;
                            orderList2.StopAfterBonus = detail.StopAfterBonus;
                            var findCurrentOrder = false;
                            foreach (var item in chaseList)
                            {
                                var orderJson = item.ToString();
                                var chaseOrder = JsonHelper.Deserialize<RedisWaitTicketOrder>(orderJson);
                                if (chaseOrder.RunningOrder != null)
                                {
                                    chaseOrder.RunningOrder.CanChase = !findCurrentOrder;
                                    if (chaseOrder.RunningOrder.SchemeId == schemeId)
                                        findCurrentOrder = true;
                                    orderList2.OrderList.Add(chaseOrder);
                                }
                            }
                            AddOrderToWaitSplitList(order.GameCode, orderList2);
                            logList.Add("重新添加成功。");
                            return string.Join(Environment.NewLine, logList);
                        }
                        //throw new Exception("追号列表中已存在KeyLine");
                    }

                    var schemeList = manager.QueryLotterySchemeByKeyLine(scheme.KeyLine);
                    if (schemeList == null || schemeList.Count <= 0)
                        throw new Exception("schemeList为空");

                    var orderList = new RedisWaitTicketOrderList();
                    orderList.KeyLine = scheme.KeyLine;
                    orderList.StopAfterBonus = detail.StopAfterBonus;
                    foreach (var item in schemeList)
                    {
                        var running = manager.QuerySports_Order_Running(item.SchemeId);
                        if (running == null) continue;

                        orderList.OrderList.Add(new RedisWaitTicketOrder
                        {
                            KeyLine = scheme.KeyLine,
                            StopAfterBonus = detail.StopAfterBonus,
                            SchemeType = (SchemeType)order.SchemeType,
                            AnteCodeList = codeList,
                            RunningOrder = running
                        });
                    }
                    if (orderList.OrderList == null || orderList.OrderList.Count <= 0)
                        throw new Exception("OrderList数据为空");
                    orderList.OrderList[0].RunningOrder.CanChase = true;
                    AddOrderToWaitSplitList(order.GameCode, orderList);
                }
                else if (order.SchemeBettingCategory == (int)SchemeBettingCategory.SingleBetting)
                {
                    logList.Add("订单是【单式上传订单】");
                    var singleCode = manager.QuerySingleScheme_AnteCode(schemeId);
                    if (singleCode == null)
                        throw new Exception("单式号码数据为空");

                    AddOrderToWaitSplitList(new RedisWaitTicketOrderSingle
                    {
                        RunningOrder = order,
                        AnteCode = singleCode,
                    });
                }
                else
                {
                    logList.Add("订单是【普通订单】");
                    AddOrderToWaitSplitList(new RedisWaitTicketOrder
                    {
                        KeyLine = order.SchemeId,
                        AnteCodeList = codeList,
                        RunningOrder = order,
                        SchemeType = (SchemeType)order.SchemeType,
                        StopAfterBonus = true,
                    });
                }

                logList.Add("添加完成");
            }
            catch (Exception ex)
            {
                logList.Add(ex.Message);
            }
            return string.Join(Environment.NewLine, logList);
        }

        /// <summary>
        /// 订单投注后加入Redis待拆票列表(追号订单)
        /// </summary>
        public static void AddOrderToWaitSplitList(string gameCode, RedisWaitTicketOrderList orderList)
        {
            if (orderList == null || orderList.OrderList.Count <= 0 || string.IsNullOrEmpty(orderList.KeyLine))
                return;

            //把追号订单以keyline为key存入
            var db = RedisHelper.DB_Chase_Order;
            foreach (var item in orderList.OrderList)
            {
                var json = JsonHelper.Serialize<RedisWaitTicketOrder>(item);
                db.ListRightPushAsync(orderList.KeyLine, json);
            }
            //把keyline存入Waiting_Chase_Order_List
            var fullKey = string.Format("{0}_{1}", RedisKeys.Key_Waiting_Chase_Order_List, gameCode);
            db.ListRightPushAsync(fullKey, orderList.KeyLine);
        }
        /// <summary>
        /// 订单投注后加入Redis待拆票列表(单式投注)
        /// </summary>
        public static void AddOrderToWaitSplitList(RedisWaitTicketOrderSingle order)
        {
            if (order == null || order.RunningOrder == null || order.AnteCode == null)
                return;

            var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "Single", order.RunningOrder.GameCode.ToUpper());
            var json = JsonHelper.Serialize<RedisWaitTicketOrderSingle>(order);
            var db = RedisHelper.DB_NoTicket_Order;
            db.ListRightPushAsync(fullKey, json);
        }

    }
}
