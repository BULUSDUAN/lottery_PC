using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using System.IO;
using System.Net;

namespace Validation.Business.Email
{
    public class ValidationEmailEntityManagement : EntityManagement<ValidationEmailBusinessManagement>
    {
    }
    public class ValidationEmailBusinessManagement : BusinessManagement
    {
        public override string NHibernateConfigFileName
        {
#if DEBUG
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.core.cfg.xml"); }
            //get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate." + Dns.GetHostName() + ".core.cfg.xml"); }
#else
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.core.cfg.xml"); }
#endif
        }
    }
}
