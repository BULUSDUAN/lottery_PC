using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.Celebritys
{
   public class WinnerModelCollection
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int ModelCollectionId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        public virtual string ModelId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
    }
}
