using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 三色
    /// </summary>
   public class SBSanSeOrderHelper : IOrderHelper
    {
        private IDbProvider DB = null;
       
        public SBSanSeOrderHelper(IDbProvider _DB) {
            DB = _DB;
        }
        public void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
            string tm = winNum.Split('|')[1];
            string zm = winNum.Split('|')[0];
            var codeArr = orderdetail.AnteCodes.Trim().Split(',');
            int userId = orderdetail.userId;
            int winCount = 0;
            List<string> winCodeList = new List<string>();

            string antuCode = orderdetail.AnteCodes;
           bool isWin = false;
            switch (antuCode)
            {
                case "Red":
                    if (SXHelper.GreenBox.Contains(tm))
                    {
                        isWin = true;
                    }

                    break;
                case "Blu":
                    if (SXHelper.BluBox.Contains(tm))
                    {
                        isWin = true;
                    }

                    break;
                case "Green":
                    if (SXHelper.GreenBox.Contains(tm))
                    {
                        isWin = true;
                    }

                    break;
                default:
                    break;
            }

            //故中奖
            //计算中奖号码
            decimal Odds = decimal.Parse(orderdetail.OddsArr);

            decimal winMoney = orderdetail.unitPrice * (Odds);

            int orderDetailId = orderdetail.id;

            string windesc = string.Join(",", winCodeList);
            if (isWin)
            {

                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    winNumber = winNum,
                    BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus = 2,  //为中奖状态
                    winNumberDesc= ""

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
            if (isWin)
            {
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId== userId);
            }
              

        }
    }
}
