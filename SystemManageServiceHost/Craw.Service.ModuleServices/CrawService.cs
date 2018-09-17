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
    [ModuleName("creaw")]
    public class CrawService : KgBaseService, ICrawService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<CrawService> _Log;
        private static IList<Service_AutoCollectWinNumber> aotoCollectList = new List<Service_AutoCollectWinNumber>();
        //  private readonly MgRepository _rep;
        public CrawService( ILogger<CrawService> log)
        {
            _Log = log;
         //   this._rep = repository;
        }
        public Task<string> CrawService_Start(string name)
        {
            switch (name)
            {
                case "CQSSC": //重庆时时彩

                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> NumLettory_WinNumber_Start(string gameName)
        {
            lock (aotoCollectList) {
                switch (gameName)
                {
                    //重庆时时彩
                    case "CQSSC":
                        var p = aotoCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                        if (p == null)
                        {
                            //执行任务
                            Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(TimeSpan.FromSeconds(20));
                            auto.Start(gameName,new CrawORMService().Start);
                            aotoCollectList.Add(auto);
                        }
                        break;
                    default:
                        break;
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
                    case "CQSSC":
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

        public Task<string> Login(string name)
        {
            throw new NotImplementedException();
        }

        //WinNumber
    }
}
