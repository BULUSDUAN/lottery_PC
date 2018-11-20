using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using Common.Business;
using GameBiz.Domain.Entities;
using Common.Communication;

namespace GameBiz.Business
{
    public class BankCardBusiness
    {
        public BankCardInfo BankCardById(string userId)
        {
            var entity = new BankCardManager().BankCardById(userId);
            if (entity == null)
                return null;
            //throw new LogicException(string.Format("查不到{0}的银行卡信息", userId));
            return new BankCardInfo()
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

        public void AddBankCard(BankCardInfo bankCard)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new BankCardManager();
                var entity = new BankCard()
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


                //#region 发送站内消息：手机短信或站内信

                //var userManager = new UserBalanceManager();
                //var user = userManager.QueryUserRegister(bankCard.UserId);
                //var pList = new List<string>();
                //pList.Add(string.Format("{0}={1}", "[UserName]", user.DisplayName));
                //pList.Add(string.Format("{0}={1}", "[BankName]", bankCard.BankName));
                //pList.Add(string.Format("{0}={1}", "[BankCardNumber]", bankCard.BankCardNumber));
                ////发送短信
                //new SiteMessageControllBusiness().DoSendSiteMessage(user.UserId, "", "ON_User_Bind_BankCard", pList.ToArray());

                //#endregion

                biz.CommitTran();
            }
        }

        public void UpdateBankCard(BankCardInfo bankCard, string userId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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

                biz.CommitTran();
            }
        }


        public void CancelBankCard(string userId)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                var manager = new BankCardManager();
                var entity = manager.BankCardById(userId);
                if (entity == null)
                {
                    throw new Exception("未查到信息");
                }
                manager.DeleteBankCard(entity);

                biz.CommitTran();
            }
        }


    }
}
