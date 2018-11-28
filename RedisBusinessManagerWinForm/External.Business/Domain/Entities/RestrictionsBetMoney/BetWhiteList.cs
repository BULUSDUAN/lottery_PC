using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.RestrictionsBetMoney
{
    public class BetWhiteList
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public virtual int WhiteLlistId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string UserDisplayName { get; set; }
        /// <summary>
        /// 是否启用白名单
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public virtual string ExpansionOne { get; set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public virtual decimal ExpansionTwo { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public virtual DateTime RegisterTime { get; set; }
        /// <summary>
        /// 创建白面单时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否开启合买
        /// </summary>
        public virtual bool IsOpenHeMai { get; set; }
        /// <summary>
        /// 是否开启宝单分享
        /// </summary>
        public virtual bool IsOpenBDFX { get; set; }
        /// <summary>
        /// 是否开启单式上传
        /// </summary>
        public virtual bool IsSingleScheme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string SiteList { get; set; }

    }
}
