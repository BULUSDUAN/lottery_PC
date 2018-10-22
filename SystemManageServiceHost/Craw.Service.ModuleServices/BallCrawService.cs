using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;



using System.Threading.Tasks;

using System.IO;
using System.Text;

using Craw.Service.IModuleServices;
using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using SystemManage.ModuleBaseServices;
using EntityModel.ExceptionExtend;
using System.Threading;
using System.Collections.Concurrent;
using Lottery.CrawGetters.Auto;

namespace Craw.Service.ModuleServices
{
    /// <summary>
    /// 足球 篮球采集
    /// </summary>
    [ModuleName("ballcreaw")]
    public class BallCrawService : KgBaseService, IBallCrawService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<NumCrawService> _Log;
        private static IList<Service_AutoCollectWinNumber> aotoCollectList = new List<Service_AutoCollectWinNumber>();
        private static IList<Service_AutoCollectBonusPool> aotoPoolCollectList = new List<Service_AutoCollectBonusPool>();
        private readonly CrawRepository rep;
        public BallCrawService( ILogger<NumCrawService> log, CrawRepository _rep)
        {
            _Log = log;
           this.rep = _rep;
        }
       
        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> NumLettory_WinNumber_Start(string gameName)
        {
           
            lock (aotoCollectList) {
                bool bol = false;
                switch (gameName)
                {
                    //重庆时时彩
                    case "CQSSC": //重庆时时彩
                    case "JX11X5"://江西11选5
                    case "SD11X5"://11选5
                    case "GD11X5":
                    case "GDKLSF":
                    case "JSKS":
                    case "SDKLPK3":
                    case "FC3D":
                    case "PL3":
                    case "SSQ":
                    case "DLT":
                        bol = true;
                       
                        break;
                    default:
                        break;
                }
                if (bol)
                {
                    var p = aotoCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                    if (p == null)
                    {
                        //执行任务
                        Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(TimeSpan.FromSeconds(20));
                        auto.Start(gameName, new CrawORMService(rep.MDB).Start);
                        aotoCollectList.Add(auto);
                    }
                }
            }


            return Task.FromResult( "数字彩采集开奖号-开始服务");
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<string> NumLettory_WinNumber_Stop(string name)
        {
            lock (aotoCollectList)
            {
                switch (name)
                {
                    //重庆时时彩
                    case "CQSSC": //重庆时时彩
                    case "JX11X5"://江西11选5
                    case "SD11X5"://11选5
                    case "GD11X5":
                    case "GDKLSF":
                    case "JSKS":
                    case "SDKLPK3":
                    case "FC3D":
                    case "PL3":
                    case "SSQ":
                    case "DLT":
                        var p = aotoCollectList.Where(b => b.Key == name).FirstOrDefault();
                        if (p != null)
                        {
                            p.Stop();
                            ////执行任务
                            //Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(TimeSpan.FromSeconds(20));
                            //auto.Start(name);
                            //aotoCollectList.Add(auto);

                        }
                        break;
                    default:
                        break;
                }
            }


            throw new NotImplementedException();
        }

        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> NumLettory_BonusPool_Start(string gameName)
        {

            lock (aotoCollectList)
            {
                //执行任务
                Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool();
                        auto.Start(gameName, new CrawORMService(rep.MDB).BonusPoolStart);
                aotoPoolCollectList.Add(auto);
                
            }


            return Task.FromResult("数字彩采集开奖号-开始服务");
        }

        public Task<string> Login(string name)
        {
            throw new NotImplementedException();
        }

        //WinNumber
    }
}
