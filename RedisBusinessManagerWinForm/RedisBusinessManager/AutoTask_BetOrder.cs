using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common.Lottery.Redis;
using GameBiz.Core;
using Common.JSON;
using System.Threading;
using System.Threading.Tasks;
using GameBiz.Service;

namespace RedisBusinessManager
{
    #region 普通数字彩订单

    /// <summary>
    /// 处理投注消息队列中的普通数字彩订单
    /// </summary>
    public abstract class AutoTask_BetOrder_Lottery : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_BetOrder_Lottery_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("订单队列-普通数字彩({0})", GetGameName(GameCode)); }
        }

        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
        }

        public void Stop()
        {
            base.BeStop = true;
        }

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public int Max_BetListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_Lottery_OrderBet_List, GameCode);
                DoOneListOrder(fullKey_old);

                var maxCount = Max_BetListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                //多线程处理多个队列
                for (int i = 0; i <= maxCount; i++)
                {
                    var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Lottery_OrderBet_List, GameCode, i);
                    //Task.Factory.StartNew((k) => {
                    //    DoOneListOrder(k.ToString());
                    //},fullKey);
                    //ThreadPool.UnsafeQueueUserWorkItem((k) =>
                    //{
                    //    DoOneListOrder(k.ToString());
                    //}, fullKey);
                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoOneListOrder(k.ToString());
                    }, fullKey);
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }

        private void DoOneListOrder(string fullKey)
        {
            try
            {
                if (base.BeStop)
                    return;
                var span = 500;
                //this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        var db = RedisHelper.DB_NoTicket_Order;
                        //this.WriteLog("开始执行队列订单处理..." + fullKey);
                        var orderArray = db.ListRangeAsync(fullKey).Result;
                        if (orderArray.Length > 0)
                        { 
                            this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
                            foreach (var item in orderArray)
                            {
                                if (!item.HasValue)
                                    continue;

                                var json = item.ToString();
                                try
                                {
                                    var info = JsonSerializer.Deserialize<RedisBet_LotteryBetting>(json);
                                    var c = (GameCode == "OZB") ? new GameBizWcfService_Core().BetOZB(info.BetInfo, info.BalancePassword, info.RedBagMoney, info.UserToken)
                                        : new GameBizWcfService_Core().LotteryBetting(info.BetInfo, info.BalancePassword, info.RedBagMoney, info.UserToken);
                                    this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
                                }
                                catch (Exception ex)
                                {
                                    this.WriteLog(json);
                                    this.WriteLog(ex.ToString());
                                }
                                finally
                                {
                                    db.ListRemoveAsync(fullKey, item);
                                }
                            }
                            this.WriteLog("===本次批投注完成===");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }

                    //递归
                    DoOneListOrder(fullKey);
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                //递归
                DoOneListOrder(fullKey);
            }
        }
    }
    public class AutoTask_BetOrder_Lottery_CQSSC : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "CQSSC"; }
        }

        public override int TaskOrder
        {
            get { return 94; }
        }
    }
    public class AutoTask_BetOrder_Lottery_JX11X5 : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "JX11X5"; }
        }

        public override int TaskOrder
        {
            get { return 95; }
        }
    }
    public class AutoTask_BetOrder_Lottery_SSQ : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "SSQ"; }
        }

        public override int TaskOrder
        {
            get { return 96; }
        }
    }
    public class AutoTask_BetOrder_Lottery_DLT : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "DLT"; }
        }

        public override int TaskOrder
        {
            get { return 97; }
        }
    }
    public class AutoTask_BetOrder_Lottery_FC3D : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "FC3D"; }
        }

        public override int TaskOrder
        {
            get { return 98; }
        }
    }
    public class AutoTask_BetOrder_Lottery_PL3 : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "PL3"; }
        }

        public override int TaskOrder
        {
            get { return 99; }
        }
    }
    public class AutoTask_BetOrder_Lottery_CTZQ : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "CTZQ"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_BetOrder_Lottery_OZB : AutoTask_BetOrder_Lottery
    {
        public override string GameCode
        {
            get { return "OZB"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }

    #endregion

    #region 普通足彩订单

    /// <summary>
    /// 处理投注消息队列中的普通足彩订单
    /// </summary>
    public abstract class AutoTask_BetOrder_Sports : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_BetOrder_Sports_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("订单队列-普通足彩({0})", GetGameName(GameCode)); }
        }

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public int Max_BetListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
        }

        public void Stop()
        {
            base.BeStop = true;
        }

        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_Sports_OrderBet_List, GameCode);
                DoOneListOrder(fullKey_old);

                var maxCount = Max_BetListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                //多线程处理多个队列
                for (int i = 0; i <= maxCount; i++)
                {
                    var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Sports_OrderBet_List, GameCode, i);
                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoOneListOrder(k.ToString());
                    }, fullKey);
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }

        }

        private void DoOneListOrder(string fullKey)
        {
            try
            {
                if (base.BeStop)
                    return;
                var span = 1000;
                //this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        var db = RedisHelper.DB_NoTicket_Order;
                        //this.WriteLog("开始执行队列订单处理..." + fullKey);
                        var orderArray = db.ListRangeAsync(fullKey).Result;
                        if (orderArray.Length > 0)
                        {
                            this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
                            foreach (var item in orderArray)
                            {
                                if (!item.HasValue)
                                    continue;

                                var json = item.ToString();
                                try
                                {
                                    var info = JsonSerializer.Deserialize<RedisBet_SportsBetting>(json);
                                    var c = new GameBizWcfService_Core().Sports_Betting(info.BetInfo, info.BalancePassword, info.RedBagMoney, info.UserToken);
                                    this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
                                }
                                catch (Exception ex)
                                {
                                    this.WriteLog(json);
                                    this.WriteLog(ex.ToString());
                                }
                                finally
                                {
                                    db.ListRemoveAsync(fullKey, item);
                                }
                            }
                            this.WriteLog("===本次批投注完成===");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }

                    //递归
                    DoOneListOrder(fullKey);
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                //递归
                DoOneListOrder(fullKey);
            }
        }
    }

    public class AutoTask_BetOrder_Sports_BJDC : AutoTask_BetOrder_Sports
    {
        public override string GameCode
        {
            get { return "BJDC"; }
        }

        public override int TaskOrder
        {
            get { return 101; }
        }
    }
    public class AutoTask_BetOrder_Sports_JCZQ : AutoTask_BetOrder_Sports
    {
        public override string GameCode
        {
            get { return "JCZQ"; }
        }

        public override int TaskOrder
        {
            get { return 102; }
        }
    }
    public class AutoTask_BetOrder_Sports_JCLQ : AutoTask_BetOrder_Sports
    {
        public override string GameCode
        {
            get { return "JCLQ"; }
        }

        public override int TaskOrder
        {
            get { return 103; }
        }
    }

    #endregion

    #region 单式订单

    /// <summary>
    /// 处理投注消息队列中的单式上传订单
    /// </summary>
    //public abstract class AutoTask_BetOrder_SingleScheme : AutoTaskBase, IAutoTask
    //{
    //    public abstract string GameCode { get; }

    //    public override string LogCategory
    //    {
    //        get { return string.Format("AutoTask_BetOrder_SingleScheme_{0}", GameCode); }
    //    }

    //    public override string TaskName
    //    {
    //        get { return string.Format("订单队列-单式上传({0})", GetGameName(GameCode)); }
    //    }

    //    /// <summary>
    //    /// 投注队列最大个数
    //    /// </summary>
    //    public int Max_BetListCount
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
    //            }
    //            catch (Exception)
    //            {
    //                return 10;
    //            }
    //        }
    //    }

    //    public void Start()
    //    {
    //        base.BeStop = false;

    //        AutoDoTask();
    //    }

    //    public void Stop()
    //    {
    //        base.BeStop = true;
    //    }

    //    private void AutoDoTask()
    //    {
    //        try
    //        {
    //            if (base.BeStop)
    //                return;

    //            var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_SingleScheme_OrderBet_List, GameCode);
    //            DoOneListOrder(fullKey_old);

    //            var maxCount = Max_BetListCount;
    //            this.WriteLog(string.Format("启动队列：{0}个", maxCount));
    //            //多线程处理多个队列
    //            for (int i = 0; i <= maxCount; i++)
    //            {
    //                var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_SingleScheme_OrderBet_List, GameCode, i);
    //                ThreadPool.QueueUserWorkItem((k) =>
    //                {
    //                    DoOneListOrder(k.ToString());
    //                }, fullKey);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            this.WriteLog(ex.ToString());

    //            //递归
    //            AutoDoTask();
    //        }
    //    }

    //    private void DoOneListOrder(string fullKey)
    //    {
    //        try
    //        {
    //            if (base.BeStop)
    //                return;
    //            var span = 500;
    //            this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
    //            Tools.ExcuteByTimer(span, () =>
    //            {
    //                try
    //                {
    //                    var db = RedisHelper.DB_NoTicket_Order;
    //                    this.WriteLog("开始执行队列订单处理..." + fullKey);
    //                    var orderArray = db.ListRangeAsync(fullKey).Result;
    //                    this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
    //                    foreach (var item in orderArray)
    //                    {
    //                        if (!item.HasValue)
    //                            continue;

    //                        var json = item.ToString();
    //                        try
    //                        {
    //                            //var info = JsonSerializer.Deserialize<RedisBet_SportsBetting>(json);
    //                            //var c = new GameBizWcfService_Core().Sports_Betting(info.BetInfo, info.BalancePassword, info.RedBagMoney, info.UserToken);
    //                            //this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            this.WriteLog(json);
    //                            this.WriteLog(ex.ToString());
    //                        }
    //                        finally
    //                        {
    //                            db.ListRemoveAsync(fullKey, item);
    //                        }
    //                    }
    //                    this.WriteLog("===本次批投注完成===");
    //                }
    //                catch (Exception ex)
    //                {
    //                    this.WriteLog(ex.ToString());
    //                }

    //                //递归
    //                DoOneListOrder(fullKey);
    //            });
    //        }
    //        catch (Exception ex)
    //        {
    //            this.WriteLog(ex.ToString());
    //            //递归
    //            DoOneListOrder(fullKey);
    //        }
    //    }

    //}

    //public class AutoTask_BetOrder_SingleScheme_JCZQ : AutoTask_BetOrder_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "JCZQ"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 104; }
    //    }
    //}
    //public class AutoTask_BetOrder_SingleScheme_JCLQ : AutoTask_BetOrder_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "JCLQ"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 105; }
    //    }
    //}
    //public class AutoTask_BetOrder_SingleScheme_BJDC : AutoTask_BetOrder_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "BJDC"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 106; }
    //    }
    //}
    //public class AutoTask_BetOrder_SingleScheme_CTZQ : AutoTask_BetOrder_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "CTZQ"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 107; }
    //    }
    //}

    #endregion

    #region 优化订单

    /// <summary>
    /// 处理投注消息队列中的优化订单
    /// </summary>
    public abstract class AutoTask_BetOrder_YouHua : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_BetOrder_YouHua_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("订单队列-优化订单({0})", GetGameName(GameCode)); }
        }

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public int Max_BetListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
        }

        public void Stop()
        {
            base.BeStop = true;
        }

        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_YouHua_OrderBet_List, GameCode);
                DoOneListOrder(fullKey_old);

                var maxCount = Max_BetListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                //多线程处理多个队列
                for (int i = 0; i <= maxCount; i++)
                {
                    var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_YouHua_OrderBet_List, GameCode, i);
                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoOneListOrder(k.ToString());
                    }, fullKey);
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }

        private void DoOneListOrder(string fullKey)
        {
            try
            {
                if (base.BeStop)
                    return;
                var span = 500;
                //this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        var db = RedisHelper.DB_NoTicket_Order;
                        //this.WriteLog("开始执行队列订单处理..." + fullKey);
                        var orderArray = db.ListRangeAsync(fullKey).Result;
                        if (orderArray.Length > 0)
                        {
                            this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
                            foreach (var item in orderArray)
                            {
                                if (!item.HasValue)
                                    continue;

                                var json = item.ToString();
                                try
                                {
                                    var info = JsonSerializer.Deserialize<RedisBet_YouHuaBet>(json);
                                    var c = new GameBizWcfService_Core().YouHuaBet(info.BetInfo, info.BalancePassword, info.RealTotalMoney, info.RedBagMoney, info.UserToken);
                                    this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
                                }
                                catch (Exception ex)
                                {
                                    this.WriteLog(json);
                                    this.WriteLog(ex.ToString());
                                }
                                finally
                                {
                                    db.ListRemoveAsync(fullKey, item);
                                }
                            }
                            this.WriteLog("===本次批投注完成===");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }

                    //递归
                    DoOneListOrder(fullKey);
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                //递归
                DoOneListOrder(fullKey);
            }
        }

    }

    public class AutoTask_BetOrder_YouHua_JCZQ : AutoTask_BetOrder_YouHua
    {
        public override string GameCode
        {
            get { return "JCZQ"; }
        }

        public override int TaskOrder
        {
            get { return 108; }
        }
    }

    #endregion

    #region 普通合买订单

    /// <summary>
    /// 处理投注消息队列中的普通合买订单
    /// </summary>
    public abstract class AutoTask_BetOrder_Together_Sports : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_BetOrder_Together_Sports_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("订单队列-普通合买订单({0})", GetGameName(GameCode)); }
        }

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public int Max_BetListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
        }

        public void Stop()
        {
            base.BeStop = true;
        }

        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_Together_Sports_OrderBet_List, GameCode);
                DoOneListOrder(fullKey_old);

                var maxCount = Max_BetListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                //多线程处理多个队列
                for (int i = 0; i <= maxCount; i++)
                {
                    var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Together_Sports_OrderBet_List, GameCode, i);
                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoOneListOrder(k.ToString());
                    }, fullKey);
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }

        private void DoOneListOrder(string fullKey)
        {
            try
            {
                if (base.BeStop)
                    return;
                var span = 500;
                //this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        var db = RedisHelper.DB_NoTicket_Order;
                        //this.WriteLog("开始执行队列订单处理..." + fullKey);
                        var orderArray = db.ListRangeAsync(fullKey).Result;
                        if (orderArray.Length > 0)
                        {
                            this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
                            foreach (var item in orderArray)
                            {
                                if (!item.HasValue)
                                    continue;

                                var json = item.ToString();
                                try
                                {
                                    var info = JsonSerializer.Deserialize<RedisBet_CreateSportsTogether>(json);
                                    var c = new GameBizWcfService_Core().CreateSportsTogether(info.BetInfo, info.BalancePassword, info.UserToken);
                                    this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
                                }
                                catch (Exception ex)
                                {
                                    this.WriteLog(json);
                                    this.WriteLog(ex.ToString());
                                }
                                finally
                                {
                                    db.ListRemoveAsync(fullKey, item);
                                }
                            }
                            this.WriteLog("===本次批投注完成===");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }

                    //递归
                    DoOneListOrder(fullKey);
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                //递归
                DoOneListOrder(fullKey);
            }
        }

    }

    public class AutoTask_BetOrder_Together_Sports_SSQ : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "SSQ"; }
        }

        public override int TaskOrder
        {
            get { return 109; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_DLT : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "DLT"; }
        }

        public override int TaskOrder
        {
            get { return 110; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_FC3D : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "FC3D"; }
        }

        public override int TaskOrder
        {
            get { return 111; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_PL3 : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "PL3"; }
        }

        public override int TaskOrder
        {
            get { return 112; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_JCZQ : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "JCZQ"; }
        }

        public override int TaskOrder
        {
            get { return 113; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_JCLQ : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "JCLQ"; }
        }

        public override int TaskOrder
        {
            get { return 114; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_BJDC : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "BJDC"; }
        }

        public override int TaskOrder
        {
            get { return 115; }
        }
    }
    public class AutoTask_BetOrder_Together_Sports_CTZQ : AutoTask_BetOrder_Together_Sports
    {
        public override string GameCode
        {
            get { return "CTZQ"; }
        }

        public override int TaskOrder
        {
            get { return 116; }
        }
    }

    #endregion

    #region 优化合买订单

    /// <summary>
    /// 处理投注消息队列中的优化合买订单
    /// </summary>
    public abstract class AutoTask_BetOrder_Together_YouHua : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_BetOrder_Together_YouHua_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("订单队列-优化合买订单({0})", GetGameName(GameCode)); }
        }

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public int Max_BetListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
        }

        public void Stop()
        {
            base.BeStop = true;
        }

        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_Together_YouHua_OrderBet_List, GameCode);
                DoOneListOrder(fullKey_old);

                var maxCount = Max_BetListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                //多线程处理多个队列
                for (int i = 0; i <= maxCount; i++)
                {
                    var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Together_YouHua_OrderBet_List, GameCode, i);
                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoOneListOrder(k.ToString());
                    }, fullKey);
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }

        }

        private void DoOneListOrder(string fullKey)
        {
            try
            {
                if (base.BeStop)
                    return;
                var span = 500;
                //this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        var db = RedisHelper.DB_NoTicket_Order;
                        //this.WriteLog("开始执行队列订单处理..." + fullKey);
                        var orderArray = db.ListRangeAsync(fullKey).Result;
                        if (orderArray.Length > 0)
                        {
                            this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
                            foreach (var item in orderArray)
                            {
                                if (!item.HasValue)
                                    continue;

                                var json = item.ToString();
                                try
                                {
                                    var info = JsonSerializer.Deserialize<RedisBet_CreateYouHuaSchemeTogether>(json);
                                    var c = new GameBizWcfService_Core().CreateYouHuaSchemeTogether(info.BetInfo, info.BalancePassword, info.RealTotalMoney, info.UserToken);
                                    this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
                                }
                                catch (Exception ex)
                                {
                                    this.WriteLog(json);
                                    this.WriteLog(ex.ToString());
                                }
                                finally
                                {
                                    db.ListRemoveAsync(fullKey, item);
                                }
                            }
                            this.WriteLog("===本次批投注完成===");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }

                    //递归
                    DoOneListOrder(fullKey);
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                //递归
                DoOneListOrder(fullKey);
            }
        }

    }

    public class AutoTask_BetOrder_Together_YouHua_JCZQ : AutoTask_BetOrder_Together_YouHua
    {
        public override string GameCode
        {
            get { return "JCZQ"; }
        }

        public override int TaskOrder
        {
            get { return 117; }
        }
    }

    #endregion

    #region 单式合买订单

    /// <summary>
    /// 处理投注消息队列中的单式合买订单
    /// </summary>
    //public abstract class AutoTask_BetOrder_Together_SingleScheme : AutoTaskBase, IAutoTask
    //{
    //    public abstract string GameCode { get; }

    //    public override string LogCategory
    //    {
    //        get { return string.Format("AutoTask_BetOrder_Together_SingleScheme_{0}", GameCode); }
    //    }

    //    public override string TaskName
    //    {
    //        get { return string.Format("订单队列-单式合买订单({0})", GetGameName(GameCode)); }
    //    }

    //    /// <summary>
    //    /// 投注队列最大个数
    //    /// </summary>
    //    public int Max_BetListCount
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return int.Parse(ConfigurationManager.AppSettings["Max_BetListCount"]);
    //            }
    //            catch (Exception)
    //            {
    //                return 10;
    //            }
    //        }
    //    }

    //    public void Start()
    //    {
    //        base.BeStop = false;

    //        AutoDoTask();
    //    }

    //    public void Stop()
    //    {
    //        base.BeStop = true;
    //    }

    //    private void AutoDoTask()
    //    {
    //        try
    //        {
    //            if (base.BeStop)
    //                return;

    //            var fullKey_old = string.Format("{0}_{1}", RedisKeys.Key_Together_SingleScheme_OrderBet_List, GameCode);
    //            DoOneListOrder(fullKey_old);

    //            var maxCount = Max_BetListCount;
    //            this.WriteLog(string.Format("启动队列：{0}个", maxCount));
    //            //多线程处理多个队列
    //            for (int i = 0; i <= maxCount; i++)
    //            {
    //                var fullKey = string.Format("{0}_{1}_{2}", RedisKeys.Key_Together_SingleScheme_OrderBet_List, GameCode, i);
    //                ThreadPool.QueueUserWorkItem((k) =>
    //                {
    //                    DoOneListOrder(k.ToString());
    //                }, fullKey);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            this.WriteLog(ex.ToString());

    //            //递归
    //            AutoDoTask();
    //        }
    //    }

    //    private void DoOneListOrder(string fullKey)
    //    {
    //        try
    //        {
    //            if (base.BeStop)
    //                return;
    //            var span = 500;
    //            this.WriteLog(string.Format("间隔{0}毫秒，处理队列 {1}", span, fullKey));
    //            Tools.ExcuteByTimer(span, () =>
    //            {
    //                try
    //                {
    //                    var db = RedisHelper.DB_NoTicket_Order;
    //                    this.WriteLog("开始执行队列订单处理..." + fullKey);
    //                    var orderArray = db.ListRangeAsync(fullKey).Result;
    //                    this.WriteLog(string.Format("队列{1}中有{0}条订单数据", orderArray.Length, fullKey));
    //                    foreach (var item in orderArray)
    //                    {
    //                        if (!item.HasValue)
    //                            continue;

    //                        var json = item.ToString();
    //                        try
    //                        {
    //                            //var info = JsonSerializer.Deserialize<RedisBet_SportsBetting>(json);
    //                            //var c = new GameBizWcfService_Core().Sports_Betting(info.BetInfo, info.BalancePassword, info.RedBagMoney, info.UserToken);
    //                            //this.WriteLog(string.Format("订单{0}投注结果:{1}", info.BetInfo.SchemeId, c.Message));
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            this.WriteLog(json);
    //                            this.WriteLog(ex.ToString());
    //                        }
    //                        finally
    //                        {
    //                            db.ListRemoveAsync(fullKey, item);
    //                        }
    //                    }
    //                    this.WriteLog("===本次批投注完成===");
    //                }
    //                catch (Exception ex)
    //                {
    //                    this.WriteLog(ex.ToString());
    //                }

    //                //递归
    //                DoOneListOrder(fullKey);
    //            });
    //        }
    //        catch (Exception ex)
    //        {
    //            this.WriteLog(ex.ToString());
    //            //递归
    //            DoOneListOrder(fullKey);
    //        }
    //    }

    //}

    //public class AutoTask_BetOrder_Together_SingleScheme_JCZQ : AutoTask_BetOrder_Together_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "JCZQ"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 118; }
    //    }
    //}
    //public class AutoTask_BetOrder_Together_SingleScheme_JCLQ : AutoTask_BetOrder_Together_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "JCLQ"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 119; }
    //    }
    //}
    //public class AutoTask_BetOrder_Together_SingleScheme_BJDC : AutoTask_BetOrder_Together_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "BJDC"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 120; }
    //    }
    //}
    //public class AutoTask_BetOrder_Together_SingleScheme_CTZQ : AutoTask_BetOrder_Together_SingleScheme
    //{
    //    public override string GameCode
    //    {
    //        get { return "CTZQ"; }
    //    }

    //    public override int TaskOrder
    //    {
    //        get { return 121; }
    //    }
    //}

    #endregion

}
