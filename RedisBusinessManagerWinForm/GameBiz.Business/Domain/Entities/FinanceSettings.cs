using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
   public class FinanceSettings
    {
       /// <summary>
       /// 主键ID，自增长
       /// </summary>
       public virtual int FinanceId { get; set; }
       /// <summary>
       /// 财务人员
       /// </summary>
       public virtual string UserId { get; set; }
       /// <summary>
       /// 操作级别 101：初级；102：高级
       /// </summary>
       public virtual string OperateRank { get; set; }
       /// <summary>
       /// 操作类型 10：提现;20充值
       /// </summary>
       public virtual string OperateType { get; set; }
       /// <summary>
       /// 最小金额
       /// </summary>
       public virtual decimal MinMoney { get; set; }
       /// <summary>
       /// 最大金额
       /// </summary>
       public virtual decimal MaxMoney { get; set; }
       /// <summary>
       /// 创建人员Id
       /// </summary>
       public virtual string OperatorId { get; set; }
       /// <summary>
       /// 创建时间
       /// </summary>
       public virtual DateTime CreateTime { get; set; }
    }
}
