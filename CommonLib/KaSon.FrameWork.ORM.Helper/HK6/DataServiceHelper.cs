using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖
    /// </summary>
   public class DataServiceHelper 
    {
        private IDbProvider DB = null;
       
        public DataServiceHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }

        public blast_data_time GetissueNo() {
           // result.IsSuccess = true;
            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            if (DateTime.Now.Hour > 21)
            {
                date = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString());
            }
            var tquery = DB.CreateQuery<blast_data_time>();
            var mb = (from b in tquery
                      where b.actionTime >= date
                      orderby b.actionTime ascending
                      select b).FirstOrDefault();

            return mb;

        }
       
    }
}
