using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;
using MongoDB.Driver;

namespace Craw.Service.ModuleServices
{
    /// <summary>
    /// 采集服务
    /// </summary>
    public class CrawRepository : BaseRepository
    {
        ILogger<CrawRepository> _Log;
        public IMongoDatabase MDB { get; set; }
        //  public NLogger UserNLog;
        public CrawRepository(ILogger<CrawRepository> log, IMongoDatabase _mDB) {
            _Log = log;
            MDB = _mDB;
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
