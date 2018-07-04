using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
   public class BankCardBusiness:DBbase
    {
        public C_BankCard BankCardById(string userId)
        {
            var entity = new BankCardManager().BankCardById(userId);
            if (entity == null)
                return null;
            //throw new LogicException(string.Format("查不到{0}的银行卡信息", userId));
            return new C_BankCard()
            {
                UserId = entity.UserId,
                BankCardNumber = entity.BankCardNumber,
                BankCode = entity.BankCode,
                BankName = entity.BankName,
                BankSubName = entity.BankSubName,
                BId = entity.BId,
                CityName = entity.CityName,
                CreateTime = entity.CreateTime,
                ProvinceName = entity.ProvinceName,
                RealName = entity.RealName,
                UpdateTime = entity.UpdateTime
            };
        }

        public void AddBankCard(C_BankCard bankCard)
        {
           
                DB.Begin();
                var manager = new BankCardManager();
                var entity = new C_BankCard()
                {
                    UserId = bankCard.UserId,
                    BankCardNumber = bankCard.BankCardNumber,
                    BankCode = bankCard.BankCode,
                    BankName = bankCard.BankName,
                    BankSubName = bankCard.BankSubName,
                    CityName = bankCard.CityName,
                    CreateTime = DateTime.Now,
                    ProvinceName = bankCard.ProvinceName,
                    RealName = bankCard.RealName,
                    UpdateTime = DateTime.Now
                };
                manager.AddBankCard(entity);            
                DB.Commit();
            
        }

        public void UpdateBankCard(C_BankCard bankCard, string userId)
        {
           
                DB.Begin();
                var manager = new BankCardManager();
                var entity = manager.BankCardById(userId);
                if (entity == null)
                {
                    throw new Exception("修改信息未被查询到");
                }
                entity.BankCardNumber = bankCard.BankCardNumber;
                entity.BankCode = bankCard.BankCode;
                entity.BankName = bankCard.BankName;
                entity.BankSubName = bankCard.BankSubName;
                entity.CityName = bankCard.CityName;
                entity.ProvinceName = bankCard.ProvinceName;
                entity.RealName = bankCard.RealName;
                entity.UpdateTime = DateTime.Now;
                manager.UpdateBankCard(entity);
                DB.Commit();
               
            }



        public void CancelBankCard(string userId)
        {

            DB.Begin();
            var manager = new BankCardManager();
            var entity = manager.BankCardById(userId);
            if (entity == null)
            {
                throw new Exception("未查到信息");
            }
            manager.DeleteBankCard(entity);
            DB.Commit();

        }
    }
}
