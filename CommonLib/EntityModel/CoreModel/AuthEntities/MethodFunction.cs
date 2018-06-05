using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 函数要求的权限信息
    /// </summary>
    public class MethodFunction
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 方法全名：命名空间.类名.方法名
        /// </summary>
        public virtual string MethodFullName { get; set; }
        /// <summary>
        /// 权限编码：C010
        /// </summary>
        public virtual string FunctionId { get; set; }
        /// <summary>
        /// R:读；W:写
        /// </summary>
        public virtual string Mode { get; set; }
        /// <summary>
        /// 方法描述
        /// </summary>
        public virtual string Description { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
