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

        public UserRepository(ILoggerFactory LogFactory) {
            _LogFactory = LogFactory;
            //NLogger nl = (NLogger)_LogFactory.CreateLogger("User");

        }
    }
}
