using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 冠军亚军和值
    /// </summary>
   public class DingWeiLiuOrderHelper : BaseOrderHelper
    {
        private int playId = 62;
        private IDbProvider DB = null;
       
        public DingWeiLiuOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
          
            string antuCode =this.BuildCodes( orderdetail.AnteCodes);
            bool isWin = false;
           
            string[] winarr = winNum.Split(',');
            if (winarr.Length!=10)
            {
                //  string tempcode=  arr[0];
                return; 
            }

            string[] weiCodes = antuCode.Split('|');
            //  int sum = int.Parse(winarr[0]) + int.Parse(winarr[1]);

            // string[] weiCodes = antuCode.Split('|');
            int wincount = 0;
            int index = 0;
            foreach (var item in weiCodes)
            {
                string[] codeList = item.Split(',');

                string code = winarr[index];
                foreach (var item1 in codeList)
                {
                    if (item1.Trim() == code)
                    {
                        wincount++;
                        break;
                    }

                }
                index++;
            }




            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

           // int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr.Split(',')[index]);
                 winMoney = decimal.Parse(orderdetail.unitPrices) * Odds * orderdetail.BeiSu * 1;
              //  BonusStatus = 2;
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
            return content.Replace("1_", "").Replace("2_", "")
                .Replace("3_", "").Replace("4_", "").Replace("5_", "").Replace("6_", "")
                 .Replace("7_", "").Replace("8_", "").Replace("9_", "").Replace("10_", "");
        }

        public override bool CheckCode(string content, List<blast_antecode> listCode ,int _playId= 62)
        {
            bool result = true;
            if (!content.Contains("|"))
            {
                return false;
            }
            List<string> clistCode = new List<string>();
            string[] arr = content.Split('|');
            foreach (var item in arr)
            {
                clistCode.Add(item);
                if (item.Contains(","))
                {
                    clistCode = item.Split(',').ToList();
                   
                }
                result = base.CheckCode(clistCode, listCode, _playId);
                clistCode.Clear();
                if (!result)
                {
                    return result;
                }
            }
           
            return result;
        }
    }
}
