using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Activity.Domain.Entities;
using NHibernate.Linq;
using Activity.Core;
using GameBiz.Domain.Entities;
using Common.Utilities;

namespace Activity.Managers
{
    public class A20130114Manager : GameBizEntityManagement
    {
        public void AddA20130114_加奖统计(params A20130114_加奖统计[] array)
        {
            this.Add<A20130114_加奖统计>(array);
        }

        public A20130114_加奖统计 QueryA20130114_加奖统计(string schemeId)
        {
            Session.Clear();
            return this.Session.Query<A20130114_加奖统计>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public List<AddMoneyQueryInfo> QueryNewAddMoneyList(int count)
        {
            Session.Clear();
            var query = from a in this.Session.Query<A20130114_加奖统计>()
                        join u in this.Session.Query<UserRegister>() on a.UserId equals u.UserId
                        orderby a.CreateTime descending
                        select new AddMoneyQueryInfo
                        {
                            AddMoney = a.AddMoney,
                            BonusMoney = a.BonusMoney,
                            GameCode = a.GameCode,
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            GainMoney = 0M,
                        };
            return query.Take(count).ToList();
        }
        public List<AddMoneyQueryInfo> QueryTotalAddMoneyList(int count)
        {
            Session.Clear();
            var sql = string.Format(@"select top {0} a.GameCode,a.UserId,u.DisplayName,u.HideDisplayNameCount, a.BonusMoney,a.AddMoney,b.GainMoney 
                                    from (SELECT GameCode,UserId,SUM(BonusMoney)BonusMoney,sum(AddMoney)AddMoney
                                      FROM  [E_A20130114_加奖统计]
                                      group by GameCode,UserId
                                      ) AS a
                                      inner join C_User_Register u on a.UserId=u.UserId
                                      left join (select GameCode,UserId,sum(TogetherSchemeSuccessGainMoney) GainMoney
                                      from C_User_Beedings 
                                      group by GameCode,UserId)AS b on a.GameCode =b.GameCode and a.UserId=b.UserId

                                      order by AddMoney desc", count);
            var list = new List<AddMoneyQueryInfo>();
            foreach (var item in this.Session.CreateSQLQuery(sql).List())
            {
                var array = item as object[];
                list.Add(new AddMoneyQueryInfo
                {
                    GameCode = UsefullHelper.GetDbValue<string>(array[0]),
                    UserId = UsefullHelper.GetDbValue<string>(array[1]),
                    UserDisplayName = UsefullHelper.GetDbValue<string>(array[2]),
                    HideDisplayNameCount = UsefullHelper.GetDbValue<int>(array[3]),
                    BonusMoney = UsefullHelper.GetDbValue<decimal>(array[4]),
                    AddMoney = UsefullHelper.GetDbValue<decimal>(array[5]),
                    GainMoney = UsefullHelper.GetDbValue<decimal>(array[6]),
                });
            }
            return list;
        }
    }
}
