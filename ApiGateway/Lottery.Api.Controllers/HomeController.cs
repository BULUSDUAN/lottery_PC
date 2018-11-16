using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using Kason.Sg.Core.ApiGateWay;
using Kason.Sg.Core.ApiGateWay.OAuth;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Routing;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Utilitys;
using Kason.Sg.Core.ApiGateWay.ServiceDiscovery;
using Kason.Sg.Core.CPlatform.Utilities;
using Lottery.ApiGateway.Model.HelpModel;
using Kason.Sg.Core.ApiGateWay.ServiceDiscovery.Implementation;
using Kason.Sg.Core.CPlatform.Address;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.ExceptionEx;
using KaSon.FrameWork.Common.Sport;
using EntityModel;
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    public class HomeController : BaseController
    {

        /// <summary>
        /// 获取所用服务的描述 
        /// 调试 http://127.0.0.1:729/api/Home/GetServiceDescriptor?address=127.0.0.1:98
        /// </summary>
        /// <param name="serviceDiscoveryProvider"></param>
        /// <param name="address"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAddress([FromServices]IServiceDiscoveryProvider serviceDiscoveryProvider, string address, string queryParam)
        {
            var adlist = await serviceDiscoveryProvider.GetAddressAsync();
            IList<object> list = new List<object>();
            foreach (ServiceAddressModel item in adlist)
            {
                var ip = item.Address as IpAddressModel;
                string str = ip.Ip + ":" + ip.Port;
                var lista = await serviceDiscoveryProvider.GetServiceDescriptorAsync(str);
                list.Add(lista);
            }
            return Json(new { A= adlist,B= list });
        }

        /// <summary>
        /// 通过路由调用服务
        /// </summary>
        /// 调试 http://127.0.0.1:729/api/Home/GetServiceByRouter  api/User/GetUser
        /// <param name="serviceDiscoveryProvider"></param>
        /// <param name="address"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetServiceByRouter([FromServices]IServiceProxyProvider _serviceProxyProvider, string address= "api/user/GetUserList")
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            model["userName"] = "userName";
            var sdt = DateTime.Now;
            var result = await _serviceProxyProvider.Invoke<object>(model, address);
            var edt = DateTime.Now;
            return Json(new { result= result, s =sdt.ToString("HH:mm:ss ffff"),e=edt.ToString("HH:mm:ss ffff") });
        }

        #region PC首页相关
        /// <summary>
        /// 首页新闻
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetIndexNews([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("category", "Lottery_GameCode");
                param.Add("gameCode", "JCZQ|JCLQ|BJDC");
                param.Add("pageIndex", 0);
                param.Add("pageSize", 4);
                //竞技彩
                var Jjc = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                param["gameCode"] = "SSQ|DLT|PL3|FC3D";
                //数字彩
                var Scz = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                param["gameCode"] = "JX11X5|CQSSC|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3";
                //高频彩
                var Gpc = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                param["gameCode"] = "";
                param["category"] = "Lottery_Hot";
                param["pageSize"] = 3;
                //今日热点
                var Cphot = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                param["category"] = "FocusCMS";
                param["pageSize"] = 10;
                //焦点新闻
                var FocusCMS = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询文章成功",
                    MsgId = "",
                    Value = new
                    {
                        Jjc = Jjc,
                        Scz = Scz,
                        Gpc = Gpc,
                        Cphot = Cphot,
                        FocusCMS = FocusCMS
                    },
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = "",
                    Value = string.Empty,
                });
            }
        }

        public async Task<IActionResult> GetSZCQuickBuy([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            try
            {
                var lotteryInfos = new Dictionary<string, LotteryInfo>();
                var gameCodeArray = new string[] { "SSQ", "DLT", "FC3D", "PL3" };
                var param = new Dictionary<string, object>();
                foreach (var gameCode in gameCodeArray)
                {
                    param.Clear();
                    param.Add("gameCode", gameCode);
                    var issuse = await _serviceProxyProvider.Invoke<LotteryIssuse_QueryInfo>(param, "api/Data/QueryCurrentIssuseByOfficialStopTime");
                    if (issuse == null) continue;

                    var pre = "";
                    if (issuse.GameCode == "SSQ" || issuse.GameCode == "DLT")
                        pre = BettingHelper.BuildLastIssuseNumber(issuse.GameCode, issuse.IssuseNumber);
                    lotteryInfos.Add(issuse.GameCode.ToLower(), new LotteryInfo
                    {
                        issue = issuse.IssuseNumber,
                        pre = pre,
                        win = "",
                        sale = 1,
                        betTime = ((issuse.OfficialStopTime - DateTime.Now).TotalSeconds - issuse.GameDelaySecond).ToString("0"),
                        awardTime = (issuse.OfficialStopTime - DateTime.Now).TotalSeconds.ToString("0")
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "快速购买期号查询成功",
                    MsgId = "",
                    Value = lotteryInfos,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "快速购买查询失败" + "●" + ex.ToString(),
                    MsgId = "",
                    Value = "快速购买查询失败",
                });
            }
        }
        #endregion
    }
}
