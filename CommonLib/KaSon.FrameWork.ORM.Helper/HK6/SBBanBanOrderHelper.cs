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
   public class SBBanBanOrderHelper : IOrderHelper
    {
        private IDbProvider DB = null;
       
        public SBBanBanOrderHelper(IDbProvider _DB) {
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
                case "RedDaDan":
                    if (SXHelper.RedBox.Contains(tm)&& int.Parse(tm) > 24 && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "RedDaSua":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm) > 24 && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }
                    break;
                case "RedXiaoDan":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm) <=24 && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "RedXiaoSua":
                    if (SXHelper.RedBox.Contains(tm) && int.Parse(tm) <= 24 && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }

                    break;
                #endregion
                #region 蓝
                case "BluDaDan":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) > 24 && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "BluDaSua":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) > 24 && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }
                    break;
                case "BluXiaoDan":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) <= 24 && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "BluXiaoSua":
                    if (SXHelper.BluBox.Contains(tm) && int.Parse(tm) <= 24 && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }

                    break;
                #endregion
                #region 绿
                case "GreenDaDan":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) > 24 && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "GreenDaSua":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) > 24 && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }
                    break;
                case "GreenXiaoDan":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) <= 24 && int.Parse(tm) % 2 != 0)
                    {
                        isWin = true;
                    }

                    break;
                case "GreenXiaoSua":
                    if (SXHelper.GreenBox.Contains(tm) && int.Parse(tm) <= 24 && int.Parse(tm) % 2 == 0)
                    {
                        isWin = true;
                    }

                    break;
                #endregion
                default:

                    break;
            }
          
            //故中奖
            //计算中奖号码
            decimal Odds = decimal.Parse(orderdetail.OddsArr);

            decimal winMoney = orderdetail.unitPrice * (Odds) * orderdetail.BeiSu;

            int orderDetailId = orderdetail.id;

            string windesc = string.Join(",", winCodeList);
            if (int.Parse(tm) == 49)
            {

                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    //winNumber = winNum,
                    //BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus =4, //为中奖状态
                    winNumber = winNum,
                    winNumberDesc = windesc
                }, b => b.id == orderDetailId);
                //是否和局
            }
            else {
                if (isWin)
                {

                    DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                    {
                        winNumber = winNum,
                        BonusAwardsMoney = winMoney,
                        updateTime = DateTime.Now,
                        BonusStatus = 2,  //为中奖状态
                        winNumberDesc = ""

                    }, b => b.id == orderDetailId);

                }
                else
                {

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
            }




            //添加用户金币 加钱  blast_lhc_member
            if (int.Parse(tm) == 49)
            {
                winMoney = orderdetail.unitPrice * orderdetail.BeiSu;
                DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId == userId);
            }
            else {
                if (isWin)
                {
                    DB.GetDal<blast_lhc_member>().Update(b => new blast_lhc_member
                    {
                        gameMoney = b.gameMoney + winMoney
                    }, b => b.userId == userId);
                }
            }
              
              

        }
    }
}
