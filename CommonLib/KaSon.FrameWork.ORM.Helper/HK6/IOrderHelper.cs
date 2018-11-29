using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖 teshu
    /// </summary>
   public interface IOrderHelper
    {

         void WinMoney(blast_bet_orderdetail orderdetail, string winNum);
    }
}
