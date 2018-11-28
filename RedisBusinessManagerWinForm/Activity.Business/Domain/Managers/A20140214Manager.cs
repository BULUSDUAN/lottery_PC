using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using GameBiz.Business;
using Activity.Domain.Entities;

namespace Activity.Business.Domain.Managers
{
    public class A20140214Manager : GameBizEntityManagement
    {
        /// <summary>
        /// 添加首次充值数据
        /// </summary>
        public void AddA20140214_首次充20送11(A20140214_首次充20送11 entity)
        {
            this.Add<A20140214_首次充20送11>(entity);
        }

        /// <summary>
        /// 查询是否有充值数据
        /// </summary>
        public A20140214_首次充20送11 QueryA20140214_首次充20送11(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20140214_首次充20送11>().FirstOrDefault(p => p.UserId == userId);
        }

        /// <summary>
        /// 添加重庆时时彩中奖加奖活动
        /// </summary>
        public void AddA20140214_SSC红包(A20140214_SSC红包 entity)
        {
            this.Add<A20140214_SSC红包>(entity);
        }
    }
}
