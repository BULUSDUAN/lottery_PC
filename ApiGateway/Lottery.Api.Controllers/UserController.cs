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
                    throw new Exception("登录名不能为空");
                if (string.IsNullOrEmpty(password))
                    throw new Exception("密码不能为空");
                //param.Add("model", new QueryUserParam());IPAddress
                param["loginName"] = loginName;
                param["password"] = password;
                param["IPAddress"] = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

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
                        RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : GetxxxString(bindInfo.RealName), // realName == null ? string.Empty : realName.RealName,
                        IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : GetBankCardNumberxxxString(bindInfo.IdCardNumber), // realName == null ? string.Empty : realName.IdCardNumber,
                        IsSetBalancePwd = balance.IsSetPwd,
                        NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                        IsBingBankCard = !string.IsNullOrEmpty(bindInfo.IdCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                        UserGrowth = balance.UserGrowth,
                        RedBagBalance = ConvertHelper.getTwoplaces(balance.RedBagBalance),
                        NeedGrowth = GrowthStatus(balance.UserGrowth),
                        IsBetHM = true,
                        UnReadMailCount = unReadCount,
                        HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                        #region 新字段
                        BankCardNumber = string.IsNullOrEmpty(bankInfo.BankCardNumber) ? "" : GetBankCardNumberxxxString(bankInfo.BankCardNumber),
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

        /// <summary>
        /// 返回隐藏中间的字符串
        /// </summary>
        /// <param name="Input">输入</param>
        /// <returns>输出</returns>
        public static string GetxxxString(string Input)
        {
            string Output = "";
            switch (Input.Length)
            {
                case 1:
                    Output = "*";
                    break;
                case 2:
                    Output = Input[0] + "*";
                    break;
                case 0:
                    Output = "";
                    break;
                default:
                    Output = Input.Substring(0, 1);
                    for (int i = 0; i < Input.Length - 1; i++)
                    {
                        Output += "*";
                    }
                    Output += Input.Substring(Input.Length - 1, 0);
                    break;
            }
            return Output;
        }

        /// <summary>
        /// 返回隐藏中间的身份证号
        /// </summary>
        /// <param name="Input">输入</param>
        /// <returns>输出</returns>
        public static string GetBankCardNumberxxxString(string Input)
        {
            string Output = "";
            if (Input.Length > 7)
            {
                Output = Input.Substring(0, 5);
                for (int i = 5; i < 8; i++)
                {
                    Output += "*";
                }
                Output += Input.Substring(7, Input.Length - 7);
            }
            return Output;
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
                Dictionary<string, object> paramCheck = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(oldPassword))
                    throw new Exception("旧密码不能为空");
                if (string.IsNullOrEmpty(newPassword))
                    throw new Exception("新密码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("Token不能为空");
                paramCheck["newPassword"] = newPassword;
                paramCheck["userId"] = userId;

                param["oldPassword"] = oldPassword;
                param["newPassword"] = newPassword;
                param["userToken"] = userToken;
                var chkPwd = await _serviceProxyProvider.Invoke<CommonActionResult>(paramCheck, "api/user/CheckIsSame2BalancePassword");
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
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "执行失败" + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "执行失败" + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });

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
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "执行失败" + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "执行失败" + "●" + ex.ToString(), MsgId = entity.MsgId, Value = null });

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
                    throw new Exception("手机验证码不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("userToken不能为空");
                param["validateCode"] = mobileCode;
                param["source"] = (int)SchemeSource.Web;
                param["userToken"] = userToken;
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
                    Message = "执行失败" + "●" + ex.ToString(),
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
                    throw new Exception("密码不能为空！");
                if (string.IsNullOrEmpty(validateCode))
                    throw new Exception("验证码不能为空！");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new ArgumentException("手机号码不能为空！");
                //string cfrom = "";
                string pid = p.pid;
                string fxid = p.fxid;
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
                param["validateCode"] = validateCode;
                param["mobile"] = mobile;
                param["source"] = (int)schemeSource;

                param["info"] = userInfo;
           
                if (!string.IsNullOrEmpty(pid))
                {
                    userInfo.AgentId = pid;
                }          
                param["fxid"] = string.IsNullOrEmpty(fxid)?"0": fxid; 
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/User/RegisterResponseMobile");
                param.Clear();             
                if (result.Message.Contains("手机认证成功") || result.Message.Contains("恭喜您注册成功"))
                {
                    
                    #region 此处判断执行订单送红包逻辑
                    if (!string.IsNullOrEmpty(schemeId))
                    {
                        param["schemeId"] =  schemeId;
                      var redbag= await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/User/OrderShareRegisterRedBag");
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
                        balanceParam["userToken"] = loginInfo.UserToken;
                        var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
                        Dictionary<string, object> bindParam = new Dictionary<string, object>();
                        bindParam["UserId"] = loginInfo.UserId;
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
                                RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : GetxxxString(bindInfo.RealName), // realName == null ? string.Empty : realName.RealName,
                                IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : GetBankCardNumberxxxString(bindInfo.IdCardNumber), // realName == null ? string.Empty : realName.IdCardNumber,
                                IsSetBalancePwd = balance.IsSetPwd,
                                NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                                IsBingBankCard = !string.IsNullOrEmpty(bindInfo.IdCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                                UserGrowth = balance.UserGrowth,
                                RedBagBalance = ConvertHelper.getTwoplaces(balance.RedBagBalance),
                                NeedGrowth = GrowthStatus(balance.UserGrowth),
                                IsBetHM = true,
                                UnReadMailCount = unReadCount,
                                HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                                #region 新字段
                                BankCardNumber = string.IsNullOrEmpty(bankInfo.BankCardNumber) ? "" : GetBankCardNumberxxxString(bankInfo.BankCardNumber),
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
                    Message = "注册失败" + "●" + ex.ToString(),
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
                string codeValue = KaSon.FrameWork.Common.Redis.RedisHelper.StringGet(key);
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
                    throw new Exception("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new Exception("手机号码格式错误");
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
                returnResult.Message = "发送失败" + "●" + ex.ToString();
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

        public IActionResult CreateValidateCode(string MsgId)
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

                    KaSon.FrameWork.Common.Redis.RedisHelper.StringSet(key, num.ToString(), 60 * 10);

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
                    Message = "获取验证码失败" + "●" + ex.ToString(),
                    //   MsgId = entity.MsgId,
                    //  Value = ex.ToGetMessage(),
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
                    Message = "验证失败" + "●" + ex.ToString(),
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
                    throw new Exception("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new Exception("手机号码格式错误");
                if (string.IsNullOrEmpty(validateCode))
                    throw new Exception("验证码不能为空");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["mobile"] = mobile;
                param["validateCode"] = validateCode;

                var flag = await _serviceProxyProvider.Invoke<bool>(param, "api/user/CheckValidateCodeByForgetPWD");
                if (!flag)
                    throw new Exception("验证码错误或已过期");
                Dictionary<string, object> paramLoginName = new Dictionary<string, object>();
                paramLoginName["loginName"] = mobile;
                string userId = await _serviceProxyProvider.Invoke<string>(paramLoginName, "api/user/GetUserIdByLoginName");
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("手机号错误，该手机号未注册");

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
                    Message = "操作失败" + "●" + ex.ToString(),
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
                    throw new Exception("手机号码不能为空");
                if (!ValidateHelper.IsMobile(mobile))
                    throw new Exception("手机号码格式错误");
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
                    Message = "发送验证码失败" + "●" + ex.ToString(),
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
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("您还未登录，请登录！");
                else if (string.IsNullOrEmpty(newPwd))
                    throw new Exception("资金密码不能为空！");
                if (isSet)
                {
                    if (!Regex.IsMatch(newPwd, "^\\d{6}$"))
                    {
                        throw new Exception("新资金密码只能使用0-9的6位数字！");
                    }

                    Dictionary<string, object> param = new Dictionary<string, object>();

                    param["newPwd"] = newPwd;
                    param["userToken"] = userToken;
                    var checkRes = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CheckIsSame2LoginPassword");

                    PreconditionAssert.IsTrue(checkRes.IsSuccess && checkRes.ReturnValue != "T", "资金密码不能与登录密码相同");
                }

                Dictionary<string, object> paramPwd = new Dictionary<string, object>();
                paramPwd["oldPassword"] = isSet ? oldPwd : newPwd;
                paramPwd["isSetPwd"] = isSet;
                paramPwd["newPassword"] = newPwd;
                paramPwd["userToken"] = userToken;

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
                    Message = "设置失败" + "●" + ex.ToString(),
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
                string strPlace = p.StrPlace;
                string pwd = p.Pwd;
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("您还未登录，请登录！");
                else if (string.IsNullOrEmpty(pwd))
                    throw new Exception("资金密码不能为空");

                Dictionary<string, object> paramPwd = new Dictionary<string, object>();
                paramPwd["pwd"] = pwd;
                paramPwd["strPlace"] = strPlace;
                paramPwd["userToken"] = userToken;

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
                    Message = "设置失败" + "●" + ex.ToString(),
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
                    throw new Exception("您还未登录，请登录！");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userToken"] = userToken;
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
                    throw new Exception("证件号码不能为空");
                if (string.IsNullOrEmpty(realName))
                    throw new Exception("真实姓名不能为空");
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("userToken不能为空");
                var userRealName = new UserRealNameInfo
                {
                    IdCardNumber = idCardNumber,
                    RealName = realName,
                };

                Dictionary<string, object> param = new Dictionary<string, object>();
                param["IdCardNumber"] = idCardNumber;
                param["RealName"] = realName;
                param["source"] = (int)SchemeSource.Web;
                param["userToken"] = userToken;


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
                    Message = "服务器内部错误，请联系接口提供商",
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
                    BankSubName = subBankName,
                    BankCardNumber = cardnumber,
                    ProvinceName = province,
                    CityName = city,
                    RealName = bankrealName,
                };
                #endregion
                Dictionary<string, object> paramCard = new Dictionary<string, object>();
                paramCard["bankCard"] = bankCard;
                paramCard["userToken"] = userToken;


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
                    throw new ArgumentException("传入参数信息有误！");

                //if (!CanDoLoadUserInfo(loginName))
                //    throw new Exception("刷新频繁，请稍后再试");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userId"] = userId;
                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                if (loginInfo == null)
                    throw new ArgumentException("未查询到当前用户信息！");
                var isBetHM = true;
                Dictionary<string, object> bindParam = new Dictionary<string, object>();
                bindParam["UserId"] = userId;
                var bindInfo = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                //var mobile = WCFClients.ExternalClient.GetMyMobileInfo(userToken);
                //var realName = WCFClients.ExternalClient.GetMyRealNameInfo(userToken);
                Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                balanceParam["userToken"] = userToken;
                var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");

                var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(balanceParam, "api/user/QueryBankCard");
                if (bankInfo == null) bankInfo = new C_BankCard();
                //var InnerMailUnReadList = WCFClients.GameQueryClient.QueryUnReadInnerMailListByReceiver(loginInfo.UserId, 0, 1000000, InnerMailHandleType.UnRead);

                var unReadCount = await _serviceProxyProvider.Invoke<int>(balanceParam, "api/user/GetMyUnreadInnerMailCount");
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询用户信息成功",
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
                        RealName = string.IsNullOrEmpty(bindInfo.RealName) ? string.Empty : GetxxxString(bindInfo.RealName), // realName == null ? string.Empty : realName.RealName,
                        IdCardNumber = string.IsNullOrEmpty(bindInfo.IdCardNumber) ? string.Empty : GetBankCardNumberxxxString(bindInfo.IdCardNumber), // realName == null ? string.Empty : realName.IdCardNumber,
                        IsSetBalancePwd = balance.IsSetPwd,
                        NeedBalancePwdPlace = string.IsNullOrEmpty(balance.NeedPwdPlace) ? string.Empty : balance.NeedPwdPlace,
                        IsBingBankCard = !string.IsNullOrEmpty(bindInfo.IdCardNumber), // bankInfo == null ? false : !string.IsNullOrEmpty(bankInfo.UserId),
                        UserGrowth = balance.UserGrowth,
                        RedBagBalance = ConvertHelper.getTwoplaces(balance.RedBagBalance),
                        NeedGrowth = GrowthStatus(balance.UserGrowth),
                        IsBetHM = true,
                        UnReadMailCount = unReadCount,
                        HideDisplayNameCount = loginInfo.HideDisplayNameCount,

                        #region 新字段
                        BankCardNumber = string.IsNullOrEmpty(bankInfo.BankCardNumber) ? "" : GetBankCardNumberxxxString(bankInfo.BankCardNumber),
                        BankName = string.IsNullOrEmpty(bankInfo.BankName) ? "" : bankInfo.BankName,
                        BankSubName = string.IsNullOrEmpty(bankInfo.BankSubName) ? "" : bankInfo.BankSubName,
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
                    Message = "服务器内部错误，请联系接口提供商" + "●" + ex.ToString(),
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
                    throw new ArgumentException("传入参数信息有误！");
                Dictionary<string, object> balanceParam = new Dictionary<string, object>();
                balanceParam["userToken"] = userToken;
#if LogInfo
                var st = new Stopwatch();
                st.Start();
#endif
                var balance = await _serviceProxyProvider.Invoke<UserBalanceInfo>(balanceParam, "api/user/QueryMyBalance");
#if LogInfo
                st.Stop();
                Log4Log.LogEX(KLogLevel.TimeInfo, "查询用户金额用时", "参数"+ userToken+"；用时："+st.Elapsed.TotalMilliseconds.ToString()+"毫秒");
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
                    Message = "服务器内部错误，请联系接口提供商" + "●" + ex.ToString(),
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
                //读取json数据
                var p = WebHelper.Decode(entity.Param);
                string token = p.token;
                string client = p.client;
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("token不能为空");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userToken"] = token;


                if ((DateTime.Now.Hour < 8 || (DateTime.Now.Hour == 8 && DateTime.Now.Minute < 50))
                    && (DateTime.Now.Hour > 1 || (DateTime.Now.Hour == 1 && DateTime.Now.Minute > 10)))
                {
                    throw new ArgumentException("提现时间早上9点到凌晨1点，请您明天9点再来，感谢配合");
                }
                var cashMoney = await _serviceProxyProvider.Invoke<UserBalanceInfo>(param, "api/user/QueryMyBalance");
                var userinfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");

                if (userinfo.IsSuccess)
                {
                    Dictionary<string, object> bindParam = new Dictionary<string, object>();
                    bindParam["UserId"] = userinfo.UserId;
                    var info = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                    if (info == null)
                        throw new ArgumentException("未找到用户信息");
                    if (string.IsNullOrEmpty(info.RealName))
                        throw new ArgumentException("请先实名认证");
                    if (string.IsNullOrEmpty(info.BankCardNumber))
                        throw new ArgumentException("请先绑定银行卡");


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
                            TotalCashMoney = cashMoney.GetTotalCashMoney()
                        }
                    });

                }
                else
                {
                    throw new Exception(userinfo.Message);
                }
            }
            catch (Exception exp)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商" + "●" + exp.ToString(),
                    MsgId = entity.MsgId,
                    Value = exp.ToGetMessage(),
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
                var p = WebHelper.Decode(entity.Param);
                string token = p.token;
                string client = p.client;
                string money = p.money;
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("token不能为空");
                if (string.IsNullOrEmpty(money))
                    throw new ArgumentException("提款金额不能为空");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userToken"] = token;
                var userinfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                if (userinfo.IsSuccess)
                {
                    Dictionary<string, object> bindParam = new Dictionary<string, object>();
                    bindParam["UserId"] = userinfo.UserId;
                    var info = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                    if (info == null)
                        throw new ArgumentException("未找到用户信息");
                    if (string.IsNullOrEmpty(info.RealName))
                        throw new ArgumentException("请先实名认证");
                    if (string.IsNullOrEmpty(info.BankCardNumber))
                        throw new ArgumentException("请先绑定银行卡");
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
                            ResponseMoney = RequestWithdraw_1.ResponseMoney,
                            Commission = RequestWithdraw_1.RequestMoney - RequestWithdraw_1.ResponseMoney,
                            IsNeedPwd = cashMoney.CheckIsNeedPassword("Withdraw")
                        }
                    });

                }
                else
                {
                    throw new Exception(userinfo.Message);
                }
            }
            catch (Exception exp)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商" + "●" + exp.ToString(),
                    MsgId = entity.MsgId,
                    Value = exp.ToGetMessage(),
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
                var p = WebHelper.Decode(entity.Param);
                string token = p.token;
                string client = p.client;
                string money = p.money;
                string balancepwd = p.balancepwd;
                decimal RequestMoney = 0;
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("token不能为空");
                if (string.IsNullOrEmpty(money))
                    throw new ArgumentException("提款金额不能为空");
                decimal.TryParse(money, out RequestMoney);
                //PreconditionAssert.IsTrue(RequestMoney >= 10, "提款金额不能小于10元");
                var minwithdrawmoney = await GetMinWithdrawMoney(_serviceProxyProvider);
                PreconditionAssert.IsTrue(int.Parse(money) >= minwithdrawmoney, "提款金额不能小于" + minwithdrawmoney.ToString() + "元");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userToken"] = token;
                var userinfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                if (userinfo.IsSuccess)
                {
                    Dictionary<string, object> bindParam = new Dictionary<string, object>();
                    bindParam["UserId"] = userinfo.UserId;
                    var info = await _serviceProxyProvider.Invoke<UserBindInfos>(bindParam, "api/user/QueryUserBindInfos");
                    if (info == null)
                        throw new ArgumentException("未找到用户信息");
                    if (string.IsNullOrEmpty(info.RealName))
                        throw new ArgumentException("请先实名认证");
                    if (string.IsNullOrEmpty(info.BankCardNumber))
                        throw new ArgumentException("请先绑定银行卡");

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
                else
                {
                    throw new Exception(userinfo.Message);
                }
            }
            catch (Exception exp)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商" + "●" + exp.ToString(),
                    MsgId = entity.MsgId,
                    Value = exp.ToGetMessage(),
                });
            }
        }


        //充值记录
        public async Task<IActionResult> Drawingsrecord([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = WebHelper.Decode(entity.Param);
                string token = p.token;
                DateTime begin = Convert.ToDateTime(p.begin);
                DateTime end = Convert.ToDateTime(p.end);
                int pageNo = Convert.ToInt32(p.pageNo);
                int PageSize = Convert.ToInt32(p.PageSize);
                var status = string.IsNullOrEmpty((string)p.Status) ? null : Convert.ToInt32(p.Status);

                //var withdrawList = WCFClients.GameFundClient.QueryMyWithdrawList(WithdrawStatus.Success, begin, end.AddDays(1), pageNo, PageSize, token);
                //var withdrawList = WCFClients.GameFundClient.QueryMyWithdrawList(null, begin, end.AddDays(1), pageNo, PageSize, token);
                if (begin < DateTime.Now.AddMonths(-1))
                    begin = DateTime.Now.AddMonths(-1);

                Dictionary<string, object> Param = new Dictionary<string, object>();
                Param["status"] = status;
                Param["startTime"] = begin;
                Param["endTime"] = end.AddDays(1);
                Param["pageIndex"] = pageNo;
                Param["pageSize"] = PageSize;
                Param["userToken"] = token;

                var withdrawList = await _serviceProxyProvider.Invoke<Withdraw_QueryInfoCollection>(Param, "api/user/QueryMyWithdrawList");

                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = withdrawList.WithdrawList
                });
            }
            catch (Exception exp)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商" + "●" + exp.ToString(),
                    MsgId = entity.MsgId,
                    Value = exp.ToGetMessage(),
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
                var p = WebHelper.Decode(entity.Param);
                string UserToken = p.UserToken;
                if (!string.IsNullOrEmpty(UserToken))
                {
                    UserToken = UserToken.Replace("%2B", "+").Replace("%26", "&");
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["userToken"] = UserToken;
                    var lInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                    if (lInfo.IsSuccess)
                    {
                        Dictionary<string, object> param2 = new Dictionary<string, object>();
                        param2.Add("key", "FillMoney_Enable_GateWay");
                        var FillMoney_Enable_GateWay = await _serviceProxyProvider.Invoke<C_Core_Config>(param2, "api/user/QueryCoreConfigByKey");
                        string[] gateWayArray = FillMoney_Enable_GateWay.ConfigValue.ToLower().Split('|');
                        return JsonEx(new LotteryServiceResponse
                        {
                            Code = ResponseCode.成功,
                            Message = "获取成功",
                            MsgId = entity.MsgId,
                            Value = LoadPayConfig("ios", gateWayArray),
                        });
                    }
                    else
                    {
                        return JsonEx(new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = "验证用户失败",
                            MsgId = entity.MsgId,
                            Value = "验证用户失败",
                        });
                    }
                }
                else
                {
                    return JsonEx(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = "验证用户失败，传入参数有误",
                        MsgId = entity.MsgId,
                        Value = "验证用户失败",
                    });
                }

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "验证用户失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
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
            foreach (WebPayItem item in baselist)
            {
                foreach (var getway in gateWayArray)
                {
                    if (getway.Split('=')[0] == item.gateway)
                    {
                        list.Add(buildPayUrl(item, os));
                    }
                }
                //if (!gateWayArray.Contains(item.gateway.ToLower()))
                //{
                //    continue;
                //}
                //list.Add(buildPayUrl(item, os));
            }
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
                string token = p.token;
                string bank = p.bank;
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("无效参数");
                }
                string amount = p.amount;
                if (!CheckInt(amount, 1, 100000))
                {
                    throw new Exception("充值金额无效");
                }

                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userToken"] = token;
                var lInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/LoginByUserToken");
                if (!lInfo.IsSuccess)
                {
                    throw new Exception(lInfo.Message);
                }
                var bankInfo = await _serviceProxyProvider.Invoke<C_BankCard>(param, "api/user/QueryBankCard");
                CallBackParam callBackParam = new CallBackParam();
                callBackParam.BankCardNo = "";
                if (bankInfo != null)
                {
                    callBackParam.BankCardNo = bankInfo.BankCardNumber;
                }
                var cui = new LoginInfo();
                cui = lInfo;
                callBackParam.UserId = cui.UserId;
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
                    Message = "失败" + "●" + ex.ToString(),
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


    }
}
