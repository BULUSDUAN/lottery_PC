using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class FinanceSettingsInfo
    {
        /// <summary>
        /// 主键ID，自增长
        /// </summary>
        public int FinanceId { get; set; }
        /// <summary>
        /// 财务人员
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 财务人员名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 操作级别 101：初级；102：高级
        /// </summary>
        public string OperateRank { get; set; }
        /// <summary>
        /// 操作类型 10：提现;20充值
        /// </summary>
        public string OperateType { get; set; }
        /// <summary>
        /// 可操作金额
        /// </summary>
        public string FinanceMoney { get; set; }
        /// <summary>
        /// 最小金额
        /// </summary>
        public decimal MinMoney { get; set; }
        /// <summary>
        /// 最大金额
        /// </summary>
        public decimal MaxMoney { get; set; }
        /// <summary>
        /// 创建人员Id
        /// </summary>
        public string OperatorId { get; set; }
        /// <summary>
        /// 创建人员名称
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class FinanceSettingsInfo_Collection
    {
        public FinanceSettingsInfo_Collection()
        {
            FinanceSettingsList = new List<FinanceSettingsInfo>();
        }
        public int TotalCount { get; set; }
        public List<FinanceSettingsInfo> FinanceSettingsList { get; set; }
    }
}
