using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;

namespace UserLottery.Service.ModuleServices
{
    public class BettingRepository : BaseRepository
    {
        ILogger<BettingRepository> _Log;
      //  ILoggerFactory logfactory;
      //  public NLogger UserNLog;
        public BettingRepository(ILogger<BettingRepository> log) {
            _Log = log;

            // _LogFactory.CreateLogger
            //     logfactory.CreateLogger<BettingRepository>();
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
