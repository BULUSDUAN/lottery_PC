using System;
using System.Linq;
using System.Transactions;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using Common.Business;
using System.Configuration;
using GameBiz.Auth.Business;
using System.Collections.Generic;
using Common.Utilities;
using Common.Net;
using System.Data;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Query : WcfService
    {
        public DataTable GetSaleSummaryDataTable(string userIdList, string dateType, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return new SqlQueryBusiness().GetSaleSummaryDataTable(userIdList, dateType, dateFrom, dateTo);
            }
            catch (Exception ex)
            {
                throw new Exception("查询营销概况出错 - " + ex.Message, ex);
            }
        }
    }
}
