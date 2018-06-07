using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Helper.Net;
using Lottery.ApiGateway.Model.Enum;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static KaSon.FrameWork.Helper.JsonHelper;

namespace Lottery.Api.Controllers
{
    [Area("User")]
    public class UserController : BaseController
    {
        /// <summary>
        /// 登录
        /// </summary>
        public async Task<IActionResult> UserLogin([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                //var p = WebHelper.Decode(entity.Param);
                string loginName = "15033333333";
                string password = "123456";
                if (string.IsNullOrEmpty(loginName))
                    throw new Exception("登录名不能为空");
                if (string.IsNullOrEmpty(password))
                    throw new Exception("密码不能为空");
                //param.Add("model", new QueryUserParam());IPAddress
                param["loginName"] = loginName;
                param["password"]=password;
                param["IPAddress"] = password;
           
                var UserImformation = await _serviceProxyProvider.Invoke<object>(param, "api/user/user_login");
               

            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "登录失败", MsgId = entity.MsgId, Value = null });

            }
            return null;

        }

        /// <summary>
        /// 修改密码
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
                //var chkPwd = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/CheckIsSame2BalancePassword");

                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/user/ChangeMyPassword");

                if (!result.IsSuccess) {
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

    }
}
