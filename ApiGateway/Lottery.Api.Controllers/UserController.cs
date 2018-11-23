using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common.ValidateCodeHelper;
using Lottery.Api.Controllers.CommonFilterActtribute;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using KaSon.FrameWork.Common.ExceptionEx;
using System.Diagnostics;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Expansion;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class UserController : BaseController
    {
        private IHttpContextAccessor _accessor;
        public UserController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        /// <summary>
        /// 登录(103)
        /// </summary>
        public async Task<IActionResult> UserLogin([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                string loginName = p.LoginName;
                string password = p.Password;
                if (string.IsNullOrEmpty(loginName))
                    throw new LogicException("登录名不能为空");
                if (string.IsNullOrEmpty(password))
                    throw new LogicException("密码不能为空");
                param["loginName"] = loginName;
                param["password"] = password;
                param["IPAddress"] = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/user_login");
                if (loginInfo == null)
                    throw new Exception("登录失败");
                if (!loginInfo.IsSuccess)
                    throw new ArgumentException(loginInfo.Message);
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["userId"] = loginInfo.UserId;
                //var bindInfo = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                var bindInfo = new UserBindInfos();
                Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                balanceParam["userId"] = loginInfo.UserId;
                //balanceParam["userToken"] = loginInfo.UserToken;
                //var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
                var balance = new UserBalanceInfo();
                var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(balanceParam, "api/user/QueryBankCard");
                //var bankInfo = new C_BankCard();
                //if (bankInfo == null) bankInfo = new C_BankCard();
                //balanceParam.Clear();
                var unReadCount = await _serviceProxyProvider.Invoke<int>(balanceParam, "api/user/GetMyUnreadInnerMailCount");
                //Task.Run(() => ToCreateGameAccount(loginInfo.UserId));
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "登录成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        UserToken = loginInfo.UserToken,
                        DisplayName = loginInfo.DisplayName,
                        LoginName = loginInfo.LoginName,
                        UserId = loginInfo.UserId,
                        VipLevel = loginInfo.VipLevel,
                        CommissionBalance = ConvertHelper.getTwoplaces(balance.CommissionBalance),
                        //CommissionBalance = 0,
                        ExpertsBalance = ConvertHelper.getTwoplaces(balance.ExpertsBalance),
                        BonusBalance = ConvertHelper.getTwoplaces(balance.BonusBalance),
                        FreezeBalance = ConvertHelper.getTwoplaces(balance.FreezeBalance),
                        FillMoneyBalance = ConvertHelper.getTwoplaces(balance.FillMoneyBalance),
                        Mobile = string.IsNullOrEmpty(bindInfo.Mobile) ? string.Empty : Regex.Replace(bindInfo.Mobile, "(\\d{3})\\d{3}(\\d{5})", "$1***$2"), //mobile == null ? string.Empty : mobile.Mobile,
                        RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : ConvertHelper.GetxxxString(bindInfo.RealName), // realName == null ? string.Empty : realName.RealName,
                        IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : ConvertHelper.GetBankCardNumberxxxString(bindInfo.IdCardNumber), // realName == null ? string.Empty : realName.IdCardNumber,
                        IsSetBalancePwd = balance.IsSetPwd,
                        NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                        IsBingBankCard = !string.IsNullOrEmpty(bindInfo.BankCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                        UserGrowth = balance.UserGrowth,
                        RedBagBalance = ConvertHelper.getTwoplaces(balance.RedBagBalance),
                        NeedGrowth = GrowthStatus(balance.UserGrowth),
                        IsBetHM = true,
                        UnReadMailCount = unReadCount,
                        HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                        #region 新字段
                        BankCardNumber = string.IsNullOrEmpty(bankInfo.BankCardNumber) ? "" : ConvertHelper.GetBankCardNumberxxxString(bankInfo.BankCardNumber),
                        BankName = string.IsNullOrEmpty(bankInfo.BankName) ? "" : bankInfo.BankName,
                        BankSubName = string.IsNullOrEmpty(bankInfo.BankSubName) ? "" : bankInfo.BankSubName,
                        #endregion
                    },
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }

        }

        #region 还需要的成长值

        private decimal GrowthStatus(decimal UserGrowth)
        {
            var needGrowth = 0M;
            //成长状态
            if (UserGrowth < 500)
            {
                needGrowth = 500 - UserGrowth;
            }
            else if (UserGrowth >= 500 && UserGrowth < 1000)
            {
                needGrowth = 1000 - UserGrowth;
            }
            else if (UserGrowth >= 1000 && UserGrowth < 2000)
            {
                needGrowth = 2000 - UserGrowth;
            }
            else if (UserGrowth >= 2000 && UserGrowth < 4000)
            {
                needGrowth = 4000 - UserGrowth;
            }
            else if (UserGrowth >= 4000 && UserGrowth < 8000)
            {
                needGrowth = 8000 - UserGrowth;
            }
            else if (UserGrowth >= 8000 && UserGrowth < 12000)
            {
                needGrowth = 12000 - UserGrowth;
            }
            else if (UserGrowth >= 12000 && UserGrowth < 16000)
            {
                needGrowth = 16000 - UserGrowth;
            }
            else if (UserGrowth >= 16000 && UserGrowth < 20000)
            {
                needGrowth = 20000 - UserGrowth;
            }
            //else if (UserGrowth >= 20000)
            //{

            //}
            return needGrowth;
        }

        #endregion
        /// <summary>
        /// 修改密码(107)
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateLoginPassword([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string oldPassword = p.OldPassword;
                string newPassword = p.NewPassword;
                string userToken = p.UserToken;
                string userId = p.UserId;
                Dictionary<string, object> param = new Dictionary<string, object>();

                if (string.IsNullOrEmpty(oldPassword))
                    throw new LogicException("旧密码不能为空");
                if (string.IsNullOrEmpty(newPassword))
                    throw new LogicException("新密码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("Token不能为空");
                string tokenuserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                if (tokenuserId != userId)
                    throw new LogicException("Token验证失败");
                param["newPassword"] = newPassword;
                param["userId"] = userId;

                var chkPwd = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CheckIsSame2BalancePassword");
                if (chkPwd.ReturnValue == "T" || chkPwd.ReturnValue == "N")
                    throw new LogicException("登录密码不能和资金密码一样");
                param.Clear();
                param["oldPassword"] = oldPassword;
                param["newPassword"] = newPassword;
                param["userId"] = userId;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/ChangeMyPassword");

                if (!result.IsSuccess)
                {
                    throw new LogicException(result.Message);
                }
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = null,
                });

            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "执行失败" + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.ToGetMessage() + "●" + ex.ToString(), MsgId = entity.MsgId, Value = ex.ToGetMessage(), });
            }
        }

        /// <summary>
        /// 请求手机认证(108)
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> RequestMobileValidate([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string mobile = p.Mobile;
                string userToken = p.UserToken;
                string userId = p.UserId;
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(mobile))
                    throw new LogicException("手机号码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("userToken不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new LogicException("手机号码格式错误");
                if (string.IsNullOrEmpty(userId))
                    throw new LogicException("用户编号不能为空！");
                string tokenuserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                if (tokenuserId != userId)
                    throw new LogicException("token验证失败！");
                //param["mobile"] = mobile;
                //param["userToken"] = userToken;
                param["userId"] = userId;

                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                if (loginInfo == null || string.IsNullOrEmpty(loginInfo.UserId))
                    throw new LogicException("未查询到用户信息");

                #region "20171108增加配置（禁止注册的手机号码）"
                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("key", "BanRegistrMobile");
                var banRegistrMobile = await _serviceProxyProvider.Invoke<C_Core_Config>(param2, "api/user/QueryCoreConfigByKey"); ;
                if (banRegistrMobile.ConfigValue.Contains(mobile))
                {
                    throw new LogicException("因检测到该号码在黑名单中，无法注册用户，请联系在线客服。");
                }

                #endregion
            }



            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "执行失败" + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.ToGetMessage() + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });

            }
            return null;
        }

        /// <summary>
        /// 回复手机认证 109
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> ResponseMobileValidate([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string mobileCode = p.MobileCode;
                string userToken = p.UserToken;
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(mobileCode))
                    throw new LogicException("手机验证码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("userToken不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                param["validateCode"] = mobileCode;
                param["source"] = (int)SchemeSource.Web;
                param["userId"] = userId;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/ResponseAuthenticationMobile");
                if (!result.IsSuccess)
                    throw new Exception(result.Message);

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = result.Message,
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "执行失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 适应web版本注册 211
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> RegisterWeb([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                string password = p.password;
                string validateCode = p.validateCode;
                string mobile = p.mobile;
                if (string.IsNullOrEmpty(password))
                    throw new LogicException("密码不能为空！");
                if (string.IsNullOrEmpty(validateCode))
                    throw new LogicException("验证码不能为空！");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new LogicException("手机号码不能为空！");
                //string cfrom = "";
                string pid = p.pid;
                string fxid = p.fxid;
                string yqid = p.yqid;
                string schemeId = p.schemeId;
                SchemeSource schemeSource = entity.SourceCode;
                //if (!string.IsNullOrEmpty(cfrom) && cfrom == "ios")
                //{
                //    schemeSource = SchemeSource.Iphone;
                //}

                var userInfo = new RegisterInfo_Local();
                //userInfo.RegisterIp = IpManager.IPAddress;
                userInfo.RegisterIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                userInfo.LoginName = mobile;
                userInfo.Password = password;
                userInfo.Mobile = mobile;
                #region ip判断
                if (schemeSource == SchemeSource.NewWeb)
                {
                    param["key"] = "BanRegistrFrequencyIPCount";
                    //限制IP注册次数时间分
                    var brfipCount = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");
                    param.Clear();
                    param["key"] = "BanRegistrFrequencyIPTime";
                    //限制分钟数
                    var brfipTime = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");
                    param.Clear();
                    param["key"] = "BanRegistrIP";
                    //限制分钟数
                    var banRegistrIP = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");

                    param.Clear();
                    if (banRegistrIP.ConfigValue.Contains(userInfo.RegisterIp))
                    {
                        return JsonEx(new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = "因检测到该IP地址异常，无法注册用户，请联系在线客服。",
                            Value = false,
                        });
                    }
                    if (Convert.ToInt32(brfipCount.ConfigValue) > 0 && Convert.ToInt32(brfipTime.ConfigValue) > 0)
                    {
                        DateTime dt = DateTime.Now.AddMinutes(-Convert.ToInt32(brfipTime.ConfigValue));
                        param["date"] = dt;
                        param["localIP"] = userInfo.RegisterIp;

                        var count = await _serviceProxyProvider.Invoke<int>(param, "api/user/GetTodayRegisterCount");
                        if (count > Convert.ToInt32(brfipCount.ConfigValue))
                        {
                            return JsonEx(new LotteryServiceResponse
                            {
                                Code = ResponseCode.失败,
                                Message = string.Format("同一IP，在{0}分钟内只能注册{1}个账号", brfipTime, brfipCount),
                                Value = false,
                            });
                        }
                    }

                }
                #endregion
                switch (schemeSource)
                {
                    case SchemeSource.NewAndroid:
                        userInfo.ComeFrom = "NewAndroid";
                        break;
                    case SchemeSource.NewIphone:
                        userInfo.ComeFrom = "NewIOS";
                        break;
                    case SchemeSource.Wap:
                        userInfo.ComeFrom = "NewTOUCH";
                        break;
                    case SchemeSource.NewWeb:
                        userInfo.ComeFrom = "NewWeb";
                        break;
                }
                param.Clear();
                param["validateCode"] = validateCode;
                param["mobile"] = mobile;
                param["source"] = (int)schemeSource;

                param["info"] = userInfo;

                if (!string.IsNullOrEmpty(pid))
                {
                    userInfo.AgentId = pid;
                }
                param["fxid"] = string.IsNullOrEmpty(fxid) ? "0" : fxid;
                param["yqid"] = string.IsNullOrEmpty(yqid) ? "0" : yqid; 
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/User/RegisterResponseMobile");
                param.Clear();
                if (result.Message.Contains("手机认证成功") || result.Message.Contains("恭喜您注册成功"))
                {

                    #region 此处判断执行订单送红包逻辑
                    if (!string.IsNullOrEmpty(schemeId))
                    {
                        param["schemeId"] = schemeId;
                        var redbag = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/User/OrderShareRegisterRedBag");
                    }
                    #endregion
                    result.Message = "注册成功";
                    Dictionary<string, object> loginparam = new Dictionary<string, object>();
                    loginparam["loginName"] = userInfo.LoginName;
                    loginparam["password"] = userInfo.Password;
                    loginparam["IPAddress"] = userInfo.RegisterIp;
                    var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(loginparam, "api/user/user_login");
                    if (loginInfo.IsSuccess)
                    {
                        Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                        balanceParam["userId"] = loginInfo.UserId;
                        var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
                        Dictionary<string, object> bindParam = new Dictionary<string, object>();
                        bindParam["userId"] = loginInfo.UserId;
                        var bindInfo = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                        var unReadCount = await _serviceProxyProvider.Invoke<int>(balanceParam, "api/user/GetMyUnreadInnerMailCount");
                        var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(balanceParam, "api/user/QueryBankCard");
                        if (bankInfo == null) bankInfo = new C_BankCard();
                        return JsonEx(new LotteryServiceResponse
                        {
                            Code = ResponseCode.成功,
                            Message = "注册成功",
                            MsgId = entity.MsgId,
                            Value = new
                            {
                                UserToken = loginInfo.UserToken,
                                DisplayName = loginInfo.DisplayName,
                                LoginName = loginInfo.LoginName,
                                UserId = loginInfo.UserId,
                                VipLevel = loginInfo.VipLevel,
                                CommissionBalance = ConvertHelper.getTwoplaces(balance.CommissionBalance),
                                //CommissionBalance = 0,
                                ExpertsBalance = ConvertHelper.getTwoplaces(balance.ExpertsBalance),
                                BonusBalance = ConvertHelper.getTwoplaces(balance.BonusBalance),
                                FreezeBalance = ConvertHelper.getTwoplaces(balance.FreezeBalance),
                                FillMoneyBalance = ConvertHelper.getTwoplaces(balance.FillMoneyBalance),
                                Mobile = string.IsNullOrEmpty(bindInfo.Mobile) ? string.Empty : Regex.Replace(bindInfo.Mobile, "(\\d{3})\\d{3}(\\d{5})", "$1***$2"), //mobile == null ? string.Empty : mobile.Mobile,
                                RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : ConvertHelper.GetxxxString(bindInfo.RealName), // realName == null ? string.Empty : realName.RealName,
                                IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : ConvertHelper.GetBankCardNumberxxxString(bindInfo.IdCardNumber), // realName == null ? string.Empty : realName.IdCardNumber,
                                IsSetBalancePwd = balance.IsSetPwd,
                                NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                                IsBingBankCard = !string.IsNullOrEmpty(bindInfo.BankCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                                UserGrowth = balance.UserGrowth,
                                RedBagBalance = ConvertHelper.getTwoplaces(balance.RedBagBalance),
                                NeedGrowth = GrowthStatus(balance.UserGrowth),
                                IsBetHM = true,
                                UnReadMailCount = unReadCount,
                                HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                                #region 新字段
                                BankCardNumber = string.IsNullOrEmpty(bankInfo.BankCardNumber) ? "" : ConvertHelper.GetBankCardNumberxxxString(bankInfo.BankCardNumber),
                                BankName = string.IsNullOrEmpty(bankInfo.BankName) ? "" : bankInfo.BankName,
                                BankSubName = string.IsNullOrEmpty(bankInfo.BankSubName) ? "" : bankInfo.BankSubName,
                                #endregion
                            }
                        });
                    }
                    else
                    {
                        return JsonEx(new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = "注册成功,登陆失败",
                            MsgId = entity.MsgId,
                            Value = "注册成功,登陆失败"
                        });
                    }
                }

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = string.IsNullOrEmpty(result.Message) ? result.Message : result.Message.Replace("验证码输入不正确", "手机验证码输入不正确"),
                    MsgId = entity.MsgId,
                    Value = string.IsNullOrEmpty(result.Message) ? result.Message : result.Message.Replace("验证码输入不正确", "手机验证码输入不正确"),
                });
            }
            catch (Exception ex)
            {

                return JsonEx(new LotteryServiceResponse
                {

                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        /// <summary>
        /// 发送手机短信 212
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> RegisterSendmsg([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var returnResult = new LotteryServiceResponse()
            {
                Code = ResponseCode.失败,
                //Message = ex.ToGetMessage(),
                MsgId = entity.MsgId,
                //Value = ex.ToGetMessage(),
            };
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string verifyCode = p.verifyCode;
                string MsgId = p.MsgId;
                //string plattype = p.plattype;

                if (string.IsNullOrEmpty(MsgId))
                {
                    returnResult.Message = "消息序号不能为空";
                    return JsonEx(returnResult);
                }
                //redis 获取验证码
                string key = "R_" + MsgId;
                //string codeValue = KaSon.FrameWork.Common.Redis.RedisHelperEx.DB_Other.Get(key);
                var codeParam = new Dictionary<string, object>();
                codeParam.Add("Key", key);
                var codeValue = await _serviceProxyProvider.Invoke<string>(codeParam, "api/user/GetRedisByOtherDbKey");
                if (codeValue != verifyCode)
                {
                    returnResult.Code = ResponseCode.ValiteCodeError;
                    returnResult.Message = "验证码错误";
                    return JsonEx(returnResult);
                }
                //if (plattype != "app")
                //{
                //    if (string.IsNullOrEmpty(verifyCode))
                //        throw new Exception("图形验证码不能为空");
                //    if (!VerifyCode(verifyCode))
                //        throw new Exception("图形验证码错误或已过期");
                //}
                string mobile = p.mobile;
                if (string.IsNullOrEmpty(mobile))
                    throw new LogicException("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new LogicException("手机号码格式错误");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/RegisterRequestMobile");

                return JsonEx(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                //return Json(new { status = false, message = exp.ToGetMessage() }, JsonRequestBehavior.AllowGet);
                returnResult.Code = ResponseCode.失败;
                returnResult.Message = ex.ToGetMessage() + "●" + ex.ToString();
                returnResult.Value = ex.ToGetMessage();
                return JsonEx(returnResult);
            }
        }

        /// <summary>
        /// 内部校验验证码是否正确 
        /// </summary>
        /// <param name="verifycode"></param>
        /// <returns></returns>
        private bool VerifyCode(string verifycode)
        {
            try
            {

                if (string.IsNullOrEmpty(verifycode) || HttpContext.Session.GetString("VerifyCode") == null)
                {
                    return false;
                }
                if (verifycode.ToLower() == HttpContext.Session.GetString("VerifyCode").ToString().ToLower())
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #region 本地验证码相关
        //public async Task<IActionResult> CreateValidateCode()
        //{
        //    var num = 0;
        //    string randomText = SelectRandomNumber(5, out num);

        //    //HttpContext.Session.SetString("VerifyCode", num.ToString());
        //    ValidateCodeGenerator vlimg = new ValidateCodeGenerator()
        //    {
        //        BackGroundColor = Color.FromKnownColor(KnownColor.LightGray),
        //        RandomWord = randomText,
        //        ImageHeight = 25,
        //        ImageWidth = 100,
        //        fontSize = 14,
        //    };
        //    var img = vlimg.OnPaint();
        //    if (img == null)
        //    {
        //        return await Task.FromResult(Content("Error")); ;
        //    }
        //    return await Task.FromResult(File(img, "image/gif"));
        //    //return vlimg;
        //}

        public async Task<IActionResult> CreateValidateCode([FromServices]IServiceProxyProvider _serviceProxyProvider, string MsgId)
        {
            try
            {
                var num = 0;
                string randomText = SelectRandomNumber(5, out num);
                var result = new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取验证码失败,请刷新验证码",
                    //   MsgId = entity.MsgId,
                    //  Value = ex.ToGetMessage(),
                };
                //HttpContext.Session.SetString("VerifyCode", num.ToString());
                ValidateCodeGenerator vlimg = new ValidateCodeGenerator()
                {
                    BackGroundColor = Color.FromKnownColor(KnownColor.LightGray),
                    RandomWord = randomText,
                    ImageHeight = 25,
                    ImageWidth = 100,
                    fontSize = 14,
                };
                var img = vlimg.OnPaint();
                if (img == null)
                {
                    // return Content("Error");
                }
                else
                {
                    result.Code = ResponseCode.成功;
                    result.Message = "成功获取验证码";

                    //录入验证码
                    var guidkey = Guid.NewGuid().ToString("N");
                    string key = "R_" + guidkey;
                    if (!String.IsNullOrEmpty(MsgId))
                    {
                        key = MsgId;
                    }

                    //KaSon.FrameWork.Common.Redis.RedisHelperEx.DB_Other.Set(key, num.ToString(), 60 * 10);
                    var param = new Dictionary<string, object>();
                    param.Add("Key", key);
                    param.Add("RValue", num.ToString());
                    param.Add("TotalSeconds", 60 * 10);
                    var flag = await _serviceProxyProvider.Invoke<bool>(param, "api/user/SetRedisOtherDbKey");
                    string base64 = Convert.ToBase64String(img);
                    //data:image/gif;base64,
                    if (!base64.StartsWith("data:image"))
                    {
                        base64 = "data:image/gif;base64," + base64;
                    }
                    result.Value = base64;
                    result.MsgId = guidkey;
                }
                return JsonEx(result);
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    // MsgId = entity.MsgId,
                    //Value = ex.ToGetMessage(),
                });
            }
            //return vlimg;
        }


        //选择随机数字
        private string SelectRandomNumber(int numberOfChars, out int num)
        {
            num = 0;
            StringBuilder randomBuilder = new StringBuilder();
            Random randomSeed = new Random();
            char[] columns = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                if (incr == 0 || incr == 2)
                {
                    var randomNum = columns[randomSeed.Next(10)].ToString();
                    randomBuilder.Append(randomNum);//取26个字符里的任意一个
                    num += int.Parse(randomNum);
                }
                if (incr == 1)
                {
                    randomBuilder.Append("+").ToString();
                }
                if (incr == 3)
                {
                    randomBuilder.Append("=").ToString();
                }
                if (incr == 4)
                {
                    randomBuilder.Append("?").ToString();
                }
            }
            return randomBuilder.ToString();
        }
        #endregion

        /// <summary>
        /// 手机号是否可注册 224
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> PhoneIsRegister([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> Keyparam = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                string mobile = p.mobile;
                Keyparam["key"] = "BanRegistrMobile";

                var banRegistrMobile = await _serviceProxyProvider.Invoke<C_Core_Config>(Keyparam, "api/user/QueryCoreConfigByKey");
                if (banRegistrMobile.ConfigValue.Contains(mobile))
                {
                    return JsonEx(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。",
                        MsgId = entity.MsgId,
                        Value = false,
                    });
                }
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                var flag = await _serviceProxyProvider.Invoke<bool>(param, "api/user/HasMobile");

                var result = new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "手机号可用",
                    MsgId = entity.MsgId,
                    Value = true,
                };
                if (flag)
                {
                    result.Message = "手机号已被注册";
                    result.Value = false;
                    return JsonEx(result);
                }
                else
                {
                    return JsonEx(result);
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = false,
                });
            }
        }


        /// <summary>
        /// 找回密码 215
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ForgetPwd([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string mobile = p.mobile;
                string validateCode = p.validateCode;
                if (string.IsNullOrEmpty(mobile))
                    throw new LogicException("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new LogicException("手机号码格式错误");
                if (string.IsNullOrEmpty(validateCode))
                    throw new LogicException("验证码不能为空");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                param["validateCode"] = validateCode;

                var flag = await _serviceProxyProvider.Invoke<bool>(param, "api/user/CheckValidateCodeByForgetPWD");
                if (!flag)
                    throw new LogicException("验证码错误或已过期");
                Dictionary<string, object> paramLoginName = new Dictionary<string, object>();
                paramLoginName["loginName"] = mobile;
                string userId = await _serviceProxyProvider.Invoke<string>(paramLoginName, "api/user/GetUserIdByLoginName");
                if (string.IsNullOrEmpty(userId))
                    throw new LogicException("手机号错误，该手机号未注册");

                //string userToken = WCFClients.ExternalClient.GetGuestToken().ReturnValue;
                //var isAuthMobile = WCFClients.ExternalClient.CheckIsAuthenticatedUserMobile(userId, userToken);
                //PreconditionAssert.IsTrue(isAuthMobile, "用户未认证手机，无法使用手机找回密码。");
                //var mobileinfo = WCFClients.ExternalClient.GetUserMobileInfo(userId, userToken);
                //PreconditionAssert.IsTrue(mobileinfo.Mobile == mobile, "认证手机不匹配，无法找回密码。");
                Dictionary<string, object> paramUsid = new Dictionary<string, object>();
                paramUsid["userId"] = userId;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(paramUsid, "api/user/FindPassword");

                string code = result.ReturnValue;
                #region 发送站内消息：手机短信或站内信
                if (!string.IsNullOrEmpty(code))
                {
                    var pwdArray = code.Split('|');
                    if (pwdArray.Length == 2)
                    {
                        var pList = new List<string>();
                        pList.Add(string.Format("{0}={1}", "[UserName]", mobile));
                        pList.Add(string.Format("{0}={1}", "[UserPassword]", pwdArray[0]));
                        pList.Add(string.Format("{0}={1}", "[UserPassword_2]", pwdArray[1]));

                        Dictionary<string, object> paramMessage = new Dictionary<string, object>();

                        paramMessage["userId"] = userId;
                        paramMessage["mobile"] = mobile;
                        paramMessage["sceneKey"] = "ON_User_Find_Password";
                        paramMessage["msgTemplateParams"] = string.Join("|", pList.ToArray());
                        //发送短信
                        var resultMessage = await _serviceProxyProvider.Invoke<CommonActionResult>(paramMessage, "api/user/DoSendSiteMessage");
                    }
                }
                #endregion
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "新密码已经发送手机",
                    MsgId = entity.MsgId,
                    Value = "新密码已经发送手机"
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        /// <summary>
        /// 忘记密码时发送验证码 214
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> ForgetPwd_VerifyCode([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string mobile = p.mobile;
                if (string.IsNullOrEmpty(mobile))
                    throw new LogicException("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new LogicException("手机号码格式错误");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/SendValidateCodeToUserMobileByForgetPWD");
                if (result.IsSuccess)
                {
                    return JsonEx(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "验证码发送成功",
                        MsgId = entity.MsgId,
                        Value = "验证码发送成功"
                    });
                }
                else
                {
                    return JsonEx(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = result.Message,
                        MsgId = entity.MsgId,
                        Value = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        #region 资金密码  161、162

        public async Task<IActionResult> SetBalancePwd([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {

                var p = WebHelper.Decode(entity.Param);
                string oldPwd = p.OldPwd;
                string newPwd = p.NewPwd;
                bool isSet = Convert.ToBoolean(p.IsSet);
                string userToken = p.UserToken;
                string strPlace = string.IsNullOrEmpty((string)p.StrPlace) ? "" : p.StrPlace;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录，请登录！");
                else if (string.IsNullOrEmpty(newPwd))
                    throw new LogicException("资金密码不能为空！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                if (isSet)
                {
                    if (!Regex.IsMatch(newPwd, "^\\d{6}$"))
                    {
                        throw new LogicException("新资金密码只能使用0-9的6位数字！");
                    }
                    //Dictionary<string, object> param = new Dictionary<string, object>();

                    //param["newPwd"] = newPwd;
                    //param["userId"] = userId;
                    //var checkRes = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CheckIsSame2LoginPassword");

                    //PreconditionAssert.IsTrue(checkRes.IsSuccess && checkRes.ReturnValue != "T", "资金密码不能与登录密码相同");
                }

                Dictionary<string, object> paramPwd = new Dictionary<string, object>();
                paramPwd["oldPassword"] = isSet ? oldPwd : newPwd;
                paramPwd["isSetPwd"] = isSet;
                paramPwd["newPassword"] = newPwd;
                paramPwd["userId"] = userId;
                paramPwd["placeList"] = strPlace;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(paramPwd, "api/user/SetBalancePassword");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.IsSuccess ? "设置资金密码成功" : "设置资金密码失败",
                    MsgId = entity.MsgId,
                    Value = result.IsSuccess ? "设置资金密码成功" : "设置资金密码失败",
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        /// <summary>
        /// 设置资金密码服务 162
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> SetBalancePwd_Place([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string strPlace = string.IsNullOrEmpty((string)p.StrPlace) ? "" : p.StrPlace;
                string pwd = p.Pwd;
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录，请登录！");
                else if (string.IsNullOrEmpty(pwd))
                    throw new LogicException("资金密码不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> paramPwd = new Dictionary<string, object>();
                paramPwd["password"] = pwd;
                paramPwd["placeList"] = strPlace;
                paramPwd["userId"] = userId;

                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(paramPwd, "api/user/SetBalancePasswordNeedPlace");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.IsSuccess ? "设置资金密码服务成功" : "设置资金密码服务失败",
                    MsgId = entity.MsgId,
                    Value = result.IsSuccess ? "设置资金密码服务成功" : "设置资金密码服务失败",
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "设置失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region "20180315 查询yqid的所有会员 有效会员 红包 "
        /// <summary>
        /// 返回结果格式以|分隔  满足条件的有效会员|推广注册总会员数|领取红包总额
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryYqidRegisterByAgentId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录，请登录！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userId"] = userId;
                var result = await _serviceProxyProvider.Invoke<string>(param, "api/user/QueryYqidRegisterByAgentIdToApp");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "返回结果成功!",
                    MsgId = entity.MsgId,
                    Value = result,
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion


        #region 实名认证
        /// <summary>
        /// 实名认证
        /// </summary>
        public async Task<IActionResult> RequestRealNameValidate([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                //if (entity.SourceCode == SchemeSource.Iphone)
                //    throw new Exception("您好！目前苹果客户端暂停实名认证。");
                var p = WebHelper.Decode(entity.Param);
                string idCardNumber = p.IdCardNumber;
                string realName = p.RealName;
                string userToken = p.UserToken;

                if (string.IsNullOrEmpty(idCardNumber))
                    throw new LogicException("证件号码不能为空");
                if (string.IsNullOrEmpty(realName))
                    throw new LogicException("真实姓名不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("userToken不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var userRealName = new UserRealNameInfo
                {
                    IdCardNumber = idCardNumber,
                    RealName = realName,
                };

                Dictionary<string, object> param = new Dictionary<string, object>();
                param["IdCardNumber"] = idCardNumber;
                param["RealName"] = realName;
                param["source"] = (int)SchemeSource.Web;
                param["userId"] = userId;


                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/AuthenticateMyRealName");

                if (!result.IsSuccess)
                    throw new Exception(result.Message);

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = result.Message,
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion


        #region 绑定银行卡
        /// <summary>
        /// 绑定银行卡
        /// </summary>
        public async Task<IActionResult> BindBank([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string bankCode = p.BankCode;
                string subBankName = p.SubBankName;
                string cardnumber = p.CardNumber;
                string province = p.Province;
                string city = p.City;
                string bankrealName = p.RealName;
                if (string.IsNullOrEmpty(bankCode))
                    throw new Exception("银行编码不能为空");
                //if (string.IsNullOrEmpty(subBankName))
                //    throw new Exception("银行名字不能为空");
                if (string.IsNullOrEmpty(cardnumber))
                    throw new Exception("银行卡号不能为空");
                if (string.IsNullOrEmpty(province))
                    throw new Exception("省会不能为空");
                if (string.IsNullOrEmpty(city))
                    throw new Exception("城市不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("userToken不能为空");
                if (string.IsNullOrEmpty(bankrealName))
                    throw new Exception("开户名不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["bankCode"] = bankCode;
                var resultbankCode = await _serviceProxyProvider.Invoke<C_Bank_Info>(param, "api/user/QueryBankInfo");

                if (resultbankCode == null)
                    throw new ArgumentException(string.Format("银行编码：{0}不可用", bankCode));

                #region "20171108增加配置（禁止注册的银行卡号码）"
                //Dictionary<string, object> param = new Dictionary<string, object>();
                //param["key"] = "BanRegistrBankCard";

                //var banRegistrBankCard = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/user/QueryCoreConfigByKey");
                //if (banRegistrBankCard!=null && banRegistrBankCard.ConfigValue.Contains(cardnumber))
                //{
                //    throw new ArgumentException("因检测到该银行卡号码在黑名单中，无法绑定，请联系在线客服。");
                //}
                C_BankCard bankCard = new C_BankCard
                {
                    BankCode = bankCode,
                    BankName = resultbankCode.BankName,
                    BankSubName = string.IsNullOrEmpty(subBankName) ? resultbankCode.BankName : subBankName,
                    BankCardNumber = cardnumber,
                    ProvinceName = province,
                    CityName = city,
                    RealName = bankrealName,
                };
                #endregion
                Dictionary<string, object> paramCard = new Dictionary<string, object>();
                paramCard["bankCard"] = bankCard;
                paramCard["userId"] = userId;


                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(paramCard, "api/user/AddBankCard");
                if (!result.IsSuccess)
                    throw new Exception(result.Message);

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "恭喜您已领取随机现金，请查收！",
                    MsgId = entity.MsgId,
                    Value = result.Message,
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "绑定失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "绑定失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion
        #region 银行信息
        private Dictionary<string, string> GetBankDic()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("CMB", "招商银行");
            dic.Add("ICBC", "中国工商银行");
            dic.Add("CCB", "中国建设银行");
            dic.Add("BOC", "中国银行");
            dic.Add("COMM", "中国交通银行");
            dic.Add("CITIC", "中信银行");
            dic.Add("CIB", "兴业银行");
            dic.Add("CEBBANK", "中国光大银行");
            dic.Add("CMBC", "中国民生银行");
            dic.Add("ABC", "中国农业银行");
            //dic.Add("SPAB", "平安银行");
            dic.Add("GDB", "广东发展银行");
            dic.Add("SDB", "深圳发展银行");
            dic.Add("BJB", "北京银行");
            dic.Add("SPDB", "上海浦东发展银行");
            dic.Add("SHB", "上海银行");
            //dic.Add("NBBANK", "宁波银行");
            //dic.Add("WZCB", "温州银行");
            dic.Add("CQCB", "重庆银行");
            return dic;
        }
        #endregion



        public async Task<IActionResult> QueryUserBalanceByUserToken([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userId = p.UserId;
                string userToken = p.UserToken;
                string loginName = p.LoginName;
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userToken) || string.IsNullOrEmpty(loginName))
                    throw new LogicException("传入参数信息有误！");
                string tokenuserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                if (tokenuserId != userId)
                    throw new LogicException("token验证失败！");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userId"] = userId;
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["userId"] = userId;
                var bindInfo = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                if (bindInfo == null)
                    throw new LogicException("未查询到当前用户信息！");
                Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                balanceParam["userId"] = userId;
                //var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
                //var unReadCount = await _serviceProxyProvider.Invoke<int>(balanceParam, "api/user/GetMyUnreadInnerMailCount");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询用户信息成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        UserToken = userToken,
                        DisplayName = bindInfo.DisplayName,
                        LoginName = bindInfo.DisplayName,
                        UserId = bindInfo.UserId,
                        VipLevel = bindInfo.VipLevel,
                        CommissionBalance = ConvertHelper.getTwoplaces(bindInfo.CommissionBalance),
                        //CommissionBalance = 0,
                        ExpertsBalance = ConvertHelper.getTwoplaces(bindInfo.ExpertsBalance),
                        BonusBalance = ConvertHelper.getTwoplaces(bindInfo.BonusBalance),
                        FreezeBalance = ConvertHelper.getTwoplaces(bindInfo.FreezeBalance),
                        FillMoneyBalance = ConvertHelper.getTwoplaces(bindInfo.FillMoneyBalance),
                        Mobile = string.IsNullOrEmpty(bindInfo.Mobile) ? string.Empty : Regex.Replace(bindInfo.Mobile, "(\\d{3})\\d{3}(\\d{5})", "$1***$2"), //mobile == null ? string.Empty : mobile.Mobile,
                        RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : ConvertHelper.GetxxxString(bindInfo.RealName), // realName == null ? string.Empty : realName.RealName,
                        IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : ConvertHelper.GetBankCardNumberxxxString(bindInfo.IdCardNumber), // realName == null ? string.Empty : realName.IdCardNumber,
                        IsSetBalancePwd = bindInfo.IsSetPwd,
                        NeedBalancePwdPlace = string.IsNullOrEmpty(bindInfo.NeedPwdPlace) ? string.Empty : bindInfo.NeedPwdPlace,
                        IsBingBankCard = !string.IsNullOrEmpty(bindInfo.BankCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                        UserGrowth = bindInfo.UserGrowth,
                        RedBagBalance = ConvertHelper.getTwoplaces(bindInfo.RedBagBalance),
                        NeedGrowth = GrowthStatus(bindInfo.UserGrowth),
                        IsBetHM = true,
                        UnReadMailCount = 0, //unReadCount,
                        HideDisplayNameCount = bindInfo.HideDisplayNameCount,
                        #region 新字段
                        BankCardNumber = string.IsNullOrEmpty(bindInfo.BankCardNumber) ? "" : ConvertHelper.GetBankCardNumberxxxString(bindInfo.BankCardNumber),
                        BankName = string.IsNullOrEmpty(bindInfo.BankName) ? "" : bindInfo.BankName,
                        BankSubName = string.IsNullOrEmpty(bindInfo.BankSubName) ? "" : bindInfo.BankSubName,
                        #endregion
                    },
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        /// <summary>
        /// 查询用户金额
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryUserBalance([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("传入参数信息有误！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                //balanceParam["UserToken"] = userToken;
                //var UserId = await _serviceProxyProvider.Invoke<string>(balanceParam, "api/user/GetUserIdByUserToken");
                //if (string.IsNullOrEmpty(UserId))
                //    throw new ArgumentException("未查询到当前用户信息！");
#if LogInfo
                var st = new Stopwatch();
                st.Start();
#endif
                balanceParam.Clear();
                balanceParam.Add("userId", userId);
                var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
#if LogInfo
                st.Stop();
                Log4Log.LogEX(KLogLevel.TimeInfo, "查询用户金额用时", "参数" + userToken + "；用时：" + st.Elapsed.TotalMilliseconds.ToString() + "毫秒");
#endif
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询用户金额成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        CommissionBalance = ConvertHelper.getTwoplaces(balance.CommissionBalance),
                        ExpertsBalance = ConvertHelper.getTwoplaces(balance.ExpertsBalance),
                        BonusBalance = ConvertHelper.getTwoplaces(balance.BonusBalance),
                        FreezeBalance = ConvertHelper.getTwoplaces(balance.FreezeBalance),
                        FillMoneyBalance = ConvertHelper.getTwoplaces(balance.FillMoneyBalance),
                        IsSetBalancePwd = balance.IsSetPwd,
                        NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                        UserGrowth = balance.UserGrowth,
                        RedBagBalance = ConvertHelper.getTwoplaces(balance.RedBagBalance),
                        NeedGrowth = GrowthStatus(balance.UserGrowth),
                        IsBetHM = true
                    },
                });
            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 验证是否可以提现 204
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> CehckDraw([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                if ((DateTime.Now.Hour < 8 || (DateTime.Now.Hour == 8 && DateTime.Now.Minute < 50))
                   && (DateTime.Now.Hour > 1 || (DateTime.Now.Hour == 1 && DateTime.Now.Minute > 10)))
                {
                    throw new LogicException("提现时间早上9点到凌晨1点，请您明天9点再来，感谢配合");
                }
                //读取json数据
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.token;
                string client = p.client;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("token不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);

                //Dictionary<string, object> param = new Dictionary<string, object>();
                //param["userToken"] = token;
                //var userinfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                //param.Add("userId", userId);
                //var cashMoney = await _serviceProxyProvider.Invoke<UserBalanceInfo>(param, "api/user/QueryMyBalance");
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["userId"] = userId;
                var info = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                if (info == null)
                    throw new LogicException("未找到用户信息");
                if (string.IsNullOrEmpty(info.RealName))
                    throw new LogicException("请先实名认证");
                if (string.IsNullOrEmpty(info.BankCardNumber))
                    throw new LogicException("请先绑定银行卡");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "可以提现",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        RealName = info.RealName,
                        BankName = info.BankName,
                        BankCardNumber = info.BankCardNumber,
                        TotalCashMoney = info.GetTotalCashMoney()
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        /// <summary>
        /// 提款确认 205
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> VerifyDraw([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                //读取json数据
                if ((DateTime.Now.Hour < 8 || (DateTime.Now.Hour == 8 && DateTime.Now.Minute < 50))
                 && (DateTime.Now.Hour > 1 || (DateTime.Now.Hour == 1 && DateTime.Now.Minute > 10)))
                {
                    throw new LogicException("提现时间早上9点到凌晨1点，请您明天9点再来，感谢配合");
                }
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.token;
                string client = p.client;
                string money = p.money;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("token不能为空");
                if (string.IsNullOrEmpty(money))
                    throw new LogicException("提款金额不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param["userToken"] = token;
                //var userinfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["userId"] = userId;
                var info = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                if (info == null)
                    throw new LogicException("未找到用户信息");
                if (string.IsNullOrEmpty(info.RealName))
                    throw new LogicException("请先实名认证");
                if (string.IsNullOrEmpty(info.BankCardNumber))
                    throw new LogicException("请先绑定银行卡");
                var minwithdrawmoney = await GetMinWithdrawMoney(_serviceProxyProvider);
                PreconditionAssert.IsTrue(decimal.Parse(money) >= minwithdrawmoney, "提款金额不能小于" + minwithdrawmoney.ToString() + "元");
                Dictionary<string, object> paramRequestWithdraw = new Dictionary<string, object>();
                paramRequestWithdraw["userId"] = info.UserId;
                paramRequestWithdraw["requestMoney"] = decimal.Parse(money);
                var RequestWithdraw_1 = await _serviceProxyProvider.Invoke<CheckWithdrawResult>(paramRequestWithdraw, "api/user/RequestWithdraw_Step1");
                if (RequestWithdraw_1.WithdrawCategory == WithdrawCategory.Error)
                {
                    return JsonEx(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = RequestWithdraw_1.Summary,
                        MsgId = entity.MsgId
                    });
                }
                param.Clear();
                param["userId"] = userId;
                var cashMoney = await _serviceProxyProvider.Invoke<UserBalanceInfo>(param, "api/user/QueryMyBalance");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "可以提现",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        RealName = info.RealName,
                        BankName = info.BankName,
                        BankCardNumber = info.BankCardNumber,
                        TotalCashMoney = cashMoney.GetTotalCashMoney(),
                        Money = money,
                        ResponseMoney = decimal.Round(RequestWithdraw_1.ResponseMoney, 2),
                        Commission = decimal.Round(RequestWithdraw_1.RequestMoney - RequestWithdraw_1.ResponseMoney, 2),
                        IsNeedPwd = cashMoney.CheckIsNeedPassword("Withdraw")
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 提款成功 206
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> Fetchsubmit([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                //读取json数据
                if ((DateTime.Now.Hour < 8 || (DateTime.Now.Hour == 8 && DateTime.Now.Minute < 50))
                 && (DateTime.Now.Hour > 1 || (DateTime.Now.Hour == 1 && DateTime.Now.Minute > 10)))
                {
                    throw new LogicException("提现时间早上9点到凌晨1点，请您明天9点再来，感谢配合");
                }
                //读取json数据
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.token;
                string client = p.client;
                string money = p.money;
                string balancepwd = p.balancepwd;
                decimal RequestMoney = 0;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("token不能为空");
                if (string.IsNullOrEmpty(money))
                    throw new LogicException("提款金额不能为空");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                decimal.TryParse(money, out RequestMoney);
                //PreconditionAssert.IsTrue(RequestMoney >= 10, "提款金额不能小于10元");
                var minwithdrawmoney = await GetMinWithdrawMoney(_serviceProxyProvider);
                PreconditionAssert.IsTrue(int.Parse(money) >= minwithdrawmoney, "提款金额不能小于" + minwithdrawmoney.ToString() + "元");
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param["userToken"] = token;
                //var userinfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["userId"] = userId;
                var info = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                if (info == null)
                    throw new LogicException("未找到用户信息");
                if (string.IsNullOrEmpty(info.RealName))
                    throw new LogicException("请先实名认证");
                if (string.IsNullOrEmpty(info.BankCardNumber))
                    throw new LogicException("请先绑定银行卡");

                Withdraw_RequestInfo withdrawinfo = new Withdraw_RequestInfo();
                withdrawinfo.BankCardNumber = info.BankCardNumber;
                //withdrawinfo.BankCode = "";
                withdrawinfo.BankName = info.BankName;
                withdrawinfo.BankSubName = info.BankSubName;
                withdrawinfo.CityName = info.CityName;
                withdrawinfo.ProvinceName = info.ProvinceName;
                withdrawinfo.RequestMoney = RequestMoney;
                withdrawinfo.WithdrawAgent = WithdrawAgentType.BankCard;
                withdrawinfo.userRealName = info.RealName;
                Dictionary<string, object> Withdraw_Param = new Dictionary<string, object>();
                Withdraw_Param["info"] = withdrawinfo;
                Withdraw_Param["userId"] = info.UserId;
                Withdraw_Param["balancepwd"] = balancepwd;
                var RequestWithdraw_Step2 = await _serviceProxyProvider.Invoke<CommonActionResult>(Withdraw_Param, "api/user/RequestWithdraw_Step2");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "提款成功",
                    MsgId = entity.MsgId
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        //充值记录
        public async Task<IActionResult> Drawingsrecord([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.token;
                int pageNo = Convert.ToInt32(p.pageNo);
                int PageSize = Convert.ToInt32(p.PageSize);
                var status = string.IsNullOrEmpty((string)p.Status) ? -1 : Convert.ToInt32(p.Status);
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("token验证失败");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                //var withdrawList = WCFClients.GameFundClient.QueryMyWithdrawList(WithdrawStatus.Success, begin, end.AddDays(1), pageNo, PageSize, token);
                //var withdrawList = WCFClients.GameFundClient.QueryMyWithdrawList(null, begin, end.AddDays(1), pageNo, PageSize, token);
                Dictionary<string, object> Param = new Dictionary<string, object>();
                Param["status"] = status;
                Param["pageIndex"] = pageNo;
                Param["pageSize"] = PageSize;
                Param["userId"] = userId;

                var withdrawList = await _serviceProxyProvider.Invoke<Withdraw_QueryInfoCollection>(Param, "api/user/QueryMyWithdrawList");

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = withdrawList.WithdrawList
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 获取充值相关json列表返回前端
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RechargePlatform([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                string fillMoney_Enable_GateWay = ConfigHelper.AllConfigInfo["FillMoney_Enable_GateWay"] != null ? ConfigHelper.AllConfigInfo["FillMoney_Enable_GateWay"].ToString() : "";
                //var p = WebHelper.Decode(entity.Param);//FillMoney_Enable_GateWay
                //string userToken = p.UserToken;
                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("key", "FillMoney_Enable_GateWay");
                var FillMoney_Enable_GateWay = await _serviceProxyProvider.Invoke<C_Core_Config>(param2, "api/Data/QueryCoreConfigByKey");
                if (FillMoney_Enable_GateWay != null)
                {
                    fillMoney_Enable_GateWay = FillMoney_Enable_GateWay.ConfigValue;
                }
                string[] gateWayArray = fillMoney_Enable_GateWay.ToLower().Split('|');
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = LoadPayConfig("ios", gateWayArray),
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        public async Task<IActionResult> TestConfig([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            try
            {
                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("key", "FillMoney_Enable_GateWay");
                var FillMoney_Enable_GateWay = await _serviceProxyProvider.Invoke<C_Core_Config>(param2, "api/user/QueryCoreConfigByKey");
                if (FillMoney_Enable_GateWay == null)
                    throw new Exception("未获取到FillMoney_Enable_GateWay配置");
                string[] gateWayArray = FillMoney_Enable_GateWay.ConfigValue.ToLower().Split('|');
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = "",
                    Value = LoadPayConfig("ios", gateWayArray),
                });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }

        }


        /// <summary>
        /// 加载可用充值json列表
        /// </summary>
        /// <param name="os"></param>
        /// <returns></returns>
        private dynamic LoadPayConfig(string os, string[] gateWayArray)
        {


            List<WebPayItem> list = new List<WebPayItem>();
            var baselist = loadPayConfig();
            foreach (var item in gateWayArray)
            {
                var obj = baselist.Find(a => a.gateway == item.Split('=')[0]);
                if (obj != null)
                    list.Add(buildPayUrl(obj, os));
            }
            //foreach (WebPayItem item in baselist)
            //{
            //    foreach (var getway in gateWayArray)
            //    {
            //        if (getway.Split('=')[0] == item.gateway)
            //        {
            //            list.Add(buildPayUrl(item, os));
            //        }
            //    }
            //}
            return new { pay = list };
        }
        private static List<WebPayItem> loadPayConfig()
        {
            string file = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"config\pay.json");
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                return JsonHelper.Deserialize<List<WebPayItem>>(sr.ReadToEnd());
            }
        }

        private WebPayItem buildPayUrl(WebPayItem item, string os)
        {
            if (item.amounts == null) item.amounts = new List<int>();
            if (string.IsNullOrEmpty(item.payType))
            {
                item.webViewUrl = item.actionUrl;
                return item;
            }
            string url = ConfigHelper.AllConfigInfo["MobileDomain"].ToString();
            StringBuilder s = new StringBuilder(url);
            s.Append("/api/user/mobile?");
            s.Append("amount={amount}");
            s.Append("&token={token}");
            s.Append("&gateway=" + HttpUtility.UrlEncode(item.gateway));
            if (item.bank != null && item.bank.Count > 0)
            {
                if (os == "ios")
                {
                    s.Append("&bank=${bank}");
                }
                else
                {
                    s.Append("&bank={bank}");
                }

            }
            if (string.IsNullOrEmpty(item.openUrl))
            {
                item.webViewUrl = s.ToString();
            }
            else
            {
                s.Append("&os=" + HttpUtility.UrlEncode(os));
                item.openUrl = s.ToString();
            }
            item.gateway = item.gateway;
            item.actionUrl = null;
            item.payType = item.payType;

            return item;
        }

        public async Task<IActionResult> mobile([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.token;
                string bank = p.bank;
                if (string.IsNullOrEmpty(userToken))
                {
                    throw new LogicException("无效参数");
                }
                string amount = p.amount;
                if (!CheckInt(amount, 1, 100000))
                {
                    throw new LogicException("充值金额无效");
                }
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param["userToken"] = token;
                //var lInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                //if (!lInfo.IsSuccess)
                //{
                //    throw new Exception(lInfo.Message);
                //}
                //param.Clear();
                param.Add("userId", userId);
                var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(param, "api/user/QueryBankCard");
                CallBackParam callBackParam = new CallBackParam();
                callBackParam.BankCardNo = "";
                if (bankInfo != null)
                {
                    callBackParam.BankCardNo = bankInfo.BankCardNumber;
                }
                //var cui = new LoginInfo();
                //cui = lInfo;
                callBackParam.UserId = userId;
                callBackParam.payAmount = amount;
                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("key", "FillMoney.CallBackDomain");
                var CallBackDomain = await _serviceProxyProvider.Invoke<C_Core_Config>(param2, "api/user/QueryCoreConfigByKey");
                callBackParam.CurrentDomain = CallBackDomain.ConfigValue;
                string gateway = p.gateway;

                var q = from c in baseConfig where c.gateway == gateway select c;
                WebPayItem item = q.FirstOrDefault();
                if (item == null || string.IsNullOrEmpty(item.actionUrl))
                {
                    return Content("不支持的支付类型");
                }
                //   "payType": "hw_bank|touch",
                if (item.bank != null && item.bank.Count > 0 && !string.IsNullOrEmpty(bank))
                {
                    callBackParam.HdpayType = item.payType.Split('|')[0] + "|" + bank;//
                }
                else
                {
                    callBackParam.HdpayType = item.payType;//
                }
                callBackParam.ActionUrl = item.actionUrl;// "http://pay2.ahmwwl.com/user/redirectpay";

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "成功",
                    MsgId = entity.MsgId,
                    Value = callBackParam,
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });

            }
        }

        private bool CheckInt(string str, int min, int max)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            try
            {
                int val = int.Parse(str);
                if (val >= min && val <= max)
                {
                    return true;
                }
            }
            catch
            {

            }
            return false;// throw new Exception("参数错误：" + name);
        }

        private List<WebPayItem> baseConfig
        {
            get
            {
                return loadPayConfig();
            }
        }

        private async Task<decimal> GetMinWithdrawMoney([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            decimal defaultmoney = 100;
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("key", "Site.Financial.MinWithDrwaMoney");
            var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
            if (config != null)
            {
                var minmoney = config.ConfigValue;
                decimal.TryParse(minmoney, out defaultmoney);
            }
            return defaultmoney;
        }

        public void ToCreateGameAccount(string UserId)
        {
            DataController.InitGameParam();
            var gameLoginName = DataController.PreName + UserId;
            var pwd = DataController.GamePassWord;
            var sign = MD5Helper.UpperMD5($"{DataController.OperatorCode}&{pwd}&{gameLoginName}&{DataController.SecretKey}");
            var strParam = new
            {
                command = "CREATE_ACCOUNT",
                gameprovider = "2",
                sign = sign,
                @params = new
                {
                    username = gameLoginName,
                    operatorcode = DataController.OperatorCode,
                    password = pwd,
                    extraparameter = new
                    {
                        type = "SMG"
                    }
                }
            }.ToJson();
            //var postParam = ConvertHelper.ReplaceFirst(strParam, "theparams", "params");
            //var result = PostManager.HttpPost(DataController.GameUrl, strParam, "utf-8");
            var result = PostManager.Post(DataController.GameUrl, strParam, Encoding.UTF8, 30, null, "application/json");
        }

        public async Task<IActionResult> LoginGiveRedEnvelopes([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string UserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                param["UserId"] = UserId;
                param["IPAddress"] = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                var GiveRedEnvelopes = await _serviceProxyProvider.Invoke<bool>(param, "api/user/LoginGiveRedEnvelopes");
                if (GiveRedEnvelopes)
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "恭喜获取登录红包，请在资金明细，查收",
                        MsgId = entity.MsgId,
                        Value = "恭喜获取登录红包，请在资金明细，查收"
                    });
                }
                else
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "",
                        MsgId = entity.MsgId,
                        Value = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #region PC相关接口
        public async Task<IActionResult> TransferFillMoneyPay([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string userToken = p.token;
                decimal money = p.payMoney;
                string type = p.payType;
                string userId = p.userId;
                string nameType = p.nameType;

                if (string.IsNullOrEmpty(nameType))
                    throw new Exception("请选择用户名或者用户id");

                if (string.IsNullOrEmpty(userId))
                    throw new Exception("转移到某个用户id不能为空");
                
                if (string.IsNullOrEmpty(money.ToString()) || money <= 0)
                    throw new Exception("转移金额不能为空且不能小于等于0");
                
                if (!new string[] { "50" }.Contains(type))
                    throw new Exception("错误的充值类型");
                //Thread.Sleep(10000);
                string uId = string.Empty;//转移对象的id
                Dictionary<string, object> param = new Dictionary<string, object>();
                //判断用户是否由此权限
                string myId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                param["userId"] = userId;
                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                if(!loginInfo.IsUserType)
                    throw new Exception("当前用户无此权限");
                if (nameType == "uName")
                {
                    param.Clear();
                    param["loginName"] = userId;
                    uId = await _serviceProxyProvider.Invoke<string>(param, "api/user/GetUserIdByLoginName");
                    if (string.IsNullOrEmpty(uId))
                    {
                        throw new Exception("用户名不存在");
                    }
                }
                else
                {
                    uId = userId;
                }
                if (uId == myId)
                {
                    throw new Exception("禁止自己给自己转移");
                    //return Json(new { IsSuccess = false, Message = "禁止自己给自己转移" }, JsonRequestBehavior.AllowGet);
                }

                UserFillMoneyAddInfo fillMoneyInfo = new UserFillMoneyAddInfo();
                fillMoneyInfo.RequestMoney = money;
                fillMoneyInfo.GoodsName = "彩金";
                fillMoneyInfo.GoodsDescription = "充值专员充值";
                fillMoneyInfo.GoodsType = "转移充值";
                fillMoneyInfo.ShowUrl = string.Empty;
                fillMoneyInfo.ReturnUrl = string.Empty;
                fillMoneyInfo.IsNeedDelivery = "false";
                fillMoneyInfo.NotifyUrl = string.Empty;
                fillMoneyInfo.FillMoneyAgent = FillMoneyAgentType.czzy;
                string agentid = string.Empty;
                //var hc_bankAddOrderResult = WCFClients.GameFundClient.UserFillMoneyByUserId(fillMoneyInfo, uId, this.CurrentUser.LoginInfo.UserId);
                param.Clear();
                param.Add("info", fillMoneyInfo);
                param.Add("userId", uId);
                param.Add("agentId", myId);
                var hc_bankAddOrderResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/UserFillMoneyByUserId");
                string orderId = string.Empty;//本地生成订单后的订单号
                if (hc_bankAddOrderResult.ReturnValue.Contains('|'))
                    orderId = hc_bankAddOrderResult.ReturnValue.Split('|')[0];
                else
                    orderId = hc_bankAddOrderResult.ReturnValue;

                //CommonActionResult car = WCFClients.GameFundClient.CompleteFillMoneyOrderByCzzy(orderId, FillMoneyStatus.Success, money, "1", string.Empty, this.CurrentUser.LoginInfo.UserId, type);
                param.Clear();
                param.Add("orderId", orderId);
                param.Add("status", FillMoneyStatus.Success);
                param.Add("money", money);
                param.Add("code", "1");
                param.Add("msg", "");
                param.Add("UserId", myId);
                param.Add("type", orderId);
                var car = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CompleteFillMoneyOrderByCzzy");
                //return Json(new { IsSuccess = true, Message = car.Message }, JsonRequestBehavior.AllowGet);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = car.Message,
                    MsgId = entity.MsgId,
                    Value = ""
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        #region 历史战绩
        /// <summary>
        /// 历史战绩
        /// </summary>
        public async Task<ActionResult> standings([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                //return Redirect("/upgrade/closeFunc.html");
                var p = WebHelper.Decode(entity.Param);
                string id = p.id;
                var pageIndex = 0;
                var pageSize = 30;
                var gameCode = string.IsNullOrEmpty((string)p.gameCode) ? "SZC" : (string)p.gameCode.ToUpper();
                var gameType = string.IsNullOrEmpty((string)p.gameType) ? "SSQ" : (string)p.gameType.ToUpper();
                if (string.IsNullOrEmpty(gameType))
                {
                    switch (gameCode)
                    {
                        case "JCZQ":
                            gameType = "BRQSPF";
                            break;
                        case "BJDC":
                            gameType = "SPF";
                            break;
                        case "JCLQ":
                            gameType = "SF";
                            break;
                        case "CTZQ":
                            gameType = "T14C";
                            break;
                        case "SZC":
                            gameType = "SSQ";
                            break;
                        default:
                            break;
                    }
                }

                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("userId", id);
                param.Add("gameCode", gameCode);
                param.Add("gameType", gameType);
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                //var blog = LoadBlogEntityStandings(id, gameCode, gameType, pageIndex, pageSize);
                var blog = await _serviceProxyProvider.Invoke<BlogEntity>(param, "api/user/QueryBlogEntityStandings");

                //new BlogEntity
                //{
                //    BonusOrderInfo = new BonusOrderInfoCollection(),
                //    CreateTime = DateTime.Now,
                //    FollowerCount = 0,
                //    ProfileBonusLevel = new ProfileBonusLevelInfo(),
                //    ProfileDataReport = new ProfileDataReport(),
                //    ProfileLastBonus = new ProfileLastBonusCollection(),
                //    ProfileUserInfo = new ProfileUserInfo(),
                //    UserBeedingListInfo = new UserBeedingListInfoCollection(),
                //    UserCurrentOrderInfo = new UserCurrentOrderInfoCollection(),
                //};// QueryBlogEntityStandings(id, gameCode, gameType, pageIndex, pageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        UserInfo = blog.ProfileUserInfo,
                        BonusLevel = blog.ProfileBonusLevel,
                        BonusListZj = blog.ProfileLastBonus,
                        DataReport = blog.ProfileDataReport,
                        BonusList = blog.BonusOrderInfo,
                        Count = blog.FollowerCount,
                        CurrentOrder = blog.UserCurrentOrderInfo,
                        GameCode = gameCode,
                        GameType = gameType,
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        //关注用户-关注和取消关注
        public async Task<ActionResult> attentionExec([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                var id = p.id;
                string userToken = p.userToken;
                var attUser = PreconditionAssert.IsNotEmptyString((string)p.attentionUserId, "被关注用户编号错误");
                var isAttention = string.IsNullOrEmpty(id) ? true : bool.Parse(id);
                var usrList = attUser.Split('|');
                var result = new CommonActionResult() { IsSuccess = false, Message = "未执行操作" };
                string UserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                foreach (var item in usrList)
                {
                    param.Add("beAttentionUserId", item);
                    param.Add("UserId", UserId);
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (isAttention)
                        {
                            result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/AttentionUser");
                        }
                        else
                        {
                            result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CancelAttentionUser");
                        }
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = result.Message,
                });
               
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                });
            }
        }

        public async Task<ActionResult> AttentAndGd([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                var user = p.UserId;
                string userToken = p.userToken;
                string beAttentionUserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var result = false;
                if (beAttentionUserId != null)
                {
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("beAttentionUserId", beAttentionUserId);
                    param.Add("currentUserId", user);
                    result = await _serviceProxyProvider.Invoke<bool>(param, "api/user/QueryIsAttention");
                    if (result)
                    {
                        return Json(new LotteryServiceResponse
                        {
                            Code = ResponseCode.成功,
                            Message = "关注成功",
                        });
                       
                    }
                    else
                    {
                        return Json(new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = "已关注",
                        });
                    }
                }
                else
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = "请登录",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message =ex.Message,
                });
            }
        }
        #endregion

        #region 登录历史记录
        public async Task<ActionResult> Loginhistory([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.userToken;
                string UserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("UserId", UserId);
                object obj = await _serviceProxyProvider.Invoke<UserLoginHistoryCollection>(param, "api/user/QueryCache_UserLoginHistoryCollection");

                var LoginHistory = (UserLoginHistoryCollection)obj;
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    Value = LoginHistory,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                });
            }
        }
        #endregion

        #region 删除站内信
        /// <summary>
        /// 删除站内信
        /// </summary>
        /// <param name="postForm"></param>
        /// <returns></returns>
       
        public async Task<ActionResult> Deleteinnermail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
              
                var p = WebHelper.Decode(entity.Param);
                var userToken = p.userToken;
                string UserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                string mailid = PreconditionAssert.IsNotEmptyString((string)p.MailId, "站内消息ID不能为空。");
                var mailid_ = mailid.Split(',');
                for (int i = 0; i < mailid_.Length; i++)
                {
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("innerMailId", mailid_[i]);
                    param.Add("UserId", UserId);
                    await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/DeleteInnerMail");
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "删除站内信完成",
                 
                });
              
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                });
            }
        }

        #endregion

        //撤销定制跟单

        public async Task<ActionResult> doc_cancel([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                var id = p.id;
                var userToken = p.userToken;
                var followId = long.Parse(PreconditionAssert.IsNotEmptyString(id, "定制跟单编号不能为空"));
                Dictionary<string, object> param = new Dictionary<string, object>();
                string UserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                param.Add("followerId", followId);
                param.Add("UserId", UserId);
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/ExistTogetherFollower");

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Value = result,

                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                });
            }
        }


        //定制跟单
        [HttpPost]
        public async Task<ActionResult> doc_setup([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                var isEdite = string.IsNullOrEmpty((string)p.isEdite) ? false : bool.Parse((string)p.isEdite);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "被跟单对象错误");
                var gameCode = PreconditionAssert.IsNotEmptyString((string)p.gameCode, "被跟单彩种不能为空");
                var gameType = string.IsNullOrEmpty((string)p.gameType) ? "" : (string)p.gameType;
                var docType = string.IsNullOrEmpty((string)p.docType) ? 0 : int.Parse((string)p.docType);
                var buyCount = string.IsNullOrEmpty((string)p.buyCount) ? 1 : int.Parse((string)p.buyCount);
                var allBuy = string.IsNullOrEmpty((string)p.allBuy) ? "不限" : (string)p.allBuy;
                var maxMoney = string.IsNullOrEmpty((string)p.maxMoney) ? "不限" : (string)p.maxMoney;
                var minMoney = string.IsNullOrEmpty((string)p.minMoney) ? "不限" : (string)p.minMoney;
                var minBalance = string.IsNullOrEmpty((string)p.minBalance) ? "不限" : (string)p.minBalance;
                var isBuySchemeMoneyNot = string.IsNullOrEmpty((string)p.isBuySchemeMoneyNot) ? false : bool.Parse((string)p.isBuySchemeMoneyNot);
                var isUsed = string.IsNullOrEmpty((string)p.isUsed) ? true : bool.Parse((string)p.isUsed);
                var isAutoStop = string.IsNullOrEmpty((string)p.isAutoStop) ? false : bool.Parse((string)p.isAutoStop);
                var autoStopCount = string.IsNullOrEmpty((string)p.autoStopCount) ? 10 : int.Parse((string)p.autoStopCount);
                var userToken = p.userToken;
                string UserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                TogetherFollowerRuleInfo info = new TogetherFollowerRuleInfo()
                {
                    CreaterUserId = userId,
                    FollowerUserId = UserId,
                    GameCode = gameCode,
                    GameType = gameType,
                    MaxSchemeMoney = maxMoney == "不限" ? -1 : int.Parse(maxMoney),
                    MinSchemeMoney = minMoney == "不限" ? -1 : int.Parse(minMoney),
                    SchemeCount = allBuy == "不限" ? -1 : int.Parse(allBuy),
                    StopFollowerMinBalance = minBalance == "不限" ? -1 : int.Parse(minBalance),
                    FollowerCount = docType == 0 ? buyCount : -1,
                    FollowerPercent = docType == 1 ? buyCount : -1,
                    CancelNoBonusSchemeCount = isAutoStop ? autoStopCount : -1,
                    CancelWhenSurplusNotMatch = isBuySchemeMoneyNot,
                    IsEnable = isUsed
                };
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (isEdite)
                {
                    var followId = long.Parse(PreconditionAssert.IsNotEmptyString((string)p.ruleId, "定制跟单编号不能为空"));
                  
                    param.Add("info", info);
                    param.Add("ruleId", followId);
                    var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/EditTogetherFollower");

                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Value = result,

                    });
                }
                else
                {
                    param.Add("info", info);
                    var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CustomTogetherFollower");
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Value = result,

                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                });
            }
        }

        #endregion




    }
}
