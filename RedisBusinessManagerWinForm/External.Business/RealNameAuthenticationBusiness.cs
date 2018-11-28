using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using External.Domain.Managers.Authentication;
using External.Domain.Entities.Authentication;
using System.Collections;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using External.Domain.Managers;
using External.Domain.Entities.Task;
using External.Core;
using External.Core.Authentication;

namespace External.Business
{
    /// <summary>
    /// 实名认证
    /// </summary>
    public class RealNameAuthenticationBusiness
    {
        public UserRealName GetAuthenticatedRealName(string userId)
        {
            using (var manager = new UserRealNameManager())
            {
                var realName = manager.GetUserRealName(userId);
                return realName;
            }
        }
        public bool IsRealNameAuthenticated(string cardType, string cardNumber)
        {
            using (var manager = new UserRealNameManager())
            {
                var other = manager.GetOtherUserCard(cardType, cardNumber, "");
                return (other != null);
            }
        }
        public void AddAuthenticationRealName(string authFrom, string userId, string realName, string cardType, string idCardNumber, string createBy, bool checkRepet)
        {
            using (var manager = new UserRealNameManager())
            {
                if (checkRepet)
                {
                    var other = manager.QueryUserRealName(idCardNumber);
                    if (other != null)
                        throw new ArgumentException(string.Format("此证件号【{0}】已被其他用户认证。", idCardNumber));
                    //var other2 = manager.QueryUserRealNameByName(realName);
                    //if (other2 != null)
                    //    throw new ArgumentException("对不起，由于系统检测到您的姓名已被绑定，请联系在线客服为您人工绑定，给您带来的不便敬请谅解，此绑定不影响您的正常购彩和提现。");
                }

                var entity = manager.GetUserRealName(userId);
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
                        entity = new UserRealName
                        {
                            UserId = userId,
                            User = manager.LoadUser(userId),
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
                    BusinessHelper.Payin_UserGrowth("实名认证", orderId, userId, 200, "完成实名认证获得200点成长值");
                    gv.AddUserTaskRecord(new UserTaskRecord
                    {
                        OrderId = orderId,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        CreateTime = DateTime.Now,
                        TaskName = "实名认证",
                        TaskCategory = TaskCategory.RealName,
                        UserId = userId,
                    });
                    //赠送成长值记录
                    gv.AddTaskList(new TaskList
                    {
                        UserId = userId,
                        OrderId = Guid.NewGuid().ToString("N"),
                        Content = "完成实名认证获得200点成长值",
                        ValueGrowth = 200,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = TaskCategory.RealName,
                        VipLevel = 0,
                        IsGive = true,
                        TaskName = "实名认证",
                        CreateTime = DateTime.Now,
                    });
                }
            }

        }
        //首页活动  绑定实名
        public void AddAuthenticationRealName_Index(string authFrom, string userId, string realName, string cardType, string idCardNumber, string createBy)
        {
            #region
            //using (var manager = new UserRealNameManager())
            //{
            //    var other = manager.GetOtherUserCard(cardType, idCardNumber, userId);
            //    if (other != null && other.IsSettedRealName)//判断证件号的唯一性
            //    {
            //        throw new ArgumentException(string.Format("此证件号【{0}】已被其他用户认证。", idCardNumber));
            //    }
            //    var entity = manager.GetUserRealName(userId);
            //    if (entity != null && !entity.IsSettedRealName && !string.IsNullOrEmpty(entity.IdCardNumber))//判断真实姓名的唯一性
            //    {
            //        entity.RealName = realName;
            //        entity.IdCardNumber = idCardNumber;
            //        entity.IsSettedRealName = true;
            //        manager.UpdateUserRealName(entity);
            //        //throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过实名认证", entity.CreateTime));
            //    }
            //    else
            //    {
            //        if (entity == null)
            //        {
            //            entity = new UserRealName
            //            {
            //                UserId = userId,
            //                User = manager.LoadUser(userId),
            //                AuthFrom = authFrom,
            //                RealName = realName,
            //                CardType = cardType,
            //                IdCardNumber = idCardNumber,
            //                IsSettedRealName = true,
            //                CreateBy = createBy,
            //                UpdateBy = createBy,
            //            };
            //            manager.AddUserRealName(entity);
            //        }
            //        else
            //            throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过实名认证", entity.CreateTime));
            //    }

            //    //修改vip等级
            //    var balanceManager = new UserBalanceManager();
            //    var user = balanceManager.QueryUserRegister(userId);
            //    user.VipLevel = 1;
            //    balanceManager.UpdateUserRegister(user);

            //    var gv = new TaskListManager();
            //    var old = gv.QueryTaskListByCategory(userId, TaskCategory.RealName);
            //    if (old == null)
            //    {
            //        var orderId = Guid.NewGuid().ToString("N");
            //        //增加成长值 
            //        BusinessHelper.Payin_UserGrowth("实名认证", orderId, userId, 200, "完成实名认证获得200点成长值");
            //        gv.AddUserTaskRecord(new UserTaskRecord
            //        {
            //            OrderId = orderId,
            //            CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
            //            CreateTime = DateTime.Now,
            //            TaskName = "实名认证",
            //            TaskCategory = TaskCategory.RealName,
            //            UserId = userId,
            //        });
            //        //赠送成长值记录
            //        gv.AddTaskList(new TaskList
            //        {
            //            UserId = userId,
            //            OrderId = Guid.NewGuid().ToString("N"),
            //            Content = "完成实名认证获得200点成长值",
            //            ValueGrowth = 200,
            //            CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
            //            TaskCategory = TaskCategory.RealName,
            //            VipLevel = 0,
            //            IsGive = true,
            //            TaskName = "实名认证",
            //            CreateTime = DateTime.Now,
            //        });
            //    }
            //} 
            #endregion

            //注意:一个身份证号码可以被绑定多次
            using (var manager = new UserRealNameManager())
            {
                //var entity = manager.GetRealNameInfoByName(realName, idCardNumber);
                //if (entity != null && entity.IsSettedRealName)//判断证件号的唯一性
                //{
                //    throw new ArgumentException(string.Format("此证件号【{0}】已被其他用户认证。", idCardNumber));
                //}
                var entity = manager.GetUserRealName(userId);
                //if (entity != null && !entity.IsSettedRealName && !string.IsNullOrEmpty(entity.IdCardNumber))//判断真实姓名的唯一性
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
                        entity = new UserRealName
                        {
                            UserId = userId,
                            User = manager.LoadUser(userId),
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
                    BusinessHelper.Payin_UserGrowth("实名认证", orderId, userId, 200, "完成实名认证获得200点成长值");
                    gv.AddUserTaskRecord(new UserTaskRecord
                    {
                        OrderId = orderId,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        CreateTime = DateTime.Now,
                        TaskName = "实名认证",
                        TaskCategory = TaskCategory.RealName,
                        UserId = userId,
                    });
                    //赠送成长值记录
                    gv.AddTaskList(new TaskList
                    {
                        UserId = userId,
                        OrderId = Guid.NewGuid().ToString("N"),
                        Content = "完成实名认证获得200点成长值",
                        ValueGrowth = 200,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = TaskCategory.RealName,
                        VipLevel = 0,
                        IsGive = true,
                        TaskName = "实名认证",
                        CreateTime = DateTime.Now,
                    });
                }
            }
        }
        public void UpdateAuthenticationRealName(string authFrom, string userId, string realName, string cardType, string idCardNumber, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserRealNameManager())
                {
                    var entity = manager.GetUserRealName(userId);
                    var realNameInfo = manager.GetRealNameInfoByName(realName, idCardNumber);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过实名认证");
                    }
                    //if (realNameInfo != null && realNameInfo.UserId != userId)
                    //{
                    //    if (!string.IsNullOrEmpty(realNameInfo.RealName) && realNameInfo.IsSettedRealName)
                    //        throw new Exception("当前信息已被他人占用！");
                    //}
                    entity.AuthFrom = authFrom;
                    entity.RealName = realName;
                    entity.CardType = cardType;
                    entity.IdCardNumber = idCardNumber;
                    entity.UpdateBy = updateBy;
                    entity.IsSettedRealName = true;

                    manager.UpdateUserRealName(entity);
                }
                biz.CommitTran();
            }
        }
        public void CancelAuthenticationRealName(string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserRealNameManager())
                {
                    var entity = manager.GetUserRealName(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过实名认证");
                    }
                    entity.IsSettedRealName = false;

                    manager.UpdateUserRealName(entity);
                }
                biz.CommitTran();
            }
        }
        /// <summary>
        /// 更新实名认证
        /// </summary>
        public void UpdateRealNameAuthentication(string userId, string realName, string idCard, string updateBy)
        {
            using (var manager = new UserRealNameManager())
            {
                var other = manager.QueryUserRealName(idCard);
                if (other != null)
                    throw new ArgumentException(string.Format("此证件号【{0}】已被其他用户认证。", idCard));
            }
           
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 注销实名认证
        /// </summary>
        public void LogOffRealNameAuthen(string userId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserRealNameManager();
                var entity = manager.GetUserRealName(userId);
                if (entity == null)
                    throw new ArgumentException("此用户从未进行过实名认证");
                manager.DeleteUserRealName(entity);

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 交换实名认证
        /// </summary>
        public void SwapRealNameAuthen(string fromUserId, string toUserId, string updateBy)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserRealNameManager();
                var fromEntity = manager.GetUserRealName(fromUserId);
                if (fromEntity == null)
                    throw new ArgumentException(fromUserId + "从未进行过实名认证");
                fromEntity.IsSettedRealName = false;
                fromEntity.UpdateBy = updateBy;
                fromEntity.UpdateTime = DateTime.Now;
                manager.UpdateUserRealName(fromEntity);

                var toEntity = manager.GetUserRealName(toUserId);
                if (toEntity != null)
                {
                    toEntity.RealName = fromEntity.RealName;
                    toEntity.IdCardNumber = fromEntity.IdCardNumber;
                    toEntity.IsSettedRealName = true;
                    toEntity.UpdateBy = updateBy;
                    toEntity.UpdateTime = DateTime.Now;
                }
                else
                {
                    var toUser = manager.LoadUser(toUserId);
                    if (toUser == null)
                        throw new ArgumentException(toUserId + "不存在");

                    manager.AddUserRealName(new UserRealName
                    {
                        UserId = toUserId,
                        User = toUser,
                        AuthFrom = fromEntity.AuthFrom,
                        RealName = fromEntity.RealName,
                        CardType = fromEntity.CardType,
                        IdCardNumber = fromEntity.IdCardNumber,
                        IsSettedRealName = true,
                        CreateBy = updateBy,
                        UpdateBy = updateBy,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                    });
                }

                biz.CommitTran();
            }
        }

        public UserRealName_Collection QueryUserRealNameCollection()
        {
            using (var manager = new UserRealNameManager())
            {
                return manager.QueryUserRealNameCollection();
            }
        }
        public UserRealNameInfo QueryRealNameByUserId(string userId)
        {
            using (var manager = new UserRealNameManager())
            {
                UserRealNameInfo info = new UserRealNameInfo();
                var realName = manager.GetUserRealName(userId);
                if (realName == null)
                    return info;
                info.AuthFrom = realName.AuthFrom;
                info.CardType = realName.CardType;
                info.CreateBy = realName.CreateBy;
                info.CreateTime = realName.CreateTime;
                info.IdCardNumber = realName.IdCardNumber;
                info.IsSettedRealName = realName.IsSettedRealName;
                info.RealName = realName.RealName;
                info.UserId = realName.UserId;
                return info;
            }
        }

    }
}
