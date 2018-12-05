using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖 teshu
    /// </summary>
   //public   class BaseOrderHelper
   // {
   //     private IDbProvider DB = null;

   //     public BaseOrderHelper(IDbProvider _DB)
   //     {
   //         DB = _DB;
   //     }
   //     public  IOrderHelper GetOrderHelper(blast_bet_orderdetail orderdetail) {
   //         IOrderHelper iorder = null;
   //         switch (orderdetail.playId)
   //         {
   //             case 1:
   //                 iorder = new TeMaOrderHelper(DB);
   //                 break;
   //             case 3:
   //                 iorder = new ZhengMaOrderHelper(DB);
   //                 break;
   //             case 2:
   //                 iorder = new LingMianOrderHelper(DB);
   //                 break;
   //             case 39:
   //                 iorder = new ZhengXiaoOrderHelper(DB);
   //                 break;
   //             case 40:
   //                 iorder = new TeXiaoOrderHelper(DB);
   //                 break;
   //             case 41:
   //                 iorder = new YiXiaoOrderHelper(DB);
   //                 break;
   //             case 42:
   //                 iorder = new ZongOrderHelper(DB);
   //                 break;
   //             case 44:
   //                 iorder = new SBSanSeOrderHelper(DB);
   //                 break;
   //             case 45:
   //                 iorder = new SBBanOrderHelper(DB);
   //                 break;
   //             case 46:
   //                 iorder = new SBBanBanOrderHelper(DB);
   //                 break;
   //             case 47:
   //                 iorder = new SBQiSeOrderHelper(DB);
   //                 break;
   //             default:
   //                 break;
   //         }

   //         return iorder;
   //     }
   //     public virtual string BuildCodes(string content)
   //     {

   //         string anteCodes = "";
   //         if (content.Contains(","))
   //         {
   //             string[] arr = content.Split(',');
   //             List<string> list = new List<string>();
   //             foreach (var item in arr)
   //             {
   //                 string temp = string.Join(",", SXHelper.ScodeArr(int.Parse(item)));
   //                 list.Add(temp);
   //             }

   //             anteCodes = string.Join("|", list);
   //         }
   //         else
   //         {
   //             anteCodes = string.Join(",", SXHelper.ScodeArr(int.Parse(content)));
   //         }
   //         return anteCodes;


   //     }
   // }
}
