using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖
    /// </summary>
   public class TeMaOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public TeMaOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
            string tm = winNum.Split('|')[1];
            string zm = winNum.Split('|')[0];

            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (tm.Trim() == orderdetail.AnteCodes.Trim())
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney = orderdetail.unitPrice * Odds*orderdetail.BeiSu;
                BonusStatus = 2;
            }
           
            DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
            {
                winNumber = winNum,
                BonusAwardsMoney = 0,
                updateTime = DateTime.Now,
                BonusStatus = BonusStatus  //为中奖状态


            }, b => b.id == orderDetailId);


            if (tm.Trim() == orderdetail.AnteCodes.Trim()) {
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId == userId.ToString());
            }

             


        }
        public override string BuildCodes(string content)
        {
            return content;
        }
        }
}
