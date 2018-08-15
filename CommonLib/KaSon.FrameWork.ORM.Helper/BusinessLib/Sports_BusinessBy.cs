using EntityModel.CoreModel;
using KaSon.FrameWork.Common.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Common;
using System.Threading.Tasks;
using System.Threading;

namespace KaSon.FrameWork.ORM.Helper.BusinessLib
{
    /// <summary>
    /// Sports_Business 扩容器
    /// </summary>
    public class Sports_BusinessBy
    {
        /// <summary>
        /// 最号订单处理
        /// </summary>
        /// <returns></returns>
        public static string WriteChaseOrderToDb()
        {
            var logList = new List<string>();
            logList.Add("<---------开始写入追号订单数据到数据库 ");
            Console.WriteLine("<---------开始写入追号订单数据到数据库 ");


            var maxDay = 5;
            var gameTypes = new LotteryGameManager().QueryEnableGameTypes();
            for (int i = 0; i < maxDay; i++)
            {
                var now = DateTime.Today.AddDays(-i);
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "ChaseOrder", now.ToString("yyyy-MM-dd"));
                logList.Add(string.Format("查询目录:{0} ", path));

                Console.WriteLine(string.Format("查询目录:{0} ", path));
                if (!Directory.Exists(path))
                    continue;

                var sportsManager = new Sports_Manager();
                var schemeManager = new SchemeManager();
           
                //日期下面只有一级文件
                var fileArray = Directory.GetFiles(path);
                logList.Add(string.Format("文件数：{0}个 ", fileArray.Length));
                Console.WriteLine(string.Format("文件数：{0}个 ", fileArray.Length));
                foreach (var item in fileArray)
                {
                    var json = File.ReadAllText(item, Encoding.UTF8);
                    var chaseOrderId = item.Substring(item.LastIndexOf("\\") + 1).Replace(".json", "");
                    var order = JsonSerializer.Deserialize<LotteryBettingInfo>(json);

                    order.IssuseNumberList.Sort((x, y) =>
                    {
                        return x.IssuseNumber.CompareTo(y.IssuseNumber);
                    });

                    logList.Add(string.Format("开始处理{0} ", chaseOrderId));
                    Console.WriteLine(string.Format("开始处理{0} ", chaseOrderId));
                    //一个追号订单，保存到数据库
                    try
                    {
                        var chaseSchemeList = sportsManager.QueryAllLotterySchemeByKeyLine(chaseOrderId);
                     //   Console.WriteLine(string.Format("开始处理1{0} ", chaseOrderId));
                        var schemeIdArray = chaseSchemeList.Select(p => p.SchemeId).ToArray();
                     //   Console.WriteLine(string.Format("开始处理2{0} ", chaseOrderId));
                        //查询三个订单表数据
                      //  string log1 = string.Join(",", schemeIdArray);
                     //   Console.WriteLine(string.Format("订单号****{0} ", log1));
                        var orderDetailList = schemeManager.QueryOrderDetailListBySchemeId(schemeIdArray);
                     //   Console.WriteLine(string.Format("开始处理3{0} ", chaseOrderId));
                        var orderRunningList = sportsManager.QueryOrderRunningBySchemeIdArray(schemeIdArray);
                      //  Console.WriteLine(string.Format("开始处理4{0} ", chaseOrderId));
                        var orderComplateList = sportsManager.QueryOrderComplateBySchemeIdArray(schemeIdArray);
                      //  Console.WriteLine(string.Format("开始处理5{0} ", chaseOrderId));
                        if (chaseSchemeList.Count == orderDetailList.Count && chaseSchemeList.Count == orderRunningList.Count + orderComplateList.Count)
                        {
                            //订单数据正常，删除订单文件
                            logList.Add("订单数据正常，删除订单文件 ");
                            Console.WriteLine("订单数据正常，删除订单文件 ");
                            File.Delete(item);
                            continue;
                        }

                        var gameTypeList = new List<GameTypeInfo>();
                        foreach (var code in order.AnteCodeList)
                        {
                            var t = gameTypes.FirstOrDefault(a => a.Game.GameCode == order.GameCode && a.GameType == code.GameType.ToUpper());
                            if (t != null && !gameTypeList.Contains(t))
                            {
                                gameTypeList.Add(t);
                            }
                        }
                        var gameType = string.Join(",", (from g in gameTypeList group g by g.GameType into g select g.Key).ToArray());

                        //计算OrderDetail表没有的数据并写入
                        foreach (var scheme in chaseSchemeList)
                        {
                            var orderDetail = orderDetailList.FirstOrDefault(p => p.SchemeId == scheme.SchemeId);
                            if (orderDetail == null)
                            {
                                logList.Add("写入orderDetail ");
                                Console.WriteLine("写入orderDetail ");
                                var currentIssuse = order.IssuseNumberList.FirstOrDefault(p => p.IssuseNumber == scheme.IssuseNumber);

                                schemeManager.AddOrderDetail(new C_OrderDetail
                                {
                                    SchemeId = scheme.SchemeId,
                                    AddMoney = 0M,
                                    AfterTaxBonusMoney = 0M,
                                    AgentId = string.Empty,
                                    Amount = currentIssuse == null ? 1 : currentIssuse.Amount,
                                    BetTime = DateTime.Now,
                                    BonusAwardsMoney = 0M,
                                    BonusStatus =(int) BonusStatus.Waitting,
                                    ComplateTime = null,
                                    CreateTime = DateTime.Now,
                                    CurrentBettingMoney = 0M,
                                    CurrentIssuseNumber = currentIssuse == null ? string.Empty : currentIssuse.IssuseNumber,
                                    GameCode = order.GameCode,
                                    GameType = gameType,
                                    GameTypeName = BettingHelper.FormatGameType(order.GameCode, gameType), //FormatGameType(order.GameCode, gameType),
                                    IsAppend = order.IsAppend,
                                    IsVirtualOrder = false,
                                    PlayType = string.Empty,
                                    PreTaxBonusMoney = 0M,
                                    ProgressStatus = (int)ProgressStatus.Waitting,
                                    RealPayRebateMoney = 0M,
                                    RedBagAwardsMoney = 0M,
                                    RedBagMoney = 0M,
                                    SchemeBettingCategory = (int)SchemeBettingCategory.GeneralBetting,
                                    SchemeSource = (int)order.SchemeSource,
                                    SchemeType = (int)SchemeType.ChaseBetting,
                                    StartIssuseNumber = string.Empty,
                                    StopAfterBonus = order.StopAfterBonus,
                                    TicketStatus = (int)TicketStatus.Waitting,
                                    TicketTime = null,
                                    TotalIssuseCount = order.IssuseNumberList.Count,
                                    TotalMoney = currentIssuse.IssuseTotalMoney,
                                    TotalPayRebateMoney = 0M,
                                    UserId = order.UserId,
                                });
                            }
                        }

                        //计算OrderRunning表没有的数据并写入
                        foreach (var scheme in chaseSchemeList)
                        {
                            var runningOrder = orderRunningList.FirstOrDefault(p => p.SchemeId == scheme.SchemeId);
                            var comlateOrder = orderComplateList.FirstOrDefault(p => p.SchemeId == scheme.SchemeId);
                            if (runningOrder == null && comlateOrder == null)
                            {
                                logList.Add("写入runningOrder ");
                                Console.WriteLine("写入runningOrder ");
                                var currentIssuse = order.IssuseNumberList.FirstOrDefault(p => p.IssuseNumber == scheme.IssuseNumber);
                                sportsManager.AddSports_Order_Running(new C_Sports_Order_Running
                                {
                                    AfterTaxBonusMoney = 0M,
                                    AgentId = string.Empty,
                                    Amount = currentIssuse == null ? 1 : currentIssuse.Amount,
                                    Attach = string.Empty,

                                    BonusStatus = (int)BonusStatus.Waitting,
                                    CanChase = false,
                                    IsVirtualOrder = false,
                                    IsPayRebate = false,
                                    RealPayRebateMoney = 0M,
                                    TotalPayRebateMoney = 0M,
                                    CreateTime = DateTime.Now,
                                    GameCode = order.GameCode,
                                    GameType = gameType,
                                    IssuseNumber = currentIssuse == null ? string.Empty : currentIssuse.IssuseNumber,
                                    PlayType = string.Empty,
                                    PreTaxBonusMoney = 0M,
                                    ProgressStatus = (int)ProgressStatus.Waitting,
                                    SchemeId = scheme.SchemeId,
                                    SchemeType = (int)SchemeType.ChaseBetting,
                                    SchemeBettingCategory = (int)SchemeBettingCategory.GeneralBetting,
                                    TicketId = string.Empty,
                                    TicketLog = string.Empty,
                                    TicketStatus = (int)TicketStatus.Waitting,
                                    TotalMatchCount = 0,
                                    TotalMoney = currentIssuse.IssuseTotalMoney,
                                    SuccessMoney = 0M,
                                    UserId = order.UserId,
                                    StopTime = DateTime.Now,
                                    SchemeSource = (int)SchemeSource.Web,
                                    BetCount = 0,
                                    BonusCount = 0,
                                    HitMatchCount = 0,
                                    RightCount = 0,
                                    Error1Count = 0,
                                    Error2Count = 0,
                                    MaxBonusMoney = 0,
                                    MinBonusMoney = 0,
                                    Security = (int)TogetherSchemeSecurity.Public,
                                    TicketGateway = string.Empty,
                                    TicketProgress = 1M,
                                    BetTime = DateTime.Now,
                                    ExtensionOne = string.Format("{0}{1}", "3X1_", (int)order.ActivityType),
                                    QueryTicketStopTime = DateTime.Now.AddMinutes(1).ToString("yyyyMMddHHmm"),
                                    IsAppend = false,
                                    RedBagMoney = 0,
                                    IsSplitTickets = false,
                                    TicketTime = null,
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("保存追号订单数据失败:{0}", ex.ToString()));
                        Console.WriteLine(string.Format("保存追号订单数据失败:{0}", ex.ToString()));
                    }
                }
            }
            //写入日志
            logList.Add("本次处理全部完成----------> ");
            Console.WriteLine("本次处理全部完成----------> ");

            string log= string.Join(Environment.NewLine, logList.ToArray());
            Console.WriteLine(log);
         //   string log = Common.JSON.JsonHelper.Serialize(logList);
            Log4Log.LogEX(KLogLevel.Info, "追号消息***", new Exception(log));

            return log;
        }


       public static void StartTaskByWriteChaseOrderToDb(int seconds) {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine(string.Format( "追号作业启动...每{0}秒执行一次",seconds));
                while (true)
                {
                    Thread.Sleep(1000 * seconds);
                   // Task.Delay(1000 * seconds);
                    Sports_BusinessBy.WriteChaseOrderToDb();
                    //this.WriteChaseOrderToDb() Task.FromResult(this.WriteChaseOrderToDb());
                }

            });
           // return 
        }


    }
}
