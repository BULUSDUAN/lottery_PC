using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using External.Domain.Entities.Authentication;
using External.Domain.Managers.Authentication;
using GameBiz.Auth.Business;
using External.Core.Login;
using GameBiz.Domain.Managers;
using External.Domain.Managers;
using External.Domain.Entities.Task;
using GameBiz.Business;
using External.Core;
using External.Core.Authentication;

namespace External.Business
{
    public class EmailAuthenticationBusiness
    {
        public UserEmail GetAuthenticatedEmail(string userId)
        {
            using (var manager = new BettingPointManager())
            {
                var Email = manager.GetUserEmail(userId);
                return Email;
            }
        }
        public bool IsEmailAuthenticated(string email)
        {
            using (var manager = new BettingPointManager())
            {
                var other = manager.GetOtherUserEmail(email, "");
                return (other != null);
            }
        }
        public void RequestAuthenticationEmail(string userId, string email, int delaySeconds, string delayDescription, int maxRequestTime, string createBy)
        {
            #region
            //using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            //{
            //    biz.BeginTran();
            //    using (var manager = new BettingPointManager())
            //    {
            //        var other = manager.GetOtherUserEmail(email, userId);
            //        if (other != null && other.IsSettedEmail)
            //        {
            //            throw new ArgumentException(string.Format("此邮箱【{0}】已被其他用户认证。", email));
            //        }
            //        var entity = manager.GetUserEmail(userId);
            //        if (entity != null)
            //        {
            //            if (entity.IsSettedEmail)
            //            {
            //                throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过邮箱认证。", entity.UpdateTime));
            //            }
            //            if (entity.RequestTimes >= maxRequestTime)
            //            {
            //                throw new ArgumentException(string.Format("已请求最大限制次数【{0}】次还未成功认证，请联系客服。", maxRequestTime));
            //            }
            //            if (email != entity.Email)
            //            {
            //                var span = entity.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
            //                if (span.TotalSeconds > 0)
            //                {
            //                    throw new ArgumentException(string.Format("换邮箱再次认证必须在【{0}】后进行。", delayDescription));
            //                }
            //            }
            //            entity.UpdateBy = createBy;
            //            entity.RequestTimes++;
            //            entity.Email = email;

            //            manager.UpdateUserEmail(entity);
            //        }
            //        else
            //        {
            //            entity = new UserEmail
            //            {
            //                UserId = userId,
            //                User = manager.LoadUser(userId),
            //                AuthFrom = "LOCAL",
            //                Email = email,
            //                IsSettedEmail = false,
            //                CreateBy = createBy,
            //                UpdateBy = createBy,
            //            };
            //            manager.AddUserEmail(entity);
            //        }
            //    }
            //    biz.CommitTran();
            //} 
            #endregion

            using (var manager = new BettingPointManager())
            {
                var entity = manager.GetEmailInfoByEmail(email);
                if (entity != null && entity.IsSettedEmail)
                {
                    throw new ArgumentException(string.Format("此邮箱【{0}】已被其他用户认证。", email));
                }
                //var entity = manager.GetUserEmail(userId);
                if (entity != null)
                {
                    if (entity.IsSettedEmail)
                    {
                        throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过邮箱认证。", entity.UpdateTime));
                    }
                    if (entity.RequestTimes >= maxRequestTime)
                    {
                        throw new ArgumentException(string.Format("已请求最大限制次数【{0}】次还未成功认证，请联系客服。", maxRequestTime));
                    }
                    if (email != entity.Email)
                    {
                        var span = entity.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
                        if (span.TotalSeconds > 0)
                        {
                            throw new ArgumentException(string.Format("换邮箱再次认证必须在【{0}】后进行。", delayDescription));
                        }
                    }
                    entity.UpdateBy = createBy;
                    entity.RequestTimes++;
                    entity.Email = email;

                    manager.UpdateUserEmail(entity);
                }
                else
                {
                    entity = new UserEmail
                    {
                        UserId = userId,
                        User = manager.LoadUser(userId),
                        AuthFrom = "LOCAL",
                        Email = email,
                        IsSettedEmail = false,
                        CreateBy = createBy,
                        UpdateBy = createBy,
                    };
                    manager.AddUserEmail(entity);
                }
            }
        }
        public LoginInfo ResponseAuthenticationEmail(string userId, int delaySeconds, string delayDescription, out string mobile)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new BettingPointManager())
                {
                    var entity = manager.GetUserEmail(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("尚未请求邮箱认证");
                    }
                    if (entity.IsSettedEmail)
                    {
                        throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过邮箱认证。", entity.UpdateTime));
                    }
                    var span = DateTime.Now - entity.UpdateTime.AddSeconds(delaySeconds);
                    if (span.TotalSeconds > 0)
                    {
                        throw new ArgumentException(string.Format("提交认证邮箱必须在请求认证后【{0}】内完成。", delayDescription));
                    }
                    entity.IsSettedEmail = true;
                    manager.UpdateUserEmail(entity);
                    mobile = entity.Email;

                    //查询vip等级
                    var balanceManager = new UserBalanceManager();
                    var userReg = balanceManager.QueryUserRegister(userId);
                    var gv = new TaskListManager();
                    var old = gv.QueryTaskListByCategory(userId, TaskCategory.EmailBinding);
                    if (old == null)
                    {
                        //增加成长值 
                        var orderId = Guid.NewGuid().ToString("N");
                        BusinessHelper.Payin_UserGrowth("邮箱绑定", orderId, userId, 50, "完成邮箱绑定可获得50点成长值");
                        gv.AddUserTaskRecord(new UserTaskRecord
                        {
                            CreateTime = DateTime.Now,
                            CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                            OrderId = orderId,
                            TaskCategory = TaskCategory.EmailBinding,
                            TaskName = "邮箱绑定",
                            UserId = userId,
                        });
                        gv.AddTaskList(new TaskList
                        {
                            UserId = userId,
                            OrderId = orderId,
                            Content = "完成邮箱绑定可获得50点成长值",
                            ValueGrowth = 50,
                            IsGive = true,
                            VipLevel = 0,
                            CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                            TaskCategory = TaskCategory.EmailBinding,
                            TaskName = "邮箱绑定",
                            CreateTime = DateTime.Now,
                        });
                    }
                }
                biz.CommitTran();
            }
            var authBiz = new GameBizAuthBusiness();
            var userToken = authBiz.GetUserToken(userId);

            var userManager = new UserBalanceManager();
            var user = userManager.GetUserRegister(userId);
            return new LoginInfo
            {
                DisplayName = user.DisplayName,
                LoginFrom = user.ComeFrom,
                UserId = userId,
                Message = "登录成功",
                Referrer = user.Referrer,
                RegType = user.RegType,
                IsSuccess = true,
                UserToken = userToken,
            };
        }
        public void AddAuthenticationEmail(string authFrom, string userId, string email, string createBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new BettingPointManager())
                {
                    var entity = manager.GetUserEmail(userId);
                    if (entity != null)
                    {
                        throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过邮箱认证", entity.CreateTime));
                    }
                    entity = new UserEmail
                    {
                        UserId = userId,
                        User = manager.LoadUser(userId),
                        AuthFrom = authFrom,
                        Email = email,
                        IsSettedEmail = true,
                        CreateBy = createBy,
                        UpdateBy = createBy,
                    };
                    manager.AddUserEmail(entity);

                    var gv = new TaskListManager();
                    //增加成长值 
                    var orderId = Guid.NewGuid().ToString("N");
                    BusinessHelper.Payin_UserGrowth("绑定邮箱", orderId, userId, 50, "完成邮箱绑定可获得50点成长值");
                    gv.AddUserTaskRecord(new UserTaskRecord
                    {
                        CreateTime = DateTime.Now,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        OrderId = orderId,
                        TaskCategory = TaskCategory.EmailBinding,
                        TaskName = "绑定邮箱",
                        UserId = userId,
                    });
                    gv.AddTaskList(new TaskList
                    {
                        UserId = userId,
                        OrderId = orderId,
                        Content = "完成邮箱绑定可获得50点成长值",
                        ValueGrowth = 50,
                        IsGive = true,
                        VipLevel = 0,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = TaskCategory.EmailBinding,
                        TaskName = "绑定邮箱",
                        CreateTime = DateTime.Now,
                    });
                }
                biz.CommitTran();
            }
        }
        public void UpdateAuthenticationEmail(string authFrom, string userId, string email, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new BettingPointManager())
                {
                    var entity = manager.GetUserEmail(userId);
                    var emailInfo = manager.GetEmailInfoByEmail(email);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过实名认证");
                    }
                    if (emailInfo != null && emailInfo.UserId != userId)
                    {
                        if (!string.IsNullOrEmpty(emailInfo.Email) && emailInfo.IsSettedEmail)
                            throw new Exception("当前邮箱已被他人占用！");
                    }
                    entity.AuthFrom = authFrom;
                    entity.Email = email;
                    entity.UpdateBy = updateBy;
                    entity.IsSettedEmail = true;

                    manager.UpdateUserEmail(entity);
                }
                biz.CommitTran();
            }
        }
        public void CancelAuthenticationEmail(string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new BettingPointManager())
                {
                    var entity = manager.GetUserEmail(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过实名认证");
                    }
                    entity.IsSettedEmail = false;

                    manager.UpdateUserEmail(entity);
                }
                biz.CommitTran();
            }
        }
        public void ManualSetUserEmail(string userId, string email, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new BettingPointManager())
                {
                    var entity = manager.GetUserEmail(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过邮箱认证");
                    }
                    entity.IsSettedEmail = true;
                    entity.Email = email;
                    entity.UpdateBy = updateBy;
                    entity.UpdateTime = DateTime.Now;
                    manager.UpdateUserEmail(entity);
                }
                biz.CommitTran();
            }
        }
        public UserEmailInfo QueryEmailByUserId(string userId)
        {
            using (var manager = new BettingPointManager())
            {
                UserEmailInfo info = new UserEmailInfo();
                var entity = manager.GetUserEmail(userId);
                if (entity == null)
                    return info;
                info.AuthFrom = entity.AuthFrom;
                info.CreateBy = entity.CreateBy;
                info.CreateTime = entity.CreateTime;
                info.Email = entity.Email;
                info.IsSettedEmail = entity.IsSettedEmail;
                info.RequestTimes = entity.RequestTimes;
                info.UserId = entity.UserId;
                return info;
            }
        }

    }
}
