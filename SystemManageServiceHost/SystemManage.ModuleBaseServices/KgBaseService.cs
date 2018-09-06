using KaSon.FrameWork.ORM.Provider;
using System;
using Kason.Sg.Core.Caching;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Routing.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Kason.Sg.Core.CPlatform.Support;
using Kason.Sg.Core.CPlatform.Support.Attributes;
using Kason.Sg.Core.CPlatform.Transport.Implementation;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;
using System.Threading.Tasks;
using KaSon.FrameWork.Common;

namespace SystemManage.ModuleBaseServices
{
    public class KgBaseService: ProxyServiceBase, IIntegrationEventHandler<EventModel>
    {

        IKgLog log = null;
        public KgBaseService()
        {

            log = new Log4Log();

        }

        /// <summary>
        /// 服务发布消息
        /// </summary>
        /// <param name="evt"></param>
        public void PublicInfo(IntegrationEvent evt) {
            Publish(evt);
        }

        Task IIntegrationEventHandler<EventModel>.Handle(EventModel @event)
        {
            throw new NotImplementedException();
        }


        public IKgLog KgLog
        {
            get
            {
                if (log == null)
                {
                    log = new Log4Log();
                }
                return log;
            }
        }

        /// <summary>
        /// 服务关注消息
        /// </summary>
        //public async Task Handle(EventModel @event)
        //{
        //    //await _userService.Update(int.Parse(@event.UserId), new EventModel()
        //    //{

        //    //});
        // //   return Task.FromResult();
        //}

    }
}
