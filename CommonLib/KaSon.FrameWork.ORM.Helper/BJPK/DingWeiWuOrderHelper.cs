using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 定位
    /// </summary>
   public class DingWeiWuOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public DingWeiWuOrderHelper(IDbProvider _DB) 
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

            string[] weiCodes = antuCode.Split('|');
            int wincount = 0;
            int index = 0;
            foreach (var item in weiCodes)
            {
                string[] codeList = item.Split(',');

                string code = winarr[index];
                foreach (var item1 in codeList)
                {
                    if (item1.Trim()==code)
                    {
                        wincount++;
                    }
                    
                }
                index++;
            }




            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney = orderdetail.unitPrice * Odds * orderdetail.BeiSu * wincount;
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
