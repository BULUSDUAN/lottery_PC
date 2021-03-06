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
   public class GYHZOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;

        public int playId { get; private set; } = 64;
        //DingWeiLiuOrderHelper
        public GYHZOrderHelper(IDbProvider _DB) 
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
            string one = winarr[0];
            string two = winarr[1];
            int sum = int.Parse(one) + int.Parse(two);

            string[] antuArr = antuCode.Split(',');
            int index = 0;
            foreach (var item in antuArr)
            {
                if (int.Parse(item)==sum)
                {
                    isWin = true;

                    break;
                }
                index++;
            }


            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

          //  int BonusStatus = 3;

            if (isWin)
            {
               

                decimal Odds = decimal.Parse(orderdetail.OddsArr.Split(',')[index]);
                 winMoney = decimal.Parse(orderdetail.unitPrices) * Odds * orderdetail.BeiSu ;
               // BonusStatus = 2;
            }

            BaseOrderModel bmodel = new BaseOrderModel()
            {
                isWin = isWin,
                winMoney = winMoney,
                orderDetailId = orderDetailId,
                userId = userId
            };
            base.buildOrder(this.DB, bmodel);




        }
        public override string BuildCodes(string content)
        {
            return content;
        }
        public override bool CheckCode(string content, List<blast_antecode> listCode,int _playId= 64) {
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
