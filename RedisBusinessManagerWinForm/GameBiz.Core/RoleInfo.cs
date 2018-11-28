using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    /// <summary>
    /// 系统角色信息
    /// </summary>
    [CommunicationObject]
    public class RoleInfo_Query
    {
        public RoleInfo_Query()
        {
            FunctionList = new List<RoleFunctionInfo>();
        }
        /// <summary>
        /// 角色编号
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public RoleType RoleType { get; set; }
        /// <summary>
        /// 是否超级管理员。如果是，在系统中将不判断权限，可以执行左右操作
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 角色包含功能列表
        /// </summary>
        public IList<RoleFunctionInfo> FunctionList { get; set; }
    }
    /// <summary>
    /// 系统角色信息
    /// </summary>
    [CommunicationObject]
    public class RoleInfo_Add
    {
        public RoleInfo_Add()
        {
            FunctionList = new List<RoleFunctionInfo>();
        }
        /// <summary>
        /// 角色编号
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public RoleType RoleType { get; set; }
        /// <summary>
        /// 是否超级管理员。如果是，在系统中将不判断权限，可以执行左右操作
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 角色包含功能列表
        /// </summary>
        public IList<RoleFunctionInfo> FunctionList { get; set; }
    }
    /// <summary>
    /// 系统角色信息
    /// </summary>
    [CommunicationObject]
    public class RoleInfo_Update
    {
        public RoleInfo_Update()
        {
            AddFunctionList = new List<RoleFunctionInfo>();
            ModifyFunctionList = new List<RoleFunctionInfo>();
            RemoveFunctionList = new List<RoleFunctionInfo>();
        }
        /// <summary>
        /// 角色编号
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 要添加的功能列表
        /// </summary>
        public IList<RoleFunctionInfo> AddFunctionList { get; set; }
        /// <summary>
        /// 要修改的功能列表
        /// </summary>
        public IList<RoleFunctionInfo> ModifyFunctionList { get; set; }
        /// <summary>
        /// 要移除的功能列表
        /// </summary>
        public IList<RoleFunctionInfo> RemoveFunctionList { get; set; }
    }
    /// <summary>
    /// 角色权限映射信息
    /// </summary>
    [CommunicationObject]
    public class RoleFunctionInfo
    {
        /// <summary>
        /// 功能权限编号
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// R:读；W:写
        /// </summary>
        public  string Mode { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 上级Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 上级路径
        /// </summary>
        public string ParentPath { get; set; }
    }
    /// <summary>
    /// 系统角色列表
    /// </summary>
    [CommunicationObject]
    public class RoleInfo_QueryCollection : List<RoleInfo_Query>
    {
    }
    /// <summary>
    /// 功能权限
    /// </summary>
    [CommunicationObject]
    public class FunctionInfo
    {
        /// <summary>
        /// 权限编号
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 上级节点Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 节点路径
        /// </summary>
        public string ParentPath { get; set; }
    }
    /// <summary>
    /// 功能权限列表
    /// </summary>
    [CommunicationObject]
    public class FunctionCollection : List<FunctionInfo>
    {
    }
}
