using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;

namespace SystemManage.Service.ModuleServices
{
    public class MgRepository : BaseRepository
    {
        ILogger<MgRepository> _Log;

      //  public NLogger UserNLog;
        public MgRepository(ILogger<MgRepository> log) {
            _Log = log;
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
