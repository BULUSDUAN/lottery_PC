using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SiteActivityManager : DBbase
    {
        /// <summary>
        /// 添加活动
        /// </summary>
        public void AddSiteActivity(E_SiteActivity entity)
        {
            DB.GetDal<E_SiteActivity>().Add(entity);
        }

        public void AddLotteryNewBonus(E_LotteryNewBonus entity)
        {
            DB.GetDal<E_LotteryNewBonus>().Add(entity);
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        public void UpdateSiteActivity(E_SiteActivity entity)
        {
            DB.GetDal<E_SiteActivity>().Update(entity);
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        public void DeleteSiteActivity(E_SiteActivity entity)
        {
            DB.GetDal<E_SiteActivity>().Delete(entity);
        }

        /// <summary>
        /// 查询所有活动配置
        /// </summary>
        public List<E_SiteActivity> QueryAllSiteActivity()
        {
           
            var query = from s in DB.CreateQuery<E_SiteActivity>()
                        select s;
            if (query != null)
            {
                return query.ToList();
            }
            return null;
        }


        /// <summary>
        /// 查询某一个活动配置
        /// </summary>
        public E_SiteActivity QuerySiteActivity(int id)
        {
         
            var query = from a in DB.CreateQuery<E_SiteActivity>()
                        where a.Id == id
                        select a;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 查询某一个活动配置
        /// </summary>
        public E_SiteActivity QuerySiteActivityInfo(int id)
        {
           
            var query = from a in DB.CreateQuery<E_SiteActivity>()
                        where a.Id == id
                        select a;
            return query.FirstOrDefault();
        }

        public List<E_LotteryNewBonus> QueryLotteryNewBonusInfoList(int count)
        {
           

            var query = from b in DB.CreateQuery<E_LotteryNewBonus>()
                        orderby b.CreateTime descending
                        select b;
            return query.Take(count).ToList();
        }


    }

}
