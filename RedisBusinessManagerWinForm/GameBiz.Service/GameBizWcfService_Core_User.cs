using System;
using System.Linq;
using System.Transactions;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using GameBiz.Auth.Business;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Core : WcfService
    {
        /// <summary>
        /// 修改用户VIP级别  
        /// </summary>
        public CommonActionResult UpdateUserVipLevel(string userId, int vipLevel, string userToken)
        {
            // 验证用户身份及权限  
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var biz = new UserBusiness();
                biz.UpdateUserVipLevel(userId, vipLevel);
                return new CommonActionResult(true, "修改用户VIP级别完成");
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                throw new Exception(string.Format("修改用户VIP级别失败 - {0} ! ", ex.Message), ex);
            }
        }
        /// <summary>
        /// 修改隐藏名称数
        /// </summary>
        /// <param name="hideDisplayNameCount"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult ChangeUserHideDisplayNameCount(int hideDisplayNameCount, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var biz = new UserBusiness();
                biz.ChangeUserHideDisplayNameCount(userId, hideDisplayNameCount);

                BusinessHelper.ExecPlugin<IChangeHideDisplayNameCount_AfterTranCommit>(new object[] { userId, hideDisplayNameCount });

                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                throw new Exception(string.Format("修改隐藏名称数失败 - {0} ! ", ex.Message), ex);
            }
        }
        /// <summary>
        /// 修改代理商编号
        /// </summary>
        public CommonActionResult UpdateUserAgentId(string userId, string agentId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new UserBusiness().UpdateUserAgentId(userId, agentId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 注销代理商
        /// </summary>
        public CommonActionResult LogOffUserAgent(string userId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new UserBusiness().LogOffUserAgent(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public CommonActionResult SaveUserBalanceLog(string saveDate)
        {
            try
            {
                new UserBusiness().SaveUserBalanceLog(saveDate);
                return new CommonActionResult(true, "保存成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }

        }

        #region 后台系统管理

        /// <summary>
        /// 查询人员列表
        /// </summary>
        public SysOpratorInfo_Collection GetOpratorCollection(int pageIndex, int pageSize, string userToken)
        {

            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBizAuthBusiness().GetOpratorCollection(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询角色列表
        /// </summary>
        public RoleInfo_QueryCollection QueryRoleCollection(string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBizAuthBusiness().QueryRoleCollection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据用户编号，查询角色
        /// </summary>
        public string QueryUserRoleIdsByUserId(string userId, string userToken)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBizAuthBusiness().QueryUserRoleIdsByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion
    }
}
