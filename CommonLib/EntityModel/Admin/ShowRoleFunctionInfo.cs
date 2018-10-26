using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class ShowRoleFunctionInfo
    {
        /// <summary>
        /// 功能权限编号
        /// </summary>
        public string FunctionId { get; set; }
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

        public int SelectType { get; set; }
    }
}
