﻿using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 大小
    /// </summary>
   public class GYHDanXiaoDSOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public GYHDanXiaoDSOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum)
        {

            string antuCode = orderdetail.AnteCodes;
            bool isWin = false;

            string[] winarr = winNum.Split(',');
            if (winarr.Length != 10)
            {
                //  string tempcode=  arr[0];
                return;
            }
            string one = winarr[0];
            string two = winarr[1];
            int sum = int.Parse(one) + int.Parse(two);
            string codes = winarr[0];
            IList<String> anlistCode = new List<string>();
            IList<String> unitPricesList = new List<string>();
            IList<String> OddsList = new List<string>();
            anlistCode.Add(antuCode);
            unitPricesList.Add(orderdetail.unitPrices);
            OddsList.Add(orderdetail.OddsArr);

            if (antuCode.Contains(","))
            {
                anlistCode = antuCode.Split(',').ToList();
            }

            if (orderdetail.unitPrices.Contains(","))
            {
                unitPricesList = orderdetail.unitPrices.Split(',');

            }
            if (orderdetail.OddsArr.Contains(","))
            {
                OddsList = orderdetail.OddsArr.Split(',');

            }

            decimal winMoney = 0;
            int index = 0;
            foreach (var item in anlistCode)
            {
                if (item == "01")
                {
                    if (sum >= 12 && sum <= 19)
                    {
                        isWin = true;
                        winMoney = winMoney +
                            decimal.Parse(unitPricesList[index]) * decimal.Parse(unitPricesList[index]);
                    }
                }
                else if (item == "02")
                {
                    if (sum >= 3 && sum <= 11)
                    {
                        isWin = true;
                        winMoney = winMoney +
                           decimal.Parse(unitPricesList[index]) * decimal.Parse(unitPricesList[index]);
                    }
                }
                else if (item == "03")
                {
                    if (sum % 2 != 0)
                    {
                        isWin = true;
                        winMoney = winMoney +
                           decimal.Parse(unitPricesList[index]) * decimal.Parse(unitPricesList[index]);
                    }
                }
                else if (item == "04")
                {
                    if (sum % 2 == 0)
                    {
                        isWin = true;
                        winMoney = winMoney +
                           decimal.Parse(unitPricesList[index]) * decimal.Parse(unitPricesList[index]);
                    }
                }
                index++;
            }
        
            string userId = orderdetail.userId;
           
           
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (isWin)
            {
               

              //  decimal Odds = decimal.Parse(orderdetail.OddsArr.Split(',')[index]);
              //   winMoney = orderdetail.unitPrice * Odds * orderdetail.BeiSu ;
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
            return content.Replace("01", "大").Replace("02", "小")
                .Replace("03", "单").Replace("04", "双");
        }
        public override bool CheckCode(string content, List<blast_antecode> listCode, int _playId = 73)
        {
            bool result = true;

            List<string> clistCode = new List<string>();


            clistCode.Add(content);
            if (content.Contains(","))
            {
                clistCode = content.Split(',').ToList();
            }

            result = base.CheckCode(clistCode, listCode, _playId);

            return result;
        }
    }
}
