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
   public class DanShuanOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public DanShuanOrderHelper(IDbProvider _DB) 
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
            string codes = winarr[0];
            switch (orderdetail.playId)
            {
                case 76:
                    codes = winarr[0];
                    break;
                case 77:
                    codes = winarr[1];
                    break;
                case 78:
                    codes = winarr[2];
                    break;
               
            }
            if (antuCode.Contains(","))
            {
                var antuArr = antuCode.Split(',');
                foreach (var item in antuArr)
                {
                    if (item=="03")
                    {
                        if (int.Parse(codes) %2 !=0)
                        {
                            isWin = true;
                        }
                    }
                    else if (item == "04")
                    {
                        if (int.Parse(codes) % 2 == 0)
                        {
                            isWin = true;
                        }
                    }
                }
            }


            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

           // int BonusStatus = 3;

            if (isWin)
            {
               

                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney = decimal.Parse(orderdetail.unitPrices) * Odds * orderdetail.BeiSu ;
                //BonusStatus = 2;
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
