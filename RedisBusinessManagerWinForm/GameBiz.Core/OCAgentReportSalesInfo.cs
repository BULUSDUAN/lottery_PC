using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class OCAgentReportSalesInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Int64 Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 父级编号
        /// </summary>
        public string ParentUserId { get; set; }
        /// <summary>
        /// 父级编号路径
        /// </summary>
        public string ParentUserIdPath { get; set; }
        /// <summary>
        /// 累计总销量(TotalCurrentUserSales+TotalAgentSales+TotalUserSales)
        /// </summary>
        public decimal TotalSales { get; set; }
        /// <summary>
        /// 累计所有下级代理销量
        /// </summary>
        public decimal TotalAgentSales { get; set; }
        /// <summary>
        /// 累计所有下级用户销量
        /// </summary>
        public decimal TotalUserSales { get; set; }
        /// <summary>
        /// 当前代理累计销量
        /// </summary>
        public decimal TotalCurrentUserSales { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class OCAgentReportSales_Collection
    {
        public OCAgentReportSales_Collection()
        {
            OcAgentReportSalesList = new List<OCAgentReportSalesInfo>();
        }
        public int TotalCount { get; set; }
        public List<OCAgentReportSalesInfo> OcAgentReportSalesList { get; set; }
    }
}
