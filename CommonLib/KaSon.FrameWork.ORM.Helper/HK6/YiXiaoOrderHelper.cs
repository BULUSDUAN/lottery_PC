using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖
    /// </summary>
   public class YiXiaoOrderHelper : IOrderHelper
    {
        private IDbProvider DB = null;
       
        public YiXiaoOrderHelper(IDbProvider _DB) {
            DB = _DB;
        }
        public void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
            string tm = winNum.Split('|')[1];
            string zm = winNum.Split('|')[0];
            var codeArr = orderdetail.AnteCodes.Trim().Split(',');
            int userId = orderdetail.userId;
            int winCount = 0;
            List<string> winCodeList = new List<string>();
            foreach (var code in codeArr)
            {
                if (winNum.Contains(code))
                {
                    //中奖一次
                    winCount++;
                    winCodeList.Add(code);
                }
            }

            //故中奖
            //计算中奖号码
            decimal Odds = decimal.Parse(orderdetail.OddsArr);
            winCount = 1;
            decimal winMoney = orderdetail.unitPrice * (Odds-1)* winCount+ orderdetail.unitPrice;

            int orderDetailId = orderdetail.id;

            string windesc = string.Join(",", winCodeList);
            if (winCount > 0)
            {

                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    winNumber = winNum,
                    BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus = 2,  //为中奖状态
                    winNumberDesc= windesc

                }, b => b.id == orderDetailId);

            }
            else {

                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    //winNumber = winNum,
                    //BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus =3, //为中奖状态
                    winNumber= winNum,
                    winNumberDesc = windesc
                }, b => b.id == orderDetailId);
            }



            //添加用户金币 加钱  blast_lhc_member
            if (winCount > 0)
            {
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId== userId);
            }
              

        }
    }
}
