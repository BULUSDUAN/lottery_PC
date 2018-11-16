using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SysLogManager:DBbase
    {
        public void AddSysOperationLog(C_Sys_OperationLog entity)
        {
            DB.GetDal<C_Sys_OperationLog>().Add(entity);
        }

        public List<SysOperationLogInfo> QuerySysOperationList(string menuName, string userId, string operUserId, DateTime startTime, DateTime endTimen, int pageIndex, int pageSize, out int totalCount)
        {
            var sTime = startTime.Date;
            var eTime = endTimen.AddDays(1).Date;
            var query = (from l in DB.CreateQuery<C_Sys_OperationLog>()
                        join u in DB.CreateQuery<C_User_Register>() on l.OperUserId equals u.UserId
                        where (menuName == "" || l.MenuName == menuName)
                        && (userId == "" || l.UserId == userId)
                        && (operUserId == "" || l.OperUserId == operUserId)
                        && (l.CreateTime >= sTime && l.CreateTime < eTime)
                        select new { l,u}).ToList().Select(p=> new SysOperationLogInfo
                        {
                            Id = p.l.Id,
                            OperUserId = p.l.OperUserId,
                            OperUserName = p.u.DisplayName,
                            UserId = p.l.UserId,
                            MenuName = p.l.MenuName,
                            Description = p.l.Description,
                            CreateTime = p.l.CreateTime,
                        });
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
