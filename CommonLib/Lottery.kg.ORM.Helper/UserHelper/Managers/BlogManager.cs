using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
   public class BlogManager:DBbase
    {
        /// <summary>
        /// 初始化用户获奖记录
        /// </summary>
        /// <param name="BlogProfileBonusLevel"></param>
        public void AddBlog_ProfileBonusLevel(E_Blog_ProfileBonusLevel BlogProfileBonusLevel)
        {

            DB.GetDal<E_Blog_ProfileBonusLevel>().Add(BlogProfileBonusLevel);
        }

        /// <summary>
        /// 用户数据统计
        /// </summary>
        /// <param name="BlogProfileBonusLevel"></param>
        public void AddBlogDataReport(E_Blog_DataReport BlogDataReport)
        {

            DB.GetDal<E_Blog_DataReport>().Add(BlogDataReport);
        }
        
    }
}
