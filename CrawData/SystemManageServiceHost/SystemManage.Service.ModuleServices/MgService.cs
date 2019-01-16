using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;

using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Sport;
using System.Threading.Tasks;

using System.IO;
using System.Text;
using EntityModel.ExceptionExtend;
using SystemManage.Service.ModuleServices;
using SystemManage.Service.IModuleServices;
using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using SystemManage.ModuleBaseServices;

namespace SystemManage.Service.ModuleServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ModuleName("mg")]
    public class MgService : KgBaseService, IMgService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<MgService> _Log;
        private readonly MgRepository _rep;
        public MgService(MgRepository repository, ILogger<MgService> log)
        {
            _Log = log;
            this._rep = repository;
        }

        public Task<CommonActionResult> Login(Sports_BetingInfo info, string password, decimal redBagMoney, string userid)
        {
            throw new NotImplementedException();
        }
    }
}
