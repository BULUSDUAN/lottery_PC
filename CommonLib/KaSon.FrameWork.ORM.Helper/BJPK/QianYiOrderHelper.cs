﻿using EntityModel;
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
   public class QianYiOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public QianYiOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
          
            string antuCode = orderdetail.AnteCodes;
            bool isWin = false;
           
            string[] arr = winNum.Split(',');
            if (arr.Length==10)
            {
              string tempcode=  arr[0];
                 
               
                if (orderdetail.AnteCodes.Contains( tempcode.Trim()))
                {
                    isWin = true;
                }
            }
             
          
   
         

            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney =decimal.Parse(orderdetail.unitPrices) * Odds * orderdetail.BeiSu;
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
            return content.Replace("1_", "");//.Replace("2_", "").Replace("3_", "");
        }
        public bool CheckCode(string content,List<blast_antecode> listCode, int playId = 58) {
            bool result = true;
            List<string> list = new List<string>();
            list.Add(content);
            if (content.Contains(","))
            {
                list = content.Split(',').ToList();
                
            }
            result = base.CheckCode(list, listCode, playId);

            return result;
        }
    }
}
