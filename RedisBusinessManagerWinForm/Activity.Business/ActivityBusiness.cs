using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activity.Core;
using Activity.Business.Domain.Managers;
using GameBiz.Core;
using Common.Communication;
using GameBiz.Business;

namespace Activity.Business
{
    public class ActivityBusiness : IOrderPrize_AfterTranCommit
    {
        public A20121009Info_Collection QueryA20121009Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                int totalCount;
                decimal totalAddMoney;
                A20121009Info_Collection collection = new A20121009Info_Collection();
                collection.ActListInfo = manage.QueryA20121009Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalAddMoney);
                collection.TotalCount = totalCount;
                collection.TotalAddMoney = totalAddMoney;
                //if (collection != null && collection.TotalCount > 0)
                //{

                //    collection.TotalAddMoney = collection.ActListInfo.Sum(a => a.AddMoney);
                //}
                //else
                //{
                //    collection.TotalAddMoney = 0;
                //}
                return collection;
            }
        }
        public A20120925Info_Collection QueryA20120925Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                int totalCount;
                decimal totalGiveMoney;
                decimal totalTransferMoney;
                A20120925Info_Collection collection = new A20120925Info_Collection();
                collection.ActListInfo = manage.QueryA20120925Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalGiveMoney, out totalTransferMoney);
                collection.TotalCount = totalCount;
                collection.TotalGiveMoney = totalGiveMoney;
                collection.TotalTransferMoney = totalTransferMoney;
                //if (collection != null && collection.TotalCount > 0)
                //{
                //    //var result = from s in collection.ActListInfo
                //    //             group s by s.Id into g
                //    //             select new A20120925Info
                //    //             {
                //    //                 GiveMoney = g.Sum(a => a.GiveMoney),
                //    //                 TransferMoney = g.Sum(a => a.TransferMoney)
                //    //             };
                //    collection.TotalGiveMoney = collection.ActListInfo.Sum(a => a.GiveMoney);
                //    collection.TotalTransferMoney = collection.ActListInfo.Sum(a => a.TransferMoney);
                //}
                //else
                //{
                //    collection.TotalGiveMoney = 0;
                //    collection.TotalTransferMoney = 0;
                //}
                return collection;
            }
        }
        public A20130807Info_Collection QueryA20130807Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                int totalCount;
                decimal totalFillMoney;
                decimal totalGiveMoney;
                A20130807Info_Collection collection = new A20130807Info_Collection();
                collection.ActListInfo = manage.QueryA20130807Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalFillMoney, out totalGiveMoney);
                collection.TotalCount = totalCount;
                collection.TotalFillMoney = totalFillMoney;
                collection.TotalGiveMoney = totalGiveMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalFillMoney = collection.ActListInfo.Sum(a => a.FillMoney);
                //    collection.TotalGiveMoney = collection.ActListInfo.Sum(a => a.GiveMoney);
                //}
                //else
                //{
                //    collection.TotalFillMoney = 0;
                //    collection.TotalGiveMoney = 0;
                //}
                return collection;
            }
        }

        public A20130808Info_Collection QueryA20130808Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                int totalCount;
                decimal totalGiveMoney;
                A20130808Info_Collection collection = new A20130808Info_Collection();
                collection.ActListInfo = manage.QueryA20130808Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalGiveMoney);
                collection.TotalCount = totalCount;
                collection.TotalGiveMoney = totalGiveMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalGiveMoney = collection.ActListInfo.Sum(a => a.GiveMoney);
                //}
                //else
                //{
                //    collection.TotalGiveMoney = 0;
                //}
                return collection;
            }
        }
        public A20120925CZGiveMoneyInfo_Collection QueryA20120925CZGiveMoneyInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20120925CZGiveMoneyInfo_Collection collection = new A20120925CZGiveMoneyInfo_Collection();
                int totalCount;
                decimal totalPayMoney;
                collection.ActListInfo = manage.QueryA20120925CZGiveMoneyInfo(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalPayMoney);
                collection.TotalCount = totalCount;
                collection.TotalPayMoney = totalPayMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalPayMoney = collection.ActListInfo.Sum(a => a.PayMoney);
                //}
                //else
                //{
                //    collection.TotalPayMoney = 0;
                //}
                return collection;
            }
        }
        public A20130903Info_Collection QueryA20130903Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20130903Info_Collection collection = new A20130903Info_Collection();
                int totalCount;
                decimal totalAddMoney;
                decimal totalOrderMoney;
                decimal totalAfterTaxBonusMoney;
                collection.ActListInfo = manage.QueryA20130903Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalAddMoney, out totalOrderMoney, out totalAfterTaxBonusMoney);
                collection.TotalCount = totalCount;
                collection.TotalAddMoney = totalAddMoney;
                collection.TotalOrderMoney = totalOrderMoney;
                collection.TotalAfterTaxBonusMoney = totalAfterTaxBonusMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalAddMoney = collection.ActListInfo.Sum(a => a.AddMoney);
                //    collection.TotalOrderMoney = collection.ActListInfo.Sum(a => a.OrderMoney);
                //    collection.TotalAfterTaxBonusMoney = collection.ActListInfo.Sum(a => a.AfterTaxBonusMoney);
                //}
                //else
                //{
                //    collection.TotalAddMoney = 0;
                //    collection.TotalOrderMoney = 0;
                //    collection.TotalAfterTaxBonusMoney = 0;
                //}
                return collection;
            }
        }
        public A20130903Info_Collection QueryA20130903ZYHBInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20130903Info_Collection collection = new A20130903Info_Collection();
                int totalCount;
                decimal totalAddMoney;
                decimal totalOrderMoney;
                decimal totalAfterTaxBonusMoney;
                collection.ActListInfo = manage.QueryA20130903ZYHBInfo(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out  totalAddMoney, out totalOrderMoney, out totalAfterTaxBonusMoney);
                collection.TotalCount = totalCount;
                collection.TotalAddMoney = totalAddMoney;
                collection.TotalOrderMoney = totalOrderMoney;
                collection.TotalAfterTaxBonusMoney = totalAfterTaxBonusMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalAddMoney = collection.ActListInfo.Sum(a => a.AddMoney);
                //    collection.TotalOrderMoney = collection.ActListInfo.Sum(a => a.OrderMoney);
                //    collection.TotalAfterTaxBonusMoney = collection.ActListInfo.Sum(a => a.AfterTaxBonusMoney);
                //}
                //else
                //{
                //    collection.TotalAddMoney = 0;
                //    collection.TotalOrderMoney = 0;
                //    collection.TotalAfterTaxBonusMoney = 0;
                //}
                return collection;
            }
        }
        public ActivityMonthReturnPointInfo_Colleciton QueryA20131101YHFDInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                ActivityMonthReturnPointInfo_Colleciton collection = new ActivityMonthReturnPointInfo_Colleciton();
                int totalCount;
                decimal totalGiveMoney;
                collection.ActListInfo = manage.QueryA20131101YHFDInfo(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalGiveMoney);
                collection.TotalCount = totalCount;
                collection.TotalGiveMoney = totalGiveMoney;

                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalGiveMoney = collection.Sum(a => a.TotalBetMoney);
                //}
                //else
                //{
                //    collection.TotalGiveMoney = 0;
                //}
                return collection;
            }
        }
        public A20131101InfoCollection QueryA20131101NewUserCZInfo(string userId, int IsGive, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20131101InfoCollection collection = new A20131101InfoCollection();
                int totalCount;
                decimal totalFillMoney;
                decimal totalGiveMoney;
                decimal totalNextMonthGiveMoney;
                collection.RecordList = manage.QueryA20131101NewUserCZInfo(userId, IsGive, startTime, endTime, pageIndex, pageSize, out totalCount, out totalFillMoney, out totalGiveMoney, out totalNextMonthGiveMoney);
                collection.TotalCount = totalCount;
                collection.TotalFillMoney = totalFillMoney;
                collection.TotalGiveMoney = totalGiveMoney;
                collection.TotalNextMonthGiveMoney = totalNextMonthGiveMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalFillMoney = collection.RecordList.Sum(a => a.FillMoney);
                //    collection.TotalGiveMoney = collection.RecordList.Sum(a => a.GivedMoney);
                //    collection.TotalNextMonthGiveMoney = collection.RecordList.Sum(a => a.NextMonthGiveMoney);
                //}
                //else
                //{
                //    collection.TotalFillMoney = 0;
                //    collection.TotalGiveMoney = 0;
                //    collection.TotalNextMonthGiveMoney = 0;
                //}
                return collection;
            }
        }
        public A20121128Info_Colleciton QueryA20121128Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20121128Info_Colleciton collection = new A20121128Info_Colleciton();
                int totalCount;
                decimal totalGiveMoney;
                collection.ActListInfo = manage.QueryA20121128Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalGiveMoney);
                collection.TotalCount = totalCount;
                collection.TotalGiveMoney = totalGiveMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalGiveMoney = collection.ActListInfo.Sum(a => a.GiveMoney);
                //}
                //else
                //{
                //    collection.TotalGiveMoney = 0;
                //}
                return collection;
            }
        }
        public A20140214Info_Collection QueryA20140214Info(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20140214Info_Collection collection = new A20140214Info_Collection();
                int totalCount;
                decimal totalFillMoney;
                decimal totalGiveMoney;
                collection.ActListInfo = manage.QueryA20140214Info(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalFillMoney, out totalGiveMoney);
                collection.TotalCount = totalCount;
                collection.TotalFillMoney = totalFillMoney;
                collection.TotalGiveMoney = totalGiveMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalFillMoney = collection.ActListInfo.Sum(a => a.FillMoney);
                //    collection.TotalGiveMoney = collection.ActListInfo.Sum(a => a.GiveMoney);
                //}
                //else
                //{
                //    collection.TotalFillMoney = 0;
                //    collection.TotalGiveMoney = 0;
                //}
                return collection;
            }

        }
        public A20140214SSCHBInfo_Collection QueryA20140214SSCHBInfo(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new ActivityManager())
            {
                A20140214SSCHBInfo_Collection collection = new A20140214SSCHBInfo_Collection();
                int totalCount;
                decimal totalAddMoney;
                decimal totalOrderMoney;
                decimal totalAfterTaxBonusMoney;
                collection.ActListInfo = manage.QueryA20140214SSCHBInfo(userId, startTime, endTime, pageIndex, pageSize, out totalCount, out totalAddMoney, out totalOrderMoney, out totalAfterTaxBonusMoney);
                collection.TotalCount = totalCount;
                collection.TotalAddMoney = totalAddMoney;
                collection.TotalOrderMoney = totalOrderMoney;
                collection.TotalAfterTaxBonusMoney = totalAfterTaxBonusMoney;
                //if (collection.TotalCount > 0)
                //{
                //    collection.TotalAddMoney = collection.ActListInfo.Sum(a => a.AddMoney);
                //    collection.TotalOrderMoney = collection.ActListInfo.Sum(a => a.OrderMoney);
                //    collection.TotalAfterTaxBonusMoney = collection.ActListInfo.Sum(a => a.AfterTaxBonusMoney);
                //}
                //else
                //{
                //    collection.TotalAddMoney = 0;
                //    collection.TotalOrderMoney = 0;
                //    collection.TotalAfterTaxBonusMoney = 0;
                //}
                return collection;
            }
        }
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            try
            {
                var business = new UserIntegralBusiness();
                UserGetPrizeInfo info = new UserGetPrizeInfo();
                info.OrderId = schemeId;
                info.PayInegral = Convert.ToInt32(orderMoney);
                info.PrizeType = "";
                info.UserId = userId;
                info.OrderMoey = orderMoney;
                info.CreateTime = DateTime.Now;
                business.UserIntegral(info, IntegralExchangeType.IntegralIn);
            }
            catch (Exception ex)
            {

            }
        }
        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
                return null;
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_A20130903_Error_", type, ex);
            }

            return null;
        }
    }
}
