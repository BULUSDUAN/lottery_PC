using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20130806Manager : GameBizEntityManagement
    {
        public void AddA20130226VipFillMoneyGiveRecord(A20130226VipFillMoneyGiveRecord entity)
        {
            this.Add<A20130226VipFillMoneyGiveRecord>(entity);
        }

        public void AddA20130226VipScoreDetail(A20130226VipScoreDetail entity)
        {
            this.Add<A20130226VipScoreDetail>(entity);
        }

        public void AddA20130226VipScoreSummary(A20130226VipScoreSummary entity)
        {
            this.Add<A20130226VipScoreSummary>(entity);
        }

        public void UpdateA20130226VipScoreSummary(A20130226VipScoreSummary entity)
        {
            this.Update<A20130226VipScoreSummary>(entity);
        }

        public A20130226VipScoreSummary QueryA20130226VipScoreSummary(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20130226VipScoreSummary>().FirstOrDefault(p => p.UserId == userId);
        }

        public A20130226VipFillMoneyGiveRecord QueryA20130226VipFillMoneyGiveRecord(string userId, int vipLeavel)
        {
            Session.Clear();
            return this.Session.Query<A20130226VipFillMoneyGiveRecord>().FirstOrDefault(p => p.UserId == userId && p.VipLeavel == vipLeavel);
        }
    }
}
