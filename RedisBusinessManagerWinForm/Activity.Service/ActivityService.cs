using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Business;
using Common;
using GameBiz.Auth.Business;
using Common.Business;
using Common.Utilities;
using GameBiz.Core;
using Activity.Core;
using Activity.Business;
using GameBiz.Domain.Managers;

namespace Activity.Service
{
    public partial class ActivityService : WcfService
    {
        public ActivityService()
        {
            KnownTypeRegister.RegisterKnownTypes(CommunicationObjectGetter.GetCommunicationObjectTypes());
        }


        #region 优惠券相关

        /// <summary>
        /// 生成优惠券
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="money"></param>
        /// <param name="count"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult BuildCoupons(string summary, decimal money, int count, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new A20131105().BuildCoupons(summary, money, count);
                return new CommonActionResult(true, "生成完成");
            }
            catch (Exception ex)
            {
                throw new Exception("生成优惠券出错 - " + ex.ToString());
            }
        }
        /// <summary>
        /// 使用优惠券
        /// </summary>
        /// <param name="couponsNumber"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult ExchangeCoupons(string couponsNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                new A20131105().ExchangeCoupons(userId, couponsNumber);
                return new CommonActionResult(true, "兑换成功");

            }
            catch (Exception ex)
            {
                throw new Exception("兑换优惠券出错 - " + ex.Message);
            }
        }
        /// <summary>
        /// 查询优惠券
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="canUsable"></param>
        /// <param name="belongUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20131105CouponsInfoCollection QueryCouponsList(string summary, bool? canUsable, string belongUserId, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new A20131105().QueryCouponsList(summary, canUsable, belongUserId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询优惠券出错 - " + ex.Message);
            }
        }

        #endregion

        #region 活动列表

        /// <summary>
        /// 查询传统足球_14场_任九_加奖
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public A20121009Info_Collection QueryA20121009Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20121009Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 二期活动_奖金转入送1个点
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public A20120925Info_Collection QueryA20120925Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20120925Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 充值送钱_认证后充200送50_充值送百分之10
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20130807Info_Collection QueryA20130807Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20130807Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 实名手机认证送3元
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20130808Info_Collection QueryA20130808Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20130808Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 用户完成手机认证以及实名认证，首次充值大于20赠送10元
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20120925CZGiveMoneyInfo_Collection QueryA20120925CZGiveMoneyInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20120925CZGiveMoneyInfo(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 竞彩足球加奖
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20130903Info_Collection QueryA20130903Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20130903Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 转运红包
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20130903Info_Collection QueryA20130903ZYHBInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20130903ZYHBInfo(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 用户返点
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public ActivityMonthReturnPointInfo_Colleciton QueryA20131101YHFDInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20131101YHFDInfo(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 新用户首充送钱
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="IsGive"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20131101InfoCollection QueryA20131101NewUserCZInfo(string userId, int IsGive, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20131101NewUserCZInfo(userId, IsGive, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 首次中奖超过100送5元
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20121128Info_Colleciton QueryA20121128Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20121128Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 首次充20送11
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public A20140214Info_Collection QueryA20140214Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20140214Info(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 时时彩红包
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public A20140214SSCHBInfo_Collection QueryA20140214SSCHBInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new ActivityBusiness().QueryA20140214SSCHBInfo(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// 豆豆兑换奖品
        /// </summary>
        public CommonActionResult ExchangeDouDou(string userId, int doudou, string password)
        {
            try
            {
                new A20140731_DouDouBusiness().ExchangeDouDou(userId, doudou, password);
                return new CommonActionResult(true, "豆豆兑换奖品成功");
            }
            catch (Exception ex)
            {
                throw new Exception("执行豆豆兑换奖品出错 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询豆豆列表
        /// </summary>
        public A20140731_DouDouInfoCollection QueryDouDouList(bool isGive, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new A20140731_DouDouBusiness().QueryDouDouList(isGive, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询出错 - " + ex.ToString());
            }
        }

        /// <summary>
        /// 修改领取奖品状态
        /// </summary>
        public CommonActionResult ToGetThePrize(int id, string userId)
        {
            try
            {
                new A20140731_DouDouBusiness().ToGetThePrize(id, userId);
                return new CommonActionResult(true, "修改领取奖品状态成功");
            }
            catch (Exception ex)
            {
                throw new Exception("执行修改领取奖品状态出错 - " + ex.ToString());
            }
        }


        #region 网站活动配置相关 2015.09.19

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
        /// 更新网站活动配置
        /// </summary>
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
        /// 修改充值赠送红包配置
        /// </summary>
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

        /// <summary>
        /// 查询加奖配置
        /// </summary>
        public AddBonusMoneyConfigInfoCollection QueryAddBonusMoneyConfig()
        {
            return new A20150919_AddBonusMoney().QueryAddBonusMoneyConfig();
        }

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

        /// <summary>
        /// 查询红包使用规则
        /// </summary>
        public string QueryRedBagUseConfig()
        {
            return new A20150919_RedBagUseConfig().QueryRedBagUseConfig();
        }

        public decimal QueryRedBagUseConfigByGameCode(string gameCode)
        {
            return new A20150919_RedBagUseConfig().QueryRedBagUseConfig(gameCode);
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

        /// <summary>
        /// 加载红包使用规则到Redis
        /// </summary>
        public CommonActionResult LoadRedBagUseConfigToRedis()
        {
            new A20150919_RedBagUseConfig().LoadRedBagUseConfigToRedis();
            return new CommonActionResult(true, "加载完成");
        }

        /// <summary>
        /// 查询彩种红包使用配置
        /// </summary>
        public decimal QueryRedBagUseConfigFromRedis(string gameCode)
        {
            return new A20150919_RedBagUseConfig().QueryRedBagUseConfigFromRedis(gameCode);
        }

        public CommonActionResult DoGiveAddBonusMoney(string schemeId)
        {
            new A20150919_AddBonusMoney().DoGiveAddMoney(schemeId);
            return new CommonActionResult(true, "执行成功");
        }

        public CommonActionResult ManualAddMoney(string schemeId)
        {
            new A20150919_AddBonusMoney().ManualAddMoney(schemeId);
            return new CommonActionResult(true, "执行成功");
        }

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


        #region 手机登录红包
        /// <summary>
        /// 手机登录红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string MobileLoginRed(string userId)
        {
            string msg = new A20150919_BindRealName_Mobile_Login().MobileLoginRed(userId);
            return msg;
          
        }
        #endregion

        /// <summary>
        /// 根据key获取配置的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string QueryActivityConfig(string key) 
        {
            return ActivityCache.QueryActivityConfig(key).ConfigValue;
        }
    }
}
