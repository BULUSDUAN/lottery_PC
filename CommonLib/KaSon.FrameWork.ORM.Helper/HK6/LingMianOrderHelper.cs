﻿using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖 liangmian
    /// </summary>
   public class LingMianOrderHelper : BaseOrderHelper
    {
        private IDbProvider DB = null;
       
        public LingMianOrderHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }
        public override void WinMoney(blast_bet_orderdetail orderdetail, string winNum) {
            string tm = winNum.Split('|')[1];
            string zm = winNum.Split('|')[0];
            string antuCode = orderdetail.AnteCodes;
            bool isWin = false;
            int sum = 0;
            char[] arr = null;
            List<XiaoModel> xiaoList = new List<XiaoModel>();
            XiaoCollectionModel xm = SXHelper.XiaoCollection();
            var t = zm.Split(',');
            sum = 0;
            foreach (var item in t)
            {
                sum += int.Parse(item);
            }
            sum += int.Parse(tm);
            switch (antuCode)
            {
                case "TD"://特单
                    if (int.Parse(tm) % 2 != 0 && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
                case "TS"://特双
                    if (int.Parse(tm) % 2 == 0 && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
                case "TDa"://特大
                    if (int.Parse(tm)>(49/2) && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
                case "TXiao"://特小
                    if (int.Parse(tm) < (49 / 2) && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
                case "THDan"://特合单
                    arr = tm.ToCharArray();
                    sum = 0;
                    foreach (var item in arr)
                    {
                        sum += int.Parse(item + "");
                    }
                    if (sum %2!=0 )
                    {
                        isWin = true;
                    }
                    break;
                case "THShuang"://特合双
                    arr = tm.ToCharArray();
                    sum = 0;
                    foreach (var item in arr)
                    {
                        sum += int.Parse(item + "");
                    }
                    if (sum % 2 == 0 )
                    {
                        isWin = true;
                    }
                    break;
                case "THDa"://特合大
                    arr =  tm.ToCharArray();
                     sum = 0;
                    foreach (var item in arr)
                    {
                        sum += int.Parse(item + "");
                    }
                    if (sum>6 && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
                case "THX"://特合小
                    arr = tm.ToCharArray();
                    sum = 0;
                    foreach (var item in arr)
                    {
                        sum += int.Parse(item + "");
                    }
                    if (sum <7 && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
                case "TWDa"://尾大
                     arr = tm.ToCharArray();
                    sum = 0;
                      sum = int.Parse(arr[1] + "");
                    if (sum >4 && int.Parse(tm) !=49)
                    {
                        isWin = true;
                    }
                    break;
                case "TWXiao"://尾小
                    arr = tm.ToCharArray();
                    sum = 0;
                    sum = int.Parse(arr[1] + "");
                    if (sum <5 && int.Parse(tm) != 49)
                    {
                        isWin = true;
                    }
                    break;
              
                case "TDXiao"://特地肖
                              //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）


                    xiaoList = (from b in xm.DiXiao
                            where b.CodeList.Contains(tm)
                            select b).ToList();
                    if (xiaoList.Count>0)
                    {
                        isWin = true;
                    }

                    break;
                case "TTXiao"://特地肖
                              //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）


                    xiaoList = (from b in xm.TianXiao
                            where b.CodeList.Contains(tm)
                            select b).ToList();
                    if (xiaoList.Count > 0)
                    {
                        isWin = true;
                    }

                    break;
                case "TQXiao"://特前肖
                              //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）


                    xiaoList = (from b in xm.QianXiao
                                where b.CodeList.Contains(tm)
                                select b).ToList();
                    if (xiaoList.Count > 0)
                    {
                        isWin = true;
                    }

                    break;
                case "THXiao"://特后肖
                              //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）


                    xiaoList = (from b in xm.HouXiao
                                where b.CodeList.Contains(tm)
                                select b).ToList();
                    if (xiaoList.Count > 0)
                    {
                        isWin = true;
                    }

                    break;
                case "TJiaXiao"://特地肖
                              //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）


                    xiaoList = (from b in xm.JiaShou
                                where b.CodeList.Contains(tm)
                                select b).ToList();
                    if (xiaoList.Count > 0)
                    {
                        isWin = true;
                    }

                    break;
                case "TYXiao"://特野肖
                              //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）


                    xiaoList = (from b in xm.YeShou
                                where b.CodeList.Contains(tm)
                                select b).ToList();
                    if (xiaoList.Count > 0)
                    {
                        isWin = true;
                    }

                    break;
                case "ZongDa"://总单
                             //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）

                    if (sum>174)
                    {
                        isWin = true;
                    }

                    break;
                case "ZongXiao"://总单
                             //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）

                    if (sum < 175)
                    {
                        isWin = true;
                    }

                    break;
                case "ZongDan"://总单
                                //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）

                    if (sum %2 !=0)
                    {
                        isWin = true;
                    }

                    break;
                case "ZongShuan"://总单
                               //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）

                    if (sum % 2 == 0)
                    {
                        isWin = true;
                    }

                    break;
                default:
                    break;
            }

            string userId = orderdetail.userId;
           
            decimal winMoney =0;
            int orderDetailId = orderdetail.id;

            int BonusStatus = 3;

            if (isWin)
            {
                decimal Odds = decimal.Parse(orderdetail.OddsArr);
                 winMoney = orderdetail.unitPrice * Odds * orderdetail.BeiSu;
                BonusStatus = 2;
            }
           
            DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
            {
                winNumber = winNum,
                BonusAwardsMoney = winMoney,
                updateTime = DateTime.Now,
                BonusStatus = BonusStatus  //为中奖状态


            }, b => b.id == orderDetailId);


            if (tm.Trim() == orderdetail.AnteCodes.Trim()) {
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
