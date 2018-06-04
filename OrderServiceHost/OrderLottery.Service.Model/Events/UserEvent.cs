using Kason.Sg.Core.CPlatform.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Service.Model
{
    public class UserEvent : IntegrationEvent
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Age { get; set; }
    }
}
