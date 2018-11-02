using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
   public class OrderInfo
    {
        public BettingOrderInfoCollection BettingOrderInfo;
        public IEnumerable<IGrouping<DateTime, BettingOrderInfo>> BettingOrder;
    }

    public class OrderDetailInfo
    {
        public BettingOrderInfoCollection DetailResult;
        public BettingOrderInfo FirstItem;
        public BettingAnteCodeInfoCollection FirstAnteCode;
        public UserQueryInfo UserResult;
        public string UserKey;
        public Sports_SchemeQueryInfo OrderDetail;
        public Sports_AnteCodeQueryInfoCollection CodeList;
    }

 
}
