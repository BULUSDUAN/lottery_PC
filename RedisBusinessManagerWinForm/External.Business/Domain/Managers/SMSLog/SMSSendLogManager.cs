using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using External.Domain.Entities.SMSLog;

namespace External.Domain.Managers.SMSLog
{
    public class SMSSendLogManager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddSMSSendLog(SMSSendLog entity)
        {
            this.Add<SMSSendLog>(entity);
        }

        public SMSSendLog QuerySMSSendLog(string keyLine)
        {
            Session.Clear();
            return this.Session.Query<SMSSendLog>().FirstOrDefault(p => p.KeyLine == keyLine);
        }
    }
}
