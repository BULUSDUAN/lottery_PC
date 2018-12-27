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
   public abstract class BaseOrderHelper
    {
       
        public abstract void WinMoney(blast_bet_orderdetail orderdetail, string winNum);

      //  public abstract string BuildCodes(string content);

        public static BaseOrderHelper GetOrderHelper(blast_bet_orderdetail orderdetail, IDbProvider DB)
        {
            BaseOrderHelper iorder = null;
            switch (orderdetail.playId)
            {
                case 1:
                    iorder = new TeMaOrderHelper(DB);
                    break;
                case 3:
                    iorder = new ZhengMaOrderHelper(DB);
                    break;
                case 2:
                    iorder = new LingMianOrderHelper(DB);
                    break;
                case 39:
                    iorder = new ZhengXiaoOrderHelper(DB);
                    break;
                case 40:
                    iorder = new TeXiaoOrderHelper(DB);
                    break;
                case 41:
                    iorder = new YiXiaoOrderHelper(DB);
                    break;
                case 42:
                    iorder = new ZongXiaoOrderHelper(DB);
                    break;
                case 44:
                    iorder = new SBSanSeOrderHelper(DB);
                    break;
                case 45:
                    iorder = new SBBanOrderHelper(DB);
                    break;
                case 46:
                    iorder = new SBBanBanOrderHelper(DB);
                    break;
                case 47:
                    iorder = new SBQiSeOrderHelper(DB);
                    break;
                default:
                    break;
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

        public bool CheckCode( List<string> colist, List<blast_antecode> listCode, int playid) {

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
    }
}
