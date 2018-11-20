using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.NHibernate;
using System.IO;
using System.Net;

namespace Validation.Business.Mobile
{
    public class ValidationMobileEntityManagement : EntityManagement<ValidationMobileBusinessManagement>
    {
    }
    public class ValidationMobileBusinessManagement : BusinessManagement
    {
        public override string NHibernateConfigFileName
        {
#if DEBUG
            //get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate." + Dns.GetHostName() + ".core.cfg.xml"); }
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.core.cfg.xml"); }
#else
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.core.cfg.xml"); }
#endif
        }
    }
}
