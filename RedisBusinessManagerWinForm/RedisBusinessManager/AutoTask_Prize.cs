using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common.Lottery.Redis;
using System.Threading;
using StackExchange.Redis;
using System.Threading.Tasks;
using GameBiz.Service;

namespace RedisBusinessManager
{
    /// <summary>
    /// 数字彩派奖
    /// </summary>
    public abstract class AutoTask_Prize_SZC : AutoTaskBase, IAutoTask
    {
        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        //public static int Max_BetListCount
        //{
        //    get
        //    {
        //        try
        //        {
        //            return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
        //        }
        //        catch (Exception)
        //        {
        //            return 10;
        //        }
        //    }
        //}

        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_Prize_SZC_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("数字彩派奖_{0}", GetGameName(GameCode)); }
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

        private Dictionary<string, string> QuerySZCWinNumber(string gameCode)
        {
            try
            {
                var db = RedisHelper.DB_Match;
                var fullKey = string.Format("{0}_{1}", gameCode, RedisKeys.Key_MatchResult_List);
                var array = db.ListRange(fullKey);
                var dic = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    if (!item.HasValue)
                        continue;
                    var k_v = item.ToString().Split('^');
                    if (k_v.Length != 2)
                        continue;
                    dic.Add(k_v[0], k_v[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                return new Dictionary<string, string>();
            }
        }

        private Task AddTask(string issuse, RedisValue[] order)
        {
            return Task.Factory.StartNew((issuseNumber) =>
            {
                var fullKey = string.Format("{0}_{1}_{2}", GameCode, RedisKeys.Key_Running_Order_List, issuseNumber.ToString());
                this.WriteLog(string.Format("====调用WCF执行Key:{0}的订单派奖，本页{1}条====", fullKey, order.Length));
                var log = new GameBizWcfService_Core().PrizeOrder_SZC_Page(GameCode, issuseNumber.ToString(), fullKey, string.Join(Environment.NewLine, order));
                this.WriteLog(log);
            }, issuse);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;


                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_SZC"]);
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_SZC"]);
                var doMaxCount = int.Parse(ConfigurationManager.AppSettings["PrizeDoMaxCount_SZC"]);
                this.WriteLog(string.Format("间隔{0}毫秒，每页{1}条，最大执行{2}条订单", span, prize_pageSize, doMaxCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");

                        //老版
                        //var log = new GameBizWcfService_Core().PrizeOrder_SZC(GameCode, prize_pageSize, doMaxCount);
                        //this.WriteLog(log);

                        //查询db
                        IDatabase db = null;
                        if (new string[] { "SSQ", "DLT", "FC3D", "PL3" }.Contains(GameCode))
                            db = RedisHelper.DB_Running_Order_SCZ_DP;
                        if (new string[] { "CQSSC", "JX11X5", "SD11X5", "GD11X5", "GDKLSF", "JSKS", "SDKLPK3" }.Contains(GameCode))
                            db = RedisHelper.DB_Running_Order_SCZ_GP;
                        if (db == null)
                            throw new Exception(string.Format("找不到{0}对应的库", GameCode));

                        //查询当前缓存中的期号和开奖号
                        var dic = QuerySZCWinNumber(GameCode);

                        this.WriteLog("开启多线程任务");
                        var taskList = new List<Task>();
                        foreach (var item in dic)
                        {
                            var fullKey = string.Format("{0}_{1}_{2}", GameCode, RedisKeys.Key_Running_Order_List, item.Key);
                            var orderList = db.ListRangeAsync(fullKey).Result;
                            if (orderList.Length <= 0)
                                continue;

                            this.WriteLog(string.Format("{0}第{1}期{2}条", GameCode, item.Key, orderList.Length));

                            var pageIndex = 0;
                            var pageSize = 100;
                            while (true)
                            {
                                var currentList = orderList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                                if (currentList.Count <= 0)
                                    break;

                                taskList.Add(AddTask(item.Key, currentList.ToArray()));
                                pageIndex++;
                            }
                        }
                        this.WriteLog(string.Format("当前任务{0}个，开始执行", taskList.Count));
                        Task.WaitAll(taskList.ToArray());
                        this.WriteLog("==================所有任务执行完成============================");
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    //递归
                    AutoDoTask();
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }
    }

    #region 数字彩派奖

    /// <summary>
    /// 双色球派奖
    /// </summary>
    public class AutoTask_Prize_SZC_SSQ : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "SSQ"; }
        }

        public override int TaskOrder
        {
            get { return 80; }
        }
    }

    /// <summary>
    /// 大乐透
    /// </summary>
    public class AutoTask_Prize_SZC_DLT : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "DLT"; }
        }

        public override int TaskOrder
        {
            get { return 81; }
        }
    }

    /// <summary>
    /// 福彩3D
    /// </summary>
    public class AutoTask_Prize_SZC_FC3D : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "FC3D"; }
        }

        public override int TaskOrder
        {
            get { return 82; }
        }
    }

    /// <summary>
    /// 排列3
    /// </summary>
    public class AutoTask_Prize_SZC_PL3 : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "PL3"; }
        }

        public override int TaskOrder
        {
            get { return 83; }
        }
    }

    /// <summary>
    /// 重庆时时彩
    /// </summary>
    public class AutoTask_Prize_SZC_CQSSC : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "CQSSC"; }
        }

        public override int TaskOrder
        {
            get { return 84; }
        }
    }

    /// <summary>
    /// 江西11选5
    /// </summary>
    public class AutoTask_Prize_SZC_JX11X5 : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "JX11X5"; }
        }

        public override int TaskOrder
        {
            get { return 85; }
        }
    }

    /// <summary>
    /// 山东11选5
    /// </summary>
    public class AutoTask_Prize_SZC_SD11X5 : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "SD11X5"; }
        }

        public override int TaskOrder
        {
            get { return 85; }
        }
    }

    /// <summary>
    /// 广东11选5
    /// </summary>
    public class AutoTask_Prize_SZC_GD11X5 : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "GD11X5"; }
        }

        public override int TaskOrder
        {
            get { return 85; }
        }
    }

    /// <summary>
    /// 广东快乐十分
    /// </summary>
    public class AutoTask_Prize_SZC_GDKLSF : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "GDKLSF"; }
        }

        public override int TaskOrder
        {
            get { return 85; }
        }
    }

    /// <summary>
    /// 江苏快三
    /// </summary>
    public class AutoTask_Prize_SZC_JSKS : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "JSKS"; }
        }

        public override int TaskOrder
        {
            get { return 85; }
        }
    }

    /// <summary>
    /// 山东快乐扑克3
    /// </summary>
    public class AutoTask_Prize_SZC_SDKLPK3 : AutoTask_Prize_SZC
    {
        public override string GameCode
        {
            get { return "SDKLPK3"; }
        }

        public override int TaskOrder
        {
            get { return 85; }
        }
    }

    #endregion

    /// <summary>
    /// 传统足球派奖
    /// </summary>
    public abstract class AutoTask_Prize_CTZQ : AutoTaskBase, IAutoTask
    {

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        //public static int Max_PrizeListCount
        //{
        //    get
        //    {
        //        try
        //        {
        //            return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
        //        }
        //        catch (Exception)
        //        {
        //            return 10;
        //        }
        //    }
        //}

        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_Prize_CTZQ_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("传统足球派奖_{0}", GetGameName(GameCode)); }
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
        /// 执行任务
        /// </summary>
        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_CTZQ"]);
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_CTZQ"]);
                var doMaxCount = int.Parse(ConfigurationManager.AppSettings["PrizeDoMaxCount_CTZQ"]);
                this.WriteLog(string.Format("间隔{0}毫秒，每页{1}条，最大执行{2}条订单", span, prize_pageSize, doMaxCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");
                        var log = new GameBizWcfService_Core().PrizeOrder_CTZQ(GameCode, prize_pageSize, doMaxCount);
                        this.WriteLog(log);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    //递归
                    AutoDoTask();
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }
    }

    #region 传统足球派奖

    public class AutoTask_Prize_CTZQ_T14C : AutoTask_Prize_CTZQ
    {
        public override string GameCode
        {
            get { return "T14C"; }
        }

        public override int TaskOrder
        {
            get { return 86; }
        }
    }
    public class AutoTask_Prize_CTZQ_TR9 : AutoTask_Prize_CTZQ
    {
        public override string GameCode
        {
            get { return "TR9"; }
        }

        public override int TaskOrder
        {
            get { return 87; }
        }
    }
    public class AutoTask_Prize_CTZQ_T6BQC : AutoTask_Prize_CTZQ
    {
        public override string GameCode
        {
            get { return "T6BQC"; }
        }

        public override int TaskOrder
        {
            get { return 88; }
        }
    }
    public class AutoTask_Prize_CTZQ_T4CJQ : AutoTask_Prize_CTZQ
    {
        public override string GameCode
        {
            get { return "T4CJQ"; }
        }

        public override int TaskOrder
        {
            get { return 89; }
        }
    }

    #endregion

    /// <summary>
    /// 北单派奖
    /// </summary>
    public class AutoTask_Prize_BJDC : AutoTaskBase, IAutoTask
    {

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        //public static int Max_PrizeListCount
        //{
        //    get
        //    {
        //        try
        //        {
        //            return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
        //        }
        //        catch (Exception)
        //        {
        //            return 10;
        //        }
        //    }
        //}

        public override int TaskOrder
        {
            get { return 90; }
        }

        public override string LogCategory
        {
            get { return "AutoTask_Prize_BJDC"; }
        }

        public override string TaskName
        {
            get { return "北京单场派奖"; }
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
        /// 执行任务
        /// </summary>
        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_BJDC"]);
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_BJDC"]);
                var doMaxCount = int.Parse(ConfigurationManager.AppSettings["PrizeDoMaxCount_BJDC"]);
                this.WriteLog(string.Format("间隔{0}毫秒，每页{1}条，最大执行{2}条订单", span, prize_pageSize, doMaxCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");
                        var log = new GameBizWcfService_Core().PrizeOrder_BJDC(prize_pageSize, doMaxCount);
                        this.WriteLog(log);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    //递归
                    AutoDoTask();
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }
    }

    /// <summary>
    /// 竞彩足球派奖
    /// </summary>
    public class AutoTask_Prize_JCZQ : AutoTaskBase, IAutoTask
    {

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_PrizeListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public override int TaskOrder
        {
            get { return 91; }
        }

        public override string LogCategory
        {
            get { return "AutoTask_Prize_JCZQ"; }
        }

        public override string TaskName
        {
            get { return "竞彩足球派奖"; }
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
        /// 执行任务
        /// </summary>
        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey = string.Format("{0}_{1}", "JCZQ", RedisKeys.Key_Running_Order_List);
                DoPrize(fullKey);

                var maxCount = Max_PrizeListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                for (int i = 0; i < maxCount; i++)
                {
                    var fullKeyNew = string.Format("{0}_{1}_{2}", "JCZQ", RedisKeys.Key_Running_Order_List, i);

                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoPrize(k.ToString());
                    }, fullKeyNew);
                }

                //var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_JCZQ"]);
                //var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_JCZQ"]);
                //var doMaxCount = int.Parse(ConfigurationManager.AppSettings["PrizeDoMaxCount_JCZQ"]);
                //this.WriteLog(string.Format("间隔{0}毫秒，每页{1}条，最大执行{2}条订单", span, prize_pageSize, doMaxCount));
                //Tools.ExcuteByTimer(span, () =>
                //{
                //    try
                //    {
                //        this.WriteLog("开始执行派奖...");
                //        var log = new GameBizWcfService_Core().PrizeOrder_JCZQ(prize_pageSize, doMaxCount);
                //        this.WriteLog(log);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(ex.ToString());
                //    }
                //    //递归
                //    AutoDoTask();
                //});
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }

        private void DoPrize(string key)
        {
            try
            {
                if (base.BeStop)
                    return;

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_JCZQ"]);
                this.WriteLog(string.Format("间隔{0}毫秒，处理队列{1}", span, key));
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_JCZQ"]);
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");
                        var log = new GameBizWcfService_Core().PrizeOrder_JCZQ(prize_pageSize, key);
                        this.WriteLog(log);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    finally
                    {
                        //递归
                        DoPrize(key);
                    }
                });

            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                DoPrize(key);
            }
        }

    }


    /// <summary>
    /// 竞彩篮球派奖
    /// </summary>
    public class AutoTask_Prize_JCLQ : AutoTaskBase, IAutoTask
    {

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_PrizeListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public override int TaskOrder
        {
            get { return 92; }
        }

        public override string LogCategory
        {
            get { return "AutoTask_Prize_JCLQ"; }
        }

        public override string TaskName
        {
            get { return "竞彩篮球派奖"; }
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
        /// 执行任务
        /// </summary>
        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var fullKey = string.Format("{0}_{1}", "JCLQ", RedisKeys.Key_Running_Order_List);
                DoPrize(fullKey);

                var maxCount = Max_PrizeListCount;
                this.WriteLog(string.Format("启动队列：{0}个", maxCount));
                for (int i = 0; i < maxCount; i++)
                {
                    var fullKeyNew = string.Format("{0}_{1}_{2}", "JCLQ", RedisKeys.Key_Running_Order_List, i);

                    ThreadPool.QueueUserWorkItem((k) =>
                    {
                        DoPrize(k.ToString());
                    }, fullKeyNew);
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }

        private void DoPrize(string key)
        {
            try
            {
                if (base.BeStop)
                    return;

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_JCLQ"]);
                this.WriteLog(string.Format("间隔{0}毫秒，处理队列{1}", span, key));
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_JCLQ"]);
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");
                        var log = new GameBizWcfService_Core().PrizeOrder_JCLQ(prize_pageSize, key);
                        this.WriteLog(log);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    finally
                    {
                        //递归
                        DoPrize(key);
                    }
                });

            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                DoPrize(key);
            }
        }
    }

    /// <summary>
    /// 欧洲杯派奖
    /// </summary>
    public class AutoTask_Prize_OZB : AutoTaskBase, IAutoTask
    {

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_PrizeListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public override string LogCategory
        {
            get { return "AutoTask_Prize_OZB"; }
        }

        public override string TaskName
        {
            get { return "欧洲杯派奖"; }
        }

        public override int TaskOrder
        {
            get { return 93; }
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

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_OZB"]);
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_OZB"]);
                var doMaxCount = int.Parse(ConfigurationManager.AppSettings["PrizeDoMaxCount_OZB"]);
                this.WriteLog(string.Format("间隔{0}毫秒，每页{1}条，最大执行{2}条订单", span, prize_pageSize, doMaxCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");
                        var log = new GameBizWcfService_Core().PrizeOrder_OZB(prize_pageSize, doMaxCount);
                        this.WriteLog(log);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    //递归
                    AutoDoTask();
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }
    }



    /// <summary>
    /// 世界杯派奖
    /// </summary>
    public class AutoTask_Prize_SJB : AutoTaskBase, IAutoTask
    {

        /// <summary>
        /// 投注队列最大个数
        /// </summary>
        public static int Max_PrizeListCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["Max_PrizeListCount"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }

        public override string LogCategory
        {
            get { return "AutoTask_Prize_SJB"; }
        }

        public override string TaskName
        {
            get { return "世界杯派奖"; }
        }

        public override int TaskOrder
        {
            get { return 94; }
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

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Prize_SJB"]);
                var prize_pageSize = int.Parse(ConfigurationManager.AppSettings["PrizePageSize_SJB"]);
                var doMaxCount = int.Parse(ConfigurationManager.AppSettings["PrizeDoMaxCount_SJB"]);
                this.WriteLog(string.Format("间隔{0}毫秒，每页{1}条，最大执行{2}条订单", span, prize_pageSize, doMaxCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行派奖...");
                        var log = new GameBizWcfService_Core().PrizeOrder_SJB(prize_pageSize, doMaxCount);
                        this.WriteLog(log);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(ex.ToString());
                    }
                    //递归
                    AutoDoTask();
                });
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());

                //递归
                AutoDoTask();
            }
        }
    }
}
