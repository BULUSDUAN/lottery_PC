using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    public class BDFXOrderBusiness
    {
        public void AddUserSchemeShareExpert(string userId, int shortIndex, CopyOrderSource source)
        {
            var manager = new BDFXManager();
            var entity = manager.QueryUserSchemeShareExpertByUserId(userId, source);
            if (entity == null)
            {
                entity = new E_User_SchemeShareExpert
                {
                    ExpertType = (int)source,
                    ShowSort = shortIndex,
                    IsEnable = true,
                    CreateTime = DateTime.Now,
                    UserId = userId
                };
                manager.AddUserSchemeShareExpert(entity);
            }
            else
            {
                entity.ExpertType = (int)source;
                entity.ShowSort = shortIndex;
                entity.UserId = userId;
                manager.UpdateUserSchemeShareExpert(entity);
            }

        }
        public void DeleteUserSchemeShareExpert(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("用户编号不能为空");
            var manager = new BDFXManager();
                manager.DeleteUserSchemeShareExpert(id);
        }
        public UserSchemeShareExpert_Collection QueryUserSchemeShareExpertList(string userKey, int source, int pageIndex, int pageSize)
        {
            var manager = new BDFXManager();
                return manager.QueryUserSchemeShareExpertList(userKey, source, pageIndex, pageSize);
        }
    }
}
