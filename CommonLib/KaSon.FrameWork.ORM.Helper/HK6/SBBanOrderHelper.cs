using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 半色
    /// </summary>
   public class SBBanOrderHelper : IOrderHelper
    {
        private IDbProvider DB = null;
       
        public SBBanOrderHelper(IDbProvider _DB) {
            DB = _DB;
        }
        public void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
            string tm = winNum.Split('|')[1];
            string zm = winNum.Split('|')[0];
            var codeArr = orderdetail.AnteCodes.Trim().Split(',');
            string userId = orderdetail.userId;
            int winCount = 0;
            List<string> winCodeList = new List<string>();

            string antuCode = orderdetail.AnteCodes;
           bool isWin = false;
            switch (antuCode)
            {
                #region 红
                case "RedDan":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm)%2!=0)
                    {
                        isWin = true;
                    }

                    break;
                case "RedSua":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }
                    break;
                case "RedXiao":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm) <=24)
                    {
                        isWin = true;
                    }

                    break;
                case "RedDa":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm) > 24)
                    {
                        isWin = true;
                    }

                    break;
                #endregion
                #region 蓝
                case "BluDan":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "BluSua":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }
                    break;
                case "BluXiao":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) <= 24)
                    {
                        isWin = true;
                    }

                    break;
                case "BluDa":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) > 24)
                    {
                        isWin = true;
                    }

                    break;
                #endregion
                #region 绿
                case "GreenDan":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "GreenSua":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }
                    break;
                case "GreenXiao":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) <= 24)
                    {
                        isWin = true;
                    }

                    break;
                case "GreenDa":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) > 24)
                    {
                        isWin = true;
                    }

                    break;
                #endregion
                default:

                    break;
            }
            if (int.Parse(tm)==49)
            {
             //   isWin = false;
                //是否和局
            }
            //故中奖
            //计算中奖号码
            decimal Odds = decimal.Parse(orderdetail.OddsArr);

            decimal winMoney = orderdetail.unitPrice * (Odds) * orderdetail.BeiSu;

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
                    winNumberDesc= ""

                }, b => b.id == orderDetailId);

            }
            if (int.Parse(tm) == 49)
            {
                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    //winNumber = winNum,
                    //BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus = 4, //和局
                    winNumber = winNum,
                    winNumberDesc = windesc
                }, b => b.id == orderDetailId);

            }
            else {
                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    //winNumber = winNum,
                    //BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus = 3, //为中奖状态
                    winNumber = winNum,
                    winNumberDesc = windesc
                }, b => b.id == orderDetailId);
            }



            //添加用户金币 加钱  blast_lhc_member
            if (int.Parse(tm) == 49)//和局
            {
                winMoney = orderdetail.unitPrice *  orderdetail.BeiSu;
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId == userId.ToString());
            }
            else if (isWin  )
            {
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId== userId.ToString());
            }
              

        }
    }
}
