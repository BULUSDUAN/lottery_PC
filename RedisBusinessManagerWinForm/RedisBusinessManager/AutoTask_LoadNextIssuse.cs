using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using GameBiz.Service;

namespace RedisBusinessManager
{
    /// <summary>
    /// 加载未来奖期
    /// </summary>
    public class AutoTask_LoadNextIssuse : AutoTaskBase, IAutoTask
    {
        public override string LogCategory
        {
            get { return "AutoTask_LoadNextIssuse"; }
        }

        public override string TaskName
        {
            get { return "加载未来奖期"; }
        }

        public override int TaskOrder
        {
            get { return 9; }
        }

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

        /// <summary>
        /// 执行任务
        /// </summary>
        private void AutoDoTask()
        {
            try
            {
                if (base.BeStop)
                    return;

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Load_NextIssuseList"]);
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
            var log = new GameBizWcfService_Core().LoadNextIssuseList();
            this.WriteLog(log.Message);
            this.WriteLog("执行完成");
        }
    }
}
