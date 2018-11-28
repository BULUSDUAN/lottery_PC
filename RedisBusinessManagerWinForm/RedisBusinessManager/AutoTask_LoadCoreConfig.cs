using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using GameBiz.Service;

namespace RedisBusinessManager
{
    /// <summary>
    /// 加载系统配置
    /// </summary>
    public class AutoTask_LoadCoreConfig : AutoTaskBase, IAutoTask
    {
        public override string LogCategory
        {
            get { return "AutoTask_LoadCoreConfig"; }
        }

        public override string TaskName
        {
            get { return "加载系统配置"; }
        }

        public override int TaskOrder
        {
            get { return 7; }
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

                var span = int.Parse(ConfigurationManager.AppSettings["TimeSpan_Load_CoreConfig"]);
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
            var log =new GameBizWcfService_Core().LoadCoreConfigToRedis();
            this.WriteLog(log.Message);
            this.WriteLog("执行完成");
        }
    }
}
