using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using External.Domain.Entities.Authentication;
using External.Domain.Managers.Authentication;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using Common.Cryptography;
using External.Domain.Managers;
using External.Domain.Entities.Task;
using External.Core;
using External.Core.Authentication;

namespace External.Business
{
    public class MobileAuthenticationBusiness
    {
        public UserMobile GetAuthenticatedMobile(string userId)
        {
            using (var manager = new UserMobileManager())
            {
                var Mobile = manager.GetUserMobile(userId);
                return Mobile;
            }
        }
        public bool IsMobileAuthenticated(string mobile)
        {
            using (var manager = new UserMobileManager())
            {
                var other = manager.GetOtherUserMobile(mobile, "");
                return (other != null);
            }
        }
        public void RequestAuthenticationMobile(string userId, string mobile, int delaySeconds, string delayDescription, int maxRequestTime, string createBy)
        {
            #region
            //using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            //{
            //    biz.BeginTran();
            //    using (var manager = new UserMobileManager())
            //    {
            //        var other = manager.GetOtherUserMobile(mobile, userId);
            //        if (other != null && other.IsSettedMobile)
            //        {
            //            throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
            //        }
            //        var entity = manager.GetUserMobile(userId);
            //        if (entity != null)
            //        {
            //            if (entity.IsSettedMobile && entity.IsSettedMobile)
            //            {
            //                throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
            //            }
            //            if (entity.RequestTimes >= maxRequestTime)
            //            {
            //                throw new ArgumentException(string.Format("已请求最大限制次数【{0}】次还未成功认证，请联系客服。", maxRequestTime));
            //            }
            //            if (mobile != entity.Mobile)
            //            {
            //                var span = entity.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
            //                if (span.TotalSeconds > 0)
            //                {
            //                    throw new ArgumentException(string.Format("换手机号码再次认证必须在【{0}】后进行。", delayDescription));
            //                }
            //            }
            //            entity.IsSettedMobile = true;
            //            entity.UpdateBy = createBy;
            //            entity.RequestTimes++;
            //            entity.Mobile = mobile;

            //            manager.UpdateUserMobile(entity);
            //        }
            //        else
            //        {
            //            entity = new UserMobile
            //            {
            //                UserId = userId,
            //                User = manager.LoadUser(userId),
            //                AuthFrom = "LOCAL",
            //                Mobile = mobile,
            //                IsSettedMobile = false,
            //                CreateBy = createBy,
            //                UpdateBy = createBy,
            //            };
            //            manager.AddUserMobile(entity);
            //        }
            //    }
            //    biz.CommitTran();
            //}
            #endregion

            using (var manager = new UserMobileManager())
            {
                var other = manager.GetMobileInfoByMobile(mobile);
                if (other != null && other.IsSettedMobile && other.UserId != userId)
                {
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                }
                //var entity = manager.GetUserMobile(userId);
                var entity = manager.GetUserMobile(userId);
                if (entity != null)
                {
                    if (!string.IsNullOrEmpty(entity.Mobile) && entity.IsSettedMobile)
                    {
                        throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
                    }
                    if (entity.RequestTimes >= maxRequestTime)
                    {
                        throw new ArgumentException(string.Format("已请求最大限制次数【{0}】次还未成功认证，请联系客服。", maxRequestTime));
                    }
                    if (mobile != entity.Mobile)
                    {
                        var span = entity.UpdateTime.AddSeconds(delaySeconds) - DateTime.Now;
                        if (span.TotalSeconds > 0)
                        {
                            throw new ArgumentException(string.Format("换手机号码再次认证必须在【{0}】后进行。", delayDescription));
                        }
                    }
                    entity.IsSettedMobile = true;
                    entity.UpdateBy = createBy;
                    entity.RequestTimes++;
                    entity.Mobile = mobile;

                    manager.UpdateUserMobile(entity);
                }
                else
                {
                    entity = new UserMobile
                    {
                        UserId = userId,
                        User = manager.LoadUser(userId),
                        AuthFrom = "LOCAL",
                        Mobile = mobile,
                        IsSettedMobile = false,
                        CreateBy = createBy,
                        UpdateBy = createBy,
                    };
                    manager.AddUserMobile(entity);
                }
            }
        }
        /// <summary>
        /// 手机认证，重发验证码或更换号码
        /// </summary>
        public void RepeatRequestMobile(string userId, string mobile, string createUserId)
        {
            using (var manager = new UserMobileManager())
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("未查询到用户编号");
                else if (string.IsNullOrEmpty(mobile))
                    throw new Exception("手机号码不能为空");
                //var entity = manager.GetUserMobile(userId);
                var other = manager.GetMobileInfoByMobile(mobile);
                if (other != null && other.IsSettedMobile && other.UserId != userId)
                {
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                }
                var entity = manager.GetUserMobile(userId);
                if (entity != null)
                {
                    entity.IsSettedMobile = false;
                    entity.UpdateBy = createUserId;
                    entity.RequestTimes++;
                    entity.Mobile = mobile;
                    manager.UpdateUserMobile(entity);
                }
                else
                {
                    entity = new UserMobile
                    {
                        UserId = userId,
                        User = manager.LoadUser(userId),
                        AuthFrom = "LOCAL",
                        Mobile = mobile,
                        IsSettedMobile = false,
                        CreateBy = createUserId,
                        UpdateBy = createUserId,
                    };
                    manager.AddUserMobile(entity);
                }
            }
        }
        // 根据手机号查询手机号是否存在
        public void RequestAuthenticationMobileIndex(string mobile)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserMobileManager())
                {
                    var other = manager.GetOtherUserMobileIndex(mobile);
                    if (other != null && other.IsSettedMobile)
                    {
                        throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                    }
                }
                biz.CommitTran();
            }
        }
        public string ResponseAuthenticationMobile(string userId, int delaySeconds, string delayDescription)
        {
            string mobile;
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var gv = new TaskListManager();
                var old = gv.QueryTaskListByCategory(userId, TaskCategory.MobilBinding);
                if (old == null)
                {
                    //增加成长值 
                    var orderId = Guid.NewGuid().ToString("N");
                    BusinessHelper.Payin_UserGrowth("绑定手机号", orderId, userId, 100, "完成手机号绑定可获得100点成长值");
                    gv.AddUserTaskRecord(new UserTaskRecord
                    {
                        CreateTime = DateTime.Now,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        OrderId = orderId,
                        TaskCategory = TaskCategory.MobilBinding,
                        TaskName = "绑定手机号",
                        UserId = userId,
                    });
                    gv.AddTaskList(new TaskList
                    {
                        UserId = userId,
                        OrderId = orderId,
                        Content = "完成手机号绑定可获得100点成长值",
                        ValueGrowth = 100,
                        IsGive = true,
                        VipLevel = 0,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = TaskCategory.MobilBinding,
                        TaskName = "绑定手机号",
                        CreateTime = DateTime.Now,
                    });
                }

                using (var manager = new UserMobileManager())
                {
                    var entity = manager.GetUserMobile(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("尚未请求手机认证");
                    }
                    if (entity.IsSettedMobile)
                    {
                        throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
                    }
                    var span = DateTime.Now - entity.UpdateTime.AddSeconds(delaySeconds);
                    if (span.TotalSeconds > 0)
                    {
                        throw new ArgumentException(string.Format("提交认证手机必须在请求认证后【{0}】内完成。", delayDescription));
                    }
                    entity.IsSettedMobile = true;

                    manager.UpdateUserMobile(entity);

                    mobile = entity.Mobile;
                }
                biz.CommitTran();
            }
            return mobile;
        }
        public void AddAuthenticationMobile(string authFrom, string userId, string mobile, string createBy)
        {
            #region

            //using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            //using (var manager = new UserMobileManager())
            //{
            //    var other = manager.GetOtherUserMobile(mobile, userId);
            //    if (other != null && other.IsSettedMobile)//判断手机号码的唯一性
            //    {
            //        throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
            //    }
            //    var entity = manager.GetUserMobile(userId);
            //    if (entity != null && !string.IsNullOrEmpty(entity.Mobile) && !entity.IsSettedMobile)
            //    {
            //        entity.Mobile = mobile;
            //        entity.IsSettedMobile = true;
            //        manager.UpdateUserMobile(entity);
            //        //throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证", entity.CreateTime));
            //    }
            //    else
            //    {
            //        if (entity == null)
            //        {
            //            entity = new UserMobile
            //            {
            //                UserId = userId,
            //                User = manager.LoadUser(userId),
            //                AuthFrom = authFrom,
            //                Mobile = mobile,
            //                IsSettedMobile = true,
            //                CreateBy = createBy,
            //                UpdateBy = createBy,
            //            };
            //            manager.AddUserMobile(entity);
            //        }
            //        else
            //            throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证", entity.CreateTime));
            //    }
            //}

            //    biz.CommitTran();
            //}
            #endregion

            using (var manager = new UserMobileManager())
            {
                var mobileInfo = manager.GetMobileInfoByMobile(mobile);
                if (mobileInfo != null && !string.IsNullOrEmpty(mobileInfo.Mobile) && mobileInfo.UserId != userId)
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                if (mobileInfo != null && !string.IsNullOrEmpty(mobileInfo.Mobile) && !mobileInfo.IsSettedMobile)
                {
                    //mobileInfo.UserId = userId;
                    mobileInfo.UpdateBy = userId;
                    mobileInfo.UpdateTime = DateTime.Now;
                    mobileInfo.Mobile = mobile;
                    mobileInfo.IsSettedMobile = true;
                    manager.UpdateUserMobile(mobileInfo);
                }
                else
                {
                    if (mobileInfo == null)
                    {
                        mobileInfo = new UserMobile
                        {
                            UserId = userId,
                            User = manager.LoadUser(userId),
                            AuthFrom = authFrom,
                            Mobile = mobile,
                            IsSettedMobile = true,
                            CreateBy = createBy,
                            UpdateBy = createBy,
                        };
                        manager.AddUserMobile(mobileInfo);
                    }
                    else
                        throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证", mobileInfo.CreateTime));
                }
            }
        }
        //首页活动  手机绑定
        public void AddAuthenticationMobile_Index(string authFrom, string userId, string mobile, string createBy)
        {
            using (var manager = new UserMobileManager())
            {
                //var other = manager.GetOtherUserMobile(mobile, userId);
                //if (other != null && other.IsSettedMobile)//判断手机号码的唯一性
                //{
                //    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                //}
                var mobileInfo = manager.GetMobileInfoByMobile(mobile);
                if (mobileInfo != null && !string.IsNullOrEmpty(mobileInfo.Mobile) && mobileInfo.UserId != userId)
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                //var entity = manager.GetUserMobile(userId);
                //if (entity != null && !string.IsNullOrEmpty(entity.Mobile) && !entity.IsSettedMobile)
                //{
                //    entity.Mobile = mobile;
                //    entity.IsSettedMobile = true;
                //    manager.UpdateUserMobile(entity);
                //    //throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证", entity.CreateTime));
                //}
                if (mobileInfo != null && !string.IsNullOrEmpty(mobileInfo.Mobile) && !mobileInfo.IsSettedMobile)
                {
                    //mobileInfo.UserId = userId;
                    mobileInfo.UpdateBy = userId;
                    mobileInfo.UpdateTime = DateTime.Now;
                    mobileInfo.Mobile = mobile;
                    mobileInfo.IsSettedMobile = true;
                    manager.UpdateUserMobile(mobileInfo);
                }
                else
                {
                    if (mobileInfo == null)
                    {
                        mobileInfo = new UserMobile
                        {
                            UserId = userId,
                            User = manager.LoadUser(userId),
                            AuthFrom = authFrom,
                            Mobile = mobile,
                            IsSettedMobile = true,
                            CreateBy = createBy,
                            UpdateBy = createBy,
                        };
                        manager.AddUserMobile(mobileInfo);

                        var gv = new TaskListManager();
                        var old = gv.QueryTaskListByCategory(userId, TaskCategory.MobilBinding);
                        if (old == null)
                        {
                            //增加成长值 
                            var orderId = Guid.NewGuid().ToString("N");
                            BusinessHelper.Payin_UserGrowth("绑定手机号", orderId, userId, 100, "完成手机号绑定可获得100点成长值");
                            gv.AddUserTaskRecord(new UserTaskRecord
                            {
                                CreateTime = DateTime.Now,
                                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                                OrderId = orderId,
                                TaskCategory = TaskCategory.MobilBinding,
                                TaskName = "绑定手机号",
                                UserId = userId,
                            });
                            gv.AddTaskList(new TaskList
                            {
                                UserId = userId,
                                OrderId = orderId,
                                Content = "完成手机号绑定可获得100点成长值",
                                ValueGrowth = 100,
                                IsGive = true,
                                VipLevel = 0,
                                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                                TaskCategory = TaskCategory.MobilBinding,
                                TaskName = "绑定手机号",
                                CreateTime = DateTime.Now,
                            });
                        }
                    }
                    else
                        throw new ArgumentException(string.Format("此用户已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证", mobileInfo.CreateTime));
                }
            }

        }
        public void UpdateAuthenticationMobile(string authFrom, string userId, string mobile, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserMobileManager())
                {
                    var other = manager.GetOtherUserMobile(mobile, userId);
                    if (other != null)
                    {
                        throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", mobile));
                    }
                    var entity = manager.GetUserMobile(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过手机认证");
                    }
                    entity.AuthFrom = authFrom;
                    entity.Mobile = mobile;
                    entity.UpdateBy = updateBy;
                    entity.IsSettedMobile = true;

                    manager.UpdateUserMobile(entity);
                }
                biz.CommitTran();
            }
        }
        public void CancelAuthenticationMobile(string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserMobileManager())
                {
                    var entity = manager.GetUserMobile(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过手机认证");
                    }
                    entity.IsSettedMobile = false;

                    manager.UpdateUserMobile(entity);
                }
                biz.CommitTran();
            }
        }

        private const string C_DefaultPassword = "123456";
        private string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        public string ResetUserBalancePwd(string userId)
        {
            var newPwd = string.Empty;
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserMobileManager();
                //var entity = manager.GetUserMobile(userId);
                //if (entity == null)
                //    throw new ArgumentException("此用户从未进行过手机认证");
                //if (entity.Mobile != mobileNumber)
                //    throw new ArgumentException("用户认证的手机号码不正确");

                var balanceManager = new UserBalanceManager();
                var balance = balanceManager.QueryUserBalance(userId);
                if (balance == null)
                    throw new ArgumentException("用户资金信息查询出错");

                balance.Password = Encipherment.MD5(string.Format("{0}{1}", C_DefaultPassword, _gbKey)).ToUpper();
                balanceManager.UpdateUserBalance(balance);

                biz.CommitTran();
            }
            return newPwd;
        }

        public void AdminResetUserBalancePwd(string userId, string pwd)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var balanceManager = new UserBalanceManager();
                var balance = balanceManager.QueryUserBalance(userId);
                if (balance == null)
                    throw new ArgumentException("用户资金信息查询出错");

                balance.Password = Encipherment.MD5(pwd);
                balanceManager.UpdateUserBalance(balance);

                biz.CommitTran();
            }
        }

        private string BuildRandomNumber(int length)
        {
            var list = new List<string>();
            var ran = new Random();
            for (int i = 0; i < length; i++)
            {
                list.Add(ran.Next(0, 10).ToString());
            }
            return string.Join("", list.ToArray());
        }

        public void SetUserMobile(string userId, string newMobile, string updateBy)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserMobileManager())
                {
                    if (!manager.ExistMobile(newMobile))
                    {
                        throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户认证。", newMobile));
                    }
                    var entity = manager.GetUserMobile(userId);
                    if (entity == null)
                    {
                        throw new ArgumentException("此用户从未进行过实名认证");
                    }
                    //if (!oldMobile.Equals(entity.Mobile))
                    //{
                    //    throw new ArgumentException(string.Format("输入的旧手机号【{0}】与原有手机号不相同。", oldMobile));
                    //}
                    entity.Mobile = newMobile;
                    entity.UpdateTime = DateTime.Now;
                    entity.UpdateBy = updateBy;
                    manager.UpdateUserMobile(entity);
                }
                biz.CommitTran();
            }
        }

        /// <summary>
        /// 更新手机认证
        /// </summary>
        public void UpdateMobileAuthen(string userId, string mobile, string updateBy)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserMobileManager();
                var mobileInfo = manager.GetMobileInfoByMobile(mobile);
                var entity = manager.GetUserMobile(userId);
                if (entity == null)
                    throw new ArgumentException("此用户从未进行过手机认证");
                if (mobileInfo != null && mobileInfo.UserId != userId)
                {
                    if (!string.IsNullOrEmpty(mobileInfo.Mobile) && mobileInfo.IsSettedMobile)
                        throw new Exception("当前手机号码已被他人占用！");
                }
                entity.Mobile = mobile;
                entity.RequestTimes = 0;
                entity.IsSettedMobile = true;
                entity.UpdateBy = updateBy;
                entity.UpdateTime = DateTime.Now;
                manager.UpdateUserMobile(entity);

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 注销手机认证
        /// </summary>
        public void LogOffMobileAuthen(string userId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserMobileManager();
                var entity = manager.GetUserMobile(userId);
                if (entity == null)
                    throw new ArgumentException("此用户从未进行过手机认证");
                manager.DeleteUserMobile(entity);

                biz.CommitTran();
            }
        }
        /// <summary>
        /// 交换手机认证
        /// </summary>
        public void SwapMobileAuthen(string fromUserId, string toUserId, string updateBy)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserMobileManager();
                var fromEntity = manager.GetUserMobile(fromUserId);
                if (fromEntity == null)
                    throw new ArgumentException(fromUserId + "从未进行过手机认证");
                fromEntity.IsSettedMobile = false;
                fromEntity.UpdateBy = updateBy;
                fromEntity.UpdateTime = DateTime.Now;
                manager.UpdateUserMobile(fromEntity);

                var toEntity = manager.GetUserMobile(toUserId);
                if (toEntity != null)
                {
                    toEntity.Mobile = fromEntity.Mobile;
                    toEntity.IsSettedMobile = true;
                    toEntity.RequestTimes = 0;
                    toEntity.UpdateBy = updateBy;
                    toEntity.UpdateTime = DateTime.Now;
                }
                else
                {
                    var toUser = manager.LoadUser(toUserId);
                    if (toUser == null)
                        throw new ArgumentException(toUserId + "不存在");
                    manager.AddUserMobile(new UserMobile
                    {
                        UserId = toUserId,
                        User = toUser,
                        AuthFrom = fromEntity.AuthFrom,
                        IsSettedMobile = true,
                        Mobile = fromEntity.Mobile,
                        RequestTimes = 0,
                        CreateBy = updateBy,
                        UpdateBy = updateBy,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                    });
                }

                biz.CommitTran();
            }
        }
        public UserMobileInfo QueryMobileByUserId(string userId)
        {
            using (var manager = new UserMobileManager())
            {
                UserMobileInfo info = new UserMobileInfo();
                var Mobile = manager.GetUserMobile(userId);
                if (Mobile == null)
                    return info;
                info.AuthFrom = Mobile.AuthFrom;
                info.CreateBy = Mobile.CreateBy;
                info.CreateTime = Mobile.CreateTime;
                info.IsSettedMobile = Mobile.IsSettedMobile;
                info.Mobile = Mobile.Mobile;
                info.RequestTimes = Mobile.RequestTimes;
                info.UserId = Mobile.UserId;
                return info;
            }
        }

        /// <summary>
        /// 检查手机是否绑定
        /// </summary>
        /// <returns></returns>
        public bool IsMobileBinded(string userId, string mobile)
        {
            using (var manager = new UserMobileManager())
            {
                var other = manager.GetOtherUserMobile(mobile, userId);
                if (other != null && other.IsSettedMobile)
                {
                    return true;
                }
                return false;
            }
        }
        public bool CheckMobileIsBind(string mobile)
        {
            using (var manager = new UserMobileManager())
            {
                var mobileEntity = manager.GetMobileInfoByMobile(mobile);
                if (mobileEntity != null && mobileEntity.Mobile == mobile)
                    return true;
                return false;
            }
        }

        #region 注册成功后验证手机


        public void RegisterRequestMobile(string mobile)
        {
            using (var manager = new UserMobileManager())
            {
                var other = manager.GetMobileInfoByMobile(mobile);
                if (other != null && other.IsSettedMobile)
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户注册。", mobile));
                //var entity = manager.GetUserMobile(userId);
                //if (entity != null)
                //{
                //    if (!string.IsNullOrEmpty(entity.Mobile) && entity.IsSettedMobile)
                //    {
                //        throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
                //    }
                //    if (entity.RequestTimes >= maxRequestTime)
                //    {
                //        throw new ArgumentException(string.Format("已请求最大限制次数【{0}】次还未成功认证，请联系客服。", maxRequestTime));
                //    }
                //    if (mobile != entity.Mobile)
                //    {
                //        entity.Mobile = mobile;
                //        manager.UpdateUserMobile(entity);
                //    }
                //}
                //else
                //{
                //    entity = new UserMobile
                //    {
                //        UserId = userId,
                //        User = manager.LoadUser(userId),
                //        AuthFrom = "LOCAL",
                //        Mobile = mobile,
                //        IsSettedMobile = false,
                //        CreateBy = createBy,
                //        UpdateBy = createBy,
                //    };
                //    manager.AddUserMobile(entity);
                //}
            }
        }
        public string RegisterResponseMobile(string userId, string mobile, int delaySeconds, string delayDescription)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var gv = new TaskListManager();
                var old = gv.QueryTaskListByCategory(userId, TaskCategory.MobilBinding);
                if (old == null)
                {
                    //增加成长值 
                    var orderId = Guid.NewGuid().ToString("N");
                    BusinessHelper.Payin_UserGrowth("绑定手机号", orderId, userId, 100, "完成手机号绑定可获得100点成长值");
                    gv.AddUserTaskRecord(new UserTaskRecord
                    {
                        CreateTime = DateTime.Now,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        OrderId = orderId,
                        TaskCategory = TaskCategory.MobilBinding,
                        TaskName = "绑定手机号",
                        UserId = userId,
                    });
                    gv.AddTaskList(new TaskList
                    {
                        UserId = userId,
                        OrderId = orderId,
                        Content = "完成手机号绑定可获得100点成长值",
                        ValueGrowth = 100,
                        IsGive = true,
                        VipLevel = 0,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = TaskCategory.MobilBinding,
                        TaskName = "绑定手机号",
                        CreateTime = DateTime.Now,
                    });
                }

                using (var manager = new UserMobileManager())
                {
                    var entity = manager.GetUserMobile(userId);
                    if (entity != null)
                    {
                        if (entity.IsSettedMobile)
                            throw new ArgumentException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
                        var span = DateTime.Now - entity.UpdateTime.AddSeconds(delaySeconds);
                        if (span.TotalSeconds > 0)
                            throw new ArgumentException(string.Format("提交认证手机必须在请求认证后【{0}】内完成。", delayDescription));
                        entity.IsSettedMobile = true;
                        manager.UpdateUserMobile(entity);
                    }
                    else
                    {
                        entity = new UserMobile
                       {
                           UserId = userId,
                           User = manager.LoadUser(userId),
                           AuthFrom = "LOCAL",
                           Mobile = mobile,
                           IsSettedMobile = true,
                           CreateBy = userId,
                           UpdateBy = userId,
                       };
                        manager.AddUserMobile(entity);
                    }

                    mobile = entity.Mobile;
                }
                biz.CommitTran();
            }
            return mobile;
        }

        #endregion

        #region 手机号是否已注册
        /// <summary>
        /// 手机号是否已注册
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool HasMobile(string mobile)
        {
            using (var manager = new UserMobileManager())
            {
                var model = manager.GetMobileInfoByMobile(mobile);
                if (model == null) return false;
                return true;
            }
        } 
        #endregion
    }
}
