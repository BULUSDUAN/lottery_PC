using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using GameBiz.Core;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Business;

namespace GameBiz.Domain.Managers
{
    public class BankCardManager : GameBizEntityManagement
    {
        public void AddBankCard(BankCard entity)
        {
            this.Add(entity);
        }
        public void UpdateBankCard(BankCard entity)
        {
            this.Update(entity);
        }
        public void DeleteBankCard(BankCard entity)
        {
            this.Delete(entity);
        }
        public BankCard BankCardById(string userId)
        {
            Session.Clear();
            return Session.Query<BankCard>().FirstOrDefault(p => p.UserId == userId);
        }

        public BankCard BankCardByCode(string bankCardNumber)
        {
            Session.Clear();
            return Session.Query<BankCard>().FirstOrDefault(p => p.BankCardNumber == bankCardNumber);
        }
    }
}
