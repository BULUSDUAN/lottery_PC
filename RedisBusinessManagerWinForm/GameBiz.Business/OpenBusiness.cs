using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common.Database.NHibernate;
using GameBiz.Domain.Managers;
using GameBiz.Domain;
using Common.Business;

namespace GameBiz.Business
{
    public class OpenBusiness
    {
        public void IssuseOpen(string gameCode, string issuseNumber)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new LotteryGameManager();
                var issuse = manager.QueryGameIssuse(gameCode, issuseNumber);
                if (!string.IsNullOrEmpty(issuse.WinNumber))
                    throw new Exception(string.Format("{0}第{1}期已有开奖号", gameCode, issuseNumber));
                if (issuse.Status != IssuseStatus.OnSale)
                    throw new Exception(string.Format("{0}第{1}期奖期状态不正确 -应该是OnSale，而实际是:  {2}", gameCode, issuseNumber, issuse.Status.ToString()));
                if (issuse.Status == IssuseStatus.Awarded)
                    throw new Exception("已开奖");

                issuse.Status = IssuseStatus.Awarded;
                manager.UpdateGameIssuse(issuse);



                biz.CommitTran();
            }
        }

    }
}
