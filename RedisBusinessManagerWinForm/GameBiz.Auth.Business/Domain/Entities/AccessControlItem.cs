using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace GameBiz.Auth.Domain.Entities
{
    /// <summary>
    /// 访问控制项
    /// </summary>
    public class AccessControlItem
    {
        public virtual Function Function { get; set; }
        public virtual string FunctionId
        {
            get { return Function.FunctionId; }
            set { Function = new Function { FunctionId = value }; }
        }
        /// <summary>
        /// 允许/禁止
        /// </summary>
        public virtual EnableStatus Status { get; set; }
        /// <summary>
        /// R:读；W:写
        /// </summary>
        public virtual string Mode { get; set; }

        public override bool Equals(object obj)
        {
            var target = obj as AccessControlItem;
            if (target == null)
            {
                return false;
            }
            return Function.Equals(target.Function) && Mode.Equals(target.Mode);
        }
        public override int GetHashCode()
        {
            return Function.GetHashCode() + Mode.GetHashCode();
        }
    }
}
