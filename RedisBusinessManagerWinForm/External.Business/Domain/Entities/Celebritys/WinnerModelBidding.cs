using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.Celebritys
{
   public class WinnerModelBidding
    {
       /// <summary>
       /// 模型编号
       /// </summary>
       public virtual string ModelId { get; set; }
       /// <summary>
       /// 用户编号
       /// </summary>
       public virtual string UserId { get; set; }
       /// <summary>
       /// 被点击次数
       /// </summary>
       public virtual int ClickNumber { get; set; }
       /// <summary>
       /// 已调整出价
       /// </summary>
       public virtual decimal BidDouDou { get; set; }
       /// <summary>
       /// 当前出价
       /// </summary>
       public virtual decimal CurrBidDouDou { get; set; }
       /// <summary>
       /// 创建时间
       /// </summary>
       public virtual DateTime CreateTime { get; set; }
       /// <summary>
       /// 调整竞价时间
       /// </summary>
       public virtual DateTime ModifyTime { get; set; }
       /// <summary>
       /// 是否系统推荐
       /// </summary>
       public virtual bool IsSystemRecom { get; set; }
    }
}
