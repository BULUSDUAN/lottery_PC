using EntityModel;
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
                string key = "UserLottery.Service.IModuleServices.IUserService.User_Login_loginName_password_IPAddress";
                var UserImformation = await _serviceProxyProvider.Invoke<object>(param, "api/user/user_login");
               

            }
            catch (ArgumentException ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "登录失败", MsgId = entity.MsgId, Value = null });

            }
            return null;

        }
    }
}
