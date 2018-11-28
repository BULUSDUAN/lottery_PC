using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Auth.Domain.Entities
{
    /// <summary>
    /// 功能权限
    /// </summary>
    public class Function
    {
        /// <summary>
        /// 权限编号
        /// </summary>
        public virtual string FunctionId { get; set; }
        /// <summary>
        /// 是否前台的基础功能
        /// </summary>
        public virtual bool IsWebBasic { get; set; }
        /// <summary>
        /// 是否后台的基础功能
        /// </summary>
        public virtual bool IsBackBasic { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// 上级节点Id
        /// </summary>
        public virtual string ParentId { get; set; }
        /// <summary>
        /// 节点路径
        /// </summary>
        public virtual string ParentPath { get; set; }

        public override bool Equals(object obj)
        {
            var target = obj as Function;
            if (target == null)
            {
                return false;
            }
            return FunctionId.Equals(target.FunctionId);
        }
        public override int GetHashCode()
        {
            return FunctionId.GetHashCode();
        }
    }
}
