using GameBiz.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisBusinessManager
{
    /// <summary>
    /// 自动修复追号订单
    /// </summary>
    public abstract class AutoTask_RepairChaseOrder : AutoTaskBase, IAutoTask
    {
        public abstract string GameCode { get; }

        public override string LogCategory
        {
            get { return string.Format("AutoTask_RepairChaseOrder_{0}", GameCode); }
        }

        public override string TaskName
        {
            get { return string.Format("自动修复追号订单_{0}", GetGameName(GameCode)); }
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

                var span = 1000 * 10;
                var doMaxCount = 100;
                this.WriteLog(string.Format("彩种 {0} 间隔{1}毫秒，最大执行{2}条订单", GameCode, span, doMaxCount));
                Tools.ExcuteByTimer(span, () =>
                {
                    try
                    {
                        this.WriteLog("开始执行...");

                        var log = new GameBizWcfService_Core().RepairChaseOrder(this.GameCode, doMaxCount);

                        this.WriteLog(log);
                        this.WriteLog("==================执行完成一次============================");

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

        public void Stop()
        {
            base.BeStop = true;
        }
    }

    public class AutoTask_RepairChaseOrder_CQSSC : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "CQSSC"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_JX11X5 : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "JX11X5"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_SSQ : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "SSQ"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_DLT : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "DLT"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_FC3D : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "FC3D"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_PL3 : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "PL3"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_SD11X5 : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "SD11X5"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_GD11X5 : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "GD11X5"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_GDKLSF : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "GDKLSF"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_JSKS : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "JSKS"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
    public class AutoTask_RepairChaseOrder_SDKLPK3 : AutoTask_RepairChaseOrder
    {
        public override string GameCode
        {
            get { return "SDKLPK3"; }
        }

        public override int TaskOrder
        {
            get { return 100; }
        }
    }
}
