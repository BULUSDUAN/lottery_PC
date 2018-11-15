using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper.ManagerLib;
using EntityModel.ExceptionExtend;
using EntityModel.Redis;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.JSON;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper.WinNumber;
using System.Diagnostics;
using System.Globalization;

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
        public GameList GetGameCodeAndGameType()
        {
            return new A20150919_AddBonusMoney().GetGameCodeAndGameType();
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

        /// <summary>
        /// 查询彩种状态
        /// </summary>
        public LotteryGameInfoCollection QueryLotteryGameList()
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBusiness().LotteryGame();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
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
        public CommonActionResult UpdateLotteryGame(string gameCode, int enableStatus)
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
        public UserSysDataCollection QueryUserList(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? IsUserType, bool? isAgent
            , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize, string userToken, string strOrderBy, int UserCreditType = -1)
        {
            var siteBiz = new LocalLoginBusiness();
            return siteBiz.QueryUserList(regFrom, regTo, keyType, keyValue, isEnable, isFillMoney, IsUserType, isAgent, commonBlance, bonusBlance, freezeBlance, vipRange, comeFrom, agentId, pageIndex, pageSize, strOrderBy, UserCreditType);
        }

        public CommonActionResult DisableUser(string userId, string userToken)
        {
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
        public UserQueryInfo QueryUserByKey(string UserId)
        {
            var siteBiz = new LocalLoginBusiness();
            return siteBiz.QueryUserByKey(UserId, string.Empty);
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
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
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
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
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
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

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
        public CommonActionResult UpdateRealNameAuthentication(string userId, string realName, string idCard, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var biz = new RealNameAuthenticationBusiness();
                biz.UpdateRealNameAuthentication(userId, realName, idCard, myId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 注销实名认证
        /// todo:后台权限
        /// </summary>
        public CommonActionResult LogOffRealNameAuthen(string userId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new RealNameAuthenticationBusiness().LogOffRealNameAuthen(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新手机认证
        /// todo:后台权限
        /// </summary>
        public CommonActionResult UpdateMobileAuthen(string userId, string mobile, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new MobileAuthenticationBusiness().UpdateMobileAuthen(userId, mobile, userToken);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 注销手机认证
        /// todo:后台权限
        /// </summary>
        public CommonActionResult LogOffMobileAuthen(string userId, string userToken)
        {
            try
            {
                new MobileAuthenticationBusiness().LogOffMobileAuthen(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 修改银行卡信息
        /// </summary>
        public CommonActionResult UpdateBankCard(C_BankCard bankCard, string userToken)
        {
            try
            {
                new BankCardBusiness().UpdateBankCard(bankCard, bankCard.UserId);
                return new CommonActionResult(true, "修改银行卡信息成功");
            }
            catch (Exception ex)
            {
                throw new Exception("修改银行卡信息出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 解除银行卡绑定
        /// </summary>
        /// <returns></returns>
        public CommonActionResult CancelBankCard(string userId)
        {
            try
            {
                new BankCardBusiness().CancelBankCard(userId);
                return new CommonActionResult(true, "银行解除绑定成功");
            }
            catch (Exception ex)
            {
                throw new Exception("银行卡解除出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户资金明细
        /// </summary>
        public UserFundDetailCollection QueryUserFundDetailList(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryUserFundDetail(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询彩种
        /// </summary>
        public GameInfoCollection QueryGameList(string userToken)
        {
            try
            {
                return new GameBusiness().QueryGameInfoCollection();
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种出错", ex);
            }
        }
        public BettingOrderInfoCollection QueryBettingOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, bool? isVirtual, string gameCode
            , DateTime startTime, DateTime endTime, int sortType, int pageIndex, int pageSize, string userToken, string fieldName, TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        {
            try
            {
                var agentId = "";
                return new SqlQueryBusiness().QueryBettingOrderList(userIdOrName, schemeType, progressStatus, bonusStatus, betCategory, isVirtual, gameCode, startTime, endTime, sortType, agentId, pageIndex, pageSize, fieldName, ticketStatus, schemeSource);
            }
            catch (Exception ex)
            {
                throw new Exception("查询投注订单列表出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询充值记录
        /// </summary>
        public FillMoneyQueryInfoCollection QueryFillMoneyList(string userKey, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId)
        {
            try
            {
                return new SqlQueryBusiness().QueryFillMoneyList(userKey, agentTypeList, statusList, sourceList, startTime, endTime, pageIndex, pageSize, orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询充值记录出错", ex);
            }
        }
        /// <summary>
        /// 根据key查询配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public C_Core_Config QueryConfigByKey(string key)
        {
            try
            {
                return new Sports_Business().QueryConfigByKey(key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询提现记录列表
        /// </summary>
        public Withdraw_QueryInfoCollection QueryWithdrawList(string userKey, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize, string bankcode)
        {
            try
            {
                return new AdminMenuBusiness().QueryWithdrawList(userKey, agent, status, minMoney, maxMoney, startTime, endTime, sortType, operUserId, pageIndex, pageSize, bankcode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 清理用户绑定数据缓存
        /// </summary>
        public CommonActionResult ManualClearUserBindCache(string userId)
        {
            ClearUserBindInfoCache(userId);
            return new CommonActionResult(true, "清理成功");
        }
        public CommonActionResult ManualBuildUserBindCache(string userId)
        {
            var info = new LocalLoginBusiness().QueryUserBindInfos(userId);
            if (info == null)
                return new CommonActionResult(false, "查询数据失败");

            //添加缓存到文件
            SaveUserBindInfoCache(userId, info);
            return new CommonActionResult(true, "生成成功");
        }
        /// <summary>
        /// 增加银行卡信息
        /// </summary>
        /// <param name="bankCard"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult AddBankCard(C_BankCard bankCard, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var entity = new BankCardManager().BankCardByCode(bankCard.BankCardNumber);
                if (entity != null)
                {
                    throw new Exception("该银行卡号已经被其他用户绑定，请选择其它银行卡号");
                }
                if (string.IsNullOrEmpty(bankCard.UserId) || bankCard.UserId == null || bankCard.UserId.Length == 0)
                    bankCard.UserId = userId;

                var bankcarduser = new BankCardManager().BankCardById(userId);
                if (bankcarduser != null)
                    throw new Exception("您已绑定了银行卡，请不要重复绑定！");
                new BankCardBusiness().AddBankCard(bankCard);
                //绑定银行卡之后实现接口
                BusinessHelper.ExecPlugin<IAddBankCard>(new object[] { bankCard.UserId, bankCard.BankCardNumber, bankCard.BankCode, bankCard.BankName, bankCard.BankSubName, bankCard.CityName, bankCard.ProvinceName, bankCard.RealName });
                return new CommonActionResult(true, "添加银行卡信息成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加银行卡信息出错 - " + ex.Message, ex);
            }
        }
        public bool UpdateUserCreditType(string userId, int updateUserCreditType)
        {
            return new FundBusiness().UpdateUserCreditType(userId, updateUserCreditType);
        }
        public CommonActionResult AuthenticateUserRealName_BackSite(string userId, string realName, string idCard, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                var biz = new RealNameAuthenticationBusiness();
                biz.AddAuthenticationRealName("LOCAL", userId, realName, "0", idCard, myId, false);
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IResponseAuthentication_AfterTranCommit>(new object[] { userId, "RealName", realName + "|0|" + idCard, SchemeSource.Web });
                return new CommonActionResult(true, "实名认证成功。");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "实名认证失败。"+ ex.Message);
            }
        }
        /// <summary>
        /// 用户手机认证
        /// todo:后台权限
        /// </summary>
        public CommonActionResult AuthenticationUserMobile(string userId, string mobile, SchemeSource source, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var validateCode = GetRandomMobileValidateCode();

                    var authenticationBiz = new MobileAuthenticationBusiness();
                    var mobileInfo = authenticationBiz.GetAuthenticatedMobile(userId);
                    authenticationBiz.AddAuthenticationMobile("LOCAL", userId, mobile, myId);
                
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<IResponseAuthentication_AfterTranCommit>(new object[] { userId, "Mobile", mobile, source });

                return new CommonActionResult(true, "手机认证成功。") { ReturnValue = validateCode };
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        private string GetRandomMobileValidateCode()
        {
            var validateCode = "8888";
            if (!UsefullHelper.IsInTest)
            {
                // 生成随机密码
                Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
                validateCode = random.Next(100, 999999).ToString().PadLeft(6, '0');
                //return RndNum(6);
            }
            return validateCode;
        }
        /// <summary>
        /// 清理用户绑定数据缓存
        /// </summary>
        private void ClearUserBindInfoCache(string userId)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelperEx.DB_UserBindData;
                db.Del(fullKey);
            }
            catch (Exception)
            {
            }
          
        }
        /// <summary>
        /// 保存用户绑定数据的缓存
        /// </summary>
        private void SaveUserBindInfoCache(string userId, UserBindInfos info)
        {

            try
            {
                var content = JsonSerializer.Serialize<UserBindInfos>(info);
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelperEx.DB_UserBindData;
                db.SetAsync(fullKey, content);
            }
            catch (Exception ex)
            {
                throw new LogicException("QueryUserBindInfos_Write:" + userId, ex);
            }
        }
        /// <summary>
        /// 用户添加qq
        /// </summary>
        public CommonActionResult AddUserQQ(string userId, string qq)
        {
            try
            {
                new QQAuthenticationBusiness().AddUserQQ(userId, qq);

                //清理用户绑定数据缓存
                ClearUserBindInfoCache(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 后台取消绑定QQ号码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public CommonActionResult CancelUserQQ(string userId)
        {
            try
            {
                new QQAuthenticationBusiness().CancelUserQQ(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 后台会员详情，更新经销商
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agentId"></param>
        public CommonActionResult UpdateUserAgentId(string userId, string agentId, string userToken)
        {
            try
            {
                new OCAgentBusiness().UpdateUserAgentId(userId, agentId);
                return new CommonActionResult(true, "更新经销商成功");
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
        /// <summary>
        /// 绑定支付宝
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="alipay"></param>
        /// <returns></returns>
        public CommonActionResult AddUserAlipay(string userId, string alipay)
        {
            try
            {
                new AlipayAuthenticationBusiness().AddUserAlipay(userId, alipay);

                //清理用户绑定数据缓存
                ClearUserBindInfoCache(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 后台解除绑定支付宝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CommonActionResult CancelUserAlipay(string userId)
        {
            try
            {
                new AlipayAuthenticationBusiness().CancelUserAlipay(userId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工完成充值
        /// </summary>
        public CommonActionResult ManualCompleteFillMoneyOrder(string orderId, FillMoneyStatus status, string myId)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            string userId = "";
            try
            {
                var vipLevel = 0;
                FillMoneyAgentType agentType;
                decimal money;
                //! 执行扩展功能代码 - 启动事务后
                userId = new FundBusiness().ManualCompleteFillMoneyOrder(orderId, status, out agentType, out money, out vipLevel, myId);
                //! 执行扩展功能代码 - 提交事务后
                BusinessHelper.ExecPlugin<ICompleteFillMoney_AfterTranCommit>(new object[] { orderId, status, agentType, money, userId, vipLevel });
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "手工完成充值成功",
                };
            }
            catch (LogicException ex)
            {
                //! 执行扩展功能代码 - 发生异常
                BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, userId, ex });
                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "充值订单重复处理",
                };
            }
            catch (Exception ex)
            {
                //! 执行扩展功能代码 - 发生异常
                BusinessHelper.ExecPlugin<ICompleteFillMoney_OnError>(new object[] { orderId, status, userId, ex });

                throw new Exception("手工完成充值 出现错误 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询提现记录列表execl
        /// </summary>
        public Withdraw_QueryInfoCollection QueryWithdrawList2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status, string userToken)
        {
            try
            {
                return new AdminMenuBusiness().QueryWithdrawList2(minMoney, maxMoney, startTime, endTime, status);
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 成功提现
        /// </summary>
        public CommonActionResult CompleteWithdraw(string orderId, string responseMsg, string userId)
        {
            try
            {
                //! 执行扩展功能代码 - 启动事务后
                new ExternalIntegralBusiness().CompleteWithdraw(orderId, responseMsg, userId);
                return new CommonActionResult(true, "完成提现成功");
            }
            catch (Exception ex)
            {
                throw new Exception("完成提现出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询提现记录
        /// </summary>
        public Withdraw_QueryInfo GetWithdrawById(string orderId)
        {
            try
            {
                return new FundBusiness().GetWithdrawById(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询提现记录列表 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 拒绝提现
        /// </summary>
        public CommonActionResult RefusedWithdraw(string orderId, string refusedMsg, string userId)
        {
            try
            {
                //! 执行扩展功能代码 - 启动事务后
                new ExternalIntegralBusiness().RefusedWithdraw(orderId, refusedMsg, userId);
                return new CommonActionResult(true, "拒绝提现成功");
            }
            catch (Exception ex)
            {
                throw new Exception("拒绝提现出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 获取系统角色
        /// todo:后台权限
        /// </summary>
        public RoleInfo_QueryCollection GetSystemRoleCollection(string userToken)
        {
            var authBiz = new GameBizAuthBusiness();
            return authBiz.GetSystemRoleCollection();
        }
        /// <summary>
        /// 发送站内信
        /// todo:后台权限
        /// </summary>
        public CommonActionResult SendInnerMail(InnerMailInfo_Send innerMail, string userId)
        {
            var siteBiz = new SiteMessageControllBusiness();
            siteBiz.SendInnerMail(innerMail, userId);

            return new CommonActionResult { IsSuccess = true, Message = "发送站内信成功", };
        }
        /// <summary>
        /// 根据角色编号查询用户编号
        /// </summary>
        public string QueryUserIdByRoleId(string roleId)
        {
            try
            {
                return new SiteMessageControllBusiness().QueryUserIdByRoleId(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询站点信息参数
        /// </summary>
        public string QuerySiteMessageTags()
        {
            return new SiteMessageControllBusiness().QuerySiteMessageTags();
        }
        /// <summary>
        /// 查询网站通知配置
        /// </summary>
        public SiteMessageSceneInfoCollection QuerySiteNoticeConfig()
        {
            return new SiteMessageControllBusiness().QuerySiteNoticeConfig();
        }
        public CommonActionResult UpdateSiteNotice(string key, SiteMessageCategory category, string title, string content)
        {
            try
            {
                new SiteMessageControllBusiness().UpdateSiteNotice(key, category, title, content);
                return new CommonActionResult(true, "更新完成");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 查询发送短信记录
        /// </summary>
        public MoibleSMSSendRecordInfoCollection QuerySMSSendRecordList(string userId, string mobileNumber, DateTime startTime, DateTime endTime, string status, int pageIndex, int pageSize)
        {
            return new SiteMessageControllBusiness().QuerySMSSendRecordList(userId, mobileNumber, startTime, endTime, status, pageIndex, pageSize);
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        public CommonActionResult SendSMS(string mobile, string content, string userId)
        {
            try
            {
                new SiteMessageControllBusiness().SendSMS(mobile, content, userId);
                return new CommonActionResult(true, "发送完成");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 根据手机号码查询手机验证信息
        /// todo:后台权限
        /// </summary>
        public ValidationMobileInfoCollection QueryValidationMobileByMobile(string mobile, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            var validationMobile = new ValidationMobileInfoCollection();
            var siteBiz = new ValidationMobileBusiness();
            var list = siteBiz.QueryValidationMobileByMobile(mobile);
            ObjectConvert.ConvertEntityListToInfoList<IList<E_Validation_Mobile>, E_Validation_Mobile, ValidationMobileInfoCollection, ValidationMobileInfo>(
                list, ref validationMobile, () => new ValidationMobileInfo());
            return validationMobile;
        }
        public CommonActionResult AddOCAgent(OCAgentCategory category, string parentUserId, string userId, CPSMode cpsmode, string channelName)
        {
            try
            {
                new OCAgentBusiness().AddOCAgent(category, parentUserId, userId, cpsmode);
                return new CommonActionResult(true, "添加下级成功");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加红人
        /// </summary>
        public CommonActionResult AddTogetherHotUser(string userId)
        {
            try
            {
                new Sports_Business().AddTogetherHotUser(userId);
                return new CommonActionResult(true, "添加成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 删除合买红人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CommonActionResult DeleteTogetherHotUser(string userId)
        {
            try
            {
                new Sports_Business().DeleteTogetherHotUser(userId);
                return new CommonActionResult(true, "删除合买红人成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 新增宝单或大单推荐专家
        /// </summary>
        public CommonActionResult AddUserSchemeShareExpert(string userId, int shortIndex, CopyOrderSource source)
        {
            try
            {
                new BDFXOrderBusiness().AddUserSchemeShareExpert(userId, shortIndex, source);
                return new CommonActionResult(true, "保存数据成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 删除宝单或大单推荐专家
        /// </summary>
        public CommonActionResult DeleteUserSchemeShareExpert(string id)
        {
            try
            {
                new BDFXOrderBusiness().DeleteUserSchemeShareExpert(id);
                return new CommonActionResult(true, "删除数据成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询宝单、大单推荐专家
        /// </summary>
        public UserSchemeShareExpert_Collection QueryUserSchemeShareExpertList(string userKey, int source, int pageIndex, int pageSize)
        {
            try
            {
                return new BDFXOrderBusiness().QueryUserSchemeShareExpertList(userKey, source, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询代理销售充值注册量明细汇总
        /// </summary>
        public OCAagentDetailInfoCollection QueryAgentDetail(string agentId, string gameCode, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, bool isRecharge)
        {
            try
            {
                return new OCAgentBusiness().QueryAgentDetail(agentId, gameCode, starTime, endTime, pageIndex, pageSize, isRecharge);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        /// <summary>
        /// 根据方案号更新单个订单的过关统
        /// </summary>
        public CommonActionResult Update_CTZQ_HitCountBySchemeId(string schemeId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().Update_CTZQ_HitCountBySchemeId(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


       
        /// <summary>
        /// 删除订单缓存
        /// </summary>
        public CommonActionResult ManualDeleteOrderCache(string schemeId)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //删除缓存文件
                new CacheDataBusiness().DeleteSchemeInfoXml(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 手工返点
        /// </summary>
        /// <param name="schemeId"></param>
        public CommonActionResult ManualAgentPayIn(string schemeId)
        {
            try
            {
                new OCAgentBusiness().AgentPayIn_CompateOrder(schemeId);
                //new SiteMessageBusiness().ManualAgentPayIn(schemeId);
                return new CommonActionResult(true, "手工返点成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        //按keyline查询追号列表
        public BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string keyLine)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var collection = new SqlQueryBusiness().QueryBettingOrderListByChaseKeyLine(keyLine);
                if (collection != null && collection.OrderList != null && collection.OrderList.Count > 0)
                {
                    collection.TotalCount = collection.OrderList.Count;
                    collection.TotalOrderMoney = collection.OrderList.Sum(o => o.TotalMoney);
                    collection.TotalBuyMoney = collection.OrderList.Sum(o => o.CurrentBettingMoney);
                    collection.TotalPreTaxBonusMoney = collection.OrderList.Sum(o => o.PreTaxBonusMoney);
                    collection.TotalAfterTaxBonusMoney = collection.OrderList.Sum(o => o.AfterTaxBonusMoney);
                    collection.TotalAddMoney = collection.OrderList.Sum(o => o.AddMoney);
                    collection.TotalUserCount = 1;
                }
                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("查询追号列表失败 - " + ex.Message, ex);
            }
        }

        //查询指定订单的投注号码列表
        public BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().QueryAnteCodeListBySchemeId(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询投注号码列表失败 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询足彩方案信息
        /// </summary>
        public Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId)
        {
            try
            {
                return new Sports_Business().QuerySportsSchemeInfo(schemeId);

                //var cacheBiz = new CacheDataBusiness();
                //var info = cacheBiz.QuerySports_SchemeQueryInfo(schemeId);
                //if (info == null)
                //{
                //    info = new Sports_Business().QuerySportsSchemeInfo(schemeId);
                //    if (info != null && (info.BonusStatus == BonusStatus.Lose || (info.BonusStatus == BonusStatus.Win && info.IsPrizeMoney)))
                //    {
                //        cacheBiz.SaveSchemeInfoToXml(info);
                //    }
                //}
                //return info;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询足彩订单投注列表
        /// </summary>
        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QuerySportsOrderAnteCodeList(schemeId);

                //var cacheBiz = new CacheDataBusiness();
                //var collection = cacheBiz.QuerySports_AnteCodeQueryInfoCollection(schemeId);
                //if (collection == null)
                //{
                //    collection = new Sports_Business().QuerySportsOrderAnteCodeList(schemeId);
                //    var grp = collection.GroupBy(a => a.BonusStatus);
                //    if (grp.Count(g => g.Key != BonusStatus.Lose && g.Key != BonusStatus.Win) == 0 && collection.ToList().Count() > 0)
                //    {
                //        cacheBiz.SaveSchemeAnteCodeCollectionToXml(schemeId, collection);
                //    }
                //}
                //return collection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 订单投注失败返钱
        /// </summary>
        public CommonActionResult BetFail(string schemeId)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().BetFail(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败 " + ex.Message, ex);
            }
        }

        public CommonActionResult ManualBet(string orderId)
        {
            new Sports_Business().ManualBet(orderId);
            return new CommonActionResult(true, "投注出票完成");
        }

        public CommonActionResult ManualPrizeOrder(string orderId)
        {
            new Sports_Business().ManualPrizeOrder(orderId);
            return new CommonActionResult(true, "票数据派奖完成");
        }
        #region 手工处理订单相关

        /// <summary>
        /// 移动中的订单数据到完成订单数据
        /// </summary>
        public CommonActionResult MoveRunningOrderToComplateOrder(string schemeId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().MoveRunningOrderToComplateOrder(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工设置订单中奖数据
        /// </summary>
        public CommonActionResult ManualSetOrderBonusMoney(string schemeId, decimal bonusMoney, int bonusCount, int hitMatchCount, string bonusCountDescription, string bonusCountDisplayName)
        {
            // 验证用户身份及权限
        
            try
            {
                new Sports_Business().ManualSetOrderBonusMoney(schemeId, bonusMoney, bonusCount, hitMatchCount, bonusCountDescription, bonusCountDisplayName);
                //删除缓存文件
                new CacheDataBusiness().DeleteSchemeInfoXml(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工设置订单为不中奖
        /// </summary>
        public CommonActionResult ManualSetOrderNotBonus(string schemeId, int hitMatchCount)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().ManualSetOrderNotBonus(schemeId, hitMatchCount);
                //删除缓存文件
                new CacheDataBusiness().DeleteSchemeInfoXml(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 分析合买
        /// </summary>
        public CommonActionResult AnalysisSchemeTogether(string schemeId)
        {
            try
            {
                //var createUserId = string.Empty;
                //var gameCode = string.Empty;
                //var gameType = string.Empty;
                //var payBackMoney = 0M;
                //var canChase = new Sports_Business().AnalysisSchemeTogether(schemeId, out canPayBack, out createUserId, out gameCode, out gameType, out payBackMoney);
                new Sports_Business().AnalysisSchemeTogether(schemeId);
                //SportsChase(schemeId);//分析合买失败后直接撤单，不投注

                return new CommonActionResult(true, "分析合买成功");
            }
            catch (Exception ex)
            {
                throw new Exception("分析合买异常 - " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 查询订单票数据
        /// </summary>
        public Sports_TicketQueryInfoCollection QuerySportsTicketList(string schemeId, int pageIndex, int pageSize)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var collection = new Sports_Business().QuerySchemeTicketList(schemeId, pageIndex, pageSize);

                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 手工修改中奖状态
        /// </summary>
        public CommonActionResult UpdateSchemeTicket(string ticketId, BonusStatus bonusStatus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new Sports_Business().UpdateSchemeTicket(ticketId, bonusStatus, preTaxBonusMoney, afterTaxBonusMoney);
                return new CommonActionResult(true, "修改票数据成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询等待派钱的订单列表
        /// </summary>
        public Sports_SchemeQueryInfoCollection QueryWaitForPrizeMoneyOrderList(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryWaitForPrizeMoneyOrderList(startTime, endTime, gameCode, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询等待派钱的订单列表异常 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 足彩派钱
        /// </summary>
        public CommonActionResult SportsPrizeMoney(string schemeIdArray)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var array = schemeIdArray.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var list = new Sports_Business().SportsPrizeMoney(array);

                foreach (var item in list)
                {
                    BusinessHelper.ExecPlugin<IOrderPrizeMoney_AfterTranCommit>(new object[] {item.UserId, item.SchemeId, item.GameCode, item.GameType, item.IssuseNumber,
                        item.TotalMoney, item.PreTaxBonusMoney, item.AfterTaxBonusMoney  });
                }

                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("方案派钱异常 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 计算用户战绩
        /// </summary>
        public CommonActionResult ComputeUserBeedings(string complateDate)
        {
            try
            {
                new Sports_Business().ComputeUserBeedings(complateDate);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("计算用户战绩异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 计算用户幸运指数
        /// </summary>
        public CommonActionResult ComputeLucyUser()
        {
            try
            {
                new Sports_Business().ComputeLucyUser();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("计算用户幸运指数异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 计算用户中奖概率
        /// </summary>
        public CommonActionResult ComputeBonusPercent()
        {
            try
            {
                new Sports_Business().ComputeBonusPercent();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("计算中奖概率异常 - " + ex.Message, ex);
            }
        }


        public static object importLocker = new object();
        public CommonActionResult ImportWinNumber(string gameCode, string issuseNumber, string winNumber)
        {
            try
            {
                lock (importLocker)
                {
                    ILotteryDataBusiness biz = null;
                    switch (gameCode)
                    {
                        case "SSQ":
                            biz = new LotteryDataBusiness_SSQ();
                            break;
                        case "DLT":
                            biz = new LotteryDataBusiness_DLT();
                            break;
                        case "FC3D":
                            biz = new LotteryDataBusiness_FC3D();
                            break;
                        case "PL3":
                            biz = new LotteryDataBusiness_PL3();
                            break;
                        case "CQSSC":
                            biz = new LotteryDataBusiness_CQSSC();
                            break;
                        case "JX11X5":
                            biz = new LotteryDataBusiness_JX11X5();
                            break;
                        case "CQ11X5":
                            biz = new LotteryDataBusiness_CQ11X5();
                            break;
                        case "CQKLSF":
                            biz = new LotteryDataBusiness_CQKLSF();
                            break;
                        case "DF6J1":
                            biz = new LotteryDataBusiness_DF6_1();
                            break;
                        case "GD11X5":
                            biz = new LotteryDataBusiness_GD11X5();
                            break;
                        case "GDKLSF":
                            biz = new LotteryDataBusiness_GDKLSF();
                            break;
                        case "HBK3":
                            biz = new LotteryDataBusiness_HBK3();
                            break;
                        case "HC1":
                            biz = new LotteryDataBusiness_HC1();
                            break;
                        case "HD15X5":
                            biz = new LotteryDataBusiness_HD15X5();
                            break;
                        case "HNKLSF":
                            biz = new LotteryDataBusiness_HNKLSF();
                            break;
                        case "JLK3":
                            biz = new LotteryDataBusiness_JLK3();
                            break;
                        case "JSKS":
                            biz = new LotteryDataBusiness_JSK3();
                            break;
                        case "JXSSC":
                            biz = new LotteryDataBusiness_JXSSC();
                            break;
                        case "LN11X5":
                            biz = new LotteryDataBusiness_LN11X5();
                            break;
                        case "PL5":
                            biz = new LotteryDataBusiness_PL5();
                            break;
                        case "QLC":
                            biz = new LotteryDataBusiness_QLC();
                            break;
                        case "QXC":
                            biz = new LotteryDataBusiness_QXC();
                            break;
                        case "SDQYH":
                            biz = new LotteryDataBusiness_SDQYH();
                            break;
                        case "SD11X5":
                            biz = new LotteryDataBusiness_YDJ11();
                            break;
                        case "SDKLPK3":
                            biz = new LotteryDataBusiness_SDKLPK3();
                            break;
                        case "CTZQ_T14C":
                        case "CTZQ_TR9":
                        case "CTZQ_T6BQC":
                        case "CTZQ_T4CJQ":
                            biz = new LotteryDataBusiness_CTZQ(gameCode);
                            break;
                        default:
                            throw new Exception(string.Format("未找到匹配的接口：{0}", gameCode));
                    }
                    biz.ImportWinNumber(issuseNumber, winNumber);
                }
                return new CommonActionResult(true, "导入成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, "导入失败 " + ex.ToString());
            }
        }

        /// <summary>
        /// 奖期派奖
        /// </summary>
        public CommonActionResult IssusePrize(string gameCode, string gameType, string issuseNumber, string winNumber)
        {
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                //奖期派奖
                new Sports_Business().LotteryIssusePrize(gameCode, gameType, issuseNumber, winNumber);
                watch.Stop();

                return new CommonActionResult(true, string.Format("奖期派奖完成,用时{0}毫秒", watch.Elapsed.TotalMilliseconds));
            }
            catch (Exception ex)
            {
                throw ex;
                //watch.Stop();
                //return new CommonActionResult(true, string.Format("{0},用时{1}毫秒", ex.Message, watch.Elapsed.TotalMilliseconds));
            }
        }

        // 批量开启重庆时时彩奖期（每天120期。白天10分钟一期(上午10点开始)，夜场5分钟一期(22点开始)）
        public CommonActionResult OpenIssuseBatch_Fast(string gameCode, DateTime dateFrom, DateTime dateTo)
        {
            switch (gameCode)
            {
                case "CQSSC":
                    var bettingOffset = 90;
                    var phases = new Dictionary<int, double>();
                    phases.Add(23, 5);
                    phases.Add(0, 475);
                    phases.Add(96, 10);
                    phases.Add(120, 5);
                    var issuseFormat = "{0:yyyyMMdd}-{1,3:D3}";

                    var admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
                    }
                    return new CommonActionResult(true, "操作成功");

                case "JX11X5":
                    bettingOffset = 120;
                    phases = new Dictionary<int, double>();
                    phases.Add(0, 540);
                    phases.Add(84, 10);
                    issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

                    admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
                    }
                    return new CommonActionResult(true, "操作成功");

                case "SD11X5":

                    bettingOffset = 40;
                    phases = new Dictionary<int, double>();
                    phases.Add(0, 506);
                    phases.Add(87, 10);
                    issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

                    admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
                    }
                    return new CommonActionResult(true, "操作成功");

                case "GD11X5":

                    bettingOffset = 30;
                    phases = new Dictionary<int, double>();
                    phases.Add(0, 540);
                    phases.Add(84, 10);
                    issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

                    admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
                    }
                    return new CommonActionResult(true, "操作成功");
                case "GDKLSF:":

                    bettingOffset = 30;
                    phases = new Dictionary<int, double>();
                    phases.Add(0, 510);
                    phases.Add(84, 10);
                    issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

                    admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
                    }
                    return new CommonActionResult(true, "操作成功");
                case "JSKS":

                    bettingOffset = 30;
                    phases = new Dictionary<int, double>();
                    phases.Add(0, 510);
                    phases.Add(82, 10);
                    issuseFormat = "{0:yyyyMMdd}-{1,2:D2}";

                    admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat);
                    }
                    return new CommonActionResult(true, "操作成功");

                case "SDKLPK3":


                    bettingOffset = 40;
                    phases = new Dictionary<int, double>();
                    phases.Add(0, 501);
                    phases.Add(88, 10);
                    issuseFormat = "{0:yyyyMMdd}-{1,2:D2}"; //"{0:yyyy}{2,3:D3}-{1,2:D2}";

                    admin = new IssuseBusiness();
                    for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
                    {
                        var dateIndex = GetDayIndex(date);
                        admin.OpenIssuseBatch_Fast(gameCode, date, bettingOffset, CheckIsOpenDay, phases, issuseFormat, dayIndex: dateIndex);
                    }
                    return new CommonActionResult(true, "操作成功");

                default:

            throw new ArgumentException("彩种不正确 - " + gameCode);

            }
        }

        public CommonActionResult OpenIssuseBatch_Daily(string gameCode, int yearFrom, int yearTo)
        {
            switch (gameCode)
            {
                case "FC3D":
                  
                    var issuseFormat = "{0:yyyy}-{1,3:D3}";
                    var admin = new IssuseBusiness();
                    for (var year = yearFrom; year <= yearTo; year++)
                    {
                        admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
                    }
                    return new CommonActionResult(true, "操作成功");

                case "PL3":

                     gameCode = "PL3";
                     issuseFormat = "{0:yyyy}-{1,3:D3}";
                     admin = new IssuseBusiness();
                    for (var year = yearFrom; year <= yearTo; year++)
                    {
                        admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, CheckIsOpenDay, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
                    }
                    return new CommonActionResult(true, "操作成功");
                default:

                   throw new ArgumentException("彩种不正确 - " + gameCode);
            }
        }

        /// <summary>
        /// 批量开启奖期
        /// </summary>
        public CommonActionResult OpenIssuseBatch_Slow(string gameCode, int yearFrom, int yearTo)
        {
            switch (gameCode)
            {
                case "SSQ":
                  
                    var issuseFormat = "{0:yyyy}-{1,3:D3}";
                    var admin = new IssuseBusiness();
                    for (var year = yearFrom; year <= yearTo; year++)
                    {
                        admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
                        {
                            if ((d.DayOfWeek == DayOfWeek.Tuesday || d.DayOfWeek == DayOfWeek.Thursday || d.DayOfWeek == DayOfWeek.Sunday))
                            {
                                return CheckIsOpenDay(d);
                            }
                            return false;
                        }, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
                    }
                    return new CommonActionResult(true, "操作成功");

                case "DLT":

                     issuseFormat = "{0:yyyy}-{1,3:D3}";
                     admin = new IssuseBusiness();
                    for (var year = yearFrom; year <= yearTo; year++)
                    {
                        admin.OpenIssuseBatch_Slow(gameCode, year, issuseFormat, (d) =>
                        {
                            if ((d.DayOfWeek == DayOfWeek.Tuesday || d.DayOfWeek == DayOfWeek.Thursday || d.DayOfWeek == DayOfWeek.Sunday))
                            {
                                return CheckIsOpenDay(d);
                            }
                            return false;
                        }, (d) => d.Date.AddHours(19).AddMinutes(50), 10);
                    }
                    return new CommonActionResult(true, "操作成功");

                default:

                    throw new ArgumentException("彩种不正确 - " + gameCode);
            }
        }
        private bool CheckIsOpenDay(DateTime date)
        {
            var calendar = new ChineseLunisolarCalendar();
            var lMonth = calendar.GetMonth(date);
            var lDay = calendar.GetDayOfMonth(date);
            if (lMonth == 1 && lDay < 7)
            {
                return false;
            }
            else
            {
                date = date.AddDays(1);
                lMonth = calendar.GetMonth(date);
                lDay = calendar.GetDayOfMonth(date);
                if (lMonth == 1 && lDay == 1)
                {
                    return false;
                }
            }
            return true;
        }

        #region 高频彩开启奖期

        private int GetDayIndex(DateTime date)
        {
            var dateIndex = 0;
            var currentDay = new DateTime(date.Year, 1, 1);
            for (var i = 0; i < date.DayOfYear; i++)
            {
                currentDay = currentDay.AddDays(1);
                if (CheckIsOpenDay(currentDay))
                {
                    dateIndex++;
                }
            }
            return dateIndex;
        }
        #endregion

        /// <summary>
        /// 按彩种查询未派奖的票并执行派奖
        /// </summary>
        public string QueryUnPrizeTicketAndDoPrizeByGameCode(string gameCode, string gameType, int count)
        {
            return new Sports_Business().QueryUnPrizeTicketAndDoPrizeByGameCode(gameCode, gameType, count);
        }

        /// <summary>
        /// 根据订单号派奖
        /// </summary>
        public CommonActionResult PrizeTicket_OrderId(string gameCode, string orderId)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "BJDC":
                        new TicketGatewayAdmin().PrizeBJDCTicket_OrderId(orderId);
                        break;
                    case "JCZQ":
                        new TicketGatewayAdmin().PrizeJCZQTicket_OrderId(orderId);
                        break;
                    case "JCLQ":
                        new TicketGatewayAdmin().PrizeJCLQTicket_OrderId(orderId);
                        break;
                    case "CTZQ":
                        new Sports_Business().ManualPrizeOrder(orderId);
                        break;
                    default:
                        throw new Exception("派奖彩种异常" + gameCode.ToUpper());
                }
                string str = new Sports_Business().DoOrderPrize(orderId);
                return new CommonActionResult(true, str);
            }
            catch (Exception ex)
            {
                return new CommonActionResult(true, "处理数据过程发生异常 - " + ex.Message);
            }
        }

        /// <summary>
        /// 未开出对应彩种奖期开奖号，按订单本金归还
        /// </summary>
        public string QueryOrderAndFundOrder(string gameCode, string issuse)
        {
            new Sports_Business().LotteryIssusePrize(gameCode, string.Empty, issuse, "-");
            //new PrizeBusiness().IssusePrize(gameCode, issuse, string.Empty);
            return "奖期更新完成";
        }


        /// <summary>
        /// 查询统计会员分布 前十名省会注册 实名 绑定卡的
        /// </summary>
        /// <returns></returns>
        public MemberSpreadInfoCollection QueryMemberSpread()
        {
           
            try
            {
                return new GameBusiness().QueryMemberSpread();
            }
            catch (Exception ex)
            {
                throw new Exception("查询统计会员分布出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询统计充值提现信息（按月统计）
        /// </summary>
        /// <returns></returns>
        public FillMoneyWithdrawInfoCollection FillMoneyWithdrawInfo()
        {
           
            try
            {
                return new GameBusiness().FillMoneyWithdrawInfo();
            }
            catch (Exception ex)
            {
                throw new Exception("查询统计充值提现信息 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 查询总平台、pc、安卓、ios、wap当天的注册人数、实名人数、付费人数
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public MemberTotalCollection QueryMemberTotal()
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBusiness().QueryMemberTotal();
               
            }
            catch (Exception ex)
            {
                throw new Exception("查询统计会员分布出错 - " + ex.Message, ex);
            }
        }

        public bool CheckIsAuthenticationFunction(string functionId, string needRight, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                string userId;
                GameBizAuthBusiness.ValidateAuthentication(userToken, needRight, functionId, out userId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查询站点描述信息
        /// </summary>
        public SiteSummaryInfo QuerySiteSummary()
        {
        
            try
            {
                return new GameBusiness().QuerySiteSummary();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
