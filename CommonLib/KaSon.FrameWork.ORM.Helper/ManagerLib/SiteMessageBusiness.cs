using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
     public class SiteMessageBusiness
    {
        public UserIdeaInfo_QueryCollection QueryMyUserIdeaList(string createUserId, int pageIndex, int pageSize)
        {
            var result = new UserIdeaInfo_QueryCollection();
            var manager = new UserIdeaManager();
            var totalCount = 0;
            result.UserIdeaList = manager.QueryMyUserIdeaList(createUserId, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }
    }
}
