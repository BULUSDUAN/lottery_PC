using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20130903Manager : GameBizEntityManagement
    {
        public void AddA20130903_JCZQ加奖(A20130903_JCZQ加奖 entity)
        {
            this.Add<A20130903_JCZQ加奖>(entity);
        }
    }
}
