using System;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.ProxyGenerator;
using System.Threading.Tasks;
using KaSon.FrameWork.Common;

namespace OrderLottery.Service.ModuleBaseServices
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
