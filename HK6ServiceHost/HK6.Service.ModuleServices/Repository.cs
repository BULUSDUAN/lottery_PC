﻿using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Nlog;
using KaSon.FrameWork.ORM;

namespace HK6.ModuleBaseServices
{
    public class Repository : BaseRepository
    {
        ILogger<Repository> _Log;

        internal IDbProvider DB = null;
        //  public NLogger UserNLog;
        public Repository(ILogger<Repository> log, IDbProvider _DB) {
            _Log = log;
            DB = _DB;
           // UserNLog = (NLogger)_LogFactory.CreateLogger("User");
            //UserNLog = (NLogger)_LogFactory.CreateLogger("User");
        }
    }
}
