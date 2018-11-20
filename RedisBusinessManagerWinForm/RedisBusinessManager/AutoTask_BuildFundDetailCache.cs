using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common.JSON;
using GameBiz.Service;

namespace RedisBusinessManager
{
    /// <summary>
    /// 生成资金明细缓存
    /// </summary>
    public class AutoTask_BuildFundDetailCache : AutoTaskBase, IAutoTask
    {
        private DateTime lastTime = DateTime.Now;
        public override string LogCategory
        {
            get { return "AutoTask_BuildFundDetailCache"; }
        }

        public override string TaskName
        {
            get { return "生成资金明细缓存"; }
        }

        public override int TaskOrder
        {
            get { return 0; }
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

                var strTime = ConfigurationManager.AppSettings["BuildFundDetailTime"].Split(':');
                if (strTime.Length != 2)
                    throw new Exception("BuildFundDetailTime配置不正确");
                Tools.ExcuteByTimer(1000 * 5, () =>
                {
                    var hh = strTime[0];
                    var mm = strTime[1];
                    this.WriteLog(string.Format("配置时间点是：{0}:{1}", hh, mm));
                if (DateTime.Now.ToString("HH") == hh && DateTime.Now.ToString("mm") == mm)
                {
                    try
                        {
                        if (lastTime.ToString("yyyyMMddhhmm") != DateTime.Now.ToString("yyyyMMddhhmm"))
                        {
                            //资金缓存根目录
                            var basePath = ConfigurationManager.AppSettings["FundDetailCacheBasePath"];
                                lastTime = DateTime.Now;
                                var saveDate = DateTime.Today.AddDays(-1);
                                var dateStr = saveDate.ToString("yyyyMMdd");
                                this.WriteLog("开始调用生成缓存服务");
                                var fundList =new GameBizWcfService_Fund().QueryFundDetailByDateTime(saveDate);
                                this.WriteLog(string.Format("已查询到资金明细共{0}条", fundList.FundDetailList.Count));
                                //按用户分组
                                var g = fundList.FundDetailList.GroupBy(p => p.UserId);
                                var userCount = g.Count();
                                this.WriteLog(string.Format("分组后用户数：{0} 条", userCount));
                                var index = 0;
                                foreach (var item in g)
                                {
                                    try
                                    {
                                        ++index;
                                        this.WriteLog(string.Format("生成用户 {0}={1}/{2}", item.Key, index, userCount));
                                        var dayList = fundList.FundDetailList.Where(p => p.UserId == item.Key).OrderByDescending(p => p.CreateTime).ToList();
                                        if (dayList.Count <= 0)
                                        {
                                            this.WriteLog("无资金明细数据");
                                            continue;
                                        }
                                        var content = JsonSerializer.Serialize<List<GameBiz.Core.FundDetailInfo>>(dayList);
                                        //var r = ServiceHelper.GameFundClient.BuildFundDetailByDate(content, item.Key, dateStr);
                                        var path = System.IO.Path.Combine(basePath, item.Key);
                                        if (!System.IO.Directory.Exists(path))
                                            System.IO.Directory.CreateDirectory(path);

                                        var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", dateStr));
                                        System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);

                                        this.WriteLog("生成成功");
                                    }
                                    catch (Exception ex)
                                    {
                                        this.WriteLog("生成异常：" + ex.Message);
                                    }
                                }

                           }
                        }
                        catch (Exception ex)
                        {
                            this.WriteLog("调用异常：" + ex.ToString());
                        }
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
