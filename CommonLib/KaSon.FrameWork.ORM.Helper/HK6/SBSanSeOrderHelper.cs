﻿using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 三色
    /// </summary>
   public class SBSanSeOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public SBSanSeOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
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
                case "Red":
                    if (SXHelper.GreenBox.Contains(tm))
                    {
                        isWin = true;
                    }

                    break;
                case "Blu":
                    if (SXHelper.BluBox.Contains(tm))
                    {
                        isWin = true;
                    }

                    break;
                case "Green":
                    if (SXHelper.GreenBox.Contains(tm))
                    {
                        isWin = true;
                    }

                    break;
                default:
                    break;
            }

            //故中奖
            //计算中奖号码
            decimal Odds = decimal.Parse(orderdetail.OddsArr);

            decimal winMoney = orderdetail.unitPrice * (Odds) * orderdetail.BeiSu;

            int orderDetailId = orderdetail.id;

            string windesc = string.Join(",", winCodeList);
            if (int.Parse(tm) == 49)//和局
            {
                DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
                {
                    winNumber = winNum,
                    BonusAwardsMoney = winMoney,
                    updateTime = DateTime.Now,
                    BonusStatus = 4,  //为中奖状态
                    winNumberDesc = ""

                }, b => b.id == orderDetailId);
            }
            else
            {


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


            //添加用户金币 加钱  blast_member
            //添加用户金币 加钱  blast_member
            if (int.Parse(tm) == 49)//和局
            {
                winMoney = orderdetail.unitPrice * orderdetail.BeiSu ;
                DB.GetDal<blast_member>().Update(b => new blast_member
                {
                    gameMoney = b.gameMoney + winMoney
                }, b => b.userId == userId.ToString());
            }
            else if (isWin)
            {
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
