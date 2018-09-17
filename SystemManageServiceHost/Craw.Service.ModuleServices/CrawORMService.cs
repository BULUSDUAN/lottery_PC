using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.ORM.Helper.WinNumber;

namespace Craw.Service.ModuleServices
{
  public  class CrawORMService
    {

        public bool Start(string gameName,ConcurrentDictionary<string, string> all, Dictionary<string, string> dic) {


            if (all == null) return false;

            var query = from q in dic where !all.ContainsKey(q.Key) select q;
            if (query.Count() > 0)
            {
                foreach (var item in query)//
                {



                    // 采集到数据

                    //导入数据
                    ILotteryDataBusiness instan = KaSon.FrameWork.ORM.Helper.WinNumber.LotteryDataBusiness.GetTypeImport(gameName);// = new LotteryDataBusiness();
                    instan.ImportWinNumber( item.Key, item.Value);

                    //奖期派奖
                    new KJGameIssuseBusiness().IssusePrize(gameName, item.Key, item.Value);





                }
                //生成相关静态数据
                //try
                //{
                //    //step 3 生成相关静态数据
                //    var dpc = new string[] { "FC3D", "PL3", "SSQ", "DLT" };
                //    this.WriteLog("开始生成静态相关数据.");

                //    this.WriteLog("1.生成最新开奖号");
                //    var log = ServiceHelper.SendBuildStaticFileNotice("401", gameCode);
                //    this.WriteLog("1.生成最新开奖号结果:" + log);

                //    //if (dpc.Contains(gameCode))
                //    //{
                //    this.WriteLog("2.生成开奖结果首页");
                //    log = ServiceHelper.SendBuildStaticFileNotice("301");
                //    this.WriteLog("2.生成开奖结果首页结果：" + log);
                //    //}

                //    this.WriteLog("3.生成彩种开奖历史");
                //    log = ServiceHelper.SendBuildStaticFileNotice("302", gameCode);
                //    this.WriteLog("3.生成彩种开奖历史结果：" + log);

                //    this.WriteLog("4.生成彩种开奖详细");
                //    log = ServiceHelper.SendBuildStaticFileNotice("303", gameCode);
                //    this.WriteLog("4.生成彩种开奖详细结果：" + log);

                //    if (dpc.Contains(gameCode))
                //    {
                //        this.WriteLog("5.生成网站首页");
                //        log = ServiceHelper.SendBuildStaticFileNotice("10");
                //        this.WriteLog("5.生成网站首页结果：" + log);
                //    }

                //    this.WriteLog("6.生成走势图");
                //    log = ServiceHelper.SendBuildStaticFileNotice("900", gameCode);
                //    this.WriteLog("6.生成走势图结果：" + log);

                //    this.WriteLog("生成静态相关数据完成.");
                //}
                //catch (Exception ex)
                //{
                //    this.WriteLog("生成静态数据异常：" + ex.Message);
                //}
            }
            return false;
        }
    }
}
