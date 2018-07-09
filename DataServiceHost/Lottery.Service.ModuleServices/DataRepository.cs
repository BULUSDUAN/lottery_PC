using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;
using log4net.Repository.Hierarchy;

namespace Lottery.Service.ModuleServices
{
    public class DataRepository : BaseRepository
    {
        Microsoft.Extensions.Logging.ILoggerFactory _LogFactory;

        public DataRepository(Microsoft.Extensions.Logging.ILoggerFactory LogFactory) {
            _LogFactory = LogFactory;
            //NLogger nl = (NLogger)_LogFactory.CreateLogger("Data");

        }
    }
}
