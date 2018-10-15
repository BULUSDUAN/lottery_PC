using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
   public class UserIdeaManager:DBbase
    {
        public List<UserIdeaInfo_Query> QueryMyUserIdeaList(string createUserId, int pageIndex, int pageSize, out int totalCount)
        {
         
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = (from i in DB.CreateQuery<E_SiteMessage_UserIdea_List>()
                        where i.CreateUserId == createUserId
                        orderby i.UpdateTime descending
                        select i).ToList().Select(i=> new UserIdeaInfo_Query
                        {
                            Category = string.IsNullOrEmpty(i.Category) ? string.Empty : i.Category,
                            CreateTime = i.CreateTime,
                            CreateUserDisplayName = i.CreateUserDisplayName,
                            CreateUserId = i.CreateUserId,
                            CreateUserMoibile = string.IsNullOrEmpty(i.CreateUserMoibile) ? string.Empty : i.CreateUserMoibile,
                            Description = string.IsNullOrEmpty(i.Description) ? string.Empty : i.Description,
                            Id = i.Id,
                            IsAnonymous = i.IsAnonymous,
                            Status = i.Status,
                            UpdateTime = i.UpdateTime,
                            UpdateUserDisplayName = string.IsNullOrEmpty(i.UpdateUserDisplayName) ? string.Empty : i.UpdateUserDisplayName,
                            UpdateUserId = string.IsNullOrEmpty(i.UpdateUserId) ? string.Empty : i.UpdateUserId,
                            ManageReply = string.IsNullOrEmpty(i.ManageReply) ? string.Empty : i.ManageReply,
                            PageOpenSpeed = i.PageOpenSpeed == 0 ? 0M : decimal.Parse(i.PageOpenSpeed.ToString()),
                            InterfaceBeautiful = i.InterfaceBeautiful == 0 ? 0M : decimal.Parse(i.InterfaceBeautiful.ToString()),
                            ComposingReasonable = i.ComposingReasonable == 0 ? 0M : decimal.Parse(i.ComposingReasonable.ToString()),
                            OperationReasonable = i.OperationReasonable == 0 ? 0M : decimal.Parse(i.OperationReasonable.ToString()),
                            ContentConveyDistinct = i.ContentConveyDistinct == 0 ? 0M : decimal.Parse(i.ContentConveyDistinct.ToString()),
                        });
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
