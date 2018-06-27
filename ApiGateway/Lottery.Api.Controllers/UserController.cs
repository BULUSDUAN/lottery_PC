using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Helper;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lottery.Api.Controllers
{
    [Area("User")]
    public class UserController : BaseController
    {
        /// <summary>
        /// 登录(103)
        /// </summary>
        public async Task<LotteryServiceResponse> UserLogin([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                //var p = WebHelper.Decode(entity.Param);
                string loginName = "18588515737";
                string password = "123456";
                if (string.IsNullOrEmpty(loginName))
                    throw new Exception("登录名不能为空");
                if (string.IsNullOrEmpty(password))
                    throw new Exception("密码不能为空");
                //param.Add("model", new QueryUserParam());IPAddress
                param["loginName"] = loginName;
                param["password"] = password;
                param["IPAddress"] = password;

                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/user_login");
                if (loginInfo == null)
                    throw new ArgumentException("登录失败");
                if (!loginInfo.IsSuccess)
                    throw new ArgumentException(loginInfo.Message);
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["UserId"] = loginInfo.UserId;
                var bindInfo = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                balanceParam["userToken"] = loginInfo.UserToken;
                var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");

                var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(balanceParam, "api/user/QueryBankCard");

                if (bankInfo == null) bankInfo = new C_BankCard();

                var unReadCount = await _serviceProxyProvider.Invoke<int>(balanceParam, "api/user/GetMyUnreadInnerMailCount");

                return new LotteryServiceResponse
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
                        CommissionBalance = balance.CommissionBalance,
                        //CommissionBalance = 0,
                        ExpertsBalance = balance.ExpertsBalance,
                        BonusBalance = balance.BonusBalance,
                        FreezeBalance = balance.FreezeBalance,
                        FillMoneyBalance = balance.FillMoneyBalance,
                        Mobile = string.IsNullOrEmpty(bindInfo.Mobile) ? string.Empty : bindInfo.Mobile, //mobile == null ? string.Empty : mobile.Mobile,
                        RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : bindInfo.RealName, // realName == null ? string.Empty : realName.RealName,
                        IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : bindInfo.IdCardNumber, // realName == null ? string.Empty : realName.IdCardNumber,
                        IsSetBalancePwd = balance.IsSetPwd,
                        NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                        IsBingBankCard = !string.IsNullOrEmpty(bindInfo.IdCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                        UserGrowth = balance.UserGrowth,
                        RedBagBalance = balance.RedBagBalance,
                        NeedGrowth = GrowthStatus(balance.UserGrowth),
                        IsBetHM = true,
                        UnReadMailCount = unReadCount,
                        HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                        #region 新字段
                        BankCardNumber = string.IsNullOrEmpty(bankInfo.BankCardNumber) ? "" : bankInfo.BankCardNumber,
                        BankName = string.IsNullOrEmpty(bankInfo.BankName) ? "" : bankInfo.BankName,
                        BankSubName = string.IsNullOrEmpty(bankInfo.BankSubName) ? "" : bankInfo.BankSubName,
                        #endregion
                    },
                };


            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
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
                string oldPassword = "123456";
                string newPassword = "123456789";
                string userToken = "12121";
                string userId = "13015";
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(oldPassword))
                    throw new Exception("旧密码不能为空");
                if (string.IsNullOrEmpty(newPassword))
                    throw new Exception("新密码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("Token不能为空");
                param["oldPassword"] = oldPassword;
                param["newPassword"] = newPassword;
                param["userToken"] = userToken;
                var chkPwd = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CheckIsSame2BalancePassword");
                if (chkPwd.ReturnValue == "T" || chkPwd.ReturnValue == "N")
                    throw new Exception("登录密码不能和资金密码一样");
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/ChangeMyPassword");

                if (!result.IsSuccess)
                {
                    throw new Exception(result.Message);
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
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.Message, MsgId = entity.MsgId, Value = null });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.Message, MsgId = entity.MsgId, Value = null });

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
                    throw new Exception("手机号码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("userToken不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new ArgumentException("手机号码格式错误");
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("用户编号不能为空！");

                param["mobile"] = mobile;
                param["userToken"] = userToken;
                param["userId"] = userId;

                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                if (loginInfo == null || string.IsNullOrEmpty(loginInfo.UserId))
                    throw new ArgumentException("未查询到用户信息");

                #region "20171108增加配置（禁止注册的手机号码）"
                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("key", "BanRegistrMobile");
                var banRegistrMobile = await _serviceProxyProvider.Invoke<C_Core_Config>(param2, "api/user/QueryCoreConfigByKey"); ;
                if (banRegistrMobile.ConfigValue.Contains(mobile))
                {
                    throw new ArgumentException("因检测到该号码在黑名单中，无法注册用户，请联系在线客服。");
                }

                #endregion
            }



            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.Message, MsgId = entity.MsgId, Value = null });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = ex.Message, MsgId = entity.MsgId, Value = null });

            }
            return null;
        }

        /// <summary>
        /// 回复手机认证 109
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> ResponseMobileValidate([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string mobileCode = p.MobileCode;
                string userToken = p.UserToken;
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(mobileCode))
                    throw new Exception("手机验证码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("userToken不能为空");
                param["validateCode"] = mobileCode;
                param["source"] = (int)SchemeSource.Web;
                param["userToken"] = userToken;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/ResponseAuthenticationMobile");
                if (!result.IsSuccess)
                    throw new Exception(result.Message);

                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = result.Message,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }

            /// <summary>
            /// 适应web版本注册 211
            /// </summary>
            /// <param name="_serviceProxyProvider"></param>
            /// <param name="entity"></param>
            /// <returns></returns>
            public async Task<LotteryServiceResponse> RegisterWeb([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);

                if (string.IsNullOrEmpty(p.password))
                    throw new Exception("密码不能为空！");
                if (string.IsNullOrEmpty(p.validateCode))
                    throw new Exception("验证码不能为空！");
                if (!ValidateHelper.IsMobile(p.mobile))
                    throw new ArgumentException("手机号码不能为空！");
                string cfrom = "";
                string pid = p.pid;
                SchemeSource schemeSource = SchemeSource.Web;
                if (!string.IsNullOrEmpty(cfrom) && cfrom == "ios")
                {
                    schemeSource = SchemeSource.Iphone;
                }

                var userInfo = new RegisterInfo_Local();
                //userInfo.RegisterIp = IpManager.IPAddress;
                userInfo.RegisterIp = "127.0.0.1";
                userInfo.LoginName = "15011111111";
                userInfo.Password = "123456";

                param["validateCode"] = "659235";
                param["mobile"] = "15011111111";
                param["source"] = (int)schemeSource;

                param["info"] = userInfo;

                if (!string.IsNullOrEmpty(pid))
                    userInfo.AgentId = pid;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/RegisterResponseMobile");
                if (result.Message.Contains("手机认证成功") || result.Message.Contains("恭喜您注册成功"))
                {

                    result.Message = "注册成功";
                    Dictionary<string, object> loginparam = new Dictionary<string, object>();
                    loginparam["loginName"] = userInfo.LoginName;
                    loginparam["password"] = userInfo.Password;
                    loginparam["IPAddress"] = userInfo.Password;
                    var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(loginparam, "api/user/user_login");
                    if (loginInfo.IsSuccess)
                    {
                        Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                        balanceParam["userToken"] = loginInfo.UserToken;
                        var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
                        Dictionary<string, object> bindParam = new Dictionary<string, object>();
                        bindParam["UserId"] = loginInfo.UserId;
                        var bindInfo = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                        var unReadCount = await _serviceProxyProvider.Invoke<int>(balanceParam, "api/user/GetMyUnreadInnerMailCount");
                        var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(balanceParam, "api/user/QueryBankCard");
                        if (bankInfo == null) bankInfo = new C_BankCard();
                        return new LotteryServiceResponse
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
                                CommissionBalance = balance.CommissionBalance,
                                //CommissionBalance = 0,
                                ExpertsBalance = balance.ExpertsBalance,
                                BonusBalance = balance.BonusBalance,
                                FreezeBalance = balance.FreezeBalance,
                                FillMoneyBalance = balance.FillMoneyBalance,
                                Mobile = string.IsNullOrEmpty(bindInfo.Mobile) ? string.Empty : bindInfo.Mobile, //mobile == null ? string.Empty : mobile.Mobile,
                                RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : bindInfo.RealName, // realName == null ? string.Empty : realName.RealName,
                                IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : bindInfo.IdCardNumber, // realName == null ? string.Empty : realName.IdCardNumber,
                                IsSetBalancePwd = balance.IsSetPwd,
                                NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                                IsBingBankCard = !string.IsNullOrEmpty(bindInfo.IdCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                                UserGrowth = balance.UserGrowth,
                                RedBagBalance = balance.RedBagBalance,
                                NeedGrowth = GrowthStatus(balance.UserGrowth),
                                IsBetHM = true,
                                UnReadMailCount = unReadCount,
                                HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                                #region 新字段
                                BankCardNumber = bankInfo.BankCardNumber,
                                BankName = bankInfo.BankName,
                                BankSubName = bankInfo.BankSubName,
                                #endregion
                            }
                        };
                    }
                    else
                    {
                        return new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = "注册成功,登陆失败",
                            MsgId = entity.MsgId,
                            Value = "注册成功,登陆失败"
                        };
                    }
                }
             
                 return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = string.IsNullOrEmpty(result.Message) ? result.Message : result.Message.Replace("验证码输入不正确", "手机验证码输入不正确"),
                    MsgId = entity.MsgId,
                    Value = string.IsNullOrEmpty(result.Message) ? result.Message : result.Message.Replace("验证码输入不正确", "手机验证码输入不正确"),
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }


        /// <summary>
        /// 发送手机短信 212
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> RegisterSendmsg([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string verifyCode = p.verifyCode;
                if (string.IsNullOrEmpty(verifyCode))
                    throw new Exception("图形验证码不能为空");
                if (!VerifyCode(verifyCode))
                    throw new Exception("图形验证码错误或已过期");
                string mobile = p.mobile;
                if (string.IsNullOrEmpty(mobile))
                    throw new Exception("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new Exception("手机号码格式错误");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/RegisterRequestMobile");               
              
                return new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.Message,
                    MsgId = entity.MsgId,
                    Value = "",
                };
            }
            catch (Exception ex)
            {
                //return Json(new { status = false, message = exp.Message }, JsonRequestBehavior.AllowGet);
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
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

        /// <summary>
        /// 手机号是否可注册 224
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<LotteryServiceResponse> PhoneIsRegister([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
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
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。",
                        MsgId = entity.MsgId,
                        Value = "因检测到该号码在黑名单中，无法注册用户，请联系在线客服。",
                    };
                }
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                var flag = await _serviceProxyProvider.Invoke<bool>(param, "api/user/HasMobile");
          
                var result = new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "手机号可用",
                    MsgId = entity.MsgId,
                    Value = "手机号可用",
                };
                if (flag)
                {
                    result.Message = "手机号已被注册";
                    result.Value = "手机号已被注册";
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }


        /// <summary>
        /// 找回密码
        /// </summary>
        /// <returns></returns>
        //public LotteryServiceResponse ForgetPwd(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = WebHelper.Decode(entity.Param);
        //        string mobile = p.mobile;
        //        string validateCode = p.validateCode;
        //        if (string.IsNullOrEmpty(mobile))
        //            throw new Exception("手机号码不能为空");
        //        if (!ValidateHelper.IsMobile(mobile))
        //            throw new Exception("手机号码格式错误");
        //        if (string.IsNullOrEmpty(validateCode))
        //            throw new Exception("验证码不能为空");

        //        var flag = WCFClients.ExternalClient.CheckValidateCodeByForgetPWD(mobile, validateCode);
        //        if (!flag)
        //            throw new Exception("验证码错误或已过期");
        //        string userId = WCFClients.ExternalClient.GetUserIdByLoginName(mobile);
        //        if (string.IsNullOrEmpty(userId))
        //            throw new Exception("手机号错误，该手机号未注册");
        //        string userToken = WCFClients.ExternalClient.GetGuestToken().ReturnValue;
        //        var isAuthMobile = WCFClients.ExternalClient.CheckIsAuthenticatedUserMobile(userId, userToken);
        //        PreconditionAssert.IsTrue(isAuthMobile, "用户未认证手机，无法使用手机找回密码。");
        //        var mobileinfo = WCFClients.ExternalClient.GetUserMobileInfo(userId, userToken);
        //        PreconditionAssert.IsTrue(mobileinfo.Mobile == mobile, "认证手机不匹配，无法找回密码。");
        //        var result = WCFClients.ExternalClient.FindPassword(userId);
        //        string code = result.ReturnValue;
        //        #region 发送站内消息：手机短信或站内信
        //        if (!string.IsNullOrEmpty(code))
        //        {
        //            var pwdArray = code.Split('|');
        //            if (pwdArray.Length == 2)
        //            {
        //                var pList = new List<string>();
        //                pList.Add(string.Format("{0}={1}", "[UserName]", mobile));
        //                pList.Add(string.Format("{0}={1}", "[UserPassword]", pwdArray[0]));
        //                pList.Add(string.Format("{0}={1}", "[UserPassword_2]", pwdArray[1]));
        //                发送短信
        //                WCFClients.GameQueryClient.DoSendSiteMessage("", mobile, "ON_User_Find_Password", string.Join("|", pList.ToArray()));
        //            }
        //        }
        //        #endregion
        //        return new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.成功,
        //            Message = "新密码已经发送手机",
        //            MsgId = entity.MsgId,
        //            Value = "新密码已经发送手机"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.失败,
        //            Message = ex.Message,
        //            MsgId = entity.MsgId,
        //            Value = ex.Message,
        //        };
        //    }
        //}

    }
}
