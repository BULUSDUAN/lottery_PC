using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 用户积分
    /// </summary>
    public class UserIntegralBalance
    {
        public virtual string UserId { get; set; }
        /// <summary>
        /// 当前积分
        /// </summary>
        public virtual int CurrIntegralBalance { get; set; }
        /// <summary>
        /// 已使用积分
        /// </summary>
        public virtual int UseIntegralBalance { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
