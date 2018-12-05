using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.Common.Hk6;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖
    /// </summary>
   public class ZongXiaoOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public ZongXiaoOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
            string tm = winNum.Split('|')[1];
            string zm = winNum.Split('|')[0];
            string AnteCodes = orderdetail.AnteCodes.Trim();

            var clist = zm.Split(',');
            List<string> CoreList = clist.ToList();
            CoreList.Add(tm);

            string userId = orderdetail.userId;
            int winCount = 0;
            List<string> winCodeList = new List<string>();
           
            List<XiaoModel> xlist = SXHelper.XiaoCollection12();
            List<XiaoModel> countList = new List<XiaoModel>();
           
            foreach (var code in CoreList)
            {

                var p = (from b in xlist
                         where b.CodeList.Contains(code)
                         select b).FirstOrDefault();
                countList.Add(p);
            }
            var count = 0;
            bool isWin = false;
            count = countList.DistinctBy(p => p.Index).Count();
            switch (AnteCodes)
            {
                case "234X":
                   
                    if (count>=2&count<=4)
                    {
                        isWin = true;
                    }
                    break;
                case "5X":
                  
                    if (count ==5)
                    {
                        isWin = true;
                    }
                    break;
                case "6X":

                    if (count == 6)
                    {
                        isWin = true;
                    }
                    break;
                case "7X":

                    if (count == 6)
                    {
                        isWin = true;
                    }
                    break;
                case "DX":

                    if (count %2 !=0)
                    {
                        isWin = true;
                    }
                    break;
                case "SX":

                    if (count % 2 == 0)
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

            decimal winMoney = (orderdetail.unitPrice * (Odds-1)* 1+ orderdetail.unitPrice) * orderdetail.BeiSu;

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
                }, b => b.userId== userId.ToString());
            }
              

        }
    }
}
