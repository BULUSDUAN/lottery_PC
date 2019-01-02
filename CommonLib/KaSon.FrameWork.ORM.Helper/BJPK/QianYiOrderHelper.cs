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
   public class QianYiOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public QianYiOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
          
            string antuCode =this.BuildCodes( orderdetail.AnteCodes);
            bool isWin = false;
           
            string[] arr = winNum.Split(',');
            if (arr.Length==10)
            {
              string tempcode=  arr[0];
                 
               
                if (antuCode.Contains( tempcode.Trim()))
                {
                    isWin = true;
                }
            }
             
          
   
         

            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

          //  int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney =decimal.Parse(orderdetail.unitPrices) * Odds * orderdetail.BeiSu;
             //   BonusStatus = 2;
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
            return content.Replace("1_", "");//.Replace("2_", "").Replace("3_", "");
        }
        public override bool CheckCode(string content,List<blast_antecode> listCode, int playId = 58) {
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
