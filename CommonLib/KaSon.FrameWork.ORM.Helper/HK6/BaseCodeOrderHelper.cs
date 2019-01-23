using EntityModel;
using KaSon.FrameWork.Common.Hk6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
  
    /// <summary>
    /// 生肖正肖 teshu
    /// </summary>
   public abstract class BaseCodeOrderHelper
    {
       
     

      //  public abstract string BuildCodes(string content);

        public static BaseCodeOrderHelper GetOrderHelper(int playId)
        {
            BaseCodeOrderHelper iorder = null;
            switch (playId)
            {
                //    #region hk6 
                case 1:
                case 3:
                case 2:
                    iorder = new TeMaOrderCodeHelper();
                    break;


                case 39:
                case 40:
                case 41:
              
                    iorder = new XiaoOrderCodeHelper();
                    //  iorder = new ZhengXiaoOrderHelper(DB);
                    break;
                case 42:
                case 44:
                case 45:
                case 46:
                case 47:
                    iorder = new TeMaOrderCodeHelper();
                    break;
                    //    case 40:
                    //        iorder = new TeXiaoOrderHelper(DB);
                    //        break;
                    //    case 41:
                    //        iorder = new YiXiaoOrderHelper(DB);
                    //        break;
                    //    case 42:
                    //        iorder = new ZongXiaoOrderHelper(DB);
                    //        break;
                    //    case 44:
                    //        iorder = new SBSanSeOrderHelper(DB);
                    //        break;
                    //    case 45:
                    //        iorder = new SBBanOrderHelper(DB);
                    //        break;
                    //    case 46:
                    //        iorder = new SBBanBanOrderHelper(DB);
                    //        break;
                    //    case 47:
                    //        iorder = new SBQiSeOrderHelper(DB);
                    //        break;
                    //    #endregion
                    //    #region pk10
                    //    case 58:
                    //        iorder = new QianYiOrderHelper(DB);
                    //        break;
                    //    case 59:
                    //        iorder = new QianErOrderHelper(DB);
                    //        break;
                    //    case 60:
                    //        iorder = new QianSanOrderHelper(DB);
                    //        break;
                    //    case 61:
                    //        iorder = new DingWeiWuOrderHelper(DB);
                    //        break;
                    //    case 62:
                    //        iorder = new DingWeiLiuOrderHelper(DB);
                    //        break;
                    //    case 63:
                    //        iorder = new DingWeiOrderHelper(DB);
                    //        break;
                    //    case 64:
                    //        iorder = new GYHZOrderHelper(DB);
                    //        break;
                    //    case 65:
                    //    case 66:
                    //    case 67:
                    //    case 68:
                    //    case 69:
                    //        iorder = new LongHuOrderHelper(DB);
                    //        break;
                    //    case 70:
                    //    case 71:
                    //    case 72:

                    //        iorder = new WuXingOrderHelper(DB);
                    //        break;
                    //    case 73:
                    //    case 74:
                    //    case 75:

                    //        iorder = new DanXiaoOrderHelper(DB);
                    //        break;
                    //    case 76:
                    //    case 77:
                    //    case 78:

                    //        iorder = new DanShuanOrderHelper(DB);
                    //        break;
                    //    case 79:

                    //        iorder = new GYHDanXiaoDSOrderHelper(DB);
                    //        break;
                    //    #endregion
                    //    default:

                    //        break;
            }

            return iorder;
        }
        public virtual string BuildCodes(string content)
        {

            string anteCodes = "";
            if (content.Contains(","))
            {
                string[] arr = content.Split(',');
                List<string> list = new List<string>();
                foreach (var item in arr)
                {
                    string temp = string.Join(",", SXHelper.ScodeArr(int.Parse(item)));
                    list.Add(temp);
                }

                anteCodes = string.Join("|", list);
            }
            else
            {
                anteCodes = string.Join(",", SXHelper.ScodeArr(int.Parse(content)));
            }
            return anteCodes;


        }
        public virtual bool CheckCode(string content, List<blast_antecode> listCode, int _playId = 73) {

            return false;
        }
        public virtual string GetOddsArr(string content, List<blast_antecode> listCode,  int _playId = 73)
        {
            string OddsArr = "";
            switch (_playId)
            {
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                    var p = listCode.Where(b => b.playid == _playId).FirstOrDefault();
                    OddsArr = p.odds+"";
                    break;
                case 64:
                case 79:
                    OddsArr = "";
                    if (!content.Contains(","))
                    {
                        var p1 = listCode.Where(b => b.AnteCode == content && b.playid == _playId).FirstOrDefault();
                        OddsArr = p1.odds + "";
                    }
                    else {
                        var p2 = content.Split(',');
                        IList<string> list = new List<string>();
                        foreach (var item in p2)
                        {
                            var p3 = listCode.Where(b => b.AnteCode == item && b.playid == _playId).FirstOrDefault();

                           // OddsArr = OddsArr + p3.odds+",";

                            list.Add(p3.odds+"");
                        }
                        OddsArr = string.Join(",", list);
                    }
                  

                    break;
              

                default:
                    break;
            }
           
            return OddsArr;
        }

        public  bool CheckCode( List<string> colist, List<blast_antecode> listCode, int playid=0) {

            var bol = true;
            foreach (var item in colist)
            {
              var p=  listCode.Where(b => b.AnteCode == item.Trim() && b.playid == playid).FirstOrDefault();
                if (p==null)
                {
                    bol = false;
                    break;
                }
            }
            return bol;

        }

        //public void buildOrder(IDbProvider DB, BaseOrderModel bmodel) {

        //    if (bmodel.isWin)
        //    {
        //        bmodel.BonusStatus = 2;
        //    }
        //    else
        //    {
        //        bmodel.BonusStatus = 3;
        //        bmodel.winMoney = 0;
        //    }
        //    DB.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail
        //    {
        //        winNumber = bmodel.winNum,
        //        BonusAwardsMoney = bmodel.winMoney,
        //        updateTime = DateTime.Now,
        //        BonusStatus = bmodel.BonusStatus  //为中奖状态


        //    }, b => b.id == bmodel.orderDetailId);


        //    if (bmodel.isWin)
        //    {

        //        DB.GetDal<blast_member>().Update(b => new blast_member
        //        {
        //            gameMoney = b.gameMoney + bmodel.winMoney
        //        }, b => b.userId == bmodel.userId.ToString());

        //    }
        //}
    }
}
