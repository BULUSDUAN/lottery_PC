using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 定位 6-10
    /// </summary>
   public class LongHuOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public LongHuOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
          
            string antuCode = orderdetail.AnteCodes;
            bool isWin = false;
           
            string[] winarr = winNum.Split(',');
            if (winarr.Length!=10)
            {
                //  string tempcode=  arr[0];
                return; 
            }
            //冠军
            string dcodes = winarr[0];
            switch (orderdetail.playId)
            {
                case 65:
                    dcodes = winarr[0];
                    break;
                case 66:
                    dcodes = winarr[1];
                    break;
                case 67:
                    dcodes = winarr[2];
                    break;
                case 68:
                    dcodes = winarr[3];
                    break;
                case 69:
                    dcodes = winarr[4];
                    break;
            }

            if (int.Parse(dcodes) > int.Parse(winarr[9]) 
                && antuCode.Contains("01"))
            {

                isWin = true;
            }
            if (int.Parse(winarr[0]) < int.Parse(winarr[9])
               && antuCode.Contains("02"))
            {

                isWin = true;
            }



            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney = orderdetail.unitPrice * Odds * orderdetail.BeiSu * 0;
                BonusStatus = 2;
            }
           
            DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
            {
                winNumber = winNum,
                BonusAwardsMoney = winMoney,
                updateTime = DateTime.Now,
                BonusStatus = BonusStatus  //为中奖状态


            }, b => b.id == orderDetailId);


            if (isWin) {
                DB.GetDal<blast_member>().Update(b => new blast_member
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
