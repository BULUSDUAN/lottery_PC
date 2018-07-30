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
        ILoggerFactory _LogFactory;

        public NLogger UserNLog;
        public BettingRepository(ILoggerFactory LogFactory) {
            _LogFactory = LogFactory;
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
