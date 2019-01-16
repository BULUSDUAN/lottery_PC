using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 用户建议
    /// </summary>
    
    public class UserIdeaInfo_Add
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 是否匿名用户
        /// </summary>
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 创建者手机
        /// </summary>
        public string CreateUserMoibile { get; set; }
        /// <summary>
        /// 页面打开速度
        /// </summary>
        public decimal PageOpenSpeed { get; set; }
        /// <summary>
        /// 界面设计美观
        /// </summary>
        public decimal InterfaceBeautiful { get; set; }
        /// <summary>
        /// 界面设计美观
        /// </summary>
        public decimal ComposingReasonable { get; set; }
        /// <summary>
        /// 操作过程合理
        /// </summary>
        public decimal OperationReasonable { get; set; }
        /// <summary>
        /// 内容传达清晰
        /// </summary>
        public decimal ContentConveyDistinct { get; set; }
        /// <summary>
        /// 管理回复
        /// </summary>
        public string ManageReply { get; set; }
    }
    /// <summary>
    /// 用户建议
    /// </summary>
    
    public class UserIdeaInfo_Query
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 是否匿名用户
        /// </summary>
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 创建者手机
        /// </summary>
        public string CreateUserMoibile { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新者编号
        /// </summary>
        public string UpdateUserId { get; set; }
        /// <summary>
        /// 更新者显示名称
        /// </summary>
        public string UpdateUserDisplayName { get; set; }
        /// <summary>
        /// 页面打开速度
        /// </summary>
        public decimal PageOpenSpeed { get; set; }
        /// <summary>
        /// 界面设计美观
        /// </summary>
        public decimal InterfaceBeautiful { get; set; }
        /// <summary>
        /// 界面设计美观
        /// </summary>
        public decimal ComposingReasonable { get; set; }
        /// <summary>
        /// 操作过程合理
        /// </summary>
        public decimal OperationReasonable { get; set; }
        /// <summary>
        /// 内容传达清晰
        /// </summary>
        public decimal ContentConveyDistinct { get; set; }
        /// <summary>
        /// 管理回复
        /// </summary>
        public string ManageReply { get; set; }
    }
    
    public class UserIdeaInfo_QueryCollection
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 公告列表
        /// </summary>
        public IList<UserIdeaInfo_Query> UserIdeaList { get; set; }
    }
}
