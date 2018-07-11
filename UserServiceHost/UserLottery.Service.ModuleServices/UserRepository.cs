using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;

namespace UserLottery.Service.ModuleServices
{
    public class UserRepository : BaseRepository
    {
        ILoggerFactory _LogFactory;

        public NLogger UserNLog;
        public UserRepository(ILoggerFactory LogFactory) {
            _LogFactory = LogFactory;
<<<<<<< HEAD
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
=======
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
>>>>>>> 83079022a291ba9001970a6c3a988eee210f84b7
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
