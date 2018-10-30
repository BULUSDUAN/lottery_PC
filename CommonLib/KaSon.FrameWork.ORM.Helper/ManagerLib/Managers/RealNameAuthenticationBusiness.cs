using EntityModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
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
                    throw new LogicException(string.Format("此证件号【{0}】已被其他用户认证。", idCardNumber));
            }
            var entity = GetAuthenticatedRealName(userId);
            if (entity != null)
            {
                entity.RealName = realName;
                entity.IdCardNumber = idCardNumber;
                entity.IsSettedRealName = true;
                entity.UpdateTime = DateTime.Now;
                manager.UpdateUserRealName(entity);
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
                        CreateTime=DateTime.Now
                    };
                    manager.AddUserRealName(entity);
                }
                else
                    throw new LogicException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过实名认证", entity.CreateTime));
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
            var manager = new UserRealNameManager();
            var entity = GetAuthenticatedRealName(userId);
            if (entity == null)
            {
                DB.Rollback();
                throw new LogicException("此用户从未进行过实名认证");
            }
            entity.AuthFrom = authFrom;
            entity.RealName = realName;
            entity.CardType = cardType;
            entity.IdCardNumber = idCardNumber;
            entity.UpdateBy = updateBy;
            entity.IsSettedRealName = true;
            manager.UpdateUserRealName(entity);
        }
        /// <summary>
        /// 更新实名认证
        /// </summary>
        public void UpdateRealNameAuthentication(string userId, string realName, string idCard, string updateBy)
        {
            var other = new UserRealNameManager().QueryUserRealName(idCard);
            if (other != null)
                throw new ArgumentException(string.Format("此证件号【{0}】已被其他用户认证。", idCard));
            //开启事务
            DB.Begin();
            var manager = new UserRealNameManager();
            var entity = manager.GetUserRealName(userId);
            var realNameInfo = manager.GetRealNameInfoByName(realName, idCard);
            if (entity == null)
                throw new ArgumentException("此用户从未进行过实名认证");
            entity.RealName = realName;
            entity.IsSettedRealName = true;
            entity.IdCardNumber = idCard;
            entity.UpdateBy = updateBy;
            entity.UpdateTime = DateTime.Now;
            manager.UpdateUserRealName(entity);
            DB.Commit();
        }
        /// <summary>
        /// 注销实名认证
        /// </summary>
        public void LogOffRealNameAuthen(string userId)
        {
            //开启事务
            DB.Begin();
            var manager = new UserRealNameManager();
            var entity = manager.GetUserRealName(userId);
            if (entity == null)
                throw new ArgumentException("此用户从未进行过实名认证");
            manager.DeleteUserRealName(entity);
            DB.Commit();
        }
    }
}
