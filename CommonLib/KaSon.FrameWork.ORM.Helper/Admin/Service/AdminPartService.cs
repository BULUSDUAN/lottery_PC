﻿using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper.ManagerLib;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper
{
    public partial class AdminService
    {
        #region 活动管理模块
        #region 优惠券相关
        /// <summary>
        /// 查询优惠券列表
        /// </summary>
        /// <returns></returns>
        //public A20131105CouponsInfoCollection QueryCouponsList(string summary, bool? canUsable, string belongUserId, int pageIndex, int pageSize, string userToken)
        //{
        //    var result = new A20131105CouponsInfoCollection();
        //    var totalCount = 0;
        //    var activityManager = new A20131105Manager();
        //    var list = activityManager.QueryCouponsList(summary, canUsable, belongUserId, pageIndex, pageSize, out totalCount);
        //    result.TotalCount = totalCount;
        //    result.List.AddRange(list);
        //    return result;
        //}
        /// <summary>
        /// 生成优惠券
        /// </summary>
        /// <returns></returns>
        //public CommonActionResult BuildCoupons(string summary, decimal money, int count, string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
        //    try
        //    {
        //        new A20131105().BuildCoupons(summary, money, count);
        //        return new CommonActionResult(true, "生成完成");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("生成优惠券出错 - " + ex.ToString());
        //    }
        //}
        #endregion

        #region 活动配置相关
        /// <summary>
        /// 获取网站活动配置
        /// </summary>
        public string GetActityConfig()
        {
            var configList = ActivityCache.QueryActivityConfig();
            var query = from c in configList
                        select string.Format("{0}_{1}_{2}", c.ConfigKey, c.ConfigName, c.ConfigValue);
            return string.Join("|", query.ToArray());
        }
        /// <summary>
        /// 更新活动配置
        /// </summary>
        /// <returns></returns>
        public CommonActionResult UpdateActivityConfig(string key, string value)
        {
            try
            {
                ActivityCache.UpdateActivityConfig(key, value);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        #endregion

        #region 红包配置相关
        /// <summary>
        /// 查询充值赠送红包配置
        /// </summary>
        public FillMoneyGiveRedBagConfigInfoCollection QueryFillMoneyGiveRedBagConfigList()
        {
            return new A20150919_FillMoneyGive().QueryFillMoneyGiveRedBagConfigList();
        }
        /// <summary>
        /// 增加充值赠送红包配置
        /// </summary>
        public CommonActionResult AddFillMoneyGiveRedBagConfig(decimal fillMoney, decimal giveMoney)
        {
            try
            {
                new A20150919_FillMoneyGive().AddFillMoneyGiveRedBagConfig(fillMoney, giveMoney);
                return new CommonActionResult(true, "添加成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 修改充值红包赠送配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fillMoney"></param>
        /// <param name="giveMoney"></param>
        /// <returns></returns>
        public CommonActionResult UpdateFillMoneyGiveRedBagConfig(int id, decimal fillMoney, decimal giveMoney)
        {
            try
            {
                new A20150919_FillMoneyGive().UpdateFillMoneyGiveRedBagConfig(id, fillMoney, giveMoney);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 删除充值赠送红包配置
        /// </summary>
        public CommonActionResult DeleteFillMoneyGiveRedBagConfig(int id)
        {
            try
            {
                new A20150919_FillMoneyGive().DeleteFillMoneyGiveRedBagConfig(id);
                return new CommonActionResult(true, "删除成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        #endregion

        #region 加奖配置相关
        /// <summary>
        /// 增加加奖配置
        /// </summary>
        public CommonActionResult AddAddBonusMoneyConfig(string gameCode, string gameType, string playType, decimal addPercent, decimal maxAddMoney, int orderIndex, string addMoneyWay)
        {
            try
            {
                new A20150919_AddBonusMoney().AddAddBonusMoneyConfig(gameCode, gameType, playType, addPercent, maxAddMoney, orderIndex, addMoneyWay);
                return new CommonActionResult(true, "增加成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 查询加奖配置
        /// </summary>
        public AddBonusMoneyConfigInfoCollection QueryAddBonusMoneyConfig()
        {
            return new A20150919_AddBonusMoney().QueryAddBonusMoneyConfig();
        }

        /// <summary>
        /// 删除加奖配置
        /// </summary>
        public CommonActionResult DeleteAddBonusMoneyConfig(int id)
        {
            try
            {
                new A20150919_AddBonusMoney().DeleteAddBonusMoneyConfig(id);
                return new CommonActionResult(true, "删除成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        #endregion

        #region 红包使用规则相关
        /// <summary>
        /// 查询红包使用规则
        /// </summary>
        public string QueryRedBagUseConfig()
        {
            return new A20150919_RedBagUseConfig().QueryRedBagUseConfig();
        }
        /// <summary>
        /// 添加红包使用规则
        /// </summary>
        public CommonActionResult AddRedBagUseConfig(string gameCode, decimal percent)
        {
            try
            {
                new A20150919_RedBagUseConfig().AddRedBagUseConfig(gameCode, percent);
                return new CommonActionResult(true, "添加成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 删除红包使用规则
        /// </summary>
        public CommonActionResult DeleteRedBagUseConfig(int id)
        {
            try
            {
                new A20150919_RedBagUseConfig().DeleteRedBagUseConfig(id);
                return new CommonActionResult(true, "删除成功");

            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        #endregion

        #region 不参与加奖相关
        /// <summary>
        /// 查询取消加奖列表
        /// </summary>
        public UserGameCodeNotAddMoneyInfoCollection QueryUserGameCodeNotAddMoneyList(string userId)
        {
            return new A20150919_UserGameCodeNotAddMoney().QueryUserGameCodeNotAddMoneyList(userId);
        }
        /// <summary>
        /// 增加用户的取消加奖
        /// </summary>
        public CommonActionResult AddUserGameCodeNotAddMoney(string userId, string gameCode, string gameType, string playType)
        {
            try
            {
                new A20150919_UserGameCodeNotAddMoney().AddUserGameCodeNotAddMoney(userId, gameCode, gameType, playType);
                return new CommonActionResult(true, "增加成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 删除用户的取消加奖
        /// </summary>
        public CommonActionResult DeleteUserGameCodeNotAddMoney(int id)
        {
            try
            {
                new A20150919_UserGameCodeNotAddMoney().DeleteUserGameCodeNotAddMoney(id);
                return new CommonActionResult(true, "删除成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        #endregion

        #region 获取系统配置
        public C_Core_Config QueryCoreConfigByKey(string key)
        {
            try
            {
                return new CacheDataBusiness().QueryCoreConfigByKey(key);
            }
            catch (Exception ex)
            {
                throw new Exception("查询系统配置出错", ex);
            }
        }
        /// <summary>
        /// 修改是否暂停投注标识
        /// </summary>
        public CommonActionResult UpdateCoreConfigInfoByKey(string configKey, string configValue)
        {
            try
            {
                new Sports_Business().UpdateCoreConfigInfo(configKey, configValue);
                return new CommonActionResult(true, "修改信息成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        #endregion
        #endregion

        #region 比赛管理模块

        #region 队伍数据更新
        #region 北京单场数据信息更新
        public CommonActionResult ManualUpdate_BJDC_MatchList(string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_BJDC_MatchList(issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 北京单场比赛结果更新
        public CommonActionResult ManualUpdate_BJDC_MatchResultList(string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_BJDC_MatchResultList(issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 竞彩足球数据更新
        public CommonActionResult ManualUpdate_JCZQ_MatchList(string userToken)
        {
            try
            {
                new IssuseBusiness().ManualUpdate_JCZQ_MatchList();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 竞彩足球SP更新
        public CommonActionResult UpdateOddsList_JCZQ_Manual()
        {
            try
            {
                new TicketGatewayAdmin().UpdateOddsList_JCZQ_Manual();

                return new CommonActionResult(true, "处理数据成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }
        #endregion

        #region 竞彩篮球数据更新
        public CommonActionResult ManualUpdate_JCLQ_MatchList(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_JCLQ_MatchList();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 竞彩篮球SP更新
        public CommonActionResult UpdateOddsList_JCLQ_Manual()
        {
            try
            {
                new TicketGatewayAdmin().UpdateOddsList_JCLQ_Manual();

                return new CommonActionResult(true, "处理数据成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }
        #endregion

        #region 传统足球数据更新
        public CommonActionResult ManualUpdate_CTZQ_MatchList(string gameCode, string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_CTZQ_MatchList(gameCode, issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 根据彩种更新比赛为取消
        public CommonActionResult DoMatchCancel(string gameCode, string matchId, string issuse)
        {
            try
            {
                new Sports_Business().DoMatchCancel(gameCode, matchId, issuse);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "更新数据过程发生异常 - " + ex.Message);
            }
        }
        #endregion
        #endregion

        #region 禁用比赛
        public CoreJCZQMatchInfoCollection QueryCurrentJCZQMatchInfo(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new IssuseBusiness().QueryCurrentJCZQMatchInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public CoreJCLQMatchInfoCollection QueryCurrentJCLQMatchInfo(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new IssuseBusiness().QueryCurrentJCLQMatchInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public CoreBJDCMatchInfoCollection QueryCurrentBJDCMatchInfo(string userToken)
        {
            try
            {
                return new IssuseBusiness().QueryCurrentBJDCMatchInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        public CommonActionResult UpdateJCZQMatchInfo(string matchId, string privilegesType, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().UpdateJCZQMatchInfo(matchId, privilegesType);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public CommonActionResult UpdateJCLQMatchInfo(string matchId, string privilegesType, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().UpdateJCLQMatchInfo(matchId, privilegesType);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public CommonActionResult UpdateBJDCMatchInfo(string Id, string privilegesType)
        {
            try
            {
                new IssuseBusiness().UpdateBJDCMatchInfo(Id, privilegesType);
                return new CommonActionResult(true, "更新成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public CommonActionResult UpdateLotteryGame(string userToken, string gameCode, int enableStatus)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new GameBusiness().UpdateLotteryGame(gameCode, enableStatus);

                return new CommonActionResult(true, string.Format("更新彩种状态成功"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 队伍图标更新
        public IndexMatch_Collection QueryIndexMatchCollection(string matchId, string hasImg, int pageIndex, int pageSize)
        {
            try
            {
                return new Sports_Business().QueryIndexMatchCollection(matchId, hasImg, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public IndexMatchInfo QueryIndexMatchInfo(int id)
        {
            try
            {
                return new Sports_Business().QueryIndexMatchInfo(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public CommonActionResult UpdateIndexMatch(int id, string imgPath)
        {
            try
            {
                new Sports_Business().UpdateIndexMatch(id, imgPath);
                return new CommonActionResult(true, "修改队伍图标成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion
        #endregion

        #region 会员管理模块
        public UserQueryInfoCollection QueryUserList(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? IsUserType, bool? isAgent
            , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize, string userToken, string strOrderBy, int UserCreditType = -1)
        {
            var siteBiz = new LocalLoginBusiness();
            return siteBiz.QueryUserList(regFrom, regTo, keyType, keyValue, isEnable, isFillMoney, IsUserType, isAgent, commonBlance, bonusBlance, freezeBlance, vipRange, comeFrom, agentId, pageIndex, pageSize, strOrderBy, UserCreditType);
        }

        public CommonActionResult DisableUser(string userId, string userToken)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new LocalLoginBusiness().DisableUser(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 启用用户
        /// todo:后台权限
        /// </summary>
        public CommonActionResult EnableUser(string userId, string userToken)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new LocalLoginBusiness().EnableUser(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 批量设置用户为内部员工
        /// </summary>
        /// <param name="userIds"></param>
        public CommonActionResult BatchSetInnerUser(string userIds)
        {
            try
            {
                new LocalLoginBusiness().BatchSetInnerUser(userIds);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public NotOnlineUserCollection QueryNotOnlineRecentlyList(int days, int pageIndex, int pageSize)
        {
            return new LocalLoginBusiness().QueryNotOnlineRecentlyList(days, pageIndex, pageSize);
        }
        public string GiveMoneyToStayUser(string userId, string operatorId)
        {
            return new LocalLoginBusiness().GiveMoneyToStayUser(userId, operatorId);
        }
        public NotOnlineUserCollection QueryNotOnlineRecentlyList(int days, int pageIndex, int pageSize, string theEarnings)
        {
            return new LocalLoginBusiness().QueryNotOnlineRecentlyList(days, pageIndex, pageSize, theEarnings);
        }
        public NotOnlineUserCollection QueryExcelNotOnlineRecentlyList(int days, string theEarnings)
        {
            var siteBiz = new LocalLoginBusiness();
            return siteBiz.QueryExcelNotOnlineRecentlyList(days, theEarnings);
        }
        /// <summary>
        /// 根据登录名查询用户Id
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public string GetUserIdByLoginName(string loginName)
        {
            var loginBiz = new LocalLoginBusiness();
            var user = loginBiz.GetUserByLoginName(loginName);
            return user.UserId;
        }
        /// <summary>
        /// 根据Key值查询用户列表
        /// todo:后台权限
        /// </summary>
        public UserQueryInfo QueryUserByKey(string userKey, string userToken)
        {
            var siteBiz = new LocalLoginBusiness();
            return siteBiz.QueryUserByKey(userKey, string.Empty);
        }
        /// <summary>
        /// 根据用户编号查询用户历史登录
        /// </summary>
        public UserLoginHistoryCollection QueryCache_UserLoginHistoryCollectionByUserId(string userId, string userToken)
        {
            try
            {
                return new CacheDataBusiness().QueryUserLoginHistoryCollection(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("获取最近登录 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据用户编号查询银行卡信息
        /// </summary>
        public C_BankCard QueryBankCardByUserId(string userId, string userToken)
        {
            return new BankCardBusiness().BankCardById(userId);
        }
        /// <summary>
        /// 获取口令
        /// todo:后台权限
        /// </summary>
        public CommonActionResult GetUserTokenByKey(string userId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            return GetUserToken(userId);
        }
        public CommonActionResult GetUserToken(string userId)
        {
            var biz = new GameBizAuthBusiness();
            var token = biz.GetUserToken(userId);
            return new CommonActionResult(true, "获取用户口令密文成功") { ReturnValue = token };
        }
        /// <summary>
        /// 查询用户冻结明细
        /// </summary>
        public UserBalanceFreezeCollection QueryUserBalanceFreezeListByUser(string userId, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new FundBusiness().QueryUserBalanceFreezeListByUser(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户冻结明细出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 检查用户是否实名认证
        /// todo:后台权限
        /// </summary>
        public bool CheckIsAuthenticatedUserRealName(string userId, string userToken)
        {
            var biz = new RealNameAuthenticationBusiness();
            var realName = biz.GetAuthenticatedRealName(userId);
            return (realName != null && realName.IsSettedRealName);
        }
        /// <summary>
        /// 获取用户实名认证信息
        /// todo:后台权限
        /// </summary>
        public UserRealNameInfo GetUserRealNameInfo(string userId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var biz = new RealNameAuthenticationBusiness();
            var realName = biz.GetAuthenticatedRealName(userId);
            if (realName == null || !realName.IsSettedRealName)
            {
                return null;
            }
            return new UserRealNameInfo
            {
                AuthFrom = realName.AuthFrom,
                RealName = realName.RealName,
                CardType = realName.CardType,
                IdCardNumber = realName.IdCardNumber,
            };
        }
        /// <summary>
        /// 检查用户是否设置手机号码
        /// todo:后台权限
        /// </summary>
        public bool CheckIsAuthenticatedUserMobile(string userId, string userToken)
        {
            var authenticationBiz = new MobileAuthenticationBusiness();
            var mobileEntity = authenticationBiz.GetAuthenticatedMobile(userId);
            return (mobileEntity != null && mobileEntity.IsSettedMobile);
        }
        /// <summary>
        /// 获取用户手机认证信息
        /// todo:后台权限
        /// </summary>
        public UserMobileInfo GetUserMobileInfo(string userId, string userToken)
        {
            var authenticationBiz = new MobileAuthenticationBusiness();
            var mobileEntity = authenticationBiz.GetAuthenticatedMobile(userId);
            if (mobileEntity == null || !mobileEntity.IsSettedMobile)
            {
                return null;
            }
            return new UserMobileInfo
            {
                AuthFrom = mobileEntity.AuthFrom,
                Mobile = mobileEntity.Mobile,
            };
        }
        /// <summary>
        /// 检测用户是否设置邮箱
        /// todo:后台权限
        /// </summary>
        public bool CheckIsAuthenticatedUserEmail(string userId, string userToken)
        {
            var authenticationBiz = new EmailAuthenticationBusiness();
            var emailEntity = authenticationBiz.GetAuthenticatedEmail(userId);
            return (emailEntity != null && emailEntity.IsSettedEmail);
        }
        /// <summary>
        /// 获取用户邮箱信息
        /// todo:后台权限
        /// </summary>
        public E_Authentication_Email GetUserEmailInfo(string userId, string userToken)
        {
            var authenticationBiz = new EmailAuthenticationBusiness();
            var emailEntity = authenticationBiz.GetAuthenticatedEmail(userId);
            if (emailEntity == null || !emailEntity.IsSettedEmail)
            {
                return null;
            }
            return new E_Authentication_Email
            {
                AuthFrom = emailEntity.AuthFrom,
                Email = emailEntity.Email,
            };
        }
        /// <summary>
        /// 重置用户密码
        /// todo:后台权限
        /// </summary>
        public CommonActionResult ResetUserPassword(string userId, string userToken)
        {
            // 验证用户身份及权限
           // var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var loginBiz = new LocalLoginBusiness();
            loginBiz.ResetUserPassword(userId);

            return new CommonActionResult(true, "重置用户密码成功");
        }
        /// <summary>
        /// 重置用户账户密码
        /// todo:后台权限
        /// </summary>
        public CommonActionResult ResetUserBalancePwd(string currUserId, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var newPwd = new MobileAuthenticationBusiness().ResetUserBalancePwd(currUserId);
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "重置成功",
                    ReturnValue = newPwd,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 修改用户VIP级别
        /// </summary>
        public CommonActionResult UpdateUserVipLevel(string userId, int vipLevel, string userToken)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
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
        /// 手工充值
        /// </summary>
        public CommonActionResult ManualFillMoney(UserFillMoneyAddInfo info, string userId, string userToken)
        {
            // 验证用户身份及权限
            var requestUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var orderId = string.Empty;
                var vipLevel = 0;
                orderId = new FundBusiness().ManualFillMoney(info, userId, requestUserId, out vipLevel);

                BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, FillMoneyStatus.Success, FillMoneyAgentType.ManualFill, info.RequestMoney, userId, vipLevel });
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工充值完成",
                    ReturnValue = orderId,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("手工充值出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工打款
        /// </summary>
        public CommonActionResult ManualAddMoney(string keyLine, decimal money, AccountType accountType, string userId, string message, string userToken)
        {
            // 验证用户身份及权限
            var requestUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var orderId = string.Empty;
                if (string.IsNullOrEmpty(keyLine))
                {
                    keyLine = BusinessHelper.GetManualFillMoneyId();
                }
                new FundBusiness().ManualHandleMoney(keyLine, keyLine, money, accountType,PayType.Payin, userId, message, requestUserId);
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工打款完成",
                    ReturnValue = orderId,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("手工打款出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工扣款
        /// </summary>
        public CommonActionResult ManualDeductMoney(string keyLine, decimal money, AccountType accountType, string userId, string message, string userToken)
        {
            // 验证用户身份及权限
            var requestUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var orderId = string.Empty;
                if (string.IsNullOrEmpty(keyLine))
                {
                    keyLine = BusinessHelper.GetManualFillMoneyId();
                }
                new FundBusiness().ManualHandleMoney(keyLine, keyLine, money, accountType,PayType.Payout, userId, message, requestUserId);
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工扣款完成",
                    ReturnValue = orderId,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("手工扣款出现错误 - " + ex.Message, ex);
            }
        }
        #endregion
    }
}
