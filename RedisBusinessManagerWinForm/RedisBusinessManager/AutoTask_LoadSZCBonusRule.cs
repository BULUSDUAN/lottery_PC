using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using GameBiz.Service;

namespace RedisBusinessManager
{
    /// <summary>
    /// 初始化数字彩中奖规则
    /// </summary>
    public class AutoTask_LoadSZCBonusRule : AutoTaskBase, IAutoTask
    {
        public void Start()
        {
            base.BeStop = false;

            AutoDoTask();
            DoTask();
        }

        public void Stop()
        {
            base.BeStop = true;
        }

        public override string LogCategory
        {
            get { return "AutoTask_LoadSZCBonusRule"; }
        }

        public override string TaskName
        {
            get { return "初始化数字彩中奖规则"; }
        }

        public override int TaskOrder
        {
            get { return 10; }
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

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Load_SZCBonusRule"]);
                this.WriteLog(string.Format("间隔{0}毫秒执行任务", span));
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
            var log = new GameBizWcfService_Core().LoadSZCBonusRule();
            this.WriteLog(log.Message);
            this.WriteLog("执行完成");
        }
    }
}
