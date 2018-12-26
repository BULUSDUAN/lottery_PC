﻿using EntityModel;
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
   public class WuXingOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public WuXingOrderHelper(IDbProvider _DB) 
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
                case 70:
                    dcodes = winarr[0];
                    break;
                case 71:
                    dcodes = winarr[1];
                    break;
                case 72:
                    dcodes = winarr[2];
                    break;
               
            }
            List<string> codeList = new List<string>();
            if (antuCode.Contains(","))
            {
                var antucodeArr = antuCode.Split(',');
                foreach (var item in antucodeArr)
                {
                    switch (item)
                    {
                        case "01":
                            codeList.Add("01");
                            codeList.Add("02");
                            break;
                        case "02":
                            codeList.Add("03");
                            codeList.Add("04");
                            break;
                        case "03":
                            codeList.Add("05");
                            codeList.Add("06");
                            break;
                        case "04":
                            codeList.Add("07");
                            codeList.Add("08");
                            break;
                        case "05":
                            codeList.Add("09");
                            codeList.Add("10");
                            break;
                        default:
                            break;
                    }
                }
            }
            else {
                switch (antuCode)
                {
                    case "01":
                        codeList.Add("01");
                        codeList.Add("02");
                        break;
                    case "02":
                        codeList.Add("03");
                        codeList.Add("04");
                        break;
                    case "03":
                        codeList.Add("05");
                        codeList.Add("06");
                        break;
                    case "04":
                        codeList.Add("07");
                        codeList.Add("08");
                        break;
                    case "05":
                        codeList.Add("09");
                        codeList.Add("10");
                        break;
                    default:
                        break;
                }
            }

            if (codeList.Contains(dcodes)) 
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
