using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    /// <summary>
    /// 网站服务项
    /// </summary>
    [CommunicationObject]
    public class UserSiteServiceInfo
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 服务类型
        /// </summary>
        public ServiceType ServiceType { get; set; }
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string ExtendedOne { get; set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public decimal ExtendedTwo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
