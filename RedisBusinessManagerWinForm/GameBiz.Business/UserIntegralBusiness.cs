using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Domain.Entities;
using GameBiz.Business.Domain.Managers;
using Common.Utilities;

namespace GameBiz.Business
{
    public class UserIntegralBusiness
    {
        /// <summary>
        /// 用户积分操作
        /// </summary>
        /// <param name="info">详细信息</param>
        /// <param name="opraType">操作类型:100:收入,如购彩增加积分；110:支出,如兑换奖品;</param>
        /// <returns></returns>
        public bool UserIntegral(UserGetPrizeInfo info, IntegralExchangeType payType)
        {
            try
            {
                using (var biz = new GameBizBusinessManagement())
                {
                    biz.BeginTran();
                    if (info != null && !string.IsNullOrEmpty(info.UserId))
                    {
                        if (payType == IntegralExchangeType.IntegralIn || payType == IntegralExchangeType.IntegralOut)//增加积分或兑换奖品
                        {
                            SaveUserIntegralDetail(info, payType);
                            SaveUserIntegral(info, payType);
                            if (info.OrderMoey >= 100)
                            {
                                GivePrizes(info);//赠送彩金
                            }
                        }
                    }
                    biz.CommitTran();
                    return true;
                }
            }
            catch {
                return false;
            }
        }
        public void SaveUserIntegral(UserGetPrizeInfo info, IntegralExchangeType opraType)
        {
            try
            {
                var manage = new UserIntegralManager();
                UserIntegralBalance entity = manage.GetUserIntegralBalance(info.UserId);
                if (entity != null && !string.IsNullOrEmpty(entity.UserId))//修改
                {
                    if (opraType == IntegralExchangeType.IntegralIn)//增加积分
                    {
                        entity.CurrIntegralBalance += info.PayInegral;
                        manage.UpdateUserIntegralBalance(entity);

                    }
                    else if (opraType == IntegralExchangeType.IntegralOut)//兑换奖品
                    {
                        if (info.PayInegral > entity.CurrIntegralBalance)
                            return;
                        ExchangePresent(info);
                        entity.CurrIntegralBalance -= info.PayInegral;
                        entity.UseIntegralBalance += info.PayInegral;
                        manage.UpdateUserIntegralBalance(entity);//修改总的资金明细                            
                    }
                }
                else//新增
                {
                    if (opraType == IntegralExchangeType.IntegralIn)
                    {
                        entity = new UserIntegralBalance();
                        entity.CurrIntegralBalance += info.PayInegral;
                        entity.UseIntegralBalance = 0;
                        entity.CreateTime = DateTime.Now;
                        entity.UserId = info.UserId;
                        manage.AddUserIntegralBalance(entity);
                    }
                    else
                    {
                        throw new Exception("您还没有积分，不能兑换奖品或领取彩金！");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void SaveUserIntegralDetail(UserGetPrizeInfo info, IntegralExchangeType opraType)
        {
            try
            {
                    var manage = new UserIntegralManager();
                    UserIntegralBalance entity = manage.GetUserIntegralBalance(info.UserId);
                    if (entity == null && opraType == IntegralExchangeType.IntegralOut)
                    {
                        throw new Exception("您还没有积分，不能兑换奖品或领取彩金！");
                    }
                    UserIntegralDetail detailEntity = new UserIntegralDetail();
                    detailEntity.UserId = info.UserId;
                    detailEntity.PayIntegral = info.PayInegral;
                    detailEntity.BeforeIntegral = entity == null ? 0 : entity.CurrIntegralBalance;
                    if (opraType == IntegralExchangeType.IntegralIn)
                    {

                        detailEntity.OrderId = info.OrderId;
                        detailEntity.Summary = "购彩" + info.OrderMoey + "元，增加积分" + Convert.ToInt32(info.OrderMoey) + "点";
                        detailEntity.AfterIntegral = detailEntity.BeforeIntegral + detailEntity.PayIntegral;
                    }
                    else if (opraType == IntegralExchangeType.IntegralOut)
                    {
                        detailEntity.OrderId = "";
                        if (info.PayInegral > entity.CurrIntegralBalance)
                            return;
                        if (info.PayInegral == 100000)
                        {
                            detailEntity.Summary = "用户10万积分，兑换IPhone5S手机一部";
                        }
                        else if (info.PayInegral == 1000000)
                        {
                            detailEntity.Summary = "用户100万积分，兑换QQ3汽车一辆";
                        }

                        detailEntity.AfterIntegral = detailEntity.BeforeIntegral - detailEntity.PayIntegral;
                    }
                    detailEntity.CreateTime = DateTime.Now;
                    manage.AddUserIntegralDetail(detailEntity);
                   
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 赠送彩金
        /// </summary>
        /// <param name="info"></param>
        public void GivePrizes(UserGetPrizeInfo info)
        {
            var manage = new UserIntegralManager();
            Dictionary<decimal, decimal> dic = new Dictionary<decimal, decimal>();
            dic.Add(100, 2);
            dic.Add(500, 5);
            dic.Add(2000, 10);
            dic.Add(10000, 50);
            dic.Add(100000, 588);
            dic.Add(500000, 2888);

            UserIntegralBalance entity = manage.GetUserIntegralBalance(info.UserId);
            var query = from s in dic where s.Key <= entity.CurrIntegralBalance select s;
            if (query != null)
            {
                foreach (var item in query)
                {
                    int vipLevel = 0;
                    //[10:IPHONE5s；20：QQ3；30：送2元；40：送5元；50：送10元；60：送58元；70：送588元；80：送2888元]
                    switch (item.Key.ToString())
                    {
                        case "100":
                            info.PrizeType = "30";
                            vipLevel = 1;
                            break;
                        case "500":
                            info.PrizeType = "40";
                            vipLevel = 2;
                            break;
                        case "2000":
                            info.PrizeType = "50";
                            vipLevel = 3;
                            break;
                        case "10000":
                            info.PrizeType = "60";
                            vipLevel = 4;
                            break;
                        case "100000":
                            info.PrizeType = "70";
                            vipLevel = 5;
                            break;
                        case "500000":
                            info.PrizeType = "80";
                            vipLevel = 6;
                            break;
                    }

                    if (!manage.UserIsPrize(info.UserId, info.PrizeType))//判断是否已赠送过彩金
                    {
                        UserGetPrize PrizeEntity = new UserGetPrize();
                        PrizeEntity.GiveMoney = item.Value;
                        PrizeEntity.PayInegral = Convert.ToInt32(item.Key);
                        PrizeEntity.PrizeType = info.PrizeType;
                        PrizeEntity.OrderMoney = info.OrderMoey;
                        PrizeEntity.Summary = "用户积分满" + Convert.ToInt32(item.Key) + ",赠送彩金" + item.Value.ToString("N2") + "元";
                        PrizeEntity.UserId = info.UserId;
                        PrizeEntity.CreateTime = DateTime.Now;
                        manage.AddUserGetPrize(PrizeEntity);//增加用户领奖记录

                        //BusinessHelper.Payin_2Balance(BusinessHelper.FundCategory_Activity, info.UserId, info.UserId, false, "", "", info.UserId, AccountType.Common, PrizeEntity.GiveMoney,
                        //     PrizeEntity.Summary);
                        manage.UpdateUserRegister(info.UserId, vipLevel);
                    }
                }

            }
        }
        /// <summary>
        /// 兑换礼品
        /// </summary>
        public void ExchangePresent(UserGetPrizeInfo info)
        {
            var manage = new UserIntegralManager();
            string Summary = string.Empty;
            if (info.PayInegral == 100000)
            {
                info.PrizeType = "10";
                Summary = "用户10万积分，兑换IPhone5S手机一部";
            }
            else if (info.PayInegral == 1000000)
            {
                info.PrizeType = "20";
                Summary = "用户100万积分，兑换QQ3汽车一辆";
            }
            if (!manage.UserIsPrize(info.UserId, info.PrizeType))//判断是否已兑换过礼品
            {
                UserGetPrize PrizeEntity = new UserGetPrize();
                PrizeEntity.GiveMoney =0;
                PrizeEntity.PayInegral = info.PayInegral;
                PrizeEntity.PrizeType = info.PrizeType;
                PrizeEntity.Summary = Summary;
                PrizeEntity.UserId = info.UserId;
                PrizeEntity.CreateTime = DateTime.Now;
                manage.AddUserGetPrize(PrizeEntity);//增加用户领奖记录
            }
        }
        /// <summary>
        /// 保存礼品配置
        /// </summary>
        public void SaveOrUpdateActivityPrizeConfig(ActivityPrizeConfigInfo info, string opeType)
        {
            using (var manage = new UserIntegralManager())
            {
                ActivityPrizeConfig entity = new ActivityPrizeConfig();
                if (opeType.ToLower() == "add")
                {
                    if (info != null)
                    {
                        entity.ActivityTitle = info.ActivityTitle;
                        entity.ActivityContent = info.ActivityContent;
                        entity.IsEnabled = info.IsEnabled;
                        entity.CreatorId = info.CreatorId;
                        entity.CreateTime = DateTime.Now;
                        manage.AddActivityPrizeConfig(entity);
                    }
                    else
                    {
                        throw new Exception("修改数据失败！");
                    }
                }
                else if (opeType.ToLower() == "update")
                {
                    if (info != null)
                    {
                        entity = manage.GetActivityPrizeConfig(info.ActivityId);
                        if (entity != null)
                        {
                            entity.ActivityTitle = info.ActivityTitle;
                            entity.ActivityContent = info.ActivityContent;
                            entity.IsEnabled = info.IsEnabled;
                            manage.UpdateActivityPrizeConfig(entity);
                        }
                        else
                        {
                            throw new Exception("未查询到数据！");
                        }
                    }
                    else
                    {
                        throw new Exception("修改数据失败！");
                    }
                }
            }
        }
        public void DeleteActivityPrizeConfig(int activityId)
        {
            using (var manage = new UserIntegralManager())
            {
                manage.DeleteActivityPrizeConfig(activityId);
            }
        }

        public ActivityPrizeConfigInfo_Collection QueryActivityPrizeConfigCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new UserIntegralManager())
            {
                ActivityPrizeConfigInfo_Collection colleciton = new ActivityPrizeConfigInfo_Collection();
                int totalCount;
                colleciton.ActConfigList = manage.QueryActivityPrizeConfigCollection(title, startTime, endTime, pageIndex, pageSize,out totalCount);
                colleciton.TotalCount = totalCount;
                return colleciton;

            }
        }
        public ActivityPrizeConfigInfo GetActivityPrizeConfig(int activityId)
        {
            using (var manage = new UserIntegralManager())
            {
                ActivityPrizeConfig entity=manage.GetActivityPrizeConfig(activityId);
                if(entity!=null)
                {
                    ActivityPrizeConfigInfo info=new ActivityPrizeConfigInfo ();
                    ObjectConvert.ConverEntityToInfo(entity, ref info);
                    return info;
                }
                return null;
            }
        }
    }
}
