using Kason.Sg.Core.ProxyGenerator;
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
        //public async Task<IActionResult> QueryBonusList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = WebHelper.Decode(entity.Param);
        //        string loginName = p.LoginName;
        //        string password = p.Password;
        //        if (string.IsNullOrEmpty(loginName))
        //            throw new Exception("登录名不能为空");
        //        if (string.IsNullOrEmpty(password))
        //            throw new Exception("密码不能为空");
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return Json(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "查询中奖列表失败", MsgId = entity.MsgId, Value = null });

        //    }
          
        //}
    }
}
