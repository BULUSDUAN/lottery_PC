using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.SiteMessage
{
    public class UserIdea
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual string Status { get; set; }
        /// <summary>
        /// 是否匿名用户
        /// </summary>
        public virtual bool IsAnonymous { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public virtual string CreateUserId{ get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public virtual string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 创建者手机
        /// </summary>
        public virtual string CreateUserMoibile { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新者编号
        /// </summary>
        public virtual string UpdateUserId { get; set; }
        /// <summary>
        /// 更新者显示名称
        /// </summary>
        public virtual string UpdateUserDisplayName { get; set; }
        /// <summary>
        /// 页面打开速度
        /// </summary>
        public virtual decimal PageOpenSpeed { get; set; }
        /// <summary>
        /// 界面设计美观
        /// </summary>
        public virtual decimal InterfaceBeautiful { get; set; }
        /// <summary>
        /// 界面设计美观
        /// </summary>
        public virtual decimal ComposingReasonable { get; set; }
        /// <summary>
        /// 操作过程合理
        /// </summary>
        public virtual decimal OperationReasonable { get; set; }
        /// <summary>
        /// 内容传达清晰
        /// </summary>
        public virtual decimal ContentConveyDistinct { get; set; }
        /// <summary>
        /// 管理回复
        /// </summary>
        public virtual string ManageReply { get; set; }
    }
}
