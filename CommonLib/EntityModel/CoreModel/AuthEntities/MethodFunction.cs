using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 函数要求的权限信息 C_Auth_MethodFunction_List
    /// </summary>
    [Entity("C_Auth_MethodFunction_List", Type = EntityType.Table)]
    public class MethodFunction
    {
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public  int Id { get; set; }
        /// <summary>
        /// 方法全名：命名空间.类名.方法名
        /// </summary>
        [Field("MethodFullName")]
        public  string MethodFullName { get; set; }
        /// <summary>
        /// 权限编码：C010
        /// </summary>
        [Field("FunctionId")]
        public  string FunctionId { get; set; }
        /// <summary>
        /// R:读；W:写
        /// </summary>
        [Field("Mode")]
        public  string Mode { get; set; }
        /// <summary>
        /// 方法描述
        /// </summary>
        [Field("Description")]
        public  string Description { get; set; }
        [Field("CreateTime")]
        public  DateTime CreateTime { get; set; }
    }
}
