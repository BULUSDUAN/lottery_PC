using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 前一
    /// </summary>
    public class QianErOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;

        public QianErOrderHelper(IDbProvider _DB)
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum)
        {
            string[] arr = winNum.Split(',');
            if (arr.Length != 10)
            {
                return;
            }
            string antuCode = orderdetail.AnteCodes;
            string one = "";
            string two = "";
            string winone = winNum.Split(',')[0];
            string wintwo = winNum.Split(',')[2];
            bool isWin = false;
            int winCount = 0;
            List<string> onelist = new List<string>();
            List<string> twolist = new List<string>();
           
            if (antuCode.Contains("|"))
            {
                one = antuCode.Split('|')[0];
                two = antuCode.Split('|')[1];
            }
            else {
                one = antuCode.Split(',')[0];
                two = antuCode.Split(',')[1];
            }
            onelist.Add(one);
            if (one.Contains(","))
            {
                onelist = one.Split(',').ToList();
            }
            twolist.Add(two);
            if (two.Contains(","))
            {
                twolist = two.Split(',').ToList();
            }
            foreach (var item in onelist)
            {
                if (item.Trim() == winone.Trim())
                {
                    foreach (var item2 in twolist)
                    {
                        if (item2.Trim() == wintwo.Trim())
                        {
                            winCount++;
                            isWin = true;
                        }

                    }
                }
            }









            string userId = orderdetail.userId;

            decimal winMoney = 0;
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                winMoney = orderdetail.unitPrice * Odds * orderdetail.BeiSu * winCount;
                BonusStatus = 2;
            }

            DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
            {
                winNumber = winNum,
                BonusAwardsMoney = winMoney,
                updateTime = DateTime.Now,
                BonusStatus = BonusStatus  //为中奖状态


            }, b => b.id == orderDetailId);


            if (isWin)
            {
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
