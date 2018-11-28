using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using FacaiActivity.Domain.Entities;

namespace FacaiActivity.Domain.Managers
{
    public class A20120101_通用加奖Manager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddAppendBonusRecord(A20120101_通用加奖_AppendBonus record)
        {
            record.CreateTime = DateTime.Now;

            Add<A20120101_通用加奖_AppendBonus>(record);
        }
    }
}
