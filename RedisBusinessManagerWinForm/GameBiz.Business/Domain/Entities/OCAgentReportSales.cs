using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
  public  class OCAgentReportSales
    {
      /// <summary>
      /// 主键Id
      /// </summary>
      public virtual Int64 Id { get; set; }
      /// <summary>
      /// 用户编号
      /// </summary>
      public virtual string UserId { get; set; }
      /// <summary>
      /// 父级编号
      /// </summary>
      public virtual string ParentUserId { get; set; }
      /// <summary>
      /// 父级编号路径
      /// </summary>
      public virtual string ParentUserIdPath { get; set; }
      /// <summary>
      /// 彩种
      /// </summary>
      public virtual string GameCode { get; set; }
      /// <summary>
      /// 累计总销量(TotalCurrentUserSales+TotalAgentSales+TotalUserSales)
      /// </summary>
      public virtual decimal TotalSales { get; set; }
      /// <summary>
      /// 累计所有下级代理销量
      /// </summary>
      public virtual decimal TotalAgentSales { get; set; }
      /// <summary>
      /// 累计所有下级用户销量
      /// </summary>
      public virtual decimal TotalUserSales { get; set; }
      /// <summary>
      /// 当前代理累计销量
      /// </summary>
      public virtual decimal TotalCurrentUserSales { get; set; }
      /// <summary>
      /// 创建时间
      /// </summary>
      public virtual DateTime CreateTime { get; set; }
    }
}
