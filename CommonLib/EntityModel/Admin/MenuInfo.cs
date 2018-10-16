using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    /// <summary>
    /// 菜单项
    /// </summary>
    [Serializable]
    public class MenuInfo
    {
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MenuId { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 上级编号
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 链接URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }
    /// <summary>
    /// 菜单集合
    /// </summary>
    [Serializable]
    public class MenuInfoCollection : List<MenuInfo>
    {
    }
   
}
