using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class LotteryScheme
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 关联值
        /// </summary>
        public virtual string KeyLine { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public virtual int OrderIndex { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public virtual bool IsComplate { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
