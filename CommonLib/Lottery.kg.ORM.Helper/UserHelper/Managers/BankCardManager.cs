using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
    public class BankCardManager:DBbase
    {
        public void AddBankCard(C_BankCard entity)
        {
            DB.GetDal<C_BankCard>().Add(entity);
        }
        public void UpdateBankCard(C_BankCard entity)
        {
            DB.GetDal<C_BankCard>().Update(entity);
        }
        public void DeleteBankCard(C_BankCard entity)
        {
            DB.GetDal<C_BankCard>().Delete(entity);
        }
        public C_BankCard BankCardById(string userId)
        {
           
            return DB.CreateQuery<C_BankCard>().FirstOrDefault(p => p.UserId == userId);
        }

        public C_BankCard BankCardByCode(string bankCardNumber)
        {
           
            return DB.CreateQuery<C_BankCard>().FirstOrDefault(p => p.BankCardNumber == bankCardNumber);
        }

    }
}
