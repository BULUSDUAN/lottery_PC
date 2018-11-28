using System;
using System.Linq;
using System.Transactions;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using GameBiz.Auth.Business;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Core : WcfService
    {
        public GameBizWcfService_Core()
        {
            KnownTypeRegister.RegisterKnownTypes(CommunicationObjectGetter.GetCommunicationObjectTypes());
        }
    }
}
