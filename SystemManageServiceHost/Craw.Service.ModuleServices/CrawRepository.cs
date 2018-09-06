using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;

namespace Craw.Service.ModuleServices
{
    /// <summary>
    /// 采集服务
    /// </summary>
    public class CrawRepository : BaseRepository
    {
        ILogger<CrawRepository> _Log;

      //  public NLogger UserNLog;
        public CrawRepository(ILogger<CrawRepository> log) {
            _Log = log;
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
