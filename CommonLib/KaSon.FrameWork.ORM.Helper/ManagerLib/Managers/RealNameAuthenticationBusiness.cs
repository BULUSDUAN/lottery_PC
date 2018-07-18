using EntityModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class RealNameAuthenticationBusiness:DBbase
    {
        public E_Authentication_RealName GetAuthenticatedRealName(string userId)
        {           
                var realName =DB.CreateQuery<E_Authentication_RealName>().Where(p => p.UserId == userId).FirstOrDefault(); ;
                return realName;
           
        }
        public void AddAuthenticationRealName(string authFrom, string userId, string realName, string cardType, string idCardNumber, string createBy, bool checkRepet)
        {
            var manager = new UserRealNameManager();

            if (checkRepet)
            {
                var other = manager.QueryUserRealName(idCardNumber);
                if (other != null)
                    throw new ArgumentException(string.Format("此证件号【{0}】已被其他用户认证。", idCardNumber));
                //var other2 = manager.QueryUserRealNameByName(realName);
                //if (other2 != null)
                //    throw new ArgumentException("对不起，由于系统检测到您的姓名已被绑定，请联系在线客服为您人工绑定，给您带来的不便敬请谅解，此绑定不影响您的正常购彩和提现。");
            }

            var entity = GetAuthenticatedRealName(userId);
            if (entity != null)
            {
                entity.RealName = realName;
                entity.IdCardNumber = idCardNumber;
                entity.IsSettedRealName = true;
                manager.UpdateUserRealName(entity);
                //throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过实名认证", entity.CreateTime));
            }
            else
            {
                if (entity == null)
                {
                    entity = new E_Authentication_RealName
                    {
                        UserId = userId,
                        AuthFrom = authFrom,
                        RealName = realName,
                        CardType = cardType,
                        IdCardNumber = idCardNumber,
                        IsSettedRealName = true,
                        CreateBy = createBy,
                        UpdateBy = createBy,
                    };
                    manager.AddUserRealName(entity);
                }
                else
                    throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过实名认证", entity.CreateTime));
            }

            //修改vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            user.VipLevel = 1;
            balanceManager.UpdateUserRegister(user);

            var gv = new TaskListManager(); 
             var old = gv.QueryTaskListByCategory(userId, TaskCategory.RealName);
            if (old == null)
            {
                var orderId = Guid.NewGuid().ToString("N");
                //增加成长值 
                //BusinessHelper.Payin_UserGrowth("实名认证", orderId, userId, 200, "完成实名认证获得200点成长值");
                var UserTaskRecord=new E_UserTaskRecord
                {
                    OrderId = orderId,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    CreateTime = DateTime.Now,
                    TaskName = "实名认证",
                    TaskCategory = (int)TaskCategory.RealName,
                    UserId = userId,
                };
                gv.AddUserTaskRecord(UserTaskRecord);
                //赠送成长值记录
                var addTaskList=new E_TaskList
                {
                    UserId = userId,
                    OrderId = Guid.NewGuid().ToString("N"),
                    Content = "完成实名认证获得200点成长值",
                    ValueGrowth = 200,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.RealName,
                    VipLevel = 0,
                    IsGive = true,
                    TaskName = "实名认证",
                    CreateTime = DateTime.Now,
                };
                gv.AddTaskList(addTaskList);
              
            }


        }


        public void UpdateAuthenticationRealName(string authFrom, string userId, string realName, string cardType, string idCardNumber, string updateBy)
        {

            using (DB) {
                DB.Begin();

                var manager = new UserRealNameManager();
                var entity = GetAuthenticatedRealName(userId);
                var realNameInfo = manager.GetRealNameInfoByName(realName, idCardNumber);
                if (entity == null)
                {
                    DB.Rollback();
                    throw new ArgumentException("此用户从未进行过实名认证");

                }
                entity.AuthFrom = authFrom;
                entity.RealName = realName;
                entity.CardType = cardType;
                entity.IdCardNumber = idCardNumber;
                entity.UpdateBy = updateBy;
                entity.IsSettedRealName = true;
                manager.UpdateUserRealName(entity);
                DB.Commit();
            } 
               

        }
    }
}
