using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Helper;
using Lottery.ApiGateway.Model.Enum;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static KaSon.FrameWork.Helper.JsonHelper;

namespace Lottery.Api.Controllers
{
    [Area("Order")]
    public class OrderController : BaseController
    {
        /// <summary>
        /// 查询订单开奖历史记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public LotteryServiceResponse QueryOrderHistoryRecord(LotteryServiceRequest entity)
        {
            try
            {
                //读取json数据
                var param = WebHelper.Decode(entity.Param);
                //读取json文件
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var fileFullName = Path.Combine(path, string.Format("lottery_open_numbers_list_{0}.json", param.GameCode));
                var data = new GameWinNumber_InfoCollection();
                if (System.IO.File.Exists(fileFullName))
                {
                    var jsonData = System.IO.File.ReadAllText(fileFullName, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(jsonData.Trim()))
                    {
                        data = JsonHelper.Deserialize<GameWinNumber_InfoCollection>(jsonData);
                    }
                }

                var list = new List<object>();
                foreach (var item in data.List)
                {
                    list.Add(new
                    {
                        IssuseNumber = item.IssuseNumber,
                        WinNumber = item.WinNumber,
                        CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                    });
                }
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询订单开奖历史记录成功",
                    MsgId = entity.MsgId,
                    Value = list,
                };
            }
            catch (ArgumentException ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                };
            }
        }
        /// <summary>
        /// 查询中奖列表
        /// </summary>
        public async Task<IActionResult> QueryBonusList([FromServices]IServiceProxyProvider _serviceProxyProvider,LotteryServiceRequest entity)
        {
            try
            {
                
                Dictionary<string, object> param = new Dictionary<string, object>();
                var p = WebHelper.Decode(entity.Param);
                if (string.IsNullOrEmpty(p.gameCode))
                    throw new Exception("彩种不能为空");
                //param.userToken = p.UserToken;
                param.Add("gameCode", p.GameCode.ToUpper());
                param.Add("gameType", p.GameType.ToUpper());
                param.Add("pageIndex", p.PageIndex);
                param.Add("pageSize", p.PageSize);
                param.Add("key",p.KeyWord);
                var list = new List<object>();
                var _issuseNumber = string.Empty;
                var _completeData = string.Empty;
                
                var bonusList = await _serviceProxyProvider.Invoke<BonusOrderInfoCollection>(param, "api/Order/QueryBonusInfoList");
                if (bonusList != null && bonusList.BonusOrderList.Count > 0)
                {
                    foreach (var item in bonusList.BonusOrderList)
                    {
                        list.Add(new
                        {
                            SchemeId = item.SchemeId,
                            BonusMoney = item.AfterTaxBonusMoney,
                            DisplayName = item.DisplayName,
                            UserId = item.UserId,
                            SchemeType = (int)item.SchemeType,
                            BetCount = item.BetCount,
                            TotalMoney = item.TotalMoney,
                            IssuseNumber = string.IsNullOrEmpty(item.IssuseNumber) ? string.Empty : item.IssuseNumber,
                        });
                    }
                }
                return Json(new LotteryServiceResponse { Code = ResponseCode.成功, Message = "查询中奖列表成功", MsgId = entity.MsgId, Value = list });                
            }            
            catch (Exception ex)
            {               
                return Json(new LotteryServiceResponse { Code = ResponseCode.失败, Message = "查询中奖列表失败", MsgId = entity.MsgId, Value = null });
            }
        }
    }
}
