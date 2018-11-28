using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Activity.Core;
using Activity.Domain.Entities;

namespace Activity.Business.Domain.Managers
{
    public class ActivityManager : GameBizEntityManagement
    {
        public List<A20121009Info> QueryA20121009Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalAddMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from a in Session.QueryOver<A20121009>().List()
                        where (a.UserId == string.Empty || a.UserId == userId) && (a.CreateTime >= sTime && a.CreateTime < eTime.AddDays(1))
                        select new A20121009Info
                        {
                             Id =a.Id,
                             UserId =a.UserId,
                             SchemeType =a.SchemeType,
                             SchemeId =a.SchemeId,
                             GameType=a.GameType,
                             IssuseNumber =a.IssuseNumber,
                             HitMatchCount =a.HitMatchCount,
                             AddMoney =a.AddMoney,
                             CreateTime =a.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalAddMoney = query.Sum(a => a.AddMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<A20120925Info> QueryA20120925Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalGiveMoney, out decimal totalTransferMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from a in Session.QueryOver<A20120925_奖金转入送1>().List()
                        where (userId == string.Empty || a.UserId == userId) && (a.CreateTime >= sTime && a.CreateTime < eTime.AddDays(1))
                        select new A20120925Info
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            TransferMoney = a.TransferMoney,
                            GiveMoney = a.GiveMoney,
                            CreateTime = a.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalGiveMoney = query.Sum(a => a.GiveMoney);
            totalTransferMoney = query.Sum(a => a.TransferMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //totalCount = query.RowCount();
            //return query.Skip(pageIndex * pageSize).Take(pageSize).List<A20120925Info>().ToList();
        }
        public List<A20130807Info> QueryA20130807Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalFillMoney, out decimal totalGiveMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from a in Session.QueryOver<A20130807_充值送钱>().List()
                        where (userId == string.Empty || a.UserId == userId) && (a.UpdateTime >= sTime && a.UpdateTime < eTime.AddDays(1))
                        select new A20130807Info
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            OrderId = a.OrderId,
                            FillMoney = a.FillMoney,
                            GiveMoney = a.GiveMoney,
                            UpdateTime = a.UpdateTime
                        };
            totalCount = query.ToList().Count;
            totalFillMoney = query.Sum(a => a.FillMoney);
            totalGiveMoney = query.Sum(a => a.GiveMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<A20130808Info> QueryA20130808Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalGiveMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from a in Session.QueryOver<A20130808_注册送3元>().List()
                        where (userId == string.Empty || a.UserId == userId) && (a.UpdateTime >= sTime && a.UpdateTime < eTime.AddDays(1))
                        select new A20130808Info
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            RealName = a.RealName,
                            Mobile = a.Mobile,
                            GiveMoney = a.GiveMoney,
                            UpdateTime = a.UpdateTime
                        };
            totalCount = query.ToList().Count;
            totalGiveMoney = query.Sum(a => a.GiveMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<A20120925CZGiveMoneyInfo> QueryA20120925CZGiveMoneyInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalPayMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());

            var query = from s in Session.QueryOver<A20120925_认证送彩金>().List()
                        join f in Session.QueryOver<GameBiz.Domain.Entities.FundDetail>().List() on s.UserId equals f.UserId
                        where (f.PayType == Common.PayType.Payin && f.AccountType == 0 && f.Summary == "用户完成手机认证以及实名认证，首次充值大于20赠送10元") && (userId == string.Empty || s.UserId == userId) && (s.UpdateTime >= sTime && s.UpdateTime < eTime.AddDays(1))
                        select new A20120925CZGiveMoneyInfo
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            PayMoney = f.PayMoney,
                            UpdateTime = s.UpdateTime

                        };
            totalCount = query.ToList().Count;
            totalPayMoney = query.Sum(a => a.PayMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<A20130903Info> QueryA20130903Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalAddMoney, out decimal totalOrderMoney, out decimal totalAfterTaxBonusMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20130903_JCZQ加奖>().List()
                        join c in Session.QueryOver<GameBiz.Domain.Entities.LotteryGame>().List() on s.GameCode equals c.GameCode
                        where s.afterTaxBonusMoney > 0 && (userId == string.Empty || s.UserId == userId) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new A20130903Info
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            SchemeId = s.SchemeId,
                            GameCode = s.GameCode,
                            GameCodeDisplayName = c.DisplayName,
                            GameType = s.GameType,
                            IssuseNumber = s.IssuseNumber,
                            AddMoney = s.AddMoney,
                            OrderMoney = s.OrderMoney,
                            AfterTaxBonusMoney = s.afterTaxBonusMoney,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalAddMoney = query.Sum(a => a.AddMoney);
            totalAfterTaxBonusMoney = query.Sum(a => a.AfterTaxBonusMoney);
            totalOrderMoney = query.Sum(a => a.OrderMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<A20130903Info> QueryA20130903ZYHBInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalAddMoney, out decimal totalOrderMoney, out decimal totalAfterTaxBonusMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20130903_JCZQ加奖>().List()
                        join c in Session.QueryOver<GameBiz.Domain.Entities.LotteryGame>().List() on s.GameCode equals c.GameCode
                        where s.afterTaxBonusMoney == 0 && s.OrderMoney >= 50 && (userId == string.Empty || s.UserId == userId) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new A20130903Info
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            SchemeId = s.SchemeId,
                            GameCode = s.GameCode,
                            GameCodeDisplayName = c.DisplayName,
                            GameType = s.GameType,
                            //GameTypeDisplayName = t.DisplayName,
                            IssuseNumber = s.IssuseNumber,
                            AddMoney = s.AddMoney,
                            OrderMoney = s.OrderMoney,
                            AfterTaxBonusMoney = s.afterTaxBonusMoney,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalAddMoney = query.Sum(a => a.AddMoney);
            totalAfterTaxBonusMoney = query.Sum(a => a.AfterTaxBonusMoney);
            totalOrderMoney = query.Sum(a => a.OrderMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<ActivityMonthReturnPointInfo> QueryA20131101YHFDInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalGiveMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20131101_用户返点>().List()
                        where (userId == string.Empty || s.UserId == userId) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new ActivityMonthReturnPointInfo
                        {

                            Id = s.Id,
                            UserId = s.UserId,
                            UserDisplayName = "",
                            Month = s.Month,
                            TotalBetMoney = s.TotalMoney,
                            GiveMoney = s.GiveMoney,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalGiveMoney = query.Sum(a => a.GiveMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<A20131101Info> QueryA20131101NewUserCZInfo(string userId, int IsGive, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalFillMoney, out decimal totalGiveMoney, out decimal totalNextMonthGiveMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20131101_新用户首充送钱>().List()
                        where (userId == string.Empty || s.UserId == userId) && (IsGive == -1 || s.IsGiveComplate == Convert.ToBoolean(IsGive)) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new A20131101Info
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            UserDisplayName = "",
                            OrderId = s.OrderId,
                            FillMoney = s.FillMoney,
                            GivedMoney = s.CurrentGiveMoney,
                            NextMonthGiveMoney = s.NextMonthGiveMoney,
                            NextMonth = s.NextMonth,
                            GiveComplate = s.IsGiveComplate,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalFillMoney = query.Sum(a => a.FillMoney);
            totalNextMonthGiveMoney = query.Sum(a => a.NextMonthGiveMoney);
            totalGiveMoney = query.Sum(a => a.GivedMoney)+totalNextMonthGiveMoney;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<A20121128Info> QueryA20121128Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalGiveMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20121128_首次中奖超过100送5元>().List()
                        join c in Session.QueryOver<GameBiz.Domain.Entities.LotteryGame>().List() on s.GameCode equals c.GameCode
                        where (userId == string.Empty || s.UserId == userId) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new A20121128Info
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            SchemeId = s.SchemeId,
                            GameCode = s.GameCode,
                            GameCodeDisplayName = c.DisplayName,
                            GameType = s.GameType,
                            IssuseNumber = s.IssuseNumber,
                            GiveMoney = s.GiveMoney,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalGiveMoney = query.Sum(a => a.GiveMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<A20140214Info> QueryA20140214Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalFillMoney, out decimal totalGiveMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20140214_首次充20送11>().List()
                        where (userId == string.Empty || s.UserId == userId) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new A20140214Info
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            OrderId = s.OrderId,
                            FillMoney = s.FillMoney,
                            GiveMoney = s.GiveMoney,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalFillMoney = query.Sum(a => a.FillMoney);
            totalGiveMoney = query.Sum(a => a.GiveMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<A20140214SSCHBInfo> QueryA20140214SSCHBInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount, out decimal totalAddMoney, out decimal totalOrderMoney, out decimal totalAfterTaxBonusMoney)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from s in Session.QueryOver<A20140214_SSC红包>().List()
                        join c in Session.QueryOver<GameBiz.Domain.Entities.LotteryGame>().List() on s.GameCode equals c.GameCode
                        where (userId == string.Empty || s.UserId == userId) && (s.CreateTime >= sTime && s.CreateTime < eTime.AddDays(1))
                        select new A20140214SSCHBInfo
                        {
                            Id = s.Id,
                            UserId = s.UserId,
                            SchemeId = s.SchemeId,
                            GameCode = s.GameCode,
                            GameCodeDisplayName = c.DisplayName,
                            GameType = s.GameType,
                            IssuseNumber = s.IssuseNumber,
                            AddMoney = s.AddMoney,
                            OrderMoney = s.OrderMoney,
                            AfterTaxBonusMoney = s.afterTaxBonusMoney,
                            CreateTime = s.CreateTime
                        };
            totalCount = query.ToList().Count;
            totalAddMoney = query.Sum(a => a.AddMoney);
            totalAfterTaxBonusMoney = query.Sum(a => a.AfterTaxBonusMoney);
            totalOrderMoney = query.Sum(a => a.OrderMoney);
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
