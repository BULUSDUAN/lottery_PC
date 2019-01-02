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
        private int playId = 59;
        public QianErOrderHelper(IDbProvider _DB)
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum)
        {
            string[] arr = winNum.Split(',');
            string antuCode = this.BuildCodes(orderdetail.AnteCodes);

            if (arr.Length != 10)
            {
                return;
            }
            if (!antuCode.Contains("|"))
            {
                return ;
            }
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
            //else {
            //    one = antuCode.Split(',')[0];
            //    two = antuCode.Split(',')[1];
            //}
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
                         //   winCount++;
                            isWin = true;
                            break;
                        }

                    }
                }
            }









            string userId = orderdetail.userId;

            decimal winMoney = 0;
            int orderDetailId = orderdetail.id;

          //  int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                winMoney = decimal.Parse(orderdetail.unitPrices) * Odds * orderdetail.BeiSu * 1;
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
            return content.Replace("1_", "").Replace("2_", "");//.Replace("3_", "");
        }

        public override bool CheckCode(string content, List<blast_antecode> listCode,int _playId= 59)
        {
            bool result = true;
            if (!content.Contains("|"))
            {
                return false;
            }
            List<string> clistCode = new List<string>();
            string one = content.Split('|')[0];
            string two = content.Split('|')[1];

            clistCode.Add(one);
            if (one.Contains(","))
            {
                clistCode = one.Split(',').ToList();
               
            }
            result = base.CheckCode(clistCode, listCode, _playId);
            if (!result)
            {
                return result;
            }

            clistCode.Clear();

            clistCode.Add(two);
            if (two.Contains(","))
            {
                clistCode = two.Split(',').ToList();
            }
            result = base.CheckCode(clistCode, listCode, _playId);
            return result;
        }
    }
}
