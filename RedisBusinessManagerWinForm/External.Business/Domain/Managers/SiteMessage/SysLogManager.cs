using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using NHibernate.Criterion;
using GameBiz.Core;
using External.Domain.Entities.AdminMenu;

namespace External.Domain.Managers.SiteMessage
{
    public class SysLogManager : GameBizEntityManagement
    {
        public void AddSysOperationLog(SysOperationLog entity)
        {
            this.Add<SysOperationLog>(entity);
        }
        public List<SysOperationLogInfo> QuerySysOperationList(string menuName, string userId, string operUserId, DateTime startTime, DateTime endTimen, int pageIndex, int pageSize, out int totalCount)
        {
            var sTime = startTime.Date;
            var eTime = endTimen.AddDays(1).Date;
            var query = from l in Session.Query<SysOperationLog>()
                        join u in Session.Query<UserRegister>() on l.OperUserId equals u.UserId
                        where (menuName == "" || l.MenuName == menuName)
                        && (userId == "" || l.UserId == userId)
                        && (operUserId == "" || l.OperUserId == operUserId)
                        && (l.CreateTime >= sTime && l.CreateTime < eTime)
                        select new SysOperationLogInfo
                        {
                            Id = l.Id,
                            OperUserId = l.OperUserId,
                            OperUserName = u.DisplayName,
                            UserId = l.UserId,
                            MenuName = l.MenuName,
                            Description = l.Description,
                            CreateTime = l.CreateTime,
                        };
            totalCount = query.ToList().Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
