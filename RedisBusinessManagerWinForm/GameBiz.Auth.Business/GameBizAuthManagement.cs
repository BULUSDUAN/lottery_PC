using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using System.IO;
using System.Net;

namespace GameBiz.Auth.Business
{
    public class GameBizAuthEntityManagement : EntityManagement<GameBizAuthBusinessManagement>
    {
    }
    public class GameBizAuthBusinessManagement : BusinessManagement
    {
        public override string NHibernateConfigFileName
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.core.cfg.xml"); }
        }
    }
}
