using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class UserSiteService
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public virtual ServiceType ServiceType { get; set; }
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public virtual string ExtendedOne { get; set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public virtual decimal ExtendedTwo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remarks { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
