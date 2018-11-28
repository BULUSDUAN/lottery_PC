using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;
using GameBiz.Domain.Entities;
using Activity.Core;

namespace Activity.Managers
{
    public class A20121210Manager : GameBizEntityManagement
    {
        public void AddA20121210_活动(A20121210_活动 entity)
        {
            this.Add<A20121210_活动>(entity);
        }

        public A20121210_活动 QueryA20121210_活动(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20121210_活动>().FirstOrDefault(p => p.UserId == userId);
        }

        public int QueryA20121210Count(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20121210_活动>().Count(p => p.UserId == userId);
        }
        public List<Activity.Core.OneDayTogetherBettingUserInfo> QueryOneDayTogetherUser(DateTime dateTime)
        {
            //            select o.UserId,COUNT(1)OrderCount
            //from  C_OrderDetail o 
            //where o.IsVirtualOrder=0  and o.SchemeType=3 and o.TotalMoney>=30
            //and o.CreateTime>='2012-12-9' and o.CreateTime<'2012-12-10'
            //group by o.UserId
            Session.Clear();
            var query = from o in Session.Query<OrderDetail>()
                        where o.TotalMoney >= 30M && !o.IsVirtualOrder && o.TicketStatus == GameBiz.Core.TicketStatus.Ticketed && o.SchemeType == GameBiz.Core.SchemeType.TogetherBetting
                        && o.CreateTime >= dateTime && o.CreateTime < dateTime.AddDays(1)
                        group o by o.UserId into t
                        select new Activity.Core.OneDayTogetherBettingUserInfo
                        {
                            UserId = t.Key,
                            OrderCount = t.Count(),
                        };
            return query.ToList();
        }

        public List<A20130111Info> QueryA20130111InfoListByBettingMoney(int count, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            //var gameCodeArray = new string[] { "CQSSC", "JXSSC", "JX11X5", "SD11X5", "GD11X5" };
            //var query = from d in Session.Query<OrderDetail>()
            //            where gameCodeArray.Contains(d.GameCode)
            //            && d.ProgressStatus == GameBiz.Core.ProgressStatus.Complate
            //            && d.TicketStatus == GameBiz.Core.TicketStatus.Ticketed
            //            && d.CreateTime >= startTime && d.CreateTime < endTime
            //            group d by d.UserId into t
            //            join u in Session.Query<UserRegister>() on t.Key equals u.UserId
            //            select new A20130111Info
            //            {
            //                UserId = t.Key,
            //                TotalMoney = t.Sum(p => p.TotalMoney),
            //                UserDisplayName = u.DisplayName,
            //                HideDisplayNameCount = u.HideDisplayNameCount,
            //            };
            //return query.OrderByDescending(p => p.TotalMoney).Take(count).ToList();

            var sql = string.Format(@"SELECT t.UserId,r.DisplayName,r.HideDisplayNameCount,t.TotalMoney AS TotalMoney
                                    FROM 
                                    (SELECT TOP {2} d.UserId,
                                    MAX(d.ProgressStatus)ProgressStatus,MAX(d.TicketStatus)TicketStatus,SUM(d.TotalMoney)TotalMoney
                                        FROM C_OrderDetail d
                                        where d.GameCode in ('CQSSC','JXSSC','JX11X5','SD11X5','GD11X5' )
                                        AND d.ProgressStatus=90 AND d.TicketStatus=90
                                        AND d.CreateTime >='{0}' AND d.CreateTime <'{1}'
                                        GROUP BY d.UserId
                                        ORDER BY TotalMoney DESC
                                        ) AS t
                                        INNER JOIN C_User_Register r ON t.UserId=r.UserId 
                                    where TotalMoney>=5000 ", startTime.ToString("yyyy-MM-dd"), endTime.ToString("yyyy-MM-dd"), count);
            var list = new List<A20130111Info>();
            foreach (var item in this.Session.CreateSQLQuery(sql).List())
            {
                var array = item as object[];
                list.Add(new A20130111Info
                {
                    UserId = array[0].ToString(),
                    UserDisplayName = array[1].ToString(),
                    HideDisplayNameCount = int.Parse(array[2].ToString()),
                    TotalMoney = decimal.Parse(array[3].ToString())
                });
            }
            return list;
        }

        public List<A20130111Info> QueryA20130111InfoListByBonusMoney(int count, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            //var gameCodeArray = new string[] { "CQSSC", "JXSSC", "JX11X5", "SD11X5", "GD11X5" };
            //var query = from d in Session.Query<OrderDetail>()
            //            where gameCodeArray.Contains(d.GameCode)
            //            && d.ProgressStatus == GameBiz.Core.ProgressStatus.Complate
            //            && d.TicketStatus == GameBiz.Core.TicketStatus.Ticketed
            //            && d.CreateTime >= startTime && d.CreateTime < endTime
            //            group d by d.UserId into t
            //            join u in Session.Query<UserRegister>() on t.Key equals u.UserId
            //            select new A20130111Info
            //            {
            //                UserId = t.Key,
            //                TotalMoney = t.Sum(p => p.AfterTaxBonusMoney),
            //                UserDisplayName = u.DisplayName,
            //                HideDisplayNameCount = u.HideDisplayNameCount,
            //            };
            //return query.OrderByDescending(p => p.TotalMoney).Take(count).ToList();

            var sql = string.Format(@"SELECT t.UserId,r.DisplayName,r.HideDisplayNameCount,t.BonusMoney AS TotalMoney
                                        FROM 
                                        (SELECT TOP {2} d.UserId,
                                        MAX(d.ProgressStatus)ProgressStatus,MAX(d.TicketStatus)TicketStatus,SUM(d.AfterTaxBonusMoney) BonusMoney
                                          FROM C_OrderDetail d
                                          where d.GameCode in ('CQSSC','JXSSC','JX11X5','SD11X5','GD11X5' )
                                          AND d.ProgressStatus=90 AND d.TicketStatus=90
                                          AND d.CreateTime >='{0}' AND d.CreateTime <'{1}'
                                          GROUP BY d.UserId
                                          ORDER BY BonusMoney DESC
                                          ) AS t
                                          INNER JOIN C_User_Register r ON t.UserId=r.UserId
                                        where t.BonusMoney>=5000 ", startTime.ToString("yyyy-MM-dd"), endTime.ToString("yyyy-MM-dd"), count);
            var list = new List<A20130111Info>();
            foreach (var item in this.Session.CreateSQLQuery(sql).List())
            {
                var array = item as object[];
                list.Add(new A20130111Info
                {
                    UserId = array[0].ToString(),
                    UserDisplayName = array[1].ToString(),
                    HideDisplayNameCount = int.Parse(array[2].ToString()),
                    TotalMoney = decimal.Parse(array[3].ToString())
                });
            }
            return list;
        }

    }
}
