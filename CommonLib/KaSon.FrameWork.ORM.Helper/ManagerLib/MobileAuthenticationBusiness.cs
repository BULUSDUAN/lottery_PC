using EntityModel;
using EntityModel.CoreModel.AuthEntities;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class MobileAuthenticationBusiness : DBbase
    {
        private const string C_DefaultPassword = "123456";
        private string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        /// <summary>
        /// 手机黑名单
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public C_Core_Config BanRegistrMobile(string key)
        {
            return DB.CreateQuery<C_Core_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
        }

        /// <summary>
        /// 查询手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public UserMobile GetMobileInfoByMobile(string mobile)
        {
            var query = DB.CreateQuery<E_Authentication_Mobile>()
                         .Where(s => s.Mobile == mobile).ToList().Select(p => new UserMobile
                         {
                             AuthFrom = p.AuthFrom,
                             CreateBy = p.CreateBy,
                             CreateTime = p.CreateTime,
                             IsSettedMobile = p.IsSettedMobile,
                             Mobile = p.Mobile

                         }).ToList();

            if (query != null && query.Count() > 0)
            {
                var resutl = query.FirstOrDefault(s => s.IsSettedMobile == true);
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.FirstOrDefault(s => s.IsSettedMobile == false);
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据用户ID查询手机号
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public E_Authentication_Mobile GetAuthenticatedMobile(string UserId) {

            return DB.CreateQuery<E_Authentication_Mobile>().Where(p => p.UserId == UserId).FirstOrDefault();
        }

        #region  检查手机号是否认证过
        public bool IsMobileAuthenticated(string mobile)
        {
           
                var other = GetOtherUserMobile(mobile, "");
                return (other != null);
           
        }

        public E_Authentication_Mobile GetOtherUserMobile(string mobile, string userId)
        {
           
            var list = DB.CreateQuery<E_Authentication_Mobile>()
                .Where(p=>p.Mobile==mobile && p.UserId==userId).ToList();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];

        }
        #endregion

        public string ResponseAuthenticationMobile(string userId, int delaySeconds, string delayDescription)
        {
            string mobile;

            try
            {
                DB.Begin();

                TaskList(userId);

                var manager = new UserMobileManager();
                var entity = manager.GetUserMobile(userId);
                if (entity == null)
                {
                    throw new LogicException("尚未请求手机认证");
                }
                if (entity.IsSettedMobile)
                {
                    throw new LogicException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
                }
                var span = DateTime.Now - entity.UpdateTime.AddSeconds(delaySeconds);
                if (span.TotalSeconds > 0)
                {
                    throw new LogicException(string.Format("提交认证手机必须在请求认证后【{0}】内完成。", delayDescription));
                }
                entity.IsSettedMobile = true;

                manager.UpdateUserMobile(entity);

                mobile = entity.Mobile;

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }

            return mobile;
        }


        public string RegisterResponseMobile(string userId, string mobile, int delaySeconds, string delayDescription)
        {

            try
            {
                DB.Begin();

                TaskList(userId);

                var manager = new UserMobileManager();
                var entity = manager.GetUserMobile(userId);
                if (entity != null)
                {
                    if (entity.IsSettedMobile)
                        throw new LogicException(string.Format("已于【{0:yyyy-MM-dd HH:mm:ss}】进行过手机认证。", entity.UpdateTime));
                    var span = DateTime.Now - entity.UpdateTime.AddSeconds(delaySeconds);
                    if (span.TotalSeconds > 0)
                        throw new LogicException(string.Format("提交认证手机必须在请求认证后【{0}】内完成。", delayDescription));
                    entity.IsSettedMobile = true;
                    manager.UpdateUserMobile(entity);
                }
                else
                {
                    entity = new E_Authentication_Mobile
                    {
                        UserId = userId,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        AuthFrom = "LOCAL",
                        Mobile = mobile,
                        IsSettedMobile = true,
                        CreateBy = userId,
                        UpdateBy = userId,
                    };
                    manager.AddUserMobile(entity);
                }

                mobile = entity.Mobile;

                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }


            return mobile;
        }

        public void TaskList(string userId)
        {

            var gv = new TaskListManager();
            var old = DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)TaskCategory.MobilBinding).FirstOrDefault();
            if (old == null)
            {
                //增加成长值 
                var orderId = Guid.NewGuid().ToString("N");
                BusinessHelper.Payin_UserGrowth("绑定手机号", orderId, userId, 100, "完成手机号绑定可获得100点成长值");
                var UserTaskRecord = new E_UserTaskRecord
                {
                    CreateTime = DateTime.Now,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    OrderId = orderId,
                    TaskCategory = (int)TaskCategory.MobilBinding,
                    TaskName = "绑定手机号",
                    UserId = userId,
                };
                DB.GetDal<E_UserTaskRecord>().Add(UserTaskRecord);

                var TaskList = new E_TaskList
                {
                    UserId = userId,
                    OrderId = orderId,
                    Content = "完成手机号绑定可获得100点成长值",
                    ValueGrowth = 100,
                    IsGive = true,
                    VipLevel = 0,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.MobilBinding,
                    TaskName = "绑定手机号",
                    CreateTime = DateTime.Now,
                };
                DB.GetDal<E_TaskList>().Add(TaskList);
            }
        }

        public void RegisterRequestMobile(string mobile)
        {
                 var manager = new UserMobileManager();
           
                var other = manager.GetMobileInfoByMobile(mobile);
                if (other != null && other.IsSettedMobile)
                    throw new ArgumentException(string.Format("此手机号【{0}】已被其他用户注册。", mobile));
             
            
        }

        #region 手机号是否已注册
        /// <summary>
        /// 手机号是否已注册
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool HasMobile(string mobile)
        {
            var manager = new UserMobileManager();
            {
                var model = manager.GetMobileInfoByMobile(mobile);
                if (model == null) return false;
                return true;
            }
        }
        #endregion

        public string ResetUserBalancePwd(string userId)
        {
            var newPwd = string.Empty;
            //开启事务
            DB.Begin();
            var manager = new UserMobileManager();
            var balanceManager = new UserBalanceManager();
            var balance = balanceManager.QueryUserBalance(userId);
            if (balance == null)
                throw new ArgumentException("用户资金信息查询出错");
            balance.Password = Encipherment.MD5(string.Format("{0}{1}", C_DefaultPassword, _gbKey)).ToUpper();
            balanceManager.UpdateUserBalance(balance);
            DB.Commit();
            return newPwd;
        }
        /// <summary>
        /// 更新手机认证
        /// </summary>
        public void UpdateMobileAuthen(string userId, string mobile, string updateBy)
        {
            //开启事务
            DB.Begin();
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
            entity.IsSettedMobile = true;
            entity.UpdateBy = updateBy;
            entity.UpdateTime = DateTime.Now;
            manager.UpdateUserMobile(entity);
            DB.Commit();
        }
        /// <summary>
        /// 注销手机认证
        /// </summary>
        public void LogOffMobileAuthen(string userId)
        {
            //开启事务
                DB.Begin();
                var manager = new UserMobileManager();
                var entity = manager.GetUserMobile(userId);
                if (entity == null)
                    throw new ArgumentException("此用户从未进行过手机认证");
                manager.DeleteUserMobile(entity);
            DB.Commit();
        }
        public void AddAuthenticationMobile(string authFrom, string userId, string mobile, string createBy)
        {

            var manager = new UserMobileManager();
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
                        mobileInfo = new E_Authentication_Mobile
                        {
                            UserId = userId,
                            //User = manager.LoadUser(userId),
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
}
