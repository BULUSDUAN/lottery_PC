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
      //  private readonly MgRepository _rep;
        public CrawService( ILogger<CrawService> log)
        {
            _Log = log;
         //   this._rep = repository;
        }

        public Task<string> Login(string name)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 数字彩采集开奖号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<string> NumLettory_WinNumber(string name)
        {
            switch (name)
            {
                //重庆时时彩
                case "CQSSC":

                break;
                default:
                    break;
            }

            throw new NotImplementedException();
        }
        //WinNumber
    }
}
