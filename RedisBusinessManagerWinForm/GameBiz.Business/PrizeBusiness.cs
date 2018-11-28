using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Managers;
using GameBiz.Core;
using Common.Database.NHibernate;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Domain.Entities;
using GameBiz.Domain;
using Common.Business;
using Common.Lottery;
using System.Threading;
using Common.Net;
using Common.Utilities;
using Common;

namespace GameBiz.Business
{
    public class PrizeBusiness
    {
        public void IssusePrize(string gameCode, string issuseNumber, string winNumber)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                LotteryGameManager manager = new LotteryGameManager();
                var issuse = manager.QueryGameIssuse(gameCode, issuseNumber);
                if (issuse.Status == IssuseStatus.Stopped)
                    throw new Exception(string.Format("{0}第{1}期奖期状态不正确 - 不能是{2}", gameCode, issuseNumber, issuse.Status.ToString()));
                //throw new Exception(string.Format("{0}第{1}期奖期状态不正确 -应该是Awarded，而实际是:  {2}", gameCode, issuseNumber, issuse.Status.ToString()));

                issuse.WinNumber = winNumber;
                issuse.AwardTime = DateTime.Now;
                issuse.Status = IssuseStatus.Stopped;
                manager.UpdateGameIssuse(issuse);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 订单手工返钱
        /// </summary>
        public void ManualPayForOrder(string orderId, decimal money, string msg, string requestBy)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new SchemeManager();
                var orderDetail = manager.QueryOrderDetail(orderId);

                var fundBiz = new FundBusiness();
                fundBiz.ManualHandleMoney(orderId, orderId, money, AccountType.Bonus, PayType.Payin, orderDetail.UserId, string.Format("订单{0}手工返钱{1:N2}元,{2}", orderId, money, msg));

                biz.CommitTran();
            }
        }

    }
}
