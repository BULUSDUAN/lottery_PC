using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
    /// <summary>
    /// kason
    /// </summary>
   public class SchemeManager : DBbase
    {
        public void AddOrderDetail(C_OrderDetail entity)
        {
            DB.GetDal<C_OrderDetail>().Add(entity);
        }

        public C_OrderDetail QueryOrderDetail(string schemeId)
        {
           // Session.Clear();
            return DB.CreateQuery<C_OrderDetail>().Where(o => o.SchemeId == schemeId).FirstOrDefault();
        }
        public void UpdateOrderDetail(C_OrderDetail entity)
        {
            DB.GetDal<C_OrderDetail>().Update(entity);
        }
    }
}
