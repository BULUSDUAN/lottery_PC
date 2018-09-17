using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.ORM.Helper.WinNumber;

namespace Craw.Service.ModuleServices
{
  public  class CrawORMService
    {

        public bool Start( string gameName,ConcurrentDictionary<string, string> all, Dictionary<string, string> dic) {


            if (all == null) return false;

            var query = from q in dic where !all.ContainsKey(q.Key) select q;
            if (query.Count() > 0)
            {
                foreach (var item in query)//
                {



                    // 采集到数据

                    //导入数据
                    ILotteryDataBusiness instan = KaSon.FrameWork.ORM.Helper.WinNumber.LotteryDataBusiness.GetTypeImport(gameName);// = new LotteryDataBusiness();
                    instan.ImportWinNumber( item.Key, item.Value);

                    //奖期派奖



                }
                //生成相关静态数据
            }
            return false;
        }
    }
}
