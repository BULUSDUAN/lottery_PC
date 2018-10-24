using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class BankCardBusiness : DBbase
    {
        public C_BankCard BankCardById(string userId)
        {
            return new BankCardManager().BankCardById(userId);
        }

        public void AddBankCard(C_BankCard bankCard)
        {
            //try
            //{
            //    DB.Begin();
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
            //    DB.Commit();
            //}
            //catch (Exception ex)
            //{
            //    DB.Rollback();
            //    throw ex;
            //}
        

        }

        public void UpdateBankCard(C_BankCard bankCard, string userId)
        {

            //DB.Begin();
            //try
            //{
                var manager = new BankCardManager();
                var entity = manager.BankCardById(userId);
                if (entity == null)
                {
                    //DB.Rollback();
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
            //    DB.Commit();
            //}
            //catch (Exception ex)
            //{
            //    DB.Rollback();
            //    throw ex;
            //}
          

        }



        public void CancelBankCard(string userId)
        {
            //try
            //{
            //    DB.Begin();
                var manager = new BankCardManager();
                var entity = manager.BankCardById(userId);
                if (entity == null)
                {
                    DB.Rollback();
                    throw new Exception("未查到信息");
                }
                manager.DeleteBankCard(entity);
            //    DB.Commit();
            //}
            //catch (Exception ex)
            //{
            //    DB.Rollback();
            //    throw ex;
            //}
          

        }
    }
}
