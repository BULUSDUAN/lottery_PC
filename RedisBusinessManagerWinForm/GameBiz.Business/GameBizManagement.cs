using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using System.IO;
using System.Net;

namespace GameBiz.Business
{
    public class GameBizEntityManagement : EntityManagement<GameBizBusinessManagement>
    {
    }
    public class GameBizBusinessManagement : BusinessManagement
    {
        public override string NHibernateConfigFileName
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.core.cfg.xml"); }
        }
    }
}
