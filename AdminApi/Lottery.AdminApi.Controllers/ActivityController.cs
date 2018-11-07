using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// 活动管理控制器
    /// </summary>
    [Area("api")]
    [ReusltFilter]
    [EnableCors("any")]
    public class ActivityController : BaseController
    {
        private static readonly AdminService _service = new AdminService();
        
        #region 活动管理 
        /// <summary>
        /// 网站活动配置
        /// </summary>
        public IActionResult GetActivityConfigList()
        {
            try
            {
                if (!CheckRights("HDPZ"))
                    throw new Exception("对不起，您的权限不足！");
                var ConfigArray = _service.GetActityConfig().Split('|');
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = ConfigArray });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
         
        }

        public IActionResult ModifyActivityConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("HDPZ_XG"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                
                string key = p.config_key;
                string value = p.config_value;
                var r = _service.UpdateActivityConfig(key, value);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败 , Message = ex.Message });
            }
        }

        /// <summary>
        /// 充值赠送红包配置
        /// </summary>
        public IActionResult GetFillGiveRedBagConfigList()
        {
            try
            {
                if (!CheckRights("CZZSPZ"))
                    throw new Exception("对不起，您的权限不足！");
                var ConfigList = _service.QueryFillMoneyGiveRedBagConfigList();

                return Json(new LotteryServiceResponse() {Code=AdminResponseCode.成功,Value=ConfigList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult AddFillGiveRedBagConfig(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                decimal fillMoney = p.fillMoney;
                decimal giveMoney = p.giveMoney;
                var r =_service.AddFillMoneyGiveRedBagConfig(fillMoney, giveMoney);
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult UpdateFillGiveRedBagConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZZSPZ_GXGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                int id = p.id;
                decimal fillMoney = p.fillMoney;
                decimal giveMoney = p.giveMoney;
                var r = _service.UpdateFillMoneyGiveRedBagConfig(id, fillMoney, giveMoney);
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult DeleteFillGiveRedBagConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZZSPZ_SCGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                int id = p.id;
                var r = _service.DeleteFillMoneyGiveRedBagConfig(id);
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new { Code = AdminResponseCode.失败, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 红包使用规则
        /// </summary>
        public IActionResult GetRedBagUseConfigList()
        {
            try
            {
                if (!CheckRights("HBSYGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var List = _service.QueryRedBagUseConfig();
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Value = List });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult AddRedBagConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("HBSYGZ_TJGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                string gameCode = p.gameCode;
                decimal percent = p.percent;
                var r =_service.AddRedBagUseConfig(gameCode, percent);
                return Json(new LotteryServiceResponse{ Code =AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult DeleteRedBagUseConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("HBSYGZ_SCGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                int id = p.id;
                var r =_service.DeleteRedBagUseConfig(id);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        /// <summary>
        /// 彩种加奖配置
        /// </summary>
        public IActionResult GetAddBonusMoneyConfigList()
        {
            try
            {
                if (!CheckRights("CZJJPZ"))
                    throw new LogicException("对不起，您的权限不足！");
                var ConfigList = _service.QueryAddBonusMoneyConfig();
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = ConfigList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult AddAddBonusMoneyConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZJJPZ_TJGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                int orderIndex = int.Parse((string)p.orderIndex);
                string gameCode = p.gameCode;
                string gameType = p.gameType;
                string playType = p.playType;
                decimal percent = decimal.Parse((string)p.percent);
                decimal maxMoney = decimal.Parse((string)p.maxMoney);
                string addMoneyWay = p.addMoneyWay;
                var r = _service.AddAddBonusMoneyConfig(gameCode, gameType, playType, percent, maxMoney, orderIndex, addMoneyWay);
                return Json(new LotteryServiceResponse { Code =AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse{ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult DeleteAddBonusMoneyConfig(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZJJPZ_SCGZ"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                int id = int.Parse((string)p.id);
                var r = _service.DeleteAddBonusMoneyConfig(id);
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        /// <summary>
        /// 彩种取消加奖用户列表
        /// </summary>
        public IActionResult GetUserGameCodeNotAddMoneyList(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string UserId = string.IsNullOrWhiteSpace((string)p.userId) ? string.Empty : ((string)p.userId).ToString();
                var UserGameCodeNotAddMoneyList = _service.QueryUserGameCodeNotAddMoneyList(UserId);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = UserGameCodeNotAddMoneyList });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult AddUserGameCodeNotAddMoney(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userIdList = p.userId;
                string gameCode = p.gameCode;
                string gameType = p.gameType;
                string playType = p.playType;
                if (string.IsNullOrEmpty(gameCode))
                    throw new LogicException("彩种传入不能为空。");

                var msgList = new List<string>();
                foreach (var userId in userIdList.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (string.IsNullOrEmpty(userId))
                        continue;
                    try
                    {
                        var result =_service.AddUserGameCodeNotAddMoney(userId, gameCode, gameType, playType);
                        msgList.Add(string.Format("用户{0}添加结果：{1}", userId, result.Message));
                    }
                    catch (Exception ex)
                    {
                        msgList.Add(string.Format("用户{0}添加结果：{1}", userId, ex.Message));
                    }
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = msgList });
                //return Json(new { IsSuccess = true, Msg = string.Join("\r\n", msgList.ToArray()) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public IActionResult DeleteUserGameCodeNotAddMoney(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int id = int.Parse((string)p.id);
                var r =_service.DeleteUserGameCodeNotAddMoney(id);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = r.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        /// <summary>
        /// 充值类型赠送配置
        /// </summary>
        public IActionResult GetPayRedBagConfigList()
        {
            try
            {
                Dictionary<int, decimal> dict = new Dictionary<int, decimal>();
                var payRedBagConfig = _service.QueryCoreConfigByKey("PayRedBagConfig").ConfigValue;
                if (!string.IsNullOrEmpty(payRedBagConfig))
                {
                    var configs = payRedBagConfig.Split(',');
                    int i = 0;
                    decimal f = 0;
                    foreach (var item in configs)
                    {
                        var arr = item.Split('|');
                        if (arr.Length == 2)
                        {
                            i = 0;
                            f = 0;
                            int.TryParse(arr[0], out i);
                            decimal.TryParse(arr[1], out f);
                            dict.Add(i, f);
                        }
                    }
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = dict });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdatePayRedBagConfig(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string value = p.postDic.ToString().TrimEnd(',');
                var result = _service.UpdateCoreConfigInfoByKey("PayRedBagConfig", value);
                if (result.IsSuccess)
                    return Json(new LotteryServiceResponse{ Code =AdminResponseCode.成功, Message = "更新数据成功" });
                else
                    return Json(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = "更新数据失败" + result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }


        #endregion
    }
}
