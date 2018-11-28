using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common.Lottery.Redis;
using System.Threading.Tasks;
using GameBiz.Service;

namespace RedisBusinessManager
{
    /// <summary>
    /// 自动任务-普通投注订单拆票
    /// </summary>
    public abstract class AutoTask_SplitTicket : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

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

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_SplitTicket"]);
                var maxSplitCount = int.Parse(ConfigurationManager.AppSettings["MaxSplitTicketCount"]);
                this.WriteLog(string.Format("间隔{0}毫秒，最大执行{1}条订单", span, maxSplitCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        DoTask(maxSplitCount);
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

        private void DoTask(int maxSplitCount)
        {
            //this.WriteLog("开始执行");
            //var log = new GameBizWcfService_Core().SplitOrderTicket(GameCode, maxSplitCount);
            //this.WriteLog(log);


            var count = int.Parse(ConfigurationManager.AppSettings["WaitingOrderListCount"]);
            this.WriteLog(string.Format("创建 {0} 个任务", count));
            var taskList = new List<Task>();
            taskList.Add(Task.Factory.StartNew(() =>
            {
                var key = string.Format("{0}_{1}_{2}", RedisKeys.Key_Waiting_Order_List, "General", GameCode);
                this.WriteLog(string.Format("开始对队列 {0} 拆票", key));
                var log = new GameBizWcfService_Core().SplitOrderTicketByKey(GameCode, key, maxSplitCount);
                WriteLog(log);
            }));
            for (int i = 0; i < count; i++)
            {
                taskList.Add(Task.Factory.StartNew((index) =>
                {
                    var key = string.Format("{0}_{1}_{2}_{3}", RedisKeys.Key_Waiting_Order_List, "General", GameCode, index.ToString());
                    this.WriteLog(string.Format("开始对队列 {0} 拆票", key));
                    var log = new GameBizWcfService_Core().SplitOrderTicketByKey(GameCode, key, maxSplitCount);
                    WriteLog(log);
                }, i));
            }

            this.WriteLog(string.Format("当前任务{0}个，开始执行", taskList.Count));
            Task.WaitAll(taskList.ToArray());
            this.WriteLog("==================所有任务执行完成============================");
        }

        public override string LogCategory
        {
            get { return string.Format("SplitTicket_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("自动拆票任务_{0}", GetGameName(GameCode)); }
        }
    }

    #region 各彩种拆票

    public class AutoTask_SplitTicket_SSQ : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "SSQ"; }
        }

        public override int TaskOrder
        {
            get { return 20; }
        }
    }
    public class AutoTask_SplitTicket_DLT : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "DLT"; }
        }

        public override int TaskOrder
        {
            get { return 21; }
        }
    }
    public class AutoTask_SplitTicket_FC3D : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "FC3D"; }
        }

        public override int TaskOrder
        {
            get { return 22; }
        }
    }
    public class AutoTask_SplitTicket_PL3 : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "PL3"; }
        }

        public override int TaskOrder
        {
            get { return 23; }
        }
    }
    public class AutoTask_SplitTicket_CQSSC : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "CQSSC"; }
        }

        public override int TaskOrder
        {
            get { return 24; }
        }
    }
    public class AutoTask_SplitTicket_JX11X5 : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "JX11X5"; }
        }

        public override int TaskOrder
        {
            get { return 25; }
        }
    }
    public class AutoTask_SplitTicket_CTZQ : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "CTZQ"; }
        }

        public override int TaskOrder
        {
            get { return 26; }
        }
    }
    public class AutoTask_SplitTicket_BJDC : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "BJDC"; }
        }

        public override int TaskOrder
        {
            get { return 27; }
        }
    }
    public class AutoTask_SplitTicket_JCZQ : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "JCZQ"; }
        }

        public override int TaskOrder
        {
            get { return 28; }
        }
    }
    public class AutoTask_SplitTicket_JCLQ : AutoTask_SplitTicket
    {
        public override string GameCode
        {
            get { return "JCLQ"; }
        }

        public override int TaskOrder
        {
            get { return 29; }
        }
    }

    #endregion

    /// <summary>
    /// 自动任务-单式订单拆票
    /// </summary>
    public abstract class AutoTask_SplitTicket_Single : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

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

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_SplitTicket_Single"]);
                var maxSplitCount = int.Parse(ConfigurationManager.AppSettings["MaxSplitTicketCount"]);
                this.WriteLog(string.Format("间隔{0}毫秒，最大执行{1}条订单", span, maxSplitCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        DoTask(maxSplitCount);
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

        private void DoTask(int maxSplitCount)
        {
            this.WriteLog("开始执行");
            var log = new GameBizWcfService_Core().SplitOrderTicket_Single(GameCode, maxSplitCount);
            this.WriteLog(log);
            this.WriteLog("执行完成");
        }

        public override string LogCategory
        {
            get { return string.Format("SplitTicket_Single_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("自动拆票任务(单式)_{0}", GetGameName(GameCode)); }
        }
    }

    #region 各彩种拆票

    public class AutoTask_SplitTicket_Single_CTZQ : AutoTask_SplitTicket_Single
    {
        public override string GameCode
        {
            get { return "CTZQ"; }
        }

        public override int TaskOrder
        {
            get { return 40; }
        }
    }
    public class AutoTask_SplitTicket_Single_BJDC : AutoTask_SplitTicket_Single
    {
        public override string GameCode
        {
            get { return "BJDC"; }
        }

        public override int TaskOrder
        {
            get { return 41; }
        }
    }
    public class AutoTask_SplitTicket_Single_JCZQ : AutoTask_SplitTicket_Single
    {
        public override string GameCode
        {
            get { return "JCZQ"; }
        }

        public override int TaskOrder
        {
            get { return 42; }
        }
    }
    public class AutoTask_SplitTicket_Single_JCLQ : AutoTask_SplitTicket_Single
    {
        public override string GameCode
        {
            get { return "JCLQ"; }
        }

        public override int TaskOrder
        {
            get { return 43; }
        }
    }

    #endregion

    /// <summary>
    /// 自动任务-追号订单拆票
    /// </summary>
    public abstract class AutoTask_ChaseOrderSplitTicket : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("ChaseOrderSplitTicket_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("追号订单自动拆票任务_{0}", GetGameName(GameCode)); }
        }

        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
        }

        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_ChaseOrderSplitTicket"]);
                this.WriteLog(string.Format("间隔{0}毫秒", span));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        DoTask();
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

        private void DoTask()
        {
            this.WriteLog("开始执行");
            var log = new GameBizWcfService_Core().SplitChaseOrderTicket(GameCode);
            this.WriteLog(log);
        }

        public void Stop()
        {
            base.BeStop = true;
        }
    }

    #region 各彩种拆票

    public class AutoTask_ChaseOrderSplitTicket_SSQ : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "SSQ"; }
        }

        public override int TaskOrder
        {
            get { return 50; }
        }
    }
    public class AutoTask_ChaseOrderSplitTicket_DLT : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "DLT"; }
        }

        public override int TaskOrder
        {
            get { return 51; }
        }
    }
    public class AutoTask_ChaseOrderSplitTicket_FC3D : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "FC3D"; }
        }

        public override int TaskOrder
        {
            get { return 52; }
        }
    }
    public class AutoTask_ChaseOrderSplitTicket_PL3 : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "PL3"; }
        }

        public override int TaskOrder
        {
            get { return 53; }
        }
    }
    public class AutoTask_ChaseOrderSplitTicket_CQSSC : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "CQSSC"; }
        }

        public override int TaskOrder
        {
            get { return 54; }
        }
    }
    public class AutoTask_ChaseOrderSplitTicket_JX11X5 : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "JX11X5"; }
        }

        public override int TaskOrder
        {
            get { return 55; }
        }
    }

    public class AutoTask_ChaseOrderSplitTicket_SD11X5 : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "SD11X5"; }
        }

        public override int TaskOrder
        {
            get { return 55; }
        }
    }

    public class AutoTask_ChaseOrderSplitTicket_GD11X5 : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "GD11X5"; }
        }

        public override int TaskOrder
        {
            get { return 55; }
        }
    }

    public class AutoTask_ChaseOrderSplitTicket_GDKLSF : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "GDKLSF"; }
        }

        public override int TaskOrder
        {
            get { return 55; }
        }
    }

    public class AutoTask_ChaseOrderSplitTicket_JSKS : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "JSKS"; }
        }

        public override int TaskOrder
        {
            get { return 55; }
        }
    }

    public class AutoTask_ChaseOrderSplitTicket_SDKLPK3 : AutoTask_ChaseOrderSplitTicket
    {
        public override string GameCode
        {
            get { return "SDKLPK3"; }
        }

        public override int TaskOrder
        {
            get { return 55; }
        }
    }

    #endregion
}
